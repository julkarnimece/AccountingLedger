using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLedger.Application.DTOs;
using AccountingLedger.Core.Entities;
using AccountingLedger.Core.Enums;
using AccountingLedger.Core.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace AccountingLedger.Application.Features.Accounts.Commands
{
    public class CreateAccountCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty;

        public AccountType Type { get; set; }
    }


    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Account name is required.")
                .MaximumLength(100).WithMessage("Account name cannot exceed 100 characters.");
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid account type.");
        }



    }

    public class CraeteAccountCommandHandler : IRequestHandler<CreateAccountCommand, int>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public CraeteAccountCommandHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository ;
            _mapper = mapper ;
        }
        public async Task<int> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = _mapper.Map<Account>(request);
            await _accountRepository.AddAsync(account);
            return account.Id;  
        }
    }


    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<CreateAccountCommand, Account>();
            CreateMap<Account, AccountDto>(); 
        }
    }



}
