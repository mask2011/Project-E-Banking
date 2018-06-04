using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Project_E_Banking
{
    enum TransactionType
    {
        Deposit,
        Transfer,
        Withdraw
    }

    class InternalBankAccounts
    {
        public string LoggedUser { get; set; }
        public string AccountHolder { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }

        public InternalBankAccounts(string loggedUser, string accountHolder, decimal amount, DateTime transactionDate, TransactionType transactionType)
        {
            LoggedUser = loggedUser;
            AccountHolder = accountHolder;
            Amount = amount;
            TransactionDate = transactionDate;
            TransactionType = transactionType;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("From" + "   | ").Append("Transaction Type | ")
                .Append("    " + "To" + "    | ").Append("   " + "Amount" +"     |")
                .AppendLine("\t    " + "\tDate" + "\t\t   |")
                .Append(AccountHolder).Append("\t      " + $"{TransactionType}" + "      ")
                .Append("   " + LoggedUser + "  ").Append("    " + $"{Amount.ToString("C", new CultureInfo("el-Gr"))}" + "     ")
                .Append("  " + $"{TransactionDate.ToString("yyyy-MM- dd HH:mm:ss.FFF")}")
                .AppendLine();

            return sb.ToString();
        }
    }
}
