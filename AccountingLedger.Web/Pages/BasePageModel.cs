using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AccountingLedger.Web.Pages
{
    public class BasePageModel : PageModel
    {
        private IMediator? _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();


    }
}
