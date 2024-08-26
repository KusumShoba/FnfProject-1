@page "/BuyPlan/{HolderId:int}/{insuranceId:int}/{policyId:int}/{calculatePremium:decimal}"


<h3>BuyPlan</h3>

@using ClientApp.Services
@using InsuranceApi.DTOs
@* @inject IPolicyDtoService PolicyService  *@
@inject IPaymentDtoService PaymentService
@inject IInsuranceTypeDtoService InsuranceTypeService
@inject IInsuredPolicyDtoService InsuredPolicyService
@inject IPolicyHolderDtoService PolicyHolderService
@inject NavigationManager Navigation

<h3>Buy Plan</h3>

<div class="card">
    <div class="card-body">
        <h5 class="card-title">Policy Details</h5>

        <div class="form-group">
            <label>Policy Id:</label>
            <input type="text" class="form-control" value="@policyId" readonly />
        </div>

        <div class="form-group">
            <label>Insurance Type:</label>
            <input type="text" class="form-control" value="@insurancetype.InsuranceType" readonly />
        </div>

        <div class="form-group">
            <label>Policy Holder Name:</label>
            <input type="text" class="form-control" value="@policyHolder?.Name" readonly />
        </div>

        <div class="form-group">
            <label>Start Date:</label>
            <input type="text" class="form-control" value="@policy.StartDate.ToShortDateString()" readonly />
        </div>

        <div class="form-group">
            <label>End Date:</label>
            <input type="text" class="form-control" value="@policy.EndDate.ToShortDateString()" readonly />
        </div>

        <div class="form-group">
            <label>Premium Amount:</label>
            <input type="text" class="form-control" value="@calculatePremium" readonly />
        </div>

        <button class="btn btn-primary" @onclick="BuyPolicy">Buy</button>
    </div>
</div>

@code {
    [Parameter] public int HolderId { get; set; }
    [Parameter] public int insuranceId { get; set; }
    [Parameter] public decimal calculatePremium { get; set; }
    [Parameter] public int policyId { get; set; }
    private PolicyDto policy;
    private PolicyHolderDto policyHolder;
    private List<InsuredDto> insuredDetailsList = new List<InsuredDto>();
    private InsuredPolicyDto insuredPolicy;
    private InsuranceTypeDto insurancetype;

    protected override async Task OnInitializedAsync()
    {

        policyHolder = await PolicyHolderService.GetById(policyId);
        // Retrieve all insureds related to the HolderId from the previous page
        insurancetype = await InsuranceTypeService.GetById(insuranceId);

        // Iterate through each insured to create an entry in the InsuredPolicy table

        foreach (var insured in insuredDetailsList)
        {
            var insuredPolicy = new InsuredPolicyDto
                {
                   // PolicyId = PolicyId,
                    InsuredId = insured.InsuredId,
                    ApprovalDate = DateTime.Now,
                    ApprovalStatus = "Pending", // or any default status
                    RenewalStatus = "Not Renewed",
                    AdminId = 2 // Set appropriately
                };

            // Add the insuredPolicy to the InsuredPolicy table
            await InsuredPolicyService.Add(insuredPolicy);
        }
     
    }

   
    private async Task BuyPolicy()
    {
        // Save Policy
        // await PolicyService.Add(policy);

        // Save Payment
        var payment = new PaymentDto
            {
                InsuredPolicyId = policy.PolicyId, // Assuming it's set by the database or service after adding
                PolicyHolderId = HolderId,
                PaymentAmount = policy.PremiumAmount,
                PaymentDate = DateTime.Now
            };

        await PaymentService.Add(payment);

        // Redirect or show confirmation
        Navigation.NavigateTo("/Confirmation");
    }
}
