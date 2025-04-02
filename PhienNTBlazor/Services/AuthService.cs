// Services/AuthService.cs
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Models;
using Repository;
using System.Threading.Tasks;

namespace PhienNTBlazor.Services
{
    public class AuthService
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly IAccountRepo _accountRepo;
        private readonly NavigationManager _navigationManager;

        public SystemAccount? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;

        public AuthService(
            ProtectedSessionStorage sessionStorage,
            IAccountRepo accountRepo,
            NavigationManager navigationManager)
        {
            _sessionStorage = sessionStorage;
            _accountRepo = accountRepo;
            _navigationManager = navigationManager;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = _accountRepo.Login(email, password);

            if (user != null)
            {
                await SaveUserSession(user);
                return true;
            }

            return false;
        }

        private async Task SaveUserSession(SystemAccount user)
        {
            CurrentUser = user;
            await _sessionStorage.SetAsync("userId", user.AccountId);
            await _sessionStorage.SetAsync("userName", user.AccountName);
            await _sessionStorage.SetAsync("userRole", user.AccountRole);
        }

        public async Task LogoutAsync()
        {
            CurrentUser = null;
            await _sessionStorage.DeleteAsync("userId");
            await _sessionStorage.DeleteAsync("userName");
            await _sessionStorage.DeleteAsync("userRole");
            _navigationManager.NavigateTo("/login");
        }

        public async Task<bool> InitializeAuthenticationStateAsync()
        {
            try
            {
                var userId = await _sessionStorage.GetAsync<int>("userId");
                var userName = await _sessionStorage.GetAsync<string>("userName");
                var userRole = await _sessionStorage.GetAsync<string>("userRole");

                if (userId.Success && userName.Success && userRole.Success)
                {
                    CurrentUser = new SystemAccount
                    {
                        AccountId = userId.Value,
                        AccountName = userName.Value,
                        AccountRole = userRole.Value
                    };
                    return true;
                }
            }
            catch
            {
                // Session storage might not be available
                CurrentUser = null;
            }

            return false;
        }

        public bool IsInRole(string role)
        {
            return IsAuthenticated && CurrentUser.AccountRole == role;
        }
    }
}