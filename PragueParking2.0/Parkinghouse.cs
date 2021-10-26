using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._0
{
    public class Parkinghouse
    {
        public static string pricePath = "../../../Vehiclevalues.txt";
        public static List<Parkinghouse> parkingHouseValues = new List<Parkinghouse>();
        public static List<Vehicle> vehicles = new List<Vehicle>(100);
        

        /*
         * När nya fordonspriser eller platser ska läggas till måste pris och storlek anges. etc; CarPrice = 20, CarSize = 4.
         */


        //************************************
        // Fields
        //************************************
        public string plateNumber;
        public string type;
        public int price;
        public int size;

        //************************************
        // Constructors
        //************************************
        public Parkinghouse()
        {

        }
        public Parkinghouse( int price)
        {
            Price = price;
        }
        public Parkinghouse(string type, int price)
        {
            Type = type;
            Price = price;
        }
        public Parkinghouse(string type, int price, int size)
        {
            Type = type;
            Price = price;
            Size = size;
        }

        //************************************
        // Properties
        //************************************

        public string Type
        {
            get { return type;  }
            set { type = value; }
        }
        public int Price
        {
            get { return price; }
            set { price = value; }
        }
        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public string PlateNumber { get; set; }

        public override string ToString()
        {
            return type + ", " + plateNumber + ", " + price + ", " + size;
        }

        //*****************************
        // Metoder
        //*****************************

        public static void InitiateParkingValues()
        {
            string[] values = File.ReadAllLines(pricePath);
            foreach (string value in values)
            {
                string[] items = value.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //Vehicle v = new Vehicle(items[0], items[1], (VehiclePricePerHour)int.Parse(items[2]));
                Parkinghouse item = new Parkinghouse(items[0], int.Parse(items[1]), int.Parse(items[2]));
                //Vehicle v = new Vehicle();
                parkingHouseValues.Add(item);
            }
            return ;
        }

        //***
        // Retunerar priset på vald type
        //***
        public static int GetPrice(string type, out int price)
        {
            foreach (var item in parkingHouseValues)
            {
                if (item.type == type)
                {
                    price = item.Price;
                    return price;
                }
            }
            price = -1;
            return -1;
        }
        //public static void AddSpotToList(string type)
        //{
        //    var value = 0;

        //    foreach (var item in parkingHouseValues)
        //    {
        //        if (item.type == type)
        //        {
        //            value = item.Size;

        //            if (value <= (int)ParkingSize.Maxsize)
        //            {
        //                ParkingSpot spot = new ParkingSpot((int)ParkingSize.Maxsize, value);

        //            }
        //        }
        //    }
        //}

        public enum ParkingSize
        {
            Maxsize = 4
        }

        public class ParkingSpot : Parkinghouse
        {
            public static List<int> parkingSize = new List<int>(100);

            public ParkingSpot(ParkingSize maxSize, int size) : base(size)
            {
                MaxSize = maxSize;
                this.size = size;
            }
            public ParkingSize MaxSize { get; private set; }

            



        }
    }
}
