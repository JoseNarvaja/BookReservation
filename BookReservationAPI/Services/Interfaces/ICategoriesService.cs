using BookReservationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace BookReservationAPI.Services.Interfaces
{
    public interface ICategoriesService : IService<Category, int>
    {
        Task UpdateAsync(Category category, int id);
        Task<Category> GetCategoryAsync(int id);
        Task DeleteAsync(int id);
    }
}
