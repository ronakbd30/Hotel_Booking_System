using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
namespace HotelBookingSystem
{
    public delegate void priceCutDelegate(Int32 hotelId, float price);

    public class Hotel
    {
        private float roomPrice; 
        private float pricingConstant; 
        private int hotelId;
        private int roomsAvailable;
        private int maxRoomsAvailable;
        private float minRoomPrice;
        private bool hotelAlive;
        private static event priceCutDelegate priceCutEvent;
        private int totalRoomsOrdered;
        private int maxRoomPriceDiff;
        private Object roomPriceLock;

        public Hotel(int id, float initialPrice, float minPrice, int maxRoomsAvailable)
        {
            hotelId = id;
            roomPrice = initialPrice;
            minRoomPrice = minPrice;
            this.maxRoomsAvailable = maxRoomsAvailable;
            roomsAvailable = maxRoomsAvailable;
            this.pricingConstant = 50; //assuming maxRoomsAvailable=1 to 20,roomsAvailable=1 to maxRoomsAvailable, totalRoomsOrdered= 5 to 15 , we divide by pricingConstant to normalize output to 0 to 1.
            totalRoomsOrdered = 0;
            maxRoomPriceDiff = 5000;
            roomPriceLock=new Object();

        }

        //calls the price cut event handler
        public void subscribePriceCut(TravelAgency agency)
        {
            priceCutEvent += new priceCutDelegate(agency.priceCut);
            Console.WriteLine("Hotel  {1}-> Agency {0} has been subscribed for pricecut Event", agency.getAgencyId(), hotelId);
        }

        public float getRoomPrice() //return the current room price
        {
            lock (roomPriceLock) 
            return roomPrice;
        }

        public int getHotelId() //return the hotel id
        {
            return hotelId;
        }

        public bool isHotelAlive() // return true if the hotel thread is running

        {
            return this.hotelAlive;
        }

        //generates room price based on the maxrooms available and total rooms ordered 
        float generateRoomPrice(int totalRooms, int available)
        {
            if (available == 0)
                available = 1;
            //float fraction = (maxRoomsAvailable * totalRooms * 1.0f )/ (available * pricingConstant);
            //Console.WriteLine("Timestamp: {2} PricingControl fraction for Hotel {0}: {1}", hotelId, (maxRoomsAvailable * totalRooms * 1.0f) / available,System.DateTime.Now);
            //float newprice=  fraction * maxRoomPriceDiff + minRoomPrice;
            float newprice = (maxRoomsAvailable * totalRooms * pricingConstant )/available + minRoomPrice;
            if (newprice > minRoomPrice + maxRoomPriceDiff)
                return minRoomPrice + maxRoomPriceDiff;
            return newprice;
        }

        //processes the order by checking the available rooms and validating credit card with BankService
        void processOrder(Object x)
        {
            try
            {

                float price1 = roomPrice;
                Order o = (Order)x;
                encryptionService.ServiceClient encrypt = new encryptionService.ServiceClient();
                int orderID = o.getOrderId();
                int agencyID = o.getAgencyId();
                int hotelID = o.getHotelId();
                int roomsOrdered = o.getRooms();
                float quotedPrice = o.getQuotePrice();
                float totalPrice = quotedPrice * roomsOrdered;
                String cardNo = o.getCardNo();

                String success = "Order " + orderID + " booked with " + hotelID + " for " + roomsOrdered + " rooms was processed successfully with a total of $" + totalPrice;
                //String failure = "Order " + orderID + " for Agency " + agencyID + " failed to proccess";


                lock (this)
                {
                    //Console.WriteLine("Thread{2} : Orderporcess ..{0}..,{1}", quotedPrice, roomPrice,Thread.CurrentThread.ManagedThreadId);
                    //Console.WriteLine("OrderProcess***********" + this);
                    if (BankService.validateCard(encrypt.Encrypt(cardNo)) == false)
                    {
                        ReceiptBuffer.setBuffer(agencyID, "The card number for Order " + orderID + " booked with Hotel " + hotelID + " is invalid");
                    }
                    else if (roomsOrdered > roomsAvailable)
                    {
                        ReceiptBuffer.setBuffer(agencyID, "The number of rooms ordered in Order " + orderID + " booked with " + hotelID + " is greater than the available rooms");
                    }
                    else if ((int)quotedPrice != (int)price1)
                    {
                        //Console.WriteLine("Thread:"+Thread.CurrentThread.ManagedThreadId+" Orderporcess ..The quoted price for Order " + orderID + " for Agency " + agencyID + " of $" + quotedPrice + " does not match the current price of $" + roomPrice);
                        ReceiptBuffer.setBuffer(agencyID, "The quoted price for Order " + orderID + " booked with Hotel " + hotelID + " of $" + quotedPrice + " does not match the current price of $" + price1);
                    }
                    else
                    {
                        roomsAvailable = roomsAvailable - roomsOrdered;
                        ReceiptBuffer.setBuffer(agencyID, success);

                    }
                }

            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.Message);
            }
        }


        

        
        //generates price cuts and the hotel thread exits if the price cuts exceeds the a certain threshold
        void runPricingControl()
        {
            try
            {

                int pricecuts = 0;
                //int pricechangecount = 0;
                Random rand = new Random();
                float previousPrice;
                //int prevRoomsAvailable;
                int prevTotalRoomsOrdered;

                while (pricecuts < 5)//threshold =5
                {

                    Thread.Sleep(rand.Next(1500, 2000));
                    //Console.WriteLine("Price Control***********"+this);

                    lock (roomPriceLock)
                    {
                        prevTotalRoomsOrdered = totalRoomsOrdered;
                        previousPrice = roomPrice;
                        lock (this)
                        {
                            roomsAvailable += rand.Next(0, maxRoomsAvailable - roomsAvailable);
                            //prevRoomsAvailable = roomsAvailable;
                            roomPrice = generateRoomPrice(prevTotalRoomsOrdered, roomsAvailable);
                        }
                    }

                    //pricechangecount++;
                    Console.WriteLine("Hotel  {0}-> Price Changed.. Price:{1}, Diff:{2},  TotalRoomsOrderedinInterval:{3}", hotelId, roomPrice, roomPrice - previousPrice, prevTotalRoomsOrdered);
                    if (roomPrice < previousPrice)
                    {
                        pricecuts++;
                        if (priceCutEvent != null)
                        {
                            priceCutEvent(hotelId, roomPrice);
                            Console.WriteLine("Hotel  {0}-> PriceCut Event sent.. Price={1} ", hotelId, roomPrice);
                        }
                        else Console.WriteLine("Hotel  {0}-> PriceCut Event is null", hotelId);
                    }
                    totalRoomsOrdered -= prevTotalRoomsOrdered;
                }

                hotelAlive = false;
                //Console.WriteLine("Timestamp: {1} Hotel {0} PricingControl terminated", hotelId, System.DateTime.Now);


            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.Message);
            }
        }


        //This method is started as a thread
        public void runHotel()
        {
            try
            {

                ArrayList orderthreadarr = new ArrayList();
                hotelAlive = true;
                String x = "";
                new Thread(new ThreadStart(runPricingControl)).Start();

                while (hotelAlive)
                {

                    for (int j = 0; j < HotelBookingSystem.Buffer.getLength(); j++)
                    {
                        x = HotelBookingSystem.Buffer.getBuffer(j);
                        if (x.Equals("-1"))
                            continue;
                        else
                        {
                            Order o = EncoderDecoder.decode(x);
                            if (o.getHotelId() == hotelId)
                            {
                                HotelBookingSystem.Buffer.freeBuffer(j);
                                totalRoomsOrdered += o.getRooms();
                                Console.WriteLine("Hotel  {0}-> Received OrderId-{1}  from Agency{2} ..PriceQuote:{3} , Rooms Ordered:{4} ", hotelId, o.getOrderId(), o.getAgencyId(), o.getQuotePrice(), o.getRooms());

                                Thread t = new Thread(new ParameterizedThreadStart(processOrder));
                                t.Start(o);
                                orderthreadarr.Add(t);


                            }
                        }
                    }
                    Thread.Sleep(500);
                }

                foreach (Thread item in orderthreadarr)
                {
                    item.Join();
                }

                Console.WriteLine("Hotel  {0}-> Terminated at Timestamp: {1}", hotelId, System.DateTime.Now);

            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.Message);
            }
        }

    }
}
