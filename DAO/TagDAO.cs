using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DAO
{
    public class TagDAO
    {
        private NewsSystemContext _dbContext;
        private static TagDAO instance;

        public TagDAO()
        {
            _dbContext = new NewsSystemContext();
        }

        public static TagDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TagDAO();
                }
                return instance;
            }
        }

        public List<Tag> GetAllTags()
        {
            return _dbContext.Tags.ToList();
        }

        public Tag GetTagById(int id)
        {
            return _dbContext.Tags.Find(id);
        }

        public Tag GetTagByName(string name)
        {
            return _dbContext.Tags.FirstOrDefault(t => t.TagName == name);
        }

        public List<Tag> SearchTags(string searchTerm)
        {
            return _dbContext.Tags
                .Where(t => t.TagName.Contains(searchTerm) ||
                            (t.Note != null && t.Note.Contains(searchTerm)))
                .ToList();
        }

        public void AddTag(Tag tag)
        {
            _dbContext.Tags.Add(tag);
            _dbContext.SaveChanges();
        }

        public void UpdateTag(Tag tag)
        {
            _dbContext.Entry(tag).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void DeleteTag(int id)
        {
            var tag = _dbContext.Tags.Find(id);
            if (tag != null)
            {
                _dbContext.Tags.Remove(tag);
                _dbContext.SaveChanges();
            }
        }

        public List<Tag> GetTagsByArticleId(int articleId)
        {
            var article = _dbContext.NewsArticles
                .Include(a => a.Tags)
                .FirstOrDefault(a => a.NewsArticleId == articleId);

            return article?.Tags?.ToList() ?? new List<Tag>();
        }

        public bool IsTagUsedInArticles(int tagId)
        {
            var tag = _dbContext.Tags
                .Include(t => t.NewsArticles)
                .FirstOrDefault(t => t.TagId == tagId);

            return tag?.NewsArticles?.Any() ?? false;
        }

        public void AddTagToNewsArticle(int articleId, int tagId)
        {
            var article = _dbContext.NewsArticles
                .Include(a => a.Tags)
                .FirstOrDefault(a => a.NewsArticleId == articleId);

            var tag = _dbContext.Tags.Find(tagId);

            if (article != null && tag != null && !article.Tags.Any(t => t.TagId == tagId))
            {
                article.Tags.Add(tag);
                _dbContext.SaveChanges();
            }
        }

        public void RemoveTagFromNewsArticle(int articleId, int tagId)
        {
            var article = _dbContext.NewsArticles
                .Include(a => a.Tags)
                .FirstOrDefault(a => a.NewsArticleId == articleId);

            if (article != null)
            {
                var tagToRemove = article.Tags.FirstOrDefault(t => t.TagId == tagId);
                if (tagToRemove != null)
                {
                    article.Tags.Remove(tagToRemove);
                    _dbContext.SaveChanges();
                }
            }
        }
    }
}