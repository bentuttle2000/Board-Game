using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    private void Start()
    {
        Location = GameObject.FindGameObjectWithTag("Go");

        Board = GameObject.FindGameObjectWithTag("Board");

        Dice = GameObject.FindGameObjectWithTag("Dice");

        Players = GameObject.FindGameObjectWithTag("Players");

        StartTurn();
    }

    public void Update()
    {
        UpdatePosition();
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

    public GameObject GetLocation()
    {
        return Location;
    }

    public void SetLocation(GameObject NewLocation)
    {
        Location = NewLocation;
    }

    public void StartTurn()
    {
        Dice.GetComponent<Dice>().NewTurn(gameObject);
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

        if (NewTile > NumTiles)
        {
            NewTile -= NumTiles;
        }

        StartCoroutine(Moving(.2f, CurTile, NumTiles, NewTile));

        return;
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
        PostMove();    
    }

    public void PostMove()
    {
        Location.GetComponent<Tile>().LandedOn(gameObject);
        IsMoving = false;
        if (!Dice.GetComponent<Dice>().PlayAgain())
        {
            //move to end phase (aka enable menus for trading and buying houses)
            EndOfTurn();
        }
    }

    public void EndOfTurn()
    {
        Dice.GetComponent<Dice>().EndOfTurn();
    }

    public void EndTurn()
    {
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

    public int GetMoney()
    {
        return Money;
    }

    public void AddMoney(int Amount)
    {
        Money += Amount;
    }

    public void TakeMoney(int Amount)
    {
        if (Money >= Amount)
        {
            Money -= Amount;
        }
    }

    public void PassGo()
    {
        AddMoney(GoMoney);
    }

}
