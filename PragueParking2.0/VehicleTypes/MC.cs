using System;
using PragueParking2._0;

class MC : Vehicle
{
    public MC(string plateNumber) : base(plateNumber)
    {
        string type = "MC";
        this.Price = VehiclePriceList.GetPrice(type);
        this.Size = 2;

    }
    public void CarParked()
    {
        Console.WriteLine("Motorcycle has now been parked at <input spot here>");
    }
}

