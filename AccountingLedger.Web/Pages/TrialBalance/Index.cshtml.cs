using AccountingLedger.Application.Features.TrialBalance.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AccountingLedger.Web.Pages.TrialBalance
{
    public class IndexModel : BasePageModel
    {
        public List<TrialBalanceEntryDto> TrialBalanceEntries { get; set; } = new List<TrialBalanceEntryDto>();

        public async Task OnGetAsync()
        {
            TrialBalanceEntries = await Mediator.Send(new GetTrialBalanceQuery());
        }
        
    }
}
