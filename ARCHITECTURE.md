# Architecture

## 1. Clean Architecture Layers

Each layer depends only inward. The Domain has zero dependencies on any other layer or external package.

```mermaid
flowchart TD
    API["<b>Products.API</b>\nControllers · Middleware\nProgram.cs · ExceptionMiddleware\nSwagger · Serilog"]
    APP["<b>Products.Application</b>\nCQRS Handlers · FluentValidation Validators\nDTOs · Pipeline Behaviours\nIJwtTokenService · DependencyInjection"]
    DOM["<b>Products.Domain</b>\nProduct Entity · BaseEntity\nIUnitOfWork · IProductRepository\nInvalidProductException · ProductNotFoundException\nDuplicateProductException"]
    INF["<b>Products.Infrastructure</b>\nUnitOfWork · ProductRepository\nInMemoryAppContext · JwtTokenService\nJwtSettings · DependencyInjection"]

    API -->|depends on| APP
    API -->|depends on| INF
    APP -->|depends on| DOM
    INF -->|depends on| APP
    INF -->|depends on| DOM

    style DOM fill:#1e3a5f,color:#fff,stroke:#4a90d9
    style APP fill:#1a4731,color:#fff,stroke:#4caf50
    style INF fill:#3d2b00,color:#fff,stroke:#f5a623
    style API fill:#3b1f3b,color:#fff,stroke:#c678dd
```

**Dependency rule (strictly enforced):**

| Layer | May reference |
|---|---|
| Domain | Nothing (pure C#) |
| Application | Domain only |
| Infrastructure | Domain + Application |
| API | Application + Infrastructure |

The Domain and Application layers are **framework-free**. Swapping InMemoryAppContext for Entity Framework Core, or replacing RabbitMQ with Azure Service Bus, requires changes only in Infrastructure.

---

## 2. Unit of Work Pattern — CreateProduct Request Flow

```mermaid
sequenceDiagram
    autonumber
    participant Client
    participant Controller as ProductsController
    participant MediatR
    participant Handler as CreateProductCommandHandler
    participant UoW as IUnitOfWork
    participant Repo as IProductRepository
    participant Store as InMemoryAppContext

    Client->>Controller: POST /api/products {name, price, colour}
    Controller->>MediatR: Send(CreateProductCommand)
    MediatR->>Handler: Handle(command, ct)

    Note over Handler: Product.Create(...) — domain validation
    Handler->>UoW: Products.Add(product)
    UoW->>Repo: Add(product) — stages change
    Repo->>Store: ConcurrentDictionary.TryAdd(id, product)

    Handler->>UoW: await CommitAsync(ct)
    Note over UoW,Store: Single transaction boundary<br/>EF Core: SaveChangesAsync()
    UoW-->>Handler: count (success)

    Handler-->>MediatR: ProductDto
    MediatR-->>Controller: ProductDto
    Controller-->>Client: 201 Created + Location header
```

### Why Unit of Work?

The Unit of Work enforces a **single atomic transaction boundary** per HTTP request.

- `IProductRepository.Add/Update/Remove` are **synchronous void** — they stage changes but never persist.
- `IUnitOfWork.CommitAsync()` is the **only** method that writes to the store. This mirrors `DbContext.SaveChangesAsync()` exactly.
- If `CommitAsync` fails (e.g. a constraint violation), `RollbackAsync()` is called — no partial state is ever committed.
- Because `UnitOfWork` is registered as **Scoped**, every HTTP request gets its own unit of work instance, preventing cross-request contamination.
- When InMemoryAppContext is swapped for EF Core's `DbContext`, only `CommitAsync` and `RollbackAsync` change — every handler, test, and interface stays identical.

---

## 3. Microservices Event-Driven Architecture

```mermaid
flowchart LR
    FE["⚛ React Frontend"]
    GW["API Gateway\nAWS API Gateway / NGINX"]
    AUTH["Shared Auth Service\nJWT Issuer / JWKS"]
    PS["Products Service\n<i>this API</i>"]
    OS["Orders Service\n.NET"]
    PAY["Payments Service\n.NET"]
    NS["Notification Service"]
    MB["Message Broker\nRabbitMQ / Azure Service Bus"]

    FE -->|"HTTPS / REST"| GW

    GW -->|"route /products/*"| PS
    GW -->|"route /orders/*"| OS

    PS -->|"validate JWT"| AUTH
    OS -->|"validate JWT"| AUTH
    PAY -->|"validate JWT"| AUTH

    OS -->|"publish OrderCreated"| MB
    PS -->|"publish ProductCreated\nStockUpdated"| MB

    MB -->|"subscribe OrderCreated"| PAY
    MB -->|"subscribe OrderCreated\nPaymentProcessed"| NS
    PAY -->|"publish PaymentProcessed\nPaymentFailed"| MB

    style FE fill:#20232a,color:#61dafb,stroke:#61dafb
    style GW fill:#1a1a2e,color:#e94560,stroke:#e94560
    style AUTH fill:#0f3460,color:#fff,stroke:#4a90d9
    style PS fill:#1a4731,color:#fff,stroke:#4caf50
    style OS fill:#1a4731,color:#fff,stroke:#4caf50
    style PAY fill:#1a4731,color:#fff,stroke:#4caf50
    style NS fill:#2d2d2d,color:#fff,stroke:#aaa
    style MB fill:#3d2b00,color:#fff,stroke:#f5a623
```

### Why CQRS?

**Command Query Responsibility Segregation** separates the write model (Commands) from the read model (Queries).

- **Write side** (`CreateProductCommand`) goes through FluentValidation, domain creation, and UoW commit — the full pipeline.
- **Read side** (`GetAllProductsQuery`, `GetProductsByColourQuery`) bypasses all mutation logic and returns DTOs directly, with minimal overhead.
- As the system grows, read and write models can be scaled independently. The read side can be served from a read replica, a cache, or a dedicated read-optimised store (e.g. Elasticsearch) without touching write handlers.
- MediatR's `IPipelineBehavior<,>` allows cross-cutting concerns (logging, validation) to be applied selectively — for example, `ValidationBehaviour` only fires when a registered `IValidator<TRequest>` exists, which is only the case for Commands.

### Why Event-Driven?

Events decouple services so they can evolve, fail, and scale independently.

- **Decoupling** — the Products Service publishes `ProductCreated` and moves on. It has no knowledge of Payments, Notifications, or any downstream consumer. Adding a new consumer never changes the producer.
- **Resilience** — the Message Broker (RabbitMQ / Azure Service Bus) acts as a durable buffer. If the Payments Service is temporarily down, `OrderCreated` events queue up and are processed when it recovers — no data loss, no cascade failures.
- **Async processing** — slow operations (sending emails, processing payments, updating analytics) happen out-of-band. The Orders Service returns a `202 Accepted` immediately; the client is notified via the Notification Service once processing completes.
- **Audit trail** — every event is a first-class fact that can be stored, replayed, and used for debugging or event sourcing in future.
