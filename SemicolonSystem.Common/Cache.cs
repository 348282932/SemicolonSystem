using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace SemicolonSystem.Common
{
    public class Cache<TData> where TData : class
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
        public DataResult<TData> GetCache(string fileName)
        {
            fileName = fileName + ".cache";

            string filePath = path + fileName;

            if (!File.Exists(filePath))
            {
                return new DataResult<TData>("找不到缓存文件！");
            }

            StreamReader sr = new StreamReader(filePath, Encoding.UTF8);

            string cache = sr.ReadToEnd();

            sr.Close();

            return new DataResult<TData>(JsonConvert.DeserializeObject<TData>(cache));
        }
    }
}
