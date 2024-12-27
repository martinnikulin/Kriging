using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kriging
{
    class BlockModel
    {
        private Stream filename;

        public double BlockSize { get; set; }
        public int NX { get; set; }
        public int NY { get; set; }
        public double Xmin { get; set; }
        public double Xmax { get; set; }
        public double Ymin { get; set; }
        public double Ymax { get; set; }
        public List<Cell> Cells { get; set; }

        public BlockModel(BlockModelParams p)
        {
            BlockSize = p.BlockSize;
            Xmin = p.Xmin;
            Xmax = p.Xmax;
            Ymin = p.Ymin;
            Ymax = p.Ymax;

            NX = (int)((Xmax - Xmin) / BlockSize) + 1;
            NY = (int)((Ymax - Ymin) / BlockSize) + 1;

            Cells = new List<Cell>();
        }
        public void CreateCells()
        {
            try
            {
                Cells = DataRepo.GetCells();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Fill(string VarName, List<Cell> Points)
        {
            Kriging.Init(Points);
            Kriging.Compute(Cells);
        }
        public void Save(string FileName)
        {
            //string strWorkPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //Path.GetTempPath() + @"\bm.txt";          
            //string fmtFileName = @"G:\SeamPointsImport.fmt"; //System.IO.Path.Combine(strWorkPath, "SeamPointsImport.fmt");
            //DataRepo.FillSeamPoints(bmFileName, fmtFileName);

            using (System.IO.StreamWriter file = new StreamWriter(FileName, false, Encoding.GetEncoding("Windows-1251")))
            {
                file.WriteLine(Xmin.ToString() + ";" + Ymin.ToString());
                file.WriteLine(Xmax.ToString() + ";" + Ymax.ToString());
                file.WriteLine(BlockSize.ToString());
                file.WriteLine(NX.ToString() + ";" + NY.ToString());

                foreach (Cell cell in Cells)
                {
                    file.WriteLine(cell.X.ToString() + ";" + cell.Y.ToString() + ";" + cell.Value.ToString());
                }
            }

        }
    }
}