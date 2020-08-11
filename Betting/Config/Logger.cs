﻿using System;
using System.Linq;

namespace Betting.Config
{
    public class Logger
    {
        public Logger(ConfigManager.LogLevel logLevel)
        {
            this.logLevel = logLevel;
        }
        public object[] Split(params object[] list)
        {
            object[] result = new object[list.Count() - 1];
            for (int i = 1; i < list.Count(); ++i)
            {
                result[i - 1] = list[i];
            }

            return result;
        }

        public void LogResult(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_RESULT)
                lock (loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(list[0].ToString(), Split(list));
                }
        }

        public void LogResultSuccess(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_RESULT)
                lock (loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(list[0].ToString(), Split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }

        }
        public void LogResultFail(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_RESULT)
                lock (loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(list[0].ToString(), Split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
        }

        public void LogInfo(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_INFO)
                lock (loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(list[0].ToString(), Split(list));
                }
        }

        public void LogInfoSuccess(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_INFO)
                lock (loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(list[0].ToString(), Split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
        }

        public void LogInfoFail(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_INFO)
                lock (loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(list[0].ToString(), Split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
        }

        public void LogDebug(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_DEBUG)
                lock (loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(list[0].ToString(), Split(list));
                }
        }

        public void LogDebugSuccess(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_DEBUG)
                lock (loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(list[0].ToString(), Split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
        }

        public void LogDebugFail(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_DEBUG)
                lock (loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(list[0].ToString(), Split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
        }

        private readonly object loggerLock = new object();
        private readonly ConfigManager.LogLevel logLevel;

    }
}
