using BookingSystem.Api.DTOs.Resource;

namespace BookingSystem.Api.Services.Resources
{
    public interface IResourceService
    {
        // Retrieves all resources
        Task<IEnumerable<ResourceDto>> GetResourcesAsync();

        // Retrieves a single resource by id
        Task<ResourceDto?> GetResourceByIdAsync(int id);

        // Creates a new resource
        Task<ResourceDto> CreateResourceAsync(CreateResourceDto dto);

        // Updates an existing resource
        Task<(bool Success, string? Error, int StatusCode)> UpdateResourceAsync(int id, UpdateResourceDto dto);

        // Deletes a resource by id
        Task<(bool Success, string? Error, int StatusCode)> DeleteResourceAsync(int id);
    }
}