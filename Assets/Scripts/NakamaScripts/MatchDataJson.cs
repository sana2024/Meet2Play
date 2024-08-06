using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama.TinyJson;

public class MatchDataJson
{
    public static string SetPeicePos(int PeiceID, Transform transform)
    {
        var values = new Dictionary<string, string>
        {
            { "PeiceID", PeiceID.ToString() },
            { "Pos_x", transform.position.x.ToString() },
            { "pos_y", transform.position.y.ToString() }
        };
        return values.ToJson();
    }


    public static string SetDicePos(Vector2 pos, int value1, int value2)
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

    public static string SetDiceSprite(int DiceId, int Index)
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
    public static string SetPieceStack(string peiceID, string from, string to, string steps, string actionType, string Movetype)
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


    public static string SetDiceCanvar(string diceColor, string diceValue1, string diceValue2)
    {
        var values = new Dictionary<string, string>
        {
            { "DiceColor" , diceColor.ToString()},
            { "DiceValue1" , diceValue1.ToString()},
            { "DiceValue2" , diceValue2.ToString()}

        };

        return values.ToJson();
    }

    public static string SetRematch(string RematchState)
    {
        var values = new Dictionary<string, string>
        {
            { "RequsetRematch" , RematchState.ToString()}

        };


        return values.ToJson();
    }


public static string SetChessTurn(string Turn)
{
    var values = new Dictionary<string, string>
        {
            { "Turn" , Turn.ToString()}

        };

    return values.ToJson();
}


public static string SetChessPostion(string piece, string PosX, string PosY)
{
    var values = new Dictionary<string, string>
        {
            { "CurrentlyDragging" , piece},
            { "PosX" ,  PosX},
            { "PosY" ,  PosY}

        };

    return values.ToJson();
}


public static string SetChessHit(int x, int y)
{
    var values = new Dictionary<string, string>
        {
            { "x",  x.ToString() },
            { "y",  y.ToString() },



        };

    return values.ToJson();
}



public static string SetPromotion(string x, string y, string team, string pieceType)
{
    var values = new Dictionary<string, string>
        {
            { "LastMove_x",  x},
            { "LastMove_Y",  y},
            { "Team",  team},
            { "Type",  pieceType},



        };

    return values.ToJson();
}


public static string SetHighLight(string x0, string y0, string x1, string y1)
{
    var values = new Dictionary<string, string>
        {
            { "x0",  x0},
            { "y0",  y0},
            { "x1",  x1},
            { "y1",  y1}
        };

    return values.ToJson();
}


public static string SetCheck(int x, int y)
{
    var values = new Dictionary<string, string>
        {
            { "Tilex",  x.ToString()},
            { "Tiley",  y.ToString()}

        };

    return values.ToJson();
}


public static string SetCheckmate(string winner)
{
    var values = new Dictionary<string, string>
        {
            { "Winner",  winner}


        };

    return values.ToJson();
}

public static string SetNotation(string Notation)
{
    var values = new Dictionary<string, string>
        {
            { "Notation",  Notation},

        };

    return values.ToJson();
}

    public static string SetDraw(string DrawType)
    {
        var values = new Dictionary<string, string>
        {
            { "Draw" , DrawType}

        };


        return values.ToJson();
    }


    public static string SetCastling(int OldX , int OldY , int NewX , int NewY)
    {
        var values = new Dictionary<string, string>
        {
            { "OldX" , OldX.ToString()},
            { "OldY" , OldY.ToString()},
            { "NewX" , NewX.ToString()},
            { "NewY" , NewY.ToString()},

        };


        return values.ToJson();
    }

    public static string SetOfferDraw(string Draw)
    {
        var values = new Dictionary<string, string>
        {
            { "DrawOffer" , Draw}

        };


        return values.ToJson();
    }

    public static string SetAndroidBackground(string Backgroundstate)
    {
        var values = new Dictionary<string, string>
        {
            { "backgroundState" , Backgroundstate}

        };


        return values.ToJson();
    }


}