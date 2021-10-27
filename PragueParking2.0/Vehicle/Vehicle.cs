using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PragueParking2._0;


    public class Vehicle
    {
        //************************************
        // Fields
        //************************************

        public string plateNumber;
        public string type;
        public int price;
        public int size;

        //************************************
        // Constructors
        //************************************

        public Vehicle()
        {
           
        }
        public Vehicle(string type, string plateNumber)
        {
            this.plateNumber = plateNumber;
            this.type = type;
        }
        public Vehicle(string type, string plateNumber, int price)
        {
            this.plateNumber = plateNumber;
            this.type = type;
            this.price = price;
        }
        public Vehicle(string type, string platenumber, int price, int size)
        {
            Type = type;
            PlateNumber = platenumber;
            Price = price;
            Size = size;
        }

        //************************************
        // Properties
        //**********************************

        public string PlateNumber
        {
            get { return plateNumber; }
            set { plateNumber = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public int Price
        {
            get { return price; }
            set { price = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }
        public override string ToString()
        {
            //return "Type: " + type + ", " +
            //    "Plate number: " + plateNumber + ", " +
            //    "Parking value: " + (int)value + ", " +
            //    "Price per hour: " + (int)price;
            //return type + ", " + plateNumber + ", " + (int)price;
            return type + ", " + plateNumber + ", " + (int)price;
        }
        public static void VehicleParked(string type)
        {
            Console.WriteLine("{0} has now been parked at <input spot here>", type);
        }

        //************************************
        // Methods
        //************************************

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
        //public MC(string type, string platenumber, VehiclePricePerHour price) : base(type, platenumber, price)
        public MC(string type, string platenumber) : base(type, platenumber)
        {

        }
        public void CarParked()
        {
            Console.WriteLine("Motorcycle has now been parked at <input spot here>");
        }
    }

