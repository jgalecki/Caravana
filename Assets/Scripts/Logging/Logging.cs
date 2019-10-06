using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Logging
{
    public interface ILog
    {
        void Error(Exception ex, string error, params object[] args);
        void Error(string stack, string error, params object[] args);
        void Warn(string warning, params object[] args);
        void Write(string message, params object[] args);
    }

    /// <summary>
    /// Static class to log errors, warnings, and information.
    /// </summary>
    static class Log
    {
        static readonly string _logName = "CaravanaDebugLog";
        // There's a bug related to file enumeration that prevents this from having more than 9 log files.
        // Specifically, this looks at log9 before it looks at log10 and crashes when it tries to move log9.
        // Fixable, but won't unless the limited number becomes an issue.
        static readonly int MAX_LOGS = 10;
        static TextWriter _logger;

        /// <summary>
        /// Creates a new logging file, increments any older files so that we're always keeping a certain number.
        /// The TextWriter passed back by Open needs to surround any code that calls this class.
        /// 
        /// This function changes log4 to log5, log3 to log4, etc.
        /// </summary>
        public static void Open()
        {
            if (_logger != null) return;

            var path = Path.Combine(Directory.GetCurrentDirectory(), _logName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var files = Directory.GetFiles(path).ToList();
            for (int fileIndex = files.Count() - 1; fileIndex >= 0; fileIndex--)
            {
                var file = files[fileIndex];
                var shortName = Path.GetFileName(file);
                if (shortName.StartsWith(_logName))
                {
                    // Increment the log number, or delete it if it's too old.
                    int logNumber;
                    if (!Int32.TryParse(shortName.Substring(_logName.Length, shortName.IndexOf('.') - _logName.Length), out logNumber))
                    {
                        continue;
                    }

                    logNumber++;
                    if (logNumber < MAX_LOGS)
                    {
                        string newFile = Path.Combine(path, String.Format("{0}{1}{2}", _logName, logNumber, ".log"));
                        File.Move(file, newFile);
                    }
                    else
                    {
                        File.Delete(file);
                    }
                }
            }
            _logger = File.CreateText(Path.Combine(path, _logName + "0" + ".log"));
            ((StreamWriter)_logger).AutoFlush = true;
        }

        internal static void Error(Exception ex, string error, params object[] args)
        {
            _logger.WriteLine("*******************************************************");
            _logger.WriteLine(String.Format(error, args));
            _logger.WriteLine(ex);
        }

        internal static void Error(string stack, string error, params object[] args)
        {
            _logger.WriteLine("*******************************************************");
            _logger.WriteLine(String.Format(error, args));
            _logger.WriteLine(stack);
        }

        internal static void Warn(string warning, params object[] args)
        {
            _logger.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
            _logger.WriteLine(String.Format(warning, args));
        }

        internal static void Write(string message, params object[] args)
        {
            _logger.WriteLine(String.Format(message, args));
        }

        internal static ILog GetLogger(Type type)
        {
            return new ClassLogger(type);
        }

        //TODO: format so that hooked-on stack trace is more readable
        internal static Application.LogCallback GetLogCallback(ILog log)
        {
            return (logString, stack, type) =>
            {
                if (type == LogType.Exception)
                {
                    log.Error(stack, logString);
                }
            };
        }
    }

    /// <summary>
    /// Prepends each log message with the sending class.
    /// </summary>
    public class ClassLogger : ILog
    {
        private string _loggingClass;

        public ClassLogger(Type type)
        {
            string prefix = "Assets.Scripts.";
            var typeString = type.ToString().StartsWith(prefix) ? type.ToString().Substring(prefix.Length) : type.ToString();
            _loggingClass = "[" + typeString + "]: ";
            Log.Open();
        }

        public void Error(Exception ex, string error, params object[] args)
        {
            Log.Error(ex, _loggingClass + error, args);
        }

        public void Error(string stack, string error, params object[] args)
        {
            Log.Error(stack, _loggingClass + error, args);
        }

        public void Warn(string warning, params object[] args)
        {
            Log.Warn(_loggingClass + warning, args);
        }

        public void Write(string message, params object[] args)
        {
            Log.Write(_loggingClass + message, args);
        }

    }
}
