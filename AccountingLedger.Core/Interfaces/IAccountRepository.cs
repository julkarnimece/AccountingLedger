using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingLedger.Core.Entities;

namespace AccountingLedger.Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> AddAsync(Account account);
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);

    }
}
