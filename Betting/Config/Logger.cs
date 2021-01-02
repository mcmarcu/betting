using System;
using System.Linq;
using System.Threading;

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
            {
                bool lockTaken = false;
                try
                {
                    _spinlock.Enter(ref lockTaken);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(list[0].ToString(), Split(list));
                }
                finally
                {
                    if (lockTaken) _spinlock.Exit(false);
                }
            }
        }

        public void LogResultSuccess(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_RESULT)
            {
                bool lockTaken = false;
                try
                {
                    _spinlock.Enter(ref lockTaken);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(list[0].ToString(), Split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
                finally
                {
                    if (lockTaken) _spinlock.Exit(false);
                }
            }

        }
        public void LogResultFail(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_RESULT)
            {
                bool lockTaken = false;
                try
                {
                    _spinlock.Enter(ref lockTaken);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(list[0].ToString(), Split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
                finally
                {
                    if (lockTaken) _spinlock.Exit(false);
                }
            }
        }

        public void LogInfo(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_INFO)
            {
                bool lockTaken = false;
                try
                {
                    _spinlock.Enter(ref lockTaken);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(list[0].ToString(), Split(list));
                }
                finally
                {
                    if (lockTaken) _spinlock.Exit(false);
                }
            }
        }

        public void LogInfoSuccess(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_INFO)
            {
                bool lockTaken = false;
                try
                {
                    _spinlock.Enter(ref lockTaken);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(list[0].ToString(), Split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
                finally
                {
                    if (lockTaken) _spinlock.Exit(false);
                }
            }
        }

        public void LogInfoFail(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_INFO)
            {
                bool lockTaken = false;
                try
                {
                    _spinlock.Enter(ref lockTaken);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(list[0].ToString(), Split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
                finally
                {
                    if (lockTaken) _spinlock.Exit(false);
                }
            }
        }

        public void LogDebug(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_DEBUG)
            {
                bool lockTaken = false;
                try
                {
                    _spinlock.Enter(ref lockTaken);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(list[0].ToString(), Split(list));
                }
                finally
                {
                    if (lockTaken) _spinlock.Exit(false);
                }
            }
        }

        public void LogDebugSuccess(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_DEBUG)
            {
                bool lockTaken = false;
                try
                {
                    _spinlock.Enter(ref lockTaken);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(list[0].ToString(), Split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
                finally
                {
                    if (lockTaken) _spinlock.Exit(false);
                }
            }
        }

        public void LogDebugFail(params object[] list)
        {
            if (logLevel <= ConfigManager.LogLevel.LOG_DEBUG)
            {
                bool lockTaken = false;
                try
                {
                    _spinlock.Enter(ref lockTaken);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(list[0].ToString(), Split(list));
                    Console.ForegroundColor = ConsoleColor.White;
                }
                finally
                {
                    if (lockTaken) _spinlock.Exit(false);
                }
            }
        }

        private SpinLock _spinlock = new SpinLock();
        private readonly ConfigManager.LogLevel logLevel;

    }
}
