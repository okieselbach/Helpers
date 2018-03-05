using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WirelessConnect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string winPEVersion = string.Empty;
        private string currentSsid = string.Empty;
        private string currentSecurityKey = string.Empty;
        private bool useWiredConnectionQuestion = true;
        private bool firstConnectLock = true;
        private bool firstConnectTriggered = false;
        private WirelessNetwork wlan;

        private NetworkConfig networkConfig;
        private ConnectionState connectionState = new ConnectionState();

        private Timer timer;
        private Timer firstConnect;

        private bool manualConfigurationIsExpanded = false;

        public MainWindow()
        {
            InitializeComponent();

            // Kill ourself if we already have a wired network connection...
            if (NetworkConfig.IsWiredNetworkAvailable())
            {
#if !DEBUG
                Application.Current.Shutdown();
#endif
            }

            var parameters = Helper.GetRawArguments();
            var parameter = string.Empty;
            var customUrl = string.Empty;
            var debug = false;
            if (parameters.Length >= 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    parameter = parameters[i];
                    if (parameter.ToLower().StartsWith("http"))
                    {
                        customUrl = parameter;
                    }
                    if (parameter.ToLower().StartsWith("debug"))
                    {
                        debug = true;
                    }
                }
            }

            // prevent clipping...
            if (SystemParameters.PrimaryScreenWidth <= 1024)
            {
                Width = 1024;
            }

            string tooltip;
            winPEVersion = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\WinPE", "Version", "");
            if (!string.IsNullOrWhiteSpace(winPEVersion))
            {
                tooltip = "WirelessConnect Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ", WinPE Version: " + winPEVersion;
            }
            else
            {
                tooltip = "WirelessConnect Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            label_Header.ToolTip = tooltip;
            Helper.PreventTouchToMousePromotion.Register(label_ManualConfiguration);

            var loaderImageName = string.Empty;
            foreach (var resource in Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                if (resource.EndsWith(".gif"))
                {
                    // "WirelessConnect.ajax-loader.gif"
                    loaderImageName = resource;
                }
            }
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(loaderImageName);
            var image = System.Drawing.Image.FromStream(stream);

            Loaded += (s, e) => {
                pictureBox_Initializing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                pictureBox_Initializing.BackColor = Color.Transparent;
                pictureBox_Initializing.Image = image;
            };

            // make sure WLAN AutoConfig Service is started (especially needed when running in WinPE)
            NetworkConfig.StartWirelessAutoConfigService();
            networkConfig = new NetworkConfig(customUrl);

            DataContext = connectionState;
            connectionState.InternetAccessable += ConnectionState_InternetAccessable;
            connectionState.CheckForInternetReady += ConnectionState_CheckForInternetReady;

            // construction of an individual delay timer for WinPE timing issues on first connect
            // now reduced to 100ms... in the next section we wait for wlan subsysten during a WinPE session
            firstConnect = new Timer((o) =>
            {
                firstConnectLock = false;
            });

            // disable all UI elements unitl network is specific delay
            listBox_Networks.IsEnabled = false;
            button_Refresh.IsEnabled = false;
            textBox_SecurityKey.IsEnabled = false;
            textBox_NetworkName.IsEnabled = false;
            comboBox_Authentication.IsEnabled = false;
            button_Continue.IsEnabled = false;
            button_ConnectDisconnect.IsEnabled = false;
            expander_ManualConfiguration.IsEnabled = false;
            label_ManualConfiguration.IsEnabled = false;
            label_SecurityKey_Clear.Visibility = Visibility.Collapsed;

            // trigger enable of UI elements after delay
            // different for WinPE session than Windows session
            // in WinPE measurements on a Surface 4 Pro showed a 60 seconds delay before the wlan subsystem was ready!
            var task = Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { Cursor = Cursors.Wait; }));

                // wait time for Windows Session
                var waitSeconds = 0.5F;
                if (!string.IsNullOrWhiteSpace(winPEVersion))
                {
                    // wait time for WinPE Session
                    if (!debug)
                    {
                        waitSeconds = 55;
                    }
                    else // prevent wait if debug command line argument is specified...
                    {
                        waitSeconds = 1;
                    }
                }
                Thread.Sleep((int)waitSeconds * 1000);

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    winFormHost.Visibility = Visibility.Collapsed;
                    button_Refresh_Click(null, null);
                    listBox_Networks.IsEnabled = true;
                    button_Refresh.IsEnabled = true;
                    button_Refresh.Content = "Refresh";
                    textBox_SecurityKey.IsEnabled = true;
                    textBox_NetworkName.IsEnabled = true;
                    comboBox_Authentication.IsEnabled = true;
                    button_Continue.IsEnabled = false;
                    button_ConnectDisconnect.IsEnabled = true;
                    expander_ManualConfiguration.IsEnabled = true;
                    label_ManualConfiguration.IsEnabled = true;
                    Cursor = Cursors.Arrow;
                }));

                StartNetworkAvailabilityBackgroundCheck();
            });
        }

        private void ConnectionState_CheckForInternetReady(object sender, EventArgs e)
        {
            if (!firstConnectLock)
            {
                firstConnectLock = false;

                    Task.Factory.StartNew(() =>
                    {
                        var waitBeforeCheckSeconds = 3;
                        Thread.Sleep(waitBeforeCheckSeconds * 1000);

                        try
                        {
                            if (NetworkConfig.IsInternetAvailable())
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    if (connectionState.InternetAccess != "Green")
                                        connectionState.InternetAccess = "Green";
                                }));
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    if (connectionState.InternetAccess != "Red")
                                        connectionState.InternetAccess = "Red";
                                }));
                            }
                        }
                        catch (Exception)
                        { }
                    });
            }
            else
            {
                var waitMiliSeconds = 100;
                if (!firstConnectTriggered)
                {
                    firstConnect.Change(waitMiliSeconds, Timeout.Infinite);
                    firstConnectTriggered = true;
                }
            }
        }

        private void ConnectionState_InternetAccessable(object sender, EventArgs e)
        {
            if (!button_Continue.IsEnabled)
            {
                button_Continue.IsEnabled = true;
            }
        }

        private void StartNetworkAvailabilityBackgroundCheck()
        {
            var seconds = 2;
            timer = new Timer((o) =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => { NetworkConnectivityBackgroundCheck(); }));
                }
                finally
                {
                    timer.Change(seconds * 1000, Timeout.Infinite);
                }
                
            });
            timer.Change(seconds * 1000, Timeout.Infinite);
        }

        private void NetworkConnectivityBackgroundCheck()
        {
            try
            {
                if (NetworkConfig.IsWiredNetworkAvailable())
                {
                    if (connectionState.Wired != "Green")
                    {
                        connectionState.Wired = "Green";
                        button_Continue.IsEnabled = true;
                    }
                    if (connectionState.Wired == "Green")
                        ConnectionState_CheckForInternetReady(null, null);

                    if (useWiredConnectionQuestion)
                    {
                        var result = MessageBox.Show("Wired network connection available\nDo you want to use the wired connection?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            Application.Current.Shutdown();
                        }
                        else
                        {
                            useWiredConnectionQuestion = false;
                        }
                    }
                }
                else
                {
                    if (connectionState.Wired != "Red")
                    {
                        connectionState.Wired = "Red";
                        connectionState.InternetAccess = "Red";
                        button_Continue.IsEnabled = false;
                    }
                    // would you like to see the choice everytime a network cable is plugged in, uncomment next line...
                    // otherwise it's displayed only at the first time
                    //useWiredConnection = true;
                }

                if (NetworkConfig.GetWirelessNetworkConnectionState())
                {
                    if (connectionState.Wireless != "Green")
                    {
                        connectionState.Wireless = "Green";

                        listBox_Networks.IsEnabled = false;
                        button_Refresh.IsEnabled = false;
                        if (!string.IsNullOrWhiteSpace(textBox_SecurityKey.Text))
                        {
                            currentSecurityKey = textBox_SecurityKey.Text;
                            textBox_SecurityKey.Text = "*********************";
                        }
                        textBox_SecurityKey.IsEnabled = false;
                        textBox_NetworkName.IsEnabled = false;
                        comboBox_Authentication.IsEnabled = false;
                        //button_Continue.IsEnabled = true;
                        button_ConnectDisconnect.Content = "Disconnect";
                        expander_ManualConfiguration.IsEnabled = false;
                        label_ManualConfiguration.IsEnabled = false;
                        button_Continue.Focus();
                        button_Continue.IsEnabled = true;
                        label_NetworkName_Clear.IsEnabled = false;
                        label_SecurityKey_Clear.IsEnabled = false;
                    }
                    if (connectionState.Wireless == "Green")
                        ConnectionState_CheckForInternetReady(null, null);
                }
                else
                {
                    if (connectionState.Wireless != "Red")
                    {
                        connectionState.Wireless = "Red";

                        button_ConnectDisconnect.Content = "Connect";
                        listBox_Networks.IsEnabled = true;
                        button_Refresh.IsEnabled = true;
                        if (!string.IsNullOrWhiteSpace(textBox_SecurityKey.Text))
                        {
                            textBox_SecurityKey.Text = currentSecurityKey;
                        }
                        textBox_SecurityKey.IsEnabled = true;
                        textBox_NetworkName.IsEnabled = true;
                        comboBox_Authentication.IsEnabled = true;
                        button_Continue.IsEnabled = false;
                        textBox_NetworkName.Text = string.Empty;
                        expander_ManualConfiguration.IsEnabled = true;
                        label_ManualConfiguration.IsEnabled = true;
                        label_NetworkName_Clear.IsEnabled = true;
                        label_SecurityKey_Clear.IsEnabled = true;
                    }
                }
            }
            catch (Exception) { }
        }

        private void button_ConnectDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            if (button_ConnectDisconnect.Content.ToString().Equals("disconnect", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    NetworkConfig.DisconnectWireless();
                    connectionState.Wireless = "Red";
                    connectionState.InternetAccess = "Red";
                    button_ConnectDisconnect.Content = "Connect";
                    listBox_Networks.IsEnabled = true;
                    button_Refresh.IsEnabled = true;
                    if (!string.IsNullOrWhiteSpace(textBox_SecurityKey.Text))
                    {
                        textBox_SecurityKey.Text = currentSecurityKey;
                    }
                    textBox_SecurityKey.IsEnabled = true;
                    textBox_NetworkName.IsEnabled = true;
                    comboBox_Authentication.IsEnabled = true;
                    if (connectionState.Wired == "Green")
                    {
                        button_Continue.IsEnabled = true;
                    }
                    else
                    {
                        button_Continue.IsEnabled = false;
                    }
                    textBox_NetworkName.Text = string.Empty;
                    expander_ManualConfiguration.IsEnabled = true;
                    label_ManualConfiguration.IsEnabled = true;
                    label_NetworkName_Clear.IsEnabled = true;
                    label_SecurityKey_Clear.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else // connect
            {
                try
                {
                    string connectResult = string.Empty;

                    if (wlan != null)
                    {
                        if (wlan.Authentication.Equals("WPA2-Enterprise", StringComparison.OrdinalIgnoreCase) || 
                            wlan.Authentication.Equals("WPA-Enterprise", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("No support for connection to WPA2-Enterprise networks!", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            Cursor = Cursors.Arrow;
                            return;
                        }
                    }

                    if (!manualConfigurationIsExpanded)
                    {
                        if (listBox_Networks.Items.Count > 0 && wlan != null)
                        {
                            if (!wlan.Authentication.Equals("open", StringComparison.OrdinalIgnoreCase) && 
                                string.IsNullOrWhiteSpace(textBox_SecurityKey.Text))
                            {
                                MessageBox.Show("Please type in a [Security Key] to connect!", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                Cursor = Cursors.Arrow;
                                return;
                            }
                            NetworkConfig.AddWirelessProfileToInterface(wlan.SSID, wlan.Authentication, textBox_SecurityKey.Text);
                            connectResult = NetworkConfig.ConnectToWireless(wlan.SSID);
                        }
                        else
                        {
                            MessageBox.Show("Please select a wireless network!", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            Cursor = Cursors.Arrow;
                            return;
                        }
                    }
                    else // Manual Configuration is expanded
                    {
                        if (!((ListBoxItem)comboBox_Authentication.SelectedItem).Content.ToString().Equals("open", StringComparison.OrdinalIgnoreCase) && 
                            string.IsNullOrWhiteSpace(textBox_SecurityKey.Text))
                        {
                            MessageBox.Show("Please type in a [Security Key] to connect!", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            Cursor = Cursors.Arrow;
                            return;
                        }
                        if (string.IsNullOrWhiteSpace(textBox_NetworkName.Text))
                        {
                            MessageBox.Show("Please type in a [Network Name] to connect!", "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            Cursor = Cursors.Arrow;
                            return;
                        }
                        NetworkConfig.AddWirelessProfileToInterface(
                            textBox_NetworkName.Text, 
                            ((ListBoxItem)comboBox_Authentication.SelectedItem).Content.ToString(), 
                            textBox_SecurityKey.Text);
                        connectResult = NetworkConfig.ConnectToWireless(textBox_NetworkName.Text);
                    }

                    if (connectResult != string.Empty) // error string returned from netsh connect...
                    {
                        MessageBox.Show(connectResult, "Information", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else // result == successfully!
                    {
                        connectionState.Wireless = "Green";
                        listBox_Networks.IsEnabled = false;
                        button_Refresh.IsEnabled = false;
                        if (!string.IsNullOrWhiteSpace(textBox_SecurityKey.Text))
                        {
                            currentSecurityKey = textBox_SecurityKey.Text;
                            textBox_SecurityKey.Text = "*********************";
                        }
                        textBox_SecurityKey.IsEnabled = false;
                        textBox_NetworkName.IsEnabled = false;
                        comboBox_Authentication.IsEnabled = false;
                        button_Continue.IsEnabled = true;
                        button_ConnectDisconnect.Content = "Disconnect";
                        expander_ManualConfiguration.IsEnabled = false;
                        label_ManualConfiguration.IsEnabled = false;
                        label_NetworkName_Clear.IsEnabled = false;
                        label_SecurityKey_Clear.IsEnabled = false;
                        button_Continue.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            Cursor = Cursors.Arrow;
        }

        private void button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            try
            {
                wlan = null;
                listBox_Networks.ItemsSource = networkConfig.GetWirelessNetworks();
                listBox_Networks.Items.Refresh();
                currentSsid = NetworkConfig.GetWirelessNetworkConnectedSsid();
                if (!string.IsNullOrWhiteSpace(currentSsid))
                {
                    var counter = 0;
                    foreach (var item in listBox_Networks.Items)
                    {
                        if (((WirelessNetwork)item).SSID == currentSsid)
                        {
                            break;
                        }
                        else
                        {
                            counter++;
                        }
                    }
                    listBox_Networks.SelectedIndex = counter;
                    listBox_Networks.IsEnabled = false;
                    button_Continue.IsEnabled = true;
                    button_ConnectDisconnect.Content = "Disconnect";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Cursor = Cursors.Arrow;
        }

        private void label_ManualConfiguration_TouchDown(object sender, TouchEventArgs e)
        {
            toggleExpander();
        }

        private void label_ManualConfiguration_MouseUp(object sender, MouseButtonEventArgs e)
        {
            toggleExpander();
        }

        private void toggleExpander()
        {
            if (manualConfigurationIsExpanded)
            {
                label_NetworkName.Visibility = Visibility.Hidden;
                textBox_NetworkName.Visibility = Visibility.Hidden;
                label_Encryption.Visibility = Visibility.Hidden;
                comboBox_Authentication.Visibility = Visibility.Hidden;
                manualConfigurationIsExpanded = false;
                expander_ManualConfiguration.IsExpanded = false;
            }
            else
            {
                label_NetworkName.Visibility = Visibility.Visible;
                textBox_NetworkName.Visibility = Visibility.Visible;
                label_Encryption.Visibility = Visibility.Visible;
                comboBox_Authentication.Visibility = Visibility.Visible;
                manualConfigurationIsExpanded = true;
                expander_ManualConfiguration.IsExpanded = true;
                textBox_NetworkName.Text = string.Empty;
            }
            Application.Current.MainWindow.UpdateLayout();
        }

        private void button_Continue_Click(object sender, RoutedEventArgs e)
        {
            timer?.Dispose();
            firstConnect?.Dispose();
            Application.Current.Shutdown();
        }

        private void listBox_Networks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            wlan = (WirelessNetwork)listBox_Networks.SelectedItem;
        }

        private void label_SecurityKey_Clear_MouseUp(object sender, MouseButtonEventArgs e)
        {
            textBox_SecurityKey.Text = string.Empty;
        }

        private void label_SecurityKey_Clear_TouchDown(object sender, TouchEventArgs e)
        {
            textBox_SecurityKey.Text = string.Empty;
        }

        private void textBox_SecurityKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox_SecurityKey.Text.Length > 0)
            {
                label_SecurityKey_Clear.Visibility = Visibility.Visible;
            }
            else
            {
                label_SecurityKey_Clear.Visibility = Visibility.Collapsed;
            }
        }

        private void label_NetworkName_Clear_TouchDown(object sender, TouchEventArgs e)
        {
            textBox_NetworkName.Text = string.Empty;
        }

        private void label_NetworkName_Clear_MouseUp(object sender, MouseButtonEventArgs e)
        {
            textBox_NetworkName.Text = string.Empty;
        }

        private void textBox_NetworkName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox_NetworkName.Text.Length > 0)
            {
                label_NetworkName_Clear.Visibility = Visibility.Visible;
            }
            else
            {
                label_NetworkName_Clear.Visibility = Visibility.Collapsed;
            }
        }

        private void button_OSK_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName="osk.exe" });
        }
    }
}
