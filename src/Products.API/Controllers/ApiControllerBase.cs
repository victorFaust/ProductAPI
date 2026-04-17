using Microsoft.AspNetCore.Mvc;

namespace Products.API.Controllers;

[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase { }
