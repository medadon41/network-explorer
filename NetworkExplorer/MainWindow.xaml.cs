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

namespace NetworkExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<NetworkNode> Nodes { get; set; } = new();

        public NetworkNode SelectedNode = new NetworkNode() { IPAdress = "None", MAC = "None", Name = "None" };

        public MainWindow()
        {
            InitializeComponent();
            FillNodes();
            RefreshList();
        }

        public void SetFields()
        {
            IPAddress_Block.Text = SelectedNode.IPAdress;
            MACAdress_Block.Text = SelectedNode.MAC ?? "None";
            NameInput.Text = SelectedNode.Name ?? "None";
        }

        public void RefreshList()
        {
            NodesList.Items.Clear();
            foreach (var node in Nodes)
            {
                NodesList.Items.Add(node.IPAdress);
            }
        }

        private void FillNodes()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();

            foreach (IPAddress address in GetArpAddresses())
            {
                bool isConnected = false;
                foreach (TcpConnectionInformation connection in connections)
                {
                    if (connection.LocalEndPoint.Address.Equals(address))
                    {
                        isConnected = true;
                        break;
                    }
                }

                if (!isConnected)
                {
                    Nodes.Add(new NetworkNode()
                    {
                        IPAdress = address.ToString()
                    });
                }
            }
        }

        List<IPAddress> GetArpAddresses()
        {
            List<IPAddress> addresses = new List<IPAddress>();

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
                        IPAddress address = IPAddress.Parse(tokens[0]);
                        addresses.Add(address);
                    }
                    catch (FormatException) { }
                }
            }

            return addresses;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RefreshList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IPHostEntry host = Dns.GetHostEntry(SelectedNode.IPAdress);
            string nodeName = host.HostName;
            Console.WriteLine(nodeName);

            ManagementScope scope = new ManagementScope($"\\\\{nodeName}\\root\\cimv2");
            scope.Connect();

            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                mo["Name"] = "newName";
                mo.Put();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void NodesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NodesList.SelectedValue is not null)
            {
                var a = NodesList.SelectedValue.ToString();
                SelectedNode = Nodes.FirstOrDefault(i => i.IPAdress == NodesList.SelectedValue.ToString());
                SetFields();
            }
        }

        private void NameInput_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
