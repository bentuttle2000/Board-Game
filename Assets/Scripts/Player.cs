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

    private void Start()
    {
        Location = GameObject.FindGameObjectWithTag("Go");

        Board = GameObject.FindGameObjectWithTag("Board");

        Dice = GameObject.FindGameObjectWithTag("Dice");
    }

    public void Update()
    {
        transform.position = new Vector3(Location.transform.position.x, Location.transform.position.y, -1);

        //Move
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Move();
        }
        
    }

    public GameObject GetLocation()
    {
        return Location;
    }

    public void SetLocation(GameObject NewLocation)
    {
        Location = NewLocation;
    }

    public void Move()
    {

        int NumSpaces = Dice.GetComponent<Dice>().Roll();

        if (NumSpaces == 0) //Roll returns 0 on 3rd double
        {
            //go to jail
        }


        int NumTiles = Board.transform.childCount - 1;

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


    }

    IEnumerator Moving(float Sec, int CurTile, int NumTiles, int NewTile)
    {
        while (CurTile != NewTile)
        {
            yield return new WaitForSecondsRealtime(Sec);
            CurTile++;
            if (CurTile > NumTiles)
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
        AddMoney(200);
    }

}
