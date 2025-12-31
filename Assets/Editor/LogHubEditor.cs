/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LogHubEditor.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/31/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System.IO;
using UnityEditor;
using UnityEngine;

namespace MGS.Log.Editors
{
    public sealed class LogHubEditor : ScriptableWizard
    {
        #region
        [MenuItem("Tools/Log Hub/Settings")]
        static void ShowEditor()
        {
            DisplayWizard<LogHubEditor>("Log Hub Settings", "Apply");
        }

        [MenuItem("Tools/Log Hub/Outputs")]
        static void ShowOutputs()
        {
            Application.OpenURL(Register.OutputDir);
        }
        #endregion

        #region
        void OnEnable()
        {
            InitSettings();
        }

        void OnWizardUpdate()
        {
            isValid = CheckSettings();
        }

        void OnWizardCreate()
        {
            ApplySettings();
        }
        #endregion

        #region
        const string SETTINGS_PATH = "Assets/LogSettings/Resources/LogSettings.asset";
        [SerializeField] private LogLevel outputLevel = LogLevel.Error | LogLevel.Assert | LogLevel.Exception;
        [SerializeField] private bool autoClear = true;
        [SerializeField] private int periodDays = 30;

        void InitSettings()
        {
            var settings = LoadSettings();
            if (settings)
            {
                outputLevel = settings.outputLevel;
                autoClear = settings.autoClear;
                periodDays = settings.periodDays;
            }
        }

        bool CheckSettings()
        {
            if (autoClear && periodDays <= 0)
            {
                return false;
            }
            return true;
        }

        void ApplySettings()
        {
            var settings = RequireSettings();
            settings.outputLevel = outputLevel;
            settings.autoClear = autoClear;
            settings.periodDays = periodDays;
            AssetDatabaseSave(settings);
            Debug.Log($"LogHubEditor has applied parameters to the LogSettings.");
        }

        LogSettings LoadSettings()
        {
            return Resources.Load<LogSettings>(nameof(LogSettings));
        }

        LogSettings RequireSettings()
        {
            var settings = LoadSettings();
            if (settings == null)
            {
                RequireDirectory(SETTINGS_PATH);
                settings = CreateInstance<LogSettings>();
                AssetDatabase.CreateAsset(settings, SETTINGS_PATH);
                Debug.Log($"LogHubEditor Create LogSettings at path {SETTINGS_PATH}");
            }
            return settings;
        }

        void RequireDirectory(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        void AssetDatabaseSave(Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssetIfDirty(asset);
        }
        #endregion
    }
}