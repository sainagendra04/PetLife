using Microsoft.AspNetCore.Mvc;
using PetLife.Dto;

namespace PetLife.Interfaces
{
    public interface IPetFood
    {
        public Task<IActionResult> AddNewPetFood(PetFoodDto petFoodDto);
        public Task<IActionResult> UpdatePetFood(Guid id, PetFoodDto petFoodDto);
        public Task<IActionResult> DeletePetFood(Guid id);
        public Task<IActionResult> GetAllPetFood();
        public Task<IActionResult> GetPetFoodById(Guid id);
        public Task<IActionResult> GetPetFoodByName(string name);
    }
}
