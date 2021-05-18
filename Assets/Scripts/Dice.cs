using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    private int NumDoubles;
    private bool Again = false;
    public Canvas RollCanvas;
    public Canvas EndOfTurnCanvas;
    public Canvas BuyPropertyCanvas;
    public Canvas ChargePlayerCanvas;

    private GameObject PlayersTurn;
    private GameObject CurrentProperty;

    private GameObject MakePaymentFrom;
    private GameObject MakePaymentTo;
    private int MakePaymentAmount;

    private void Start()
    {
        transform.GetChild(0).GetComponent<Animator>().SetInteger("Dice", 1);
        transform.GetChild(1).GetComponent<Animator>().SetInteger("Dice", 1);
    }

    public int Roll()
    {
        RollCanvas.gameObject.SetActive(false);

        int Dice1 = Random.Range(1, 7); //returns random int 1-6 (the 7 is not inclusive)
        int Dice2 = Random.Range(1, 7);

        StartCoroutine(Wait(.2f, Dice1, Dice2));

        if (Dice1 == Dice2)
        {
            NumDoubles++;

            Again = true;
        }
        if (NumDoubles == 3) //If adding option for no max doubles add (&& DoubleCapEnabled) 
        {
            return 0;
        }
        return Dice1 + Dice2;
    }

    public void NewTurn(GameObject Player)
    {
        PlayersTurn = Player;

        EndOfTurnCanvas.gameObject.SetActive(false);
        BuyPropertyCanvas.gameObject.SetActive(false);
        ChargePlayerCanvas.gameObject.SetActive(false);

        NumDoubles = 0;
        Again = false;

        if (Player.GetComponent<Player>().IsInJail())
        {
            //ask if player wants to leave jail
            return; //roll canvas will be enabled when player selects yes or no
        }

        RollCanvas.gameObject.SetActive(true);

    }

    public void MovePlayer()
    {
        PlayersTurn.GetComponent<Player>().Move();
    }

    public bool PlayAgain()
    {
        if (Again)
        {
            RollCanvas.gameObject.SetActive(true);

            Again = false;
            return true;
        }
        return false;
    }

    public void EndOfTurn()
    {
        EndOfTurnCanvas.gameObject.SetActive(true);
    }

    public void EndTurn()
    {
        PlayersTurn.GetComponent<Player>().EndTurn();
    }

    IEnumerator Wait(float Sec, int D1, int D2)
    {
        transform.GetChild(0).GetComponent<Animator>().SetInteger("Dice", 0);
        yield return new WaitForSecondsRealtime(.1f);
        transform.GetChild(1).GetComponent<Animator>().SetInteger("Dice", 0);

        yield return new WaitForSecondsRealtime(Sec);
        transform.GetChild(0).GetComponent<Animator>().SetInteger("Dice", D1);
        transform.GetChild(1).GetComponent<Animator>().SetInteger("Dice", D2);
    }

    public void OpenBuyPropertyMenu(GameObject Property)
    {
        BuyPropertyCanvas.gameObject.SetActive(true);
        Vector4 TileColor = new Vector4(0, 0, 0, 1);

        switch (Property.GetComponent<Tile>().Color)
        {
            case Tile.Colors.Brown:
                TileColor = new Vector4(.7f, .25f, .05f, 1.0f);
                break;
            case Tile.Colors.LBlue:
                TileColor = new Vector4(.5f, 1.0f, 1.0f, 1.0f);
                break;
            case Tile.Colors.Pink:
                TileColor = new Vector4(1.0f, 0f, .6f, 1.0f);
                break;
            case Tile.Colors.Orange:
                TileColor = new Vector4(1.0f, .5f, 0f, 1.0f);
                break;
            case Tile.Colors.Red:
                TileColor = new Vector4(1.0f, 0f, 0f, 1.0f);
                break;
            case Tile.Colors.Yellow:
                TileColor = new Vector4(1.0f, 1.0f, 0f, 1.0f);
                break;
            case Tile.Colors.Green:
                TileColor = new Vector4(0f, .7f, .1f, 1.0f);
                break;
            case Tile.Colors.DBlue:
                TileColor = new Vector4(0f, .05f, .9f, 1.0f);
                break;
        }

        BuyPropertyCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = TileColor;

        BuyPropertyCanvas.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = Property.GetComponent<Tile>().Name;

        BuyPropertyCanvas.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = "Rent: " + Property.GetComponent<Tile>().RentPrice.ToString();

        string HousesText =
            "1 House:           " + Property.GetComponent<Tile>().HousePrices[0].ToString() +
            "\n2 House:           " + Property.GetComponent<Tile>().HousePrices[1].ToString() +
            "\n3 House:           " + Property.GetComponent<Tile>().HousePrices[2].ToString() +
            "\n4 House:           " + Property.GetComponent<Tile>().HousePrices[3].ToString()
            ;

        BuyPropertyCanvas.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = HousesText;

        BuyPropertyCanvas.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "Purchase: " + Property.GetComponent<Tile>().PurchasePrice.ToString();

        CurrentProperty = Property;
    }

    public void PurchaseProperty()
    {
        if (PlayersTurn.GetComponent<Player>().TakeMoney(CurrentProperty.GetComponent<Tile>().PurchasePrice))
        {
            CurrentProperty.GetComponent<Tile>().SetOwner(PlayersTurn);
            BuyPropertyCanvas.gameObject.SetActive(false);
            PlayersTurn.GetComponent<Player>().PostMove();
        }
    }
    public void DeclinePurchase()
    {
        //start auciton
        BuyPropertyCanvas.gameObject.SetActive(false);
        PlayersTurn.GetComponent<Player>().PostMove();
    }

    public void ChargePlayerToPlayer(GameObject PlayerFrom, GameObject PlayerTo, int Amount, string Message)
    {
        ChargePlayerCanvas.gameObject.SetActive(true);

        ChargePlayerCanvas.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Message;

        MakePaymentFrom = PlayerFrom;
        MakePaymentTo = PlayerTo;
        MakePaymentAmount = Amount;
    }

    public void ChargePlayerToBank(GameObject PlayerFrom, int Amount, string Message)
    {
        ChargePlayerCanvas.gameObject.SetActive(true);

        ChargePlayerCanvas.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Message;

        MakePaymentFrom = PlayerFrom;
        MakePaymentTo = null;
        MakePaymentAmount = Amount;
    }

    public void PayCharge()
    {
        if (MakePaymentFrom.GetComponent<Player>().TakeMoney(MakePaymentAmount))
        {
            if (MakePaymentTo != null) //pay player
            {
                ChargePlayerCanvas.gameObject.SetActive(false);
                MakePaymentTo.GetComponent<Player>().AddMoney(MakePaymentAmount);
            }
            else //pay free parking
            {
                ChargePlayerCanvas.gameObject.SetActive(false);
                GameObject.FindGameObjectWithTag("FreeParking").GetComponent<Tile>().FreeParkingMoney += MakePaymentAmount;
            }

            MakePaymentFrom.GetComponent<Player>().PostMove();
        }
    }
}
