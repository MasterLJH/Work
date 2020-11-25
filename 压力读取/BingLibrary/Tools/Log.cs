using NLog;
using System;
using System.IO;
using System.Text;

namespace BingLibrary.hjb.tools
{
    public static class Log
    {
        public static nLog Default { get; private set; }

        static Log()
        {
            Default = new nLog(LogManager.GetCurrentClassLogger());
        }

        public class nLog
        {
            private Logger logger;

            public nLog(Logger logger)
            {
                this.logger = logger;
            }

            public nLog(string name)
                    : this(LogManager.GetLogger(name))
            {
            }

            public void Debug(string msg, params object[] args)
            {
                logger.Debug(msg, args);
            }

            public void Debug(string msg, Exception err)
            {
                logger.Debug(err, msg);
            }

            public void Info(string msg, params object[] args)
            {
                logger.Info(msg, args);
            }

            public void Info(string msg, Exception err)
            {
                logger.Info(err, msg);
            }

            public void Warn(string msg, params object[] args)
            {
                logger.Warn(msg, args);
            }

            public void Warn(string msg, Exception err)
            {
                logger.Warn(err, msg);
            }

            public void Trace(string msg, params object[] args)
            {
                logger.Trace(msg, args);
            }

            public void Trace(string msg, Exception err)
            {
                logger.Trace(err, msg);
            }

            public void Error(string msg, params object[] args)
            {
                logger.Error(msg, args);
            }

            public void Error(string msg, Exception err)
            {
                logger.Error(err, msg);
            }

            public void Fatal(string msg, params object[] args)
            {
                logger.Fatal(msg, args);
            }

            public void Fatal(string msg, Exception err)
            {
                logger.Fatal(err, msg);
            }
        }
    }

   
}