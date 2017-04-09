
namespace SemicolonSystem.Model
{
    public class WeightModel
    {
        /// <summary>
        /// 位置
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 左偏移量
        /// </summary>
        public decimal OffsetLeft { get; set; }

        /// <summary>
        /// 右偏移量
        /// </summary>
        public decimal OffsetRight { get; set; }

        /// <summary>
        /// 优先级（从小到大依次递减）
        /// </summary>
        public short PriorityLevel { get; set; }
    }
}
