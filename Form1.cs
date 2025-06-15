using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace 随机抽样检测模拟
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            intable();
        }
        DataTable table = new DataTable();
        MyAlog data = new MyAlog();
        void intable()
        {
            table.Clear();
            table.Columns.Add("id", typeof(string));
            table.Columns.Add("x", typeof(double));
            table.Columns.Add("y",typeof(double));
            table.Columns.Add("z", typeof(double));
        }
        private void 打开OToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "文本文件|*.txt";
            if (open.ShowDialog()==DialogResult.OK)
            {
                
                using(var sr=new StreamReader(open.FileName))
                {
                    while(!sr.EndOfStream)
                    {
                        data.DYnumber = double.Parse(sr.ReadLine());
                        while(!sr.EndOfStream)
                        {
                            var bvr = sr.ReadLine().Split(',');
                            MyPoint point = new MyPoint(bvr[0], double.Parse(bvr[1]), double.Parse(bvr[2]), double.Parse(bvr[3]));
                            data.Points.Add(point);
                        }
                    }
                }
            }
            foreach(var ite in data.Points)
            {
                DataRow rows = table.NewRow();
                rows["id"] = ite.id;
                rows["x"] = ite.x;
                rows["y"] = ite.y;
                rows["z"] = ite.z;
                table.Rows.Add(rows);
            }
            dataGridView1.DataSource = table;
        }

        private void 打印PToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = data.BG();
        }

        private void 剪切UToolStripButton_Click(object sender, EventArgs e)
        {
            Draw_Point();
        }
        public void Draw_Point()
        {
            // 原始点集
            Series chart = new Series("数据点");
            chart.ChartType = SeriesChartType.Point;
            chart.MarkerSize = 8;
            chart.MarkerStyle = MarkerStyle.Circle;
            chart.Color = System.Drawing.Color.Blue;
            foreach (var point in data.Points)
            {
                chart.Points.AddXY(point.x, point.y);
            }
            chart1.Series.Clear();
            chart1.Series.Add(chart);

        }
    }
}
