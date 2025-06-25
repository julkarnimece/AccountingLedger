using AccountingLedger.Application.Features.JournalEntries.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AccountingLedger.Web.Pages.JournalEntries
{
    public class IndexModel : BasePageModel
    {

        public List<JournalEntryDto> JournalEntries { get; set; } = new List<JournalEntryDto>();  

        [BindProperty(SupportsGet = true)]
        public JournalEntryFilter Filter { get; set; } = new JournalEntryFilter();



        public async Task OnGetAsync()
        {
            var query = new GetJournalEntriesQuery
            {
                StartDate = Filter.StartDate,
                EndDate = Filter.EndDate,
            };

            JournalEntries = await Mediator.Send(query);

        }
    }


    public class JournalEntryFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
