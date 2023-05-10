using MinimalisticTelnet;
using NetworkExplorer.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetworkExplorer
{
    /// <summary>
    /// Interaction logic for AuthPrompt.xaml
    /// </summary>
    public partial class AuthPrompt : Window
    {
        public AuthPrompt()
        {
            InitializeComponent();
        }

        public event EventHandler<CredsEnteredEventArgs> DataEntered;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginInput.Text;
            string password = PasswordInput.Text;

            try
            {
                TelnetConnection tc = new TelnetConnection("192.168.1.1", 23);

                string s = tc.Login(login, password, 1000);
                Console.Write(s);

                string prompt = s.TrimEnd();
                prompt = s.Substring(prompt.Length - 1, 1);
                if (prompt != "$" && prompt != ">")
                    throw new Exception("Connection failed");
                MessageBox.Show("Success!");
                DataEntered?.Invoke(this, new CredsEnteredEventArgs(login, password));
                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
