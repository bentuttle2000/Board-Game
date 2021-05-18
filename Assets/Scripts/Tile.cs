using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public string Name;

    public enum Types { Property, Railroad, Utility, Draw, Tax, Go, VisitingJail, InJail, GoToJail, Free };
    public Types Type;

    //used if property (RR and Utility use Purchase Price
    private GameObject Owner = null;
    public int PurchasePrice = 0;
    public int HousePrice = 0;
    public int RentPrice;
    public int[] HousePrices = {0, 0, 0, 0};
    private int NumHouses = 0;
    public int HotelPrice = 0;
    private int NumHotels = 0;
    public GameObject[] ColorSet;

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
                if (Owner != null) //property is unowned
                {
                    //offer to buy
                }
                else if (Owner != Player) //property is not owned by the player
                {
                    //charge rent and give to owner
                }
                else //property is owned by the player
                {
                    //you are home
                }
                Player.GetComponent<Player>().PostMove(); //move this later
                break;
            case Types.Draw:
                if (DrawType == DrawTypes.Chance)
                {
                    //draw chance
                }
                else
                {
                    //draw community chest
                }
                Player.GetComponent<Player>().PostMove(); //move this later
                break;
            case Types.Tax:
                //charge player tax amount
                Player.GetComponent<Player>().TaxMoney(TaxPrice);
                break;
            case Types.GoToJail:
                //send player to jail
                Player.GetComponent<Player>().SendToJail();
                break;
            case Types.Free:
                //give player money on free parking
                Player.GetComponent<Player>().AddMoney(FreeParkingMoney);
                FreeParkingMoney = 0;
                Player.GetComponent<Player>().PostMove();
                break;
            default:
                //do nothing on go and visiting jail
                Player.GetComponent<Player>().PostMove();
                break;
        }
    }
}
