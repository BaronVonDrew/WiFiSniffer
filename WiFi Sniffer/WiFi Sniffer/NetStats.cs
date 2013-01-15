using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Data;
using System.Net;
using System.Threading;

namespace WiFi_Sniffer
{
    class NetStats
    {
        //Objects and instances used for moderation
        DataTable cons = new DataTable();
        IPGlobalProperties ipgp = IPGlobalProperties.GetIPGlobalProperties();
        static TcpConnectionInformation[] tcpi;
        TcpStatistics tcpstat;
        public static string HOST = "--";

        public NetStats()
        {
            //Configures the DataTable "cons" to hold four data columns.
            cons.Columns.Add("Local Port");
            cons.Columns.Add("Remote Address");
            cons.Columns.Add("Status");
        }

        public void PopulateTable()
        {
            //Renews TCP connection information. Clears DataTable "cons," ready for a new set of data.
            tcpi = ipgp.GetActiveTcpConnections();
            tcpstat = ipgp.GetTcpIPv4Statistics();
            cons.Rows.Clear();

            //Creates a Row in the DataTable "cons" displaying the connections remote properties / status.
            foreach (TcpConnectionInformation t in tcpi)
            {
                cons.Rows.Add(new Object[] { t.LocalEndPoint.Port, t.RemoteEndPoint.Address, t.State });
            }
        }

        public DataTable GetDataTable() { return cons; }

        public string GetReceived() {
            //Sets the amount of segments received to variable "rec"
            long rec = tcpstat.SegmentsReceived;

            ///Returns value "rec" in a string formatted to KB, MB, or GB depending on the amount.
            if ((rec / 1000) < 1000) { return Math.Round((rec / 1000.00), 2).ToString() + "KB"; }
            else if ((rec / 1000000) < 1000) { return Math.Round((rec / 1000000.00), 2).ToString() + "MB"; }
            else { return Math.Round((rec / (1000.00 * 1000000)), 2).ToString() + "GB"; }
        }

        public string GetSent() {
            //Sets the amount of data sent to variable "sent"
            long sent = tcpstat.SegmentsSent;

            //Formats and returns variable "sent" in KB/MB/GB format
            if ((sent / 1000) < 1000) { return Math.Round((sent / 1000.00),2).ToString() + "KB"; }
            else if ((sent / 1000000) < 1000) { return Math.Round((sent / 1000000.00),2).ToString() + "MB"; }
            else { return Math.Round((sent / (1000.00 * 1000000)),2).ToString() + "GB"; }
        }

        public static void GetHostName(int index)
        {
            try
            {
                //Attempts to resolve the address to a Host. If the host cannot be found, it returns "Unknown"
                //HOST = Dns.GetHostEntry(tcpi[index].RemoteEndPoint.Address).HostName.ToString();
                HOST = Dns.Resolve(tcpi[index].RemoteEndPoint.Address.ToString()).HostName.ToString();
            }
            catch (Exception) { HOST = "Unknown"; }
        }

        public int Count() { return tcpi.Length; }

        public int Established()
        {
            int num = 0;

            //Loops through all connections and counts the number of which are "Established"
            foreach (TcpConnectionInformation t in tcpi)
            {
                if (t.State.ToString().Equals("Established")) { num++; }
            }

            return num;
        }
    }
}
