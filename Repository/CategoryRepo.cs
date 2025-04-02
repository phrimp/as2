using DAO;
using Models;
using System;
using System.Collections.Generic;

namespace Repository
{
    public class CategoryRepo : ICategoryRepo
    {
        public IEnumerable<Category> GetAllCategories() => CategoryDAO.Instance.GetAllCategories();

        public Category GetCategoryById(int id) => CategoryDAO.Instance.GetCategoryById(id);

        public void CreateCategory(Category category) => CategoryDAO.Instance.AddCategory(category);

        public void UpdateCategory(Category category) => CategoryDAO.Instance.UpdateCategory(category);

        public void DeleteCategory(int id) => CategoryDAO.Instance.DeleteCategory(id);

        public IEnumerable<Category> GetActiveCategories()
        {
            return CategoryDAO.Instance.GetAllCategories().Where(c => c.IsActive == true);
        }

        public IEnumerable<Category> SearchCategories(string searchTerm)
        {
            return CategoryDAO.Instance.GetAllCategories()
                .Where(c => c.CategoryName.Contains(searchTerm) ||
                           (c.CategoryDesciption != null && c.CategoryDesciption.Contains(searchTerm)));
        }

        public bool IsCategoryUsedInArticles(int categoryId)
        {
            var newsArticles = new NewsArticleDAO().GetAllNewsArticles();
            return newsArticles.Any(a => a.CategoryId == categoryId);
        }
    }
}