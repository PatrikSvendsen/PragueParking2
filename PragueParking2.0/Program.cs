using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Spectre.Console;
using PragueParking2._0;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

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
 * Använda Delegat för att skriva ut CW vid parkerat fordon, finns i genomgång 15. Kolla om Multicast går att använda vid registrering av fordon för att hitta rätt kostnad etc.
 * 
 * 
 * REMOVE VEHICLE
 * Fungerar att ta bort och lägga till fordon. Availblesize ändras efterhand. Går att ta bort bil och lägga till 2 nya MC etc eller ta bort 1 MC och lägga till en ny.
 * Justera kostnad så att den printas ut.
 * Fixa felmed vid full parkering.
 * 
 */

namespace PragueParking2._0
{

    class Program
    {

        //public static ParkingHouse pHouseList = new();
        public static ParkingHouse pHouseList = new();

        public static void Main(string[] args)
        {
            //************************************
            // Main 
            //************************************


            //ParkingHouse.InitializeParkedVehiclesList<Vehicle>();   // Läser in sparad fil med parkerade bilar

            VehiclePriceList.InitiateParkingValues();

            var menuChoice = "";
            var subMenuChoice = "";
            var typeValue = new Vehicle();
            bool mainmenu = true;
            var table10 = new Table();

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

                menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(new[] {"Register new vehicle", "Handle parked vehicle",
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
                    case "Handle parked vehicle":
                        break;

                    case "Show parkingview":
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        AnsiConsole.Write(table4);
                        subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                            .AddChoices(new[] { "Small parkingview", "Large parkingview", "Go back" }));

                        switch (subMenuChoice)
                        {
                            case "Small parkingview": break;
                            case "Large parkingview": break;

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
                            .AddChoices(new[] { "Read new values via text file", "Go back" }));
                        switch (subMenuChoice)
                        {
                            case "Read new values via text file":
                                Console.WriteLine("Adding new values...");
                                Console.WriteLine("Please wait...");
                                foreach (var item in VehiclePriceList.parkingHouseValues)
                                {
                                    //Console.WriteLine("Vehicle type: " + item.Type + ", " + "Price per hour: " + item.Price + ", " + "Vehicle size in parkinghouse: " + item.VehicleSize);
                                }
                                VehiclePriceList.parkingHouseValues.Clear();
                                //ParkingHouse.InitiateParkingValues();
                                Console.WriteLine("All done. Printing new values");
                                foreach (var item in VehiclePriceList.parkingHouseValues)
                                {
                                    //Console.WriteLine("Vehicle type: " + item.Type + ", " + "Price per hour: " + item.Price + ", " + "Vehicle size in parkinghouse: " + item.VehicleSize);
                                }
                                Console.WriteLine("Press enter to return to main menu.");
                                Console.ReadKey();

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
                    bool check;
                    Console.WriteLine("Please enter a plate number: ");
                    AskForVehiclePlate(out plateNumber);

                    while (!String.IsNullOrEmpty(plateNumber))         // kollar om sträng är null och inte empty. Mest för while loop ska fortsätta om man 
                                                                       //skrivit fel för att enklare avbryta den.
                    {
                        if (ValitedateVehiclePlateInput(plateNumber) && ParkingHouse.CheckIfExists(plateNumber) == false)        //metod for checkifexists skall returnera false om den inte finns.
                        {
                            Console.Clear();
                            AnsiConsole.Write(table1);
                            check = AddVehicleToList(plateNumber, subMenuChoice);
                            if (check == true)
                            {
                                Console.WriteLine("Vehicle has now been parked");
                            }
                            else
                            {
                                Console.WriteLine("There no empty spaces");
                            }

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
                if (menuChoice == "Handle parked vehicle")
                {
                    string plateNumber;
                    int index;
                    bool check;
                    Console.Clear();
                    AnsiConsole.Write(table1);
                    AnsiConsole.Write(table4);

                    Console.WriteLine("Please enter a plate number: ");
                    AskForVehiclePlate(out plateNumber);
                    plateNumber = SendOkIfPlateIsOk(plateNumber, out index, out check);      // tar emot regnr och kontrollerar så det är korrekt.
                    if (plateNumber == null)
                    {
                        continue;           // om plateNumber kommer tillbaka null betyder det att man valt att ange endast enter vid angivning av regnr. Prompt                    som kommer upp om man skriver fel och ska ange nytt regnr. Skriv in nytt eller tryck enter för retur till                               mainmenu.
                    }
                    else if (check == false)
                    {
                        Console.WriteLine("We cannot find a vehicle with that plate number here");
                        Console.WriteLine("Press enter to return to mainmenu.");
                        Console.ReadKey();
                    }
                    else if (check == true)                    // om check kommer tillbaka true betyder det att fordonet finns och ett index har returnerats.
                    {
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                            .AddChoices(new[] { "Find vehicle", "Collect vehicle", "Move vehicle", "Go back" }));

                        switch (subMenuChoice)
                        {
                            case "Find vehicle": break;
                            case "Collect vehicle": break;
                            case "Move vehicle": break;
                            default:
                                if (subMenuChoice == "Go back")
                                {
                                    subMenuChoice = null;
                                }; break;
                        }
                        if (subMenuChoice == "Find vehicle")
                        {
                            if (check == true)
                            {
                                int spot = ParkingHouse.FindSpot(plateNumber);
                                Console.Clear();
                                AnsiConsole.Write(table1);
                                Console.WriteLine("We found a vehicle with that registration nu" +
                                    "mber here.");
                                //Console.WriteLine(ParkingSpot.ParkedVehicles[index]);
                                Console.WriteLine(index + 1);
                                Console.ReadKey();
                            }
                        }
                        else if (subMenuChoice == "Collect vehicle")
                        {
                            check = ParkingHouse.RemoveVehicle(plateNumber);
                            if (check == true)
                            {
                                Console.WriteLine("Vehicle removed");
                            }
                            else
                            {
                                Console.WriteLine("Vehicle not removed");
                            }

                            Console.ReadKey();
                        }
                        else if (subMenuChoice == "Move vehicle")
                        {

                            ParkingHouse.MoveVehicle(plateNumber);
                            Console.ReadKey();
                        }
                    }
                }

                //**********************************************************
                // Visa parkeringslista // hitta fordon
                //**********************************************************

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
        //************************************
        // Initierar parkerade fordon vid uppstart av program
        //************************************

        //static List<Vehicle> InitializeParkingList<T>()
        //{
        //    List<string> currentParkedVehicles = new List<string>(100);
        //    currentParkedVehicles = File.ReadAllLines(filePath).ToList();
        //    foreach (string vehicle in currentParkedVehicles)
        //    {
        //        string[] items = vehicle.Split(new char[] { ',', ' '}, StringSplitOptions.RemoveEmptyEntries);
        //        Vehicle v = new Vehicle(items[0], items[1], int.Parse(items[2]), DateTime.ParseExact(items[3], "dd/MM/yyyy-HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None));
        //        ParkingHouse.vehicles.Add(v);
        //    }
        //    return ParkingHouse.vehicles;
        //}

        //************************************
        // För att spara ned nya fordon till lista
        //************************************



        //************************************
        // För att lägga till fordon i parkeringslistan
        //************************************

        //TODO: Se över metod, flytta ut allt till menyn?
        public static bool AddVehicleToList(string vehiclePlate, string type)
        {
            bool check = false;
            if (type == "Car")
            {
                check = pHouseList.ParkVehicle(new Car(vehiclePlate));

            }
            else if (type == "MC")
            {
                check = pHouseList.ParkVehicle(new MC(vehiclePlate));

            }
            return check;
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
            bool regCheck;
            string specialChar = @"^[^\s!.,;:#¤%&\/\\()=?`´@'£$$€{}[\]]{0,10}$";
            Regex reg = new Regex(specialChar);
            regCheck = reg.IsMatch(vehiclePlate);

            return regCheck;
        }

        //************************************
        // Ska fungera som en mindre validerings metod. "mindre"
        //************************************

        public static string SendOkIfPlateIsOk(string plateNumber, out int index, out bool check)
        {
            //TODO: Se över metod, för krånlig1??
            check = false;
            index = -1;
            Console.WriteLine("\n");
            while (!String.IsNullOrEmpty(plateNumber))
            {
                if (ValitedateVehiclePlateInput(plateNumber))       // kontrollerar så regnr inte har specialtecken eller mellanslag
                {
                    //index = ParkingSpot.FindVehicle(plateNumber);           // kontrollerar så regnr inte redan finns
                    check = ParkingHouse.CheckIfExists(plateNumber);           // kontrollerar så regnr inte redan finns

                    if (check == true)
                    {
                        //index = ParkingSpot.ParkedVehicles.FindIndex(index => index.PlateNumber == plateNumber);        // Ska returnera index om forondet existerar i listan. 
                        check = true;
                        return plateNumber;             // returnerar regnr om det finns.
                    }
                    else if (check == false)
                    {
                        check = false;
                        return plateNumber;        // Returnerar false om regnr inte finns.
                    }
                }
                else
                {
                    plateNumber = null;
                    Console.WriteLine("You entered a non-valid plate number. Try again or press enter to return to main menu.");
                    AskForVehiclePlate(out plateNumber);
                }
            }
            return null;
        }

        //************************************
        // Visa parkerade bilar
        //************************************

        public static void Showparkingview()
        {
            for (int i = 0; i < ParkingHouse.PHouse.Count; i++)
            {
                Console.WriteLine(i);
                pHouseList.ToString();
            }
        }

        //TODO: Gömd kod
        //************************************************************************************************************
        #region gömd kod, pris per timme etc
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

        //************************************************************************************************************

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
    }
}

