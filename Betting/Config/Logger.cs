using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betting.Config
{
    public class Logger
    {
        public static object[] split(params object[] list)
        {
            object[] result = new object[list.Count()-1];
            for (int i=1;i<list.Count();++i)
            {
                result[i - 1] = list[i];
            }

            return result;
        }

        public static void LogResult(params object[] list)
        {
            if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_RESULT)
                lock (Logger.loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(list[0].ToString(), split(list));
                }
        }

        public static void LogResultSuccess(params object[] list)
        {
            if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_RESULT)
                lock (Logger.loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(list[0].ToString(), split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }

        }
        public static void LogResultFail(params object[] list)
        {
            if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_RESULT)
                lock (Logger.loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(list[0].ToString(), split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

        public static void LogInfo(params object[] list)
        {
            if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_INFO)
                lock (Logger.loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(list[0].ToString(), split(list));
                }
        }

        public static void LogInfoSuccess(params object[] list)
        {
            if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_INFO)
                lock (Logger.loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(list[0].ToString(), split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
        }

        public static void LogInfoFail(params object[] list)
        {
            if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_INFO)
                lock (Logger.loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(list[0].ToString(), split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
        }

        public static void LogDebug(params object[] list)
        {
            if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_DEBUG)
                lock (Logger.loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(list[0].ToString(), split(list));
                }
        }

        public static void LogDebugSuccess(params object[] list)
        {
            if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_DEBUG)
                lock (Logger.loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(list[0].ToString(), split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
        }

        public static void LogDebugFail(params object[] list)
        {
            if (ConfigManager.Instance.GetLogLevel() <= ConfigManager.LogLevel.LOG_DEBUG)
                lock (Logger.loggerLock)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(list[0].ToString(), split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
        }

        static private object loggerLock = new object();

    }
}
