using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_E_Banking
{
    class ApplicationMenus : DBAccess
    {
        public static void CheckUserLevel(User loggedUser)
        {
            if (loggedUser.ID == 1)
            {
                AdminMenu(loggedUser);
            }
            else
            {
                UserMenu(loggedUser);
            }
        }

        private static void AdminMenu(User loggedUser)
        {
            string choice = String.Empty;

            while (choice != "q" && choice != "Q")
            {
                Console.WriteLine($"You are logged in as {loggedUser.Username}\n");
                Console.WriteLine("1.View Account" + "  " + "2.View Members' Account\n" +
                                  "3.Deposit" + "\t" + "4.Withdraw\n5.Transfer" + " \t" + "6.Send to Statement\n");
                Console.Write("Please choose a transaction number or press Q for Quit: ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        ViewMyAccount(loggedUser);
                        break;
                    case "2":
                        Console.Clear();
                        ViewAllAccounts();
                        break;
                    case "3":
                        Console.Clear();
                        Deposit(loggedUser);
                        break;
                    case "4":
                        Console.Clear();
                        Withdraw(loggedUser);
                        break;
                    case "5":
                        Console.Clear();
                        Transfer(loggedUser);
                        break;
                    case "6":
                        Console.Clear();
                        FileAccess.WriteToStatementFile(loggedUser);
                        break;
                    case "Q":
                    case "q":
                        FileAccess.WriteToStatementFile(loggedUser);
                        Environment.Exit(0);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong input. Please try again");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                }
            }
        }

        private static void UserMenu(User loggedUser)
        {
            string choice = String.Empty;

            while (choice != "q" && choice != "Q")
            {
                Console.WriteLine($"You are logged in as {loggedUser.Username}\n");
                Console.WriteLine("1.View Account" + "   " + "2.Deposit\n3.Transfer" + "\t " + "4.Send To Statement\n");
                Console.Write("Please choose a transaction number or press Q for Quit: ");
                choice = Console.ReadLine();

                switch (choice.ToString())
                {
                    case "1":
                        Console.Clear();
                        ViewMyAccount(loggedUser);
                        break;
                    case "2":
                        Console.Clear();
                        Deposit(loggedUser);
                        break;
                    case "3":
                        Console.Clear();
                        Transfer(loggedUser);
                        break;
                    case "4":
                        Console.Clear();
                        FileAccess.WriteToStatementFile(loggedUser);
                        break;
                    case "Q":
                    case "q":
                        FileAccess.WriteToStatementFile(loggedUser);
                        Environment.Exit(0);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong input. Please try again");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                }
            }
        }

        private static User GetAccountHolder(User accountHolder)
        {
            do
            {
                Console.Write("Please give the username of the account holder: ");
                accountHolder.Username = Console.ReadLine();

                int checkUserExists = CheckUserExistsQuery(accountHolder);

                if (checkUserExists > 0)
                {
                    GetAccountHolderQuery(accountHolder);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Non existing user. Please try again.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            } while (accountHolder.ID == 0);

            return accountHolder;
        }

        private static void Deposit(User loggedUser)
        {
            TransactionType transaction = TransactionType.Deposit;
            decimal depositedAmount = CheckAmount(transaction);
            DateTime transactionDate = DateTime.Now;

            string question = "Do you want to deposit to another account? y/n: ";
            string answer = ValidateAnswer(question);

            User accountHolder = new User();
            
            if (answer == "y")
            {
                accountHolder = GetAccountHolder(accountHolder);
            }
            else
            {
                accountHolder = loggedUser;
            }

            DepositQuery(accountHolder, depositedAmount, transactionDate);

            InternalBankAccounts transactionHolder = new InternalBankAccounts(accountHolder.Username, loggedUser.Username, depositedAmount, transactionDate, transaction);
            Console.WriteLine(transactionHolder);

            ConfirmTransaction(transactionHolder);
            ViewMyAccount(loggedUser);
        }

        private static void Withdraw(User loggedUser)
        {
            DateTime transactionDate = DateTime.Now;
            decimal DBAmount = 0;
            User accountHolder = new User();

            TransactionType transaction = TransactionType.Withdraw;
            decimal withdrawAmount = CheckAmount(transaction);
            
            string question = "Do you want to withdraw from another account? y/n: ";
            string answer = ValidateAnswer(question);
            
            if (answer == "y")
            {
                accountHolder = GetAccountHolder(accountHolder);

                DBAmount = GetAmountFromDB(accountHolder);
            }
            else
            {
                accountHolder = loggedUser;

                DBAmount = loggedUser.Balance;
            }

            if (withdrawAmount <= DBAmount)
            {
                WithdrawQuery(accountHolder, withdrawAmount, transactionDate);

                InternalBankAccounts transactionHolder = new InternalBankAccounts(loggedUser.Username, accountHolder.Username, withdrawAmount, transactionDate, transaction);
                Console.WriteLine(transactionHolder);

                ConfirmTransaction(transactionHolder);
                ViewMyAccount(loggedUser);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not Enough Balance");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        private static void Transfer(User loggedUser)
        {
            TransactionType transaction = TransactionType.Transfer;
            decimal transferAmount = CheckAmount(transaction);
            DateTime transactionDate = DateTime.Now;

            User accountHolder = new User();
            accountHolder = GetAccountHolder(accountHolder);

            if (loggedUser.ID == accountHolder.ID)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You cannot transfer money to your account");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                if (transferAmount <= loggedUser.Balance)
                {
                    TransferQuery(loggedUser, accountHolder, transferAmount, transactionDate);

                    InternalBankAccounts transactionHolder = new InternalBankAccounts(accountHolder.Username, loggedUser.Username, transferAmount, transactionDate, transaction);
                    Console.WriteLine(transactionHolder);

                    ConfirmTransaction(transactionHolder);
                    ViewMyAccount(loggedUser);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not Enough Balance");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }

        public static string ValidateAnswer(string question)
        {
            bool check = false;
            string answer = String.Empty;

            do
            {
                Console.Write(question);
                answer = Console.ReadLine();

                check = answer == "y" || answer == "n";

                if (!check)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Please press y or n");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

            } while (!check);

            return answer;
        }

        private static decimal CheckAmount(TransactionType transaction)
        {
            decimal amountChecked = 0;
            bool check = false;

            do
            {
                Console.Write($"Enter the amount you want to {transaction}: ");
                string amount = Console.ReadLine();

                if (check = Decimal.TryParse(amount, out amountChecked))
                {
                    amountChecked = Convert.ToDecimal(amount);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You didn't enter a valid number, please try again.");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            } while (!check);

            return amountChecked;
        }
    }
}
