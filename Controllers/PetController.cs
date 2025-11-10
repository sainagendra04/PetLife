using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetLife.Dto;
using PetLife.Serivce;

namespace PetLife.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetController : Controller
    {
        private readonly PetService petService;
        public PetController(PetService _petService)
        {
            petService = _petService;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddPet([FromBody] PetDto pet)
        {
            return await petService.AddNewPet(pet);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePet(Guid id)
        {
            return await petService.DeletePet(id);
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePet(Guid id, [FromBody] Dto.PetDto updatedPet)
        {
            return await petService.UpdatePet(id, updatedPet);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPets()
        {
            return await petService.GetAllPets();

        }
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailablePets()
        {
            return await petService.GetAvailablePets();
        }
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetPetById(Guid id)
        {
            return await petService.GetPetById(id);
        }
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetPetByName(string name)
        {
            return await petService.GetPetByName(name);
        }
        [HttpGet("petsbybreed/{breed}")]
        public async Task<IActionResult> GetPetsByBreed(string breed)
        {
            var pets = await petService.GetPetsByBreed(breed);
            return Ok(pets);
        }
        [HttpGet("petsbyprice")]
        public async Task<IActionResult> GetPetsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var pets = await petService.GetPetsByPriceRange(minPrice, maxPrice);
            return Ok(pets);
        }

    }
}
