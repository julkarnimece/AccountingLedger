using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLedger.Core.Entities
{
    public class JournalEntryLine
    {
        public int Id { get; set; }
        public int JournalEntryId { get; set; }
        public JournalEntry JournalEntry { get; set; } = default!;

        public int AccountId { get; set; }
        public Account Account { get; set; } = default!;

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
