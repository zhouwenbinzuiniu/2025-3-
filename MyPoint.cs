using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 随机抽样检测模拟
{
    internal class MyPoint
    {
        public string id;
        public double x;
        public double y;
        public double z;
        public MyPoint()
        {

        }
        public MyPoint(string id,double x,double y,double z)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
