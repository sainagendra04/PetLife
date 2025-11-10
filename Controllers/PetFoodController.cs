using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetLife.Dto;
using PetLife.Serivce;

namespace PetLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetFoodController : ControllerBase
    {
        private readonly PetFoodService petFoodService;
        public PetFoodController(PetFoodService _service)
        {
            petFoodService = _service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPetsFood()
        {
            return await petFoodService.GetAllPetFood();
        }
        [Authorize(Roles = "admin")]
        [HttpPost("add")]
        public async Task<IActionResult> AddNewPetFood([FromBody] PetFoodDto petFood)
        {
            return await petFoodService.AddNewPetFood(petFood);
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePetFood(Guid id)
        {
            return await petFoodService.DeletePetFood(id);
        }
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetPetFoodById(Guid id)
        {
            return await petFoodService.GetPetFoodById(id);
        }
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetPetFoodByName(string name)
        {
            return await petFoodService.GetPetFoodByName(name);
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePetFood(Guid id, [FromBody] PetFoodDto updatedPetFood)
        {
            return await petFoodService.UpdatePetFood(id, updatedPetFood);
        }

    }
}
