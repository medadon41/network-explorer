using System;
using System.Management;

namespace CP5;

public class NetworkDevice
{
    public static void ChangeDeviceName(string remoteMachineName, string oldName, string newName, string userName, string password)
    {
        try
        {
            var options = new ConnectionOptions
            {
                Username = userName,
                Password = password,
                EnablePrivileges = true
            };
            var scope = new ManagementScope($"\\\\{remoteMachineName}\\root\\CIMV2", options);
            scope.Connect();

            var query = new ObjectQuery($"SELECT * FROM Win32_ComputerSystem WHERE Name='{oldName}'");
            var searcher = new ManagementObjectSearcher(scope, query);
            var computerSystems = searcher.Get();
            foreach (ManagementObject computerSystem in computerSystems)
            {
                computerSystem["Name"] = newName;
                computerSystem.Put();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
    
    public static void DisableDevice(string remoteMachineName, string name, string userName, string password)
    {
        try
        {
            var options = new ConnectionOptions
            {
                Username = userName,
                Password = password,
                EnablePrivileges = true
            };
            var scope = new ManagementScope($"\\\\{remoteMachineName}\\root\\CIMV2", options);
            scope.Connect();

            var query = new ObjectQuery($"SELECT * FROM Win32_NetworkAdapterConfiguration WHERE Description='{name}'");
            var searcher = new ManagementObjectSearcher(scope, query);
            var networkAdapters = searcher.Get();
            foreach (ManagementObject networkAdapter in networkAdapters)
            {
                networkAdapter.InvokeMethod("Disable", null);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}