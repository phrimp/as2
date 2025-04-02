using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DAO
{
    public class AccountDAO
    {
        private NewsSystemContext _dbContext;
        private static AccountDAO instance;

        public AccountDAO()
        {
            _dbContext = new NewsSystemContext();
        }

        public static AccountDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AccountDAO();
                }
                return instance;
            }
        }

        public SystemAccount GetAccount(String email, String password)
        {
            var account = _dbContext.SystemAccounts.FirstOrDefault(m => m.AccountEmail == email && m.AccountPassword == password);
            Console.WriteLine($"Account found: {(account == null ? "NULL" : account.AccountEmail)}");
            return account;
        }

        public List<SystemAccount> GetAllAccounts()
        {
            return _dbContext.SystemAccounts.AsNoTracking().ToList();

        }

        public SystemAccount GetAccountById(int id)
        {
            return _dbContext.SystemAccounts.AsNoTracking().FirstOrDefault(a => a.AccountId == id);

        }

        public SystemAccount GetAccountByEmail(string email)
        {
            return _dbContext.SystemAccounts
        .AsNoTracking()
        .FirstOrDefault(a => a.AccountEmail == email);
        }

        public List<SystemAccount> GetAccountsByRole(string role)
        {
            return _dbContext.SystemAccounts
        .AsNoTracking()
        .Where(a => a.AccountRole == role)
        .ToList();
        }

        public void AddAccount(SystemAccount account)
        {
            account.NewsArticleCreatedBies = new List<NewsArticle>();
            account.NewsArticleUpdatedBies = new List<NewsArticle>();

            _dbContext.SystemAccounts.Add(account);
            _dbContext.SaveChanges();
        }

        public void UpdateAccount(SystemAccount account)
        {
            var existingAccount = _dbContext.SystemAccounts.Find(account.AccountId);

            if (existingAccount != null)
            {
                _dbContext.Entry(existingAccount).CurrentValues.SetValues(account);
                _dbContext.SaveChanges();
            }
        }

        public void DeleteAccount(int id)
        {
            var account = _dbContext.SystemAccounts.Find(id);
            if (account != null)
            {
                _dbContext.SystemAccounts.Remove(account);
                _dbContext.SaveChanges();
            }
        }

        public List<SystemAccount> SearchAccounts(string searchTerm)
        {
            return _dbContext.SystemAccounts
                .Where(a => a.AccountName.Contains(searchTerm) ||
                            a.AccountEmail.Contains(searchTerm) ||
                            a.AccountRole.Contains(searchTerm))
                .ToList();
        }

        public bool IsEmailExists(string email)
        {
            return _dbContext.SystemAccounts.Any(a => a.AccountEmail == email);
        }

        public bool IsEmailExists(string email, int excludeId)
        {
            return _dbContext.SystemAccounts.Any(a => a.AccountEmail == email && a.AccountId != excludeId);
        }

        public bool HasCreatedArticles(int accountId)
        {
            return _dbContext.NewsArticles.Any(a => a.CreatedById == accountId);
        }

        public bool HasUpdatedArticles(int accountId)
        {
            return _dbContext.NewsArticles.Any(a => a.UpdatedById == accountId);
        }

        public List<NewsArticle> GetArticlesCreatedByAccount(int accountId)
        {
            return _dbContext.NewsArticles
        .AsNoTracking()
        .Include(a => a.Category)
        .Where(a => a.CreatedById == accountId)
        .OrderByDescending(a => a.CreatedDate)
        .ToList();
        }

        public List<NewsArticle> GetArticlesUpdatedByAccount(int accountId)
        {
            return _dbContext.NewsArticles
        .AsNoTracking()
        .Include(a => a.Category)
        .Where(a => a.UpdatedById == accountId)
        .OrderByDescending(a => a.ModifiedDate)
        .ToList();
        }
    }
}