using Products.Domain.Entities;

namespace Products.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    void Add(User user);
}
