/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LogHub.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/31/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MGS.Log
{
    public sealed class LogHub
    {
        #region
        static readonly List<ILogger> loggers = new();
        static LogSettings settings;
        const string KEY_LOG_CLEAR_LAST_TIME = "KEY_LOG_CLEAR_LAST_TIME";

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        static void Initialize()
        {
            InitSettings();
            InitListener();
        }

        static void InitSettings()
        {
            settings = Resources.Load<LogSettings>(nameof(LogSettings));
            if (settings.autoClear)
            {
                var lastClearTime = GetRecord();
                if ((DateTime.Now - lastClearTime).Days > settings.periodDays)
                {
                    InitCleaner();
                }
            }
        }

        static void InitListener()
        {
            Application.logMessageReceivedThreaded += OnLogMessage;
        }
        #endregion

        #region
        static void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            if (!IfOutput(type))
            {
                return;
            }

            foreach (var logger in loggers)
            {
                logger.Output(condition, stackTrace, type);
            }
        }

        static bool IfOutput(LogType type)
        {
            var level = LogLevel.None;
            switch (type)
            {
                case LogType.Log:
                    level = LogLevel.Log;
                    break;
                case LogType.Warning:
                    level = LogLevel.Warning;
                    break;
                case LogType.Error:
                    level = LogLevel.Error;
                    break;
                case LogType.Assert:
                    level = LogLevel.Assert;
                    break;
                case LogType.Exception:
                    level = LogLevel.Exception;
                    break;
            }
            return (settings.outputLevel & level) > 0;
        }
        #endregion

        #region
        public static void Register(ILogger logger)
        {
            loggers.Add(logger);
        }

        public static void Unregister(ILogger logger)
        {
            loggers.Remove(logger);
        }

        public static void Clear()
        {
            loggers.Clear();
        }
        #endregion

        #region
        static DateTime GetRecord()
        {
            var record = PlayerPrefs.GetString(KEY_LOG_CLEAR_LAST_TIME);
            if (DateTime.TryParse(record, out DateTime dateTime))
            {
                return dateTime;
            }

            UpdateRecord();
            return DateTime.Now;
        }

        static void UpdateRecord()
        {
            PlayerPrefs.SetString(KEY_LOG_CLEAR_LAST_TIME, DateTime.Now.ToString());
        }

        static void InitCleaner()
        {
            var listener = LogHubListener.CreateOne();
            listener.OnAppQuit += OnAppQuit;
        }

        static void OnAppQuit()
        {
            foreach (var logger in loggers)
            {
                logger.Clear(settings.periodDays);
            }
            UpdateRecord();
        }

        class LogHubListener : MonoBehaviour
        {
            public event Action OnAppQuit;

            private void OnApplicationQuit()
            {
                OnAppQuit?.Invoke();
            }

            public static LogHubListener CreateOne()
            {
                var go = new GameObject(nameof(LogHubListener))
                {
                    hideFlags = HideFlags.HideInInspector
                };
                DontDestroyOnLoad(go);
                return go.AddComponent<LogHubListener>();
            }
        }
        #endregion
    }
}