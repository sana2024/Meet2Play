using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    int wins;
    int level;


    public void Update()
    {
        CheckWins();

        PassData.level = level;
    }



    public void CheckWins()
    {

        int wins = PassData.wins;

        if (wins >= 0)
        {
            level = 1;
        }
        if (wins >= 2)
        {
            level = 2;
        }
        if (wins >= 5)
        {
            level = 3;
        }
        if (wins >= 10)
        {
            level = 4;
        }
        if (wins >= 15)
        {
            level = 5;
        }
        if (wins >= 20)
        {
            level = 6;
        }
        if (wins >= 25)
        {
            level = 7;
        }
        if (wins >= 30)
        {
            level = 8;
        }
        if (wins >= 35)
        {
            level = 9;
        }
        if (wins >= 40)
        {
            level = 10;
        }

 
    }

  

}
