using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MA.Dao.Entity.Linq
{
    internal static class ReflectionUtils
    {
        public static readonly Type[] EmptyTypes;

        static ReflectionUtils()
        {
            EmptyTypes = Type.EmptyTypes;
        }
    }

    internal class MAEntityLinqReflection
    {
        private static DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(name, returnType, parameterTypes, owner, true);

            return dynamicMethod;
        }

        public delegate object ObjectConstructor<T>(params object[] args);
        public static ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method)
        {
            DynamicMethod dynamicMethod = CreateDynamicMethod(method.ToString(), typeof(object), new[] { typeof(object[]) }, method.DeclaringType);
            ILGenerator generator = dynamicMethod.GetILGenerator();

            GenerateCreateMethodCallIL(method, generator, 0);

            return (ObjectConstructor<object>)dynamicMethod.CreateDelegate(typeof(ObjectConstructor<object>));
        }


        private static void GenerateCreateMethodCallIL(MethodBase method, ILGenerator generator, int argsIndex)
        {
            ParameterInfo[] args = method.GetParameters();

            Label argsOk = generator.DefineLabel();

            // throw an error if the number of argument values doesn't match method parameters
            generator.Emit(OpCodes.Ldarg, argsIndex);
            generator.Emit(OpCodes.Ldlen);
            generator.Emit(OpCodes.Ldc_I4, args.Length);
            generator.Emit(OpCodes.Beq, argsOk);
            generator.Emit(OpCodes.Newobj, typeof(TargetParameterCountException).GetConstructor(ReflectionUtils.EmptyTypes));
            generator.Emit(OpCodes.Throw);

            generator.MarkLabel(argsOk);

            if (!method.IsConstructor && !method.IsStatic)
            {
                generator.PushInstance(method.DeclaringType);
            }

            LocalBuilder localConvertible = generator.DeclareLocal(typeof(IConvertible));
            LocalBuilder localObject = generator.DeclareLocal(typeof(object));

            for (int i = 0; i < args.Length; i++)
            {
                ParameterInfo parameter = args[i];
                Type parameterType = parameter.ParameterType;

                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();

                    LocalBuilder localVariable = generator.DeclareLocal(parameterType);

                    // don't need to set variable for 'out' parameter
                    if (!parameter.IsOut)
                    {
                        generator.PushArrayInstance(argsIndex, i);

                        if (parameterType.IsValueType)
                        {
                            Label skipSettingDefault = generator.DefineLabel();
                            Label finishedProcessingParameter = generator.DefineLabel();

                            // check if parameter is not null
                            generator.Emit(OpCodes.Brtrue_S, skipSettingDefault);

                            // parameter has no value, initialize to default
                            generator.Emit(OpCodes.Ldloca_S, localVariable);
                            generator.Emit(OpCodes.Initobj, parameterType);
                            generator.Emit(OpCodes.Br_S, finishedProcessingParameter);

                            // parameter has value, get value from array again and unbox and set to variable
                            generator.MarkLabel(skipSettingDefault);
                            generator.PushArrayInstance(argsIndex, i);
                            generator.UnboxIfNeeded(parameterType);
                            generator.Emit(OpCodes.Stloc_S, localVariable);

                            // parameter finished, we out!
                            generator.MarkLabel(finishedProcessingParameter);
                        }
                        else
                        {
                            generator.UnboxIfNeeded(parameterType);
                            generator.Emit(OpCodes.Stloc_S, localVariable);
                        }
                    }

                    generator.Emit(OpCodes.Ldloca_S, localVariable);
                }
                else if (parameterType.IsValueType)
                {
                    generator.PushArrayInstance(argsIndex, i);
                    generator.Emit(OpCodes.Stloc_S, localObject);

                    // have to check that value type parameters aren't null
                    // otherwise they will error when unboxed
                    Label skipSettingDefault = generator.DefineLabel();
                    Label finishedProcessingParameter = generator.DefineLabel();

                    // check if parameter is not null
                    generator.Emit(OpCodes.Ldloc_S, localObject);
                    generator.Emit(OpCodes.Brtrue_S, skipSettingDefault);

                    // parameter has no value, initialize to default
                    LocalBuilder localVariable = generator.DeclareLocal(parameterType);
                    generator.Emit(OpCodes.Ldloca_S, localVariable);
                    generator.Emit(OpCodes.Initobj, parameterType);
                    generator.Emit(OpCodes.Ldloc_S, localVariable);
                    generator.Emit(OpCodes.Br_S, finishedProcessingParameter);

                    // argument has value, try to convert it to parameter type
                    generator.MarkLabel(skipSettingDefault);

                    if (parameterType.IsPrimitive)
                    {
                        // for primitive types we need to handle type widening (e.g. short -> int)
                        MethodInfo toParameterTypeMethod = typeof(IConvertible)
                            .GetMethod("To" + parameterType.Name, new[] { typeof(IFormatProvider) });

                        if (toParameterTypeMethod != null)
                        {
                            Label skipConvertible = generator.DefineLabel();

                            // check if argument type is an exact match for parameter type
                            // in this case we may use cheap unboxing instead
                            generator.Emit(OpCodes.Ldloc_S, localObject);
                            generator.Emit(OpCodes.Isinst, parameterType);
                            generator.Emit(OpCodes.Brtrue_S, skipConvertible);

                            // types don't match, check if argument implements IConvertible
                            generator.Emit(OpCodes.Ldloc_S, localObject);
                            generator.Emit(OpCodes.Isinst, typeof(IConvertible));
                            generator.Emit(OpCodes.Stloc_S, localConvertible);
                            generator.Emit(OpCodes.Ldloc_S, localConvertible);
                            generator.Emit(OpCodes.Brfalse_S, skipConvertible);

                            // convert argument to parameter type
                            generator.Emit(OpCodes.Ldloc_S, localConvertible);
                            generator.Emit(OpCodes.Ldnull);
                            generator.Emit(OpCodes.Callvirt, toParameterTypeMethod);
                            generator.Emit(OpCodes.Br_S, finishedProcessingParameter);

                            generator.MarkLabel(skipConvertible);
                        }
                    }

                    // we got here because either argument type matches parameter (conversion will succeed),
                    // or argument type doesn't match parameter, but we're out of options (conversion will fail)
                    generator.Emit(OpCodes.Ldloc_S, localObject);

                    generator.UnboxIfNeeded(parameterType);

                    // parameter finished, we out!
                    generator.MarkLabel(finishedProcessingParameter);
                }
                else
                {
                    generator.PushArrayInstance(argsIndex, i);

                    generator.UnboxIfNeeded(parameterType);
                }
            }

            if (method.IsConstructor)
            {
                generator.Emit(OpCodes.Newobj, (ConstructorInfo)method);
            }
            else
            {
                generator.CallMethod((MethodInfo)method);
            }

            Type returnType = method.IsConstructor
                ? method.DeclaringType
                : ((MethodInfo)method).ReturnType;

            if (returnType != typeof(void))
            {
                generator.BoxIfNeeded(returnType);
            }
            else
            {
                generator.Emit(OpCodes.Ldnull);
            }

            generator.Return();
        }
    }

    internal static class ILGeneratorExtensions
    {
        public static void PushInstance(this ILGenerator generator, Type type)
        {
            generator.Emit(OpCodes.Ldarg_0);
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Unbox, type);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, type);
            }
        }

        public static void PushArrayInstance(this ILGenerator generator, int argsIndex, int arrayIndex)
        {
            generator.Emit(OpCodes.Ldarg, argsIndex);
            generator.Emit(OpCodes.Ldc_I4, arrayIndex);
            generator.Emit(OpCodes.Ldelem_Ref);
        }

        public static void BoxIfNeeded(this ILGenerator generator, Type type)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Box, type);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, type);
            }
        }

        public static void UnboxIfNeeded(this ILGenerator generator, Type type)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, type);
            }
        }

        public static void CallMethod(this ILGenerator generator, MethodInfo methodInfo)
        {
            if (methodInfo.IsFinal || !methodInfo.IsVirtual)
            {
                generator.Emit(OpCodes.Call, methodInfo);
            }
            else
            {
                generator.Emit(OpCodes.Callvirt, methodInfo);
            }
        }

        public static void Return(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Ret);
        }
    }
}
