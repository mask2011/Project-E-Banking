using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_E_Banking
{
    class Program
    {
        static void Main(string[] args)
        {
            User loggedUser = LoginScreen.LoginSystem();

            ApplicationMenus.CheckUserLevel(loggedUser);
        }
    }
}
