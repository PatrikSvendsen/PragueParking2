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
        /// Method to check time diff between current time and parked time.
        /// </summary>
        /// <param name="price"></param>
        /// <param name="parkedTime"></param>
        /// <param name="parkedPrice"></param>
        /// <returns></returns>
        public bool CalculateTimeParked(Vehicle vehicle, out TimeSpan timeDiff)
        {
            var timeNow = DateTime.Now;
            var timeParkedMinusFreeMin = vehicle.timeParked;
            timeDiff = timeNow - timeParkedMinusFreeMin;
            Console.WriteLine("Parked: " + timeDiff.Hours + "hours " + "and" + " " + timeDiff.Minutes + "minutes.");
            return true;
        }

        /// <summary>
        /// Method to calculate price on checkout.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int CalculatePriceOnCheckOut(TimeSpan time, int price)
        {
            int priceToPay = 0;

            if (time.Hours != 0)
            {
                return priceToPay = time.Hours * price + price;
            }
            else if (time.Minutes > ConfigValues.FreeParkingTime)
            {
                return priceToPay += price;
            }

            return 0;
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
    }
}
