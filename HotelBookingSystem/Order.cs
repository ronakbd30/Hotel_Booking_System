using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingSystem
{
    public class Order
    {
        private int agencyId;
        private String cardNo;
        private int hotelId;
        private int rooms;
        private int orderId;
        private float quotePrice;

        public Order()
        {
            
        }
        //sets the order object
        public void setOrder(int agencyId, String cardNo, int hotelId, int rooms, int orderId,float quotePrice)
        {
            this.agencyId = agencyId;
            this.cardNo = cardNo;            
            this.hotelId = hotelId;
            this.rooms = rooms;
            this.orderId = orderId;
            this.quotePrice = quotePrice;
        }

        public int getHotelId()
        {
            return this.hotelId;
        }

        public int getAgencyId()
         {
             return this.agencyId;
         }

         public String getCardNo()
         {
             return this.cardNo;
         }

         public int getRooms()
         {
             return this.rooms;
         }

         public int getOrderId()
         {
             return this.orderId;
         }

        public float getQuotePrice()
         {
             return this.quotePrice;
         }
    }
}
