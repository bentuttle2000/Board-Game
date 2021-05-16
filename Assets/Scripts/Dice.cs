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

        StartCoroutine(Wait(.2f, Dice1, Dice2));

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

    IEnumerator Wait(float Sec, int D1, int D2)
    {
        transform.GetChild(0).GetComponent<Animator>().SetInteger("Dice", 0);
        yield return new WaitForSecondsRealtime(.1f);
        transform.GetChild(1).GetComponent<Animator>().SetInteger("Dice", 0);

        yield return new WaitForSecondsRealtime(Sec);
        transform.GetChild(0).GetComponent<Animator>().SetInteger("Dice", D1);
        transform.GetChild(1).GetComponent<Animator>().SetInteger("Dice", D2);
    }
}
