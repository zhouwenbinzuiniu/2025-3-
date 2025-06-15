using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace 随机抽样检测模拟
{
    internal class MyAlog
    {
        public List<MyPoint> Points = new List<MyPoint>();
        public double DYnumber;
        public List<MyPoint> [,]SGPoints ;
        StringBuilder read = new StringBuilder();
        

        public string BG()
        {
            var xmin = Points.OrderBy(t => t.x).First().x;
            var xmax = Points.OrderBy(t => t.x).Last().x;
            var ymin = Points.OrderBy(t => t.y).First().y;
            var ymax = Points.OrderBy(t => t.y).Last().y;
            var zmin = Points.OrderBy(t => t.z).First().z;
            var zmax = Points.OrderBy(t => t.z).Last().z;
            var data3 = Points.OrderBy(t => t.z);
            

            read.AppendLine($"--------------------------");
            
            read.AppendLine($"{Points[4].id}坐标分量x:{Points[4].x}");
            read.AppendLine($"{Points[4].id}坐标分量y:{Points[4].y}");
            read.AppendLine($"{Points[4].id}坐标分量z:{Points[4].z}");
            read.AppendLine($"坐标分量x的最小值:{xmin}");
            read.AppendLine($"坐标分量x的最小值:{xmax}");
            read.AppendLine($"坐标分量x的最小值:{ymin}");
            read.AppendLine($"坐标分量x的最小值:{ymax}");
            read.AppendLine($"坐标分量x的最小值:{zmin}");
            read.AppendLine($"坐标分量x的最小值:{zmax}");

            initSG();
            CalSGdata();
            RANSAC();


            return read.ToString();
        }
        /// <summary>
        /// 初始
        /// </summary>
        public void initSG()
        {
            SGPoints = new List<MyPoint>[10, 10];
            for (int i=0;i<10;i++)
            {
                for(int j=0;j<10;j++)
                {
                    SGPoints[i, j] = new List<MyPoint> ();
                }
            }
        }
        
        /// <summary>
        /// 计算栅格中的数据
        /// </summary>
        public void CalSGdata()
        {
            
            int dx = 10;
            int dy = 10;
            for(int i=0;i<Points.Count;i++)
            {
                int i_sg = (int)(Points[i].y / dy);
                int j_sg = (int)(Points[i].x / dx);
                SGPoints[i_sg, j_sg].Add(Points[i]);
            }
            var data = Points.Select(t => new { id=t.id,z=t.z,x = t.x, y = t.y, SGY = (int)t.y / dy, SGX = (int)t.x / dx }).ToList();
            var a=data.Where(t => t.id == "P5").First();
            read.AppendLine($"{a.id}所在的栅格行i:{a.SGX}");
            read.AppendLine($"{a.id}所在的栅格行j:{a.SGY}");
            read.AppendLine($"C中的点云数量{SGPoints[2, 3].Count}");
            var Csum = SGPoints[2, 3].Count;
            double Cz = SGPoints[2, 3].Sum(t => t.z);
            double Chighmax = SGPoints[2, 3].OrderBy(t => t.z).Last().z;
            double Chighmin = SGPoints[2, 3].OrderBy(t => t.z).First().z;
            var high = (double)(Cz/Csum);
            read.AppendLine($"C中的平均高度:{high:F6}");
            read.AppendLine($"C中的最大高度:{Chighmax}");
            read.AppendLine($"C中的高度差:{Chighmax - Chighmin}");
            var CFC = SGPoints[2, 3].Sum(t => Math.Pow(t.z - high, 2))/Csum;
            read.AppendLine($"C中的方差:{CFC:F6}");
            



        }
        public void RANSAC()
        {
            var area = Triangelare(Points[0], Points[1], Points[3]);
            read.AppendLine($"p1-p2-p3的平面面积:{area}");
            var data = CalXS(Points[0], Points[1], Points[2]);
            read.AppendLine($"参数A:{data.A}");
            read.AppendLine($"参数B;{data.B}");
            read.AppendLine($"参数C:{data.C}");
            read.AppendLine($"参数D:{data.D}");
            read.AppendLine($"P1000到平面距离:{PMDistance(Points[0], Points[1], Points[2], Points[Points.Count - 1])}");
            read.AppendLine($"P5到平面距离:{PMDistance(Points[0], Points[1], Points[2], Points[4])}");
            var NbPoint = new List<MyPoint>();
            var VbPoint = new List<MyPoint>();
            for(int i=3;i<Points.Count;i++)
            {
                double d = PMDistance(Points[0], Points[1], Points[2], Points[i]);
                if(d<0.1)
                {
                    NbPoint.Add(Points[i]);
                }
                else
                {
                    VbPoint.Add(Points[i]);
                }
            }
            read.AppendLine($"内部点的数量:{NbPoint.Count}");
            read.AppendLine($"外部点的数量:{VbPoint.Count}");
            int MaxPoint = 0;
            
            for (int i=0;i<300;i++)
            {
                var MaxPoint1 = new List<MyPoint>();
                for (int k = 0; k < Points.Count; k++)
                {
                    if (k == i * 3 || k == i * 3 + 1 || k == i * 3 + 2) { continue; }
                    double d = PMDistance(Points[i * 3], Points[i * 3 + 1], Points[i * 3 + 2], Points[k]);
                    if (d < 0.1)
                    {
                        MaxPoint1.Add(Points[k]);
                    }

                }
                if (MaxPoint < MaxPoint1.Count)
                {
                    MaxPoint = MaxPoint1.Count;
                }
            }
            read.AppendLine($"最佳平面分割点:{997-MaxPoint}");
        }
        /// <summary>
        /// 计算三角形面积
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public double Triangelare(MyPoint p1,MyPoint p2,MyPoint p3)
        {
            double a = distance(p1, p2);
            double b = distance(p2, p3);
            double c = distance(p1, p3);
            double p = (a + b + c) / 2;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }
        /// <summary>
        /// 计算点到平面距离(二维的）
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double distance(MyPoint p1,MyPoint p2)
        {
            double dx = p1.x - p2.x;
            double dy = p1.y - p2.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        /// <summary>
        /// 计算点到平面距离（三维的)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        public double PMDistance(MyPoint p1,MyPoint p2,MyPoint p3,MyPoint p4)
        {
            var data = CalXS(p1, p2, p3);
            var A = data.A;
            var B = data.B;
            var C = data.C;
            var D = data.D;
            double over = Math.Abs(A * p4.x + B * p4.y + C * p4.z + D);
            double under = Math.Sqrt(A * A + B * B + C * C);
            return over / under;
        }
        /// <summary>
        /// 计算平面系数
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="P3"></param>
        /// <returns></returns>
        public (double A, double B, double C, double D) CalXS(MyPoint P1,MyPoint P2,MyPoint P3)
        {
            double a = (P2.y - P1.y) * (P3.z - P1.z) - (P3.y - P1.y) * (P2.z - P1.z);
            double b = (P2.z - P1.z) * (P3.x - P1.x) - (P3.z - P1.z) * (P2.x - P1.x);
            double c = (P2.x - P1.x) * (P3.y - P1.y) - (P3.x - P1.x) * (P2.y - P1.y);
            double d = (-1 * a * P1.x) - (b * P1.y) - (c * P1.z);
            return (a, b, c, d);
        }
        
        
       
    }
}
