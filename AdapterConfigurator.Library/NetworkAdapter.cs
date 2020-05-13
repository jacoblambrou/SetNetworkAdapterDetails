using System;
using System.Collections.Generic;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace AdapterConfigurator.Library
{
    public class NetworkAdapter /*: INetworkAdapter*/
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string SubnetMask { get; set; } = string.Empty;
        public string DefaultGateway { get; set; } = string.Empty;
        public string PrimaryDns { get; set; } = string.Empty;
        public string SecondaryDns { get; set; } = string.Empty;
        public bool DhcpStatus { get; set; } = true;
        public string? DhcpServerAddress { get; set; }
        public string MacAddress { get; set; } = string.Empty;
        public uint? Index { get; set; }

        public NetworkAdapter()
        {

        }

        public NetworkAdapter(NetworkInterface nic)
        {
            this.Name = GetAdapterName(nic);
            this.Description = GetAdapterDescription(nic);
            this.Type = GetAdapterType(nic);
            this.Status = GetAdapterStatus(nic);
            this.IpAddress = GetIpAddress(nic);
            this.SubnetMask = GetSubnetMask(nic);
            this.DefaultGateway = GetDefaultGateway(nic);
            this.PrimaryDns = GetDnsServers(nic)[0];
            this.SecondaryDns = GetDnsServers(nic)[1];
            this.DhcpStatus = GetDhcpStatus(nic);
            this.DhcpServerAddress = GetDhcpServerAddress(nic);
            this.Id = GetAdapterId(nic);
            this.MacAddress = GetMacAddress(nic);
            this.Index = GetIndex(nic);
        }

        public string GetAdapterId(NetworkInterface nic)
        {
            return nic.Id.ToString();       // .Trim('{').Trim('}');
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

        public string[] GetDnsServers(NetworkInterface nic)
        {
            //string dnsServers = string.Empty;
            List<string> dnsServers = new List<string>();
            foreach (IPAddress dnsServer in nic.GetIPProperties().DnsAddresses)
            {
                if (dnsServer.AddressFamily == AddressFamily.InterNetwork)
                    dnsServers.Add(dnsServer.ToString());
            }
            if (dnsServers.Count > 1)      // Check if DNS server(s) exist
            {
                return dnsServers.ToArray();
            }
            while (dnsServers.Count < 2)
            {
                dnsServers.Add("Not Set");
            }
            return dnsServers.ToArray();
        }

        public bool GetDhcpStatus(NetworkInterface nic)
        {
            return nic.GetIPProperties().GetIPv4Properties().IsDhcpEnabled;

        }

        public string? GetDhcpServerAddress(NetworkInterface nic)
        {
            var dhcpServer = nic.GetIPProperties().DhcpServerAddresses;
            if (dhcpServer.Count < 1)
                return "Not used";

            return dhcpServer[0].ToString();
        }

        public string GetMacAddress(NetworkInterface nic)
        {
            string mac = nic.GetPhysicalAddress().ToString();
            return Regex.Replace(mac,".{2}","$0-").Trim('-');       // Insert - every 2 characters, then trim - from either end
        }

        public uint? GetIndex(NetworkInterface nic)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_NetworkAdapter");
            foreach (ManagementObject adapter in searcher.Get())
            {
                if (adapter["Guid"]?.ToString() == GetAdapterId(nic))
                    return Convert.ToUInt32(adapter["Index"]);
            }
            return null;
        }


        public string DisplayAdapterInfo()
        {
            StringBuilder adapterBuilder = new StringBuilder();
            adapterBuilder.Append($"Index . . . . . . . . . : {Index}\r\n");
            adapterBuilder.Append($"Name. . . . . . . . . . : {Name}\r\n");
            adapterBuilder.Append($"Description . . . . . . : {Description}\r\n");
            adapterBuilder.Append($"Type. . . . . . . . . . : {Type}\r\n");
            adapterBuilder.Append($"Status. . . . . . . . . : {Status}\r\n");
            adapterBuilder.Append($"IP Address. . . . . . . : {IpAddress}\r\n");
            adapterBuilder.Append($"Subnet Mask . . . . . . : {SubnetMask}\r\n");
            adapterBuilder.Append($"Default Gateway . . . . : {DefaultGateway}\r\n");
            adapterBuilder.Append($"Primary DNS . . . . . . : {PrimaryDns}\r\n");
            adapterBuilder.Append($"Secondary DNS . . . . . : {SecondaryDns}\r\n");
            adapterBuilder.Append($"DHCP Server Address . . : {DhcpServerAddress}\r\n");
            adapterBuilder.Append($"MAC Address . . . . . . : {MacAddress}\r\n"); 
            adapterBuilder.Append($"Id. . . . . . . . . . . : {Id}\r\n");

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
            adapterBuilder.Append($"MAC Address . . . . . . : {GetMacAddress(nic)}\r\n");
            adapterBuilder.Append($"Id. . . . . . . . . . . : {GetAdapterId(nic)}\r\n");

            return adapterBuilder.ToString();
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return "Adapter must have a name";

            return string.Format(Name);
        }
    }
}
