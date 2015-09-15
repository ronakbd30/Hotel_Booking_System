using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace HotelBookingSystem
{
    public class TravelAgency
    {
         
        private int agencyId;
        private String cardNo;
        Hotel[] h;
        static private int orderId;
        private int pendingReciept;
        float [] currentPrice=new float[3];
        Order o1;
        static Random rng = new Random();
        private bool agencyAlive;
        public TravelAgency(int id, Hotel []h)
        {
            this.h=h;
            for (int i = 0; i < 3; i++)
            {
                currentPrice[i] = h[i].getRoomPrice();
            }
                cardNo=BankService.getCreditCardNo();
            agencyId = id;
            pendingReciept = 0;
        }

        /*checks if atleast one of the hotel is alive*/
        public bool atLeastOneIsAlive()
        {
            for(int i=0;i<h.Length;i++){
                if(h[i].isHotelAlive())
                    return true;
            }
                return false;
        }

        /*checks if atleast one of the hotel is dead*/
        public bool atLeastOneIsDead()
        {
            for (int i = 0; i < h.Length; i++)
            {
                if (!h[i].isHotelAlive())
                    return true;
            }
            return false;
        }

        /*gets the order confirmation from the RecieptBuffer class, wheather 
         the order is allocated romms or is cancelled*/
        public void getReciept()
        {
            try
            {

                while (pendingReciept > 0 || atLeastOneIsAlive() || agencyAlive)
                {

                    if (pendingReciept >= 0)
                    {
                        if (atLeastOneIsDead())
                        {

                            for (int i = 0; i < HotelBookingSystem.Buffer.getLength(); i++)
                            {
                                String x = HotelBookingSystem.Buffer.getBuffer(i);
                                if (x.Equals("-1"))
                                    continue;
                                else
                                {
                                    //Console.WriteLine("Hi");
                                    Order o = EncoderDecoder.decode(x);
                                    for (int j = 0; j < h.Length; j++)

                                        if (o.getAgencyId() == agencyId && h[j].getHotelId() == o.getHotelId() && !h[j].isHotelAlive())
                                        {
                                            Buffer.freeBuffer(i);
                                            pendingReciept--;
                                            Console.WriteLine("Agency {1}-> Cancelling Order {0} to Hotel {2} because Hotel has terminated.", o.getOrderId(), agencyId, o.getHotelId(), System.DateTime.Now);
                                        }
                                }
                            }
                        }

                        String receipt = ReceiptBuffer.getBuffer(agencyId);
                        if (!receipt.Equals("-1"))
                        {
                            Console.WriteLine("Agency {0}-> Confirmation Received: " + receipt, agencyId);
                            pendingReciept--;
                        }

                        Thread.Sleep(500);

                    }
                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
            
        }

        /*method started as a thread by main class, gets room price from hotel and call calculate rooms method*/
        public void agencyRun(){//keeps polling
            try
            {
                agencyAlive = true;
                Thread t = new Thread(new ThreadStart(getReciept));
                t.Start();
                while (atLeastOneIsAlive())
                {
                    for (int i = 0; i < h.Length; i++)
                    {
                        if (h[i].isHotelAlive())
                        {
                            float newPrice = h[i].getRoomPrice();
                            calcRooms(i, newPrice);
                        }
                    }


                }
                agencyAlive = false;
                t.Join();
                Console.WriteLine("Agency {1}-> Terminated at Timestamp: {0} ", System.DateTime.Now, agencyId);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }


        /*calulates the no of rooms based on the price difference and the need 
         calls the encoder to encode the order and places the order in the buffer*/
        public void calcRooms(int i, float newPrice)
        {
            try
            {

                int need = rng.Next(0, 5);
                int maxRoomPriceDiff = 5000;
                float diff = currentPrice[i] - newPrice;
                currentPrice[i] = newPrice;

                float fraction = (diff + maxRoomPriceDiff) / (2 * maxRoomPriceDiff);
                int rooms = (Int32)Math.Round(fraction * need);
                if (rooms == 0)
                    return;
                o1 = new Order();
                int tempOrder = ++orderId;
                o1.setOrder(agencyId, cardNo, h[i].getHotelId(), rooms, tempOrder, newPrice);
                String buffer_str = EncoderDecoder.encode(o1);
                pendingReciept++;//increment the pending reciept with every order that is placed in buffer
                Buffer.setBuffer(buffer_str, agencyId, h[i].getHotelId(), tempOrder, newPrice);

                Thread.Sleep(rng.Next(400, 600));
                return;

            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.Message);
            }
        }
        
        //event handler for pricecut notifications send by the hotel class
        public void priceCut(Int32 hotel_id,float newPrice)
        {
            Console.WriteLine("Agency {0}-> Recieved price cut {1} for hotel {2}", agencyId, newPrice, hotel_id);
            calcRooms(hotel_id-1, newPrice);
        }

        public int getAgencyId()
        {
            return agencyId;
        }

        public static int getOrderId()
        {
            return orderId;
        }

       
    }
}
                    