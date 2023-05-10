using NetworkExplorer.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Management;
using ConsoleApp1;
using System.Text.RegularExpressions;
using NetworkExplorer.Misc;
using MinimalisticTelnet;
using System.IO;
using System.Text.Json;
using System.ComponentModel;
using System.Xml.Linq;

namespace NetworkExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<NetworkNode> Nodes { get; set; } = new();

        public ObservableCollection<NetworkNode> DisabledNodes { get; set; } = new();

        private CollectionViewSource _nodesViewSource;

        private CollectionViewSource _disabledNodesViewSource;

        private string? adminLogin {get; set;}

        private string? adminPassword { get; set;}

        public NetworkNode SelectedNode = new NetworkNode() { IPAdress = "None", MAC = "None", Name = "None" };

        public MainWindow()
        {
            InitializeComponent();
            FillNodes();
            _nodesViewSource =
                    (CollectionViewSource)FindResource(nameof(_nodesViewSource));
            _disabledNodesViewSource =
                    (CollectionViewSource)FindResource(nameof(_disabledNodesViewSource));
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _nodesViewSource.Source = Nodes.ToList();
            await RestoreDisabledNodes();
        }

        private void FillNodes()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();


            foreach (IpMacPair pair in GetArpAddresses())
            {
                if (!pair.IPAddress.ToString().StartsWith("192")) {
                    continue;
                }

                bool isConnected = false;
                foreach (TcpConnectionInformation connection in connections)
                {
                    if (connection.LocalEndPoint.Address.Equals(pair.IPAddress))
                    {
                        isConnected = true;
                        break;
                    }
                }

                if (!isConnected)
                {
                    Nodes.Add(new NetworkNode()
                    {
                        Name = GetNodeName(pair.IPAddress.ToString()) ?? "Unknown",
                        IPAdress = pair.IPAddress.ToString(),
                        MAC = pair.PhysicalAddress.ToString(),
                    });
                }
            }
        }


        private string? GetNodeName(string ipadress)
        {
            Process process = new Process();
            process.StartInfo.FileName = "nbtstat";
            process.StartInfo.Arguments = $"-a {ipadress}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            string pattern = @"(\b\w+-*\w*\b)\s+<20>";
            Match match = Regex.Match(output, pattern);

            if (match.Success)
            {
                string computerName = match.Groups[1].Value;
                return computerName;
            }
            return null;
        }

        private List<IpMacPair> GetArpAddresses()
        {
            List<IpMacPair> addresses = new List<IpMacPair>();

            ProcessStartInfo psi = new ProcessStartInfo("arp", "-a");
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;

            Process process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();

            string[] lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 3; i < lines.Length; i++)
            {
                string[] tokens = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length >= 2)
                {
                    try
                    {
                        var a = tokens;
                        IPAddress address = IPAddress.Parse(tokens[0]);
                        PhysicalAddress mac = PhysicalAddress.Parse(tokens[1]);
                        addresses.Add(new IpMacPair
                        {
                            IPAddress = address,
                            PhysicalAddress = mac
                        });
                    }
                    catch (FormatException) { }
                }
            }


            return addresses;
        }



        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            AuthPrompt authPrompt = new();
            authPrompt.DataEntered += AuthPrompt_DataEntered;
            authPrompt.Show();
        }


        public async void OnWindowClosing(object sender, CancelEventArgs e)
        {
            await SaveDisabledNodes();
        }

        private async Task SaveDisabledNodes()
        {
            if (DisabledNodes.Count > 0)
            {
                using (FileStream fs = new FileStream($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments)}\\disabledNodes.json", FileMode.Truncate))
                {
                    //var nodes = Nodes.ToList();
                    await JsonSerializer.SerializeAsync<List<NetworkNode>>(fs, DisabledNodes.ToList());
                    Console.WriteLine("Data has been saved to file");
                }
            }
        }

        private async Task RestoreDisabledNodes()
        {
            using (FileStream fs = new FileStream($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments)}\\disabledNodes.json", FileMode.OpenOrCreate))
            {
                var nodes = new List<NetworkNode>();
                try
                {
                    nodes = await JsonSerializer.DeserializeAsync<List<NetworkNode>>(fs);
                }
                catch (Exception ex)
                {
                }

                foreach (var node in nodes)
                {
                    DisabledNodes.Add(node);
                }
            }
            Console.WriteLine();
        }


        private void AdminButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            NetworkNode node = button.DataContext as NetworkNode;
            if (node != null)
            {
                NodeInfoWindow nodeInfoWindow = new NodeInfoWindow(adminLogin, adminPassword, node);
                nodeInfoWindow.NodeDisabled += NodeInfo_NodeDisabled;
                nodeInfoWindow.NodeEnabled += NodeInfo_NodeEnabled;
                nodeInfoWindow.Show();
            }
        }

        private void AuthPrompt_DataEntered(object sender, CredsEnteredEventArgs e)
        {
            adminLogin = e.Login;
            adminPassword = e.Password;
            AdminButton.IsEnabled = false;
            EditButtons.Visibility = Visibility.Visible;
            DisabledNodesTable.Visibility = Visibility.Visible;
            DisabledNodesTable.ItemsSource = null;
            DisabledNodesTable.ItemsSource = DisabledNodes;
        }

        private void NodeInfo_NodeDisabled(object sender, NetworkNodeEventArgs e)
        {
            DisabledNodes.Add(e.Node);
        }

        private void NodeInfo_NodeEnabled(object sender, NetworkNodeEventArgs e)
        {
            DisabledNodes.Remove(e.Node);
        }

        private void DisabledNodeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            NetworkNode node = button.DataContext as NetworkNode;
            if (node != null)
            {
                TelnetConnection tc = new TelnetConnection("192.168.1.1", 23);

                string s = tc.Login(adminLogin, adminPassword, 1000);
                Console.Write(s);

                string mac = string.Join(":", Enumerable.Range(0, 6)
                    .Select(i => node.MAC.Substring(i * 2, 2).ToLower()));


                tc.WriteLine($"ip hotspot host {mac} permit");

            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Nodes.Clear();
            FillNodes();
        }
    }
}
