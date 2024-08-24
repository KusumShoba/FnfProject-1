using System.Net.Http;
using System.Net.Http.Json;
using InsuranceApi.DTOs;
namespace ClientApp.Services
{
    public class AuthService
    {
        private bool _isAuthenticated;
        private readonly HttpClient httpClient;
        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            private set => _isAuthenticated = value;
        }

        public async Task LoginAsync(string email, string password)
        {
            // Simulate a login process and set IsAuthenticated to true if successful
            // You should replace this with actual authentication logic
            var response = await httpClient.PostAsJsonAsync("PolicyHolder/authenticate", new { Email = email, Password = password });
            if (response.IsSuccessStatusCode)
            {
                IsAuthenticated = true;
                // Store authentication token or user details as needed
            }
        }

        public void Logout()
        {
            IsAuthenticated = false;
            // Clear any stored authentication token or user details
        }
    }
}
