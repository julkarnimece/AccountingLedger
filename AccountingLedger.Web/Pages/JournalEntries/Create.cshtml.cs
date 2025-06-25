using AccountingLedger.Application.DTOs;
using AccountingLedger.Application.Features.Accounts.Queries;
using AccountingLedger.Application.Features.JournalEntries.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AccountingLedger.Web.Pages.JournalEntries
{
    public class CreateModel : BasePageModel
    {

        [BindProperty]
        public CreateJournalEntryCommand Command { get; set; } = new CreateJournalEntryCommand
        {
            Date = DateTime.Today,
            Lines = new List<JournalEntryLineDto> { new JournalEntryLineDto() } 
        };

        public List<AccountDto> Accounts { get; set; } = new List<AccountDto>(); 

        public async Task<IActionResult> OnGetAsync()
        {
            Accounts = await Mediator.Send(new GetAccountsQuery()); 
                                                                    
            if (!Command.Lines.Any())
            {
                Command.Lines.Add(new JournalEntryLineDto());
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Accounts = await Mediator.Send(new GetAccountsQuery());
            if (!ModelState.IsValid)
            {
                ViewData["Accounts"] = Accounts;
                return Page();
            }

            try
            {
                var journalEntryId = await Mediator.Send(Command);
                TempData["SuccessMessage"] = $"Journal Entry created successfully with ID: {journalEntryId}.";
                return RedirectToPage("/JournalEntries/Index");
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    if (error.PropertyName.StartsWith("Lines["))
                    {
                        ModelState.AddModelError($"Command.{error.PropertyName}", error.ErrorMessage);
                    }
                    else
                    {
                        ModelState.AddModelError($"Command.{error.PropertyName}", error.ErrorMessage);
                    }
                }
                ViewData["Accounts"] = Accounts;
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                ViewData["Accounts"] = Accounts;
                return Page();
            }
        }
    }
}

