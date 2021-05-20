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

    public void DrawCard(GameObject Player)
    {
        Effect(Player);

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
    }

    public void Effect(GameObject Player)
    {
        switch (DeckType)
        {
            case DeckTypes.Chance:
                switch (ID)
                {
                    case 0:
                        Message = "Advance to Go";
                        Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("Go"));
                        break;
                    case 1:
                        Message = "Quinn pooped on your bed. Pay 20";
                        Dice.GetComponent<Dice>().ChargePlayerToBank(Player, 20, Message);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    case 2:
                        Message = "You got an A on your exam. Collect 30";
                        Player.GetComponent<Player>().AddMoney(30);
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
                        Message = "Advance to Go";
                        Player.GetComponent<Player>().MoveTo(GameObject.FindGameObjectWithTag("Go"));
                        break;
                    case 1:
                        Message = "Quinn pooped on your bed. Pay 20";
                        Dice.GetComponent<Dice>().ChargePlayerToBank(Player, 20, Message);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    case 2:
                        Message = "You got an A on your exam. Collect 30";
                        Player.GetComponent<Player>().AddMoney(30);
                        Player.GetComponent<Player>().PostMove();
                        break;
                    default:
                        //Not a valid card
                        Player.GetComponent<Player>().PostMove();
                        break;
                }
                break;
        }
        print(Message);

    }

    IEnumerator MoveThenLand(GameObject Player)
    {
        yield return new WaitForSecondsRealtime(1);
        
    }
}
