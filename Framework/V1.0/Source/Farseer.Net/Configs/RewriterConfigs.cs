using System;
using System.Collections.Generic;

namespace FS.Configs
{
    /// <summary>
    ///     全局
    /// </summary>
    public class RewriterConfigs : BaseConfigs<RewriterConfig> { }

    /// <summary>
    ///     重写地址规则
    /// </summary>
    [Serializable]
    public class RewriterConfig
    {
        /// <summary>
        ///     重写地址规则列表
        /// </summary>
        public List<RewriterRule> Rules = new List<RewriterRule>();
    }

    /// <summary>
    ///     重写地址规则
    /// </summary>
    public class RewriterRule
    {
        /// <summary>
        ///     请求地址
        /// </summary>
        public string LookFor { get; set; }

        /// <summary>
        ///     重写地址
        /// </summary>
        public string SendTo { get; set; }

        /// <summary>
        ///     通过索引返回实体
        /// </summary>
        public static implicit operator RewriterRule(int index)
        {
            return RewriterConfigs.ConfigInfo.Rules.Count <= index ? null : RewriterConfigs.ConfigInfo.Rules[index];
        }
    }
}