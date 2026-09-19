using CleanArchitectureTemplate.Domain.Entities;
using CleanArchitectureTemplate.Domain.RepositoryInterfaces;
using CleanArchitectureTemplate.Infrastructure.Context;

namespace CleanArchitectureTemplate.Infrastructure.Repositories;

public sealed class ProductRepository(AppDbContext context)
    : Repository<Product>(context), IProductRepository
{
}
