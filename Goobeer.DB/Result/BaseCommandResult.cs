using Goobeer.DB.Command;
using System;
using System.Collections.Generic;

namespace Goobeer.DB.Result
{
    public class BaseCommandResult : ICommandResult
    {
        /// <summary>
        /// 操作条件或操作数据
        /// </summary>
        public Dictionary<string,dynamic> Data { get; set; }

        public string RenderCommandResult(BaseCmdData data)
        {
            throw new NotImplementedException();
        }
    }
}
