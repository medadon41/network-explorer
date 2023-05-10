using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class IpMacPair
    {
        public IPAddress IPAddress { get; set; }

        public PhysicalAddress PhysicalAddress { get; set; }
    }
}
