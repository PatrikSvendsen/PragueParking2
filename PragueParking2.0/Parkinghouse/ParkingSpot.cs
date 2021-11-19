using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PragueParking2._0.DataConfig;

namespace PragueParking2._0
{
    class ParkingSpot
    {
        public List<Vehicle> ParkedVehicles { get; set; } = new List<Vehicle>();

        internal int Spot { get; set; }
        private int AvailableSize { get; set; }

        public ParkingSpot(int spotSize, int spot)
        {
            Spot = spot;
            AvailableSize = spotSize;
        }
        public ParkingSpot(int spot, Vehicle vehicle)
        {
            Spot = spot;
        }
        public ParkingSpot()
        {
            
        }

        public override string ToString()
        {
            return Spot.ToString();
        }

        /// <summary>
        /// Method to park the vehicle to an empty spot.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="spot"></param>
        /// <returns></returns>
        internal bool ParkVehicleToSpot(Vehicle vehicle, int spot)
        {
            foreach (var line in ParkingHouse.PHouse)
            {
                if (line.Spot == spot + 1)
                {
                    ParkedVehicles.Add(vehicle);
                    AvailableSize -= vehicle.Size;
                    vehicle.Spot = Spot;
                    return true;
                }
            } 
            return false;
        }

        /// <summary>
        /// Method to check for size left on the parkingspot.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool CheckSpace(Vehicle vehicle)
        {
            if (AvailableSize == ConfigValues.ParkingSpotSize & vehicle.Size == ConfigValues.CarSize)
            {
                return true;
            }
            else if (AvailableSize == ConfigValues.ParkingSpotSize || AvailableSize == ConfigValues.MCSize & vehicle.Size == ConfigValues.MCSize)
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Method to find a specific vehicle using platenumber.
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        public bool FindVehicle(string plateNumber)
        {
            bool check = false;
            foreach (var item in ParkedVehicles)
            {
                if (check = item.PlateNumber == plateNumber)
                {
                    return check;
                }
            }
            return check;
        }

        /// <summary>
        /// Method to remove obj vehicle from the list.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool RemoveVehicle(Vehicle vehicle)
        {
            bool check = false;
            check = ParkedVehicles.Remove(vehicle);
            AvailableSize += vehicle.Size;
            vehicle = null;
            return check;
        }

        /// <summary>
        /// Method to return a object using platenumber
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        public Vehicle ReturnObjectVehicle(string plateNumber)
        {
            return ParkedVehicles.FirstOrDefault(o => o.PlateNumber == plateNumber);  // kollar om det finns något objekt med detta regnr. Retunerar hela objektet eller null
        }

        /// <summary>
        /// Method to find on what spot vehicle is parked on
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        public int FindSpot(string plateNumber)
        {
            var vehicle = ReturnObjectVehicle(plateNumber);
            if (vehicle != null)
            {
                ParkingHouse.PHouse.FirstOrDefault(i => i.Spot == vehicle.Spot);        // hittar objektplatsen men problem att printa fram den
                return vehicle.Spot;
            }
            return -1;       // 0 är standard värde på spot. Alltså att den inte finns.
        }
    }
}

