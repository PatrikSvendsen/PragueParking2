using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PragueParking2._0.DataConfig
{
    internal class Config
    {
        public static int CarSize { get; set; }
        //public static int MCSize { get; set; }
        //public static int ParkingSpotSize { get; set; }
        //public static int ParkingHouseSize { get; set; }
        //public static int CarPrice { get; set; }
        //public static int MCPrice { get; set; }
        //public static int FreeMin { get; set; }

        public static void InitiateData()
        {
            string filePath = @"../../../DataConfig/parkedVehicle.txt";
            List<string> dumpList = File.ReadAllLines(filePath).ToList();

            foreach (var data in dumpList)
            {
                if (data.Contains("CarSize"))
                {
                    string[] carSize = data.Split(":");
                    CarSize = int.Parse(carSize[1]);
                }

                //else if (data.Contains("McSize"))
                //{
                //    string[] mcSize = data.Split(":");
                //    MCSize = int.Parse(mcSize[1]);
                //}

                //else if (data.Contains("ParkingSpot"))
                //{
                //    string[] spotSize = data.Split(":");
                //    ParkingSpotSize = int.Parse(spotSize[1]);
                //}

                //else if (data.Contains("ParkingGarage"))
                //{
                //    string[] ParkingGarageSize = data.Split(":");
                //    ParkingHouseSize = int.Parse(ParkingGarageSize[1]);
                //}

                //else if (data.Contains("CarPrice"))
                //{
                //    string[] carPrice = data.Split(":");
                //    CarPrice = int.Parse(carPrice[1]);
                //}

                //else if (data.Contains("MCPrice"))
                //{
                //    string[] mcPrice = data.Split(":");
                //    MCPrice = int.Parse(mcPrice[1]);
                //}

                //else if (data.Contains("FreeTime"))
                //{
                //    string[] freeTime = data.Split(":");
                //    FreeMin = int.Parse(freeTime[1]);
                //}

                //else
                //{
                //    Console.Clear();
                //    Console.ForegroundColor = ConsoleColor.Red;
                //    var pHouseError = new Table()
                //        .AddColumn(new TableColumn("Error - Something went wrong when reading text document 'ParkingHouseInfo'."))
                //        .Width(100);
                //    AnsiConsole.Write(pHouseError);
                //    Console.ResetColor();
                //}
            }
        }

        public static void ReadVehicleList()
        {
            string fileLocation = @"../../../Documents/ParkingVehicleList.txt";
            string jsonTxt = File.ReadAllText(fileLocation);

            List<Vehicle> data = JsonSerializer.Deserialize<List<Vehicle>>(jsonTxt).ToList();

            foreach (Vehicle vehicle in data)
            {

            }
        }

    }
}
