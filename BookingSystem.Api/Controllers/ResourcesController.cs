using BookingSystem.Api.DTOs.Resource;
using BookingSystem.Api.Services.Resources;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/resources")]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceService _resourceService;

        // Injects resource service into controller
        public ResourcesController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        // Retrieves all resources
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetResources()
        {
            var resources = await _resourceService.GetResourcesAsync();
            return Ok(resources);
        }

        // Retrieves a single resource by id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResourceDto>> GetResourceById([FromRoute] int id)
        {
            var resource = await _resourceService.GetResourceByIdAsync(id);

            if (resource == null)
            {
                return NotFound();
            }

            return Ok(resource);
        }

        // Creates a new resource
        [HttpPost]
        public async Task<ActionResult<ResourceDto>> CreateResource(CreateResourceDto dto)
        {
            var result = await _resourceService.CreateResourceAsync(dto);

            return CreatedAtAction(
                nameof(GetResourceById),
                new { id = result.Id },
                result
            );
        }

        // Updates an existing resource
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateResource([FromRoute] int id, UpdateResourceDto dto)
        {
            var result = await _resourceService.UpdateResourceAsync(id, dto);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return NoContent();
        }

        // Deletes a resource
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteResource([FromRoute] int id)
        {
            var result = await _resourceService.DeleteResourceAsync(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { error = result.Error });
            }

            return NoContent();
        }
    }
}