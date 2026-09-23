using CleanArchitectureTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CleanArchitectureTemplate.Infrastructure.Context;

public static class ModelBuilderExtensions
{
    public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var propertyMethodInfo = typeof(EF).GetMethod("Property")!.MakeGenericMethod(typeof(bool));
                var isDeletedProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant("IsDeleted"));
                var compareExpression = Expression.MakeBinary(ExpressionType.Equal, isDeletedProperty, Expression.Constant(false));
                var lambda = Expression.Lambda(compareExpression, parameter);
                
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
