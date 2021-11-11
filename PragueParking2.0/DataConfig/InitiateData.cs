using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PragueParking2._0.DataConfig
{
    /*
     * Tack till Rasmus L för denna hjälp.
     */

    public class InitiateData
    {
        public static void LoadVehicleFromFile()
        {
            string filePath = @"../../../DataConfig/parkedVehicle.json";
            string convertJson = File.ReadAllText(filePath);
            List<Vehicle> vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(convertJson).ToList();

            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle.Size == 4)
                {
                    ParkingHouse.ReParkVehicle(new Car(vehicle.PlateNumber, vehicle.Price, vehicle.Spot, vehicle.timeParked), vehicle.Spot); // läser in och lägger fordon på rätt parkeringsplats.
                }
                else if (vehicle.Size == 2)
                {
                    ParkingHouse.ReParkVehicle(new MC(vehicle.PlateNumber, vehicle.Price, vehicle.Spot, vehicle.timeParked), vehicle.Spot);
                }
            }
        }

        public static void SetValuesFromConfig()
        {
            string filePath = @"../../../DataConfig/ConfigValues.json";
            string jsonConfig = File.ReadAllText(filePath);
            JsonConvert.DeserializeObject<ConfigValues>(jsonConfig);
        }

        public static void SaveVehicleToFile()
        {
            //string filepath = @"../../../dataconfig/parkedvehicle.json";
            //string parkedvehicles = JsonConvert.SerializeObject(ParkingSpot.ParkedVehicles);
            //File.WriteAllText(filepath, parkedvehicles);
        }

        //[{"PlateNumber":"MAF505","Size":4,"Price":20,"Spot":1,"timeParked":"2021-11-11T15:01:55.5035927+01:00"},
        //{"PlateNumber":"AAA111","Size":2,"Price":10,"Spot":3,"timeParked":"2021-11-11T15:03:16.486994+01:00"},
        //{ "PlateNumber":"MAF450","Size":4,"Price":20,"Spot":5,"timeParked":"2021-11-11T15:02:16.9346886+01:00"}]

    }
}
