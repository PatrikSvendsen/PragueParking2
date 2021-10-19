using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
 * Fixa med regex vid input av regnr
 * fixa med 100 parkeringsplatser och att en plats tildelas.
 * Använda Delegat för att skriva ut CW vid parkerat fordon, finns i genomgång 15. Kolla om Multicast går att använda vid registrering av fordon för att hitta rätt kostnad etc.
 * Använda foreach att loopa igenom en enum för att sedan ta .Contains för att hitta rätt värde till fordon.
 * 
 * Flytta currentParkedVehicles med endast regnr till när man ska söka efter fordon.
 */

namespace PragueParking2._0
{
    public class Program
    {
        public static string filePath = "../../../parkinglist.txt";                 // bör flyttas ut i parking house class
        public static string parkedVehicle = "../../../parkedVehicle.txt";          // Här ska man printa ut alla fordon med type etc
        public static string pricePath = "../../../VehiclePricePerHour.txt";
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

            InitializeParkingList<Vehicle>();   // Läser in sparad fil med parkerade bilar
            InitiateSearchRegList<string>();    // Efter man läst in alla sparade fordon från lista och konventerat dem till obejkt 
            InitiatePriceList(out Vehicle CurrentPrice);                                   // Så töms currentParkedVehicle listan för att endast innehålla regnr, kommer användas för searchReg.

            //var arrangedList = ArrangeListWithFormat<string>(parkingList);             // Lägger till lite information för en seperatlista.

            var menuChoice = "";
            var subMenuChoice = "";
            var typeValue = new Vehicle();
            bool mainmenu = true;

            while (mainmenu == true)
            {
                Console.Clear();
                var table1 = new Table()
                .AddColumn(new TableColumn("Welcome to this program for parking vehicles").Centered())
                .Width(100);
                var table2 = new Table()
                .AddColumn("Please chose an option");
                AnsiConsole.Write(table1);
                AnsiConsole.Write(table2);

                menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(new[] {"Register new vehicle", "Collect vehicle - Fungerar ej", "Move vehicle",
                    "Show parkingview", "Configure program", "Exit Program"}));
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
                            case "Car": break;
                            case "MC": break;

                            default:
                                if (subMenuChoice == "Go back")
                                {
                                    subMenuChoice = null;
                                }; break;
                        }
                        break;
                    case "Collect vehicle": break;
                    case "Move vehicle": break;
                    case "Show parkingview":
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        var table4 = new Table()
                        .AddColumn(new TableColumn("What do you want to do="));
                        AnsiConsole.Write(table4);
                        subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                            .AddChoices(new[] { "Find vehicle", "Small parkingview", "Large parkingview" }));
                        switch (subMenuChoice)
                        {
                            case "Find vehicle": break;

                            default:
                                break;
                        }



                        break;
                    case "Configure program": break;
                    case "Exit Program":
                        mainmenu = false;
                        Console.WriteLine("Closing program...");
                        Console.ReadLine();
                        break;
                    default:
                        Console.WriteLine("Please choose a correct option.");
                        break;
                }
                //**********************************************************
                // Registrera fordon
                //**********************************************************
                if (menuChoice == "Register new vehicle" && subMenuChoice != null)
                {
                    Console.WriteLine("Please enter your registration number: ");
                    string vehiclePlate = Console.ReadLine().ToUpper();
                    
                    while (!String.IsNullOrEmpty(vehiclePlate))         // kollar om sträng är null och inte empty. Mest för while loop ska fortsätta om man 
                                                                        //skrivit fel för att enklare avbryta den.
                    {
                        //bool regCheck = ValitedateVehiclePlateInput(vehiclePlate);       // Kollar input från regnr via regex.

                        if (ValitedateVehiclePlateInput(vehiclePlate))
                        {
                            AddVehicleToList(vehiclePlate, subMenuChoice);              // Går in i metoden och lägger till fordonet 
                            Vehicle.VehicleParked(subMenuChoice);                       // Printar ut att fordonet har blivit parkerat, måste ha parkeringsplats med.
                            Console.ReadKey();
                            break;
                        }
                        else
                        {
                            vehiclePlate = null;
                            Console.WriteLine("Something went wrong. Please enter a new platenumber or enter to go back to main menu.");
                            vehiclePlate = Console.ReadLine();
                        }
                    }
                }
                //**********************************************************
                // Visa parkeringslista
                //**********************************************************
                if (menuChoice == "Show parkingview")
                {
                    if (subMenuChoice == "Find vehicle")
                    {
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        AnsiConsole.Write(table2);
                        Console.WriteLine("Please enter your registration number: ");
                        string plateNumber = Console.ReadLine();

                        // Nedan fungerar för att hitta fordon från en lista. Kolla över lambda för att hitta fordon i en lista.
                        var titles = vehicles
                            .Where(Vehicle => Vehicle.PlateNumber.Contains(plateNumber))
                            .Select(Vehicle => Vehicle.PlateNumber);
                        foreach (var title in titles)
                        {
                            Console.WriteLine(title);
                        }

                        // lambda men fungerar inte korrekt
                        if (vehicles.Exists(v => v.PlateNumber == plateNumber))
                        {
                            Console.WriteLine("Yes, does car does exist.");
                        }
                        else
                        {
                            Console.WriteLine("We cannot find a car with that platenumber.");
                        }

                        
                        Console.ReadKey();
                    }
                    else if (menuChoice == "Small parkingview")
                    {
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        AnsiConsole.Write(table2);
                        Showparkingview();
                        ReturnToMenu();
                        continue;
                    }

                }
                if (menuChoice == "Configure program")
                {
                    Vehicle.NewPrice = ChangePriceList();
                }
            }
            //************************************
            // Metoder
            //************************************
        }

        static List<Vehicle> InitializeParkingList<T>()          // out List<string> inputList
        {
            currentParkedVehicles = File.ReadAllLines(filePath).ToList();
            foreach (string vehicle in currentParkedVehicles)
            {
                string[] items = vehicle.Split(',');
                Vehicle v = new Vehicle(items[0], items[1], (VehiclePricePerHour)int.Parse(items[2]));
                //Vehicle v = new Vehicle();
                vehicles.Add(v);
            }
            return vehicles;
        }
        static public List<string> InitiateSearchRegList<T>()
        {
            currentParkedVehicles.Clear();
            var myList = vehicles.ConvertAll(x => x.ToString());
            foreach (string vehicle in myList)
            {
                string[] items = vehicle.Split(',');
                var v = items[1];
                currentParkedVehicles.Add(v);
            }
            return currentParkedVehicles;
        }
        public static int InitiatePriceList(out Vehicle CurrentPrice)
        {
            CurrentPrice = null;
            var priceList = File.ReadAllLines(pricePath);
            foreach (var price in priceList)
            {
                string[] prices = price.Split(",");
            }
            foreach (int price in Enum.GetValues(typeof(VehiclePricePerHour)))
            {
                 
            }
            
            //Vehicle.CurrentPrice = int.Parse(prices[0]);

            //Console.ReadKey();
            return Vehicle.CurrentPrice;

            //CurrentPrice = null;
            //string[] prices = File.ReadAllLines(pricePath);
            //Vehicle.CurrentPrice = int.Parse(prices[0]);
            //return Vehicle.CurrentPrice;
        }

        private static int ChangePriceList()
        {
            Console.Clear();
            Console.WriteLine("Please give car a new value: ");
            Vehicle.NewPrice = int.Parse(Console.ReadLine());

            if (Vehicle.NewPrice != Vehicle.CurrentPrice)
            {
                Vehicle.CurrentPrice = Vehicle.NewPrice;
                return Vehicle.CurrentPrice;
            }
            else
            {
                return Vehicle.CurrentPrice;
            }
        }

        public static List<Vehicle> ArrangeListWithFormat<T>()
        {
            List<string> outParkedVehicle = new List<string>();
            foreach (Vehicle v in vehicles)
            {
                outParkedVehicle.Add(v.ToString());
            }
            File.WriteAllLines(parkedVehicle, outParkedVehicle);

            return vehicles;
        }

        // För att spara senaste fordon till fil
        private static bool SaveVehicleToFile(Vehicle newVehicle, string vehiclePlate)
        {
            string[] lines = { newVehicle.ToString() };
            File.AppendAllLines(Path.Combine(filePath), lines);
            //lines[0] = vehiclePlate;
            //File.AppendAllLines(Path.Combine(searchPlatenumber), lines);              // path finns ej kvar. 
            return true;
        }

        // För att lägga till bilar i parkeringslistan
        public static bool AddVehicleToList(string vehiclePlate, string type)
        {
            if (true)
            {
                //ChangePriceList();
                Vehicle.GetCorrectInfo(type, out Vehicle typePrice);                 // hämtar korrekt info om priset per timme för vilken typ av fordon

                Vehicle newVehicle = new Vehicle(type, vehiclePlate, (VehiclePricePerHour)(int)Vehicle.CurrentPrice);

                vehicles.Add(newVehicle);                                           // lägger till fordonet i Vehicle-lista
                SaveVehicleToFile(newVehicle, vehiclePlate);                         // lägger till det nya fordonet i textfilen
                return true;
            }
        }
        // Validerar input från användaren samt kollar längden.
        private static bool ValitedateVehiclePlateInput(string vehiclePlate)
        {
            bool regCheck = false;
            bool correctinput = false;

            while (!correctinput)
            {
                if (vehiclePlate != null)
                {
                    // Kollar så det inte finns massa onödiga specialtecken samt att det 10 eller mindre.
                    string specialChar = @"^[^\s!.,;:#¤%&\/\\()=?`´@£$$€{}[\]]{0,10}$";
                    Regex reg = new Regex(specialChar);
                    regCheck = reg.IsMatch(vehiclePlate);
                    return regCheck;
                }
                else
                {
                    break;
                }
            }
            return false;
        }


        // Visa parkerade bilar
        public static void Showparkingview()
        {
            //var carPrice = new Vehicle(VehiclePricePerHour.Car);
            //var mcPrice = new Vehicle(VehiclePricePerHour.MC);

            //var vehicles = File.ReadAllLines(filePath).ToList();
            foreach (var vehicle in vehicles)
            {
                Console.WriteLine(vehicle.ToString());
            }
        }

        // Ska fungera som en bakåt-metod
        public static void ReturnToMenu()
        {
            Console.WriteLine("\nPress enter to return to main menu.");
            Console.ReadLine();
        }
    }
}

