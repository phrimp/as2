using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DAO
{
    public class NewsArticleDAO
    {
        private NewsSystemContext _dbContext;
        private static NewsArticleDAO instance;

        public NewsArticleDAO()
        {
            _dbContext = new NewsSystemContext();
        }

        public static NewsArticleDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NewsArticleDAO();
                }
                return instance;
            }
        }

        public IEnumerable<NewsArticle> GetAllNewsArticles()
        {
            return _dbContext.NewsArticles
                .Include(a => a.Category)
                .Include(a => a.CreatedBy)
                .Include(a => a.UpdatedBy)
                .Include(a => a.Tags)
                .ToList();
        }

        public NewsArticle GetNewsArticleById(int id)
        {
            return _dbContext.NewsArticles
                .Include(a => a.Category)
                .Include(a => a.CreatedBy)
                .Include(a => a.UpdatedBy)
                .Include(a => a.Tags)
                .FirstOrDefault(a => a.NewsArticleId == id);
        }

        public void AddNewsArticle(NewsArticle article)
        {
            _dbContext.NewsArticles.Add(article);
            _dbContext.SaveChanges();
        }

        public void UpdateNewsArticle(NewsArticle article)
        {
            var existingArticle = _dbContext.NewsArticles.Find(article.NewsArticleId);

            if (existingArticle != null)
            {
                existingArticle.NewsTitle = article.NewsTitle;
                existingArticle.Headline = article.Headline;
                existingArticle.NewsContent = article.NewsContent;
                existingArticle.NewsSource = article.NewsSource;
                existingArticle.CategoryId = article.CategoryId;
                existingArticle.NewsStatus = article.NewsStatus;
                existingArticle.UpdatedById = article.UpdatedById;
                existingArticle.ModifiedDate = article.ModifiedDate;

                _dbContext.SaveChanges();
            }
        }

        public void DeleteNewsArticle(int id)
        {
            var article = _dbContext.NewsArticles.Find(id);
            if (article != null)
            {
                _dbContext.NewsArticles.Remove(article);
                _dbContext.SaveChanges();
            }
        }

    }
}