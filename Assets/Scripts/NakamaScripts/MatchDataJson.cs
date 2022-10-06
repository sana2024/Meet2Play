using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama.TinyJson;

public class MatchDataJson
{
    public static string SetPeicePos(int PeiceID , Transform transform)
    {
        var values = new Dictionary<string, string>
        {
            { "PeiceID", PeiceID.ToString() },
            { "Pos_x", transform.position.x.ToString() },
            { "pos_y", transform.position.y.ToString() }
        };

        return values.ToJson();
    }

 
    public static string SetDicePos(Vector2 pos , int value1 , int value2)
    {
        var values = new Dictionary<string, string>
        {
            { "Pos_X", pos.x.ToString()},
            { "Pos_Y", pos.y.ToString()},
            { "Value1", value1.ToString()},
            { "Value2", value2.ToString()}

        };

        return values.ToJson();
    }

    public static string SetDiceSprite(int DiceId ,int Index)
    {
        var values = new Dictionary<string, string>
        {
            { "Dice_Id",  DiceId.ToString() },
            { "Dice_sprite_index", Index.ToString() }
        };

        return values.ToJson();
    }

    public static string SetDiceVisability(string visability)
    {
        var values = new Dictionary<string, string>
        {
            { "Dice_sprite_index", visability }
        };

        return values.ToJson();
    }

    public static string SetCurrentPlayer(string currentPlayer)
    {
        var values = new Dictionary<string, string>
        {
            { "Current_Player" , currentPlayer}
     
        };

        return values.ToJson();
    }

    //for adding and removes from the stack for Undo and hit actions
    public static string SetPieceStack(string peiceID,  string from, string to , string steps, string actionType , string Movetype)
    {
        var values = new Dictionary<string, string>
        {
            {"PeiceID" , peiceID },
            {"From" , from },
            {"To" , to },
            {"Steps" , steps },
            {"ActionType" , actionType },
            {"MoveType" , Movetype }

        };

        return values.ToJson();
    }


    public static string SetCameraBackground(bool SetBackground)
    {
        var values = new Dictionary<string, string>
        {
            { "Camera_Background" , SetBackground.ToString()}

        };

        return values.ToJson();
    }


    public static string SetTimer(int SetTime)
    {
        var values = new Dictionary<string, string>
        {
            { "Timer" , SetTime.ToString()}

        };

        return values.ToJson();
    }


    public static string SetLevel(string SetLevel)
    {
        var values = new Dictionary<string, string>
        {
            { "Level" , SetLevel.ToString()}

        };

        return values.ToJson();
    }


    public static string SetDouble(string Double)
    {
        var values = new Dictionary<string, string>
        {
            { "Double" , Double.ToString()}

        };

        return values.ToJson();
    }

    public static string SetAccept(string Accept)
    {
        var values = new Dictionary<string, string>
        {
            { "Accept" , Accept.ToString()}

        };

        return values.ToJson();
    }

    public static string SetReject(string Reject)
    {
        var values = new Dictionary<string, string>
        {
            { "Reject" , Reject.ToString()}

        };

        return values.ToJson();
    }

    public static string SetLeaveMatch(string Leave)
    {
        var values = new Dictionary<string, string>
        {
            { "Leave" , Leave.ToString()}

        };

        return values.ToJson();
    }


    public static string SetDiceCanvar(string diceColor, string diceValue1 , string diceValue2)
    {
        var values = new Dictionary<string, string>
        {
            { "DiceColor" , diceColor.ToString()},
            { "DiceValue1" , diceValue1.ToString()},
            { "DiceValue2" , diceValue2.ToString()}

        };

        return values.ToJson();
    }



}
