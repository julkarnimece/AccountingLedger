using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLedger.Core.Entities;
using AccountingLedger.Core.Enums;
using AccountingLedger.Infrastructure.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountingLedger.Application.Features.TrialBalance.Queries
{
    public class GetTrialBalanceQuery : IRequest<List<TrialBalanceEntryDto>>    
    {
    }


    public class GetTrialBalanceQueryHandler : IRequestHandler<GetTrialBalanceQuery, List<TrialBalanceEntryDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetTrialBalanceQueryHandler(ApplicationDbContext context)
        {
                _context = context;
        }
        public async Task<List<TrialBalanceEntryDto>> Handle(GetTrialBalanceQuery request, CancellationToken cancellationToken)
        {

            var trialBalanceResults = await _context.Set<TrialBalanceRawResult>() 
                .FromSqlRaw("EXEC dbo.sp_GetTrialBalance")
                .ToListAsync(cancellationToken);

            var trialBalanceDtos = new List<TrialBalanceEntryDto>();
            foreach (var rawResult in trialBalanceResults)
            {
                var dto = new TrialBalanceEntryDto
                {
                    AccountId = rawResult.AccountId,
                    AccountName = rawResult.AccountName,
                    DebitBalance = rawResult.TotalDebit,
                    CreditBalance = rawResult.TotalCredit,
                    AccountType = (AccountType)rawResult.AccountType,
                };

               
                trialBalanceDtos.Add(dto);
            }

            foreach (var entry in trialBalanceDtos)
            {
                var netBalance = entry.DebitBalance - entry.CreditBalance;
                if (netBalance > 0)
                {
                    entry.DebitBalance = netBalance;
                    entry.CreditBalance = 0;
                }
                else
                {
                    entry.CreditBalance = Math.Abs(netBalance);
                    entry.DebitBalance = 0;
                }
            }

            return trialBalanceDtos;
        }

        
    }


    

    public class TrialBalanceEntryDto
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public AccountType AccountType { get; set; }
        public decimal DebitBalance { get; set; }
        public decimal CreditBalance { get; set; }
    }





}
