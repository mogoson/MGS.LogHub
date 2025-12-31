/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LogLevel.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  12/31/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System;

namespace MGS.Log
{
    [Flags]
    public enum LogLevel
    {
        None = 0,
        Log = 1 << 0,
        Warning = 1 << 1,
        Error = 1 << 2,
        Assert = 1 << 3,
        Exception = 1 << 4,
    }
}