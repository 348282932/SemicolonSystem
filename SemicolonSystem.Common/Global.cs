using Microsoft.Win32;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Management;

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
            VerificationAuthorization();
        }
        /// <summary>
        /// 密钥
        /// </summary>
        public const string Key = "JZFH";

        /// <summary>
        /// 软件是否授权
        /// </summary>
        public static bool IsAuthorization { get; set; }

        /// <summary>
        /// 安装路径
        /// </summary>
        public static string InstallPath { get; set; }

        /// <summary>
        /// 指纹码（系统唯一标识）
        /// </summary>
        public static string FingerPrint { get; set; }

        /// <summary>
        /// 验证授权
        /// </summary>
        static void VerificationAuthorization()
        {
            IsAuthorization = false;

            FingerPrint = GetFingerPrint();

            var guidCache = new Cache<string>().GetCache("SecretKey");

            if (guidCache.IsSuccess)
            {
                if (Encryption.MD5(FingerPrint, Key) == guidCache.Data)
                {
                    IsAuthorization = true;
                }
            }
        }

        /// <summary>
        /// 初始化安装路径
        /// </summary>
        static void InitBasePath()
        {
            try
            {
                #region 获取注册表安装路径

                RegistryKey regKey = Registry.LocalMachine;

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
            catch(Exception)
            {
                InstallPath = @"C:\Program Files (x86)\OrangeSemicolon";
            }
        }

        #region 获取机器唯一标识

        public static string GetFingerPrint()
        {
            return GetHash("CPU >> " + cpuId() + "\nBIOS >> " + biosId() + "\nBASE >> " + baseId() + "\nVIDEO >> " + videoId());
        }

        private static string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }
        private static string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }
        #region Original Device ID Getting Code
        //Return a hardware identifier
        private static string identifier
        (string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            ManagementClass mc = new ManagementClass(wmiClass);
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    //Only get the first one
                    if (result == "")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }
        //Return a hardware identifier
        private static string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            ManagementClass mc = new ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                //Only get the first one
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }
        private static string cpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "") //If no UniqueID, use ProcessorID
            {
                retVal = identifier("Win32_Processor", "ProcessorId");
                if (retVal == "") //If no ProcessorId, use Name
                {
                    retVal = identifier("Win32_Processor", "Name");
                    if (retVal == "") //If no Name, use Manufacturer
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                    //Add clock speed for extra security
                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return retVal;
        }
        //BIOS Identifier
        private static string biosId()
        {
            return identifier("Win32_BIOS", "Manufacturer")
            + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + identifier("Win32_BIOS", "IdentificationCode")
            + identifier("Win32_BIOS", "SerialNumber")
            + identifier("Win32_BIOS", "ReleaseDate")
            + identifier("Win32_BIOS", "Version");
        }
        //Main physical hard drive ID
        private static string diskId()
        {
            return identifier("Win32_DiskDrive", "Model")
            + identifier("Win32_DiskDrive", "Manufacturer")
            + identifier("Win32_DiskDrive", "Signature")
            + identifier("Win32_DiskDrive", "TotalHeads");
        }
        //Motherboard ID
        private static string baseId()
        {
            return identifier("Win32_BaseBoard", "Model")
            + identifier("Win32_BaseBoard", "Manufacturer")
            + identifier("Win32_BaseBoard", "Name")
            + identifier("Win32_BaseBoard", "SerialNumber");
        }
        //Primary video controller ID
        private static string videoId()
        {
            return identifier("Win32_VideoController", "DriverVersion")
            + identifier("Win32_VideoController", "Name");
        }
        //First enabled network card ID
        private static string macId()
        {
            return identifier("Win32_NetworkAdapterConfiguration",
                "MACAddress", "IPEnabled");
        }
        #endregion

        #endregion
    }
}
