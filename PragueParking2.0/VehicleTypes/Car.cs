using System;
using PragueParking2._0;

class Car : Vehicle
{
    public Car(string plateNumber) : base(plateNumber)
    {
        string type = "Car";
        this.Price = VehiclePriceList.GetPrice(type);
        this.Size = 4;
    }
    //public Car(string plateNumber, int price, DateTime TimeParked)
    //{
    //    PlateNumber = plateNumber;
    //    Price = price;
    //    this.Size = 4;
    //    DateTime timeparked = TimeParked;
    //}

    public override void PrintVehicleParked()
    {
        Console.WriteLine("Car has now been parked at <input spot here>");
    }
}


