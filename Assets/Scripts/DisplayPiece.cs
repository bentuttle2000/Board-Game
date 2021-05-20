using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPiece : MonoBehaviour
{
    public GameObject Tile;

    private GameObject Dice;

    private void Start()
    {
        Dice = GameObject.FindGameObjectWithTag("Dice");
    }

    public void ManageProperty()
    {
        Tile.GetComponent<Tile>().ManageProperty();
    }

    public void AddToGiveList()
    {
        Dice.GetComponent<Dice>().AddToGiveList(Tile);
    }
}
