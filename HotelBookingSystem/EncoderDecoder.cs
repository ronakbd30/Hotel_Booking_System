using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingSystem
{
    public class EncoderDecoder
    {
        //Encode the Order object into string object
        public static String encode(Order o)
        {
            String enc_string = o.getAgencyId().ToString();
            enc_string += "@" + o.getCardNo() + "@" + o.getHotelId() + "@" + o.getRooms() + "@" + o.getOrderId() + "@" + o.getQuotePrice();
            return enc_string;
        }

        //decode the String object into Order object
        public static Order decode(String s)
        {
            if (s == null)
                return null;
            else
            {
                String []temp=s.Split(new char[] {'@'});
                Order dec_obj=new Order();

                dec_obj.setOrder(Convert.ToInt32(temp[0]), temp[1], Convert.ToInt32(temp[2]), Convert.ToInt32(temp[3]), Convert.ToInt32(temp[4]), float.Parse(temp[5]));

                return dec_obj;

            }
        }
    }
    
}
