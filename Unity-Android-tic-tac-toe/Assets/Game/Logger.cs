using UnityEngine;
using System.Collections;

public class Logger : MonoBehaviour
{
    public static LogTheme theAcceptedTheme = LogTheme.all;
    public LogTheme acceptedTheme;

    private void Awake()
    {
        theAcceptedTheme = acceptedTheme;
    }

    public static void Log(string message, LogTheme theme = LogTheme.all, LogMode mode = LogMode.message)
    {
        if (theAcceptedTheme != LogTheme.all && theAcceptedTheme != theme) return;
        Debug.Log(message);
    }
}

public enum LogTheme
{
    all = 0,
    gameEvent,
    gameLoop,
    gameDebug,
    server,
    client,
}

public enum LogMode
{
    message = 0,
    warning = 1,
    error = 2,
}
