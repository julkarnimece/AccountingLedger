using AccountingLedger.Application.DTOs;
using AccountingLedger.Application.Features.Accounts.Commands;
using AccountingLedger.Application.Features.Accounts.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AccountingLedger.Web.Pages.Accounts
{
    public class IndexModel : BasePageModel
    {
        [BindProperty]
        public CreateAccountCommand Account { get; set; } = new CreateAccountCommand();

        public List<AccountDto> Accounts { get; set; } = new List<AccountDto>();

        public async Task OnGetAsync()
        {
            Accounts = await Mediator.Send(new GetAccountsQuery());
        }

        public async Task<IActionResult> OnPostCreateAccountAsync()
        {
            if (!ModelState.IsValid)
            {
                Accounts = await Mediator.Send(new GetAccountsQuery()); 
                return Page();
            }

            try
            {
                var accountId = await Mediator.Send(Account);
                TempData["SuccessMessage"] = $"Account '{Account.Name}' created successfully with ID: {accountId}.";
                return RedirectToPage("/Accounts/Index"); 
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError($"Account.{error.PropertyName}", error.ErrorMessage);
                }
                Accounts = await Mediator.Send(new GetAccountsQuery()); 
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                Accounts = await Mediator.Send(new GetAccountsQuery()); 
                return Page();
            }
        }


    }
}
