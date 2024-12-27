using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kriging
{
    public class Cell
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Value { get; set; }
        public Cell(double x, double y, double value)
        {
            X = x;
            Y = y;
            Value = value;
        }
    }

    public class BlockModelParams
    {
        public double Xmin { get; set; }
        public double Xmax { get; set; }
        public double Ymin { get; set; }
        public double Ymax { get; set; }
        public double BlockSize { get; set; }
    }
    public class KrigingParams
    {
        public double Xmin, Xmax, Ymin, Ymax, BlockSize;
    }
}