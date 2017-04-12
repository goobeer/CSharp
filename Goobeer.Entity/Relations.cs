using Goobeer.DB.DataAttributeHelper;
using Goobeer.Entity.Base;
using System;

namespace Goobeer.Entity
{
    [Table(TableName = "[O.Relations]")]
    public class Relations:OrderBase
    {
        public Guid CID1 { get; set; }
        public Guid CID2 { get; set; }
        public Guid FieldID { get; set; }

        /// <summary>
        /// 关系方向
        /// </summary>
        public RDirect RDirect { get; set; }

        /// <summary>
        /// 关系强度
        /// </summary>
        public int RStrength { get; set; }

        /// <summary>
        /// 关系是否可传递
        /// </summary>
        public bool IsTransitive { get; set; }
    }

    [Flags]
    public enum RDirect
    {
        Forward=1,
        Reversion,
        Both=Forward | Reversion
    }
}
