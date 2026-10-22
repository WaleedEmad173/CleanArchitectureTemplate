using CleanArchitectureTemplate.Application.Common.Pagination;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitectureTemplate.Application.Common.Querying;

public class QueryParams : PaginationParams
{
    public const int MaxSearchLength = 100;

    [StringLength(MaxSearchLength)]
    public string? Search { get; init; }

    [StringLength(128)]
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
}
