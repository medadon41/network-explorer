using MinimalisticTelnet;
using NetworkExplorer.Data;
using NetworkExplorer.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace NetworkExplorer
{
    /// <summary>
    /// Interaction logic for NodeInfoWindow.xaml
    /// </summary>
    public partial class NodeInfoWindow : Window
    {
        public event EventHandler<NetworkNodeEventArgs> NodeDisabled;

        public event EventHandler<NetworkNodeEventArgs> NodeEnabled;

        string adminLogin { get; set; }

        string adminPassword { get; set; }

        NetworkNode node { get; set; }

        int speedRate { get; set; }

        string networkName { get; set; }

        bool isDisabled { get; set; }

        public NodeInfoWindow(string adminLogin, string adminPassword, NetworkNode node)
        {
            InitializeComponent();
            this.adminLogin = adminLogin;
            this.adminPassword = adminPassword;
            this.node = node;
            FillNode();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MacTextbox.Text = node.MAC;
            IpTextbox.Text = node.IPAdress;
            NodeName.Text = node.Name ?? "Not defined";
            NodeNetworkName.Text = networkName;
            SpeedRateTextbox.Text = speedRate.ToString();
            ToggleLockButton.Content = isDisabled ? "Enable node" : "Disable node";
        }

        private void FillNode()
        {
            TelnetConnection tc = new TelnetConnection("192.168.1.1", 23);

            string s = tc.Login(adminLogin, adminPassword, 1000);
            Console.Write(s);

            string mac = string.Join(":", Enumerable.Range(0, 6)
                .Select(i => node.MAC.Substring(i * 2, 2).ToLower()));

            tc.WriteLine($"show ip hotspot {mac}");

            string result = tc.Read();

            networkName = Regex.Match(result, (@"(?<=\bname: )[^\r\n]+(?<!\bhost\s+name:)")).ToString();
            isDisabled = Regex.Match(result, @"(?<=\baccess: )[^\r\n]+").ToString().Equals("deny");
            speedRate = int.Parse(Regex.Match(result, @"(?<=\brx: )\d+").ToString());
        }

        private void NodeNameChange_Button(object sender, RoutedEventArgs e)
        {
            try
            {
                TelnetConnection tc = new TelnetConnection("192.168.1.1", 23);

                string s = tc.Login(adminLogin, adminPassword, 1000);
                Console.Write(s);

                string mac = string.Join(":", Enumerable.Range(0, 6)
                    .Select(i => node.MAC.Substring(i * 2, 2).ToLower()));

                string name = NodeName.Text;

                tc.WriteLine($"known host {name} {mac}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SpeedRateChangeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TelnetConnection tc = new TelnetConnection("192.168.1.1", 23);

                string s = tc.Login(adminLogin, adminPassword, 1000);
                Console.Write(s);

                string mac = string.Join(":", Enumerable.Range(0, 6)
                    .Select(i => node.MAC.Substring(i * 2, 2).ToLower()));

                int rate = int.Parse(SpeedRateTextbox.Text);

                if (rate > 0)
                    tc.WriteLine($"ip traffic-shape host {mac} rate {rate}");
                else
                    tc.WriteLine($"no ip traffic-shape host {mac}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ToggleLock(bool isDisabled)
        {
            TelnetConnection tc = new TelnetConnection("192.168.1.1", 23);

            string s = tc.Login(adminLogin, adminPassword, 1000);
            Console.Write(s);

            string mac = string.Join(":", Enumerable.Range(0, 6)
                .Select(i => node.MAC.Substring(i * 2, 2).ToLower()));

            bool toEnable = isDisabled;

            if (toEnable)
                tc.WriteLine($"ip hotspot host {mac} permit");
            else
                tc.WriteLine($"ip hotspot host {mac} deny");
        }
        private void ToggleLockButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isDisabled)
            {
                NodeDisabled?.Invoke(sender, new NetworkNodeEventArgs(node));
            } 
            else
            {
                NodeEnabled?.Invoke(sender, new NetworkNodeEventArgs(node));
            }
            ToggleLock(isDisabled);
            isDisabled = !isDisabled;
            ToggleLockButton.Content = isDisabled ? "Enable node" : "Disable node";
        }
    }
}
