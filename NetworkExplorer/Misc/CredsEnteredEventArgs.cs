using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkExplorer.Misc
{
    public class CredsEnteredEventArgs : EventArgs
    {
        public CredsEnteredEventArgs(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get; set; }

        public string Password { get; set; }
    }
}
