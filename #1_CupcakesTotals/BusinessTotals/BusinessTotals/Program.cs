using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace BusinessTotals
{
    class Program
    {
        

        static void Main(string[] args)
        {
            DataSet DsTablesYear = new DataSet();
            DataSet DsTablesMonth = new DataSet();
            DataSet DsTablesWeek = new DataSet();


            string folder = Directory.GetCurrentDirectory();
            string fileTypes = "*.txt";

            string[] fileEntries = Directory.GetFiles(folder,fileTypes);

            foreach (var file in fileEntries)
            {
                
                double priceUnit = 0;


                
                string[] lines = System.IO.File.ReadAllLines(file);

                DataTable dtYear = new DataTable();
                dtYear.TableName = lines[0];
                dtYear.Columns.Add("Year", typeof(int));
                dtYear.Columns.Add("PriceUnit", typeof(double));
                dtYear.Columns.Add("TotalCupcakes", typeof(double));


                
                DataTable dtMonth = new DataTable();
                dtMonth.TableName = lines[0];
                dtMonth.Columns.Add("Month", typeof(int));
                dtMonth.Columns.Add("Year", typeof(int));
                dtMonth.Columns.Add("PriceUnit", typeof(double));
                dtMonth.Columns.Add("TotalCupcakes", typeof(double));
                


                DataTable dtWeek = new DataTable();
                dtWeek.TableName = lines[0] ;
                dtWeek.Columns.Add("BeginDate", typeof(DateTime));
                dtWeek.Columns.Add("EndDate", typeof(DateTime));
                dtWeek.Columns.Add("PriceUnit", typeof(double));
                dtWeek.Columns.Add("TotalCupcakes", typeof(double));
                



                DateTime day = new DateTime(2021,10,11);

                int lastYear = day.Year;
                int lastMonth = day.Month;
                DateTime lastDate = day;

                

                double totYear = 0;
                double totMonth = 0;
                double totWeek = 0;




                switch (lines[0])
                {
                    case "Basic Cupcake:":
                        priceUnit = 5;
                        break; 
                    case "Delux Cupcakes:":
                        priceUnit = 6;
                        break;
                    default:
                        priceUnit = 0;
                        break;
                }



                for (int i = lines.Length - 1; i >= 1; i--)
                {

                    //Week
                    if (day.DayOfWeek == 0 || i == 1)
                    {
                        dtWeek.Rows.Add(day.Date.AddDays(1), lastDate, priceUnit, totWeek.ToString());
                        lastDate = day;
                        totWeek = 0;
                        totWeek += Convert.ToDouble(lines[i]);
                    }
                    else
                    {
                        totWeek += Convert.ToDouble(lines[i]);
                    }


                    //month
                    if (day.Month != lastMonth || i == 1)
                    {
                        dtMonth.Rows.Add(lastMonth, lastYear, priceUnit, totMonth.ToString());
                        lastMonth = day.Month;
                        totMonth = 0;
                    }
                    else
                    {
                        totMonth += Convert.ToDouble(lines[i]);
                    }

                    //year
                    if (day.Year != lastYear || i == 1)
                    {
                        dtYear.Rows.Add(lastYear, priceUnit,totYear.ToString());
                        lastYear = day.Year;
                        totYear = 0;
                    }
                    else
                    {
                        totYear += Convert.ToDouble(lines[i]);
                    }

                    day = day.AddDays(-1);
                }

                DsTablesYear.Tables.Add(dtYear);
                DsTablesMonth.Tables.Add(dtMonth);
                DsTablesWeek.Tables.Add(dtWeek);

            }

            bool leave = false;

            while (!leave)
            {
                Console.WriteLine("---------------------------------------------------");
                Console.WriteLine("1 - By Year");
                Console.WriteLine("2 - By Month");
                Console.WriteLine("3 - By Week");
                Console.WriteLine("another key - leave");
                Console.WriteLine("---------------------------------------------------\n");


                switch (Convert.ToString(Console.ReadLine()))
                {
                    case "1":
                        ByYear(DsTablesYear);
                        break;
                    case "2":
                        ByMonth(DsTablesMonth);
                        break;
                    case "3":
                        ByWeek(DsTablesWeek);
                        break;
                    default:
                        leave = true;
                        break;
                }
                Console.ReadKey();
                Console.Clear();
            }
        }


        static void ByYear(DataSet dataSetYear)
        {
            Console.WriteLine("Year:\n");
            foreach (var year in dataSetYear.Tables[0].DefaultView.ToTable(true,"Year").Select())
            {
                double totCupcakes = 0;
                double totEarns = 0;

                Console.WriteLine(year["Year"]);
                foreach (DataTable tb in dataSetYear.Tables)
                {
                    double totCup = Convert.ToDouble(tb.Compute("SUM(TotalCupcakes)", "Year = '" + year["Year"] + "'"));
                    double price = Convert.ToDouble(tb.Select("Year = '" + year["Year"] + "'").FirstOrDefault()["PriceUnit"]);


                    Console.WriteLine(tb.TableName + " " + totCup.ToString() + " * " + price.ToString() + "$" + " = " + (totCup * price).ToString() + "$");
                    totCupcakes += totCup;
                    totEarns += totCup * price;
                }
                Console.WriteLine("Totals: " + totCupcakes + " Cupcakes       " + totEarns + "$\n");
            }
        }

        static void ByMonth(DataSet dataSetMonth)
        {
            Console.WriteLine("Month:\n");
            foreach (var _r in dataSetMonth.Tables[0].DefaultView.ToTable(true, "Month","Year").Select())
            {
                double totCupcakes = 0;
                double totEarns = 0;

                Console.WriteLine(_r["Month"] + "/" + _r["Year"]);
                foreach (DataTable tb in dataSetMonth.Tables)
                {
                    double totCup = Convert.ToDouble(tb.Compute("SUM(TotalCupcakes)", "Month = '" + _r["Month"] + "'AND Year = '" + _r["Year"] + "'"));
                    double price = Convert.ToDouble(tb.Select("Month = '" + _r["Month"] + "'AND Year = '" + _r["Year"] + "'").FirstOrDefault()["PriceUnit"]);


                    Console.WriteLine(tb.TableName + " " + totCup.ToString() + " * " + price.ToString() + "$" + " = " + (totCup * price).ToString() + "$");
                    totCupcakes += totCup;
                    totEarns += totCup * price;
                }
                Console.WriteLine("Totals: " + totCupcakes + " Cupcakes       " + totEarns + "$\n");
            }

        }

        static void ByWeek(DataSet dataSetWeek)
        {
            Console.WriteLine("Year:\n");
            foreach (var _r in dataSetWeek.Tables[0].DefaultView.ToTable(true, "BeginDate", "EndDate").Select())
            {
                double totCupcakes = 0;
                double totEarns = 0;

                Console.WriteLine("From " + Convert.ToDateTime(_r["BeginDate"]).ToString("dd/MM/yyyy") + " to " + Convert.ToDateTime(_r["EndDate"]).ToString("dd/MM/yyyy"));
                foreach (DataTable tb in dataSetWeek.Tables)
                {
                    double totCup = Convert.ToDouble(tb.Compute("SUM(TotalCupcakes)", "BeginDate = '" + _r["BeginDate"] + "' AND EndDate = '" + _r["EndDate"] + "'"));
                    double price = Convert.ToDouble(tb.Select("BeginDate = '" + _r["BeginDate"] + "' AND EndDate = '" + _r["EndDate"] + "'").FirstOrDefault()["PriceUnit"]);


                    Console.WriteLine(tb.TableName + " " + totCup.ToString() + " * " + price.ToString() + "$" + " = " + (totCup * price).ToString() + "$");

                    totCupcakes += totCup;
                    totEarns += totCup * price;
                }
                Console.WriteLine("Totals: " + totCupcakes + " Cupcakes       " + totEarns + "$\n");
            }
        }
    }
}
