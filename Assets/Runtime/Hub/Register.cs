/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  Register.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/31/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEngine;

namespace MGS.Log
{
    public sealed class Register
    {
        public static string OutputDir { get; }

        static Register()
        {
            OutputDir = $"{Application.persistentDataPath}/Log";
        }

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        static void Initialize()
        {
            var logger = new FileLogger(OutputDir);
            LogHub.Register(logger);
            Debug.Log($"Register FileLogger with path {OutputDir}");
        }
    }
}