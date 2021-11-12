using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PragueParking2._0.DataConfig;

namespace PragueParking2._0
{
    /// <summary>
    /// A class to handle parkingspots and creating list of parkingspots.
    /// </summary>

    public class ParkingHouse
    {
        public static List<ParkingSpot> PHouse = new();

        private int ParkingSpotSize = ConfigValues.ParkingSpotSize;
        private int PHouseSize { get; } = ConfigValues.ParkingHouseSize;

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
                LoadVehicleFromFile();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong, Line 49", e);
            }
        }
        public override string ToString()
        {
            return PHouse.ToString();
        }

        //public override string ToString()d
        //{
        //    return type + ", " + price;
        //}
        /// <summary>
        /// Method to check type of vehicle that needs to be created.
        /// </summary>
        /// <param name="vehiclePlate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool AddVehicleToList(string vehiclePlate, string type)
        {
            bool check = false;
            if (type == "Car")
            {
                check = ParkVehicle(new Car(vehiclePlate));
            }
            else if (type == "MC")
            {
                check = ParkVehicle(new MC(vehiclePlate));
            }
            return check;
        }
        /// <summary>
        /// Method to check if spot is empty. 
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool IsSpotEmpty(Vehicle vehicle, int i)
        {
            bool isSpotEmpty = false;

            foreach (var spot in PHouse)
            {
                if (spot.Spot == i)
                {
                    isSpotEmpty = spot.CheckSpace(vehicle);
                    return isSpotEmpty;
                }
            }
            return isSpotEmpty;
        }
        /// <summary>
        /// Method used to repark a vehicle - used in both move and when loading vehicles from JSON file.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool ReParkVehicle(Vehicle vehicle, int i)
        {
            bool check;
            check = IsSpotEmpty(vehicle, i);

            if (check == true)
            {
                int price = vehicle.Price;
                //RemoveVehicle(vehicle);
                PHouse[i - 1].ParkVehicleToSpot(vehicle, i - 1);
                vehicle.Price = price;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Method to park a new vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool ParkVehicle(Vehicle vehicle)
        {
            for (int i = 0; i < PHouse.Count; i++)
            {
                bool isSpotEmpty = PHouse[i].CheckSpace(vehicle);

                if (isSpotEmpty)
                {
                    bool check = PHouse[i].ParkVehicleToSpot(vehicle, i);
                    if (check == true)
                    {
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
        /// <summary>
        /// Method to check if platenumber alread exist, if it deos it returns the obj.
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Method to remove a specific obj.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool RemoveVehicle(Vehicle vehicle)
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
        /// <summary>
        /// Method to find the specific spot from input.
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        public int FindSpot(string plateNumber)
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
        /// <summary>
        /// Method to calculate price on check-out.
        /// </summary>
        /// <param name="price"></param>
        /// <param name="parkedTime"></param>
        /// <param name="parkedPrice"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Method to load saved parked vehicles from a JSON file.
        /// </summary>
        public void LoadVehicleFromFile()
        {
            string filePath = @"../../../DataConfig/parkedVehicle.json";
            string convertJson = File.ReadAllText(filePath);
            List<Vehicle> vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(convertJson).ToList();

            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle.Size == 4)
                {
                    ReParkVehicle(new Car(vehicle.PlateNumber, vehicle.Price, vehicle.Spot, vehicle.timeParked), vehicle.Spot); // läser in och lägger fordon på rätt parkeringsplats.
                }
                else if (vehicle.Size == 2)
                {
                    ReParkVehicle(new MC(vehicle.PlateNumber, vehicle.Price, vehicle.Spot, vehicle.timeParked), vehicle.Spot);
                }
            }
            vehicles.Clear();
        }
        /// <summary>
        /// Method to initiate config values from a JSON file.
        /// </summary>
        public static void SetValuesFromConfig()
        {
            string filePath = @"../../../DataConfig/ConfigValues.json";
            string jsonConfig = File.ReadAllText(filePath);
            JsonConvert.DeserializeObject<ConfigValues>(jsonConfig);
        }
        /// <summary>
        /// Method to save parkedvehiclelist to a JSON file.
        /// </summary>
        public static void SaveVehicleToFile()                   // finns inte inlagd någonstans. Behöver kontrolleras.
        {
            List<Vehicle> parkedVehiclesList = new List<Vehicle>();
            foreach (var vehicle in ParkingHouse.PHouse)
            {
                foreach (var item in vehicle.ParkedVehicles)
                {
                    parkedVehiclesList.Add(item);
                }
            }
            string filepath = @"../../../dataconfig/parkedvehicle.json";
            string vehicles = JsonConvert.SerializeObject(parkedVehiclesList, Formatting.Indented);
            File.WriteAllText(filepath, vehicles);
        }

        //public void SaveVehicleToFile()                   // finns inte inlagd någonstans. Behöver kontrolleras.
        //{
        //    string filepath = @"../../../dataconfig/parkedvehicle.json";
        //    string vehicles = JsonConvert.SerializeObject(parkingTest.ParkedVehicles, Formatting.Indented);
        //    File.WriteAllText(filepath, vehicles);
        //}

        //public static void SaveVehicleToFile(Vehicle vehicle) // finns inte inlagd någonstans. Behöver kontrolleras.
        //{
        //    if (vehicle != null)
        //    {
        //        string filePath = @"../../../dataconfig/parkedvehicle.json";
        //        string convertJson = File.ReadAllText(filePath);
        //        List<Vehicle> vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(convertJson).ToList();
        //        vehicles.Add(vehicle);
        //        string parkedvehicles = JsonConvert.SerializeObject(vehicles, Formatting.Indented);
        //        File.WriteAllText(filePath, parkedvehicles);
        //    }
        //    else
        //    {
        //        string filePath = @"../../../dataconfig/parkedvehicle.json";
        //        string convertJson = File.ReadAllText(filePath);
        //        List<Vehicle> vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(convertJson).ToList();
        //        string parkedvehicles = JsonConvert.SerializeObject(vehicles, Formatting.Indented);
        //        File.WriteAllText(filePath, parkedvehicles);
        //    }
        //}


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
