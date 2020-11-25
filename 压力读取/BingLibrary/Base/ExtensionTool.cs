using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;

namespace BingLibrary.hjb.mvvm
{
    internal class FastInvoke
    {
        public delegate object FastInvokeHandler(object target, object[] paramters);

        private static object InvokeMethod(FastInvokeHandler invoke, object target, params object[] paramters)
        {
            return invoke(null, paramters);
        }

        public static FastInvokeHandler GetMethodInvoker(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();
            ParameterInfo[] ps = methodInfo.GetParameters();
            Type[] paramTypes = new Type[ps.Length];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    paramTypes[i] = ps[i].ParameterType.GetElementType();
                else
                    paramTypes[i] = ps[i].ParameterType;
            }
            LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

            for (int i = 0; i < paramTypes.Length; i++)
            {
                locals[i] = il.DeclareLocal(paramTypes[i], true);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitFastInt(il, i);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCastToReference(il, paramTypes[i]);
                il.Emit(OpCodes.Stloc, locals[i]);
            }
            if (!methodInfo.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    il.Emit(OpCodes.Ldloca_S, locals[i]);
                else
                    il.Emit(OpCodes.Ldloc, locals[i]);
            }
            if (methodInfo.IsStatic)
                il.EmitCall(OpCodes.Call, methodInfo, null);
            else
                il.EmitCall(OpCodes.Callvirt, methodInfo, null);
            if (methodInfo.ReturnType == typeof(void))
                il.Emit(OpCodes.Ldnull);
            else
                EmitBoxIfNeeded(il, methodInfo.ReturnType);

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(il, i);
                    il.Emit(OpCodes.Ldloc, locals[i]);
                    if (locals[i].LocalType.IsValueType)
                        il.Emit(OpCodes.Box, locals[i].LocalType);
                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            il.Emit(OpCodes.Ret);
            FastInvokeHandler invoder = (FastInvokeHandler)dynamicMethod.CreateDelegate(typeof(FastInvokeHandler));
            return invoder;
        }

        private static void EmitCastToReference(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        private static void EmitBoxIfNeeded(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
        }

        private static void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;

                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;

                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;

                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;

                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;

                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;

                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;

                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;

                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;

                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            if (value > -129 && value < 128)
            {
                il.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, value);
            }
        }
    }

    internal class RefelectionExtension
    {
        public static Dictionary<string, object> getMethods(object obj)
        {
            if (obj == null)
                return null;
            Dictionary<string, object> tempd = new Dictionary<string, object>();

            foreach (var m in obj.GetType().GetMethods())
            {
                if (!new Regex(RegexStr1).Match(m.DeclaringType.Name).Success && (m.ToString().Substring(0, 4) == RegexStr3 || m.ToString().Contains(RegexStr4)) && Regex.IsMatch(m.Name[0].ToString(), "[A-Z]"))
                {
                    FastInvoke.FastInvokeHandler fastInvoker = FastInvoke.GetMethodInvoker(m);

                    if (m.GetParameters().Length == 0)
                    {
                        Action act = () =>
                        {
                            fastInvoker(obj, null);
                        };
                        
                        if (tempd.ContainsKey(m.Name))
                            Debug.WriteLine("■■■找到重复键：" + m.Name + "。已采用默认值。");
                        else
                            tempd.Add(m.Name, act);
                    }
                    else
                    {
                        Action<object> act2 = (param) =>
                        {
                            object[] objs = new object[1];
                            objs[0] = param;
                            fastInvoker(obj, objs);
                        };
                       

                        if (tempd.ContainsKey(m.Name))
                            Debug.WriteLine("■■■找到重复键：" + m.Name + "。已采用默认值。");
                        else
                            tempd.Add(m.Name, act2);
                    }
                }
            }
            return tempd;
        }

        private const string RegexStr1 = "DataSource";
        private const string RegexStr2 = "Proxy";//
        private const string RegexStr3 = "Void";
        private const string RegexStr4 = "System.Threading.Tasks.Task";
    }

    internal class DataSourceExt
    {
        [ImportMany(MEF.Contracts.Data)]
        public IEnumerable<Lazy<object, IDictionary<string, object>>> AllData { set; get; }

        public object data(object key)
        {
            return MEF.lookup(this.AllData, key, null);
        }
    }

    internal static class DataModule
    {
        public static DataSourceExt allDatas = MEF.compose<DataSourceExt>(new DataSourceExt());
        public static Dictionary<string, object> allMethods = new Dictionary<string, object>();
        public static List<string> keys = new List<string>();
    }

    public class DataExtension : MarkupExtension
    {
        private string Key;

        private object getData(object key)
        {
            return DataModule.allDatas.data(key);
        }

        public DataExtension(object key)
        {
            this.Key = key.ToString();
        }

        public override object ProvideValue(IServiceProvider provider)
        {
            if (DataModule.allDatas.data(Key) == null)
                return null;
            if (!DataModule.keys.Contains(Key))
            {
                DataModule.keys.Add(Key);
                foreach (var temp in RefelectionExtension.getMethods(DataModule.allDatas.data(Key)))
                {
                    if (DataModule.allMethods.ContainsKey(temp.Key))
                    {
                        Debug.WriteLine("■■■找到重复键：" + temp.Key + "。已采用默认值。");
                    }
                    else
                    {
                        DataModule.allMethods.Add(temp.Key, temp.Value);
                    }
                }
            }
            return DataModule.allDatas.data(Key);
        }
    }

    internal class ExtensionTool
    {
        public static object getDataContext(IServiceProvider provider)
        {
            return typeof(FrameworkElement).IsAssignableFrom(((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetObject.GetType()) ? ((FrameworkElement)((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetObject).DataContext : null;
        }

        public static Type getDestinationType(IServiceProvider provider)
        {
            switch (((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty.GetType().Name)
            {
                case "PropertyInfo": return (((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty as PropertyInfo).PropertyType;
                case "DependencyProperty": return (((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty as DependencyProperty).PropertyType;
                case "EventInfo": return (((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty as EventInfo).EventHandlerType;
                case "RuntimeEventInfo": return (((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty as EventInfo).EventHandlerType;
                case "MethodInfo":
                    if (new Regex("[Add|Remove].+Handler").IsMatch((((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty as MethodInfo).Name))
                    {
                        if ((((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty as MethodInfo).GetParameters().Length == 2)
                        {
                            if (typeof(MulticastDelegate).IsAssignableFrom((((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty as MethodInfo).GetParameters()[1].ParameterType))
                            {
                                return (((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty as MethodInfo).GetParameters()[1].ParameterType;
                            }
                            else
                                return null;
                        }
                        else
                            return null;
                    }
                    else
                        return null;

                case "RuntimeMethodInfo":
                    if (new Regex("[Add|Remove].+Handler").IsMatch((((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty as MethodInfo).Name))
                    {
                        if ((((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty as MethodInfo).GetParameters().Length == 2)
                        {
                            if (typeof(MulticastDelegate).IsAssignableFrom((((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty as MethodInfo).GetParameters()[1].ParameterType))
                            {
                                return (((IProvideValueTarget)provider.GetService(typeof(IProvideValueTarget))).TargetProperty as MethodInfo).GetParameters()[1].ParameterType;
                            }
                            else
                                return null;
                        }
                        else
                            return null;
                    }
                    else
                        return null;
            }

            return null;
        }

        public static object createDelegate(Type handlerType, Action<object, object> handler)
        {
            if (handlerType == null)
            {
                return null;
            }
            List<ParameterExpression> pe = new List<ParameterExpression>();
            for (int i = 0; i < handlerType.GetMethod("Invoke").GetParameters().Length; i++)
            {
                pe.Add(System.Linq.Expressions.Expression.Parameter(handlerType.GetMethod("Invoke").GetParameters()[i].ParameterType));
            }
            List<System.Linq.Expressions.Expression> exp = new List<System.Linq.Expressions.Expression>();
            exp.Add(pe[0]);
            exp.Add(System.Linq.Expressions.Expression.Convert(pe[1], typeof(object)));
            return Delegate.CreateDelegate(handlerType, System.Linq.Expressions.Expression.Lambda(System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(new Action<object, object>(handler)), typeof(Action<object, object>).GetMethod("Invoke"), exp), pe).Compile(), "Invoke");
        }

        public static object createAction(IServiceProvider provider, Action method)
        {
            string s = ExtensionTool.getDestinationType(provider).Name;
            switch (ExtensionTool.getDestinationType(provider).Name)
            {
                case "ICommand":
                    return (new RelayCommand<object>(param => method()));

                case "Delegate":
                    return ExtensionTool.createDelegate(ExtensionTool.getDestinationType(provider), (object sender, object args) => method());

                case "EventHandler":
                    return ExtensionTool.createDelegate(ExtensionTool.getDestinationType(provider), (object sender, object args) => method());

                case "MouseButtonEventHandler":
                    return ExtensionTool.createDelegate(ExtensionTool.getDestinationType(provider), (object sender, object args) => method());

                case "RoutedEventHandler":
                    return ExtensionTool.createDelegate(ExtensionTool.getDestinationType(provider), (object sender, object args) => method());

                default:
                    return null;
            }
        }

        public static object createActionWithParam(IServiceProvider provider, Action<object> method)
        {
            switch (ExtensionTool.getDestinationType(provider).Name)
            {
                case "ICommand":
                    return (new RelayCommand<object>(param => method(param)));

                default:
                    return null;
            }
        }

        public static object createEventAction(IServiceProvider provider, Action<object> method)
        {
            switch (ExtensionTool.getDestinationType(provider).Name)
            {
                case "ICommand":
                    return null;

                case "Delegate":
                    return ExtensionTool.createDelegate(ExtensionTool.getDestinationType(provider), (object sender, object args) => method(args));

                default:
                    return null;
            }
        }

        //private static bool isexedinit = false;

        //private static ActionExt rslt = (ActionExt)MEF.compose(new ActionExt());

        //public static ActionExt actions()
        //{
        //    if (!isexedinit)
        //    {
        //        isexedinit = true;
        //        foreach (var import in rslt.Initializes)
        //        {
        //            if (((object)import.Value) == null)
        //            {
        //                Debug.WriteLine("■■■Initialize导出项为null。请尝试为该导出设置独立的__Export导出类。");
        //            }
        //            else
        //            {
        //                import.Value();
        //            }
        //        }
        //    }
        //    else
        //    { }
        //    return rslt;
        //}
    }

    public class ActionAutoExtension : MarkupExtension
    {
        private string Key;

        public ActionAutoExtension(object key)
        {
            Key = key.ToString();
        }

        public override object ProvideValue(IServiceProvider provider)
        {
            Action execute;
            if (DataModule.allMethods.ContainsKey(Key))
            {
                execute = (Action)DataModule.allMethods[Key];
            }
            else
            {
                Debug.WriteLine("■■■未找到键：" + Key + "。已采用默认值。");
                execute = null;
            }

            if (execute == null)
                return null;
            return ExtensionTool.createAction(provider, execute);
        }
    }

    public class ActionAutoWithParamExtension : MarkupExtension
    {
        private string Key;

        public ActionAutoWithParamExtension(object key)
        {
            Key = key.ToString();
        }

        public override object ProvideValue(IServiceProvider provider)
        {
            Action<object> executeWithParam;
            if (DataModule.allMethods.ContainsKey(Key))
            {
                executeWithParam = (Action<object>)DataModule.allMethods[Key];
            }
            else
            {
                Debug.WriteLine("■■■未找到键：" + Key + "。已采用默认值。");
                executeWithParam = null;
            }

            if (executeWithParam == null)
                return null;
            return ExtensionTool.createActionWithParam(provider, executeWithParam);
        }
    }

    internal class ActionMessageExtensionTool
    {
        public static ActionMessageExt rslt = MEF.compose<ActionMessageExt>(new ActionMessageExt());
    }

    internal class ActionMessageExt
    {
        [ImportMany(MEF.Contracts.ActionMessage)]
        public IEnumerable<Lazy<Action, IDictionary<string, object>>> Executes { set; get; }

        public Action execute(object key)
        {
            return (Action)MEF.lookup(this.Executes, key, null);
        }
    }

    public static class ActionMessages
    {
        public static object GetAction(string key)
        {
            var execute = ActionMessageExtensionTool.rslt.execute(key);
            return execute ?? null;
        }

        public static void ExecuteAction(string key)
        {
            ActionMessageExtensionTool.rslt.execute(key).Invoke();
        }
    }
}