@page "/YourPolicies/{PolicyHolderId:int}"
@using Client.Services
@using InsuranceApi.DTOs
@using ClientApp.Services
@inject IPolicyDtoService PolicyService
@inject IInsuredPolicyDtoService InsuredPolicyService
@inject IInsuredDtoService InsuredService
@inject IPolicyHolderDtoService PolicyHolderService
@inject IInsuranceTypeDtoService InsuranceTypeService
@inject NavigationManager Navigation
@inject HttpClient Http
@inject IJSRuntime JsRuntime

<h3 class="text-center text-primary mt-4">Your Policies</h3>

<div class="container mt-4">
    @if (isLoading)
    {
        <div class="d-flex justify-content-center">
            <div class="spinner-border text-primary" role="status">
            </div>
            <span class="sr-only">Loading...</span>
        </div>
    }
    else if (!string.IsNullOrEmpty(errorMessage))
    {
        <div class="alert alert-danger" role="alert">
            @errorMessage
        </div>
    }
    else
    {
        <table class="table table-striped table-bordered ">
            <thead>
                <tr class="table-secondary">
                    <th scope="col">Policy ID</th>
                    <th scope="col">Insured Policy ID</th>
                    <th scope="col">Insurance Type</th>
                    <th scope="col">Start Date</th>
                    <th scope="col">End Date</th>
                    <td scope="col">daysSinceStart</td>
                    <th scope="col">Status</th>
                    <th scope="col">Premium Amount</th>
                    <th scope="col">Days Left for Renewal</th>
                    <th scope="col">Actions</th>
                </tr>
            </thead>
            <tbody>
                @if (insuredPolicies != null && insuredPolicies.Any())
                {
                    var displayedPolicyIds = new HashSet<int>();

                    @foreach (var insuredPolicy in insuredPolicies)
                    {
                        var policy = policies.FirstOrDefault(p => p.PolicyId == insuredPolicy.PolicyId);

                        if (policy != null && !displayedPolicyIds.Contains(policy.PolicyId))
                        {
                            displayedPolicyIds.Add(policy.PolicyId); // Mark this policy as displayed

                            var daysLeft = (policy.EndDate - DateTime.Today).Days;
                            var insuranceType = InsuranceType.FirstOrDefault(it => it.InsuranceId == policy.InsuranceTypeId);
                            var daysSinceStart = (DateTime.Today - policy.StartDate).Days;

                            <tr class="@GetRowClass(policy.EndDate)">
                                <td>@insuredPolicy.PolicyId</td>
                                <td>@insuredPolicy.InsuredPolicyId</td>
                                <td>@insuranceType?.InsuranceType</td>
                                <td>@policy.StartDate.ToShortDateString()</td>
                                <td>@policy.EndDate.ToShortDateString()</td>
                                <td>@daysSinceStart</td>
                                <td>@insuredPolicy.ApprovalStatus</td>
                                <td>@policy.PremiumAmount</td>
                                <td>@(daysLeft > 0 ? $"{daysLeft} days" : "Expired")</td>
                                <td>
                                    <div class="btn-group" role="group">
                                        @if (insuredPolicy.ApprovalStatus == "Pending" || insuredPolicy.ApprovalStatus == "Withdrawal")
                                        {
                                            <button class="btn btn-secondary me-2" disabled>
                                                <i class="fas fa-lock"></i> Renew
                                            </button>
                                            <button class="btn btn-secondary me-2" disabled>
                                                <i class="fas fa-lock"></i> Claim
                                            </button>
                                            <button class="btn btn-secondary me-2" disabled>
                                                <i class="fas fa-lock"></i> Download
                                            </button>
                                        }
                                        else if (policy.EndDate < DateTime.Today)
                                        {
                                            <button class="btn btn-danger me-2" @onclick="() => ConfirmRenewPolicy(policy.PolicyId)">
                                                <i class="fas fa-sync-alt"></i> Renew
                                            </button>
                                        }
                                        else
                                        {
                                            <button class="btn btn-success me-2" disabled>
                                                <i class="fas fa-check-circle"></i> Active
                                            </button>
                                        }

                                        @if (insuredPolicy.ApprovalStatus == "Approved" && insuredPolicy.ApprovalStatus == "Expired")
                                        {
                                            <button class="btn btn-success me-2" @onclick="() => ConfirmClaimPolicy(policy.PolicyId)">
                                                <i class="fas fa-handshake"></i> Claim
                                            </button>
                                            <button class="btn btn-info me-2" @onclick="() => ConfirmDownloadPolicy(policy.PolicyId)">
                                                <i class="fas fa-download"></i> Download
                                            </button>
                                        }

                                        @if (insuredPolicy.ApprovalStatus == "Approved" && daysSinceStart <= 45)
                                        {
                                            <button class="btn btn-warning me-2" @onclick="() => ConfirmWithdrawPolicy(policy.PolicyId)">
                                                <i class="fas fa-times-circle"></i> Withdraw
                                            </button>
                                        }
                                    </div>
                                </td>
                            </tr>
                        }
                    }
                }
                else
                {
                    <tr>
                        <td colspan="9" class="text-center">No policies found.</td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

@code {
    private List<InsuredPolicyDto> insuredPolicies = new List<InsuredPolicyDto>();
    private List<PolicyDto> policies = new List<PolicyDto>();
    private List<InsuranceTypeDto> InsuranceType = new List<InsuranceTypeDto>();
    private bool isLoading = true;
    private string errorMessage = string.Empty;

    [Parameter]
    public int PolicyHolderId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadPolicies();
    }

    private async Task LoadPolicies()
    {
        try
        {
            var policyHolder = await PolicyHolderService.GetById(PolicyHolderId);
            if (policyHolder == null)
            {
                errorMessage = "Policy holder not found.";
                return;
            }

            var insureds = (await InsuredService.GetAll())
                            .Where(i => i.PolicyHolderId == policyHolder.PolicyHolderId)
                            .ToList();

            if (insureds.Any())
            {
                var insuredIds = insureds.Select(i => i.InsuredId).Distinct().ToList();
                insuredPolicies = (await InsuredPolicyService.GetAll())
                                    .Where(ip => insuredIds.Contains(ip.InsuredId))
                                    .ToList();

                if (insuredPolicies.Any())
                {
                    var policyIds = insuredPolicies.Select(ip => ip.PolicyId).Distinct().ToList();
                    policies = (await PolicyService.GetAll())
                                .Where(p => policyIds.Contains(p.PolicyId))
                                .ToList();

                    InsuranceType = await InsuranceTypeService.GetAll();

                    foreach (var insuredPolicy in insuredPolicies)
                    {
                        var policy = policies.FirstOrDefault(p => p.PolicyId == insuredPolicy.PolicyId);
                        if (policy != null)
                        {
                            var daysLeft = (policy.EndDate - DateTime.Today).Days;
                            insuredPolicy.RenewalStatus = daysLeft > 0 ? $"{daysLeft} days left" : "Expired";

                            // Save the updated RenewalStatus to the database
                            await InsuredPolicyService.Update(insuredPolicy);
                        }
                    }
                }
            }
            else
            {
                errorMessage = "No insured individuals found for this policy holder.";
            }
        }
        catch (Exception)
        {
            errorMessage = $"NO Policies To Display !!!";
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task WithdrawPolicy(int policyId)
    {
        try
        {
            var insuredPolicy = insuredPolicies.FirstOrDefault(ip => ip.PolicyId == policyId);
            if (insuredPolicy != null)
            {
                insuredPolicy.ApprovalStatus = "Withdrawal"; // Change status to pending
                await InsuredPolicyService.Update(insuredPolicy); // Update the insured policy
                await LoadPolicies(); // Reload policies to reflect changes
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to withdraw policy: {ex.Message}";
        }
    }

    private async Task RenewPolicy(int policyId)
    {
        try
        {
            var policyToRenew = policies.FirstOrDefault(p => p.PolicyId == policyId);
            if (policyToRenew != null)
            {
                // policyToRenew.PolicyId = policyId;
                // policyToRenew.EndDate = DateTime.Today.AddYears(1);
                // policyToRenew.StartDate = DateTime.Now;
                // await PolicyService.Update(policyToRenew);
                await LoadPolicies();
                Navigation.NavigateTo($"/BuyPlan/{policyToRenew}");
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to renew policy: {ex.Message}";
        }
    }

    private void ClaimPolicy(int policyId)
    {
        var insuredPolicy = insuredPolicies.FirstOrDefault(ip => ip.PolicyId == policyId);
        if (insuredPolicy == null || insuredPolicy.ApprovalStatus != "Approved")
        {
            errorMessage = "Policy cannot be claimed as it is not approved.";
        }
        else
        {
            Navigation.NavigateTo($"/Claim/{PolicyHolderId}");
        }
    }

    private async Task DownloadPolicy(int policyId)
    {
        try
        {
            var insuredPolicy = insuredPolicies.FirstOrDefault(ip => ip.PolicyId == policyId);
            if (insuredPolicy == null || insuredPolicy.ApprovalStatus != "Approved")
            {
                errorMessage = "Policy cannot be downloaded as it is not approved.";
                return;
            }

            var response = await Http.GetAsync($"pdf/GeneratePolicyPdf?policyHolderId={PolicyHolderId}&policyId={policyId}");
            response.EnsureSuccessStatusCode();
            var fileName = $"Policy_{policyId}.pdf";
            var fileBytes = await response.Content.ReadAsByteArrayAsync();
            await JsRuntime.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(fileBytes));
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to download policy: {ex.Message}";
        }
    }

    private string GetRowClass(DateTime endDate)
    {
        var daysLeft = (endDate - DateTime.Today).Days;
        if (daysLeft <= 0)
        {
            return "table-danger";
        }
        else if (daysLeft <= 30)
        {
            return "table-warning";
        }
        else
        {
            return "table-success"; 
        }
    }

    // conform methods


    private async Task ConfirmWithdrawPolicy(int policyId)
    {
        var confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to withdraw this policy?");
        if (confirmed)
        {
            await WithdrawPolicy(policyId);
        }
    }

    private async Task ConfirmRenewPolicy(int policyId)
    {
        var confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to renew this policy?");
        if (confirmed)
        {
            await RenewPolicy(policyId);
        }
    }

    private async Task ConfirmClaimPolicy(int policyId)
    {
        var confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to claim this policy?");
        if (confirmed)
        {
            ClaimPolicy(policyId);
        }
    }

    private async Task ConfirmDownloadPolicy(int policyId)
    {
        var confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to download this policy?");
        if (confirmed)
        {
            await DownloadPolicy(policyId);
        }
    }

}
