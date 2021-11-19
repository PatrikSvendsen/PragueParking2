using System;
using PragueParking2._0.DataConfig;

class Car : Vehicle
{
    /// <summary>
    /// Constructor for Car, used when reading and parking from JSON file.
    /// </summary>
    /// <param name="plateNumber"></param>
    public Car(string plateNumber, int price, int spot, DateTime timeParked) : base(plateNumber)
    {
        PlateNumber = plateNumber;
        Price = price;
        this.Size = ConfigValues.CarSize;
        this.TimeParked = timeParked;
        Spot = spot;
    }

    /// <summary>
    /// constructor for Car, used when adding new car to list.
    /// </summary>
    /// <param name="plateNumber"></param>
    public Car(string plateNumber) : base(plateNumber)
    {
        this.Price = ConfigValues.CarPricePerHour;
        this.Size = ConfigValues.CarSize;
    }
}


