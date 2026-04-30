using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace SharedPagination;

public class PaginatedList<T> : IPaginatedList where T : class
{
    [JsonPropertyName("items")]
    public IList<T> Items { get; set; } = [];

    [JsonPropertyName("totalItemCount")]
    public int TotalItemCount { get; set; }

    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage => PageNumber > 1;

    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedList()
    {
    }

    public PaginatedList(List<T> items, int totalItemCount, int pageNumber, int pageSize)
    {
        TotalItemCount = totalItemCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalItemCount / (double)pageSize);

        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var totalItemCount = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, totalItemCount, pageNumber, pageSize);
    }
}