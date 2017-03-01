
namespace SemicolonSystem.Model.Enum
{
    /// <summary>
    /// 匹配程度枚举
    /// </summary>
    public enum MatchingLevel : byte
    {   
        /// <summary>
        /// 完全匹配
        /// </summary>
        PerfectMatch = 1,

        /// <summary>
        /// 勉强匹配
        /// </summary>
        BarelyMatch = 2,

        /// <summary>
        /// 强行匹配（完全不匹配，取最接近的）
        /// </summary>
        ForceMatching = 3

    }
}
