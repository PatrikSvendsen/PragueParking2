using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._0
{
    public class VehiclePriceList
    {
        /*
            * När nya fordonspriser eller platser ska läggas till måste pris och storlek anges. etc; CarPrice = 20, CarSize = 4.
        */

        public static string pricePath = "../../../Parkinghouse/Vehiclevalues.txt";

        public static List<VehiclePriceList> parkingHouseValues = new List<VehiclePriceList>();

        public int Price { get; set; }
        public string Type { get; set; }

        public VehiclePriceList(string type, int price)
        {
            Type = type;
            Price = price;
        }

        public static void InitiateParkingValues()
        {
            //Vehicle item;
            string[] values = File.ReadAllLines(pricePath);
            foreach (string value in values)
            {
                string[] items = value.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                VehiclePriceList item = new VehiclePriceList(items[0], int.Parse(items[1]));
                parkingHouseValues.Add(item);
            }
            return;
        }

        //***
        // Retunerar priset på vald type
        //***

        public static int GetPrice(string type)
        {
            int price = 0;
            foreach (var item in parkingHouseValues)
            {
                if (item.Type == type)
                {
                    price = item.Price;
                    return price;
                }
            }
            price = -1;
            return -1;
        }
    }
}
