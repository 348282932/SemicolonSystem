using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace SemicolonSystem.Common
{
    public class Cache<TData>
    {
        private string path = AppDomain.CurrentDomain.BaseDirectory + "\\Cache\\";

        /// <summary>
        /// 主键
        /// </summary>
        private string Key { get; set; }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void SetCache(string fileName, TData data)
        {
            fileName = fileName + ".cache";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filePath = path + fileName;

            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            string cache = JsonConvert.SerializeObject(data);

            File.WriteAllText(filePath, cache, Encoding.UTF8);
        }

        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public TData GetCache(string fileName)
        {
            fileName = fileName + ".cache";

            string filePath = path + fileName;

            StreamReader sr = new StreamReader(filePath, Encoding.UTF8);

            string cache = sr.ReadToEnd();

            sr.Close();

            return JsonConvert.DeserializeObject<TData>(cache);
        }
    }
}
