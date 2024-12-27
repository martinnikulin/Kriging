using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L = Science.Mathematics.LinearAlgebra;

namespace Kriging
{
       static class Kriging
    {
        static L.Matrix AG, AGI, AZ;
        static double[,] G;
        static readonly int threadCount = 50;
        static int pointsInThread;
        static int pointCount;
        static List<Cell> points;

        static public void Init(List<Cell> Points)
        {
            points = Points;
            G = new double[points.Count + 1, points.Count + 1];
            pointCount = points.Count;
            for (int i = 0; i <= points.Count - 1; i++)
            {
                for (int j = 0; j <= points.Count - 1; j++)
                {
                    G[j, i] = Math.Sqrt((points[i].X - points[j].X) * (points[i].X - points[j].X) + (points[i].Y - points[j].Y) * (points[i].Y - points[j].Y));
                };
                G[i, points.Count] = 1;
                G[points.Count, i] = 1;
            };
            AG = new L.Matrix(G);
            AGI = AG.Inverse;
        }
        static public void Compute(List<Cell> cells)
        {
            double[,] Z = new double[1, points.Count];

            for (int i = 0; i <= points.Count - 1; i++)
            {
                //if (points[i].Value != null)
                //{
                    Z[0, i] = (double)points[i].Value;
                //}
            };
            AZ = new L.Matrix(Z);

            pointsInThread = cells.Count / threadCount + 1;
            Parallel.For(0, threadCount + 1, m => CalcCollectionData(cells, m));
        }
        static private void CalcCollectionData(List<Cell> cells, int i)
        {
            int m = cells.Count;
            if ((i + 1) * pointsInThread < m)
            {
                m = (i + 1) * pointsInThread;
            };

            double[,] g = new double[pointCount + 1, 1];
            L.Matrix AResult;

            for (int j = i * pointsInThread; j < m; j++)
            {
                for (int k = 0; k < pointCount; k++)
                {
                    g[k, 0] = Math.Sqrt((points[k].X - cells[j].X) * (points[k].X - cells[j].X) + (points[k].Y - cells[j].Y) * (points[k].Y - cells[j].Y));
                };
                g[pointCount, 0] = 1;

                L.Matrix Ag = new L.Matrix(g);
                L.Matrix Alambda = (AGI * Ag);
                AResult = AZ * Alambda;
                cells[j].Value = AResult[0, 0];
            }
        }
    }
}
