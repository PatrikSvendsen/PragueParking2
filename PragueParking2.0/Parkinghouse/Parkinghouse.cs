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
using Spectre.Console;

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
                Console.WriteLine("Something went wrong, Line 41", e);
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
            var timeParkedMinusFreeMin = vehicle.TimeParked;
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
                    ReParkVehicle(new Car(vehicle.PlateNumber, vehicle.Price, vehicle.Spot, vehicle.TimeParked), vehicle.Spot);
                }
                else if (vehicle.Size == 2)
                {
                    ReParkVehicle(new MC(vehicle.PlateNumber, vehicle.Price, vehicle.Spot, vehicle.TimeParked), vehicle.Spot);
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
        public void SaveVehicleToFile()
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

        /// <summary>
        /// Method to print a small parkingview.
        /// </summary>
        public void ShowParkingViewSmall()
        {
            List<Vehicle> parkedVehiclesList = new List<Vehicle>();
            int column = 6;
            int rows = 1;
            Console.WriteLine("\nBelow is the parking lot with current parked vehicles.\n");
            for (int i = 0; i < PHouse.Count; i++)
            {
                if (rows >= column && rows % column == 0)
                {
                    Console.WriteLine();
                    rows = 1;
                }
                if (PHouse[i].ParkedVehicles.Count == 0)
                {
                    Console.Write(i + 1 + ": Empty \t");
                    rows++;
                }
                else if (PHouse[i].ParkedVehicles != null)
                {
                    if (PHouse[i].ParkedVehicles.Count == 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(i + 1 + ": " + "Occupied" + "\t");
                        Console.ResetColor();
                        rows++;
                    }
                    else if (PHouse[i].ParkedVehicles.Count == 1)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(i + 1 + ": " + "Occupied" + "\t");
                            Console.ResetColor();
                            rows++;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(i + 1 + ": " + "Room for 1" + "\t");
                            Console.ResetColor();
                            rows++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method to print a large parkingview.
        /// </summary>
        public void ShowParkingViewLarge()
        {
            ParkingSpot parkingSpot = new ParkingSpot();

            List<ParkingSpot> clonedList = new List<ParkingSpot>(PHouse);

            var clonedList2 = PHouse;

            for (int i = 0; i < clonedList2.Count; i++)
            {
                if (clonedList2[i].ParkedVehicles.Count == 0)
                {
                    clonedList2[i].ParkedVehicles.Insert(0, new Vehicle());
                    clonedList2[i].ParkedVehicles.Insert(1, new Vehicle());
                    clonedList2[i].ParkedVehicles[0].PlateNumber = String.Empty;
                    clonedList2[i].ParkedVehicles[0].TimeParked = DateTime.MinValue;
                    clonedList2[i].ParkedVehicles[1].PlateNumber = String.Empty;
                    clonedList2[i].ParkedVehicles[1].TimeParked = DateTime.MinValue;
                }
                else if (clonedList2[i].ParkedVehicles.Count == 1 && clonedList2[i].ParkedVehicles[0].GetType().ToString() == "Car")
                {
                    clonedList2[i].ParkedVehicles.Insert(1, new Vehicle());
                    clonedList2[i].ParkedVehicles[1].PlateNumber = String.Empty;
                    clonedList2[i].ParkedVehicles[1].TimeParked = DateTime.MinValue;
                }
                else if (clonedList2[i].ParkedVehicles.Count == 1 && clonedList2[i].ParkedVehicles[0].GetType().ToString() == "MC")
                {
                    clonedList2[i].ParkedVehicles.Insert(1, new Vehicle());
                    clonedList2[i].ParkedVehicles[1].PlateNumber = String.Empty;
                    clonedList2[i].ParkedVehicles[1].TimeParked = DateTime.MinValue;
                }
            }
            //Console.WriteLine("\nBelow is the parking lot with current parked vehicles.\n");
            //int column = 20;
            var rule = new Rule();
            int rows = 1;
            Console.Clear();

            //För rad 1-10
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 1-10
            var table5 = new Table();
            table5.AddColumns("1", "2", "3", "4", "5", "6", "7", "8", "9", "10");
            table5.HeavyBorder();
            table5.Columns[0].Width(28);
            table5.Columns[1].Width(28);
            table5.Columns[2].Width(28);
            table5.Columns[3].Width(28);
            table5.Columns[4].Width(28);
            table5.Columns[5].Width(28);
            table5.Columns[6].Width(28);
            table5.Columns[7].Width(28);
            table5.Columns[8].Width(28);
            table5.Columns[9].Width(28);
            for (int i = 0; i < 10; i++)
            {
                if (PHouse[i].ParkedVehicles.Count != 0)
                {
                    if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                    {
                        table5.Columns[i].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                        rows++;
                    }
                    else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                    {

                        table5.Columns[i].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);

                        rows++;
                    }
                    else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                    {
                        table5.Columns[i].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                    }
                }
            }
            table5.InsertRow(0,
            "Spot: " + PHouse[0].Spot + " | " + " " + PHouse[0].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[1].Spot + " | " + " " + PHouse[1].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[2].Spot + " | " + " " + PHouse[2].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[3].Spot + " | " + " " + PHouse[3].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[4].Spot + " | " + " " + PHouse[4].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[5].Spot + " | " + " " + PHouse[5].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[6].Spot + " | " + " " + PHouse[6].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[7].Spot + " | " + " " + PHouse[7].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[8].Spot + " | " + " " + PHouse[8].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[9].Spot + " | " + " " + PHouse[9].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"));


            rows = 1;
            var table6 = new Table();
            table6.AddColumns("", "", "", "", "", "", "", "", "", "");
            table6.HeavyBorder();
            table6.Columns[0].Width(28);
            table6.Columns[1].Width(28);
            table6.Columns[2].Width(28);
            table6.Columns[3].Width(28);
            table6.Columns[4].Width(28);
            table6.Columns[5].Width(28);
            table6.Columns[6].Width(28);
            table6.Columns[7].Width(28);
            table6.Columns[8].Width(28);
            table6.Columns[9].Width(28);
            for (int i = 0; i < 10; i++)
            {
                if (PHouse[i].ParkedVehicles.Count != 0)
                {
                    if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                    {
                        //table6.Columns[i].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                        table6.Columns[i].Header(PHouse[i].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M") + "   " + PHouse[i].ParkedVehicles[1].PlateNumber);
                        rows++;
                    }
                    else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                    {
                        //table6.Columns[i].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                        table6.Columns[i].Header(PHouse[i].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M") + "   " + PHouse[i].ParkedVehicles[1].PlateNumber);
                        rows++;
                    }
                    else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                    {
                        //table6.Columns[i].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                        table6.Columns[i].Header(PHouse[i].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M") + "   " + PHouse[i].ParkedVehicles[1].PlateNumber);
                        rows++;
                    }
                }
            }
            //table6.InsertRow(0, PHouse[0].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[1].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[2].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[3].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[4].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[5].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[6].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[7].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[8].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[9].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"));
            table5.HideHeaders();
            table5.Border = TableBorder.Simple;
            //table6.Border = TableBorder.Simple;
            AnsiConsole.Write(table5);
            AnsiConsole.Write(table6);
            AnsiConsole.Write(rule);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //För rad 11-20
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 11-20
            rows = 1;
            var table7 = new Table();
            table7.AddColumns("11", "12", "13", "14", "15", "16", "17", "18", "19", "20");
            table7.HeavyBorder();
            table7.Columns[0].Width(28);
            table7.Columns[1].Width(28);
            table7.Columns[2].Width(28);
            table7.Columns[3].Width(28);
            table7.Columns[4].Width(28);
            table7.Columns[5].Width(28);
            table7.Columns[6].Width(28);
            table7.Columns[7].Width(28);
            table7.Columns[8].Width(28);
            table7.Columns[9].Width(28);
            for (int i = 10; i < 20; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table7.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table7.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table7.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                        }
                    }
                    i++;
                }
            table7.InsertRow(0,
            "Spot  " + PHouse[10].Spot + " | " + " " + PHouse[10].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[11].Spot + " | " + " " + PHouse[11].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[12].Spot + " | " + " " + PHouse[12].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[13].Spot + " | " + " " + PHouse[13].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[14].Spot + " | " + " " + PHouse[14].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[15].Spot + " | " + " " + PHouse[15].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[16].Spot + " | " + " " + PHouse[16].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[17].Spot + " | " + " " + PHouse[17].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[18].Spot + " | " + " " + PHouse[18].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot: " + PHouse[19].Spot + " | " + " " + PHouse[19].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"));

            rows = 1;
            var table8 = new Table();
            table8.AddColumns("", "", "", "", "", "", "", "", "", "");
            table8.HeavyBorder();
            table8.Columns[0].Width(28);
            table8.Columns[1].Width(28);
            table8.Columns[2].Width(28);
            table8.Columns[3].Width(28);
            table8.Columns[4].Width(28);
            table8.Columns[5].Width(28);
            table8.Columns[6].Width(28);
            table8.Columns[7].Width(28);
            table8.Columns[8].Width(28);
            table8.Columns[9].Width(28);
            for (int i = 10; i < 20; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            //table8.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            table8.Columns[y].Header(PHouse[i].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M") + "   " + PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            //table8.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            table8.Columns[y].Header(PHouse[i].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M") + "   " + PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            //table8.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            table8.Columns[y].Header(PHouse[i].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M") + "   " + PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                    }
                    i++;
                }
            //table8.InsertRow(0, PHouse[10].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[11].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[12].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[13].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[14].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[15].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[16].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[17].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[18].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[19].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"));
            table7.HideHeaders();
            table7.Border = TableBorder.Simple;
            //table8.Border = TableBorder.Simple;
            AnsiConsole.Write(table7);
            AnsiConsole.Write(table8);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //För rad 21-30
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 21-30
            rows = 1;
            var table9 = new Table();
            table9.AddColumns("21", "22", "23", "24", "25", "26", "27", "28", "29", "30");
            table9.HeavyBorder();
            table9.Columns[0].Width(28);
            table9.Columns[1].Width(28);
            table9.Columns[2].Width(28);
            table9.Columns[3].Width(28);
            table9.Columns[4].Width(28);
            table9.Columns[5].Width(28);
            table9.Columns[6].Width(28);
            table9.Columns[7].Width(28);
            table9.Columns[8].Width(28);
            table9.Columns[9].Width(28);
            for (int i = 20; i < 30; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table9.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table9.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table9.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                        }
                    }
                    i++;
                }
            table9.InsertRow(0,
            "Spot  " + PHouse[20].Spot + " | " + " " + PHouse[20].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[21].Spot + " | " + " " + PHouse[21].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[22].Spot + " | " + " " + PHouse[22].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[23].Spot + " | " + " " + PHouse[23].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[24].Spot + " | " + " " + PHouse[24].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[25].Spot + " | " + " " + PHouse[25].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[26].Spot + " | " + " " + PHouse[26].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[27].Spot + " | " + " " + PHouse[27].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[28].Spot + " | " + " " + PHouse[28].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[29].Spot + " | " + " " + PHouse[29].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"));

            rows = 1;
            var table10 = new Table();
            table10.AddColumns("", "", "", "", "", "", "", "", "", "");
            table10.HeavyBorder();
            table10.Columns[0].Width(28);
            table10.Columns[1].Width(28);
            table10.Columns[2].Width(28);
            table10.Columns[3].Width(28);
            table10.Columns[4].Width(28);
            table10.Columns[5].Width(28);
            table10.Columns[6].Width(28);
            table10.Columns[7].Width(28);
            table10.Columns[8].Width(28);
            table10.Columns[9].Width(28);
            for (int i = 20; i < 30; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            //table10.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            table10.Columns[y].Header(PHouse[i].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M") + "   " + PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            //table10.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            table10.Columns[y].Header(PHouse[i].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M") + "   " + PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            //table10.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            table10.Columns[y].Header(PHouse[i].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M") + "   " + PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                    }
                    i++;
                }
            //table10.InsertRow(0, 
            //PHouse[20].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[21].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[22].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[23].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[24].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[25].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[26].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[27].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[28].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"),
            //PHouse[29].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M"));
            table9.HideHeaders();
            table9.Border = TableBorder.Simple;
            AnsiConsole.Write(table9);
            AnsiConsole.Write(table10);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 31-40
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 31-40
            rows = 1;
            var table11 = new Table();
            table11.AddColumns("31", "32", "33", "34", "35", "36", "37", "38", "39", "40");
            table11.HeavyBorder();
            table11.Columns[0].Width(28);
            table11.Columns[1].Width(28);
            table11.Columns[2].Width(28);
            table11.Columns[3].Width(28);
            table11.Columns[4].Width(28);
            table11.Columns[5].Width(28);
            table11.Columns[6].Width(28);
            table11.Columns[7].Width(28);
            table11.Columns[8].Width(28);
            table11.Columns[9].Width(28);
            for (int i = 30; i < 40; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table11.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table11.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table11.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                        }
                    }
                    i++;
                }
            table11.InsertRow(0,
            "Spot  " + PHouse[30].Spot + " | " + " " + PHouse[30].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[31].Spot + " | " + " " + PHouse[31].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[32].Spot + " | " + " " + PHouse[32].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[33].Spot + " | " + " " + PHouse[33].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[34].Spot + " | " + " " + PHouse[34].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[35].Spot + " | " + " " + PHouse[35].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[36].Spot + " | " + " " + PHouse[36].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[37].Spot + " | " + " " + PHouse[37].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[38].Spot + " | " + " " + PHouse[38].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            "Spot  " + PHouse[39].Spot + " | " + " " + PHouse[39].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"));

            rows = 1;
            var table12 = new Table();
            table12.AddColumns("", "", "", "", "", "", "", "", "", "");
            table12.HeavyBorder();
            table12.Columns[0].Width(28);
            table12.Columns[1].Width(28);
            table12.Columns[2].Width(28);
            table12.Columns[3].Width(28);
            table12.Columns[4].Width(28);
            table12.Columns[5].Width(28);
            table12.Columns[6].Width(28);
            table12.Columns[7].Width(28);
            table12.Columns[8].Width(28);
            table12.Columns[9].Width(28);
            for (int i = 30; i < 40; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            //table12.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            table12.Columns[y].Header(PHouse[i].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M") + "   " + PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            //table12.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            table12.Columns[y].Header(PHouse[i].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M") + "   " + PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            //table12.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            table12.Columns[y].Header(PHouse[i].ParkedVehicles[1].TimeParked.ToString("HH:mm | d/M") + "   " + PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                    }
                    i++;
                }
            //table12.InsertRow(0, PHouse[30].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[31].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[32].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[33].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[34].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[35].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[36].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[37].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[38].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            //PHouse[39].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"));
            table11.HideHeaders();
            table11.Border = TableBorder.Simple;
            AnsiConsole.Write(table11);
            AnsiConsole.Write(table12);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 41-50
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 41-50
            rows = 1;
            var table13 = new Table();
            table13.AddColumns("41", "42", "43", "44", "45", "46", "47", "48", "49", "50");
            table13.HeavyBorder();
            table13.Columns[0].Width(28);
            table13.Columns[1].Width(28);
            table13.Columns[2].Width(28);
            table13.Columns[3].Width(28);
            table13.Columns[4].Width(28);
            table13.Columns[5].Width(28);
            table13.Columns[6].Width(28);
            table13.Columns[7].Width(28);
            table13.Columns[8].Width(28);
            table13.Columns[9].Width(28);
            for (int i = 40; i < 50; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table13.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table13.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table13.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                        }
                    }
                    i++;
                }
            table13.InsertRow(0, 
            PHouse[40].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[41].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[42].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[43].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[44].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[45].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[46].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[47].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[48].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[49].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"));

            rows = 1;
            var table14 = new Table();
            table14.AddColumns("", "", "", "", "", "", "", "", "", "");
            table14.HeavyBorder();
            table14.Columns[0].Width(28);
            table14.Columns[1].Width(28);
            table14.Columns[2].Width(28);
            table14.Columns[3].Width(28);
            table14.Columns[4].Width(28);
            table14.Columns[5].Width(28);
            table14.Columns[6].Width(28);
            table14.Columns[7].Width(28);
            table14.Columns[8].Width(28);
            table14.Columns[9].Width(28);
            for (int i = 40; i < 50; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table14.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table14.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table14.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                    }
                    i++;
                }
            table14.InsertRow(0, 
            PHouse[40].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[41].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[42].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[43].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[44].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[45].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[46].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[47].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[48].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[49].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"));
            AnsiConsole.Write(table13);
            AnsiConsole.Write(table14);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 51-60
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 51-60
            rows = 1;
            var table15 = new Table();
            table15.AddColumns("51", "52", "53", "54", "55", "56", "57", "58", "59", "60");
            table15.HeavyBorder();
            table15.Columns[0].Width(28);
            table15.Columns[1].Width(28);
            table15.Columns[2].Width(28);
            table15.Columns[3].Width(28);
            table15.Columns[4].Width(28);
            table15.Columns[5].Width(28);
            table15.Columns[6].Width(28);
            table15.Columns[7].Width(28);
            table15.Columns[8].Width(28);
            table15.Columns[9].Width(28);
            for (int i = 50; i < 60; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table15.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table15.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table15.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                        }
                    }
                    i++;
                }
            table15.InsertRow(0,
            PHouse[50].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[51].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[52].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[53].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[54].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[55].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[56].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[57].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[58].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[59].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"));

            rows = 1;
            var table16 = new Table();
            table16.AddColumns("", "", "", "", "", "", "", "", "", "");
            table16.HeavyBorder();
            table16.Columns[0].Width(28);
            table16.Columns[1].Width(28);
            table16.Columns[2].Width(28);
            table16.Columns[3].Width(28);
            table16.Columns[4].Width(28);
            table16.Columns[5].Width(28);
            table16.Columns[6].Width(28);
            table16.Columns[7].Width(28);
            table16.Columns[8].Width(28);
            table16.Columns[9].Width(28);
            for (int i = 50; i < 60; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table16.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table16.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table16.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                    }
                    i++;
                }
            table16.InsertRow(0,
            PHouse[50].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[51].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[52].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[53].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[54].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[55].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[56].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[57].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[58].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[59].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"));
            AnsiConsole.Write(table15);
            AnsiConsole.Write(table16);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 61-70
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 61-70
            rows = 1;
            var table17 = new Table();
            table17.AddColumns("61", "62", "63", "64", "65", "66", "67", "68", "69", "70");
            table17.HeavyBorder();
            table17.Columns[0].Width(28);
            table17.Columns[1].Width(28);
            table17.Columns[2].Width(28);
            table17.Columns[3].Width(28);
            table17.Columns[4].Width(28);
            table17.Columns[5].Width(28);
            table17.Columns[6].Width(28);
            table17.Columns[7].Width(28);
            table17.Columns[8].Width(28);
            table17.Columns[9].Width(28);
            for (int i = 60; i < 70; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table17.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table17.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table17.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                        }
                    }
                    i++;
                }
            table17.InsertRow(0,
            PHouse[60].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[61].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[62].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[63].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[64].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[65].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[66].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[67].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[68].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[69].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"));

            rows = 1;
            var table18 = new Table();
            table18.AddColumns("", "", "", "", "", "", "", "", "", "");
            table18.HeavyBorder();
            table18.Columns[0].Width(28);
            table18.Columns[1].Width(28);
            table18.Columns[2].Width(28);
            table18.Columns[3].Width(28);
            table18.Columns[4].Width(28);
            table18.Columns[5].Width(28);
            table18.Columns[6].Width(28);
            table18.Columns[7].Width(28);
            table18.Columns[8].Width(28);
            table18.Columns[9].Width(28);
            for (int i = 60; i < 70; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table18.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table18.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table18.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                    }
                    i++;
                }
            table18.InsertRow(0,
            PHouse[60].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[61].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[62].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[63].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[64].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[65].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[66].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[67].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[68].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[69].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"));
            AnsiConsole.Write(table17);
            AnsiConsole.Write(table18);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 71-80
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 71-80
            rows = 1;
            var table19 = new Table();
            table19.AddColumns("71", "72", "73", "74", "75", "76", "77", "78", "79", "80");
            table19.HeavyBorder();
            table19.Columns[0].Width(28);
            table19.Columns[1].Width(28);
            table19.Columns[2].Width(28);
            table19.Columns[3].Width(28);
            table19.Columns[4].Width(28);
            table19.Columns[5].Width(28);
            table19.Columns[6].Width(28);
            table19.Columns[7].Width(28);
            table19.Columns[8].Width(28);
            table19.Columns[9].Width(28);
            for (int i = 70; i < 80; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table19.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table19.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table19.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                        }
                    }
                    i++;
                }
            table19.InsertRow(0,
            PHouse[70].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[71].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[72].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[73].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[74].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[75].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[76].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[77].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[78].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[79].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"));

            rows = 1;
            var table20 = new Table();
            table20.AddColumns("", "", "", "", "", "", "", "", "", "");
            table20.HeavyBorder();
            table20.Columns[0].Width(28);
            table20.Columns[1].Width(28);
            table20.Columns[2].Width(28);
            table20.Columns[3].Width(28);
            table20.Columns[4].Width(28);
            table20.Columns[5].Width(28);
            table20.Columns[6].Width(28);
            table20.Columns[7].Width(28);
            table20.Columns[8].Width(28);
            table20.Columns[9].Width(28);
            for (int i = 70; i < 80; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table20.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table20.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table20.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                    }
                    i++;
                }
            table20.InsertRow(0,
            PHouse[70].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[71].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[72].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[73].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[74].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[75].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[76].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[77].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[78].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[79].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"));
            AnsiConsole.Write(table19);
            AnsiConsole.Write(table20);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------


            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 81-90
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 81-90
            rows = 1;
            var table21 = new Table();
            table21.AddColumns("81", "82", "83", "84", "85", "86", "87", "88", "89", "90");
            table21.HeavyBorder();
            table21.Columns[0].Width(28);
            table21.Columns[1].Width(28);
            table21.Columns[2].Width(28);
            table21.Columns[3].Width(28);
            table21.Columns[4].Width(28);
            table21.Columns[5].Width(28);
            table21.Columns[6].Width(28);
            table21.Columns[7].Width(28);
            table21.Columns[8].Width(28);
            table21.Columns[9].Width(28);
            for (int i = 80; i < 90; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table21.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table21.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table21.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                        }
                    }
                    i++;
                }
            table21.InsertRow(0,
            PHouse[80].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[81].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[82].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[83].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[84].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[85].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[86].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[87].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[88].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[89].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"));

            rows = 1;
            var table22 = new Table();
            table22.AddColumns("", "", "", "", "", "", "", "", "", "");
            table22.HeavyBorder();
            table22.Columns[0].Width(28);
            table22.Columns[1].Width(28);
            table22.Columns[2].Width(28);
            table22.Columns[3].Width(28);
            table22.Columns[4].Width(28);
            table22.Columns[5].Width(28);
            table22.Columns[6].Width(28);
            table22.Columns[7].Width(28);
            table22.Columns[8].Width(28);
            table22.Columns[9].Width(28);
            for (int i = 80; i < 90; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table22.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table22.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table22.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                    }
                    i++;
                }
            table22.InsertRow(0,
            PHouse[80].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[81].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[82].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[83].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[84].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[85].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[86].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[87].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[88].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[89].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"));
            AnsiConsole.Write(table21);
            AnsiConsole.Write(table22);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 91-100
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 91-100
            rows = 1;
            var table23 = new Table();
            table23.AddColumns("91", "92", "93", "94", "95", "96", "97", "98", "99", "100");
            table23.HeavyBorder();
            table23.Columns[0].Width(28);
            table23.Columns[1].Width(28);
            table23.Columns[2].Width(28);
            table23.Columns[3].Width(28);
            table23.Columns[4].Width(28);
            table23.Columns[5].Width(28);
            table23.Columns[6].Width(28);
            table23.Columns[7].Width(28);
            table23.Columns[8].Width(28);
            table23.Columns[9].Width(28);
            for (int i = 90; i < 100; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table23.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table23.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table23.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
                        }
                    }
                    i++;
                }
            table23.InsertRow(0,
            PHouse[90].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[91].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[92].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[93].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[94].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[95].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[96].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[97].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[98].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
            PHouse[99].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"));

            rows = 1;
            var table24 = new Table();
            table24.AddColumns("", "", "", "", "", "", "", "", "", "");
            table24.HeavyBorder();
            table24.Columns[0].Width(28);
            table24.Columns[1].Width(28);
            table24.Columns[2].Width(28);
            table24.Columns[3].Width(28);
            table24.Columns[4].Width(28);
            table24.Columns[5].Width(28);
            table24.Columns[6].Width(28);
            table24.Columns[7].Width(28);
            table24.Columns[8].Width(28);
            table24.Columns[9].Width(28);
            for (int i = 90; i < 100; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            table24.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table24.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                        else if (PHouse[i].ParkedVehicles[1].GetType().ToString() == "MC")
                        {
                            table24.Columns[y].Header(PHouse[i].ParkedVehicles[1].PlateNumber);
                            rows++;
                        }
                    }
                    i++;
                }
            table24.InsertRow(0,
            PHouse[90].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[91].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[92].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[93].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[94].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[95].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[96].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[97].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[98].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
            PHouse[99].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"));
            table23.Border = TableBorder.Double;
            AnsiConsole.Write(table23);
            AnsiConsole.Write(table24);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------




            //AnsiConsole.Write(table5);
            //AnsiConsole.Write(table8);
            //var table6 = new Table();
            //table6.AddColumns("1", "2", "3", "4", "5", "6", "7", "8", "9", "10");
            //table6.HideHeaders();
            //table6.Expand();
            //rows = 1;
            //for (int y = 10; y <= 20; i++)
            //    for (int y = 0; y < 10; y++)
            //    {
            //        if (PHouse[i].ParkedVehicles.Count == 0)
            //        {
            //            table6.Columns[y].Footer("Empty spot");
            //            AnsiConsole.Write(table6);
            //            rows++;
            //        }
            //        else if (PHouse[i].ParkedVehicles.Count == 1)
            //        {
            //            if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
            //            {
            //                table6.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);

            //                AnsiConsole.Write(table6);
            //                rows++;
            //            }
            //            else
            //            {
            //                table6.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
            //                AnsiConsole.Write(table6);
            //                rows++;
            //            }
            //        }

            //        i++;
            //        continue;
            //    }
            //var table7 = new Table();
            //table7.AddColumns("1", "2", "3", "4", "5", "6", "7", "8", "9", "10");
            //table7.HideHeaders();
            //table7.Expand();
            //rows = 1;
            //    for (int i = 20; i <= 30; i++)
            //        for (int y = 0; y < 10; y++)
            //        {
            //            if (PHouse[i].ParkedVehicles.Count == 0)
            //            {
            //                table7.Columns[y].Footer("Empty spot");
            //                rows++;
            //            }
            //            else if (PHouse[i].ParkedVehicles.Count == 1)
            //            {
            //                if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
            //                {
            //                    table7.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
            //                    rows++;
            //                }
            //                else
            //                {
            //                    table7.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
            //                    rows++;
            //                }
            //            }
            //            i++;
            //            continue;
            //        }
            //    AnsiConsole.Write(table5);
            //    AnsiConsole.Write(table6);
            //    AnsiConsole.Write(table7);
            //}



        }
    }
}

//public void ShowParkingViewLarge()
//{
//    ParkingSpot parkingSpot = new ParkingSpot();

//    List<ParkingSpot> clonedList = new List<ParkingSpot>(PHouse);

//    var clonedList2 = PHouse;

//    for (int i = 0; i < clonedList2.Count; i++)
//    {
//        if (clonedList2[i].ParkedVehicles.Count == 0)
//        {
//            clonedList2[i].ParkedVehicles.Insert(0, new Vehicle());
//            clonedList2[i].ParkedVehicles.Insert(1, new Vehicle());
//            clonedList2[i].ParkedVehicles[0].PlateNumber = String.Empty;
//            clonedList2[i].ParkedVehicles[0].TimeParked = DateTime.MinValue;
//            clonedList2[i].ParkedVehicles[1].PlateNumber = String.Empty;
//            clonedList2[i].ParkedVehicles[1].TimeParked = DateTime.MinValue;
//        }
//        else if (clonedList2[i].ParkedVehicles.Count == 1 && clonedList2[i].ParkedVehicles[0].GetType().ToString() == "Car")
//        {
//            clonedList2[i].ParkedVehicles.Insert(1, new Vehicle());
//            clonedList2[i].ParkedVehicles[1].PlateNumber = String.Empty;
//            clonedList2[i].ParkedVehicles[1].TimeParked = DateTime.MinValue;
//        }
//        else if (clonedList2[i].ParkedVehicles.Count == 1 && clonedList2[i].ParkedVehicles[0].GetType().ToString() == "MC")
//        {
//            clonedList2[i].ParkedVehicles.Insert(1, new Vehicle());
//            clonedList2[i].ParkedVehicles[1].PlateNumber = String.Empty;
//            clonedList2[i].ParkedVehicles[1].TimeParked = DateTime.MinValue;
//        }
//    }

//    //int column = 20;
//    int rows = 1;
//    var table5 = new Table();
//    Console.WriteLine("\nBelow is the parking lot with current parked vehicles.\n");
//    table5.AddColumns("1", "2", "3", "4", "5", "6", "7", "8", "9", "10");
//    table5.HideHeaders();
//    table5.Expand();
//    AnsiConsole.Write(table5);
//    for (int i = 0; i < 10; i++)
//    {
//        //if (rows >= column && rows % column == 0)
//        //{
//        //    var table7 = new Table();
//        //    Console.WriteLine();
//        //    table7.AddColumns("1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1", "1");
//        //    AnsiConsole.Write(table7);
//        //    //rows = 1;
//        //}
//        if (PHouse[i].ParkedVehicles.Count == 0)
//        {
//            //table5.Columns[i].Footer("Empty spot");
//            //AnsiConsole.Write(table5);
//            rows++;
//        }
//        //if (PHouse[i].ParkedVehicles.Count == 2)
//        //{
//        //    var table5 = new Table();
//        //    //table5.AddColumns("vehicle 1", "vehicle 1");
//        //    //table5.HideHeaders();
//        //    table5.AddRow((PHouse[i].ParkedVehicles[0].PlateNumber), (PHouse[i].ParkedVehicles[1].PlateNumber));
//        //    table5.AddRow(PHouse[i].ParkedVehicles[0].timeParked.ToString("HH:mm - d/M"), (PHouse[i].ParkedVehicles[0].timeParked.ToString("HH:mm - d/M")));

//        //    table6.Border = TableBorder.Rounded;
//        //    AnsiConsole.Write(table6);
//        //    //Console.Write("\t");
//        //    rows++;
//        //}
//        else if (PHouse[i].ParkedVehicles.Count != 0)
//        {
//            if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
//            {
//                //var table5 = new Table();
//                //table5.AddColumns("vehicle 1");
//                //table6.AddRow((PHouse[i].ParkedVehicles[0].PlateNumber));
//                //table6.AddRow(PHouse[i].ParkedVehicles[0].timeParked.ToString("HH:mm - d/M"));
//                // table5.Width(37);
//                //table6.Border = TableBorder.Rounded;
//                table5.Columns[i].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
//                //table5.AddRow(PHouse[i].ParkedVehicles[0].timeParked.ToString("HH:mm - d/M"));
//                //AnsiConsole.Write(table5);
//                //Console.Write("\t");
//                //AnsiConsole.Write(
//                //new Table()
//                //    .AddColumn(new TableColumn((PHouse[i].ParkedVehicles[0].PlateNumber).ToString()))
//                //    .AddColumn(new TableColumn((PHouse[i].ParkedVehicles[1].PlateNumber).ToString()))
//                //    .AddRow((PHouse[i].ParkedVehicles[0].TimeParked).ToString("HH:mm - d/M"))
//                //    .AddRow((PHouse[i].ParkedVehicles[1].TimeParked).ToString("HH:mm - d/M")));
//                //AnsiConsole.Write(table5);

//                rows++;
//            }
//            else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
//            {
//                //var table5 = new Table();
//                //table6.AddColumns("vehicle 1");
//                //table5.HideHeaders();
//                //table6.AddColumns((PHouse[i].ParkedVehicles[0].PlateNumber));
//                //table6.AddRow(PHouse[i].ParkedVehicles[0].timeParked.ToString("HH:mm - d/M"));
//                //table6.Width(37);
//                //table6.Border = TableBorder.Rounded;
//                table5.Columns[i].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
//                //table5.InsertRow(r, PHouse[i].ParkedVehicles[0].timeParked.ToString("HH:mm - d/M"));
//                //AnsiConsole.Write(table5);
//                //Console.Write("\t");

//                rows++;
//            }
//            else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC" && PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
//            {
//                table5.Columns[i].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
//            }
//        }
//    }

//    table5.InsertRow(0, PHouse[0].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
//    PHouse[1].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
//    PHouse[2].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
//    PHouse[3].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
//    PHouse[4].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
//    PHouse[5].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
//    PHouse[6].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
//    PHouse[7].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
//    PHouse[8].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
//    PHouse[9].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"));
//    //table5.InsertRow(0, PHouse[1].ParkedVehicles[0].TimeParked.ToString("HH:mm - d/M"),
//    //    PHouse[1].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M").ToString(),
//    //    PHouse[2].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
//    //    PHouse[3].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
//    //    PHouse[4].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
//    //    PHouse[5].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
//    //    PHouse[6].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
//    //    PHouse[7].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
//    //    PHouse[8].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"),
//    //    PHouse[9].ParkedVehicles[1].TimeParked.ToString("HH:mm - d/M"));
//    AnsiConsole.Write(table5);
//    var table6 = new Table();
//    table6.AddColumns("1", "2", "3", "4", "5", "6", "7", "8", "9", "10");
//    table6.HideHeaders();
//    table6.Expand();
//    rows = 1;
//    for (int i = 10; i <= 20; i++)
//        for (int y = 0; y < 10; y++)
//        {
//            if (PHouse[i].ParkedVehicles.Count == 0)
//            {
//                table6.Columns[y].Footer("Empty spot");
//                AnsiConsole.Write(table6);
//                rows++;
//            }
//            else if (PHouse[i].ParkedVehicles.Count == 1)
//            {
//                if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
//                {
//                    table6.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);

//                    AnsiConsole.Write(table6);
//                    rows++;
//                }
//                else
//                {
//                    table6.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
//                    AnsiConsole.Write(table6);
//                    rows++;
//                }
//            }

//            i++;
//            continue;
//        }
//    var table7 = new Table();
//    table7.AddColumns("1", "2", "3", "4", "5", "6", "7", "8", "9", "10");
//    table7.HideHeaders();
//    table7.Expand();
//    rows = 1;
//    for (int i = 20; i <= 30; i++)
//        for (int y = 0; y < 10; y++)
//        {
//            if (PHouse[i].ParkedVehicles.Count == 0)
//            {
//                table7.Columns[y].Footer("Empty spot");
//                rows++;
//            }
//            else if (PHouse[i].ParkedVehicles.Count == 1)
//            {
//                if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
//                {
//                    table7.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
//                    rows++;
//                }
//                else
//                {
//                    table7.Columns[y].Footer(PHouse[i].ParkedVehicles[0].PlateNumber);
//                    rows++;
//                }
//            }
//            i++;
//            continue;
//        }
//    AnsiConsole.Write(table5);
//    AnsiConsole.Write(table6);
//    AnsiConsole.Write(table7);
//}
//    }
//}
//table5.AddColumn(new TableColumn("Spot: " + item.Spot.ToString() + " " + "Type: " + item.GetType().ToString()));
//                    table5.AddRow(item.PlateNumber);
//                    table5.AddRow(item.timeParked.ToString("HH:mm - d/M"));
//                    table5.Width(21);
//                    table5.Border = TableBorder.Rounded;
//                    rows++;

