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
        public static string pricePath = "../../../Vehiclevalues.txt";
        public delegate void SimpleDelegate(string plateNumber);

        public static void Main(string[] args)
        {
            //************************************
            // Main 
            //************************************
            DateTime current = DateTime.Now;                            // Aktuell tid
                                                                        //List<Vehicle> test = new List<Vehicle>(100);              // skapar ny lista
                                                                        // skapar ny lista

            InitializeParkingList<Vehicle>();   // Läser in sparad fil med parkerade bilar
            //InitiatePriceList(out Vehicle CurrentPrice);                                   // Så töms currentParkedVehicle listan för att endast innehålla regnr, kommer användas för searchReg.
            Parkinghouse.InitiateParkingValues();
            foreach (Parkinghouse item in Parkinghouse.parkingHouseValues)
            {
                Console.WriteLine(item);
            }
            foreach (Vehicle item in Parkinghouse.vehicles)
            {
                Console.WriteLine(item);
            }

            //var arrangedList = ArrangeListWithFormat<string>(parkingList);             // Lägger till lite information för en seperatlista.

            var menuChoice = "";
            var subMenuChoice = "";
            var typeValue = new Vehicle();
            bool mainmenu = true;

            while (mainmenu == true)
            {
                menuChoice = null;
                subMenuChoice = null;
                Console.Clear();
                var table1 = new Table()
                .AddColumn(new TableColumn("Welcome to this program for parking vehicles").Centered())
                .Width(100);
                var table2 = new Table()
                .AddColumn("Please chose an option");
                AnsiConsole.Write(table1);
                AnsiConsole.Write(table2);
                var table4 = new Table()
                    .AddColumn(new TableColumn("What do you want to do?"));
                menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(new[] {"Register new vehicle", "Find vehicle",
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
                            case "Car" : break;
                            case "MC" : break;

                            default:
                                if (subMenuChoice == "Go back")
                                {
                                    subMenuChoice = null;
                                }; break;
                        }
                        break;
                    case "Find vehicle" : 
                        /*
                         * 
                         * Gör om programmet så att det frågar efter regnr innan den går vidare till valmöjligheterna. Om regnr finns -> Gå vidare om INTE så ge fel.
                         */
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        AnsiConsole.Write(table4);
                        subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                            .AddChoices(new[] { "Find vehicle", "Collect vehicle", "Move vehicle", "Go back" }));
                        switch (subMenuChoice)
                        {
                            case "Find vehicle" : break;
                            case "Collect vehicle" : break;
                            case "Move vehicle" : break;
                            default:
                                if (subMenuChoice == "Go back")
                                {
                                    subMenuChoice = null;
                                }; break;
                        }

                        break;
                    case "Show parkingview":
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        AnsiConsole.Write(table4);
                        subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                            .AddChoices(new[] { "Small parkingview", "Large parkingview", "Go back" }));
                        switch (subMenuChoice)
                        {
                            case "Small parkingview" : break;
                            case "Large parkingview" : break;

                            default:
                                if (subMenuChoice == "Go back")
                                {
                                    subMenuChoice = null;
                                }; break;
                        }
                        break;
                    case "Configure program": 
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        AnsiConsole.Write(table4);
                        subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                            .AddChoices(new[] { "Add new values", "Go back" }));
                        switch (subMenuChoice)
                        {
                            case "Add new values":
                                Console.WriteLine("Adding new values...");
                                Console.WriteLine("Please wait...");
                                Parkinghouse.parkingHouseValues.Clear();
                                Parkinghouse.InitiateParkingValues();
                                Console.WriteLine("All done. Press enter to return to main menu.");
                                Console.ReadKey();
                                foreach (Parkinghouse item in Parkinghouse.parkingHouseValues)
                                {
                                    Console.WriteLine(item);
                                }
                                break;

                            default:
                                break;
                        }
                        break;
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
                    Console.Clear();
                    AnsiConsole.Write(table1);
                    string plateNumber = " ";
                    Console.WriteLine("Please enter a plate number: ");
                    AskForVehiclePlate(out plateNumber);

                    while (!String.IsNullOrEmpty(plateNumber))         // kollar om sträng är null och inte empty. Mest för while loop ska fortsätta om man 
                                                                       //skrivit fel för att enklare avbryta den.
                    {
                        if (ValitedateVehiclePlateInput(plateNumber) && FindVehicle(plateNumber) <= 0)
                        {
                            Console.Clear();
                            AnsiConsole.Write(table1);
                            //Parkinghouse.AddSpotToList(subMenuChoice);
                            AddVehicleToList(plateNumber, subMenuChoice);              // Lägger till fordon
                            Vehicle.VehicleParked(subMenuChoice);                       // Printar ut att fordonet har blivit parkerat, måste ha parkeringsplats                                                             med.
                            Console.ReadKey();
                            break;
                        }
                        else
                        {
                            plateNumber = null;
                            Console.WriteLine("You entered a non-valid plate number. Try again or press enter to return to main menu.");
                            AskForVehiclePlate(out plateNumber);
                        }
                    }
                }
                if (menuChoice == "Find vehicle")
                {
                    if (subMenuChoice == "Find vehicle")
                    {
                        string plateNumber = " ";
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        Console.WriteLine("Please enter a plate number: ");
                        AskForVehiclePlate(out plateNumber);

                        while (!String.IsNullOrEmpty(plateNumber))
                        {
                            if (ValitedateVehiclePlateInput(plateNumber))
                            {
                                int index = FindVehicle(plateNumber);

                                if (index >= 0)
                                {
                                    Console.Clear();
                                    AnsiConsole.Write(table1);
                                    Console.WriteLine("We found a vehicle with that registration number here.");
                                    Console.WriteLine(Parkinghouse.vehicles[index].PlateNumber);
                                    Console.ReadKey();
                                    plateNumber = null;
                                }
                            }
                            else
                            {
                                plateNumber = null;
                                Console.WriteLine("You entered a non-valid plate number. Try again or press enter to return to main menu.");
                                AskForVehiclePlate(out plateNumber);
                            }
                        }
                    }
                    else if (subMenuChoice == "Collect vehicle")
                    {

                    }
                }
                //**********************************************************
                // Visa parkeringslista // hitta fordon
                //**********************************************************
                /*
                 * Gör om find vehicle så att alla funktioner finns i den, ex collect, move eller visa info.
                 */

                //String.IsNullOrEmpty

                if (menuChoice == "Show parkingview")
                {
                    

                    if (subMenuChoice == "Small parkingview")
                    {
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        AnsiConsole.Write(table2);
                        Showparkingview();
                        Console.ReadKey();
                        continue;
                    }
                }
                if (menuChoice == "Configure program")
                {

                }
            }
            //************************************
            // Metoder
            //************************************
        }

        static List<Vehicle> InitializeParkingList<T>()          // out List<string> inputList
        {
            List<string> currentParkedVehicles = new List<string>(100);
            currentParkedVehicles = File.ReadAllLines(filePath).ToList();
            foreach (string vehicle in currentParkedVehicles)
            {
                string[] items = vehicle.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Vehicle v = new Vehicle(items[0], items[1], int.Parse(items[2]));
                Parkinghouse.vehicles.Add(v);
            }
            return Parkinghouse.vehicles;
        }


        //************************************************************************************************************
        #region gömd kod, pris per timme etc
        //************************************
        // Ändra priset på parking per timme
        //************************************

        //private static int ChangePriceList()
        //{
        //    Console.Clear();
        //    Console.WriteLine("Please give car a new value: ");
        //    Vehicle.NewPrice = int.Parse(Console.ReadLine());

        //    if (Vehicle.NewPrice != Vehicle.CurrentPrice)
        //    {
        //        Vehicle.CurrentPrice = Vehicle.NewPrice;
        //        return Vehicle.CurrentPrice;
        //    }
        //    else
        //    {
        //        return Vehicle.CurrentPrice;
        //    }
        //}
        //************************************
        // Tanken är att lägga till beteckning för listan, enklare hantering.
        //************************************

        //public static List<Vehicle> ArrangeListWithFormat<T>()
        //{
        //    List<string> outParkedVehicle = new List<string>();
        //    foreach (Vehicle v in Parkinghouse<Vehicle>.vehicles)
        //    {
        //        outParkedVehicle.Add(v.ToString());
        //    }
        //    File.WriteAllLines(parkedVehicle, outParkedVehicle);

        //    return Parkinghouse<Vehicle>.vehicles;
        //}
        //************************************
        // För att spara senaste fordon till fil
        //************************************
        #endregion************************************************************************************************************
        //************************************************************************************************************


        public static bool SaveVehicleToFile(Vehicle newVehicle, string vehiclePlate)
        {
            string[] lines = { newVehicle.ToString() };
            File.AppendAllLines(Path.Combine(filePath), lines);
            return true;
        }
        //************************************
        // För att lägga till bilar i parkeringslistan
        //************************************

        public static bool AddVehicleToList(string vehiclePlate, string type)
        {
            int typePrice = 0;
            if (true)
            {
                Parkinghouse.GetPrice(type, out typePrice);
                Vehicle newVehicle = new Vehicle(type, vehiclePlate, typePrice);
                Parkinghouse.vehicles.Add(newVehicle);                                           // lägger till fordonet i Vehicle-lista
                SaveVehicleToFile(newVehicle, vehiclePlate);                         // lägger till det nya fordonet i textfilen
                return true;
            }
        }
        //************************************
        // För att hitta speficfikt fordon i listan.
        //************************************

        public static int FindVehicle(string plateNumber)
        {
            int index = -1;
            return index = Parkinghouse.vehicles.FindIndex(index => index.PlateNumber == plateNumber);
        }

        //************************************
        // Fråga efter regnr
        //************************************

        public static string AskForVehiclePlate(out string plateNumber)
        {
            plateNumber = " ";
            plateNumber = Console.ReadLine().ToUpper();
            return plateNumber;
        }

        //************************************
        // Validerar input från användaren samt kollar längden.
        //************************************

        public static bool ValitedateVehiclePlateInput(string vehiclePlate)
        {
            bool regCheck = false;
            //bool correctinput = false;

            string specialChar = @"^[^\s!.,;:#¤%&\/\\()=?`´@£$$€{}[\]]{0,10}$";
            Regex reg = new Regex(specialChar);
            regCheck = reg.IsMatch(vehiclePlate);

            return regCheck;
        }

        //************************************
        // Visa parkerade bilar
        //************************************
        public static void Showparkingview()
        {
            foreach (var vehicle in Parkinghouse.vehicles)
            {
                Console.WriteLine(vehicle.ToString());
            }
        }
        //************************************
        // Ska fungera som en bakåt-metod
        //************************************

        public static void ReturnToMenu()
        {
            Console.WriteLine("\nPress enter to return to main menu.");
            Console.ReadLine();
        }
        //************************************
        // 
        //************************************
        //static public List<string> InitiateSearchRegList<T>()
        //{
        //    currentParkedVehicles.Clear();
        //    var myList = vehicles.ConvertAll(x => x.ToString());
        //    foreach (string vehicle in myList)
        //    {
        //        string[] items = vehicle.Split(',');
        //        var v = items[1];
        //        currentParkedVehicles.Add(v);
        //    }
        //    return currentParkedVehicles;
        //}
    }
}

