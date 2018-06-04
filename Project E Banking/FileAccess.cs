using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Project_E_Banking
{
    class FileAccess
    {
        public static List<InternalBankAccounts> memoryBuffer = new List<InternalBankAccounts>();

        public static void CreateStatementFile(User loggedUser)
        {
            string filePath = $"C:\\Bootcamp3\\C#\\C# Projects\\Project E Banking\\Statements\\statement_{loggedUser.Username}_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}.txt";

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
        }

        public static void WriteToStatementFile(User loggedUser)
        {
            if (memoryBuffer.Any())
            {
                var convertBufferToStringList = memoryBuffer.ConvertAll(x => Convert.ToString(x));
                StreamWriter file = new StreamWriter($@"C:\\Bootcamp3\\C#\\C# Projects\\Project E Banking\\Statements\\statement_{loggedUser.Username}_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}.txt", true);

                using (file)
                {
                    foreach (string line in convertBufferToStringList)
                    {
                        if (!line.Contains(DateTime.Now.ToString()))
                        {
                            file.WriteLine(line);
                        }
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Transactions were written to Statement File\n");
                Console.ForegroundColor = ConsoleColor.Gray;
                memoryBuffer.Clear();
            }
        }
    }
}
