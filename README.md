@page "/PlansDetailAfter/{HolderId:int}/{Id:int}"

@using ClientApp.Services
@using InsuranceApi.DTOs
@inject IPolicyDtoService PolicyService
@inject IInsuredPolicyDtoService InsuredPolicyService
@inject IInsuranceTypeDtoService InsuranceService
@inject IInsuredDtoService service
@inject NavigationManager nav
@inject IJSRuntime js

<div class="card mt-4">
    <div class="card-header">
        Insurance Plan
    </div>
    <div class="card-body">
        <EditForm Model="@premiumModel" OnValidSubmit="@HandleValidSubmit">
            <DataAnnotationsValidator />
            <ValidationSummary />

            <!-- Number of Members Input -->
            <div class="form-group">
                <label for="numMembers">Number of Members: </label>
                <InputNumber id="numMembers" class="form-control" @bind-Value="premiumModel.NumberOfMembers" />
                <ValidationMessage For="@(() => premiumModel.NumberOfMembers)" />
            </div>

            <!-- Coverage Type Input -->
            <div class="form-group">
                <label for="coverageType">Coverage Type: </label>
                <InputSelect id="coverageType" class="form-control" @bind-Value="premiumModel.CoverageType">
                    <option value="">Select Coverage Type</option>
                    <option value="Bronze">Bronze 3 Lakh</option>
                    <option value="Silver">Silver 6 Lakh</option>
                    <option value="Gold">Gold 9 Lakh</option>
                </InputSelect>
                <ValidationMessage For="@(() => premiumModel.CoverageType)" />
            </div>

            <!-- Insured Details Form -->
            @if (insuredDetailsList != null && insuredDetailsList.Count > 0)
            {
                if (premiumModel.NumberOfMembers > premiumModel.CoverageSize)
                {
                    // Handle validation error
                    js.InvokeVoidAsync("alert", "Number Of Members Exceeding the coverage size.");
                }
                else
                {
                    @for (int i = 0; i < premiumModel.NumberOfMembers; i++)
                    {
                        var insured = insuredDetailsList.ElementAtOrDefault(i);
                        var insuredPolicy = insuinsuredPolicyData.ElementAtOrDefault(i);
                        if (insured != null && insuredPolicy != null)
                        {
                            <div class="form-group">
                                <label for="@($"insuredName_{i}")">Insured Name:</label>
                                <InputText id="@($"insuredName_{i}")" class="form-control" @bind-Value="insured.Name" />
                                <ValidationMessage For="@(() => insured.Name)" />

                                <label for="@($"dob_{i}")">Date of Birth: </label>
                                <InputDate id="@($"dob_{i}")" class="form-control" @bind-Value="insured.Dob" />
                                <ValidationMessage For="@(() => insured.Dob)" />

                                <label for="@($"gender_{i}")">Gender: </label>
                                <InputSelect id="@($"gender_{i}")" class="form-control" @bind-Value="insured.Gender">
                                    <option value="">Select Gender</option>
                                    <option value="Male">Male</option>
                                    <option value="Female">Female</option>
                                </InputSelect>
                                <ValidationMessage For="@(() => insured.Gender)" />

                                <!-- Health Factors Checklist -->
                                <div class="form-group">
                                    <label>Health Factors: </label>
                                    <div>
                                        @foreach (var healthFactor in healthFactors)
                                        {
                                            <InputCheckbox @bind-Value="healthFactor.IsSelected" /> @healthFactor.Name
                                        }
                                    </div>
                                </div>
                            </div>
                            <hr />
                        }
                    }
                }
            }

            <!-- Submit Buttons -->
            <button type="submit" class="btn btn-primary"  @onclick="() => PoliCyUpdate(InsuranceType?.InsuranceId ?? 0)">Check Premium</button>

            <button type="button" class="btn btn-success" @onclick="() => BuyPlan(InsuranceType?.InsuranceId ?? 0)">Buy Plan</button>
        </EditForm>
    </div>
</div>

@if (calculatePremium.HasValue)
{
    <div class="alert alert-info mt-3">
        Estimated Premium: @calculatePremium.Value.ToString("C")
    </div>
}

@code {
    [Parameter] public int Id { get; set; }
    [Parameter] public int HolderId { get; set; }
    private PremiumModel premiumModel = new PremiumModel();
    private InsuranceTypeDto? InsuranceType;
    private List<InsuredDto> insuredDetailsList = new List<InsuredDto>();
    private List<InsuredPolicyDto> insuinsuredPolicyData = new List<InsuredPolicyDto>();
    private PolicyDto policy = new PolicyDto();
    private List<HealthFactor> healthFactors = new List<HealthFactor>();
    private decimal? calculatePremium;

    protected override async Task OnInitializedAsync()
    {
        InsuranceType = await InsuranceService.GetById(Id);
        if (InsuranceType != null)
        {
            premiumModel.BaseRate = InsuranceType.BaseRate ?? 0m;
            premiumModel.CoverageSize = InsuranceType.CoverageSize ?? 0;
            UpdateInsuredDetails();
            InitializeHealthFactors();
        }
    }

    private void InitializeHealthFactors()
    {
        healthFactors = new List<HealthFactor>
        {
            new HealthFactor { Name = "Heart Disease", Value = 0.02m },
            new HealthFactor { Name = "Asthma", Value = 0.02m },
            new HealthFactor { Name = "Diabetes", Value = 0.02m },
            new HealthFactor { Name = "Cholesterol", Value = 0.02m },
            new HealthFactor { Name = "Blood Pressure", Value = 0.02m },
            new HealthFactor { Name = "Cancer", Value = 0.02m },
            new HealthFactor { Name = "None", Value = 0.00m }
        };


    }
    private async Task PoliCyUpdate(int insuranceId)
    {
        foreach (var insured in insuredDetailsList)
        {
            // Ensure the InsuredDto details are set correctly
            insured.PolicyHolderId = HolderId; // Ensure this is set correctly
            insured.RegistrationDate = DateTime.Now; // Set registration date
            await service.Add(insured); // Save to the Insured table
        }


            if (insuranceId > 0)
            {
                policy = new PolicyDto
                    {
                        PolicyNumber = GeneratePolicyNumber(HolderId, insuranceId),
                        InsuranceTypeId = insuranceId,
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddYears(1),
                        PremiumAmount = CalculatePremium()
                    };

                await PolicyService.Add(policy);
            }
       
       
    }

    private void UpdateInsuredDetails()
    {
        insuredDetailsList.Clear();
        insuinsuredPolicyData.Clear();

        for (int i = 0; i < premiumModel.CoverageSize; i++)
        {
            insuredDetailsList.Add(new InsuredDto());
            insuinsuredPolicyData.Add(new InsuredPolicyDto());
        }
    }

    private async Task HandleValidSubmit()
    {
        calculatePremium = CalculatePremium();
    }

    private async Task BuyPlan(int insuranceId)
    {
        // Save the policy details
    

        // Process each insured and create associated InsuredPolicyDto entries
        foreach (var insured in insuredDetailsList)
        {

            // Create and save InsuredPolicyDto
            var insuredPolicy = new InsuredPolicyDto()
                {
                    PolicyId = policy.PolicyId, // Ensure the PolicyId is set correctly
                    InsuredId = insured.InsuredId, // Use the saved InsuredID
                    ApprovalDate = DateTime.Now,
                    ApprovalStatus = "Pending", // or any default status
                    RenewalStatus = "Not Renewed",
                    AdminId = 2 // Set appropriately
                };

            // Add the insuredPolicy to the InsuredPolicy table
            await InsuredPolicyService.Add(insuredPolicy);
        }

        // Navigate to another page or perform any other action after saving
        nav.NavigateTo($"/BuyPlan/{HolderId}/{insuranceId}/{calculatePremium}");
    }



    private decimal CalculatePremium()
    {
        decimal coverageAmount = premiumModel.CoverageType switch
        {
            "Bronze" => 300000m,
            "Silver" => 600000m,
            "Gold" => 900000m,
            _ => 0m
        };

        decimal healthFactorSum = healthFactors.Where(h => h.IsSelected).Sum(h => h.Value);
        decimal premium = (((premiumModel.BaseRate + coverageAmount) / 100) + premiumModel.BaseRate + (healthFactorSum * premiumModel.BaseRate)) * premiumModel.NumberOfMembers;

        return premium;
    }

    private string GeneratePolicyNumber(int insuredId, int insuranceId)
    {
        return $"{insuredId:D3}{insuranceId:D3}";
    }

    public class PremiumModel
    {
        public decimal BaseRate { get; set; }
        public int NumberOfMembers { get; set; }
        public string CoverageType { get; set; } = string.Empty;
        public int CoverageSize { get; set; }
    }

    public class HealthFactor
    {
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public bool IsSelected { get; set; }
    }
}
