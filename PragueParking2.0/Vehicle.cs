﻿using System;

public class Vehicle
{
    public Vehicle()
    {

    }
    public Vehicle(string plateNumber)
    {
        this.PlateNumber = plateNumber;
        TimeParked = DateTime.Now;
        this.Spot = 0;
    }
    public string PlateNumber { get; set; }
    public int Size { get; set; }
    public int Price { get; set; }
    public int Spot { get; set; }
    public DateTime TimeParked { get; set; }    
}

