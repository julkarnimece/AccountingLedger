using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLedger.Core.Enums;

namespace AccountingLedger.Core.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public AccountType Type { get; set; }
    }
}
