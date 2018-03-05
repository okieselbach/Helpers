using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Security;
using System.Text;
using System.Threading;

namespace WirelessConnect
{
    public class NetworkConfig
    {
        private static string urlSwitch;
        private List<WirelessNetwork> wirelessNetworks = new List<WirelessNetwork>();

        public NetworkConfig()
        {
            urlSwitch = string.Empty;
        }

        public NetworkConfig(string url)
        {
            urlSwitch = url;
        }

        public static void AddWirelessProfileToInterface(string ssid, string authentication, string secretKey)
        {
            try
            {
                var wiFiProfile =
@"<?xml version=""1.0""?>
<WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1"">
  <name>{NAME}</name>
  <SSIDConfig>
    <SSID>
      <hex>{SSID-HEX}</hex>
      <name>{SSID-NAME}</name>
    </SSID>
  </SSIDConfig>
  <connectionType>ESS</connectionType>
  <connectionMode>manual</connectionMode>
  <MSM>
    <security>
      <authEncryption>
        <authentication>{AUTHENTICATION}</authentication>
        <encryption>{ENCRYPTION}</encryption>
        <useOneX>false</useOneX>
      </authEncryption>{SHAREDKEY}
    </security>
  </MSM>
  <MacRandomization xmlns=""http://www.microsoft.com/networking/WLAN/profile/v3"">
    <enableRandomization>false</enableRandomization>
  </MacRandomization>
</WLANProfile>";

                var wiFiProfileSharedKey =
@"
      <sharedKey>
        <keyType>passPhrase</keyType>
        <protected>false</protected>
        <keyMaterial>{KEYMATERIAL}</keyMaterial>
      </sharedKey>";

                // convert values... scanned values are *-personal and dropdown list values are plein WPA*...
                if (authentication.ToLower() == "wpa2-personal" || authentication.ToLower() == "wpa2") { authentication = "WPA2PSK"; }
                if (authentication.ToLower() == "wpa-personal" || authentication.ToLower() == "wpa") { authentication = "WPAPSK"; }
                if (authentication.ToLower() == "open") { authentication = "open"; }

                wiFiProfile = wiFiProfile.Replace("{NAME}", SecurityElement.Escape(ssid));
                wiFiProfile = wiFiProfile.Replace("{SSID-HEX}", ConvertToHex(ssid));
                wiFiProfile = wiFiProfile.Replace("{SSID-NAME}", SecurityElement.Escape(ssid));
                wiFiProfile = wiFiProfile.Replace("{AUTHENTICATION}", authentication);
                wiFiProfile = wiFiProfile.Replace("{ENCRYPTION}", authentication.ToLower() == "open" ? "none" : "AES");
                wiFiProfileSharedKey = wiFiProfileSharedKey.Replace("{KEYMATERIAL}", SecurityElement.Escape(secretKey));
                if (authentication.ToLower() == "open")
                {
                    wiFiProfile = wiFiProfile.Replace("{SHAREDKEY}", "");
                }
                else
                {
                    wiFiProfile = wiFiProfile.Replace("{SHAREDKEY}", wiFiProfileSharedKey);
                }

                var wiFiProfilePath = Environment.ExpandEnvironmentVariables("%temp%") + @"\Wi-Fi-Net.xml";

                File.WriteAllText(wiFiProfilePath, wiFiProfile);

                var result = ExecuteEx("netsh", "wlan add profile filename=\"" + wiFiProfilePath + "\"");
                File.WriteAllText(Environment.ExpandEnvironmentVariables("%temp%") + @"\Wi-Fi-Net-debug.log", result);

                //File.Delete(wiFiProfilePath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string ExecuteEx(string command, string arguments)
        {
            try
            {
                var sr = Process.Start(new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }).StandardOutput;

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadToEnd();
                    return line;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string ConnectToWireless(string ssid)
        {
            try
            {
                var sr = Process.Start(new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "wlan connect name=\"" + ssid + "\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }).StandardOutput;

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line.ToLower().Contains("successfully"))
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return line;
                    }
                }
                return string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool GetWirelessNetworkConnectionState()
        {
            try
            {
                var sr = Process.Start(new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "wlan show interfaces",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }).StandardOutput;

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadToEnd();
                    if (line.ToLower().Contains(": connected"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void DisconnectWireless()
        {
            Execute("netsh", "wlan disconnect");
        }

        public List<WirelessNetwork> GetWirelessNetworks()
        {
            try
            {
                wirelessNetworks.Clear();

                var sr = Process.Start(new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "wlan show networks mode=bssid",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }).StandardOutput;

                // example output:
                //
                //Interface name : Wi - Fi
                //There are 12 networks currently visible.
                //
                //SSID 3 : ZGxoNG1vYmlsZQ
                //    Network type            : Infrastructure
                //    Authentication          : WPA2 - Enterprise
                //    Encryption              : CCMP
                //    BSSID 1                 : 00:11:20:70:7e:71
                //         Signal             : 62 %
                //         Radio type         : 802.11g
                //         Channel            : 11
                //         Basic rates (Mbps) : 1 2 5.5 11
                //         Other rates (Mbps) : 6 9 12 18 24 36 48 54
                //    BSSID 2                 : 00:11:20:70:83:01
                //         Signal             : 100 %
                //         Radio type         : 802.11g
                //         Channel            : 11
                //         Basic rates (Mbps) : 1 2 5.5 11
                //         Other rates (Mbps) : 6 9 12 18 24 36 48 54

                // skip first 4 lines
                for (int i = 0; i < 4; i++)
                {
                    sr.ReadLine();
                }
                var wirelessNetwork = new WirelessNetwork();

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var property = line.Trim().ToLower();

                    if (line.Trim().ToLower().StartsWith("ssid "))
                    {
                        var prop = line.Split(':')[1].TrimStart();
                        if (!string.IsNullOrWhiteSpace(prop)) wirelessNetwork.SSID = prop;
                    }

                    if (line.Trim().ToLower().StartsWith("network type "))
                    {
                        var prop = line.Split(':')[1].TrimStart();
                        if (!string.IsNullOrWhiteSpace(prop)) wirelessNetwork.NetworkType = prop;
                    }

                    if (line.Trim().ToLower().StartsWith("authentication "))
                    {
                        var prop = line.Split(':')[1].TrimStart();
                        if (!string.IsNullOrWhiteSpace(prop)) wirelessNetwork.Authentication = prop;
                    }

                    if (line.Trim().ToLower().StartsWith("encryption "))
                    {
                        var prop = line.Split(':')[1].TrimStart();
                        if (!string.IsNullOrWhiteSpace(prop)) wirelessNetwork.Encryption = prop;
                    }

                    if (line.Trim().ToLower().StartsWith("bssid "))
                    {
                        var bss = new Bss();
                        bss.ID = line.TrimStart().Split(' ')[1];

                        while (!sr.EndOfStream)
                        {
                            line = sr.ReadLine();

                            if (line.Trim().ToLower().StartsWith("bssid "))
                            {
                                wirelessNetwork.BSS.Add(bss);
                                bss = new Bss();
                                bss.ID = line.TrimStart().Split(' ')[1];
                                continue;
                            }

                            if (line.Trim().ToLower().StartsWith("signal "))
                            {
                                var prop = line.Split(':')[1].Trim();
                                if (!string.IsNullOrWhiteSpace(prop))
                                {
                                    bss.Signal = prop;
                                    continue;
                                }
                            }

                            if (line.Trim().ToLower().StartsWith("radio type "))
                            {
                                var prop = line.Split(':')[1].Trim();
                                if (!string.IsNullOrWhiteSpace(prop))
                                {
                                    bss.RadioType = prop;
                                    continue;
                                }
                            }

                            if (line.Trim().ToLower().StartsWith("channel "))
                            {
                                var prop = line.Split(':')[1].Trim();
                                if (!string.IsNullOrWhiteSpace(prop))
                                {
                                    bss.Channel = prop;
                                    continue;
                                }
                            }

                            if (line.Trim().ToLower().StartsWith("basic rates (mbps) "))
                            {
                                var prop = line.Split(':')[1].Trim();
                                if (!string.IsNullOrWhiteSpace(prop))
                                {
                                    bss.BasicRatesMdps = prop;
                                    continue;
                                }
                            }

                            if (line.Trim().ToLower().StartsWith("other rates (mbps) "))
                            {
                                var prop = line.Split(':')[1].Trim();
                                if (!string.IsNullOrWhiteSpace(prop))
                                {
                                    bss.OtherRatesMbps = prop;
                                    continue;
                                }
                            }

                            if (string.IsNullOrWhiteSpace(line))
                            {
                                wirelessNetwork.BSS.Add(bss);
                                break;
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        wirelessNetworks.Add(wirelessNetwork);
                        wirelessNetwork = new WirelessNetwork();
                    }
                }
                return wirelessNetworks;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool IsWiredNetworkAvailable()
        {
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    // discard because of standard reasons
                    if ((ni.OperationalStatus != OperationalStatus.Up) ||
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                        continue;

                    // min 10 Mbit/s, this allow to filter modems, serial, etc.
                    if (ni.Speed < 10 * 1000 * 1000)
                        continue;

                    // discard virtual cards (virtual box, virtual pc, etc.)
                    if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                        continue;

                    // discard wireless cards
                    if ((ni.Name.IndexOf("wi-fi", StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (ni.Name.IndexOf("wireless", StringComparison.OrdinalIgnoreCase) >= 0))
                        continue;

                    // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                    if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // discard if not gateway ip is assigned
                    if (ni.GetIPProperties().GatewayAddresses.Count == 0)
                        continue;

                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetWirelessNetworkConnectedSsid()
        {
            try
            {
                var sr = Process.Start(new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = "wlan show interfaces",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }).StandardOutput;

                while (!sr.EndOfStream)
                {
                    var connected = false;
                    var line = sr.ReadLine();
                    if (line.ToLower().Trim().StartsWith("state "))
                    {
                        var state = line.Split(':')[1].Trim();
                        if (state.Equals("connected", StringComparison.OrdinalIgnoreCase))
                        {
                            connected = true;
                        }
                        if (connected)
                        {
                            line = sr.ReadLine();
                            if (line.ToLower().Trim().StartsWith("ssid "))
                            {
                                var ssid = line.Split(':')[1].Trim();
                                return ssid;
                            }
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return string.Empty;
        }

        public static bool IsInternetAvailable()
        {
            // ignore certificate errors...
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            // adjust to your needs if a TLS site is used for verification..
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var contentCompare = true;
            var contentToCompare = "Microsoft NCSI";
            var urlToCheck = "http://www.msftncsi.com/ncsi.txt";
            if (!string.IsNullOrWhiteSpace(urlSwitch))
            {
                urlToCheck = urlSwitch;
                contentCompare = false;
            }

            bool returnValue = false;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlToCheck);
                var timeout = 2;
                request.Timeout = timeout * 1000;
                request.ReadWriteTimeout = timeout * 1000;
                var webResponse = (HttpWebResponse)request.GetResponse();
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    if (contentCompare)
                    {
                        using (var sr = new StreamReader(webResponse.GetResponseStream()))
                        {
                            if (sr.ReadLine().Equals(contentToCompare, StringComparison.OrdinalIgnoreCase))
                            {
                                returnValue = true;
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

            return returnValue;
        }

        public static void StartWirelessAutoConfigService()
        {
            // start WLAN AutoConfig Service in WinPE
            Execute("net", "start wlansvc");
        }

        private static Process Execute(string filename, string arguments)
        {
            try
            {
                return Process.Start(new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string ConvertToHex(string text)
        {
            var byteText = Encoding.ASCII.GetBytes(text);
            var hexText = string.Empty;
            foreach (var c in byteText)
            {
                hexText += string.Format("{0:X2}", c);
            }
            return hexText;
        }
    }
}
