/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  FileLogger.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/31/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using System.IO;
using UnityEngine;

namespace MGS.Log
{
    public class FileLogger : ILogger
    {
        #region
        public string RootDir { protected set; get; }
        public const string EXTENSION = ".log";

        public FileLogger(string rootDir)
        {
            RootDir = rootDir;
        }
        #endregion

        #region
        public void Output(string condition, string stackTrace, LogType type)
        {
            var logFile = ResolveFile(RootDir);
            var logContent = ResolveContent(condition, stackTrace, type);
            AppendToFile(logFile, logContent);
        }

        protected virtual string ResolveFile(string rootDir)
        {
            var fileName = DateTime.Now.ToString("yyyy-MM-dd");
            return $"{rootDir}/{fileName}{EXTENSION}";
        }

        protected virtual string ResolveContent(string condition, string stackTrace, LogType type)
        {
            var timeStamp = DateTime.Now.ToLongTimeString();
            return $"{timeStamp} {type}\r\n{condition}\r\n{stackTrace}\r\n";
        }

        protected void AppendToFile(string filePath, string content)
        {
            try
            {
                RequireDirectory(filePath);
                File.AppendAllText(filePath, content);
            }
            catch { }
        }

        protected void RequireDirectory(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        #endregion

        #region
        public void Clear(int daysBefore)
        {
            var files = Directory.GetFiles(RootDir, $"*{EXTENSION}");
            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (DateTime.TryParse(fileName, out DateTime fileTime))
                {
                    if ((DateTime.Now - fileTime).Days < daysBefore)
                    {
                        continue;
                    }
                }

                DeleteFile(file);
            }
        }

        protected void DeleteFile(string filePath)
        {
            try { File.Delete(filePath); }
            catch { }
        }
        #endregion
    }
}