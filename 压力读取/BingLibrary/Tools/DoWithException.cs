﻿using System;
using System.Windows;
using System.IO;

namespace BingLibrary.hjb.tools
{
   public class DoWithException
    {

        public const string 示例提示 = "软件错误,无法继续。\n请将日志文件提供给供应商，用于分析解决问题。\n给您造成不便，非常抱歉。";
        public const string ExampleHint = "This software has broken and could not be continued. \n Please provide the log file to the vendor for analysis to solve the problem. \n Sorry.";
        /// <summary>
        /// 在当前线程上启用标准处理 
        /// </summary>
        public static void 标准处理(string 额外提示 = 示例提示)
        {
            if (!已挂载)
            {
                //UI异常
                Application.Current.DispatcherUnhandledException += (s, a) =>
                {
                    Exception e = a.Exception;
                    if (e.InnerException != null)
                        e = e.InnerException;
                    标准处理(e);
                    MessageBox.Show(e.GetType().Name + " " + e.Message +
                        "\r\n日志保存在" + Environment.CurrentDirectory + "\r\n" + 额外提示);
                    Application.Current.Shutdown();
                };
                //非UI异常
                AppDomain.CurrentDomain.UnhandledException += (s, a) =>
                {
                    Exception e = a.ExceptionObject as Exception;
                    if (e.InnerException != null)
                        e = e.InnerException;
                    标准处理(e);
                    MessageBox.Show(e.GetType().Name + " " + e.Message +
                        "\r\n日志保存在" + Environment.CurrentDirectory + "\r\n" + 额外提示);
                    Application.Current.Shutdown();
                    MessageBox.Show(额外提示);
                    Application.Current.Shutdown();
                };

                已挂载 = true;
            }
        }
        private static void 标准处理(Exception e)
        {
            var sw = new StreamWriter("exception.log", true);
            sw.WriteLine(e.ToString());
            sw.Flush();
            sw.Close();
        }

        private static bool 已挂载 = false;
  

}
}
