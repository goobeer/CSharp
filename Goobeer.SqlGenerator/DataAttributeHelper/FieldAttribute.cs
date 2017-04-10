using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Goobeer.DB.DataAttributeHelper
{
    /// <summary>
    /// 字段 属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute :Attribute
    {
        /// <summary>
        /// 字段 名称
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPK { get; set; }

        /// <summary>
        /// 是引用类型
        /// </summary>
        public bool IsClass { get; set; }

        /// <summary>
        /// 是否自动生成
        /// </summary>
        public bool AutoCreate { get; set; }

        /// <summary>
        /// 版本(记录修改次数)
        /// </summary>
        public int Version { get; set; }

        public FieldAttribute()
        {
        }

        public FieldAttribute(bool isPK)
            : this(string.Empty, isPK)
        { }

        public FieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public FieldAttribute(string fieldName,bool isPK):this(fieldName)
        {
            IsPK = isPK;
        }
    }

    public class AopProxy:RealProxy
    {
        public AopProxy(Type serverType):base(serverType)
        {

        }

        public override IMessage Invoke(IMessage msg)
        {
            //消息拦截
            if (msg is IConstructionCallMessage)
            {
                IConstructionCallMessage constructCallMsg = msg as IConstructionCallMessage;
                IConstructionReturnMessage constructionReturnMessage = this.InitializeServerObject((IConstructionCallMessage)msg);
                RealProxy.SetStubData(this, constructionReturnMessage.ReturnValue);
                return constructionReturnMessage;
            }
            else if (msg is IMethodCallMessage)
            {
                IMethodCallMessage callMsg = msg as IMethodCallMessage;
                object[] args = callMsg.Args;
                IMessage message;
                try
                {
                    if (callMsg.MethodName.StartsWith("set_") && args.Length == 1)
                    {
                        //这里检测到是set方法，然后应怎么调用对象的其它方法呢？
                        Console.WriteLine("wocao");
                    }
                    object o = callMsg.MethodBase.Invoke(GetUnwrappedServer(), args);
                    message = new ReturnMessage(o, args, args.Length, callMsg.LogicalCallContext, callMsg);
                }
                catch (Exception e)
                {
                    message = new ReturnMessage(e, callMsg);
                }
                return message;
            }
            return msg;
        }
    }
}
