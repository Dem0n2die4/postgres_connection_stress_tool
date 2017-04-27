using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Npgsql;

namespace postgress_connection_stress_test_tool
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = ConfigurationManager.ConnectionStrings["database"].ConnectionString;

            ConsoleKeyInfo userInput = new ConsoleKeyInfo();
            bool inModeSelection = true;
            List<NpgsqlConnection> dbs = new List<NpgsqlConnection>();

            Console.WriteLine("try to connect to database...");
            try
            {
                NpgsqlConnection testDbConnect = new NpgsqlConnection(context);
                testDbConnect.Open();
                testDbConnect.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Can't connect to database");
                Console.ReadKey();
                return;
            }


            while (inModeSelection)
            {
                Console.WriteLine("Do you want to define connections count manually (M) or get all available connections(A)?");
                userInput = Console.ReadKey();

                switch(userInput.Key){
                    case ConsoleKey.M:
                    case ConsoleKey.A:
                        {
                            inModeSelection = false;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (userInput.Key == ConsoleKey.M)
            {
                Console.WriteLine("Please define connection count");
                var countInput = Console.ReadLine();
                var connectionCount = Int32.Parse(countInput);

                Console.WriteLine("try to connect...");

                for (int i = 0; i < connectionCount; ++i)
                {
                    try
                    {
                        NpgsqlConnection conn = new NpgsqlConnection(context);
                        conn.Open();
                        dbs.Add(conn);
                        Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Can't create more connections");
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("try to connect...");

                while (true)
                {
                    try
                    {
                        NpgsqlConnection conn = new NpgsqlConnection(context);
                        conn.Open();
                        dbs.Add(conn);
                        Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        break;
                    }

                }
            }


            Console.WriteLine(String.Format("{0} connections successful opened", dbs.Count));

            Console.WriteLine("waiting for interrupt...");
            Console.ReadKey();
            foreach (var connect in dbs)
            {
                connect.Close();
            }
        }
    }
}
