
namespace SemicolonSystem.Common
{
    public class DataResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        public DataResult()
        {
            this.IsSuccess = true;
        }

        /// <summary>
        /// 创建请求成功的默认返回值
        /// </summary>
        public DataResult(string message)
        {
            this.IsSuccess = false;
            this.Message = message;
        }
    }

    public class DataResult<TData> : DataResult where TData : class
    {
        /// <summary>
        /// 返回的数据
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// 创建请求成功的默认返回值
        /// </summary>
        public DataResult() : base() { }

        /// <summary>
        /// 创建请求失败的默认返回值
        /// </summary>
        public DataResult(string message) : base(message) { }

        /// <summary>
        /// 根据返回的数据创建请求成功的返回值
        /// </summary>
        /// <param name="data"></param>
        public DataResult(TData data)
            : base()
        {
            this.Data = data;
        }
    }
}
