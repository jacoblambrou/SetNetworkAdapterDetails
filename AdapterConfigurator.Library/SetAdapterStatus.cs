using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;

namespace AdapterConfigurator.Library
{
    class SetAdapterStatus
    {
        public static void EnableAdapter(string guid)
        {
            using (var networkAdapters = new ManagementClass("Win32_NetworkAdapter"))
            {
                using (var adapterCollection = networkAdapters.GetInstances())
                {
                    foreach (var adapter in adapterCollection.Cast<ManagementObject>().Where(adapter => (string)adapter["GUID"] == guid))
                    {
                        adapter.InvokeMethod("Enable", null, null);

                        ushort isConnected = (ushort)adapter["NetConnectionStatus"];

                        for (int i = 0; i < 5; i++)
                        {
                            Thread.Sleep(2000);
                            isConnected = (ushort)adapter["NetConnectionStatus"];
                            if (isConnected == 2)
                            {
                                Thread.Sleep(2000);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static void DisableAdapter(string guid)
        {
            using (var networkAdapters = new ManagementClass("Win32_NetworkAdapter"))
            {
                using (var adapterCollection = networkAdapters.GetInstances())
                {
                    foreach (var adapter in adapterCollection.Cast<ManagementObject>().Where(adapter => (string)adapter["GUID"] == guid))
                    {
                        adapter.InvokeMethod("Disable", null, null);
                    }
                }
            }
        }
    }
}
