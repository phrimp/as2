using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DAO
{
    public class CategoryDAO
    {
        private NewsSystemContext _dbContext;
        private static CategoryDAO instance;

        public CategoryDAO()
        {
            _dbContext = new NewsSystemContext();
        }

        public static CategoryDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CategoryDAO();
                }
                return instance;
            }
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _dbContext.Categories.ToList();
        }

        public Category GetCategoryById(int id)
        {
            return _dbContext.Categories.FirstOrDefault(c => c.CategoryId == id);
        }

        public void AddCategory(Category category)
        {
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            var existingCategory = _dbContext.Categories.Find(category.CategoryId);

            if (existingCategory != null)
            {
                _dbContext.Entry(existingCategory).CurrentValues.SetValues(category);
                _dbContext.SaveChanges();
            }
        }

        public void DeleteCategory(int id)
        {
            var category = _dbContext.Categories.Find(id);
            if (category != null)
            {
                _dbContext.Categories.Remove(category);
                _dbContext.SaveChanges();
            }
        }
    }
}