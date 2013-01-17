using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Data;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace WiFi_Sniffer
{
    class NetStats
    {
        //Objects and instances used for moderation
        DataTable cons = new DataTable(), recv = new DataTable(), send = new DataTable();
        IPGlobalProperties ipgp = IPGlobalProperties.GetIPGlobalProperties();
        static TcpConnectionInformation[] tcpi;
        TcpStatistics tcpstat;
        IPStatHelper ipsh = new IPStatHelper();

        //Instance variables
        public static long RECV_TOTAL = 0;
        public static long SEND_TOTAL = 0;
        public static string HOST = "";

        public NetStats()
        {
            //Sets up the base for Send/Receive data calculations.
            tcpstat = ipgp.GetTcpIPv4Statistics();
            RECV_TOTAL = tcpstat.SegmentsReceived;
            SEND_TOTAL = tcpstat.SegmentsSent;

            //Configures the DataTable "cons" to hold four data columns.
            cons.Columns.Add("Process");
            cons.Columns.Add("ID");
            cons.Columns.Add("Local Port");
            cons.Columns.Add("Remote Address");
            cons.Columns.Add("Status");

            //Configuring DataTable "recv" to represent received data
            recv.Columns.Add("Time", Type.GetType("System.Int32"));
            recv.Columns.Add("Speed", Type.GetType("System.Int32"));
            recv.Rows.Add(0, 0);

            //Configuring DataTable "send" to represent received data
            send.Columns.Add("Time", Type.GetType("System.Int32"));
            send.Columns.Add("Speed", Type.GetType("System.Int32"));
            send.Rows.Add(0, 0);
        }

        public void PopulateTable()
        {
            //# UPDATES CONNECTION TABLE #
            //Renews TCP connection information. Clears DataTable "cons," ready for a new set of data.
            tcpi = ipgp.GetActiveTcpConnections();
            tcpstat = ipgp.GetTcpIPv4Statistics();
            cons.Rows.Clear();

            //Creates a Row in the DataTable "cons" displaying the connections remote properties / status.
            foreach (TcpConnectionInformation t in tcpi)
            {
                //Fetches process according to ports
                Process pro = ipsh.GetProcessByPort(t.LocalEndPoint.Port);
                cons.Rows.Add(new Object[] { pro.ProcessName, pro.Id, t.LocalEndPoint.Port, t.RemoteEndPoint.Address, t.State });
            }
            //# END CONNECTION TABLE #
            //# START RECV TABLE UPDATE #
            DataTable tmp = new DataTable();
            tmp.Columns.Add("Time", Type.GetType("System.Int32"));
            tmp.Columns.Add("Speed", Type.GetType("System.Int32"));

            tmp.Rows.Add(0,(tcpstat.SegmentsReceived - RECV_TOTAL));

            for (int a = 0; a < 59; a++)
            {
                try
                {
                    DataRow dr = recv.Rows[a];
                    int time = Convert.ToInt32(dr.ItemArray[0].ToString());
                    tmp.Rows.Add(new Object[] { time++, recv.Rows[a].ItemArray[1] });
                }
                catch (Exception) { break; }
            }
            RECV_TOTAL = tcpstat.SegmentsReceived;
            recv = tmp;
            //# END RECV TABLE UPDATE #
            //# START SEND TABLE UPDATE #
            DataTable snd = new DataTable();
            snd.Columns.Add("Time", Type.GetType("System.Int32"));
            snd.Columns.Add("Speed", Type.GetType("System.Int32"));

            snd.Rows.Add(0,(tcpstat.SegmentsSent - SEND_TOTAL));

            for (int a = 0; a < 59; a++)
            {
                try
                {
                    DataRow dr = send.Rows[a];
                    int time = Convert.ToInt32(dr.ItemArray[0].ToString());
                    snd.Rows.Add(new Object[] { time++, send.Rows[a].ItemArray[1] });
                }
                catch (Exception) { break; }
            }
            SEND_TOTAL = tcpstat.SegmentsSent;
            send = snd;
        }

        public DataTable GetRecvTable() { return recv; }

        public DataTable GetSendTable() { return send; }

        public DataTable GetDataTable() { return cons; }

        public string GetReceived() {
            //Sets the amount of segments received to variable "rec"
            tcpstat = ipgp.GetTcpIPv4Statistics();
            long rec = tcpstat.SegmentsReceived;

            RECV_TOTAL = rec;

            //Returns value "rec" in a string formatted to KB, MB, or GB depending on the amount.
            return FormatBytes(rec);
        }

        public string GetSent() {
            //Sets the amount of data sent to variable "sent"
            tcpstat = ipgp.GetTcpIPv4Statistics();
            long sent = tcpstat.SegmentsSent;

            //Returns amount of data sent formatted by method FormatBytes(long amt);
            return FormatBytes(sent);
        }

        //Formats bytes to KB/MB/GB format automatically.
        public string FormatBytes(long amt)
        {
            if ((amt / 1000) < 1000) { return Math.Round((amt / 1000.00), 2).ToString() + "KB"; }
            else if ((amt / 1000000) < 1000) { return Math.Round((amt / 1000000.00), 2).ToString() + "MB"; }
            else { return Math.Round((amt / (1000.00 * 1000000)), 2).ToString() + "GB"; }
        }

        public static void GetHostName(int index)
        {
            try
            {
                //Attempts to resolve the address to a Host. If the host cannot be found, it returns "Unknown"
                HOST = Dns.GetHostEntry(tcpi[index].RemoteEndPoint.Address).HostName.ToString();
                //HOST = Dns.Resolve(tcpi[index].RemoteEndPoint.Address.ToString()).HostName.ToString();
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
