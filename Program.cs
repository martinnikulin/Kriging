using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kriging
{
    class Program
    {
        static void Main(string[] args)
        {
            string ConnString = args[0];
            int SeamId = Int32.Parse(args[1]);
            int InterbedId = Int32.Parse(args[2]);
            int VarIndex = Int32.Parse(args[3]);
            int BlockSize = Int32.Parse(args[4]);
            string FileName = args[5];

            DataRepo.Init(ConnString, SeamId, InterbedId);
            var p =  DataRepo.GetParameters();
            p.BlockSize = BlockSize;

            BlockModel bm = new BlockModel(p);
            bm.CreateCells();
            if (bm.Cells.Count > 0)
            {
                List<Cell> points;
                string VarName = "";

                switch (VarIndex)
                {
                    case 1: VarName = "AdCoal"; break;
                    case 2: VarName = "AdFull"; break;
                    case 3: VarName = "ThCoal"; break;
                    case 4: VarName = "ThFull"; break;
                }

                points = DataRepo.GetPoints(VarName);
                bm.Fill(VarName, points);

                bm.Save(FileName);
            }
        }
    }

    public class HoleSeam
    {
        public double X, Y, Value;
    }
}
