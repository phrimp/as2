﻿using Models;
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
            return _dbContext.SystemAccounts.FirstOrDefault(m => m.AccountEmail == email && m.AccountPassword == password);
        }

        public List<SystemAccount> GetAllAccounts()
        {
            return _dbContext.SystemAccounts.ToList();
        }

        public SystemAccount GetAccountById(int id)
        {
            return _dbContext.SystemAccounts.Find(id);
        }

        public SystemAccount GetAccountByEmail(string email)
        {
            return _dbContext.SystemAccounts.FirstOrDefault(a => a.AccountEmail == email);
        }

        public List<SystemAccount> GetAccountsByRole(string role)
        {
            return _dbContext.SystemAccounts.Where(a => a.AccountRole == role).ToList();
        }

        public void AddAccount(SystemAccount account)
        {
            _dbContext.SystemAccounts.Add(account);
            _dbContext.SaveChanges();
        }

        public void UpdateAccount(SystemAccount account)
        {
            _dbContext.Entry(account).State = EntityState.Modified;
            _dbContext.SaveChanges();
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
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .Where(a => a.CreatedById == accountId)
                .OrderByDescending(a => a.CreatedDate)
                .ToList();
        }

        public List<NewsArticle> GetArticlesUpdatedByAccount(int accountId)
        {
            return _dbContext.NewsArticles
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .Where(a => a.UpdatedById == accountId)
                .OrderByDescending(a => a.ModifiedDate)
                .ToList();
        }
    }
}