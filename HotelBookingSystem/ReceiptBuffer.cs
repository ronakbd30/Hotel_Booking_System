
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HotelBookingSystem
{
    //Order confirmation class places success or failure message in the buffer
    class ReceiptBuffer
    {
        private static String nullvalue = "-1";
        private static String[] buffer = { nullvalue, nullvalue, nullvalue, nullvalue, nullvalue };
        private static Semaphore[] semaswrite = { new Semaphore(1, 1), new Semaphore(1, 1), new Semaphore(1, 1), new Semaphore(1, 1), new Semaphore(1, 1) };
                
        public static void  setBuffer(int agencyid,String str)
        {
            semaswrite[agencyid-1].WaitOne();
            buffer[agencyid-1] = str;
                                                                     
            
        }

        public static String getBuffer(int agencyid)
        {
            
            String str = buffer[agencyid-1];
            if (!str.Equals("-1"))
            {
                buffer[agencyid-1] = "-1";
                semaswrite[agencyid-1].Release();
            }
            return str;                  
            
        }


    }
}
