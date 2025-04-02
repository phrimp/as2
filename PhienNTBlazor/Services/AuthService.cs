// Services/AuthService.cs
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
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
        private readonly IJSRuntime _jsRuntime;

        public SystemAccount? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;

        public AuthService(
            ProtectedSessionStorage sessionStorage,
            IAccountRepo accountRepo,
            NavigationManager navigationManager,
            IJSRuntime jsRuntime)
        {
            _sessionStorage = sessionStorage;
            _accountRepo = accountRepo;
            _navigationManager = navigationManager;
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var user = _accountRepo.Login(email, password);

                if (user != null)
                {
                    await SaveUserSession(user);
                    return true;
                }

                return false;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
            {
                return false;
            }
        }

        private async Task SaveUserSession(SystemAccount user)
        {
            CurrentUser = user;
            try
            {
                await _sessionStorage.SetAsync("userId", user.AccountId);
                await _sessionStorage.SetAsync("userName", user.AccountName);
                await _sessionStorage.SetAsync("userEmail", user.AccountEmail);
                await _sessionStorage.SetAsync("userRole", user.AccountRole);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
            {
                // Session storage might not be available during prerendering
                // We've already set CurrentUser, so the application can continue
            }
        }

        public async Task LogoutAsync()
        {
            CurrentUser = null;
            try
            {
                await _sessionStorage.DeleteAsync("userId");
                await _sessionStorage.DeleteAsync("userName");
                await _sessionStorage.DeleteAsync("userEmail");
                await _sessionStorage.DeleteAsync("userRole");
                _navigationManager.NavigateTo("/login");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
            {
                // Session storage might not be available during prerendering
                _navigationManager.NavigateTo("/login");
            }
        }

        public async Task<bool> InitializeAuthenticationStateAsync()
        {
            try
            {
                var userId = await _sessionStorage.GetAsync<int>("userId");
                var userName = await _sessionStorage.GetAsync<string>("userName");
                var userEmail = await _sessionStorage.GetAsync<string>("userEmail");
                var userRole = await _sessionStorage.GetAsync<string>("userRole");

                if (userId.Success && userName.Success && userRole.Success)
                {
                    CurrentUser = new SystemAccount
                    {
                        AccountId = userId.Value,
                        AccountName = userName.Value,
                        AccountEmail = userEmail.Value,
                        AccountRole = userRole.Value
                    };
                    return true;
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("JavaScript interop"))
            {
                // Session storage might not be available during prerendering
                // This is expected and not an error - just return false
            }
            catch (Exception)
            {
                // Other errors - the user is not authenticated
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