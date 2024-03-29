﻿/// File: Core.Definitions.cs
/// Purpose: Defines the keyboard manager parts. 

/* 
Copyright (c) 2019, All rights are reserved by AB, 70 Technologies. 
https://www.Tech70.cf
This program is licensed under the Apache License, Version 2.0 (the "License");
you may not download, install or use this program except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*/ 

using System;

namespace XKeyboard.Core
{
    /// <summary>
    /// Defines the keyboard states or modes to use
    /// </summary>
    public enum KeyboardState
    {
        /// <summary>
        /// Indicates that all the keys must be forwarded.
        /// </summary>
        Enabled = 2,
        /// <summary>
        /// Indicates that all the keys must be discarded. 
        /// </summary>
        Disabled = 4,
        /// <summary>
        /// Indicates that all the keys must be intercepted by the program and changed. 
        /// </summary>
        Intercept = 8
    }
    /// <summary>
    /// Defines the hook parameters
    /// </summary>
    public struct HookCallBackParams
    {
        public int nCode;
        public IntPtr lParam;
        public IntPtr wParam;
        public int keyCode;
        public HookCallBackParams(int _keyCode, int _nCode, IntPtr _lParam, IntPtr _wParam)
        {
            keyCode = _keyCode;
            nCode = _nCode;
            lParam = _lParam;
            wParam = _wParam;
        }
    }
    /// <summary>
    /// Defines the message priorities used during notifications
    /// </summary>
    public enum MessagePriority
    {
        /// <summary>
        /// Indicates that the message must be highly prioritised. 
        /// Messages with HIGH priority are shown to the user in a dialog box and are also logged in a logfile.
        /// </summary>
        High,
        /// <summary>
        /// Indicates that the message must be prioritised more than the normal log message. 
        /// Messages with this priority are shown to the user on the NotifyIcon and aren't logged. 
        /// </summary>
        Mid,
        /// <summary>
        /// Indicates that the message has no priorities. 
        /// Messages with this priority are only logged in the log file without any notification to the user. 
        /// </summary>
        Low
    }
    /// <summary>
    /// Defines the type of message sent by the application for the user. 
    /// </summary>
    public enum MessageKind
    {
        /// <summary>
        /// Indicates that the message is a informational message. 
        /// </summary>
        Info,
        /// <summary>
        /// Indicates that the message is about some errors occured during the application runtime. 
        /// </summary>
        Error,
        /// <summary>
        /// Indicates that the message is a warning related to the application. 
        /// </summary>
        Warning
    }
}
