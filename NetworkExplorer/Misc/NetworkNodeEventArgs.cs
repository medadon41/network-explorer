using NetworkExplorer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkExplorer.Misc
{
    public class NetworkNodeEventArgs : EventArgs
    {
        public NetworkNodeEventArgs(NetworkNode node)
        {
            Node = node;
        }

        public NetworkNode Node { get; private set; }

    }
}
