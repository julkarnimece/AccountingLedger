using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLedger.Application.Features.JournalEntries.Queries;
using AccountingLedger.Core.Entities;
using AccountingLedger.Core.Interfaces;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace AccountingLedger.Application.Features.JournalEntries.Commands
{
    public class CreateJournalEntryCommand : IRequest<int>
    {
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;
        public List<JournalEntryLineDto> Lines { get; set; } = new List<JournalEntryLineDto>();
    }

    public class CreateJournalEntryCommandValidator : AbstractValidator<CreateJournalEntryCommand>
    {
        public CreateJournalEntryCommandValidator()
        {
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
            RuleFor(x => x.Lines).NotEmpty().WithMessage("At least one journal entry line is required.");
            RuleFor(x => x.Lines).Must(lines =>
            {
                var totalDebit = lines.Sum(l => l.Debit);
                var totalCredit = lines.Sum(l => l.Credit);
                return totalDebit == totalCredit;
            }).WithMessage("Total Debit must equal Total Credit.");

            RuleForEach(x => x.Lines).ChildRules(line =>
            {
                line.RuleFor(l => l.AccountId).GreaterThan(0).WithMessage("Account ID is required for each line.");
                line.RuleFor(l => l.Debit).GreaterThanOrEqualTo(0).WithMessage("Debit must be non-negative.");
                line.RuleFor(l => l.Credit).GreaterThanOrEqualTo(0).WithMessage("Credit must be non-negative.");
                line.RuleFor(l => l.Debit + l.Credit).GreaterThan(0).WithMessage("Debit or Credit must be greater than zero for each line.");
                line.RuleFor(l => l.Debit).Must((line, debit) => !(debit > 0 && line.Credit > 0)).WithMessage("A line cannot have both a Debit and a Credit value.");
            });
        }
    }


    public class CreateJournalEntryCommandHandler : IRequestHandler<CreateJournalEntryCommand, int>
    {
        private readonly IJournalEntryRepository _journalEntryRepository;
        private readonly IAccountRepository _accountRepository; // To validate account IDs
        private readonly IMapper _mapper;

        public CreateJournalEntryCommandHandler(
            IJournalEntryRepository journalEntryRepository,
            IAccountRepository accountRepository,
            IMapper mapper)
        {
            _journalEntryRepository = journalEntryRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateJournalEntryCommand request, CancellationToken cancellationToken)
        {
            // Validate Account IDs exist
            foreach (var line in request.Lines)
            {
                var account = await _accountRepository.GetByIdAsync(line.AccountId);
                if (account == null)
                {
                    throw new ValidationException($"Account with ID {line.AccountId} not found.");
                }
            }

            var journalEntry = _mapper.Map<JournalEntry>(request);
            foreach (var lineDto in request.Lines)
            {
                journalEntry.JournalEntryLines.Add(new JournalEntryLine
                {
                    AccountId = lineDto.AccountId,
                    Debit = lineDto.Debit,
                    Credit = lineDto.Credit
                });
            }

            await _journalEntryRepository.AddAsync(journalEntry);
            return journalEntry.Id;
        }
    }

    public class JournalEntryProfile : Profile
    {
        public JournalEntryProfile()
        {
            CreateMap<CreateJournalEntryCommand, JournalEntry>();
            CreateMap<JournalEntryLine, JournalEntryLineViewDto>()
            .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account.Name))
            .ForMember(dest => dest.Debit, opt => opt.MapFrom(src => src.Debit))
            .ForMember(dest => dest.Credit, opt => opt.MapFrom(src => src.Credit));

            CreateMap<JournalEntry, JournalEntryDto>()
                .ForMember(dest => dest.Lines, opt => opt.MapFrom(src => src.JournalEntryLines));
        }
    }







    public class JournalEntryLineDto
    {
        public int AccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
