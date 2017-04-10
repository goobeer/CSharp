using Goobeer.DB.DataAttributeHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Goobeer.DB.ReflectionHelper
{
    public class EntityReflection<E> where E : class,new()
    {
        public E GetEntity()
        {
            return new E();
        }

        public IDictionary<PropertyInfo, FieldAttribute> GetFields<E>() where E : class, new()
        {
            IDictionary<PropertyInfo, FieldAttribute> idcPF = null;
            Type type = typeof(E);

            TableAttribute tabAttributes = type.GetCustomAttributes(typeof(TableAttribute), true).Cast<TableAttribute>().SingleOrDefault();
            idcPF = new Dictionary<PropertyInfo, FieldAttribute>();

            if (tabAttributes != null)
            {
                tabAttributes.TableName = tabAttributes.TableName ?? type.Name;
            }
            else
            {
                tabAttributes = new TableAttribute(type.Name);
            }

            IEnumerable<PropertyInfo> properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty).Where(pi => (pi.GetCustomAttribute<FieldAttribute>(true) == null && pi.GetCustomAttribute<FieldIgnoreAttribute>(true) == null) || pi.GetCustomAttribute<FieldAttribute>(true) != null || pi.GetCustomAttribute<FieldIgnoreAttribute>(true) == null);

            foreach (PropertyInfo item in properties)
            {
                FieldAttribute attribute = item.GetCustomAttribute(typeof(FieldAttribute), true) as FieldAttribute;

                if (attribute != null)
                {
                    attribute.FieldName = attribute.FieldName ?? item.Name;
                }
                else
                {
                    attribute = new FieldAttribute(item.Name);
                }
                idcPF.Add(item, attribute);
            }

            return idcPF;
        }

        public object GetPropertyValue(PropertyInfo pi,E e)
        {
            object result = null;
            
            var getPropDelegate = CreateGetDelegate(e, pi);
            result = getPropDelegate.DynamicInvoke(e);
            return result;
        }

        /// <summary>
        /// 获得 set Action
        /// </summary>
        /// <returns></returns>
        public Delegate CreateSetDelegate(E target, PropertyInfo pi, object val)
        {
            Delegate setDelegate = null;
            if (target!=null && pi !=null)
            {
                MethodInfo mthSet = pi.GetSetMethod();
                Type paraType = mthSet.GetParameters()[0].ParameterType;

                DynamicMethod method = new DynamicMethod(string.Empty, null, new Type[] { target.GetType(), typeof(object) }, pi.PropertyType, true);

                ILGenerator il = method.GetILGenerator();
                var localVar = il.DeclareLocal(paraType,true);
                il.Emit(OpCodes.Ldarg_1);
                if (paraType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, paraType);
                }
                else
                {
                    il.Emit(OpCodes.Castclass, paraType);
                }
                il.Emit(OpCodes.Stloc, localVar);
                il.Emit(OpCodes.Ldarg_0);   // 加载第一个参数 owner
                il.Emit(OpCodes.Ldloc, localVar);// 加载本地参数
                il.EmitCall(OpCodes.Callvirt, mthSet, null);//调用函数
                il.Emit(OpCodes.Ret);

                method.DefineParameter(1, ParameterAttributes.In, string.Empty);
                method.DefineParameter(2, ParameterAttributes.In, string.Empty);

                Type genericDelegateType = typeof(Action<,>).MakeGenericType(target.GetType(), typeof(object));
                setDelegate = method.CreateDelegate(genericDelegateType);
                //缓存setDelegate以提升性能
                setDelegate.DynamicInvoke(target, val);
            }
            return setDelegate;
        }

        /// <summary>
        /// 获得 get Func
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public static Delegate CreateGetDelegate(E e, PropertyInfo pi)
        {
            Delegate getProperyDelegate = null;
            if (e != null && pi != null)
            {
                MethodInfo mthGet = pi.GetGetMethod();
                var getGeneic = typeof(Func<,>).MakeGenericType(e.GetType(), pi.PropertyType);

                var getMthDynm = new DynamicMethod(string.Empty, pi.PropertyType, new Type[] { e.GetType() });
                ILGenerator ilg = getMthDynm.GetILGenerator();
                ilg.Emit(OpCodes.Ldarg_0);
                ilg.EmitCall(OpCodes.Callvirt, mthGet, null);
                ilg.Emit(OpCodes.Ret);
                getMthDynm.DefineParameter(1, ParameterAttributes.In, string.Empty);

                getProperyDelegate = getMthDynm.CreateDelegate(getGeneic);//需要存储这个Delegate,以提升程序的性能
            }
            return getProperyDelegate;
        }
    }
}
