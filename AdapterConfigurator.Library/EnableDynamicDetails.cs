using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace AdapterConfigurator.Library
{
    public static class EnableDynamicDetails
    {
        public static void SetDynamicIp(uint index, string guid)
        {
            using (var networkConfigMngr = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                using (var networkConfigs = networkConfigMngr.GetInstances())
                {
                    foreach (var managementObject in networkConfigs.Cast<ManagementObject>().Where(managementObject => (bool)managementObject["IPEnabled"] && (uint)managementObject["Index"] == index))
                    {
                        managementObject.InvokeMethod("EnableDHCP", null, null);
                        // TODO: Use WMI to reset interface to ensure routing table is correctly updated and duplicate Default Route is removed
                    }
                }
            }
        }

        public static void SetDynamicDns(uint index, string guid)
        {
            using (var networkConfigMngr = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                using (var networkConfigs = networkConfigMngr.GetInstances())
                {
                    foreach (var managementObject in networkConfigs.Cast<ManagementObject>().Where(managementObject => (bool)managementObject["IPEnabled"] && (uint)managementObject["Index"] == index))
                    {
                        EnableStaticDetails.SetNameServersToNull(guid);
                        managementObject.InvokeMethod("SetDNSServerSearchOrder", null, null);
                    }
                }
            }
        }
    }
}
