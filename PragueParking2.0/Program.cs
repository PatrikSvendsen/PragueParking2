using System;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace PragueParking2._0
{
    class Program
    {
        public static void Main(string[] args)
        {
            Program program = new Program();
            ParkingHouse.SetValuesFromConfig();
            ParkingHouse parkingHouse = new ParkingHouse();

            var menuChoice = String.Empty;
            var subMenuChoice = String.Empty;
            var typeValue = new Vehicle();
            bool mainmenu = true;
            var table10 = new Table();

            while (mainmenu == true)
            {
                menuChoice = null;
                subMenuChoice = null;
                Console.Clear();
                var table1 = new Table()
                .AddColumn(("Welcome to this program for parking vehicles"));
                table1.Border = TableBorder.Horizontal;
                var table2 = new Table()
                    .AddColumn(("Please choose an option"))
                    .Width(80);
                table2.Border = TableBorder.Horizontal;
                AnsiConsole.Write(table1);
                PrintFlag();
                Console.WriteLine("\t");
                menuChoice = RegisterMenuChoice();
                if (menuChoice == "Register new vehicle")
                {
                    Console.Clear();
                    AnsiConsole.Write(table1);
                    program.RegisterSubMenuChoice(menuChoice, out subMenuChoice);
                    if (subMenuChoice == "Go back")
                    {
                        continue;
                    }
                    Console.Clear();
                    AnsiConsole.Write(table1);
                    string plateNumber = String.Empty;
                    bool check;
                    Vehicle vehicle;
                    Console.WriteLine("Please enter a plate number: ");
                    program.AskForVehiclePlate(out plateNumber);
                    plateNumber = program.IsPlateOk(plateNumber, out vehicle, out check);
                    if (plateNumber == null)
                    {
                        continue;
                    }
                    if (check == false && vehicle == null)
                    {
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        check = parkingHouse.AddVehicleToList(plateNumber, subMenuChoice);
                        if (check == true)
                        {
                            ParkingHouse.CheckIfExists(plateNumber, out vehicle);
                            var table41 = new Table();
                            table41.AddColumn("Vehicle has now been parked at parkingspot: " + vehicle.Spot);
                            AnsiConsole.Write(table41);
                            vehicle = null;                    }
                        else
                        {
                            var table = new Table();
                            table.AddColumn("[orangered1]There are no empty spots left.[/]");
                            AnsiConsole.Write(table);
                        }
                    }
                    else if (check == true && vehicle != null)
                    {
                        Console.WriteLine("Vehicle already exist");
                    }
                Console.ReadKey();
                }

                if (menuChoice == "Handle parked vehicle")
                {
                    string plateNumber;
                    bool check;
                    Vehicle vehicle;
                    Console.Clear();
                    AnsiConsole.Write(table1);
                    Console.WriteLine("Please enter a plate number: ");
                    program.AskForVehiclePlate(out plateNumber);
                    plateNumber = program.IsPlateOk(plateNumber, out vehicle, out check);
                    if (plateNumber == null)
                    {
                        continue;
                    }
                    else if (check == false)
                    {
                        var table41 = new Table();
                        table41.AddColumn("[yellow]We cannot find a vehicle with that platenumber here![/]");
                        table41.AddRow("[yellow]Press enter to return to mainmenu.[/]");
                        AnsiConsole.Write(table41);
                    }
                    else if (check == true)
                    {
                        Console.Clear();
                        TimeSpan timeParked;
                        parkingHouse.CalculateTimeParked(vehicle, out timeParked);
                        int priceToPay = parkingHouse.CalculatePriceOnCheckOut(timeParked, vehicle.Price);
                        AnsiConsole.Write(table1);
                        program.PrintVehicle(vehicle, priceToPay);
                        program.RegisterSubMenuChoice(menuChoice, out subMenuChoice);

                        if (subMenuChoice == "Collect vehicle")
                        {
                            check = parkingHouse.RemoveVehicle(vehicle);
                            if (check == true)
                            {
                                var table42 = new Table();
                                table42.AddColumn("[green]Vehicle removed. Price to pay: " + priceToPay + "CZK[/]");
                                AnsiConsole.Write(table42);
                            }
                            else
                            {
                                var table43 = new Table();
                                table43.AddColumn("[red]Vehicle not removed, please try again[/]");
                                AnsiConsole.Write(table43);
                            }
                        }

                        else if (subMenuChoice == "Move vehicle")
                        {
                            string spot;
                            parkingHouse.ShowParkingViewSmall();
                            Console.WriteLine("\t");
                            program.AskForNewSpot(out spot);
                            int spotInt = program.IsSpotOk(spot, out check);
                            if (spotInt == -1)
                            {
                                continue;
                            }
                            if (parkingHouse.IsSpotEmpty(vehicle, spotInt))
                            {
                                check = parkingHouse.RemoveVehicle(vehicle);
                                if (check == true)
                                {
                                    var table = new Table();
                                    parkingHouse.ReParkVehicle(vehicle, spotInt);
                                    table.AddColumn("Vehicle can now be moved.");
                                    AnsiConsole.Write(table);
                                }
                            }
                            else
                            {
                                var table40 = new Table()
                                .AddColumn("[red]This spot is taken[/]");
                                AnsiConsole.Write(table40);
                            }
                        }
                    }
                    Console.ReadKey();
                }

                if (menuChoice == "Show parkingview")
                {
                    Console.Clear();
                    AnsiConsole.Write(table1);
                    program.RegisterSubMenuChoice(menuChoice, out subMenuChoice);

                    if (subMenuChoice == "Small parkingview")
                    {
                        parkingHouse.ShowParkingViewSmall();
                    }
                    else if (subMenuChoice == "Large parkingview")
                    {
                        parkingHouse.ShowParkingViewLarge();
                    }
                    else
                    {
                        continue;
                    }
                    Console.ReadKey();
                }

                if (menuChoice == "Configure values")
                {
                    Console.WriteLine("Please change the value inside config file then press enter to continue.");
                    Console.ReadKey();
                    ParkingHouse.SetValuesFromConfig();
                    Console.WriteLine("Press enter to return to main menu.");
                }

                if (menuChoice == "Reload Mainmenu")
                {
                    continue;
                }

                if (menuChoice == "Exit Program")
                {
                    mainmenu = false;
                    Console.WriteLine("Closing program...");
                    Console.ReadLine();
                }
                parkingHouse.SaveVehicleToFile();
            }
        }

        /// <summary>
        /// MEthod to ask for submenu choice
        /// </summary>
        /// <param name="menuChoice"></param>
        /// <param name="subMenuChoice"></param>
        /// <returns></returns>
        private string RegisterSubMenuChoice(string menuChoice, out string subMenuChoice)
        {
            subMenuChoice = null;
            if (menuChoice == "Register new vehicle")
            {
                switch (subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(new[] { "Car", "Mc", "Go back" })))
                {
                    case "Car": break;
                    case "Mc": break;

                    default: subMenuChoice = "Go back"; break;
                }
            }

            if (menuChoice == "Handle parked vehicle")
            {
                switch (subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                            .AddChoices(new[] { "Collect vehicle", "Move vehicle", "Go back" })))
                {
                    case "Collect vehicle": break;
                    case "Move vehicle": break;
                    default:
                        if (subMenuChoice == "Go back")
                        {
                            subMenuChoice = null;
                        }; break;
                }
            }

            if (menuChoice == "Show parkingview")
            {
                switch (subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .AddChoices(new[] { "Small parkingview", "Large parkingview", "Go back" })))
                {
                    case "Small parkingview": break;
                    case "Large parkingview": break;

                    default:
                        if (subMenuChoice == "Go back")
                        {
                            subMenuChoice = null;
                        };
                        break;
                }
            }
            return subMenuChoice;
        }

        /// <summary>
        /// Method to ask for Main menu choice.
        /// </summary>
        /// <returns></returns>
        private static string RegisterMenuChoice()
        {
            string menuChoice;
            var table3 = new Table();
            table3.AddColumns(menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(new[] {"Register new vehicle", "Handle parked vehicle",
                "Show parkingview","Configure values","Reload Mainmenu", "Exit Program" })));

            switch (menuChoice)
            {
                case "Register new vehicle": break;
                case "Handle parked vehicle": break;
                case "Show parkingview": break;
                case "Configure values": break;
                case "Reload Mainmenu": break;
                case "Exit Program": break;
                default:
                    Console.WriteLine("Please choose a correct option.");
                    break;
            }
            return menuChoice;
        }

        /// <summary>
        /// Method to print input vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="priceToPay"></param>
        private void PrintVehicle(Vehicle vehicle, int priceToPay)
        {
            var table4 = new Table()
                   .AddColumn(new TableColumn("Current vehicle: "))
                   .AddColumn(vehicle.GetType().ToString());
            table4.AddRow("Platenumber: ", vehicle.PlateNumber);
            table4.AddRow("Parked at spot: ", vehicle.Spot.ToString());
            table4.AddRow("Started parking: ", vehicle.TimeParked.ToString("HH:mm - d/M"));
            table4.AddRow("Current cost: ", priceToPay.ToString());
            table4.Border = TableBorder.Horizontal;
            AnsiConsole.Write(table4);
        }

        /// <summary>
        /// Method that ask userinput for platenumber.
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        private string AskForVehiclePlate(out string plateNumber)
        {
            plateNumber = " ";
            plateNumber = Console.ReadLine().ToUpper();
            return plateNumber;
        }

        /// <summary>
        /// Method that ask user for new parkingspot.
        /// </summary>
        /// <param name="spot"></param>
        /// <returns></returns>
        private string AskForNewSpot(out string spot)
        {
            Console.WriteLine("Please write a new spot: ");
            return spot = Console.ReadLine();
        }

        /// <summary>
        /// Method with regex to validate input from user is correct.
        /// </summary>
        /// <param name="vehiclePlate"></param>
        /// <returns></returns>
        private bool ValidateVehiclePlateInput(string vehiclePlate)
        {
            bool regCheck;
            string specialChar = @"^[^\s!.,;:#¤%&\/\\()=?`´@'§½£$$€{}[\]]{0,10}$";
            Regex reg = new Regex(specialChar);
            regCheck = reg.IsMatch(vehiclePlate);
            return regCheck;
        }

        /// <summary>
        /// Method with regex to validate that spot is numbers only and between 1-100 "ish"
        /// </summary>
        /// <param name="spot"></param>
        /// <returns></returns>
        private bool ValidateSpotInput(string spot)
        {
            bool spotCheck;
            string specialChar = @"^[1-9][0-9]?$|^100\d*$";
            Regex reg = new Regex(specialChar);
            spotCheck = reg.IsMatch(spot);
            return spotCheck;
        }

        /// <summary>
        /// Method to validate and check spotinput from user. 
        /// </summary>
        /// <param name="spot"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        private int IsSpotOk(string spot, out bool check)
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
            return -1;
        }

        /// <summary>
        /// Method to check if platenumber is correct and it doesnt exist. If it does, it returns the vehicle obj else null.
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <param name="vehicle"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        private string IsPlateOk(string plateNumber, out Vehicle vehicle, out bool check)
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
                        return plateNumber;             // returnerar regnr om det finns.
                    }
                    else if (check == false)
                    {
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

        /// <summary>
        /// Method to print the flag.
        /// </summary>
        public static void PrintFlag()
        {
            var Rule1 = new Rule();
            Rule1.Style = Style.Parse("White");

            var Rule3 = new Rule("[blue]Prague[/]").LeftAligned();
            Rule3.Style = Style.Parse("White");
            var Rule4 = new Rule("[blue]Parking[/]").LeftAligned();
            Rule4.Style = Style.Parse("Red");
            var Rule5 = new Rule("[blue]2.0[/]").LeftAligned();
            Rule4.Style = Style.Parse("Red");
            var Rule6 = new Rule();
            Rule5.Style = Style.Parse("Red");
            AnsiConsole.Write(Rule3);
            AnsiConsole.Write(Rule4);
            AnsiConsole.Write(Rule5);
        }
    }
}

