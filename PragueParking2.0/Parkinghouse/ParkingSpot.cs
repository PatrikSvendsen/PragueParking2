using System.Collections.Generic;
using System.Linq;

namespace PragueParking2._0
{

    public class ParkingSpot
    {

        public ParkingSpot(int spotSize, int spot)
        {
            Spot = spot;
            Size = spotSize;
            AvailableSize = spotSize;
            //Size = spotSize;
        }

        int Spot { get; set; }
        private int Size { get; set; }

        private int AvailableSize { get; set; }  // = parkingSpotSize = 4;

        public List<Vehicle> ParkedVehicles = new();

        public override string ToString()
        {
            return Spot.ToString(); // + ParkedVehicles.ToString();
        }

        public bool ParkVehicleToSpot(Vehicle vehicle, int spot)
        {
            for (int i = 0; i < ParkingHouse.PHouse.Count; i++)
            {
                if (i == spot)
                {
                    ParkedVehicles.Insert(spot, vehicle);
                    //AvailableSize -= vehicle.Size;
                    AvailableSize -= vehicle.Size;
                    vehicle.Spot = Spot;
                    Vehicle.SaveVehicleToFile(vehicle, spot);
                    return true;
                }
            }

            //return vehicles.Contains(vehicle);   
            return true;
        }
        public bool CheckSpace(Vehicle vehicle)
        {
            for (int i = 0; i < ParkingHouse.PHouse.Count; i++)
            {

                if (AvailableSize == 4 & vehicle.Size == 4)
                {
                    //spot = i;
                    return true;
                }
                else if (AvailableSize == 2 || AvailableSize == 4 & vehicle.Size == 2)
                {
                    return true;
                }

            }
            //spot = -1;
            return false;
        }
        //public int FindVehicle(string plateNumber)
        //{
        //    int index = -1;
        //    return index = ParkedVehicles.FindIndex(index => index.PlateNumber == plateNumber);
        //}

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

            //var vehicle = ReturnObjectVehicle(plateNumber);
            //if (vehicle != null)
            //{
            //    return true;
            //}

            return check;
        }
        public bool RemoveVehicle(string plateNumber)
        {
            bool check = false;
            for (int i = 0; i < ParkingHouse.PHouse.Count; i++)
            {
                if (FindVehicle(plateNumber))
                {
                    //var vehicle = ParkedVehicles.FirstOrDefault(o => o.PlateNumber == plateNumber);
                    var vehicle = ReturnObjectVehicle(plateNumber);
                    check = ParkedVehicles.Remove(vehicle);
                    AvailableSize += vehicle.Size;
                    vehicle.Spot = 0;
                    break;
                }
            }
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
            return 0;       // 0 är standard värde på spot. Alltså att den inte finns.
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

