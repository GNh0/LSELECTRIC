using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LSELEC
{
    /// <summary>
    /// PLC 통신 로거 - PLC 통신 로그를 기록
    /// </summary>
    public class PLCLogger
    {
        private readonly string _logDirectory;
        private readonly string _logFileName;
        private readonly bool _enableConsoleOutput;
        private readonly object _lockObject = new object();

        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Critical
        }

        public PLCLogger(string logDirectory = "Logs", string logFileName = "plc_log.txt", bool enableConsoleOutput = true)
        {
            _logDirectory = logDirectory;
            _logFileName = logFileName;
            _enableConsoleOutput = enableConsoleOutput;

            // 로그 디렉토리 생성
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        /// <summary>
        /// 로그 메시지 기록
        /// </summary>
        public void Log(LogLevel level, string message, Exception? exception = null)
        {
            try
            {
                var logMessage = FormatLogMessage(level, message, exception);

                lock (_lockObject)
                {
                    // 파일에 기록
                    WriteToFile(logMessage);

                    // 콘솔 출력
                    if (_enableConsoleOutput)
                    {
                        WriteToConsole(level, logMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"로그 기록 실패: {ex.Message}");
            }
        }

        /// <summary>
        /// Debug 레벨 로그
        /// </summary>
        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Info 레벨 로그
        /// </summary>
        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        /// <summary>
        /// Warning 레벨 로그
        /// </summary>
        public void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Error 레벨 로그
        /// </summary>
        public void Error(string message, Exception? exception = null)
        {
            Log(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// Critical 레벨 로그
        /// </summary>
        public void Critical(string message, Exception? exception = null)
        {
            Log(LogLevel.Critical, message, exception);
        }

        private string FormatLogMessage(LogLevel level, string message, Exception? exception)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("[{0:yyyy-MM-dd HH:mm:ss.fff}] ", DateTime.Now);
            sb.AppendFormat("[{0,-8}] ", level.ToString().ToUpper());
            sb.Append(message);

            if (exception != null)
            {
                sb.AppendLine();
                sb.AppendFormat("Exception: {0}", exception.Message);
                if (exception.StackTrace != null)
                {
                    sb.AppendLine();
                    sb.Append(exception.StackTrace);
                }
            }

            return sb.ToString();
        }

        private void WriteToFile(string message)
        {
            var logFilePath = Path.Combine(_logDirectory, _logFileName);
            File.AppendAllText(logFilePath, message + Environment.NewLine);
        }

        private void WriteToConsole(LogLevel level, string message)
        {
            var originalColor = Console.ForegroundColor;

            switch (level)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
            }

            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// 로그 파일 크기 제한 및 로테이션
        /// </summary>
        public void RotateLogIfNeeded(long maxSizeBytes = 10 * 1024 * 1024) // 기본 10MB
        {
            try
            {
                var logFilePath = Path.Combine(_logDirectory, _logFileName);
                if (File.Exists(logFilePath))
                {
                    var fileInfo = new FileInfo(logFilePath);
                    if (fileInfo.Length > maxSizeBytes)
                    {
                        var backupFileName = $"{Path.GetFileNameWithoutExtension(_logFileName)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(_logFileName)}";
                        var backupFilePath = Path.Combine(_logDirectory, backupFileName);
                        File.Move(logFilePath, backupFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"로그 로테이션 실패: {ex.Message}");
            }
        }
    }
}
