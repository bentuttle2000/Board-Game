using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public string Name;

    public enum Types { Property, Railroad, Utility, Draw, Tax, Go, VisitingJail, InJail, GoToJail, Free };
    public Types Type;

    //used if property (RR and Utility use Purchase Price and IsMortgaged) (RR Uses houses for payment price)
    private GameObject Owner = null;
    public int PurchasePrice = 0;
    public int HousePrice = 0;
    public int RentPrice;
    public int[] HousePrices = { 0, 0, 0, 0 };
    private int NumHouses = 0;
    public int HotelPrice = 0;
    private int NumHotels = 0;
    public GameObject[] ColorSet;
    public enum Colors { Brown, LBlue, Pink, Orange, Red, Yellow, Green, DBlue, Not };
    public Colors Color = Colors.Not;
    private bool IsMortgaged = false;

    //used if draw
    public enum DrawTypes { Chance, Chest, Not };
    public DrawTypes DrawType = DrawTypes.Not;

    //Used if Tax
    public int TaxPrice = 0;

    //Used if Free parking
    public int FreeParkingMoney = 0;

    private GameObject Dice;

    private void Start()
    {
        Dice = GameObject.FindGameObjectWithTag("Dice");
    }

    private void Update()
    {
        if (Type == Types.Property)
        {
            UpdateHouses();
        }
        if (Type == Types.Property || Type == Types.Railroad || Type == Types.Utility)
        {
            UpdateMortage();
        }
    }

    public void UpdateHouses()
    {
        //update houses
        for (int i = 1; i <= 4; i++)
        {
            bool Active = false;
            if (NumHouses >= i)
            {
                Active = true;
            }
            transform.GetChild(i).gameObject.SetActive(Active); //set necessary houses active
        }

        if (NumHotels == 1)
        {
            transform.GetChild(5).gameObject.SetActive(true); //set hotel active
        }
        else
        {
            transform.GetChild(5).gameObject.SetActive(false); //set hotel inactive
        }
    }

    public void UpdateMortage()
    {
        if (IsMortgaged)
        {
            transform.GetChild(0).gameObject.SetActive(true); //set mortgage active
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false); //set mortgage inactive

        }
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
                else if (Owner != Player && !IsMortgaged) //property is not owned by the player and is not mortgaged, must pay owner
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
            case Types.Railroad:
                if (Owner == null) //property is unowned
                {
                    //offer to buy
                    Dice.GetComponent<Dice>().OpenBuyPropertyMenu(this.gameObject);
                }
                else if (Owner != Player && !IsMortgaged) //property is not owned by the player and is not mortgaged, must pay owner
                {
                    int ChargeAmount;

                    int j = 0;

                    for (int i = 0; i < ColorSet.Length; i++)
                    {
                        if (Owner == ColorSet[i].GetComponent<Tile>().Owner)
                        {
                            j++;
                        }
                    }

                    ChargeAmount = HousePrices[j - 1];

                    //Charge player charge amount
                    Dice.GetComponent<Dice>().ChargePlayerToPlayer(Player, Owner, ChargeAmount, Player.GetComponent<Player>().Name + " owes " + Owner.GetComponent<Player>().Name + " " + ChargeAmount + " for landing on " + Name);
                }
                else //property is owned by the player
                {
                    //you are home, do nothing
                    Player.GetComponent<Player>().PostMove();
                }
                break;
            case Types.Utility:
                if (Owner == null) //property is unowned
                {
                    //offer to buy
                    Dice.GetComponent<Dice>().OpenBuyPropertyMenu(this.gameObject);
                }
                else if (Owner != Player && !IsMortgaged) //property is not owned by the player and is not mortgaged, must pay owner
                {
                    int ChargeAmount;

                    if (ColorSet[0].GetComponent<Tile>().Owner != ColorSet[1].GetComponent<Tile>().Owner) //owner owns 1
                    {
                        ChargeAmount = Dice.GetComponent<Dice>().GetRecentRoll() * 4;
                    }
                    else //owner owns both
                    {
                        ChargeAmount = Dice.GetComponent<Dice>().GetRecentRoll() * 10;
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
                    Player.GetComponent<Player>().DrawCard(0);
                }
                else
                {
                    //draw community chest
                    Player.GetComponent<Player>().DrawCard(1);
                }
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

    public void ManageProperty()
    {
        Dice.GetComponent<Dice>().OpenManagePropertyMenu(this.gameObject);
    }

    public void BuyHouse()
    {
        for (int i = 0; i < ColorSet.Length; i++)
        {
            if (ColorSet[i].GetComponent<Tile>().IsMortgaged)
            {
                return; //do not let houses go if any property in set is mortgaegs
            }
        }

        for (int i = 0; i  < ColorSet.Length; i++)
        {
            if (ColorSet[i].GetComponent<Tile>().NumHouses - this.NumHouses > 1 || ColorSet[i].GetComponent<Tile>().NumHouses - this.NumHouses < 0)
            {
                if (ColorSet[i].GetComponent<Tile>().NumHotels == 0)
                {
                    return; //Can not build uneven houses
                }
            }
        }

        if (IsColorSetOwned(Owner) && NumHotels == 0)
        {
            if (Owner.GetComponent<Player>().TakeMoney(HousePrice))
            {
                if (NumHouses < 4)
                {
                    NumHouses++;
                }
                else
                {
                    NumHouses = 0;
                    NumHotels = 1;
                }
            }
        }
    }

    public void SellHouse()
    {
        for (int i = 0; i < ColorSet.Length; i++)
        {
            if (ColorSet[i].GetComponent<Tile>().NumHouses - this.NumHouses > 0 || ColorSet[i].GetComponent<Tile>().NumHouses - this.NumHouses < -1)
            {
                if (NumHotels == 0)
                {
                    return; //Can not build uneven houses
                }
            }
        }

        if (NumHotels == 1)
        {
            NumHotels = 0;
            NumHouses = 4;
            Owner.GetComponent<Player>().AddMoney(HousePrice / 2);
        }
        else if (NumHouses > 0)
        {
            NumHouses--;
            Owner.GetComponent<Player>().AddMoney(HousePrice / 2);
        }
    }

    public void Mortgage()
    {
        for (int i = 0; i < ColorSet.Length; i++)
        {
            if (ColorSet[i].GetComponent<Tile>().NumHouses > 0 || ColorSet[i].GetComponent<Tile>().NumHotels > 0)
            {
                return; //do not let mortgage if any properties in set have houses
            }
        }

        if (NumHouses == 0 && NumHotels == 0 && !IsMortgaged)
        {
            Owner.GetComponent<Player>().AddMoney(PurchasePrice / 2);
            IsMortgaged = true;
        }
    }

    public void Unmortgage()
    {
        if (IsMortgaged)
        {
            if (Owner.GetComponent<Player>().TakeMoney(PurchasePrice))
            {
                IsMortgaged = false;
            }
        }
    }

    public void TileReset()
    {
        IsMortgaged = false;
        NumHouses = 0;
        NumHotels = 0;
        Owner = null;
    }

    public int GetRepairCost()
    {
        int Price = 0;
        if (NumHotels == 1)
        {
            Price = 100;
        }
        else
        {
            Price = NumHouses * 25;
        }
        return Price;
    }
}
