using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLedger.Application.DTOs;
using AccountingLedger.Core.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountingLedger.Application.Features.Accounts.Queries
{
    public class GetAccountsQuery : IRequest<List<AccountDto>>  
    {
    }

    public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, List<AccountDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        public GetAccountsQueryHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }
        public async Task<List<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        {
            var accounts = await _accountRepository.GetAllAsync();
            return _mapper.Map<List<AccountDto>>(accounts);
        }
    }
}
