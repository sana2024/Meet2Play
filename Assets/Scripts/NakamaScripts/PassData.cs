using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;
using System.Linq;
using UnityEngine.Networking;
using System;


public class PassData
{
 

    //-----------
    //user ID
    //-----------

    public static ISocket isocket;
    public static IMatch Match;
    public static IClient iClient;
    public static ISession isession;


    public static IUserPresence MyPresense;
    public static IUserPresence OtherPresence;

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
    public static string EditAvatar;
    public static Texture MyURLTexture;
    public static string OpponentURL;
    public static int DiceId;
    public static string BoardType;
    public static int WalletMoney;
    public static int BoardPrice;
    public static int AbsuluteBoardPrice;
    public static int betAmount;
    public static int wins;
    public static int losses;
    public static int level;
    public static string version;
    public static int JoinedPlayers;
    public static string RecivedLevel;
    public static string FacebookID;
    public static string DateAndTime;
    public static Texture OtherPlayerTexture;




    //rules

    public static int DoubleValue;


    //leaderboard
    public static string ImageURL;


    //waiting players
    public static string Board;
    public static string Queue;
    public static int ReadPermission;

    //Next Round 
    public static bool IsFirstRound;
    public static Player PlayerWonRound;


    //Chess Datas
    public static string ThemeColor;
    public static string PlayerDesc;
    public static int ChessTimer;
    public static string SkillLevel;
    public static int ChessLevel;
    public static int CurrentXP;
    public static int RequiredXP;
    public static int ChessELO;
    public static int TotalOpponentsElo;
    public static int ChessWins;
    public static int ChessLooses;
    public static int ChessDraws;
    public static int AIRating;
    public static int AIlevel;
    public static int BotLeveling;

    public static int AddedXP;
    public static string ChallangeType;


    // socials
    public static string DiscordUrl;
    public static float BonusTimer;


    // match count
    public static int matchCount;


}