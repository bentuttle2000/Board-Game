using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum DeckTypes { Chance, Chest };
    public DeckTypes DeckType;
    public int ID;

    private GameObject Decks;

    private int DeckSize = 3;

    private string Message;

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

    public void Effect(GameObject Player)
    {
        Player P = Player.GetComponent<Player>();
        switch (DeckType)
        {
            case DeckTypes.Chance:

                switch (ID)
                {
                    case 0:
                        Message = "Advance to Go";
                        P.SetLocation(GameObject.FindGameObjectWithTag("Go"));
                        P.LandedOn();
                        break;
                    case 1:
                        Message = "Quinn pooped on your bed. Pay 20";
                        Dice.GetComponent<Dice>().ChargePlayerToBank(Player, 20, Message);
                        break;
                    case 2:
                        Message = "You got an A on your exam. Collect 30";
                        P.AddMoney(30);
                        break;
                    default:
                        //Not a valid card
                        break;
                }


                break;

            case DeckTypes.Chest:




                break;
        }

    }
}
