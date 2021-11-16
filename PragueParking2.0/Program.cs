﻿using System;
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
    public class Program
    {
        public static void Main(string[] args)
        {
            Program program = new Program();
            ParkingHouse.SetValuesFromConfig();
            ParkingHouse parkingHouse = new ParkingHouse();
            //TableClass tableClass = new TableClass();

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
                .AddColumn(new TableColumn("Welcome to this program for parking vehicles").Centered());
                table1.Border = TableBorder.Horizontal;
                var table2 = new Table()
                    .AddColumn(new TableColumn("Please choose an option").Centered()).LeftAligned()
                    .Width(80);
                table2.Border = TableBorder.Horizontal;
                AnsiConsole.Write(table1);
                //AnsiConsole.Write(table2);
                menuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(new[] {"Register new vehicle", "Handle parked vehicle",
                    "Show parkingview", "Exit Program"}));

                switch (menuChoice)
                {
                    case "Register new vehicle":
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                            .AddChoices(new[] { "Car", "MC", "Go back" }));
                        Console.Clear();
                        AnsiConsole.Write(table1);

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
                            Console.WriteLine("Vehicle has now been parked");
                            Console.ReadKey();
                        }
                        else
                        {
                            Console.WriteLine("No empty spots left, please come back at another time.");
                            Console.ReadKey();
                        }
                    }
                    else if (check == true && vehicle != null)
                    {
                        Console.WriteLine("Vehicle already exist"); 
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
                    Console.WriteLine("Please enter a plate number: ");
                    program.AskForVehiclePlate(out plateNumber);
                    plateNumber = program.IsPlateOk(plateNumber, out vehicle, out check);      // tar emot regnr och kontrollerar så det är korrekt.
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
                        TimeSpan timeParked;
                        parkingHouse.CalculateTimeParked(vehicle, out timeParked);
                        int priceToPay = parkingHouse.CalculatePriceOnCheckOut(timeParked, vehicle.Price);
                        AnsiConsole.Write(table1);
                        program.PrintVehicle(vehicle, priceToPay);
                        subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                            .AddChoices(new[] { "Collect vehicle", "Move vehicle", "Go back" }));

                        switch (subMenuChoice)
                        {
                            case "Collect vehicle": break;
                            case "Move vehicle": break;
                            default:
                                if (subMenuChoice == "Go back")
                                {
                                    subMenuChoice = null;
                                }; break;
                        }

                        if (subMenuChoice == "Collect vehicle")
                        {
                            check = parkingHouse.RemoveVehicle(vehicle);
                            if (check == true)
                            {
                                Console.WriteLine("Vehicle removed. Customer needs to pay: {0} CSK", priceToPay);
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
                            program.AskForNewSpot(out spot);
                            int spotInt = program.IsSpotOk(spot, out check);
                            if (parkingHouse.IsSpotEmpty(vehicle, spotInt))
                            {
                                check = parkingHouse.RemoveVehicle(vehicle);
                                if (check == true)
                                {
                                    parkingHouse.ReParkVehicle(vehicle, spotInt);
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
                            };
                            break;
                    }

                    if (subMenuChoice == "Small parkingview")
                    {
                        Console.Clear();
                        //AnsiConsole.Write(table1);
                        parkingHouse.ShowParkingViewSmall();
                        Console.ReadKey();
                        continue;
                    }
                    else if (subMenuChoice == "Large parkingview")
                    {
                        Console.Clear();
                        AnsiConsole.Write(table1);
                        parkingHouse.ShowParkingViewLarge();
                        Console.ReadKey();
                        continue;
                    }
                }
                parkingHouse.SaveVehicleToFile();
            }
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
            string specialChar = @"^[^\s!.,;:#¤%&\/\\()=?`´@'£$$€{}[\]]{0,10}$";
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
            return spotInt;
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
        /// Method to convert from string to int - NOT IN USE! - REMOVE
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private int ConvertStringToInt(string input)
        {
            int intInput;
            return intInput = Int32.Parse(input);
        }
    }
    #region gömd kod

    //                if (menuChoice == "Configure program")
    //                {
    //                    Console.Clear();
    //                    AnsiConsole.Write(table1);
    //                    //AnsiConsole.Write(table4);
    //                    subMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
    //                        .AddChoices(new[] { "Read new values via text file", "Go back" }));
    //switch (subMenuChoice)
    //{
    //    case "Read new values via text file":
    //        Console.WriteLine("Adding new values...");
    //        Console.WriteLine("Please wait...");
    //        foreach (var item in VehiclePriceList.parkingHouseValues)
    //        {
    //            //Console.WriteLine("Vehicle type: " + item.Type + ", " + "Price per hour: " + item.Price + ", " + "Vehicle size in parkinghouse: " + item.VehicleSize);
    //        }
    //        VehiclePriceList.parkingHouseValues.Clear();
    //        //ParkingHouse.InitiateParkingValues();
    //        Console.WriteLine("All done. Printing new values");
    //        foreach (var item in VehiclePriceList.parkingHouseValues)
    //        {
    //            //Console.WriteLine("Vehicle type: " + item.Type + ", " + "Price per hour: " + item.Price + ", " + "Vehicle size in parkinghouse: " + item.VehicleSize);
    //        }
    //        Console.WriteLine("Press enter to return to main menu.");
    //        Console.ReadKey();
    //        break;

    //    default:
    //        break;
    //}
    //                }
    #endregion
}

