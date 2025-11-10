using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetLife.Dto;
using PetLife.Interfaces;
using PetLife.Models;
using PetLife.Models.DBContext;

namespace PetLife.Serivce
{
    public class PetFoodService : Controller, IPetFood
    {
        private readonly PetLifeDBContext context;
        public PetFoodService(PetLifeDBContext _context)
        {
            context = _context;
        }
        public async Task<IActionResult> AddNewPetFood(PetFoodDto petFoodDto)
        {
            var petFood = await context.PetFoods.FirstOrDefaultAsync(food => food.Name == petFoodDto.Name);
            if (petFood != null)
            {
                return BadRequest("Pet food with the same name already exists.");
            }
            var petFoodDetail = new PetFood
            {
                Name = petFoodDto.Name,
                Description = petFoodDto.Description,
                PetCategoryId = petFoodDto.CategoryId,
                Price = petFoodDto.Price
            };
            context.PetFoods.Add(petFoodDetail);
            await context.SaveChangesAsync();
            return Ok(petFoodDetail);

        }

        public async Task<IActionResult> DeletePetFood(Guid id)
        {
            var petFood = await context.PetFoods.FindAsync(id);
            if (petFood == null)
            {
                return BadRequest("PetFood with the Id doesn't exist");
            }
            context.PetFoods.Remove(petFood);
            await context.SaveChangesAsync();
            return Ok(petFood);            
        }

        public async Task<IActionResult> GetAllPetFood()
        {
            var petsFood = await context.PetFoods
                .Include(c => c.Category)
                .Select( c=>
                    new PetFoodListDto
                    {
                        PetFoodId = c.PetFoodId,
                        Name = c.Name,
                        Description = c.Description,
                        Price = c.Price,
                        Category = new Category
                        {
                            CategoryId = c.Category.PetCategoryId,
                            CategoryName = c.Category.CategoryName
                        }
                    }
                )
                .ToListAsync();
            return Ok(petsFood);
        }

        public async Task<IActionResult> GetPetFoodById(Guid id)
        {
            var petFood = await context.PetFoods
                .Include(c => c.Category)
                .Select(
                    c => new PetFoodListDto
                    {
                        PetFoodId = c.PetFoodId,
                        Name = c.Name,
                        Description = c.Description,
                        Price = c.Price,
                        Category = new Category
                        {
                            CategoryId = c.Category.PetCategoryId,
                            CategoryName = c.Category.CategoryName
                        }
                    }
                )
                .FirstOrDefaultAsync(food => food.PetFoodId == id);
            if (petFood == null)
                return NotFound();
            return Ok(petFood);
        }

        public async Task<IActionResult> GetPetFoodByName(string name)
        {
            var petFood = await context.PetFoods
                .Include(c => c.Category)
                .Select(
                    c => new PetFoodListDto
                    {
                        PetFoodId = c.PetFoodId,
                        Name = c.Name,
                        Description = c.Description,
                        Price = c.Price,
                        Category = new Category
                        {
                            CategoryId = c.Category.PetCategoryId,
                            CategoryName = c.Category.CategoryName
                        }
                    }
                )
                .FirstOrDefaultAsync(food => food.Name == name);
            if (petFood == null)
                return NotFound();
            return Ok(petFood);
        }

        public async Task<IActionResult> UpdatePetFood(Guid id, PetFoodDto petFoodDto)
        {
            var petFood = await context.PetFoods.FindAsync(id);
            if (petFood == null)
            {
                return BadRequest("The food with Id doesn't exists");
            }
            petFood.Name = petFoodDto.Name;
            petFood.Description = petFoodDto.Description;
            petFood.PetCategoryId = petFoodDto.CategoryId;
            petFood.Price = petFoodDto.Price;
            context.SaveChanges();
            return Ok(petFood);
        }
    }
}
