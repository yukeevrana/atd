using UnityEngine;
using System.IO;
using System;

public class FileLogger : MonoBehaviour
{
    private string filePath;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "game_log.txt");
        WriteToFile("\n--- Session Started: " + DateTime.Now + " ---\n");
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Время [Тип] Сообщение
        string entry = $"[{DateTime.Now:HH:mm:ss}] [{type}] {logString}";
        
        if (type == LogType.Exception || type == LogType.Error)
        {
            entry += "\n" + stackTrace;
        }

        WriteToFile(entry);
    }

    void WriteToFile(string message)
    {
        try
        {
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(message);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write to log file: " + e.Message);
        }
    }
}