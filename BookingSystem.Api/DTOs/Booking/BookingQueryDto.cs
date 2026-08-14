using BookingSystem.Api.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Api.DTOs.Booking
{
    public class BookingQueryDto
    {
        [FromQuery(Name = "page")]
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        [FromQuery(Name = "resourceId")]
        [Range(1, int.MaxValue)]
        public int? ResourceId { get; set; }

        [FromQuery(Name = "fromDate")]
        public DateTime? FromDate { get; set; }

        [FromQuery(Name = "toDate")]
        public DateTime? ToDate { get; set; }

        [FromQuery(Name = "status")]
        public BookingStatus? Status { get; set; }
    }
}