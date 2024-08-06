using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class huggingFaceStock : MonoBehaviour
{


    string[] begginers = { "https://sanaomerunity-stockfish-1.hf.space/run/predict", "https://sanaomerunity-stockfish-2.hf.space/run/predict", "https://sanaomerunity-stockfish-3.hf.space/run/predict", "https://sanaomerunity-stockfish-4.hf.space/run/predict", "https://sanaomerunity-stockfish-5.hf.space/run/predict" };

    string[] Intermidiates = { "https://sanaomerunity-stockfish-6.hf.space/run/predict", "https://sanaomerunity-stockfish-7.hf.space/run/predict", "https://sanaomerunity-stockfish-8.hf.space/run/predict", "https://sanaomerunity-stockfish-9.hf.space/run/predict", "https://sanaomerunity-stockfish-10.hf.space/run/predict", "https://sanaomerunity-stockfish-11.hf.space/run/predict" };

    string[] advanced = { "https://sanaomerunity-stockfish-12.hf.space/run/predict", "https://sanaomerunity-stockfish-13.hf.space/run/predict", "https://sanaomerunity-stockfish-14.hf.space/run/predict", "https://sanaomerunity-stockfish-15.hf.space/run/predict", "https://sanaomerunity-stockfish-16.hf.space/run/predict", "https://sanaomerunity-stockfish-17.hf.space/run/predict", "https://sanaomerunity-stockfish-18.hf.space/run/predict", "https://sanaomerunity-stockfish-19.hf.space/run/predict", "https://sanaomerunity-stockfish-20.hf.space/run/predict" };

    public string stockData;

    public string skill;

    public static huggingFaceStock Instance;
    
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
 
        switch (PassData.BotLeveling)
        {
            case <=1 :
                skill = "https://sanaomerunity-stockfish-1.hf.space/run/predict";
                break;

            case 2:
                skill = "https://sanaomerunity-stockfish-2.hf.space/run/predict";
                break;

            case 3:
                skill = "https://sanaomerunity-stockfish-3.hf.space/run/predict";
                break;


            case 4:
                skill = "https://sanaomerunity-stockfish-4.hf.space/run/predict";
                break;

            case 5:
                skill = "https://sanaomerunity-stockfish-5.hf.space/run/predict";
                break;

            case 6:
                skill = "https://sanaomerunity-stockfish-6.hf.space/run/predict";
                break;

            case 7:
                skill = "https://sanaomerunity-stockfish-7.hf.space/run/predict";
                break;

            case 8:
                skill = "https://sanaomerunity-stockfish-8.hf.space/run/predict";
                break;

            case 9:
                skill = "https://sanaomerunity-stockfish-9.hf.space/run/predict";
                break;

            case 10:
                skill = "https://sanaomerunity-stockfish-10.hf.space/run/predict";
                break;


            case 11:
                skill = "https://sanaomerunity-stockfish-11.hf.space/run/predict";
                break;


            case 12:
                skill = "https://sanaomerunity-stockfish-12.hf.space/run/predict";
                break;


            case 13:
                skill = "https://sanaomerunity-stockfish-13.hf.space/run/predict";
                break;


            case 14:
                skill = "https://sanaomerunity-stockfish-14.hf.space/run/predict";
                break;


            case 15:
                skill = "https://sanaomerunity-stockfish-15.hf.space/run/predict";
                break;


            case 16:
                skill = "https://sanaomerunity-stockfish-16.hf.space/run/predict";
                break;


            case 17:
                skill = "https://sanaomerunity-stockfish-17.hf.space/run/predict";
                break;

            case 18:
                skill = "https://sanaomerunity-stockfish-18.hf.space/run/predict";
                break;

            case 19:
                skill = "https://sanaomerunity-stockfish-19.hf.space/run/predict";
                break;


            case >= 20:
                skill = "https://sanaomerunity-stockfish-20.hf.space/run/predict";
                break;
        }

    }

    public void FenToStock(string fen)
    {
        Debug.Log("fen  " + skill);

        StartCoroutine(ProcessRequest(skill, fen));

    }




    private IEnumerator ProcessRequest(string uri, string fen)
    {
        yield return new WaitForSeconds(Random.Range(2, 5));
        ChessFen f = new ChessFen { data = new List<string>() { fen } };
        Debug.Log("f " + f);
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
        }
        request.Dispose();

    }

    public void WinGame()
    {
        AIGameResult.Instance.AIWinnerResult();
    }

}
public class ChessFen
{
    public List<string> data;


}
