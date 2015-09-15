using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelBookingSystem.encryptionService;

namespace HotelBookingSystem
{
    public class BankService
    {
        
        
        public static String[] creditCardNumbers = new String[5];
        private static int totalCards = 0;

 
        /*getCreditCardNo method called by travel Agency while applying for credit card*/
        public static String getCreditCardNo()
        {
            Random rand = new Random();
            String cardNo = rand.Next(10000000, 99999999) + "";
            creditCardNumbers[totalCards] = cardNo;
            totalCards++;
            return cardNo;

        }

        /*validate credit card*/
        public static Boolean validateCard(String card)
        {
            encryptionService.ServiceClient decrypt = new encryptionService.ServiceClient();
            Boolean found = false;
            for(int i = 0; i < 5; i++)
            {
                if(decrypt.Decrypt(card) == creditCardNumbers[i])
                {
                    found = true;
                }
            }
            return found;

        }





    }
}
