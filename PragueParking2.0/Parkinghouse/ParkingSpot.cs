using System.Collections.Generic;
using System.Linq;
using PragueParking2._0.DataConfig;

namespace PragueParking2._0
{
    public class ParkingSpot
    {
        public List<Vehicle> ParkedVehicles = new(); 
        internal int Spot { get; set; }
        private int Size { get; set; }
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

        public override string ToString()
        {
            return Spot.ToString();
        }

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

            //return vehicles.Contains(vehicle);   
            return false;
        }

        internal bool ParkVehicleFromList(Vehicle vehicle, int spot)
        {
            foreach (var item in ParkingHouse.PHouse)
            {
                if (item.Spot == spot +1)
                {
                    ParkedVehicles.Insert(spot, vehicle);
                    AvailableSize -= vehicle.Size;
                    vehicle.Spot = Spot;
                    return true;
                }
            }
            return false;
        }

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
        public bool RemoveVehicle(Vehicle vehicle)
        {
            bool check = false;
            check = ParkedVehicles.Remove(vehicle);
            AvailableSize += vehicle.Size;
            return check;
        }

        public Vehicle ReturnObjectVehicle(string plateNumber)
        {
            return ParkedVehicles.FirstOrDefault(o => o.PlateNumber == plateNumber);  // kollar om det finns något objekt med detta regnr. Retunerar hela objektet eller null
        }

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

        public bool MoveVehicle(string plateNumber)
        {
            bool check = false;
            int spot;

            spot = FindSpot(plateNumber);

            foreach (var item in ParkingHouse.PHouse)
            {
                if (item.Spot == 1)
                {
                    var vehicle = ReturnObjectVehicle(plateNumber);
                    if (vehicle == null)
                    {
                        break;
                    }
                    vehicle.Spot = 2;
                }
            }
            return check;
        }

    }
}

