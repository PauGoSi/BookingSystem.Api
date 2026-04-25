using Microsoft.AspNetCore.Mvc;

public class BookingQueryDto
{
    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "pageSize")]
    public int PageSize { get; set; } = 10;

    [FromQuery(Name = "userId")]
    public int? UserId { get; set; }

    [FromQuery(Name = "resourceId")]
    public int? ResourceId { get; set; }

    [FromQuery(Name = "fromDate")]
    public DateTime? FromDate { get; set; }

    [FromQuery(Name = "toDate")]
    public DateTime? ToDate { get; set; }
}