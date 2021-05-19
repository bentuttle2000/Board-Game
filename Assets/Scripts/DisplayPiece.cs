using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPiece : MonoBehaviour
{
    public GameObject Tile;

    public void ManageProperty()
    {
        Tile.GetComponent<Tile>().ManageProperty();
    }
}
