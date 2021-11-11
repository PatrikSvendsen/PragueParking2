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
 * 
 * ParkVehile
 * Fungerar, lägger till bil eller mc om plats finns. Dock Om bil tas bort på plats 1 och en mc står på plats 2 så kommer den att lägga till mc på plats 1. Funktionen är
 * inte "optimerad"
 * 
 * 
 * REMOVE VEHICLE
 * Fungerar att ta bort och lägga till fordon. Availblesize ändras efterhand. Går att ta bort bil och lägga till 2 nya MC etc eller ta bort 1 MC och lägga till en ny.
 * Justera kostnad så att den printas ut.
 * Fixa felmed vid full parkering.
 * 
 * 
 * MOVE VEHICLE
 * 
 * När man försöker flytta fordon och platsen är tagen ska man inte slungas ut till huvudmenyn utan tillbaka till submenu för hantera fordon. Dubbelkolla resten av 
 */

namespace PragueParking2._0
{
    class Program
    {
        //public static ParkingHouse pHouseList = new();

        public static void Main(string[] args)
        {

            DataConfig.InitiateData.SetValuesFromConfig();

            ParkingHouse pHouseList = new();

            //DataConfig.Config.InitiateData();

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
                    case "Handle parked vehicle": break;
                    case "Show parkingview": break;
                    case "Configure program":
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

                if (menuChoice == "Register new vehicle" && subMenuChoice != null)
                {
                    Console.Clear();
                    AnsiConsole.Write(table1);
                    string plateNumber = String.Empty;

                    bool check;
                    Vehicle vehicle;
                    Console.WriteLine("Please enter a plate number: ");
                    AskForVehiclePlate(out plateNumber);
                    plateNumber = IsPlateOk(plateNumber, out vehicle, out check);
                    if (plateNumber == null)
                    {
                        continue;
                    }
                    if (check == false && vehicle == null)
                    {
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        check = AddVehicleToList(plateNumber, subMenuChoice);
                        if (check == true)
                        {
                            Console.WriteLine("Vehicle has now been parked");
                            Console.ReadKey();
                        }
                    }
                    else if (check == true && vehicle != null)
                    {
                        Console.WriteLine("Vehicle already exist"); // TODO: Måste dubbelkollas
                        Console.ReadKey();
                    }
                }
                if (menuChoice == "Handle parked vehicle")
                {
                    string plateNumber;
                    bool check;
                    Vehicle vehicle;
                    Console.Clear();
                    AnsiConsole.Write(table1);
                    AnsiConsole.Write(table4);

                    Console.WriteLine("Please enter a plate number: ");
                    AskForVehiclePlate(out plateNumber);
                    plateNumber = IsPlateOk(plateNumber, out vehicle, out check);      // tar emot regnr och kontrollerar så det är korrekt.
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
                                Console.WriteLine("We found a vehicle with that registration number here.");
                                Console.ReadKey();
                            }
                        }
                        else if (subMenuChoice == "Collect vehicle")
                        {
                            bool priceCheck;
                            int price;
                            priceCheck = ParkingHouse.CalculatePriceOnCheckOut(vehicle.Price, vehicle.timeParked, out price);
                            check = ParkingHouse.RemoveVehicle(vehicle);
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
                            string spot;
                            AskForNewSpot(out spot);
                            int spotInt = IsSpotOk(spot, out check); 
                            if (ParkingHouse.IsSpotEmpty(vehicle, spotInt))
                            {
                                check = ParkingHouse.RemoveVehicle(vehicle);
                                if (check == true)
                                {
                                    ParkingHouse.ReParkVehicle(vehicle, spotInt); 
                                    Console.WriteLine("Vehicle can now be moved.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("This spot is taken.");
                            }
                            Console.ReadKey();
                        }
                    }
                }
                if (menuChoice == "Show parkingview")
                {
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

                    if (subMenuChoice == "Small parkingview")
                    {
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        AnsiConsole.Write(table2);
                        //Showparkingview();
                        Console.ReadKey();
                        continue;
                    }
                }
                if (menuChoice == "Configure program")
                {
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
                }
                DataConfig.InitiateData.SaveVehicleToFile();
                Console.ReadKey();
            }
        }

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

        public static bool AddVehicleToList(string vehiclePlate, string type)
        {
            bool check = false;
            if (type == "Car")
            {
                check = ParkingHouse.ParkVehicle(new Car(vehiclePlate));
            }
            else if (type == "MC")
            {
                check = ParkingHouse.ParkVehicle(new MC(vehiclePlate));
            }
            return check;

        }

        public static string AskForVehiclePlate(out string plateNumber)
        {
            plateNumber = " ";
            plateNumber = Console.ReadLine().ToUpper();
            return plateNumber;
        }

        public static string AskForNewSpot(out string spot)
        {
            Console.WriteLine("Please write a new spot: ");
            return spot = Console.ReadLine();
        }

        public static bool ValidateVehiclePlateInput(string vehiclePlate)
        {
            bool regCheck;
            string specialChar = @"^[^\s!.,;:#¤%&\/\\()=?`´@'£$$€{}[\]]{0,10}$";
            Regex reg = new Regex(specialChar);
            regCheck = reg.IsMatch(vehiclePlate);
            return regCheck;
        }

        public static bool ValidateSpotInput(string spot)
        {
            bool spotCheck;
            string specialChar = @"^[1-9][0-9]?$|^100\d*$";
            Regex reg = new Regex(specialChar);
            spotCheck = reg.IsMatch(spot);
            return spotCheck;
        }

        public static int IsSpotOk(string spot, out bool check)
        {
            check = false;
            int spotInt = 0;
            while (!String.IsNullOrEmpty(spot))
            {
                if (ValidateSpotInput(spot))
                {
                    spotInt = Int32.Parse(spot);
                    if (spotInt <= 100 && spotInt > 1)
                    {
                        check = true;
                        return spotInt;
                    }
                    else
                    {
                        check = false;
                        return spotInt;
                    }
                }
                else
                {
                    spot = null;
                    Console.WriteLine("Incorrect input, please enter numbers only.");
                    AskForNewSpot(out spot);
                }
            }
            return spotInt;
        }

        /// <summary>
        /// Returnerar false om regnr inte finns, true om det finns. Finns det skickas vehicle med.
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <param name="vehicle"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        public static string IsPlateOk(string plateNumber, out Vehicle vehicle, out bool check)
        {
            check = false;

            Console.WriteLine("\n");
            while (!String.IsNullOrEmpty(plateNumber))
            {
                if (ValidateVehiclePlateInput(plateNumber))       // kontrollerar så regnr inte har specialtecken eller mellanslag
                {
                    check = ParkingHouse.CheckIfExists(plateNumber, out vehicle);           // kontrollerar så regnr inte redan finns

                    if (check == true)
                    {
                        //check = true;
                        return plateNumber;             // returnerar regnr om det finns.
                    }
                    else if (check == false)
                    {
                        //check = false;
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
            vehicle = null;
            return null;
        }
        public static int ConvertStringToInt(string spot)
        {
            int intSpot;
            return intSpot = Int32.Parse(spot);
        }


        //TODO: Fixa så att detta finns i ParkingHouse
        //public static void Showparkingview()
        //{
        //    for (int i = 0; i < ParkingHouse.PHouse.Count; i++)
        //    {
        //        Console.WriteLine(i);
        //        pHouseList.ToString();
        //    }
        //}

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

