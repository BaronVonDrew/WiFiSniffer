using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace WiFi_Sniffer
{
    public partial class Form1 : Form
    {

        NetStats ns = new NetStats();
        bool freeze = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Populates the DataTable and binds the object to the DataGridView as it's data source
            ns.PopulateTable();
            dataGridView1.DataSource = ns.GetDataTable();

            //Configures "Send" Chart
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 60;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 100;
            chart1.ChartAreas[0].AxisX.Interval = 10;
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(30, Color.Green);
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(30, Color.Green);
            chart1.ChartAreas[0].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chart1.ChartAreas[0].BorderColor = Color.Green;
            chart1.ChartAreas[0].BackColor = Color.FromArgb(20, Color.Green);

            //"Sent" data Configuration
            chart1.Series[0].Name = "Sent";
            chart1.Series[0].Color = Color.Green;
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[0].IsVisibleInLegend = false;
            chart1.Series[0].Points.DataBind(ns.GetSendTable().DefaultView, "Time", "Speed", null);

            //Configure "Received" Chart
            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Maximum = 60;
            chart2.ChartAreas[0].AxisY.Minimum = 0;
            chart2.ChartAreas[0].AxisY.Maximum = 100;
            chart2.ChartAreas[0].AxisX.Interval = 10;
            chart2.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(30, Color.Red);
            chart2.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(30, Color.Red);
            chart2.ChartAreas[0].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chart2.ChartAreas[0].BorderColor = Color.Red;
            chart2.ChartAreas[0].BackColor = Color.FromArgb(20, Color.Red);

            //Configure "Received" chart data
            chart2.Series[0].Name = "Received";
            chart2.Series[0].Color = Color.Red;
            chart2.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart2.Series[0].IsVisibleInLegend = false;
            chart2.Series[0].Points.DataBind(ns.GetRecvTable().DefaultView, "Time","Speed", null);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Closes the environment
            Environment.Exit(0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                //Saves current selection, and row index to be restored after updating table data
                int s = dataGridView1.FirstDisplayedScrollingRowIndex;
                int row = dataGridView1.CurrentCell.RowIndex;

                //Updates table data and restores previous configuration
                ns.PopulateTable();
                dataGridView1.DataSource = ns.GetDataTable();
                dataGridView1.CurrentCell = dataGridView1.Rows[row].Cells[0];
                dataGridView1.FirstDisplayedScrollingRowIndex = s;

                //Updates labels to display current and accurate data
                label1.Text = "Received: " + ns.GetReceived();
                label2.Text = "Sent: " + ns.GetSent();

                //Updates Graphing DataTables
                chart1.Series[0].Points.DataBind(ns.GetSendTable().DefaultView, "Time", "Speed", null);
                chart2.Series[0].Points.DataBind(ns.GetRecvTable().DefaultView, "Time", "Speed", null);
            }
            catch (Exception) { }
        }

        //# TIMER INTERVALS #
        private void secondToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            timer1.Interval = 1000;
        }

        private void secondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Interval = 5000;
        }

        private void secondsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timer1.Interval = 15000;
        }

        private void secondsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            timer1.Interval = 30000;
        }

        private void freezeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (freeze == false) { timer1.Stop(); freeze = true; freezeToolStripMenuItem.Text = "Unfreeze"; }
            else { timer1.Start(); freeze = false; freezeToolStripMenuItem.Text = "Freeze"; }
        }
        //# END TIMER INTERVALS #

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {

        }
    }
}
