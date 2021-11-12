using System;

using PragueParking2._0.DataConfig;

class MC : Vehicle
{
    /// <summary>
    /// Tar emot regnr och sätter sedan reg, storlek, pris och tid för ankomst.
    /// </summary>
    /// <param name="plateNumber"></param>

    public MC(string plateNumber, int price, int spot, DateTime timeParked) : base(plateNumber)
    {
        PlateNumber = plateNumber;
        Price = price;
        this.Size = ConfigValues.MCSize;
        this.timeParked = timeParked;
        Spot = spot;
    }

    public MC(string plateNumber) : base(plateNumber)
    {
        this.Price = ConfigValues.MCPricePerHour;
        this.Size = ConfigValues.MCSize;
    }
    public void CarParked()
    {
        Console.WriteLine("Motorcycle has now been parked at <input spot here>");
    }
}

