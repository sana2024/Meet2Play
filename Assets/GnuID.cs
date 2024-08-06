using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class GnuID : MonoBehaviour
{
    //____________________ MATCH ID _______________________
    //turning the dice numbers to binary substrings
    string Dice1ToBinary = "";
    string Dice2ToBinary = "";

    char[] Dice1ToArray;
    char[] Dice2ToArray;

    string ReversedDice1 = "";
    string ReversedDice2 = "";

    string BothDices = "";
    string MatchIDBigEndian = "";

    // creating 8 digit substring to turn them to little endian for match ID
    int substringLength = 8;
    char[] MatchSubstringToArray;
    string MatchIDSubstring = "";
    string ReversedMatchSubstring = "";
    List<string> MatchIDsubstrings = new List<string>();
    string MatchIDinBinary = "";
    string MatchIDtoBase64 = "";
    string MatchIDResult = "";
    string MatchIDSummed = "";

    //____________________ POSTION ID _______________________
    List<string> PosSubstrings = new List<string>();
    string PosSubstring = "";
    char[] PosSubstringArray;
    string ReversedPosSubstring;
    string PostionIDBinary = "";
    string PostionIDBase64 = "";

    string stockData;
    string BackgammonData = "https://sanaomerunity-backgammon.hf.space/run/predict";

    public static GnuID Instance;


    //results
    int pips;
    int source;
    int destination;
 
    // Start is called before the first frame update
    [Obsolete]
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }

      //  GenerateMove(4 , 5);
    }

    public void GenerateMove(string postionid ,int dice1, int dice2)
    {
        Debug.Log(postionid);
        string DataString = PositionID(postionid) + "}}" + MatchID(dice1, dice2);
        StartCoroutine(ProcessRequest(BackgammonData, DataString));
    }

    // Update is called once per frame
    void Update()
    {
 
 
    }

    public string MatchID(int dice1, int dice2)
    {
        //------------------ DICE1 -------------------
        Dice1ToBinary = Convert.ToString(dice1, 2);
        if (Dice1ToBinary.Length == 1)
        {
            Dice1ToBinary = "00" + Dice1ToBinary;
        }
        if (Dice1ToBinary.Length == 2)
        {
            Dice1ToBinary = "0" + Dice1ToBinary;
        }
         Dice1ToArray = Dice1ToBinary.ToCharArray();
         Array.Reverse(Dice1ToArray);
         ReversedDice1 = new string(Dice1ToArray);

        //------------------ DICE2 -------------------

        Dice2ToBinary = Convert.ToString(dice2, 2);
        if (Dice2ToBinary.Length == 1)
        {
            Dice2ToBinary = "00" + Dice2ToBinary;
        }
        if (Dice2ToBinary.Length == 2)
        {
            Dice2ToBinary = "0" + Dice2ToBinary;
        }
        Dice2ToArray = Dice2ToBinary.ToCharArray();
        Array.Reverse(Dice2ToArray);
        ReversedDice2 = new string(Dice2ToArray);




        BothDices = ReversedDice1 + ReversedDice2;

       // Debug.Log("summed string " + Input);

        MatchIDBigEndian = "100000101001000" + BothDices + "100100000000000010000000000000001000000000000000000";
  
        for (int i = 0; i < MatchIDBigEndian.Length; i += substringLength)
        {
            int length = Mathf.Min(substringLength, MatchIDBigEndian.Length - i);
            string substring = MatchIDBigEndian.Substring(i, length);
            MatchIDsubstrings.Add(substring);
        }

        // Print the substrings
        foreach (string substring in MatchIDsubstrings)
        {
            //Debug.Log(substring);
            MatchSubstringToArray = substring.ToCharArray();
            Array.Reverse(MatchSubstringToArray);
            ReversedMatchSubstring = new string(MatchSubstringToArray);
            MatchIDSummed = MatchIDSummed + ReversedMatchSubstring;
           // Debug.Log(reversedString);

        }

       // Debug.Log(result);

        byte[] bytes = new byte[MatchIDSummed.Length / 8];

        for (int i = 0; i < MatchIDSummed.Length; i += 8)
        {
            MatchIDinBinary = MatchIDSummed.Substring(i, 8);
            byte b = Convert.ToByte(MatchIDinBinary, 2);
            bytes[i / 8] = b;
        }

        MatchIDtoBase64 = Convert.ToBase64String(bytes);

        MatchIDResult = MatchIDtoBase64.Substring(0, MatchIDtoBase64.Length - 1);

        //Debug.Log(MatchIDResult + "A");
        return MatchIDResult +"A";

    }

    public string PositionID(string PositionString)
    {
 
        
        for (int i = 0; i < PositionString.Length; i += substringLength)
        {
            int length = Mathf.Min(substringLength, PositionString.Length - i);
            PosSubstring = PositionString.Substring(i, length);
            PosSubstrings.Add(PosSubstring);
        }

        // Print the substrings
        foreach (string substring in PosSubstrings)
        {
            PosSubstringArray = substring.ToCharArray();
            Array.Reverse(PosSubstringArray);
            ReversedPosSubstring = new string(PosSubstringArray);
            PostionIDBinary = PostionIDBinary + ReversedPosSubstring;

        }
 
        byte[] bytes = new byte[PostionIDBinary.Length / 8];

        for (int i = 0; i < PostionIDBinary.Length; i += 8)
        {
            string binaryByte = PostionIDBinary.Substring(i, 8);
            byte b = Convert.ToByte(binaryByte, 2);
            bytes[i / 8] = b;
        }

        PostionIDBase64 = Convert.ToBase64String(bytes);

        string PostionIDResult = PostionIDBase64.Substring(0, PostionIDBase64.Length - 2);

        Debug.Log(PostionIDResult);
        return PostionIDResult;

    }

    public void ResetMatchID()
    {
        MatchIDSubstring = "";
        ReversedMatchSubstring = "";
        MatchIDsubstrings.Clear();
        MatchIDinBinary = "";
        MatchIDtoBase64 = "";
        MatchIDResult = "";
        MatchIDSummed = "";
    }

    public void ResetPostionID()
    {
        PosSubstrings.Clear();
        PosSubstring = "";
        ReversedPosSubstring = "";
        PostionIDBinary = "";
        PostionIDBase64 = "";
    }

    [Obsolete]
    private IEnumerator ProcessRequest(string uri, string fen)
    {
        MatchId f = new MatchId { data = new List<string>() { fen } };
        string postData = JsonUtility.ToJson(f);
        //   Debug.Log("PostData "+postData);
        //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(postData);
        UnityWebRequest request = UnityWebRequest.Put(uri, postData);
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");
        //{"data": ["rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"]}
        //Content-Type: application/json' -d '{"data": ["rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"]}
        yield return request.SendWebRequest();


        if (request.isNetworkError)
        {
            Debug.Log(" error request  " + request.error);
        }
        else
        {
            stockData = request.downloadHandler.text;
            Debug.Log("stock data " + stockData);

            
            if (stockData.Contains("null"))
            {
                Debug.Log("AI No move");
                BackgammonAI.Instance.doneButton.SetActive(false);
                BackgammonAI.Instance.endTurn();
            }
            else
            {
                string modifiedString = stockData.Remove(0, 11);
                int index = modifiedString.IndexOf(")\"],");
                string trimmedString = modifiedString.Substring(0, index);
                string pattern = @"Move\([^)]+\)";
                MatchCollection matches = Regex.Matches(trimmedString, pattern);

                foreach (Match match in matches)
                {
                    yield return new WaitForSeconds(2f);
                    string substring = match.Value;
                    Source_Destination(substring);
                }

                BackgammonAI.Instance.endTurn();


            }
        }
            request.Dispose();
        
    }


    public class MatchId
    {
        public List<string> data;


    }

    public void Source_Destination(string ResponseString)
    {
        Debug.Log("move " + ResponseString);
        string pattern = @"pips=(\d+).*source=(\d+|None).*destination=(\d+|None)";
        Match match = Regex.Match(ResponseString, pattern);

        pips = int.Parse(match.Groups[1].Value);
        //destination = int.Parse(match.Groups[3].Value);

        if (match.Groups[2].Value != "None")
        {
            source = int.Parse(match.Groups[2].Value);
        }
        else
        {
            source = 26;
        }


        if (match.Groups[3].Value != "None")
        {
            destination = int.Parse(match.Groups[3].Value);
        }
        else
        {
            destination = 27;
        }

        BackgammonAI.Instance.curTile = BackgammonAI.Instance.tiles[source];
        BackgammonAI.Instance.makeMove(destination);
 
    }
}
