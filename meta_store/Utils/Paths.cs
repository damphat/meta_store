﻿using System;
using System.Globalization;

namespace meta_store.Utils
{
    // TODO int to string cache
    internal static class Paths
    {
        public static void CheckKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{key}' is not a valid key");
            }

            if (Paths.ShouldSplit(key))
            {
                throw new ArgumentException($"'{key}' contains '/'");
            }
        }

        public static object ToPath(object v)
        {
            switch (v)
            {
                case null: return null;
                case bool b: return b ? "true" : "false";
                case string s when s == "": return "";
                case string s: return ShouldSplit(s) ? (object)Split(s) : s;
                case IConvertible ic: return ic.ToString(CultureInfo.InvariantCulture);
                default:
                    throw new NotImplementedException(v.GetType().Name);
            }
        }

        public static bool ShouldSplit(string path) => path.IndexOf('/') != -1;

        public static string[] Split(string path) => path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        public static bool IsIdentifierOrInteger(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            var c = key[0];

            if (c == '0')
            {
                return key.Length == 1;
            }
            else if (c >= '1' && c <= '9')
            {
                for (var i = 1; i < key.Length; i++)
                {
                    c = key[i];
                    if (c < '0' || c > '9')
                    {
                        return false;
                    }
                }

                return true;
            }
            else if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_')
            {
                for (var i = 1; i < key.Length; i++)
                {
                    c = key[i];
                    if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c != '_' && (c < '0' || c > '9'))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}