using System;

class Vehicle
{
    /// <summary>
    /// Constructor for vehicle. 
    /// </summary>
    public Vehicle()
    {

    }
    /// <summary>
    /// Constructor for vehicle
    /// </summary>
    /// <param name="plateNumber"></param>
    public Vehicle(string plateNumber)
    {
        this.PlateNumber = plateNumber;
        TimeParked = DateTime.Now;
        this.Spot = 0;
    }
    public string PlateNumber { get; set; }
    public int Size { get; set; }
    public int Price { get; set; }
    public int Spot { get; set; }
    public DateTime TimeParked { get; set; }    
}

