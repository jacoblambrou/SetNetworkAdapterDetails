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


namespace NetworkAdapter.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool ShowAll;
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

        private void RefreshManualListView(bool activeOnly = false)
        {
            adapterCollection.Clear();
            adapterCollection = AddAdaptersToCollection(ShowAll);
            lvAdapters.ItemsSource = adapterCollection;
        }

        private void Button_RefreshManualListView(object sender, RoutedEventArgs e)
        {
            RefreshManualListView(ShowAll);
        }

        

        private void CheckBox_ShowAllAdapters_Checked(object sender, RoutedEventArgs e)
        {
            ShowAll = false;         // Refresh must honour checkbox status
            RefreshManualListView(ShowAll);
        }

        private void CheckBox_ShowAllAdapters_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowAll = true;          // Refresh must honour checkbox status
            RefreshManualListView(ShowAll);
        }

        private void LvAdapters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string adapterName = e.AddedItems[0]?.ToString();
                Library.NetworkAdapter selectedAdapter = adapterCollection.Where(x => x.Name == adapterName).FirstOrDefault();


                rtbExraDetails.Clear();
                //rtbExraDetails.Document.Blocks.Clear();
                rtbExraDetails.AppendText($"Description:\t{selectedAdapter.Description}\r\nType:\t\t{selectedAdapter.Type}\r\nStatus:\t\t{selectedAdapter.Status}\r\nDHCP Server:\t{selectedAdapter.DhcpServerAddress}\r\nMAC Address:\t{selectedAdapter.MacAddress}\r\nGUID:\t\t{selectedAdapter.Id}");
                
                tbIpAddress.Text = selectedAdapter.IpAddress;
                tbSubnetMask.Text = selectedAdapter.SubnetMask;
                tbDefaultGateway.Text = selectedAdapter.DefaultGateway;
                tbPrimaryDns.Text = selectedAdapter.PrimaryDns;
                tbSecondaryDns.Text = selectedAdapter.SecondaryDns;
                if (selectedAdapter.DhcpStatus)
                    rUseDhcp.IsChecked = true;
                else
                    rStaticIp.IsChecked = true;
            }
        }

        public void rStaticIp_Checked_cbAutomaticDns_Disabled(object sender, RoutedEventArgs e)
        {
                cbAutomaticDns.IsEnabled = false;
        }

        public void rUseDhcp_Checked_cbAutomaticDns_Enabled(object sender, RoutedEventArgs e)
        {
            cbAutomaticDns.IsEnabled = true;
        }
    }
}
