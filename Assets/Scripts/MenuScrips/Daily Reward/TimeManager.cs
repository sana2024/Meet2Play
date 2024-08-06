using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static DateTime CurrentTime { get; private set; }

    private const string timeApiUrl = "https://worldtimeapi.org/api/timezone/Etc/UTC";

    void Start()
    {
        StartCoroutine(GetTimeFromServer());
    }

    private IEnumerator GetTimeFromServer()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(timeApiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = request.downloadHandler.text;
                var dateTime = JsonUtility.FromJson<WorldTimeApiResponse>(jsonResult);
                CurrentTime = DateTime.Parse(dateTime.utc_datetime);
            }
            else
            {
                Debug.LogError("Failed to fetch time from server");
                // Fallback to system time if the server request fails
                CurrentTime = DateTime.UtcNow;

            }
        }
    }

    [Serializable]
    private class WorldTimeApiResponse
    {
        public string utc_datetime;
    }
}
