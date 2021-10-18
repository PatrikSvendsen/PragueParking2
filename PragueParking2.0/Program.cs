using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PragueParking2._0;
using Spectre.Console;

/**Uppdaterat program för Prague Parking. 
 * Data skall sparas så att det inte försvinner om programmet stängs av.
• Det skall gå att hantera P-platser av olika storlek så att även stora fordon (bussar) och små 
fordon (cyklar) kan hanteras.
• En karta över P-huset skall visas, så att man enkelt kan se beläggningen. Det skall gå att stå 
en bit ifrån skärmen och se översiktligt hur många platser som är lediga
• Det skall finnas en prislista i form av en textfil. Filen läses in vid programstart och kan vid 
behov läsas in på nytt via ett menyval. Filen skall gå att ändra även för en med låg ITkunskap, så formatet behöver vara lätt att förstå. (TIPS: om allt efter ”#” på en rad är 
kommentarer, kan man ha dokumentation inne i själva filen)
• Det skall finnas en textfil med konfigurationsdata för systemet. Filformatet är fritt, men XML 
eller JSON kan vara lämpliga att använda.
• I konfigurationsfilen skall man kunna konfigurera
o Antal P-platser i garaget
o Fordonstyper (Bil och MC, men det kan komma fler)
o Antal fordon per P-plats för respektive fordonstyp
• Prisstrukturen är till en början
o Bil: 20 CZK per påbörjad timme
o MC: 10 CZK per påbörjad timme
o De första tio minuterna är gratis
• Filerna för prislista och konfiguration kan gärna kombineras till en fil
• Trots att det är en konsolapplikation skall den göras så snygg som möjligt. Användaren skall 
uppleva att bilden är stabil, tydlig och lätt att förstå
 * 
 * 
 * 
 * TODO: 
 * 
 * 
 */

namespace PragueParking2._0
{

}
class Program
{
    public static string filePath = "../../../parkinglist.txt";                 // bör flyttas ut i parking house class
    public static string parkedVehicle = "../../../parkedVehicle.txt";          // Här ska man printa ut alla fordon med type etc
    public static string searchPlatenumber = "../../../searchPlatenumber.txt";  // Här bör endast regnr skickas in för kontroll vid dubletter
    public static List<Vehicle> vehicles = new List<Vehicle>(100);
    public static List<string> currentParkedVehicles = new List<string>(100);



    public static void Main(string[] args)
    {
        //************************************
        // Main 
        //************************************
        DateTime current = DateTime.Now;                            // Aktuell tid
                                                                    //List<Vehicle> test = new List<Vehicle>(100);              // skapar ny lista
                                                                    // skapar ny lista

        InitializeParkingList<Vehicle>(); // läser in sparad fil med parkerade bilar
        //var arrangedList = ArrangeListWithFormat<string>(parkingList);             // Lägger till lite information för en seperatlista.

        var menuChoice = "";
        var subMenuChoice = "";
        var typeValue = new Vehicle();

        while (true)
        {
            //var arrangedList = ArrangeListWithFormat<string>(parkingList);
            Console.Clear();
            var table1 = new Table()
            .AddColumn(new TableColumn("Welcome to this program for parking vehicles").Centered())
            .Width(100)
            .Border(TableBorder.HeavyEdge);
            var table2 = new Table()
            .AddColumn("Please chose an option")
            .Border(TableBorder.HeavyEdge);
            AnsiConsole.Write(table1);
            AnsiConsole.Write(table2);

            menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(new[] {"Register new vehicle", "Collect vehicle", "Move vehicle", "Show parkingview", "Configure prices",
                                    "Configure options", "Exit Program"}));
            switch (menuChoice)
            {
                case "Register new vehicle":
                    Console.Clear();
                    AnsiConsole.Write(table1);
                    var table3 = new Table()
                    .AddColumn(new TableColumn("What kind of vehicle?").Centered())
                    .Border(TableBorder.HeavyEdge);
                    AnsiConsole.Write(table3);
                    subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(new[] { "Car", "MC", "Go back" }));

                    Console.Clear();
                    AnsiConsole.Write(table1);
                    AnsiConsole.Write(table2);
                    switch (subMenuChoice)
                    {
                        case "Car":break;
                        case "MC": break;

                        default: break;
                    }
                    break;
                case "Collect vehicle": break;
                case "Move vehicle": break;
                case "Show parkingview": break;
                case "Configure prices": break;
                case "Configure options": break;
                case "Exit Program": Console.WriteLine("Closing program..."); break;
                default:
                    Console.WriteLine("Please choose a correct option.");
                    break;
            }

            if (menuChoice == "Register new vehicle" && subMenuChoice != null)
            {
                Console.WriteLine("Please enter your registration number: ");
                string vehiclePlate = Console.ReadLine();
                AddVehicleToList(vehiclePlate, subMenuChoice);
                Console.ReadKey();
            }
            if (menuChoice == "Show parkingview")
            {
                Console.Clear();
                AnsiConsole.Write(table1);
                AnsiConsole.Write(table2);
                Showparkingview();
                ReturnToMenu();
                continue;
            }
        }
        //************************************
        // Metoder
        //************************************
    }

    // För att spara ned listan tilll textfil


    // För att initiera text-filer
    static List<Vehicle> InitializeParkingList<T>()          // out List<string> inputList
    {
        currentParkedVehicles = File.ReadAllLines(filePath).ToList();
        foreach (string vehicle in currentParkedVehicles)
        {
            string[] items = vehicle.Split(',');
            //Vehicle v = new Vehicle(items[0], items[1], carValue.Value, carPrice.Price);
            Vehicle v = new Vehicle();
            vehicles.Add(v);
        }
        return vehicles;
    }

    public static List<Vehicle> ArrangeListWithFormat<T>()
    {
        //foreach (string output in outputList)
        //{
        //    string[] items = output.Split(',');
        //    Vehicle v = new Vehicle(items[0], items[1]);
        //    vehicles.Add(v);
        //}
        List<string> outParkedVehicle = new List<string>();
        foreach (Vehicle v in vehicles)
        {
            outParkedVehicle.Add(v.ToString());
        }
        File.WriteAllLines(parkedVehicle, outParkedVehicle);

        return vehicles;
    }

    // För att lägga till bilar i parkeringslistan
    public static bool AddVehicleToList(string vehiclePlate, string type)
    {
        //var typeValue = new Vehicle();
        //var typePrice = new Vehicle();
        // här ska regex check ligga
        
        if (true)
        {
            GetCorrectInfo(type, out Vehicle typePrice);
            Vehicle newVehicle = new Vehicle(type, vehiclePlate, typePrice.Price);
            vehicles.Add(newVehicle);
            SaveVehicleToFile(newVehicle);
            return true;
        }
        else if (false)
        {
            Console.WriteLine("Something went wrong, probably licenseplate input is not correct.");
        }
    }

    // För att spara senaste fordon till fil
    private static bool SaveVehicleToFile(Vehicle newVehicle)
    {
        string[] lines = { newVehicle.ToString() };
        File.AppendAllLines(Path.Combine(filePath), lines);
        return true;
    }

    // Visa parkerade bilar
    public static void Showparkingview()
    {
        //var carType = new Vehicle(VehicleValue.Car);
        //var mcType = new Vehicle(VehicleValue.MC);
        //var carPrice = new Vehicle(VehiclePricePerHour.Car);
        //var mcPrice = new Vehicle(VehiclePricePerHour.MC);

        //var vehicles = File.ReadAllLines(filePath).ToList();
        foreach (var vehicle in vehicles)
        {
            Console.WriteLine(vehicle.ToString());
        }
    }
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
    // Ska fungera som en bakåt-metod
    public static void ReturnToMenu()
    {
        Console.WriteLine("\nPress enter to return to main menu.");
        Console.ReadLine();
    }
}

