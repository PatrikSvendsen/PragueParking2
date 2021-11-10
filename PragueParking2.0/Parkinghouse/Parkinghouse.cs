using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PragueParking2._0
{

    /// <summary>
    /// A class to handle parkingspots and creating list of parkingspots.
    /// </summary>
    /// 

    public class ParkingHouse
    {
        //public static List<Vehicle> vehicles = new List<Vehicle>();
        public static string filePath = "../../../Parkinghouse/parkinglist.txt";                 // bör flyttas ut i parking house class
        const int ParkingSpotSize = 4;
        /// <summary>
        /// Main constructor for ParkingHouse, creates a list of parkingspots.
        /// </summary>
        public ParkingHouse()
        {
            try
            {
                for (int i = 1; i <= PHouseSize; i++)
                {
                    PHouse.Add(new ParkingSpot(ParkingSpotSize, i));
                }
                // läs in sparad data här, från dataConfig class med JSON.
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
        private int PHouseSize { get; } = 100;           // ändra till 100 

        internal static List<ParkingSpot> PHouse = new();

        //public override string ToString()d
        //{
        //    return type + ", " + price;
        //}


        public static bool IsSpotEmpty(Vehicle vehicle, int i)          //TODO: Behöver göras om så att den kollar om enbart SPOT är ledig eller inte, ska inte kontrollera storlek etc.
        {
            bool isSpotEmpty = false;

            foreach (var spot in PHouse)
            {
                if (spot.Spot == i)
                {
                    //isSpotEmpty = PHouse[i].CheckSpace(vehicle);
                    isSpotEmpty = spot.CheckSpace(vehicle);
                    return isSpotEmpty;
                }
            }
            return isSpotEmpty;
        }
        public static bool ReParkVehicle(Vehicle vehicle, int i)
        {
            bool check;
            check = IsSpotEmpty(vehicle, i);    //Måste anges för att hitta rätt index plats.

            if (check == true)
            {
                //DateTime saved = vehicle.TimeParked;
                int price = vehicle.Price;
                RemoveVehicle(vehicle);
                PHouse[i - 1].ParkVehicleToSpot(vehicle, i - 1);
                //vehicle.TimeParked = saved;     // ta bort
                vehicle.Price = price;
                return true;
            }
            return false;
        }

        public bool ParkVehicle(Vehicle vehicle)  //TODO: Se över så att den loopar igenom alla parkerade fordon när den ska placera en MC så de läggs på samma.
        {
            for (int i = 0; i <= PHouse.Count; i++)
            {
                bool isSpotEmpty = PHouse[i].CheckSpace(vehicle);

                if (isSpotEmpty)
                {
                    bool check = PHouse[i].ParkVehicleToSpot(vehicle, i);
                    if (check == true)
                    {
                        string jsonString = JsonSerializer.Serialize(vehicle);
                        Console.WriteLine(jsonString);
                        File.AppendAllLines((filePath), jsonString.ToString()); // Ska lösas så att den skriver till fil. Bör flyttas till Config.cs fil.
                        return check;
                    }
                    else
                    {
                        return check;
                    }
                }
            }
            return false;
        }

        public static bool CheckIfExists(string plateNumber, out Vehicle vehicle)
        {
            bool check = false;
            for (int i = 0; i < PHouse.Count; i++)
            {
                if (check = PHouse[i].FindVehicle(plateNumber) == true)
                {
                    vehicle = PHouse[i].ReturnObjectVehicle(plateNumber);
                    return check;
                }
            }
            vehicle = null;
            return check;
        }

        public static bool RemoveVehicle(Vehicle vehicle)
        {
            bool check = false;

            foreach (var spot in PHouse)
            {
                if (spot.Spot == vehicle.Spot)
                {
                    check = spot.RemoveVehicle(vehicle);
                    return check;
                }
            }
            return check;
        }

        public static int FindSpot(string plateNumber)
        {
            int spot = -1;
            foreach (var item in PHouse)
            {
                spot = item.FindSpot(plateNumber);
                if (spot > 0)
                {
                    return spot;
                }
            }
            return spot;
        }

        public static bool CalculatePriceOnCheckOut(int price, DateTime parkedTime, out int parkedPrice)
        {
            var timeNow = DateTime.Now.AddMinutes(-10);
            TimeSpan timeDiff = timeNow - parkedTime;
            //var res = String.Format("{0}:{1}", timeDiff.Hours, timeDiff.Minutes);
            // https://jamesmccaffrey.wordpress.com/2014/02/04/converting-a-date-to-an-integer-in-c/
            // Bör göra så att om timediff är - så ska ingen kostand tas ut utan skickas vidare/ut ur loopen direkt.
            // är den inte negativ ska det tas ut en kostnad, kolla ovan länk.
            parkedPrice = 0;
            Console.WriteLine(timeDiff);
            return false;
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
