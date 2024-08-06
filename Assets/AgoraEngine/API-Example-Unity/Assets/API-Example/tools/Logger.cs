using UnityEngine;
using UnityEngine.UI;

public class Logger
{
 

    public Logger(Text text)
    {
       
    }

    public void UpdateLog(string logMessage, bool error = false)
    {
 
    }

    public bool DebugAssert(bool condition, string message)
    {
        if (!condition)
        {
            UpdateLog("<color=red>" + message + "</color>", error: true);
            return false;
        }
        Debug.Assert(condition, message);
        return true;
    }
}