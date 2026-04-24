using BookingSystem.Api.Data;
using BookingSystem.Api.DTOs.Resource;
using BookingSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Api.Services.Resources
{
    public class ResourceService : IResourceService
    {
        private readonly AppDbContext _context;

        // Injects database context for resource data access
        public ResourceService(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all resources as DTOs
        public async Task<IEnumerable<ResourceDto>> GetResourcesAsync()
        {
            return await _context.Resources
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
        }

        // Retrieves a single resource by id
        public async Task<ResourceDto?> GetResourceByIdAsync(int id)
        {
            return await _context.Resources
                .Where(r => r.Id == id)
                .Select(r => new ResourceDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Location = r.Location,
                    Capacity = r.Capacity,
                    IsActive = r.IsActive
                })
                .FirstOrDefaultAsync();
        }

        // Creates a new resource and returns it as DTO
        public async Task<ResourceDto> CreateResourceAsync(CreateResourceDto dto)
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

            return new ResourceDto
            {
                Id = resource.Id,
                Name = resource.Name,
                Description = resource.Description,
                Location = resource.Location,
                Capacity = resource.Capacity,
                IsActive = resource.IsActive
            };
        }

        // Updates an existing resource
        public async Task<(bool Success, string? Error, int StatusCode)> UpdateResourceAsync(int id, UpdateResourceDto dto)
        {
            var resource = await _context.Resources.FindAsync(id);

            if (resource == null)
            {
                return (false, "Resource not found.", 404);
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return (false, "Resource name is required.", 400);
            }

            if (dto.Capacity <= 0)
            {
                return (false, "Capacity must be greater than zero.", 400);
            }

            resource.Name = dto.Name;
            resource.Description = dto.Description;
            resource.Location = dto.Location;
            resource.Capacity = dto.Capacity;
            resource.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return (true, null, 204);
        }

        // Deletes a resource by id
        public async Task<(bool Success, string? Error, int StatusCode)> DeleteResourceAsync(int id)
        {
            var resource = await _context.Resources.FindAsync(id);

            if (resource == null)
            {
                return (false, "Resource not found.", 404);
            }

            var hasBookings = await _context.Bookings.AnyAsync(b => b.ResourceId == id);

            if (hasBookings)
            {
                return (false, "Resource cannot be deleted because it has bookings.", 409);
            }

            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();

            return (true, null, 204);
        }
    }
}