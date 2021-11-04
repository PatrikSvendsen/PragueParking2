using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragueParking2._0
{
    public class ParkingHouse
    {
        public static List<Vehicle> vehicles = new List<Vehicle>();
        public static string filePath = "../../../Parkinghouse/parkinglist.txt";                 // bör flyttas ut i parking house class

        //************************************
        // Fields
        //************************************

        const int ParkingSpotSize = 4;

        //************************************
        // Constructors
        //************************************

        public ParkingHouse()
        {
            try
            {
                for (int i = 1; i <= PHouseSize; i++)
                {
                    PHouse.Add(new ParkingSpot(ParkingSpotSize, i));
                }

                //List<string> currentParkedVehicles = new List<string>();
                //currentParkedVehicles = File.ReadAllLines(filePath).ToList();
                //foreach (string vehicle in currentParkedVehicles)
                //{
                //    Vehicle v;
                //    string[] items = vehicle.Split(new char[] { ',', ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries); //TODO: lägg till på datetime så att den innehåller : eller -
                //                                                                                                              //Vehicle v = new Vehicle(items[0], items[1], int.Parse(items[2])); //DateTime.ParseExact(items[3], "dd/MM/yyyy-HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None));
                //    if (items.Contains("Car"))
                //    {
                //        //ParkingSpot.ParkedVehicles.Add(new Car(items[1], int.Parse(items[2]), DateTime.ParseExact(items[3], "dd/MM/yyyy-HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None)));
                //        v = new Car(items[2], int.Parse(items[3]), DateTime.ParseExact(items[4], "dd/MM/yyyy-HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None));
                //        //ParkingSpot.ParkedVehicles.Add(v);
                //        PHouse[int.Parse(items[0])].ParkVehicleToSpot(v, int.Parse(items[0]));
                //    }
                //}
            }

            catch (Exception e)
            {
                Console.WriteLine("Something went wrong, Line 54", e);
            }
        }
        public override string ToString()
        {
            return PHouse.ToString();
        }

        //************************************
        // Properties
        //************************************

        private int PHouseSize { get; } = 3;           // ändra till 100 

        public static List<ParkingSpot> PHouse = new();

        //public override string ToString()d
        //{
        //    return type + ", " + price;
        //}

        //*****************************
        // Metoder
        //*****************************
        public bool IsSpotEmpty(Vehicle vehicle, out int i) // Vehicle vehicle
        {
            i = 0;
            bool isSpotEmpty = false;

            for (i = 0; i <= PHouse.Count; i++)
            {
                isSpotEmpty = PHouse[i].CheckSpace(vehicle.Size);
            }

            return isSpotEmpty;
        }

        public bool ParkVehicle(Vehicle vehicle)
        {

            for (int i = 0; i <= PHouse.Count; i++)
            {
                bool isSpotEmpty = PHouse[i].CheckSpace(vehicle.Size);
                //int i;
                //bool isSpotEmpty = IsSpotEmpty(vehicle, out i);

                if (isSpotEmpty)
                {
                    PHouse[i].ParkVehicleToSpot(vehicle, i);
                    return true;
                }
            }
            return false;
        }
        public static bool CheckIfExists(string plateNumber)
        {
            /*
             * Tar emot string regnr, om den existerar ger den true annars false.
             */

            bool check = false;
            for (int i = 0; i < PHouse.Count; i++)
            {
                if (check = PHouse[i].FindVehicle(plateNumber) == true)
                {
                    return check;
                }
            }
            return check;
        }
        public static bool RemoveVehicle(string plateNumber)
        {
            bool check = false;
            for (int i = 0; i < PHouse.Count;)
            {
                return check = PHouse[i].RemoveVehicle(plateNumber);
            }

            return check;
        }

        public static int FindSpot(string plateNumber)
        {
            int spot = -1;
            //for (int i = 0; i < PHouse.Count; i++)
            //{
            //    spot = PHouse[i].FindSpot(plateNumber);
            //}
            foreach (var item in PHouse)
            {
                item.FindSpot(plateNumber);
            }

            return spot;
        }
        public static bool ReParkVehicle(string plateNumber, int spot)
        {

            //bool isSpotEmpty = PHouse[spot].CheckSpace(vehicle);


            return true;
        }

        //public void RunList()
        //{

        //    List<string> currentParkedVehicles = new List<string>();
        //    currentParkedVehicles = File.ReadAllLines(filePath).ToList();
        //    foreach (string vehicle in currentParkedVehicles)
        //    {
        //        Vehicle v;
        //        string[] items = vehicle.Split(new char[] { ',', ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries); //TODO: lägg till på datetime så att den innehåller : eller -
        //        //Vehicle v = new Vehicle(items[0], items[1], int.Parse(items[2])); //DateTime.ParseExact(items[3], "dd/MM/yyyy-HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None));
        //        if (items.Contains("Car"))
        //        {
        //            //ParkingSpot.ParkedVehicles.Add(new Car(items[1], int.Parse(items[2]), DateTime.ParseExact(items[3], "dd/MM/yyyy-HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None)));
        //            //v = new Car(items[2], int.Parse(items[3]), DateTime.ParseExact(items[4], "dd/MM/yyyy-HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None));
        //            //ParkingSpot.ParkedVehicles.Add(v);
        //            PHouse[int.Parse(items[0])].ParkVehicleToSpot(v, int.Parse(items[0]));
        //        }
        //    }
        //}
    }
}
