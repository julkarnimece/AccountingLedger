using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountingLedger.Core.Entities
{
    public class TrialBalanceRawResult
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public int AccountType { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
    }
}
