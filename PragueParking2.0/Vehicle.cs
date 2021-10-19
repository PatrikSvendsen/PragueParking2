using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PragueParking2._0;

namespace PragueParking2._0
{

    //************************************
    // Enumeratorm
    //************************************
    public enum VehicleValue
    {
        //Bike = 1,
        MC = 2,
        Car = 4,
        //Bus = 16
    }

    public enum VehiclePricePerHour
    {
        // Price in CZK /hour
        //Bike = 5,
        MC = 10,
        Car = 20
        //Bus = 80
    }
    public class Vehicle
    {
        public static string pricePath = "../../../VehiclePricePerHour.txt";
        //************************************
        // Fields
        //************************************
        public string plateNumber;
        public string type;
        public VehiclePricePerHour price;
        public VehicleValue value;
        //public int Price;

        //************************************
        // Constructors
        //************************************
        public Vehicle()
        {
            //OldPrice = 20;
        }
        public Vehicle(int newPrice, int currentPrice)
        {
            NewPrice = newPrice;
            CurrentPrice = currentPrice;
        }
        public Vehicle(VehicleValue value)
        {
            this.value = value;
        }
        public Vehicle(VehiclePricePerHour price)
        {
            this.price = price;
        }
        public Vehicle(string type, string plateNumber)
        {
            this.plateNumber = plateNumber;
            this.type = type;
        }
        public Vehicle(string plateNumber, VehiclePricePerHour price)
        {
            this.plateNumber = plateNumber;
            this.price = price;
        }
        public Vehicle(string type, string plateNumber, VehiclePricePerHour price)
        {
            Type = type;
            PlateNumber = plateNumber;
            Price = price;
        }
        //************************************
        // Properties
        //************************************

        public string PlateNumber
        {
            get { return plateNumber; }
            set { plateNumber = value; }
        }
        //public string PlateNumber
        //{
        //    get { return this.plateNumber; }
        //    set { this.plateNumber = value; }
        //}
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public static VehiclePricePerHour Price { get; set; }

        //public VehiclePricePerHour Price
        //{
        //    get { return this.price; }
        //    set {  this.price = value; }
        //}
        public static int NewPrice { get; internal set; }
        public static int CurrentPrice { get; internal set; }
        
        public VehicleValue Value
        {
            get { return value; }
        }

        public override string ToString()
        {
            //return "Type: " + type + ", " +
            //    "Plate number: " + plateNumber + ", " +
            //    "Parking value: " + (int)value + ", " +
            //    "Price per hour: " + (int)price;
            return type + ", " + plateNumber + ", " + (int)price;
        }
        public static void VehicleParked(string type)
        {
            Console.WriteLine("{0} has now been parked at <input spot here>", type);
        }

        //************************************
        // Methods
        //************************************
        // Försök till att hämta ut rätt värden för olika typer.
        // Se om det går att använda <T> och gör den generisk
        public static void GetCorrectInfo(string type, out Vehicle typePrice)
        {
            typePrice = new Vehicle();
            if (type == "Car")
            {
                typePrice = new Vehicle(VehiclePricePerHour.Car);
            }
            else if (type == "MC")
            {
                typePrice = new Vehicle(VehiclePricePerHour.MC);
            }
        }
    }

    class Car : Vehicle
    {
        //public Car(string type, string platenumber, VehiclePricePerHour price, int value) : base(type, platenumber, price)
        public Car()
        {

        }
        public static void VehicleParked()
        {
            Console.WriteLine("Car has now been parked at <input spot here>");
        }
    }
    class MC : Vehicle
    {
        public MC(string type, string platenumber, VehiclePricePerHour price) : base(type, platenumber, price)
        {

        }
        public void CarParked()
        {
            Console.WriteLine("Motorcycle has now been parked at <input spot here>");
        }
    }
}
