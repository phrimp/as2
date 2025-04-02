using DAO;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository
{
    public class NewsArticleRepo : INewsArticleRepo
    {
        public IEnumerable<NewsArticle> GetAllNewsArticles() =>
            NewsArticleDAO.Instance.GetAllNewsArticles();

        public NewsArticle GetNewsArticleById(int id) =>
            NewsArticleDAO.Instance.GetNewsArticleById(id);

        public void AddNewsArticle(NewsArticle article) =>
            NewsArticleDAO.Instance.AddNewsArticle(article);

        public void UpdateNewsArticle(NewsArticle article) =>
            NewsArticleDAO.Instance.UpdateNewsArticle(article);

        public void DeleteNewsArticle(int id) =>
            NewsArticleDAO.Instance.DeleteNewsArticle(id);

        public IEnumerable<NewsArticle> GetActiveNewsArticles()
        {
            return NewsArticleDAO.Instance.GetAllNewsArticles()
                .Where(a => a.NewsStatus != "Draft" && a.NewsStatus != "Archived");
        }

        public IEnumerable<NewsArticle> GetNewsArticlesByCategory(int categoryId)
        {
            return NewsArticleDAO.Instance.GetAllNewsArticles()
                .Where(a => a.CategoryId == categoryId);
        }

        public IEnumerable<NewsArticle> GetNewsArticlesByCreator(int creatorId)
        {
            return NewsArticleDAO.Instance.GetAllNewsArticles()
                .Where(a => a.CreatedById == creatorId);
        }

        public IEnumerable<NewsArticle> SearchNewsArticles(string searchTerm)
        {
            return NewsArticleDAO.Instance.GetAllNewsArticles()
                .Where(a => a.NewsTitle.Contains(searchTerm) ||
                           a.Headline.Contains(searchTerm) ||
                           a.NewsContent.Contains(searchTerm));
        }

        public IEnumerable<NewsArticle> GetNewsArticlesByDate(DateTime startDate, DateTime endDate)
        {
            return NewsArticleDAO.Instance.GetAllNewsArticles()
                .Where(a => a.CreatedDate >= startDate && a.CreatedDate <= endDate);
        }

        public void AddTagToNewsArticle(int articleId, int tagId)
        {
            TagDAO.Instance.AddTagToNewsArticle(articleId, tagId);
        }

        public void RemoveTagFromNewsArticle(int articleId, int tagId)
        {
            TagDAO.Instance.RemoveTagFromNewsArticle(articleId, tagId);
        }
    }
}