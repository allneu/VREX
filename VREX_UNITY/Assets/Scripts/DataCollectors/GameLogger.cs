using System;
using System.IO;
using UnityEngine;

namespace DataCollectors
{
    public class GameLogger
    {
        private static GameLogger _instance;
        private static StreamWriter _mainLogWriter;
        private static StreamWriter _timestampLogWriter;
        private static StreamWriter _conditionalLogWriter;
        private static StreamWriter _allLogWriter;
        private static PlayerData _playerData;
        public static bool isLoggerEnabled = true;

        private GameLogger()
        {
        }

        public static GameLogger instance
        {
            get
            {
                if (_instance == null) _instance = new GameLogger();
                return _instance;
            }
        }

        public void Initialize()
        {
            if (!isLoggerEnabled) return;

            if (_mainLogWriter == null && _timestampLogWriter == null && _conditionalLogWriter == null &&
                _allLogWriter == null)
                try
                {
                    _playerData = PlayerData.instance;
                    var user = _playerData.username + _playerData.id;
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var dayTimeStamp = DateTime.Now.ToString("yyyyMMdd");
                    var logDirectory = Path.Combine(Application.persistentDataPath, "vr-excavator" + dayTimeStamp, $"{user}");

                    if (!Directory.Exists(logDirectory))
                        try
                        {
                            Directory.CreateDirectory(logDirectory);
                            Debug.Log($"Directory created successfully at: {logDirectory}");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to create directory: {ex.Message}");
                            return;
                        }
                    else
                        Debug.Log($"Directory already exists at: {logDirectory}");

                    _mainLogWriter = new StreamWriter(Path.Combine(logDirectory, $"vr-exc-{user}_Log_main.txt"), true);
                    _timestampLogWriter =
                        new StreamWriter(Path.Combine(logDirectory, $"vr-exc-{user}_Log_timestamp.txt"), true);
                    _conditionalLogWriter =
                        new StreamWriter(Path.Combine(logDirectory, $"vr-exc-{user}_Log_conditional.txt"), true);
                    _allLogWriter = new StreamWriter(Path.Combine(logDirectory, $"vr-exc-{user}_Log_all.txt"), true);

                    LogAll($"--- New Game Session Started: {DateTime.Now} by {user} ---");
                    Debug.Log($"Log files created successfully in: {logDirectory}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to create log files: {ex.Message}");
                }
        }

        public static void LogAll(string message)
        {
            Debug.Log(message);
            if (!isLoggerEnabled) return;
            LogToWriter(_mainLogWriter, message);
            LogToWriter(_timestampLogWriter, message);
            LogToWriter(_conditionalLogWriter, message);
            LogToWriter(_allLogWriter, message);
        }

        public static void LogTimestamp(string message)
        {
            if (!isLoggerEnabled) return;
            LogToWriter(_timestampLogWriter, message);
            LogToWriter(_allLogWriter, message);
        }

        public static void LogConditional(string message)
        {
            Debug.Log(message);
            if (!isLoggerEnabled) return;
            LogToWriter(_conditionalLogWriter, message);
            LogToWriter(_allLogWriter, message);
        }

        private static void LogToWriter(StreamWriter writer, string message)
        {
            if (writer != null)
            {
                Debug.Log(message);
                writer.WriteLine($"{DateTime.Now}: {message}");
                writer.Flush();
            }
        }

        public static void Close()
        {
            if (!isLoggerEnabled) return;
            CloseWriter(_mainLogWriter);
            CloseWriter(_timestampLogWriter);
            CloseWriter(_conditionalLogWriter);
            CloseWriter(_allLogWriter);
        }

        private static void CloseWriter(StreamWriter writer)
        {
            if (writer != null)
            {
                writer.WriteLine($"--- Game Session Ended: {DateTime.Now} ---");
                writer.Close();
                writer = null;
            }
        }
    }
}