using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kriging
{
    public static class DataRepo
    {
        private static string ConnString;
        private static int SeamId;
        private static int InterbedId;
        public static List<Cell> Points;

        public static void Init(string connString, int seamId, int interbedId)
        {
            ConnString = connString;
            SeamId = seamId;
            InterbedId = interbedId;
        }

        public static BlockModelParams GetParameters()
        {
            using (SqlConnection connection = new SqlConnection(ConnString))
            {
                connection.Open();
                string sql = "select ParamName, ParamValue from Settings where Section = 'BlockModel'";
                DataTable dt = GetTable(connection, sql);

                var p = new BlockModelParams();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["ParamName"].ToString() == "Xmin")
                        p.Xmin = Convert.ToDouble(row["ParamValue"]);
                    if (row["ParamName"].ToString() == "Xmax")
                        p.Xmax = Convert.ToDouble(row["ParamValue"]);
                    if (row["ParamName"].ToString() == "Ymin")
                        p.Ymin = Convert.ToDouble(row["ParamValue"]);
                    if (row["ParamName"].ToString() == "Ymax")
                        p.Ymax = Convert.ToDouble(row["ParamValue"]);
                }
                return p;
            }
        }

        public static List<Cell> GetPoints(string varName)
        {
            using (SqlConnection connection = new SqlConnection(ConnString))
            {
                string sql = "select X, Y, " + varName + " from fHoleSeamValues(" + SeamId.ToString() + ", " +
                    InterbedId.ToString() + ") where not " + varName + " is null";

                connection.Open();
                DataTable dt = GetTable(connection, sql);

                List<Cell> points = new List<Cell>();
                foreach (DataRow row in dt.AsEnumerable())
                {
                    Cell point = new Cell(row.Field<double>("X"), row.Field<double>("Y"), Convert.ToDouble(row[varName]));
                    points.Add(point);
                }
                return points;
            }
        }

        public static List<Cell> GetCells()
        {
            using (SqlConnection connection = new SqlConnection(ConnString))
            {
                connection.Open();

                string sql = "select X, Y from Points p ";
                            //"inner join  PlanGeoms pg on " +
                            //"pg.FeatureType = 1 and " +
                            //"pg.SeamId = " + SeamId.ToString() + " and " +
                            //"pg.InterbedId = " + InterbedId.ToString(); //" + " and " +
                            //"pg.Geom.STIntersects(p.Geom) = 1";

                DataTable dt = GetTable(connection, sql);

                List<Cell> cells = new List<Cell>();

                foreach (DataRow row in dt.AsEnumerable())
                {
                    Cell cell = new Cell(row.Field<double>("X"), row.Field<double>("Y"), Double.NaN);
                    cells.Add(cell);
                }
                return cells;
            } 
        }

        public static void FillSeamPoints(string bmfileName, string fmtFileName)
        {
            string sql = "delete from SeamPoints where SeamId = " + SeamId.ToString() + " and InterbedId = " + InterbedId.ToString() + ";";
            sql = sql +
            "insert into SeamPoints (SeamId, InterbedId, X, Y, Z, AdCoal, AdFull, ThCoal, ThFull, Geom) " +
            "select SeamId, InterbedId, X, Y, Z, AdCoal, AdFull, ThCoal, ThFull, geometry::Point(X, Y, 0) as Geom " +
            "from openrowset(bulk '" + bmfileName + @"', formatfile = '" + fmtFileName + "') t";
            ExecSQL(sql);
        }

        private static DataTable GetTable(SqlConnection connection, string sql)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            return dataSet.Tables[0];
        }

        private static void ExecSQL(string sql)
        {
            using (SqlConnection connection = new SqlConnection(ConnString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static double[,] ConvertToArray(DataTable dt)
        {
            var rows = dt.Rows;
            int rowCount = rows.Count;
            int colCount = dt.Columns.Count;
            var result = new double[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
            {
                var row = rows[i];
                for (int j = 0; j < colCount; j++)
                {
                    result[i, j] = (double)row[j];
                }
            }
            return result;
        }
    }
}
