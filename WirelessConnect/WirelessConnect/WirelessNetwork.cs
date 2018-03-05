using System.Collections.Generic;

namespace WirelessConnect
{
    public class WirelessNetwork
    {
        public string SSID { get; set; }
        public string NetworkType { get; set; }
        public string Authentication { get; set; }
        public string Encryption { get; set; }
        public List<Bss> BSS { get; set; }

        public override string ToString()
        {
            int signal = 0;
            foreach (var bss in BSS)
            {
                signal = bss.GetSignalInt();
            }
            return string.Format(@"{0,3}% - {1}", signal, SSID);
        }
        public WirelessNetwork()
        {
            BSS = new List<Bss>();
        }
    }

    public class Bss
    {
        public string ID { get; set; }
        public string Signal { get; set; }
        public string RadioType { get; set; }
        public string Channel { get; set; }
        public string BasicRatesMdps { get; set; }
        public string OtherRatesMbps { get; set; }

        public Bss()
        {
            Signal = "0";
        }
        public int GetSignalInt()
        {
            return int.Parse(Signal.Replace("%", ""));
        }
    }
}
