
namespace SemicolonSystem.Model
{
    public class WeightModel
    {
        /// <summary>
        /// 位置
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 偏移量
        /// </summary>
        public decimal Offset { get; set; }

        /// <summary>
        /// 优先级（从小到大依次递减）
        /// </summary>
        public short PriorityLevel { get; set; }
    }
}
