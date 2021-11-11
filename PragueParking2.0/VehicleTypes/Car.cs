using System;
using PragueParking2._0;
using PragueParking2._0.DataConfig;

class Car : Vehicle
{
    /// <summary>
    /// Tar emot regnr och sätter sedan reg, storlek, pris och tid för ankomst.
    /// </summary>
    /// <param name="plateNumber"></param>
   
    public Car(string plateNumber, int price, int spot, DateTime timeParked) : base(plateNumber)
    {
        PlateNumber = plateNumber;
        Price = price;
        this.Size = ConfigValues.CarSize;
        this.timeParked = timeParked;
        Spot = spot;
    }

    public Car(string plateNumber) : base(plateNumber)
    {
        this.Price = ConfigValues.CarPricePerHour;
        this.Size = ConfigValues.CarSize;
    }

    public override void PrintVehicleParked()
    {
        Console.WriteLine("Car has now been parked at <input spot here>");
    }
}


