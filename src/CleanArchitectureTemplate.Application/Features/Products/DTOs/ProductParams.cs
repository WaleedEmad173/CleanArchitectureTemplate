using CleanArchitectureTemplate.Application.Common.Pagination;

namespace CleanArchitectureTemplate.Application.Features.Products.DTOs;

public class ProductParams : QueryParams
{
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? InStockOnly { get; set; }
}
