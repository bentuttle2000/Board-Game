using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    private int NumDoubles;

    public int Roll()
    {
        int Dice1 = Random.Range(1, 7); //returns random int 1-6 (the 7 is not inclusive)
        int Dice2 = Random.Range(1, 7);
        print(Dice1);
        print(Dice2);
        if (Dice1 == Dice2)
        {
            print("Doubles");
            NumDoubles++;
            
        }
        if (NumDoubles == 3) //If adding option for no max doubles add (&& DoubleCapEnabled) 
        {
            return 0;
        }
        return Dice1 + Dice2;
    }

    public void NewTurn()
    {
        NumDoubles = 0;
    }
}
