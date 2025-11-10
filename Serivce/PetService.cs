using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetLife.Dto;
using PetLife.Interfaces;
using PetLife.Models;
using PetLife.Models.DBContext;

namespace PetLife.Serivce
{
    public class PetService : Controller, IPet
    {
        private readonly PetLifeDBContext context;
        public PetService(PetLifeDBContext _context)
        {
            context = _context;
        }
        public async Task<IActionResult> AddNewPet(PetDto petDto)
        {
            var petWithName = await context.Pets.FirstOrDefaultAsync(pet => pet.Name == petDto.Name);
            if(petWithName != null)
            {
                return BadRequest("Pet with the same name already exists.");
            }
            var pet = new Pet
            {
                Name = petDto.Name,
                Breed = petDto.Breed,
                PetCategoryId = petDto.PetCategoryId,
                Age = petDto.Age,
                Price = petDto.Price,
                Available = petDto.Available
            };
            context.Pets.Add(pet);
            await context.SaveChangesAsync();
            return Ok(petDto);

        }
        
        public async Task<IActionResult> DeletePet(Guid id)
        {
            var pet = await context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound("Pet not found.");
            }
            context.Pets.Remove(pet);
            await context.SaveChangesAsync();
            return Ok("Pet deleted successfully.");
        }
        public async Task<IActionResult> UpdatePet(Guid id, PetDto petDto)
        {
            var pet = await context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound("Pet not found.");
            }
            pet.Name = petDto.Name;
            pet.Breed = petDto.Breed;
            pet.PetCategoryId = petDto.PetCategoryId;
            pet.Age = petDto.Age;
            pet.Price = petDto.Price;
            pet.Available = petDto.Available;

            await context.SaveChangesAsync();
            return Ok(pet);
        }
        public async Task<IActionResult> GetAllPets()
        {
            var pets = await context.Pets
                .Include(c => c.Category)
                .Select(c => new PetDetailsDto
                {
                    Name = c.Name,
                    Breed = c.Breed,
                    Age = c.Age,
                    Price = c.Price,
                    Available = c.Available,
                    Category = new PetCategoryDto
                    {
                        CategoryId = c.Category.PetCategoryId,
                        CategoryName = c.Category.CategoryName
                    }
                }).ToListAsync();
            if (pets == null || !pets.Any())
                return Ok(new { Message = "No Pets Available" });
            return Ok(pets);
        }

        public async Task<IActionResult> GetAvailablePets()
        {
            var pets = await context.Pets
               .Include(c => c.Category)
               .Where(x => x.Available == true)
               .Select(c => new PetDetailsDto
               {
                   Name = c.Name,
                   Breed = c.Breed,
                   Age = c.Age,
                   Price = c.Price,
                   Available = c.Available,
                   Category = new PetCategoryDto
                   {
                       CategoryId = c.Category.PetCategoryId,
                       CategoryName = c.Category.CategoryName
                   }
               }).ToListAsync();
            if (pets == null || !pets.Any())
                return Ok(new { Message = "No Pets are Available" });
            return Ok(pets);
        }
        public async Task<IEnumerable<PetDetailsDto>> GetPetsByBreed(string breed)
        {
            var pets = await context.Pets
               .Include(c => c.Category)
               .Where(x => x.Breed == breed)
               .Select(c => new PetDetailsDto
               {
                   Name = c.Name,
                   Breed = c.Breed,
                   Age = c.Age,
                   Price = c.Price,
                   Available = c.Available,
                   Category = new PetCategoryDto
                   {
                       CategoryId = c.Category.PetCategoryId,
                       CategoryName = c.Category.CategoryName
                   }
               }).ToListAsync();
            if (pets == null)
            {
                throw new NotImplementedException();
            }
            return pets;
        }

        public async Task<IActionResult> GetPetById(Guid id)
        {
            var pet = await context.Pets
               .Where(x => x.PetId == id)
               .Include(c => c.Category)
               .Select(c => new PetDetailsDto
               {
                   Name = c.Name,
                   Breed = c.Breed,
                   Age = c.Age,
                   Price = c.Price,
                   Available = c.Available,
                   Category = new PetCategoryDto
                   {
                       CategoryId = c.Category.PetCategoryId,
                       CategoryName = c.Category.CategoryName
                   }
               }).FirstOrDefaultAsync();
            if (pet == null)
                throw new NotImplementedException();
            return Ok(pet);
        }

        public async Task<IActionResult> GetPetByName(string name)
        {
            var pet = await context.Pets
               .Where(x => x.Name == name)
               .Include(c => c.Category)
               .Select(c => new PetDetailsDto
               {
                   Name = c.Name,
                   Breed = c.Breed,
                   Age = c.Age,
                   Price = c.Price,
                   Available = c.Available,
                   Category = new PetCategoryDto
                   {
                       CategoryId = c.Category.PetCategoryId,
                       CategoryName = c.Category.CategoryName
                   }
               }).FirstOrDefaultAsync();
            if (pet == null)
            {
                throw new NotImplementedException();
            }
            return Ok(pet);
        }

        public async Task<IEnumerable<PetDetailsDto>> GetPetsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            var pets = await context.Pets
               .Include(c => c.Category)
               .Where(x => x.Price>= minPrice && x.Price <= maxPrice && x.Available == true)
               .Select(c => new PetDetailsDto
               {
                   Name = c.Name,
                   Breed = c.Breed,
                   Age = c.Age,
                   Price = c.Price,
                   Available = c.Available,
                   Category = new PetCategoryDto
                   {
                       CategoryId = c.Category.PetCategoryId,
                       CategoryName = c.Category.CategoryName
                   }
               }).ToListAsync();
            if (pets == null)
            {
                throw new NotImplementedException();
            }
            return pets;
        }
    }
}
