using System;
using PragueParking2._0.DataConfig;

class Mc : Vehicle
{
    /// <summary>
    /// Constructor for MC, used when reading and parking from JSON file.
    /// </summary>
    /// <param name="plateNumber"></param>

    public Mc(string plateNumber, int price, int spot, DateTime timeParked) : base(plateNumber)
    {
        PlateNumber = plateNumber;
        Price = price;
        this.Size = ConfigValues.MCSize;
        this.TimeParked = timeParked;
        Spot = spot;
    }

    /// <summary>
    /// constructor for Mc, used when adding new car to list.
    /// </summary>
    /// <param name="plateNumber"></param>
    public Mc(string plateNumber) : base(plateNumber)
    {
        this.Price = ConfigValues.MCPricePerHour;
        this.Size = ConfigValues.MCSize;
    }
}

