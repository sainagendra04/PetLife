using Microsoft.AspNetCore.Mvc;
using PetLife.Dto;
using PetLife.Models;

namespace PetLife.Interfaces
{
    public interface IPet
    {
        Task<IActionResult> GetAllPets();
        Task<IActionResult> GetPetById(Guid id);
        Task<IActionResult> GetAvailablePets();
        Task<IEnumerable<PetDetailsDto>> GetPetsByPriceRange(decimal minPrice, decimal maxPrice);
        Task<IActionResult> GetPetByName(string name);
        Task<IEnumerable<PetDetailsDto>> GetPetsByBreed(string breed);
        Task<IActionResult> AddNewPet(PetDto pet);
        Task<IActionResult> DeletePet(Guid id);
        Task<IActionResult> UpdatePet(Guid id, PetDto updatedPet);
    }
}
