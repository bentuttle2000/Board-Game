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
    public Canvas LeaveJailCanvas;
    public Canvas ManagePropertyCanvas;
    public Canvas TradeCanvas;
    public Canvas TradeDisplayCanvas;
    public Canvas SelectPlayerCanvas;
    public Canvas SelectGiveCanvas;
    public Canvas SelectWantCanvas;
    public Canvas ResolveCardCanvas;

    private GameObject PlayersTurn;
    private GameObject CurrentProperty;

    private GameObject MakePaymentFrom;
    private GameObject MakePaymentTo;
    private int MakePaymentAmount;

    private int MostRecentRoll;

    private GameObject Players;

    private GameObject TradePartner = null;

    //trading lists
    private List<GameObject> GiveList = new List<GameObject>();
    private List<GameObject> WantList = new List<GameObject>();

    private int GiveMoney = 0;
    private int WantMoney = 0;

    //Card stuff
    private GameObject Decks;
    private int CurrentDeck = 0;


    private void Start()
    {
        transform.GetChild(0).GetComponent<Animator>().SetInteger("Dice", 1);
        transform.GetChild(1).GetComponent<Animator>().SetInteger("Dice", 1);
        Players = GameObject.FindGameObjectWithTag("Players");
        Decks = GameObject.FindGameObjectWithTag("Decks");
    }

    private void Update()
    {
        if (PlayersTurn == null)
        {
            Players.transform.GetChild(0).GetComponent<Player>().StartTurn();
        }

        //display trade
        if (TradePartner != null)
        {
            UpdateTrade();
        }
        else
        {
            TradeDisplayCanvas.gameObject.SetActive(false);
        }
        
    }

    public void UpdateTrade()
    {
        TradeDisplayCanvas.gameObject.SetActive(true);

        for (int i = 0; i < 28; i++)
        {
            if (GiveList.Contains(SelectGiveCanvas.transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<DisplayPiece>().Tile.gameObject))
            {
                TradeDisplayCanvas.transform.GetChild(0).GetChild(3).GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                TradeDisplayCanvas.transform.GetChild(0).GetChild(3).GetChild(i).gameObject.SetActive(false);
            }

            if (WantList.Contains(SelectGiveCanvas.transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<DisplayPiece>().Tile.gameObject))
            {
                TradeDisplayCanvas.transform.GetChild(1).GetChild(3).GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                TradeDisplayCanvas.transform.GetChild(1).GetChild(3).GetChild(i).gameObject.SetActive(false);
            }

        }
        TradeDisplayCanvas.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = PlayersTurn.GetComponent<Player>().Name;
        TradeDisplayCanvas.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "Money: " + GiveMoney;

        TradeDisplayCanvas.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = TradePartner.GetComponent<Player>().Name;
        TradeDisplayCanvas.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = "Money: " + WantMoney;
    }

    public int Roll()
    {
        RollCanvas.gameObject.SetActive(false);

        int Dice1 = Random.Range(1, 7); //returns random int 1-6 (the 7 is not inclusive)
        int Dice2 = Random.Range(1, 7);

        MostRecentRoll = Dice1 + Dice2;

        StartCoroutine(Wait(.2f, Dice1, Dice2));

        if (Dice1 == Dice2)
        {
            NumDoubles++;

            Again = true;

            if (PlayersTurn.GetComponent<Player>().IsInJail()) //if player in jail when double, release them
            {
                LeaveJailRoll();
            }
        }

        if (NumDoubles == 3) //if third doubles send to jail by returning 0
        {
            return 0;
        }

        if (PlayersTurn.GetComponent<Player>().IsInJail()) //if player is still in jail, do not move
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
        LeaveJailCanvas.gameObject.SetActive(false);
        ManagePropertyCanvas.gameObject.SetActive(false);
        TradeCanvas.gameObject.SetActive(false);
        TradeDisplayCanvas.gameObject.SetActive(false);
        SelectPlayerCanvas.gameObject.SetActive(false);
        SelectGiveCanvas.gameObject.SetActive(false);
        SelectWantCanvas.gameObject.SetActive(false);
        ResolveCardCanvas.gameObject.SetActive(false);

        NumDoubles = 0;
        Again = false;

        if (Player.GetComponent<Player>().IsInJail())
        {
            //ask if player wants to leave jail
            LeaveJailCanvas.gameObject.SetActive(true);

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

        ChargePlayerCanvas.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Make Payment";

        MakePaymentFrom = PlayerFrom;
        MakePaymentTo = PlayerTo;
        MakePaymentAmount = Amount;
    }

    public void ChargePlayerToBank(GameObject PlayerFrom, int Amount, string Message)
    {
        ChargePlayerCanvas.gameObject.SetActive(true);

        ChargePlayerCanvas.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Message;

        ChargePlayerCanvas.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "Make Payment";

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

    public void LeaveJailPay()
    {
        PlayersTurn.GetComponent<Player>().GetOutOfJail();
        LeaveJailCanvas.gameObject.SetActive(false);
        Again = true; //allow player to move after getting charged

        ChargePlayerToBank(PlayersTurn, 50, PlayersTurn.GetComponent<Player>().Name + " owes 50 for getting out of jail");
    }
    public void StayInJail()
    {
        if (PlayersTurn.GetComponent<Player>().GetJailTime() <= 3)
        {
            RollCanvas.gameObject.SetActive(true);
            LeaveJailCanvas.gameObject.SetActive(false);
        }
    }

    public void LeaveJailRoll()
    {
        PlayersTurn.GetComponent<Player>().GetOutOfJail();
    }

    public int GetRecentRoll()
    {
        return MostRecentRoll;
    }

    public void OpenTradeMenu()
    {
        TradeCanvas.gameObject.SetActive(true);
        EndOfTurnCanvas.gameObject.SetActive(false);
        if (TradePartner == null)
        {
            TradeCanvas.transform.GetChild(0).gameObject.SetActive(true);
            TradeCanvas.transform.GetChild(1).gameObject.SetActive(false);
            TradeCanvas.transform.GetChild(2).gameObject.SetActive(false);
            TradeCanvas.transform.GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            TradeCanvas.transform.GetChild(0).gameObject.SetActive(false);
            TradeCanvas.transform.GetChild(1).gameObject.SetActive(true);
            TradeCanvas.transform.GetChild(2).gameObject.SetActive(true);
            TradeCanvas.transform.GetChild(3).gameObject.SetActive(true);
        }
    }

    public void CloseTradeMenu()
    {
        TradeCanvas.gameObject.SetActive(false);
        EndOfTurnCanvas.gameObject.SetActive(true);
        TradePartner = null;
        GiveList.Clear();
        WantList.Clear();
        GiveMoney = 0;
        WantMoney = 0;
    }

    public void OpenSelectPlayerMenu()
    {
        SelectPlayerCanvas.gameObject.SetActive(true);
        for (int i = 0; i < SelectPlayerCanvas.transform.childCount; i++)
        {
            if (i < Players.transform.childCount)
            {
                SelectPlayerCanvas.transform.GetChild(i).gameObject.SetActive(true);

                SelectPlayerCanvas.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = Players.transform.GetChild(i).GetComponent<Player>().Name;

                if (Players.transform.GetChild(i).GetComponent<Player>().Name == PlayersTurn.GetComponent<Player>().Name)
                {
                    SelectPlayerCanvas.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else
            {
                SelectPlayerCanvas.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        TradeCanvas.gameObject.SetActive(false);
    }

    public void OpenSelectGiveMenu()
    {
        SelectGiveCanvas.gameObject.SetActive(true);
        TradeCanvas.gameObject.SetActive(false);

        for (int i = 0; i < 28; i++)
        {
            if (SelectGiveCanvas.transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<DisplayPiece>().Tile.GetComponent<Tile>().GetOwner() == PlayersTurn)
            {
                SelectGiveCanvas.transform.GetChild(0).GetChild(3).GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                SelectGiveCanvas.transform.GetChild(0).GetChild(3).GetChild(i).gameObject.SetActive(false);
            }
        }
        SelectGiveCanvas.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = PlayersTurn.GetComponent<Player>().Name;
        SelectGiveCanvas.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "Money: " + PlayersTurn.GetComponent<Player>().GetMoney().ToString();
    }
    public void CloseSelectGiveMenu()
    {
        SelectGiveCanvas.gameObject.SetActive(false);
        OpenTradeMenu();
    }
    public void OpenSelectWantMenu()
    {
        SelectWantCanvas.gameObject.SetActive(true);
        TradeCanvas.gameObject.SetActive(false);

        for (int i = 0; i < 28; i++)
        {
            if (SelectWantCanvas.transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<DisplayPiece>().Tile.GetComponent<Tile>().GetOwner() == TradePartner)
            {
                SelectWantCanvas.transform.GetChild(0).GetChild(3).GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                SelectWantCanvas.transform.GetChild(0).GetChild(3).GetChild(i).gameObject.SetActive(false);
            }
        }
        SelectWantCanvas.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = TradePartner.GetComponent<Player>().Name;
        SelectWantCanvas.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "Money: " + TradePartner.GetComponent<Player>().GetMoney().ToString();
    }
    public void CloseSelectWantMenu()
    {
        SelectWantCanvas.gameObject.SetActive(false);
        OpenTradeMenu();
    }

    public void AddToGiveList(GameObject Tile)
    {
        GiveList.Add(Tile);
    }
    public void AddToWantList(GameObject Tile)
    {
        WantList.Add(Tile);
    }

    public void AddGiveMoney(int Amount)
    {
        if (PlayersTurn.GetComponent<Player>().GetMoney() >= GiveMoney + Amount)
        {
            GiveMoney += Amount;
        }
    }

    public void RemoveGiveMoney(int Amount)
    {
        if (GiveMoney >= Amount)
        {
            GiveMoney -= Amount;
        }
    }
    public void AddWantMoney(int Amount)
    {
        if (TradePartner.GetComponent<Player>().GetMoney() >= GiveMoney + Amount)
        {
            WantMoney += Amount;
        }
    }

    public void RemoveWantMoney(int Amount)
    {
        if (WantMoney >= Amount)
        {
            WantMoney -= Amount;
        }
    }

    public void SelectPlayerForTrade(int Player)
    {
        TradePartner = Players.transform.GetChild(Player).gameObject;
        SelectPlayerCanvas.gameObject.SetActive(false);
        OpenTradeMenu();
    }

    public void OpenBuyPropertyMenu(GameObject Property)
    {
        BuyPropertyCanvas.gameObject.SetActive(true);
        DisplayPropertyMenu(BuyPropertyCanvas, Property);
        CurrentProperty = Property;
    }

    public void ConfirmTrade()
    {

        PlayersTurn.GetComponent<Player>().AddMoney(WantMoney);
        PlayersTurn.GetComponent<Player>().TakeMoney(GiveMoney);
        TradePartner.GetComponent<Player>().TakeMoney(WantMoney);
        TradePartner.GetComponent<Player>().AddMoney(GiveMoney);

        GiveMoney = 0;
        WantMoney = 0;

        for (int i = 0; i < 28; i++)
        {
            if (GiveList.Contains(SelectGiveCanvas.transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<DisplayPiece>().Tile.gameObject) && TradePartner != null)
            {
                SelectGiveCanvas.transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<DisplayPiece>().Tile.gameObject.GetComponent<Tile>().SetOwner(TradePartner);
            }

            if (WantList.Contains(SelectGiveCanvas.transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<DisplayPiece>().Tile.gameObject) && TradePartner != null)
            {
                SelectGiveCanvas.transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<DisplayPiece>().Tile.gameObject.GetComponent<Tile>().SetOwner(PlayersTurn);
            }
        }
        CloseTradeMenu();
    }

    public void OpenManagePropertyMenu(GameObject Property)
    {
        if (BuyPropertyCanvas.gameObject.activeInHierarchy || ResolveCardCanvas.gameObject.activeInHierarchy || TradeDisplayCanvas.gameObject.activeInHierarchy)
        {
            return; //can not manage properties while doing other things
        }

        ManagePropertyCanvas.gameObject.SetActive(true);
        DisplayPropertyMenu(ManagePropertyCanvas, Property);
        CurrentProperty = Property;

        switch (Property.GetComponent<Tile>().Type)
        {
            case Tile.Types.Property:
                ManagePropertyCanvas.transform.GetChild(1).gameObject.SetActive(true);
                ManagePropertyCanvas.transform.GetChild(2).gameObject.SetActive(true);

                break;
            case Tile.Types.Railroad:
                ManagePropertyCanvas.transform.GetChild(1).gameObject.SetActive(false);
                ManagePropertyCanvas.transform.GetChild(2).gameObject.SetActive(false);
                break;
            case Tile.Types.Utility:
                ManagePropertyCanvas.transform.GetChild(1).gameObject.SetActive(false);
                ManagePropertyCanvas.transform.GetChild(2).gameObject.SetActive(false);
                break;
        }
    }

    public void ClosePropertyMenu()
    {
        ManagePropertyCanvas.gameObject.SetActive(false);
    }

    public void DisplayPropertyMenu(Canvas Can, GameObject Property)
    {
        switch (Property.GetComponent<Tile>().Type)
        {
            case Tile.Types.Property:

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

                Can.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = TileColor;

                Can.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = "Rent: " + Property.GetComponent<Tile>().RentPrice.ToString();

                string HousesText =
                    "1 House:           " + Property.GetComponent<Tile>().HousePrices[0].ToString() +
                    "\n2 House:           " + Property.GetComponent<Tile>().HousePrices[1].ToString() +
                    "\n3 House:           " + Property.GetComponent<Tile>().HousePrices[2].ToString() +
                    "\n4 House:           " + Property.GetComponent<Tile>().HousePrices[3].ToString() +
                    "\nHotel:               " + Property.GetComponent<Tile>().HotelPrice.ToString()
                    ;

                Can.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = HousesText;

                break;

            case Tile.Types.Railroad:
                Can.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Vector4(.5f, .5f, .5f, 1.0f);

                Can.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = " ";

                string TrainsText =
                    "1 Train:           " + Property.GetComponent<Tile>().HousePrices[0].ToString() +
                    "\n2 Train:           " + Property.GetComponent<Tile>().HousePrices[1].ToString() +
                    "\n3 Train:           " + Property.GetComponent<Tile>().HousePrices[2].ToString() +
                    "\n4 Train:           " + Property.GetComponent<Tile>().HousePrices[3].ToString()
                    ;

                Can.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = TrainsText;

                break;
            case Tile.Types.Utility:
                Can.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Vector4(.5f, .5f, .5f, 1.0f);

                Can.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = " ";

                string UtilityText =
                    "1 Utility:" +
                    "\n         4x Dice Roll" +
                    "\n2 Utility:" +
                    "\n         10x Dice Roll"
                    ;

                Can.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = UtilityText;
                break;
        }

        Can.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = Property.GetComponent<Tile>().Name;

        Can.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<Text>().text = "Purchase: " + Property.GetComponent<Tile>().PurchasePrice.ToString();

        CurrentProperty = Property;
    }

    public void BuyHouse()
    {
        CurrentProperty.GetComponent<Tile>().BuyHouse();
    }
    public void SellHouse()
    {
        CurrentProperty.GetComponent<Tile>().SellHouse();
    }
    public void Mortgage()
    {
        CurrentProperty.GetComponent<Tile>().Mortgage();
    }
    public void Unmortgage()
    {
        CurrentProperty.GetComponent<Tile>().Unmortgage();
    }

    public void GoBankrupt()
    {
        if (MakePaymentTo == null) //bankrupt to bank
        {
            MakePaymentFrom.GetComponent<Player>().GoBankruptToBank();
        }
        else //bankrupt to player they are paying
        {
            MakePaymentFrom.GetComponent<Player>().GoBankruptToPlayer(MakePaymentTo);
        }
    }

    public void ResolveCard(int DeckNum)
    {
        CurrentDeck = DeckNum;
        GameObject ResolveCard = null;

        ResolveCard = Decks.transform.GetChild(CurrentDeck).GetChild(0).gameObject;

        ResolveCardCanvas.gameObject.SetActive(true);
        ResolveCardCanvas.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = ResolveCard.GetComponent<Card>().GetMessage();
    }

    public void ResolveCardEffect()
    {
        GameObject ResolveCard = null;

        ResolveCard = Decks.transform.GetChild(CurrentDeck).GetChild(0).gameObject;

        if (ResolveCard == null)
        {
            print("Error");
            return;
        }

        ResolveCard.GetComponent<Card>().Effect(PlayersTurn);
        ResolveCardCanvas.gameObject.SetActive(false);
    }

}
