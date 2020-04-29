using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace NetworkAdapter.Library
{
    public class NetworkAdapter : INetworkAdapter
    {
        public string AdapterName { get; set; } = string.Empty;
        public string AdapterDescription { get; set; } = string.Empty;
        public string AdapterType { get; set; } = string.Empty;
        public string AdapterStatus { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string SubnetMask { get; set; } = string.Empty;
        public string DefaultGateway { get; set; } = string.Empty;
        public string DnsServers { get; set; } = string.Empty;

        public NetworkAdapter()
        {

        }

        public NetworkAdapter(NetworkInterface nic)
        {
            this.AdapterName = GetAdapterName(nic);
            this.AdapterDescription = GetAdapterDescription(nic);
            this.AdapterType = GetAdapterType(nic);
            this.AdapterStatus = GetAdapterStatus(nic);
            this.IpAddress = GetIpAddress(nic);
            this.SubnetMask = GetSubnetMask(nic);
            this.DefaultGateway = GetDefaultGateway(nic);
            this.DnsServers = GetDnsServers(nic);
        }

        public string GetAdapterName(NetworkInterface nic)
        {            
            return nic.Name.ToString();
        }

        public string GetAdapterDescription(NetworkInterface nic)
        {
            return nic.Description.ToString();
        }

        public string GetAdapterType(NetworkInterface nic)
        {
            return nic.NetworkInterfaceType.ToString();
        }

        public string GetAdapterStatus(NetworkInterface nic)
        {
            return nic.OperationalStatus.ToString();
        }

        public string GetIpAddress(NetworkInterface nic)
        {
            foreach (UnicastIPAddressInformation unicastAddress in nic.GetIPProperties().UnicastAddresses)
            {
                if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return unicastAddress.Address.ToString();
                }
            }

            return "Not set";
        }

        public string GetSubnetMask(NetworkInterface nic)
        {
            foreach (var unicastAddress in nic.GetIPProperties().UnicastAddresses)
            {
                if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return unicastAddress.IPv4Mask.ToString();
                }
            }

            return "Not set";
        }

        public string GetDefaultGateway(NetworkInterface nic)
        {
            foreach (GatewayIPAddressInformation gw in nic.GetIPProperties().GatewayAddresses)
            {
                return gw.Address.ToString();
            }

            return "Not set";
        }

        public string GetDnsServers(NetworkInterface nic)
        {
            string dnsServers = string.Empty;
            foreach (IPAddress dnsServer in nic.GetIPProperties().DnsAddresses)
            {
                if (dnsServer.AddressFamily == AddressFamily.InterNetwork)
                    dnsServers += $"{dnsServer}\r\n                          ";
            }
            if (dnsServers.Length > 2)      // Check if DNS server(s) exist
            {
                return dnsServers.Trim();
            }

            return "Not set";
        }

        
        public string DisplayAdapterInfo()
        {
            StringBuilder adapterBuilder = new StringBuilder();
            adapterBuilder.Append($"Name. . . . . . . . . . : {AdapterName}\r\n");
            adapterBuilder.Append($"Description . . . . . . : {AdapterDescription}\r\n");
            adapterBuilder.Append($"Type. . . . . . . . . . : {AdapterType}\r\n");
            adapterBuilder.Append($"Status. . . . . . . . . : {AdapterStatus}\r\n");
            adapterBuilder.Append($"IP Address. . . . . . . : {IpAddress}\r\n");
            adapterBuilder.Append($"Subnet Mask . . . . . . : {SubnetMask}\r\n");
            adapterBuilder.Append($"Default Gateway . . . . : {DefaultGateway}\r\n");
            adapterBuilder.Append($"DNS Servers . . . . . . : {DnsServers}\r\n");
            
            return adapterBuilder.ToString();
        }

        public string DisplayAdapterInfo(NetworkInterface nic)
        {
            StringBuilder adapterBuilder = new StringBuilder();
            adapterBuilder.Append($"Name. . . . . . . . . . : {GetAdapterName(nic)}\r\n");
            adapterBuilder.Append($"Description . . . . . . : {GetAdapterDescription(nic)}\r\n");
            adapterBuilder.Append($"Type. . . . . . . . . . : {GetAdapterType(nic)}\r\n");
            adapterBuilder.Append($"Status. . . . . . . . . : {GetAdapterStatus(nic)}\r\n");
            adapterBuilder.Append($"IP Address. . . . . . . : {GetIpAddress(nic)}\r\n");
            adapterBuilder.Append($"Subnet Mask . . . . . . : {GetSubnetMask(nic)}\r\n");
            adapterBuilder.Append($"Default Gateway . . . . : {GetDefaultGateway(nic)}\r\n");
            adapterBuilder.Append($"DNS Server. . . . . . . : {GetDnsServers(nic)}\r\n");
            
            return adapterBuilder.ToString();
        }
        
        public string DisplayAdapterInfo(NetworkInterface nic, bool showDisconnectedInterfaces)
        {
            return "";
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(AdapterName))
                return "Adapter must have a name";

            return string.Format(AdapterName);
        }
    }
}
