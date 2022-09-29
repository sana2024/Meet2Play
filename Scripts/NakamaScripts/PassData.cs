using System.Collections;
using System.Collections.Generic;
using Nakama;

public class PassData
{
    //-----------
    //user ID
    //-----------

    public static ISocket isocket;
    public static IMatch Match;
    public static IClient iClient;
    public static ISession isession;


    //-----------
    //user ID
    //-----------

    public static string UserIDState;
    public static string OtherUserId;
    public static string otherUsername;
    public static string hostPresence;
    public static string SecondPresence;



    //-----------
    //user profile
    //-----------

    public static string Username;
    public static string MyURL;
    public static string OpponentURL;
    public static int DiceId;
    public static string BoardType;
    public static int WalletMoney;
    public static int BoardPrice;
    public static int betAmount;
    public static int wins;
    public static int losses;
    public static int level;
    public static string version;
    public static int JoinedPlayers;


    //rules

    public static int DoubleValue;


    //leaderboard
    public static string ImageURL;

}
