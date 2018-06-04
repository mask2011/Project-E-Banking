using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_E_Banking
{
    class DBAccess
    {
        private static afdempDBDataContext afdempDB = new afdempDBDataContext();
        
        public static User CheckCredentials()
        {
            User loggedUser = new User();
            
            int counter = 0; //to give three tries for the username/password input.
            int triesLeft = 3; //for letting the user know how many tries are left.

            do
            {
                List<string> inputs = LoginScreen.GetInputs();
                triesLeft -= 1;
                counter += 1;
                int queryDB = 0;

                try
                {
                    queryDB = (from u in afdempDB.users
                               where u.username.Equals(inputs[0]) &&
                               u.password.Equals(inputs[1])
                               select u).Count();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }


                if (queryDB > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Successfull Login");
                    Console.ForegroundColor = ConsoleColor.Gray;

                    loggedUser = GetUser(inputs, loggedUser);
                    FileAccess.CreateStatementFile(loggedUser);

                    System.Threading.Thread.Sleep(1000);
                    Console.Clear();
                    break;
                }
                else
                {
                    if (counter == 3)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You are locked out!!!");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Environment.Exit(0);//τερματίζει την εφαρμογή. Το 0 είναι ότι τερματίστηκε χωρίς σφάλμα.
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Incorrect Username/Password. You have {triesLeft} tries left.");
                    }
                }
            } while (counter < 3);

            return loggedUser;
        }

        private static User GetUser(List<string> inputs, User loggedUser)
        {
            try
            {
                var getUserQuery = (from u in afdempDB.users
                                    where u.username.Equals($"{inputs[0]}")
                                    select u).FirstOrDefault();

                var getUsersAccountQuery = (from acc in afdempDB.accounts
                                            where acc.user_id.Equals(getUserQuery.id)
                                            select acc).FirstOrDefault();

                loggedUser.ID = getUserQuery.id;

                loggedUser.Username = getUserQuery.username;

                loggedUser.Balance = getUsersAccountQuery.amount;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return loggedUser;
        }

        protected static void ConfirmTransaction(InternalBankAccounts transactionHolder)
        {
            string question = "Confirm transaction? y/n: ";
            string answer = ApplicationMenus.ValidateAnswer(question);

            if (answer == "y")
            {
                FileAccess.memoryBuffer.Add(transactionHolder);

                try
                {
                    afdempDB.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        protected static void TransferQuery(User loggedUser, User accountHolder, decimal transferAmount, DateTime transactionDate)
        {
            try
            {
                var transferFromAccount = (from acc in afdempDB.accounts
                                           where acc.user_id.Equals(loggedUser.ID)
                                           select acc).SingleOrDefault();

                transferFromAccount.amount -= transferAmount;
                transferFromAccount.transaction_date = transactionDate;

                var transferToAccount = (from acc in afdempDB.accounts
                                         where acc.user_id.Equals(accountHolder.ID)
                                         select acc).SingleOrDefault();

                transferToAccount.amount += transferAmount;
                transferToAccount.transaction_date = transactionDate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        protected static void WithdrawQuery(User accountHolder, decimal withdrawAmount, DateTime transactionDate)
        {
            try
            {
                var withdrawQuery = (from acc in afdempDB.accounts
                                     where acc.user_id.Equals(accountHolder.ID)
                                     select acc).SingleOrDefault();

                withdrawQuery.amount -= withdrawAmount;
                withdrawQuery.transaction_date = transactionDate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        protected static decimal GetAmountFromDB(User accountHolder)
        {
            decimal DBAmount = 0;

            try
            {
                DBAmount = (from acc in afdempDB.accounts
                            where acc.user_id.Equals(accountHolder.ID)
                            select acc.amount).SingleOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return DBAmount;
        }

        protected static void DepositQuery(User accountHolder, decimal depositedAmount, DateTime transactionDate)
        {
            try
            {
                var depositQuery = (from acc in afdempDB.accounts
                                    where acc.user_id.Equals(accountHolder.ID)
                                    select acc).SingleOrDefault();

                depositQuery.amount += depositedAmount;
                depositQuery.transaction_date = transactionDate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        protected static void ViewMyAccount(User loggedUser)
        {
            try
            {
                afdempDB.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, afdempDB.accounts);

                var viewMyAccountQuery = (from acc in afdempDB.accounts
                                          where acc.user_id.Equals(loggedUser.ID)
                                          select acc).FirstOrDefault();

                loggedUser.Balance = viewMyAccountQuery.amount;

                Console.WriteLine($"Your Balance is: {viewMyAccountQuery.amount}\n" +
                                  $"Your last transaction was on {viewMyAccountQuery.transaction_date}\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        protected static void ViewAllAccounts()
        {
            try
            {
                var viewAllAccountsQuery = (from acc in afdempDB.accounts
                                            join u in afdempDB.users
                                            on acc.user_id equals u.id
                                            where acc.user_id != 1
                                            select acc).ToList();

                viewAllAccountsQuery.ForEach(acc => Console.WriteLine($"{acc.user.username}'s Balance is: {acc.amount}\n" +
                                                                      $"Last Transaction was on {acc.transaction_date}\n"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        protected static int CheckUserExistsQuery(User accountHolder)
        {
            int checkUserExistsQuery = 0;

            try
            {
                checkUserExistsQuery = (from u in afdempDB.users
                                        where u.username.Equals(accountHolder.Username)
                                        select u).Count();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return checkUserExistsQuery;
        }

        protected static void GetAccountHolderQuery(User accountHolder)
        {
            try
            {
                accountHolder.ID = (from u in afdempDB.users
                                    where u.username.Equals(accountHolder.Username)
                                    select u.id).SingleOrDefault();

                accountHolder.Username = (from u in afdempDB.users
                                          where u.username.Equals(accountHolder.Username)
                                          select u.username).SingleOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
