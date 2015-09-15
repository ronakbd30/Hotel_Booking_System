using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
namespace HotelBookingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            String path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);//for storing output file on current users desktop.
            FileStream fs = new FileStream(path+"\\TeamLogicAssgn2Output.txt", FileMode.Create);            
            StreamWriter sw = new StreamWriter(fs);
            Console.WriteLine("Main-> Program Started..Writing Output to File at" +path+"\\TeamLogicAssgn2Output.txt..Takes 2-3 mins, since we have used price cuts as:5 and the generated prices depends on random function..... Please Wait");

            TextWriter tmp = Console.Out;// First, save the standard output.
            Console.SetOut(sw);

            DateTime start = System.DateTime.Now;
            Console.WriteLine("Main-> Program Start Time:{0}",start);
            ArrayList threadlist = new ArrayList();
            Hotel []hotel=new Hotel[3];//3 hotel objects
            TravelAgency[] ta = new TravelAgency[5];//5 travel agency objects

            hotel[0] = new Hotel(1, 1500, 1200, 15);
            hotel[1] = new Hotel(2, 1600, 1300, 10);
            hotel[2] = new Hotel(3, 1200, 1000, 20);

            for (int i = 0; i < 5; i++)
            {
                ta[i] = new TravelAgency(i + 1, hotel);

            }

            //subscribing Travel Agencies to hotel events
            hotel[0].subscribePriceCut(ta[0]);
            hotel[0].subscribePriceCut(ta[2]);
            hotel[0].subscribePriceCut(ta[3]);

            hotel[1].subscribePriceCut(ta[1]);
            hotel[1].subscribePriceCut(ta[4]);

            hotel[2].subscribePriceCut(ta[3]);
            hotel[2].subscribePriceCut(ta[1]);
            hotel[2].subscribePriceCut(ta[2]);
            hotel[2].subscribePriceCut(ta[4]);


            try
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread t = new Thread(new ThreadStart(hotel[i].runHotel));//starting hotel threads
                    t.Start();
                    threadlist.Add(t);
                    while (!hotel[i].isHotelAlive()) ;

                }

                for (int i = 0; i < 5; i++)
                {
                    Thread t = new Thread(new ThreadStart(ta[i].agencyRun));//starting Travel Agencies threads
                    t.Start();
                    threadlist.Add(t);

                }

                foreach (Thread item in threadlist)
                {
                    item.Join();
                }
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.Message);
            }

            

            DateTime end = System.DateTime.Now;
            Console.WriteLine("Main-> Total Orders Placed:{0}",TravelAgency.getOrderId());
            Console.WriteLine("Main-> End Time:{0}", end);
            Console.WriteLine("Main-> Total Execution Time:{0}", end-start);
            Console.SetOut(tmp);
            Console.WriteLine("Main->Done..Please check file");
            sw.Close();         
            


        }
    }
}
