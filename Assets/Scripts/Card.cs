using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum DeckTypes { Chance, Chest };
    public DeckTypes DeckType;
    public int ID;

    private GameObject Decks;

    private int DeckSize = 15;

    private GameObject Dice;

    public void Start()
    {
        Decks = GameObject.FindGameObjectWithTag("Decks");
        Dice = GameObject.FindGameObjectWithTag("Dice");
        Shuffle(0);
        Shuffle(1);
    }

    public void Shuffle(int Deck)
    {
        for (int i = 0; i < DeckSize; i++)
        {
            int TempID = Decks.transform.GetChild(Deck).GetChild(i).GetComponent<Card>().ID;
            int NewIndex = Random.Range(0, DeckSize);
            Decks.transform.GetChild(Deck).GetChild(i).GetComponent<Card>().ID = Decks.transform.GetChild(Deck).GetChild(NewIndex).GetComponent<Card>().ID;
            Decks.transform.GetChild(Deck).GetChild(NewIndex).GetComponent<Card>().ID = TempID;
        }
    }

    public void DrawCard(GameObject Player)
    {
        int Deck = 0;

        int TempID = ID;

        switch (DeckType)
        {
            case DeckTypes.Chance:
                Deck = 0;
                break;
            case DeckTypes.Chest:
                Deck = 1;
                break;
        }

        for (int i = 1; i < DeckSize; i++)
        {
            Decks.transform.GetChild(Deck).GetChild(i - 1).GetComponent<Card>().ID = Decks.transform.GetChild(Deck).GetChild(i).GetComponent<Card>().ID;
        }
        Decks.transform.GetChild(Deck).GetChild(DeckSize - 1).GetComponent<Card>().ID = TempID;

        Dice.GetComponent<Dice>().ResolveCard(Deck);
    }

    public void Effect(GameObject Player)
    {
        switch (DeckType)
        {
            case DeckTypes.Chance:
                switch (ID)
                {
                    case 0:
                        Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("Go"));
                        break;
                    case 1:
                        Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("Red3"));
                        break;
                    case 2:
                        Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("Pink1"));
                        break;
                    case 3:
                        if(Player.GetComponent<Player>().GetLocationAsInt() < 12 || Player.GetComponent<Player>().GetLocationAsInt() > 28) //12 is first utility, 28 is second utility
                        {
                            //advance to Utility1
                            Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("Utility1"));
                        }
                        else
                        {
                            //advance to Utility2
                            Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("Utility2"));
                        }
                        break;
                    case 4:
                        if (Player.GetComponent<Player>().GetLocationAsInt() < 5 || Player.GetComponent<Player>().GetLocationAsInt() > 35)
                        {
                            //advance to RR1
                            Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("RR1"));
                        }
                        else if (Player.GetComponent<Player>().GetLocationAsInt() < 15)
                        {
                            //advance to RR2
                            Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("RR2"));
                        }
                        else if (Player.GetComponent<Player>().GetLocationAsInt() < 25)
                        {
                            //advance to RR3
                            Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("RR3"));
                        }
                        else //if (Player.GetComponent<Player>().GetLocationAsInt() < 35)
                        {
                            //advance to RR4
                            Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("RR4"));
                        }
                        break;
                    case 5:
                        Player.GetComponent<Player>().AddMoney(50);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    case 6:
                        Player.GetComponent<Player>().AddMoney(100);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    case 7:
                        Player.GetComponent<Player>().GoBackSpaces(3);
                        break;
                    case 8:
                        Player.GetComponent<Player>().SendToJail();
                        break;
                    case 9:
                        Player.GetComponent<Player>().PayHouseRepairs();
                        break;
                    case 10:
                        Dice.GetComponent<Dice>().ChargePlayerToBank(Player, 15, GetMessage());
                        break;
                    case 11:
                        Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("RR1"));
                        break;
                    case 12:
                        Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("DBlue2"));
                        break;
                    case 13:
                        Dice.GetComponent<Dice>().ChargePlayerToBank(Player, 100, GetMessage());
                        break;
                    case 14:
                        Player.GetComponent<Player>().AddMoney(150);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    default:
                        //Not a valid card
                        Player.GetComponent<Player>().PostMove();
                        break;
                }
                break;

            case DeckTypes.Chest:
                switch (ID)
                {
                    case 0:
                        Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("Go"));
                        break;
                    case 1:
                        Player.GetComponent<Player>().AddMoney(200);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    case 2:
                        Dice.GetComponent<Dice>().ChargePlayerToBank(Player, 50, GetMessage());
                        break;
                    case 3:
                        Player.GetComponent<Player>().AddMoney(50);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    case 4:
                        Player.GetComponent<Player>().AddMoney(100);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    case 5:
                        Player.GetComponent<Player>().SendToJail();
                        break;
                    case 6:
                        Player.GetComponent<Player>().AddMoney(100);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    case 7:
                        Player.GetComponent<Player>().AddMoney(20);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    case 8:
                        Player.GetComponent<Player>().AddMoney(30);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    case 9:
                        Player.GetComponent<Player>().AddMoney(100);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    case 10:
                        Dice.GetComponent<Dice>().ChargePlayerToBank(Player, 50, GetMessage());
                        break;
                    case 11:
                        Dice.GetComponent<Dice>().ChargePlayerToBank(Player, 50, GetMessage());
                        break;
                    case 12:
                        Dice.GetComponent<Dice>().ChargePlayerToBank(Player, 25, GetMessage());
                        break;
                    case 13:
                        Player.GetComponent<Player>().PayHouseRepairs();
                        break;
                    case 14:
                        Player.GetComponent<Player>().AddMoney(10);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    default:
                        //Not a valid card
                        break;
                }
                break;
        }
    }

    public string GetMessage()
    {
        string Message = "";

        switch (DeckType)
        {
            case DeckTypes.Chance:
                switch (ID)
                {
                    case 0:
                        Message = "Advance to Go";
                        break;
                    case 1:
                        Message = "Advance to Red3";
                        break;
                    case 2:
                        Message = "Advance to Pink1";
                        break;
                    case 3:
                        Message = "Advance to the nearest Utility";
                        break;
                    case 4:
                        Message = "Advance to the nearest Railroad";
                        break;
                    case 5:
                        Message = "You did something cool. Collect 50";
                        break;
                    case 6:
                        Message = "You sold your soul. collect 100 (It was barely worth the 100)";
                        break;
                    case 7:
                        Message = "You slipped on a banana. Go Back 3 Spaces";
                        break;
                    case 8:
                        Message = "Go To Jail";
                        break;
                    case 9:
                        Message = "If you have houses, you are screwed. Make Home Repairs. (25 per house, 100 per hotel)";
                        break;
                    case 10:
                        Message = "Pay Poor tax of 15";
                        break;
                    case 11:
                        Message = "Take a trip to RR1";
                        break;
                    case 12:
                        Message = "Take a walk on the boardwalk";
                        break;
                    case 13:
                        Message = "Pay 100. Just do it.";
                        break;
                    case 14:
                        Message = "Collect 150 because you are cool.";
                        break;
                    default:
                        //Not a valid card
                        break;
                }
                break;

            case DeckTypes.Chest:
                switch (ID)
                {
                    case 0:
                        Message = "Advance to Go";
                        break;
                    case 1:
                        Message = "Bank Error: Collect $200";
                        break;
                    case 2:
                        Message = "Doctor's Fees. pay 50";
                        break;
                    case 3:
                        Message = "From sale of stock you get 50";
                        break;
                    case 4:
                        Message = "You inherit $100";
                        break;
                    case 5:
                        Message = "Go to Jail";
                        break;
                    case 6:
                        Message = "Holiday Funds mature. Recieve 100";
                        break;
                    case 7:
                        Message = "Income Tax Refund. Collect 20";
                        break;
                    case 8:
                        Message = "It is your birthday, collect 30";
                        break;
                    case 9:
                        Message = "Life insurance Matures. collect 100";
                        break;
                    case 10:
                        Message = "Hospital fees. pay $50";
                        break;
                    case 11:
                        Message = "School fees. pay 50";
                        break;
                    case 12:
                        Message = "recieve $25 consultancy fee";
                        break;
                    case 13:
                        Message = "Assessed for street repairs";
                        break;
                    case 14:
                        Message = "You have won Second Prize in a beauty contest. collect 10";
                        break;
                    default:
                        //Not a valid card
                        break;
                }
                break;
        }

        return Message;
    }
}
