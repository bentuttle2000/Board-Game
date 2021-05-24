using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string Name;

    private GameObject Location;
    private GameObject Board;

    private int Money = 1500;

    private GameObject Dice;

    private int GoMoney = 200;

    private bool IsMoving = false;

    private GameObject Players;

    private bool InJail = false;

    private int TurnsInJail = 0;

    private GameObject Decks = null;

    private void Start()
    {
        Location = GameObject.FindGameObjectWithTag("Go");

        Board = GameObject.FindGameObjectWithTag("Board");

        Dice = GameObject.FindGameObjectWithTag("Dice");

        Players = GameObject.FindGameObjectWithTag("Players");

        Decks = GameObject.FindGameObjectWithTag("Decks");

        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Update()
    {
        UpdatePosition();

        UpdateDisplay();
    }

    public void UpdatePosition()
    {
        float DX = 0, DY = 0;
        int NumPlayers = Players.transform.childCount;
        for (int i = 0; i < NumPlayers; i++)
        {
            if (Players.transform.GetChild(i).GetComponent<Player>().Name == Name)
            {
                switch (i)
                {
                    case 0:
                        DX = 0;
                        DY = .1f;
                        break;
                    case 1:
                        DX = 0;
                        DY = -.1f;
                        break;
                    case 2:
                        DX = .1f;
                        DY = 0;
                        break;
                    case 3:
                        DX = -.1f;
                        DY = 0;
                        break;
                    case 4:
                        DX = .1f;
                        DY = .1f;
                        break;
                    case 5:
                        DX = -.1f;
                        DY = -.1f;
                        break;
                }

            }
        }

        transform.position = new Vector3(Location.transform.position.x + DX, Location.transform.position.y + DY, -1);

    }

    public void UpdateDisplay()
    {
        transform.GetChild(0).GetChild(1).GetComponent<Text>().text = Name;
        transform.GetChild(0).GetChild(2).GetComponent<Text>().text = "Money: " + Money;
        
        for (int i = 0; i < 28; i++)
        {
            if (transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<DisplayPiece>().Tile.GetComponent<Tile>().GetOwner() == this.gameObject)
            { 
                transform.GetChild(0).GetChild(3).GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(0).GetChild(3).GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public GameObject GetLocation()
    {
        return Location;
    }

    public int GetLocationAsInt()
    {
        int CurTile = -1;
        for (int i = 0; i < Board.transform.childCount && CurTile < 0; i++)
        {
            if (Board.transform.GetChild(i).gameObject == Location.gameObject)
            {
                CurTile = i;
            }
        }
        return CurTile;
    }

    public void SetLocation(GameObject NewLocation)
    {
        Location = NewLocation;
    }

    public void StartTurn()
    {
        if (IsInJail())
        {
            TurnsInJail++;
        }

        Dice.GetComponent<Dice>().NewTurn(gameObject);
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void Move()
    {
        if (IsMoving)
        {
            return; //do not try to move if already moving
        }

        IsMoving = true;

        int NumSpaces = Dice.GetComponent<Dice>().Roll();

        if (NumSpaces == 0) //Roll returns 0 on 3rd double
        {
            //go to jail
            SendToJail();
            return;
        }

        int NumTiles = Board.transform.childCount;

        int CurTile = -1;

        for (int i = 0; i < NumTiles && CurTile < 0; i++)
        {
            if (Board.transform.GetChild(i).gameObject == Location.gameObject)
            {
                CurTile = i;
            }
        }

        int NewTile = CurTile + NumSpaces;

        if (NewTile >= NumTiles)
        {
            NewTile -= NumTiles;
        }

        StartCoroutine(Moving(0.2f, CurTile, NumTiles, NewTile));

        return;
    }

    public void MoveTo(GameObject NewLocation) 
    {
        int NumTiles = Board.transform.childCount;

        int CurTile = -1;

        int NewTile = -1;

        for (int i = 0; i < NumTiles && CurTile < 0; i++)
        {
            if (Board.transform.GetChild(i).gameObject == Location.gameObject)
            {
                CurTile = i;
            }
        }

        for (int i = 0; i < NumTiles && NewTile < 0; i++)
        {
            if (Board.transform.GetChild(i).gameObject == NewLocation.gameObject)
            {
                NewTile = i;
            }
        }

        if (NewTile >= NumTiles)
        {
            NewTile -= NumTiles;
        }

        StartCoroutine(Moving(.05f, CurTile, NumTiles, NewTile));

        return;
    }

    public void GoBackSpaces(int NumSpaces)
    {
        Location = Board.transform.GetChild(GetLocationAsInt() - NumSpaces).gameObject;
        LandedOn();
    }

    IEnumerator Moving(float Sec, int CurTile, int NumTiles, int NewTile)
    {
        while (CurTile != NewTile)
        {
            yield return new WaitForSecondsRealtime(Sec);
            CurTile++;
            if (CurTile >= NumTiles)
            {
                CurTile -= NumTiles;
                PassGo();
            }
            SetLocation(Board.transform.GetChild(CurTile).gameObject);
        }
        yield return new WaitForSecondsRealtime(Sec);
        LandedOn();    
    }

    public void LandedOn()
    {
        Location.GetComponent<Tile>().LandedOn(gameObject);
    }

    public void PostMove()
    {
        IsMoving = false;
        if (!Dice.GetComponent<Dice>().PlayAgain())
        {
            //move to end phase (aka enable menus for trading and buying houses)
            EndOfTurn();
        }
    }

    public void EndOfTurn()
    {
        IsMoving = false;
        Dice.GetComponent<Dice>().EndOfTurn();
    }

    public void EndTurn()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        int NumPlayers = Players.transform.childCount;
        for (int i = 0; i < NumPlayers; i++)
        {
            if (Players.transform.GetChild(i).GetComponent<Player>().Name == Name)
            {
                //set next players turn (loop around if last player) 
                int NextPlayer = i + 1;
                if (NextPlayer == NumPlayers)
                {
                    NextPlayer = 0;
                }
                Players.transform.GetChild(NextPlayer).GetComponent<Player>().StartTurn();
                return;
            }
        }
    }

    public void SendToJail()
    {
        GameObject Jail = GameObject.FindGameObjectWithTag("InJail");
        SetLocation(Jail);
        InJail = true;
        EndOfTurn();
    }

    public void GetOutOfJail()
    {
        GameObject Visiting = GameObject.FindGameObjectWithTag("VisitingJail");
        SetLocation(Visiting);
        InJail = false;
        ResetJailTime();
    }

    public bool IsInJail()
    {
        return InJail;
    }

    public void ResetJailTime()
    {
        TurnsInJail = 0;
    }

    public int GetJailTime()
    {
        return TurnsInJail;
    }

    public int GetMoney()
    {
        return Money;
    }

    public void AddMoney(int Amount)
    {
        Money += Amount;
    }

    public bool TakeMoney(int Amount)
    {
        if (Money >= Amount)
        {
            Money -= Amount;
            return true;
        }
        else
        {
            //player does not have enough money, must manage properties or go bankrupt
            //load menu for not enough money
            return false;
        }
    }
    public void ChargeMoney(int Amount)
    {
        if (TakeMoney(Amount))
        {
            PostMove(); 
        }
        
    }

    public void PassGo()
    {
        AddMoney(GoMoney);
    }

    public void GoBankruptToBank()
    {
        for (int i = 0; i < Board.transform.childCount; i++)
        {
            if (Board.transform.GetChild(i).GetComponent<Tile>().GetOwner() == this.gameObject)
            {
                Board.transform.GetChild(i).GetComponent<Tile>().TileReset();
            }
        }
        EndTurn();
        Destroy(gameObject);
    }

    public void GoBankruptToPlayer(GameObject Player)
    {
        for (int i = 0; i < Board.transform.childCount; i++)
        {
            if (Board.transform.GetChild(i).GetComponent<Tile>().GetOwner() == this.gameObject)
            {
                Board.transform.GetChild(i).GetComponent<Tile>().SetOwner(Player);
            }
        }
        Player.GetComponent<Player>().AddMoney(Money);
        EndTurn();
        Destroy(gameObject);
    }

    public void DrawCard(int Deck)
    {
        Decks.transform.GetChild(Deck).GetChild(0).GetComponent<Card>().DrawCard(gameObject);
    }

    public void PayHouseRepairs()
    {
        int Price = 0;
        for (int i = 0; i < 28; i++)
        {
            if (transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<DisplayPiece>().Tile.GetComponent<Tile>().GetOwner() == this.gameObject)
            {
                Price += transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<DisplayPiece>().Tile.GetComponent<Tile>().GetRepairCost();
            }
        }
        Dice.GetComponent<Dice>().ChargePlayerToBank(gameObject, Price, "Home Repair Cost: " + Price);
    }
}
