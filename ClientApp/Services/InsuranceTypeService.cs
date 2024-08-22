using System.Net.Http.Json;

using InsuranceApi.DTOs;

namespace ClientApp.Services
{

    public interface IInsuranceTypeDtoService
    {
        Task Add(InsuranceTypeDto employee);
        Task DeleteById(int id);
        Task<List<InsuranceTypeDto>> GetAll();
        Task<InsuranceTypeDto> GetById(int id);
        Task Update(InsuranceTypeDto employee);
    }

    public class InsuranceTypeDtoService : IInsuranceTypeDtoService
    {
        private readonly HttpClient httpClient;
        public InsuranceTypeDtoService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<InsuranceTypeDto>> GetAll()
        {
            return await httpClient.GetFromJsonAsync<List<InsuranceTypeDto>>("InsuranceTypeContoller");
        }

        public async Task<InsuranceTypeDto> GetById(int id)
        {
            return await httpClient.GetFromJsonAsync<InsuranceTypeDto>($"InsuranceTypeContoller/{id}");
        }

        public async Task Add(InsuranceTypeDto employee)
        {
            await httpClient.PostAsJsonAsync<InsuranceTypeDto>("InsuranceTypeContoller", employee);
        }

        public async Task DeleteById(int id)
        {
            await httpClient.DeleteAsync($"InsuranceTypeContoller/{id}");
        }

        public async Task Update(InsuranceTypeDto employee)
        {
            await httpClient.PutAsJsonAsync<InsuranceTypeDto>("InsuranceTypeContoller", employee);
        }
    }
}