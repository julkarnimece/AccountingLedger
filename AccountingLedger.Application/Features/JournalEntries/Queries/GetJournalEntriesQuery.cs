using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLedger.Core.Interfaces;
using AutoMapper;
using MediatR;

namespace AccountingLedger.Application.Features.JournalEntries.Queries
{
    public class GetJournalEntriesQuery : IRequest<List<JournalEntryDto>>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class JournalEntryDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<JournalEntryLineViewDto> Lines { get; set; } = new List<JournalEntryLineViewDto>();
        public decimal TotalDebit => Lines.Sum(l => l.Debit);
        public decimal TotalCredit => Lines.Sum(l => l.Credit);
    }


    public class JournalEntryLineViewDto
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }

    public class GetJournalEntriesQueryHandler : IRequestHandler<GetJournalEntriesQuery, List<JournalEntryDto>>
    {
        private readonly IJournalEntryRepository _journalEntryRepository;
        private readonly IMapper _mapper;

        public GetJournalEntriesQueryHandler(IJournalEntryRepository journalEntryRepository, IMapper mapper)
        {
            _journalEntryRepository = journalEntryRepository;
            _mapper = mapper;
        }

        public async Task<List<JournalEntryDto>> Handle(GetJournalEntriesQuery request, CancellationToken cancellationToken)
        {
            var journalEntries = await _journalEntryRepository.GetFilteredAsync(request.StartDate, request.EndDate);
            return _mapper.Map<List<JournalEntryDto>>(journalEntries);
        }
    }


}
