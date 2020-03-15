﻿using System;
using System.Reflection;

namespace ConsoleLoop
{
    internal static class Changes
    {
        internal static string Between(object obj1, object obj2, string path = "")
        {
            string path1 = string.IsNullOrEmpty(path) ? "" : path + ": ";
            if (obj1 == null && obj2 != null)
                return path1 + "null != not null";
            else if (obj2 == null && obj1 != null)
                return path1 + "not null != null";
            else if (obj1 == null && obj2 == null)
                return null;

            if (!obj1.GetType().Equals(obj2.GetType()))
                return "different types: " + obj1.GetType() + " and " + obj2.GetType();

            Type type = obj1.GetType();
            if (string.IsNullOrEmpty(path))
                path = type.Name;

            if (type.IsPrimitive || typeof(string).Equals(type))
            {
                if (!obj1.Equals(obj2))
                    return path1 + "'" + obj1 + "' != '" + obj2 + "'";
                return null;
            }
            if (type.IsArray)
            {
                Array first = obj1 as Array;
                Array second = obj2 as Array;
                if (first.Length != second.Length)
                    return path1 + "array size differs (" + first.Length + " vs " + second.Length + ")";

                var en = first.GetEnumerator();
                int i = 0;
                while (en.MoveNext())
                {
                    string res = Between(en.Current, second.GetValue(i), path);
                    if (res != null)
                        return res + " (Index " + i + ")";
                    i++;
                }
            }
            else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
            {
                System.Collections.IEnumerable first = obj1 as System.Collections.IEnumerable;
                System.Collections.IEnumerable second = obj2 as System.Collections.IEnumerable;


                var en = first.GetEnumerator();
                var en2 = second.GetEnumerator();
                int i = 0;
                while (true)
                {
                    var firstMoveNext = en.MoveNext();
                    var secondMoveNext = en2.MoveNext();

                    if (firstMoveNext == false && secondMoveNext == false)
                        break;

                    if (firstMoveNext ^ secondMoveNext)
                        return path + ": enumerable size differs";

                    string res = Between(en.Current, en2.Current, path);
                    if (res != null)
                        return res + " (Index " + i + ")";
                    i++;
                }
            }
            else
            {
                foreach (PropertyInfo pi in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
                {
                    try
                    {
                        var val = pi.GetValue(obj1);
                        var tval = pi.GetValue(obj2);
                        var pathNew = (path.Length == 0 ? "" : path + ".") + pi.Name;
                        string res = Between(val, tval, pathNew);
                        if (res != null)
                            return res;
                    }
                    catch (TargetParameterCountException)
                    {
                        //index property
                    }
                }
                foreach (FieldInfo fi in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
                {
                    var val = fi.GetValue(obj1);
                    var tval = fi.GetValue(obj2);
                    var pathNew = (path.Length == 0 ? "" : path + ".") + fi.Name;
                    string res = Between(val, tval, pathNew);
                    if (res != null)
                        return res;
                }
            }
            return null;
        }
    }
}
