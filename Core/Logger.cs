﻿/// File: Logger.cs
/// Purpose: Defines the logger used in this application to log various messages. 
/// Version: 1.0.0.1 (beta)
/// Date: 23, Feb, 2019

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
using System.IO;

namespace XKeyboard.Core
{
    /// <summary>
    /// This class contains functions used to log, notify or show corresponding messages to the user.
    /// 
    /// </summary>
    class Logger
    {
        //Log file
        static StreamWriter logFile;
        /// <summary>
        /// Initializes the logger. 
        /// </summary>
        public static void Init()
        {
            //Executed when the class is defined on the application level.
            //Check if log file exceeds max file size (1MB)
            if (logFile != null)
            {
                logFile.Close();
                logFile = null;
            }
            if (File.Exists("logs.txt") && new FileInfo("logs.txt").Length >= (Math.Pow(1024, 3) * 2)) //2MB
                File.Delete("logs.txt");
            logFile = new StreamWriter("logs.txt", true);
            logFile.AutoFlush = true;
        }
        public static void WriteHeader()
        {
            if (logFile == null)
                Init();
            logFile.Write("XKeyboard".Fill(80, '-') + "\r\n");
        }
        /// <summary>
        /// Saves the current log file and closes it. 
        /// </summary>
        public static void Save()
        {
            if (logFile == null) return;
            logFile.Close();
            logFile.Dispose();
            logFile = null;
        }
        /// <summary>
        /// Logs the message in a file.
        /// </summary>
        /// <param name="output">The message to log.</param>
        public static void Log(string output, MessagePriority priority = MessagePriority.Low, MessageKind kind = MessageKind.Info)
        {
            if (logFile == null)
                Init();
            string head = (priority == MessagePriority.Low) ? "[LOW]" : (priority == MessagePriority.Mid ? "[MIDIUM]" : "[HIGH]");
            head += kind == MessageKind.Info ? "[INFO]" : (kind == MessageKind.Warning ? "[WARNING]" : "[ERROR]");
            logFile.Write($"[{DateTime.Now}]{head}{output}\r\n");
            Save();
        }  
        /// <summary>
        /// Notifies the user with a message using NotifyIcon and windows desktop alerts. 
        /// </summary>
        /// <param name="msg">The message to show in notification. </param>s
        public static void Notify(string msg, MessageKind kind)
        {
            /////TO-DO/////
            /// Call notify icon
            ///////////////
            NotificationManager.Show(msg, kind);
        }
        /// <summary>
        /// Shows a metro-styled message box to the user and returns the dialog result.
        /// </summary>
        /// <param name="msg">The message to show. </param>
        /// <param name="img">The message box icon.</param>
        /// <param name="btn">Message box buttons. </param>
        public static void ShowMessage(string msg, MessageKind kind)
        {
            UI.Metro.MessageBoxEx.Show(null, msg, kind);
        }
        /// <summary>
        /// Shows a message to the user, notifies via notify icon or logs the message. 
        /// </summary>
        /// <param name="msg">THe message. </param>
        /// <param name="priority">Priority of the message. </param>
        public static void Write(string msg, MessagePriority priority = MessagePriority.Low, MessageKind kind = MessageKind.Info)
        {
            Log(msg, priority, kind);
            switch (priority)
            {
                case MessagePriority.High:
                    //Show message to the user and log it as well 
                    ShowMessage(msg, kind);
                    break;
                case MessagePriority.Mid:
                    //Notify the message to the user
                    Notify(msg, kind);
                    break;
                case MessagePriority.Low:
                    //Log the message. 
                    break;
                default:
                    break;
            }
        }
    }
    
}
