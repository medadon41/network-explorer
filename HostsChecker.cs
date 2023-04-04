using System.Net;
using System.Net.NetworkInformation;

namespace CP5;

public static class HostsChecker
{
    public static void Collect()
    {
        string ipAddress = "192.168.1.";
        int timeout = 1000;
        for (int i = 1; i < 255; i++)
        {
            string ip = ipAddress + i.ToString();
            Ping ping = new Ping();
            PingReply reply = ping.Send(ip, timeout);
            if (reply.Status == IPStatus.Success)
            {
                try
                {
                    IPHostEntry host = Dns.GetHostEntry(ip);
                    Console.WriteLine("Device found: " + ip);
                    Console.WriteLine("  Host name: " + host.HostName);
                    PhysicalAddress macAddr = GetMacAddress(ip);
                    if (macAddr != null)
                    {
                        Console.WriteLine("  MAC address: " + macAddr.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to get information for device " + ip + ": " + ex.Message);
                }
            }
        }
    }
    
    static PhysicalAddress GetMacAddress(string ipAddress)
    {
        IPAddress addr = IPAddress.Parse(ipAddress);
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ip.Address.Equals(addr))
                {
                    return ni.GetPhysicalAddress();
                }
            }
        }
        return null;
    }
}