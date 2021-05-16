﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public string Name;
    
    public enum Types { Property, Draw, Tax, Go, VisitingJail, InJail, GoToJail, Free };
    public Types Type;

    //used if property
    private GameObject Owner = null;
    public int PurchasePrice = 0;
    public int HousePrice = 0;
    public int[] RentPrice = { 0 };
    private int NumHouses = 0;

    //used if draw
    public enum DrawTypes { Chance, Chest, Not };
    public DrawTypes DrawType = DrawTypes.Not;

    //Used if Tax
    public int TaxPrice = 0;

    //Used if Go
    private int GoMoney = 200;

    //Used if Free parking
    private int FreeParkingMoney = 0;

    public void LandedOn(GameObject Player)
    {
        switch (Type)
        {
            case Types.Property:
                if (Owner != null)
                {
                    //offer to buy
                }
                else
                {
                    //charge rent and give to owner
                    int Price = RentPrice[NumHouses];
                    Player.GetComponent<Player>().TakeMoney(Price);
                }
                break;
            case Types.Draw:
                break;
            case Types.Tax:
                break;
            case Types.Go:
                break;
            case Types.VisitingJail:
                break;
            case Types.InJail:
                break;
            case Types.GoToJail:
                break;
            case Types.Free:
                break;
        }

    }
}