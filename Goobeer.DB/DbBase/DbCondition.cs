namespace Goobeer.DB.DbBase
{
    public class DbCondition
    {
        /// <summary>
        /// 条件名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 条件值
        /// </summary>
        public dynamic FieldVal { get; set; }

        /// <summary>
        /// 条件名称 与 值 之间的关系
        /// </summary>
        public DataOperator DataOperator { get; set; }

        public ConditionOperator NextConditionOperator { get; set; }
    }
}
