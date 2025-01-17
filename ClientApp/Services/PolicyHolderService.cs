﻿using System.Net.Http;
using System.Net.Http.Json;
using InsuranceApi.DTOs;

namespace ClientApp.Services
{
    public interface IPolicyHolderDtoService
    {
        Task Add(PolicyHolderDto employee);
        Task DeleteById(int id);
        Task<List<PolicyHolderDto>> GetAll();
        Task<PolicyHolderDto> GetById(int id);
        Task Update(PolicyHolderDto employee);

        Task<string> LoginAsync(LoginDto loginModel);

    }

    public class PolicyHolderDtoService : IPolicyHolderDtoService
    {
        private readonly HttpClient httpClient;
        public PolicyHolderDtoService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<PolicyHolderDto>> GetAll()
        {
            return await httpClient.GetFromJsonAsync<List<PolicyHolderDto>>("PolicyHolder");
        }

        public async Task<PolicyHolderDto> GetById(int id)
        {
            return await httpClient.GetFromJsonAsync<PolicyHolderDto>($"PolicyHolder/{id}");
        }

        public async Task Add(PolicyHolderDto employee)
        {
            await httpClient.PostAsJsonAsync<PolicyHolderDto>("PolicyHolder", employee);
        }

        public async Task DeleteById(int id)
        {
            await httpClient.DeleteAsync($"PolicyHolder/{id}");
        }

        public async Task Update(PolicyHolderDto employee)
        {
            await httpClient.PutAsJsonAsync<PolicyHolderDto>("PolicyHolder", employee);
        }
        public async Task<string> LoginAsync(LoginDto loginModel)
        {
            var response = await httpClient.PostAsJsonAsync("PolicyHolder/authenticate", loginModel);

            if (response.IsSuccessStatusCode)
            {
                // Assuming the response contains a token or other session management data
                var result = await response.Content.ReadAsStringAsync();
                return result; // Return the token or relevant data
            }

            return null; // Login failed
        }

      
    }
}