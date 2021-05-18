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
    public enum Colors { Brown, LBlue, Pink, Orange, Red, Yellow, Green, DBlue, Not };
    public Colors Color = Colors.Not;

    //used if draw
    public enum DrawTypes { Chance, Chest, Not };
    public DrawTypes DrawType = DrawTypes.Not;

    //Used if Tax
    public int TaxPrice = 0;

    //Used if Go
    private int GoMoney = 200;

    //Used if Free parking
    public int FreeParkingMoney = 0;

    private GameObject Dice;

    private void Start()
    {
        Dice = GameObject.FindGameObjectWithTag("Dice");
    }

    public void LandedOn(GameObject Player)
    {
        switch (Type)
        {
            case Types.Property:
                if (Owner == null) //property is unowned
                {
                    //offer to buy
                    Dice.GetComponent<Dice>().OpenBuyPropertyMenu(this.gameObject);
                }
                else if (Owner != Player) //property is not owned by the player
                {
                    int ChargeAmount;
                    //charge rent and give to owner
                    if (NumHouses == 0 && NumHotels == 0)
                    {
                        if (IsColorSetOwned(Owner))
                        {
                            //charge player double rent
                            ChargeAmount = RentPrice * 2;
                        }
                        else
                        {
                            //charge player rent
                            ChargeAmount = RentPrice;
                        }
                    }
                    else if (NumHotels == 0)
                    {
                        //charge equal to HousePrices[NumHouses]
                        ChargeAmount = HousePrices[NumHouses];
                    }
                    else //hotel
                    {
                        //charge equal to price of hotel
                        ChargeAmount = HotelPrice;
                    }

                    //Charge player charge amount
                    Dice.GetComponent<Dice>().ChargePlayerToPlayer(Player, Owner, ChargeAmount, Player.GetComponent<Player>().Name + " owes " + Owner.GetComponent<Player>().Name + " " + ChargeAmount + " for landing on " + Name);
                }
                else //property is owned by the player
                {
                    //you are home, do nothing
                    Player.GetComponent<Player>().PostMove(); 
                }
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
                Dice.GetComponent<Dice>().ChargePlayerToBank(Player, TaxPrice, Player.GetComponent<Player>().Name + " owes " + TaxPrice + " to taxes.");
                //Player.GetComponent<Player>().ChargeMoney(TaxPrice);
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

    public void SetOwner(GameObject NewOwner)
    {
        Owner = NewOwner;
    }

    public GameObject GetOwner()
    {
        return Owner;
    }

    public bool IsColorSetOwned(GameObject Player)
    {
        for (int i = 0; i < ColorSet.Length; i++)
        {
            if (ColorSet[i].GetComponent<Tile>().GetOwner() != Player)
            {
                return false;
            }
        }
        return true;
    }
}
