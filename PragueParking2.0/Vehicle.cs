using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PragueParking2._0;


public class Vehicle
{
    public static string filePath = "../../../Parkinghouse/parkinglist.txt";                 // bör flyttas ut i parking house class

    //************************************
    // Fields
    //************************************

    public DateTime dateTime;

    //************************************
    // Constructors
    //************************************

    public Vehicle()
    {

    }
    public Vehicle(string plateNumber)
    {
        this.PlateNumber = plateNumber;
        TimeParked = DateTime.Now;
        Spot = 0;
        
    }
    //************************************
    // Properties
    //**********************************


    public string PlateNumber { get; set; }
    public int Size { get; set; }
    public int Price { get; set; }
    public int Spot { get; set; }
    public DateTime TimeParked { get; set; }

    public override string ToString()
    {
        return GetType() + ", " + PlateNumber + ", " + Price + ", " + TimeParked.ToString("dd/MM/yyyy-HH:mm");
    }

    //************************************
    // Methods
    //************************************

    public virtual void PrintVehicleParked()
    {
        Console.WriteLine("Vehicle has now been parked.");
    }
    public static bool SaveVehicleToFile(Vehicle vehicle, int spot)
    {
        var type = vehicle.GetType();
        string[] lines = { (spot+1, type, vehicle.PlateNumber, vehicle.Price, vehicle.TimeParked.ToString("dd/MM/yyyy-HH:mm")).ToString() }; //TODO: Lösa så att parkeringsruta komme med
        File.AppendAllLines(Path.Combine(filePath), lines);
        return true;
    }

    //************************************
    // För att hitta speficfikt fordon i listan.
    //************************************


}

