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
            return await httpClient.GetFromJsonAsync<List<PolicyHolderDto>>("PolicyHolderContoller");
        }

        public async Task<PolicyHolderDto> GetById(int id)
        {
            return await httpClient.GetFromJsonAsync<PolicyHolderDto>($"PolicyHolderContoller/{id}");
        }

        public async Task Add(PolicyHolderDto employee)
        {
            await httpClient.PostAsJsonAsync<PolicyHolderDto>("PolicyHolderContoller", employee);
        }

        public async Task DeleteById(int id)
        {
            await httpClient.DeleteAsync($"PolicyHolderContoller/{id}");
        }

        public async Task Update(PolicyHolderDto employee)
        {
            await httpClient.PutAsJsonAsync<PolicyHolderDto>("PolicyHolderContoller", employee);
        }
    }
}