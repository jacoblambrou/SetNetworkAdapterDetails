using AdapterConfigurator.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
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


namespace AdapterConfigurator.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool ShowAll;
        private bool AdapterIsSelected = false;
        private uint SelectedIndex;
        private string SelectedGuid;


        //private bool ShowDisconnected;
        //private bool ShowDisabled;
        //private bool ShowVirtual;
        //private bool ShowLoopback;

        private ObservableCollection<Library.NetworkAdapter> adapterCollection = new ObservableCollection<Library.NetworkAdapter>();

        public MainWindow()
        {
            InitializeComponent();
        }


        
        public ObservableCollection<Library.NetworkAdapter> AddAdaptersToCollection(bool showConnectedOnly = false)
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            if (showConnectedOnly)
            {

                foreach (NetworkInterface nic in nics)
                {
                    var adapter = new Library.NetworkAdapter(nic);
                    if (adapter.Status == "Up" && adapter.Type != "Loopback" && !adapter.Description.Contains("Bluetooth Device") && !adapter.Description.Contains("Virtual Adapter"))
                        adapterCollection.Add(adapter);
                }
                return adapterCollection;
            }
            
            foreach (NetworkInterface nic in nics)
            {
                var adapter = new Library.NetworkAdapter(nic);
                if (adapter.Type != "Loopback" && !adapter.Description.Contains("Bluetooth Device") && !adapter.Description.Contains("Virtual Adapter"))
                    adapterCollection.Add(adapter);
            }
            return adapterCollection;
        }

        private void RefreshManualListView()
        {
            rtbLastRefreshed.Clear();
            rtbLastRefreshed.Text = "Refreshing...";
            adapterCollection.Clear();
            adapterCollection = AddAdaptersToCollection(ShowAll);
            lvAdapters.ItemsSource = adapterCollection;
            rtbLastRefreshed.Clear();
            rtbLastRefreshed.Text = $"Last refreshed at: {DateTime.Now.ToLongTimeString()}";
        }

        private void Button_Refresh_ManualListView(object sender, RoutedEventArgs e)
        {
            RefreshManualListView();
        }

        

        private void CheckBox_ShowAllAdapters_Checked(object sender, RoutedEventArgs e)
        {
            ShowAll = false;         // Refresh must honour checkbox status
            RefreshManualListView();
        }

        private void CheckBox_ShowAllAdapters_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowAll = true;          // Refresh must honour checkbox status
            RefreshManualListView();
        }

        private void LvAdapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {

                Library.NetworkAdapter selectedAdapter = FindSelectedAdapter(e);

                rtbExraDetails.Clear();     // Clear existing info from TextBox

                // Print Extra Details to TextBox
                rtbExraDetails.AppendText($"Index\t\t{selectedAdapter.Index}\r\nDescription:\t{selectedAdapter.Description}\r\nType:\t\t{selectedAdapter.Type}\r\nStatus:\t\t{selectedAdapter.Status}\r\nDHCP Server:\t{selectedAdapter.DhcpServerAddress}\r\nMAC Address:\t{selectedAdapter.MacAddress}\r\nGUID:\t\t{selectedAdapter.Guid}");
                
                // Print properties to relevant TextBoxes
                tbIpAddress.Text = selectedAdapter.IpAddress;
                tbSubnetMask.Text = selectedAdapter.SubnetMask;
                tbDefaultGateway.Text = selectedAdapter.DefaultGateway;
                tbPrimaryDns.Text = selectedAdapter.PrimaryDns;
                tbSecondaryDns.Text = selectedAdapter.SecondaryDns;

                // Select relevant RadioButton
                if (selectedAdapter.DhcpStatus)
                    rUseDhcp.IsChecked = true;
                else
                    rStaticIp.IsChecked = true;

                // Tick/Untick Automatic DNS
                if (selectedAdapter.DnsStatus)
                    cbAutomaticDns.IsChecked = true;
                else
                    cbAutomaticDns.IsChecked = false;

                AdapterIsSelected = true;
                this.SelectedIndex = Convert.ToUInt32(selectedAdapter.Index);
                this.SelectedGuid = selectedAdapter.Guid;

            }
        }

        /// <summary>
        /// Automatically unchecks and disables Automatic DNS checkbox if Static IP radio button is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void rStaticIp_Checked_cbAutomaticDns_Disabled_TextFields_Enabled(object sender, RoutedEventArgs e)
        {
            cbAutomaticDns.IsChecked = false;
            cbAutomaticDns.IsEnabled = false;
            tbIpAddress.IsEnabled = true;
            tbSubnetMask.IsEnabled = true;
            tbDefaultGateway.IsEnabled = true;
        }

        /// <summary>
        /// Automatically enables Automatic DNS checkbox if Use DHCP radio button is selected. This does not check the box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void rUseDhcp_Checked_cbAutomaticDns_Enabled_TextFields_Disabled(object sender, RoutedEventArgs e)
        {
            cbAutomaticDns.IsEnabled = true;
            tbIpAddress.IsEnabled = false;
            tbSubnetMask.IsEnabled = false;
            tbDefaultGateway.IsEnabled = false;
        }

        /// <summary>
        /// Finds the adapter that's been selected from the ListView
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public Library.NetworkAdapter FindSelectedAdapter(SelectionChangedEventArgs e)
        {
            //if (e.AddedItems.Count < 1)
                //TODO: Please select an adapter

            string adapterName = e.AddedItems[0]?.ToString();
            Library.NetworkAdapter selectedAdapter = adapterCollection.Where(x => x.Name == adapterName).FirstOrDefault();
            return selectedAdapter;
        }

        public void Button_Apply_UpdateSelectedAdapterSettings(object sender, RoutedEventArgs e)
        {
            if (AdapterIsSelected)
            {
                // TODO: Generate method to extract details
                string ipAddress = tbIpAddress.Text;
                string subnetMask = tbSubnetMask.Text;
                string gw = tbDefaultGateway.Text;
                string receivedPrimaryDns = tbPrimaryDns.Text;
                string receivedSecondaryDns = tbSecondaryDns.Text;

                string primaryDns = null;
                string secondaryDns = null;

                if (!string.IsNullOrWhiteSpace(receivedPrimaryDns))
                    primaryDns = receivedPrimaryDns;

                if (!string.IsNullOrWhiteSpace(receivedSecondaryDns))
                    secondaryDns = receivedSecondaryDns;

                if (rStaticIp.IsChecked ?? false)
                {
                    

                    EnableStaticDetails.SetStaticIP(ipAddress, subnetMask, gw, SelectedIndex);
                    EnableStaticDetails.SetNameServers(primaryDns, secondaryDns, SelectedIndex, SelectedGuid);
                }
                else if (rUseDhcp.IsChecked ?? false)
                {
                    EnableDynamicDetails.SetDynamicIp(SelectedIndex, SelectedGuid);

                    if (cbAutomaticDns.IsChecked ?? false)
                    {
                        EnableDynamicDetails.SetDynamicDns(SelectedIndex, SelectedGuid);
                    }
                    else
                    {
                        EnableStaticDetails.SetNameServers(primaryDns, secondaryDns, SelectedIndex, SelectedGuid);
                    }
                }
            }
            else
            {
                // TODO: "Please select an adapter"
            }

            Button_Refresh_ManualListView(sender, e);
        }
    }
}
