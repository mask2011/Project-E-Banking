using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_E_Banking
{
    class LoginScreen
    {
        public static User LoginSystem()
        {
            Console.WriteLine("Welcome to the Internal Banking System");
            User loggedUser = DBAccess.CheckCredentials();

            return loggedUser;
        }

        public static List<string> GetInputs()
        {
            List<string> inputs = new List<string>();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Please enter your username: ");
            string username = Console.ReadLine();
            inputs.Add(username);

            Console.Write("Please enter your password: ");
            Console.ForegroundColor = ConsoleColor.Black;//for hiding the typing of the password.
            string password = Encryption.EncryptString(Console.ReadLine());
            inputs.Add(password);
            Console.ForegroundColor = ConsoleColor.Gray;

            return inputs;
        }
    }
}
