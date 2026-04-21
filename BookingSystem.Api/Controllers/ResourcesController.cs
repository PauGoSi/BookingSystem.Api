using BookingSystem.Api.Data;
using BookingSystem.Api.DTOs.Resource;
using BookingSystem.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ResourcesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetResources()
        {
            var resources = await _context.Resources
                .Select(r => new ResourceDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Location = r.Location,
                    Capacity = r.Capacity,
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return Ok(resources);
        }

        [HttpPost]
        public async Task<ActionResult<ResourceDto>> CreateResource(CreateResourceDto dto)
        {
            var resource = new Resource
            {
                Name = dto.Name,
                Description = dto.Description,
                Location = dto.Location,
                Capacity = dto.Capacity,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();

            var result = new ResourceDto
            {
                Id = resource.Id,
                Name = resource.Name,
                Description = resource.Description,
                Location = resource.Location,
                Capacity = resource.Capacity,
                IsActive = resource.IsActive
            };

            return CreatedAtAction(nameof(GetResources), new { id = result.Id }, result);
        }
    }
}