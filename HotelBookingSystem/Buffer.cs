using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HotelBookingSystem
{
    /* buffer class used to place the order by travel agency class */
    class Buffer
    {
        private static String[] buffer = new String [3] { "-1","-1","-1"};
        private static Semaphore _pool = new Semaphore(3, 3);//semaphore used to keep track of no. of resources available

        /*place the order in one of the buffer cell*/
        public static void setBuffer(String s,int agencyId, int hotelId,int orderId,float price)
        {
            
            //Console.WriteLine("Agency {0} begins and waits for the placing the order", agencyId);
            _pool.WaitOne();
           
            for (int i = 0; i < buffer.Length; i++)
            {
                lock (buffer[i])
                {
                  if (buffer[i].Equals("-1"))
                  {

                      buffer[i] = s;
                      Console.WriteLine("Agency {0}-> Placed OrderId {1} for Hotel {2}..PriceQuote:{3}", agencyId, orderId, hotelId, price,System.DateTime.Now);
                      break;
                    
                  }
                }
            }
            return;
        }

        /*returns the order from buffer cell*/
        public static String getBuffer(int i)
        {

                    return buffer[i];
            
            
        }

        /*emptys the buffer cell*/
        public static void freeBuffer(int i)
        {
            lock (buffer[i])
            {
                buffer[i] = "-1";
                _pool.Release();
            }
        }

        public static int getLength(){
            return buffer.Length;
        }

    }
}
