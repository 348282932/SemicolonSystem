﻿using SemicolonSystem.Common;
using System;
using System.Windows.Forms;

namespace SemicolonSystem.Show
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Global.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Global.IsAuthorization)
            {
                Application.Run(new MainForm());
            }
            else
            {
                Application.Run(new ActivationForm());
            }
        }
    }
}
