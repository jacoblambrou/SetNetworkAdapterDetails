using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace NetworkAdapter.Library
{
    public interface INetworkAdapter
    {
        string Name { get; set; }
        string Description { get; set; }
        string Type { get; set; }
        string Status { get; set; }
        string IpAddress { get; set; }
        string SubnetMask { get; set; }
        string DefaultGateway { get; set; }
        string DnsServers { get; set; }

        string GetAdapterName(NetworkInterface nic);
        string GetAdapterDescription(NetworkInterface nic);
        string GetAdapterType(NetworkInterface nic);
        string GetAdapterStatus(NetworkInterface nic);
        string GetIpAddress(NetworkInterface nic);
        string GetSubnetMask(NetworkInterface nic);
        string GetDefaultGateway(NetworkInterface nic);
        string GetDnsServers(NetworkInterface nic);
    }
}
