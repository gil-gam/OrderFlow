namespace OrderFlow.Application.DTOs;

public sealed record PaginatedList<T>(
    List<T> Items,
    int PageIndex,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}