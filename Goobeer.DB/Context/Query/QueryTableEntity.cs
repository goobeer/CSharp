using Goobeer.DB.ReflectionHelper;
using System.Collections.Generic;

namespace Goobeer.DB.Context.Query
{
    public class QueryTableEntity
    {
        /// <summary>
        /// 主表
        /// </summary>
        public string TabName
        {
            get; set;
        }

        public static string GetTabName<E>() where E : class, new()
        {
            var tabAttr = EntityReflection<E>.GetTableAttr();
            return tabAttr.TableName;
        }

        /// <summary>
        /// 别名
        /// </summary>
        public string AliasTabName { get; set; }
        public List<string> SelectFields { get; set; }

        private QueryPredicate _QP;
        public QueryPredicate QP
        {
            get
            {
                if (_QP==null)
                {
                    InitQP();
                }
                return _QP;
            }
            set
            {
                _QP = value;
            }
        }

        private void InitQP()
        {
            _QP= new QueryPredicate() { Data = new Command.BaseCmdData() { TableName = AliasTabName ?? TabName ,DBCC=new DbBase.DbConditionCollection()} };
        }
    }
}
