using Microsoft.AspNetCore.Http;
using Models;
using Repository;
using System.Text.Json;
namespace PhienNTBlazor.Services
{
    public class AuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepo _accountRepo;
        public SystemAccount? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;

        // Add an event for authentication state changes
        public event EventHandler? AuthenticationStateChanged;

        public AuthService(
            IHttpContextAccessor httpContextAccessor,
            IAccountRepo accountRepo)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountRepo = accountRepo;
            InitializeUserFromSession();
        }

        private void InitializeUserFromSession()
        {
            try
            {
                var userJson = _httpContextAccessor.HttpContext?.Session.GetString("CurrentUser");
                if (!string.IsNullOrEmpty(userJson))
                {
                    CurrentUser = JsonSerializer.Deserialize<SystemAccount>(userJson);
                }
            }
            catch
            {
                // Ignore errors during initialization
                CurrentUser = null;
            }
        }

        private void NotifyAuthenticationStateChanged()
        {
            AuthenticationStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool Login(string email, string password)
        {
            var user = _accountRepo.Login(email, password);
            if (user != null)
            {
                CurrentUser = user;
                // Save user to session
                var userJson = JsonSerializer.Serialize(user);
                _httpContextAccessor.HttpContext?.Session.SetString("CurrentUser", userJson);
                NotifyAuthenticationStateChanged();
                return true;
            }
            return false;
        }

        public void Logout()
        {
            CurrentUser = null;
            if (_httpContextAccessor.HttpContext?.Session != null)
            {
                _httpContextAccessor.HttpContext.Session.Clear(); 
            }
            NotifyAuthenticationStateChanged();
        }

        public bool InitializeAuthenticationState()
        {
            InitializeUserFromSession();
            return IsAuthenticated;
        }

        public bool IsInRole(string role)
        {
            return IsAuthenticated && CurrentUser?.AccountRole == role;
        }
    }
}