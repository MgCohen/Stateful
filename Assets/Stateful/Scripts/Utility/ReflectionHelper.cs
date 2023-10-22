using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Scaffold.Stateful
{
    public static class ReflectionHelper
    {
        public static List<Type> GetAllDerivedTypes(Type baseType, bool includesSource = false)
        {
            var domain = AppDomain.CurrentDomain;
            var result = domain
              .GetAssemblies()
              .SelectMany(x => x.GetTypes())
              .Where(type => !type.IsAbstract && baseType.IsAssignableFrom(type))
              .ToList();


            if (!includesSource)
            {
                result.Remove(baseType);
            }

            return result;
        }

        public static bool IsFieldOrProperty(this MemberInfo memberInfo)
        {
            return memberInfo is FieldInfo or PropertyInfo;
        }

        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }

        public static string ReadableName(this Type t)
        {
            return t switch
            {
                { IsGenericType: true } => $"{t.Name.Split('`')[0]}<{string.Join(", ", t.GetGenericArguments().Select(ReadableName))}>",
                _ => t.Name
            };
        }

        private static readonly Dictionary<Type, Func<object>> Cache = new Dictionary<Type, Func<object>>();

        public static object New(Type t)
        {
            if (Cache.TryGetValue(t, out Func<object> func)) return func.Invoke();
            Func<object> newFunc = Creator(t);
            Cache.Add(t, newFunc);
            return newFunc();

            //this function compiles a lambda expression with new T() when cached 
            //it's way faster than using Activator.CreateInstance or FormatterServices.GetUninitializedObject 

            static Func<object> Creator(Type t)
            {
                if (typeof(ScriptableObject).IsAssignableFrom(t))
                    return () => ScriptableObject.CreateInstance(t);
                if (t == typeof(string))
                    return Expression.Lambda<Func<object>>(Expression.Constant(string.Empty)).Compile();
                if (t.HasDefaultConstructor())
                    return Expression.Lambda<Func<object>>(Expression.Convert(Expression.New(t), typeof(object))).Compile();
                return () => FormatterServices.GetUninitializedObject(t);
            }
        }

        public static bool Is(this Type o, Type t)
        {
            return t.IsAssignableFrom(o);
        }
    }
}