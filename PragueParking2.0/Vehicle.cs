using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PragueParking2._0;

namespace PragueParking2._0
{

    //************************************
    // Enumerator
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
        //************************************
        // Fields
        //************************************
        public string plateNumber;
        public string type;
        private readonly VehiclePricePerHour price;
        private readonly VehicleValue value;

        //************************************
        // Constructors
        //************************************
        public Vehicle()
        {
            
        }
        public Vehicle(string plateNumber)
        {
            this.plateNumber = plateNumber;
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
            this.type = type;
            this.plateNumber = plateNumber;
            this.price = price;
        }

        //************************************
        // Properties
        //************************************

        public string PlateNumber
        {
            get { return this.plateNumber; }
            set { this.plateNumber = value; }
        }

        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
        public VehiclePricePerHour Price
        {
            get { return this.price; }
        }
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

        //************************************
        // Methods
        //************************************
        // Försök till att hämta ut rätt värden för olika typer.
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

    //class Car : Vehicle
    //{
    //    char TypeCar;                   // Unikt för varje ärvd class, att gå en char som skall stå i början på varje regnr. C = Car C#ABC123

    //    public Car(VehicleType t, string p, char TypeCar) : base(t, p)
    //    {
    //        TypeCar = 'C';
    //    }
    //}
    class MC : Vehicle
    {
        string VehicleType;

        public MC(string VehicleType, string platenumber, VehiclePricePerHour price) : base(platenumber, price)
        {
            VehicleType = "MC";
        }
    }
}
