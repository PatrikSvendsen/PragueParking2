using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
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

            //var rule1 = new Rule();
            //rule1.Style = Style.Parse("red dim");
            //rule2.Style = Style.Parse("blue");
            Console.Clear();

            //För rad 1-10
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            #region Row 1-10
            var table5 = new Table();

            table5.AddColumns("1", "2", "3", "4", "5", "6", "7", "8", "9", "10");
            table5.HeavyBorder();

            for (int i = 0; i < 10; i++)
            {
                if (PHouse[i].ParkedVehicles.Count != 0)
                {
                    if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                    {
                        if (PHouse[i].ParkedVehicles.Count == 2)
                        {
                            table5.Columns[i].Footer("[deeppink3]" + PHouse[i].ParkedVehicles[0].PlateNumber + "     " + PHouse[i].ParkedVehicles[1].PlateNumber + "[/]");
                        }
                        else
                        {
                            table5.Columns[i].Footer("[green3_1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                        }
                    }
                    else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                    {
                        table5.Columns[i].Footer("[orangered1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                    }
                }
                table5.Columns[i].Width(28);
            }
            table5.InsertRow(0,
            "Spot: " + PHouse[0].Spot,
            "Spot: " + PHouse[1].Spot,
            "Spot: " + PHouse[2].Spot,
            "Spot: " + PHouse[3].Spot,
            "Spot: " + PHouse[4].Spot,
            "Spot: " + PHouse[5].Spot,
            "Spot: " + PHouse[6].Spot,
            "Spot: " + PHouse[7].Spot,
            "Spot: " + PHouse[8].Spot,
            "Spot: " + PHouse[9].Spot).Centered();

            table5.HideHeaders();
            table5.Border = TableBorder.SimpleHeavy;
            AnsiConsole.Write(table5);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //För rad 11-20
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 11-20
            var table7 = new Table();

            table7.AddColumns("11", "12", "13", "14", "15", "16", "17", "18", "19", "20");
            table7.HeavyBorder();

            for (int i = 10; i < 20; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            if (PHouse[i].ParkedVehicles.Count == 2)
                            {
                                table7.Columns[y].Footer("[deeppink3]" + PHouse[i].ParkedVehicles[0].PlateNumber + "     " + PHouse[i].ParkedVehicles[1].PlateNumber + "[/]");
                            }
                            else
                            {
                                table7.Columns[y].Footer("[green3_1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                            }
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table7.Columns[y].Footer("[orangered1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                        }
                    }
                    table7.Columns[y].Width(28);
                    i++;
                }
            table7.InsertRow(0,
            "Spot  " + PHouse[10].Spot,
            "Spot: " + PHouse[11].Spot,
            "Spot: " + PHouse[12].Spot,
            "Spot: " + PHouse[13].Spot,
            "Spot: " + PHouse[14].Spot,
            "Spot: " + PHouse[15].Spot,
            "Spot: " + PHouse[16].Spot,
            "Spot: " + PHouse[17].Spot,
            "Spot: " + PHouse[18].Spot,
            "Spot: " + PHouse[19].Spot).Centered();

            table7.HideHeaders();
            table7.Border = TableBorder.SimpleHeavy;
            AnsiConsole.Write(table7);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //För rad 21-30
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 21-30
            var table9 = new Table();
            table9.AddColumns("21", "22", "23", "24", "25", "26", "27", "28", "29", "30");
            table9.HeavyBorder();

            for (int i = 20; i < 30; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            if (PHouse[i].ParkedVehicles.Count == 2)
                            {
                                table7.Columns[y].Footer("[deeppink3]" + PHouse[i].ParkedVehicles[0].PlateNumber + "     " + PHouse[i].ParkedVehicles[1].PlateNumber + "[/]");
                            }
                            else
                            {
                                table9.Columns[y].Footer("[green3_1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                            }
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table9.Columns[y].Footer("[orangered1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                        }
                    }
                    table9.Columns[y].Width(28);
                    i++;
                }
            table9.InsertRow(0,
            "Spot  " + PHouse[20].Spot,
            "Spot  " + PHouse[21].Spot,
            "Spot  " + PHouse[22].Spot,
            "Spot  " + PHouse[23].Spot,
            "Spot  " + PHouse[24].Spot,
            "Spot  " + PHouse[25].Spot,
            "Spot  " + PHouse[26].Spot,
            "Spot  " + PHouse[27].Spot,
            "Spot  " + PHouse[28].Spot,
            "Spot  " + PHouse[29].Spot).Centered();

            table9.HideHeaders();
            table9.Border = TableBorder.SimpleHeavy;
            AnsiConsole.Write(table9);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 31-40
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 31-40
            var table11 = new Table();
            table11.AddColumns("31", "32", "33", "34", "35", "36", "37", "38", "39", "40");
            table11.HeavyBorder();

            for (int i = 30; i < 40; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            if (PHouse[i].ParkedVehicles.Count == 2)
                            {
                                table11.Columns[y].Footer("[deeppink3]" + PHouse[i].ParkedVehicles[0].PlateNumber + "     " + PHouse[i].ParkedVehicles[1].PlateNumber + "[/]");
                            }
                            else
                            {
                                table11.Columns[y].Footer("[green3_1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                            }
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table11.Columns[y].Footer("[orangered1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                        }
                    }
                    table11.Columns[y].Width(28);
                }
            table11.InsertRow(0,
            "Spot  " + PHouse[30].Spot,
            "Spot  " + PHouse[31].Spot,
            "Spot  " + PHouse[32].Spot,
            "Spot  " + PHouse[33].Spot,
            "Spot  " + PHouse[34].Spot,
            "Spot  " + PHouse[35].Spot,
            "Spot  " + PHouse[36].Spot,
            "Spot  " + PHouse[37].Spot,
            "Spot  " + PHouse[38].Spot,
            "Spot  " + PHouse[39].Spot).Centered();

            table11.HideHeaders();
            table11.Border = TableBorder.SimpleHeavy;
            AnsiConsole.Write(table11);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 41-50
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 41-50
            var table13 = new Table();
            table13.AddColumns("41", "42", "43", "44", "45", "46", "47", "48", "49", "50");
            table13.HeavyBorder();

            for (int i = 40; i < 50; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            if (PHouse[i].ParkedVehicles.Count == 2)
                            {
                                table13.Columns[y].Footer("[deeppink3]" + PHouse[i].ParkedVehicles[0].PlateNumber + "     " + PHouse[i].ParkedVehicles[1].PlateNumber + "[/]");
                            }
                            else
                            {
                                table13.Columns[y].Footer("[green3_1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                            }
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table13.Columns[y].Footer("[orangered1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                        }
                    }

                    table13.Columns[y].Width(28);
                    i++;
                }
            table13.InsertRow(0,
            "Spot  " + PHouse[40].Spot,
            "Spot  " + PHouse[41].Spot,
            "Spot  " + PHouse[42].Spot,
            "Spot  " + PHouse[43].Spot,
            "Spot  " + PHouse[44].Spot,
            "Spot  " + PHouse[45].Spot,
            "Spot  " + PHouse[46].Spot,
            "Spot  " + PHouse[47].Spot,
            "Spot  " + PHouse[48].Spot,
            "Spot  " + PHouse[49].Spot).Centered();

            table13.HideHeaders();
            table13.Border = TableBorder.SimpleHeavy;
            AnsiConsole.Write(table13);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 51-60
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 51-60
            var table15 = new Table();
            table15.AddColumns("51", "52", "53", "54", "55", "56", "57", "58", "59", "60");
            table15.HeavyBorder();
            for (int i = 50; i < 60; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            if (PHouse[i].ParkedVehicles.Count == 2)
                            {
                                table15.Columns[y].Footer("[deeppink3]" + PHouse[i].ParkedVehicles[0].PlateNumber + "     " + PHouse[i].ParkedVehicles[1].PlateNumber + "[/]");
                            }
                            else
                            {
                                table15.Columns[y].Footer("[green3_1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                            }
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table15.Columns[y].Footer("[orangered1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                        }
                    }
                    table15.Columns[y].Width(28);
                    i++;
                }
            table15.InsertRow(0,
            "Spot  " + PHouse[50].Spot,
            "Spot  " + PHouse[51].Spot,
            "Spot  " + PHouse[52].Spot,
            "Spot  " + PHouse[53].Spot,
            "Spot  " + PHouse[54].Spot,
            "Spot  " + PHouse[55].Spot,
            "Spot  " + PHouse[56].Spot,
            "Spot  " + PHouse[57].Spot,
            "Spot  " + PHouse[58].Spot,
            "Spot  " + PHouse[59].Spot).Centered();

            table15.HideHeaders();
            table15.Border = TableBorder.SimpleHeavy;
            AnsiConsole.Write(table15);
            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 61-70
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 61-70
            var table17 = new Table();
            table17.AddColumns("61", "62", "63", "64", "65", "66", "67", "68", "69", "70");
            table17.HeavyBorder();

            for (int i = 60; i < 70; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            if (PHouse[i].ParkedVehicles.Count == 2)
                            {
                                table17.Columns[y].Footer("[deeppink3]" + PHouse[i].ParkedVehicles[0].PlateNumber + "     " + PHouse[i].ParkedVehicles[1].PlateNumber + "[/]");
                            }
                            else
                            {
                                table17.Columns[y].Footer("[green3_1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                            }
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table17.Columns[y].Footer("[orangered1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                        }
                    }
                    table17.Columns[y].Width(28);
                    i++;
                }
            table17.InsertRow(0,
            "Spot  " + PHouse[60].Spot,
            "Spot  " + PHouse[61].Spot,
            "Spot  " + PHouse[62].Spot,
            "Spot  " + PHouse[63].Spot,
            "Spot  " + PHouse[64].Spot,
            "Spot  " + PHouse[65].Spot,
            "Spot  " + PHouse[66].Spot,
            "Spot  " + PHouse[67].Spot,
            "Spot  " + PHouse[68].Spot,
            "Spot  " + PHouse[69].Spot).Centered();

            table17.HideHeaders();
            table17.Border = TableBorder.SimpleHeavy;
            AnsiConsole.Write(table17);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 71-80
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 71-80
            var table19 = new Table();
            table19.AddColumns("71", "72", "73", "74", "75", "76", "77", "78", "79", "80");
            table19.HeavyBorder();
            for (int i = 70; i < 80; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            if (PHouse[i].ParkedVehicles.Count == 2)
                            {
                                table19.Columns[y].Footer("[deeppink3]" + PHouse[i].ParkedVehicles[0].PlateNumber + "     " + PHouse[i].ParkedVehicles[1].PlateNumber + "[/]");
                            }
                            else
                            {
                                table19.Columns[y].Footer("[green3_1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                            }
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table19.Columns[y].Footer("[orangered1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                        }
                    }
                    table19.Columns[y].Width(28);
                    i++;
                }
            table19.InsertRow(0,
            "Spot  " + PHouse[70].Spot,
            "Spot  " + PHouse[71].Spot,
            "Spot  " + PHouse[72].Spot,
            "Spot  " + PHouse[73].Spot,
            "Spot  " + PHouse[74].Spot,
            "Spot  " + PHouse[75].Spot,
            "Spot  " + PHouse[76].Spot,
            "Spot  " + PHouse[77].Spot,
            "Spot  " + PHouse[78].Spot,
            "Spot  " + PHouse[79].Spot).Centered();

            table19.HideHeaders();
            table19.Border = TableBorder.SimpleHeavy;
            AnsiConsole.Write(table19);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------


            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            //För rad 81-90
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            #region Row 81-90
            var table22 = new Table();
            table22.AddColumns("81", "82", "83", "84", "85", "86", "87", "88", "89", "90");
            table22.HeavyBorder();
            for (int i = 80; i < 90; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            if (PHouse[i].ParkedVehicles.Count == 2)
                            {
                                table22.Columns[y].Footer("[deeppink3]" + PHouse[i].ParkedVehicles[0].PlateNumber + "     " + PHouse[i].ParkedVehicles[1].PlateNumber + "[/]");
                            }
                            else
                            {
                                table22.Columns[y].Footer("[green3_1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                            }
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table22.Columns[y].Footer("[orangered1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                        }
                    }
                    table22.Columns[y].Width(28);
                    i++;
                }
            table22.InsertRow(0,
            "Spot  " + PHouse[80].Spot,
            "Spot  " + PHouse[81].Spot,
            "Spot  " + PHouse[82].Spot,
            "Spot  " + PHouse[83].Spot,
            "Spot  " + PHouse[84].Spot,
            "Spot  " + PHouse[85].Spot,
            "Spot  " + PHouse[86].Spot,
            "Spot  " + PHouse[87].Spot,
            "Spot  " + PHouse[88].Spot,
            "Spot  " + PHouse[89].Spot).Centered();
            table22.HideHeaders();
            table22.Border = TableBorder.SimpleHeavy;
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
            var table23 = new Table();
            table23.AddColumns("91", "92", "93", "94", "95", "96", "97", "98", "99", "100");
            table23.HeavyBorder();
            for (int i = 90; i < 100; i++)
                for (int y = 0; y < 10; y++)
                {
                    if (PHouse[i].ParkedVehicles.Count != 0)
                    {
                        if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "MC")
                        {
                            if (PHouse[i].ParkedVehicles.Count == 2)
                            {
                                table23.Columns[y].Footer("[deeppink3]" + PHouse[i].ParkedVehicles[0].PlateNumber + "     " + PHouse[i].ParkedVehicles[1].PlateNumber + "[/]");
                            }
                            else
                            {
                                table23.Columns[y].Footer("[green3_1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                            }
                        }
                        else if (PHouse[i].ParkedVehicles[0].GetType().ToString() == "Car")
                        {
                            table23.Columns[y].Footer("[orangered1]" + PHouse[i].ParkedVehicles[0].PlateNumber + "[/]");
                        }
                    }
                    table23.Columns[y].Width(28);
                    i++;
                }
            table23.InsertRow(0,
            "Spot  " + PHouse[90].Spot,
            "Spot  " + PHouse[91].Spot,
            "Spot  " + PHouse[92].Spot,
            "Spot  " + PHouse[93].Spot,
            "Spot  " + PHouse[94].Spot,
            "Spot  " + PHouse[95].Spot,
            "Spot  " + PHouse[96].Spot,
            "Spot  " + PHouse[97].Spot,
            "Spot  " + PHouse[98].Spot,
            "Spot  " + PHouse[99].Spot).Centered();

            table23.HideHeaders();
            table23.Border = TableBorder.SimpleHeavy;
            AnsiConsole.Write(table23);

            #endregion
            //-------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------

            Table table30 = new Table()
                .AddColumn("Currently parked vehicles")
                .Width(100)
                .Centered();


            AnsiConsole.Write(table30);





            Console.ReadKey();
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

