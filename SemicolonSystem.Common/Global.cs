using Microsoft.Win32;
using System;

namespace SemicolonSystem.Common
{
    public class Global
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            InitBasePath();
        }

        /// <summary>
        /// 安装路径
        /// </summary>
        public static string InstallPath { get; set; }

        /// <summary>
        /// 初始化安装路径
        /// </summary>
        static void InitBasePath()
        {
            try
            {
                #region 获取注册表安装路径

                RegistryKey regKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);

                RegistryKey regSubKey = regKey.OpenSubKey(@"SOFTWARE\OrangeSemicolon");

                object objResult = regSubKey.GetValue("InstallPath");

                RegistryValueKind regValueKind = regSubKey.GetValueKind("InstallPath");

                if (regValueKind == RegistryValueKind.String)
                {
                    InstallPath = objResult.ToString();

                    InstallPath = InstallPath.Substring(0, InstallPath.LastIndexOf("\\"));
                }
                else
                {
                    InstallPath = @"C:\Program Files (x86)\OrangeSemicolon";
                }

                #endregion
            }
            catch(Exception ex)
            {
                InstallPath = @"C:\Program Files (x86)\OrangeSemicolon";
            }
        }
    }
}
