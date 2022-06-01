using meta_store.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace meta_store.Language
{

    // TODO recursive object: a = {x:1}; a = a * {0, a}   
    public class SigoParser
    {
        private readonly PeekableLexer lexer;
        private readonly Dictionary<string, object> global = new Dictionary<string, object>();
        private Token t;

        public SigoParser(string src)
        {
            lexer = new PeekableLexer(src, 0, 1); // Peek(0), Peek(1)
            t = lexer.Peek(0);
        }

        private string Expected(string thing) => $"{thing} expected, found {(t.Kind == Kind.Eof ? "eof" : $"'{t.Raw}'")} at {t.Start}";

        private void Next()
        {
            lexer.Move(1);
            t = lexer.Peek(0);
        }

        public object Parse() => Sigo.From(ParseStatements());

        //public ISigo Parse(ISigo input, out ISigo output) {
        //    global = input.Freeze();
        //    var ret = Parse();
        //    output = global;
        //    return ret;
        //}

        private List<object> ParseArray()
        {
            Next();
            var list = new List<object>();
            if (t.Kind == Kind.CloseBracket)
            {
                Next();
                return list;
            }

            while (true)
            {
                list.Add(ParseExpr());

                var sep = ParseObjectSeparator();

                if (t.Kind == Kind.CloseBracket)
                {
                    Next();
                    return list;
                }

                if (sep == 0)
                {
                    throw new Exception(Expected("','"));
                }
            }
        }

        private object ParseAssignment()
        {
            var key = t.Raw;
            Next();
            Next();
            var value = Sigo.From(ParseExpr());
            global[key] = value;
            return value;
        }

        private double ParseDouble()
        {
            if (t.Kind == Kind.Number)
            {
                var d = (double)t.Value;
                Next();
                return d;
            }

            if (t.Kind == Kind.Identifier)
            {
                switch (t.Raw)
                {
                    case "Infinity":
                        Next();
                        return double.PositiveInfinity;
                    case "NaN":
                        Next();
                        return double.NaN;
                }
            }

            throw new Exception(Expected("number"));
        }

        private object ParseIdentifier()
        {
            if (lexer.Peek(1).Kind == Kind.Eq)
            {
                return ParseAssignment();
            }

            var raw = t.Raw;
            switch (raw)
            {
                case "true":
                    Next();
                    return true;
                case "false":
                    Next();
                    return false;
                case "NaN":
                    Next();
                    return double.NaN;
                case "Infinity":
                    Next();
                    return double.PositiveInfinity;
                default:
                    if (global.TryGetValue(raw, out var value))
                    {
                        Next();
                        return value;
                    }

                    throw new Exception($"unexpected identifier '{raw}'");
            }
        }

        private object ParseNumber()
        {
            var ret = t.Value;
            Next();
            return ret;
        }

        private object ParseObject()
        {
            Next();
            var f = ParseObjectFlag();
            var ret = Sigo.Create(f ?? 3);

            if (f != null)
            {
                var sep = ParseObjectSeparator();
                if (t.Kind == Kind.Close)
                {
                    Next();
                    return ret;
                }

                if (sep == 0)
                {
                    throw new Exception(Expected("','"));
                }
            }
            else
            {
                if (t.Kind == Kind.Close)
                {
                    Next();
                    return ret;
                }
            }

            while (true)
            {
                var keys = ParseObjectKeys();
                if (keys == null)
                {
                    throw new Exception(Expected("key"));
                }

                if (t.Kind == Kind.Colon)
                {
                    Next();
                    var value = Sigo.From(ParseExpr());
                    ret = Sigo.Set(ret, keys, 0, value);
                }
                else
                {
                    var last = keys[keys.Count - 1];
                    if (global.TryGetValue(last, out var value))
                    {
                        ret = Sigo.Set(ret, keys, 0, value);
                    }
                    else
                    {
                        throw new Exception(Expected($"':' after {string.Join("/", keys)} "));
                    }
                }

                var sep = ParseObjectSeparator();

                if (t.Kind == Kind.Close)
                {
                    Next();
                    return ret;
                }

                if (sep == 0)
                {
                    throw new Exception(Expected("','"));
                }
            }
        }

        private int? ParseObjectFlag()
        {
            if (t.Kind != Kind.Number)
            {
                return null;
            }

            var k1 = lexer.Peek(1).Kind;
            if (k1 == Kind.Colon || k1 == Kind.Div)
            {
                return null;
            }

            var raw = t.Raw;
            if (raw.Length != 1)
            {
                return null;
            }

            var f = raw[0];

            if (f < '0' || f > '7')
            {
                return null;
            }

            Next();

            return f - '0';
        }

        // return null | string | string[]
        private object ParseObjectKey()
        {
            string key;
            if (t.Kind == Kind.Identifier)
            {
                key = t.Raw;
                Next();
                return key;
            }

            if (t.Kind == Kind.Number)
            {
                key = ((double)t.Value).ToString(CultureInfo.InvariantCulture);
                Next();
                return key;
            }

            if (t.Kind == Kind.String)
            {
                key = (string)t.Value;
                Next();
                if (Paths.ShouldSplit(key))
                {
                    return Paths.Split(key);
                }

                return key;
            }

            return null;
        }

        // TODO gc problem
        private List<string> ParseObjectKeys()
        {
            var key = ParseObjectKey();
            if (key == null)
            {
                return null;
            }

            var keys = new List<string>();
            if (key is string s)
            {
                if (s != "")
                {
                    keys.Add(s);
                }
            }
            else if (key is string[] ss)
            {
                keys.AddRange(ss);
            }

            while (t.Kind == Kind.Div)
            {
                Next();
                key = ParseObjectKey();
                if (key == null)
                {
                    throw new Exception(Expected("key"));
                }

                if (key is string @string)
                {
                    keys.Add(@string);
                }
                else if (key is string[] ss)
                {
                    keys.AddRange(ss);
                }
            }

            return keys;
        }

        private int ParseObjectSeparator()
        {
            if (t.Kind == Kind.Comma || t.Kind == Kind.SemiColon)
            {
                Next();
                return 4;
            }

            return t.Separator;
        }

        private object ParseStatements()
        {
            object ret = null;
            while (true)
            {
                if (t.Kind == Kind.Eof)
                {
                    return ret;
                }

                ret = ParseExpr();
                if (t.Kind == Kind.SemiColon)
                {
                    Next();
                }
            }
        }

        private object ParseString()
        {
            var ret = t.Value;
            Next();
            return ret;
        }

        private object ParseUnary()
        {
            var k = t.Kind;
            Next();
            var d = ParseDouble();
            return k == Kind.Minus ? -d : d;
        }

        private object ParseMemberAccess()
        {
            var left = ParseValue();

            while (t.Kind == Kind.Div)
            {
                Next();
                if (t.Kind == Kind.Identifier)
                {
                    var key = t.Raw;
                    Next();
                    left = Sigo.Get1(left, key);

                }
                else if (t.Kind == Kind.Number)
                {
                    var key = t.Raw;
                    Next();
                    left = Sigo.Get1(left, key);
                }
                else
                {
                    throw new Exception(Expected("indentifier or int"));
                }
            }

            return left;
        }

        private object ParseExpr()
        {
            var left = ParseMemberAccess();

            while (t.Kind == Kind.Mul)
            {
                Next();
                var right = ParseValue();
                left = Sigo.Merge(left, right);
            }

            return left;
        }

        private object ParseParens()
        {
            Next();

            var value = ParseExpr();

            if (t.Kind == Kind.CloseParens)
            {
                Next();
                return value;
            }
            else
            {
                throw new Exception(Expected(")"));
            }
        }

        private object ParseValue()
        {
            switch (t.Kind)
            {
                case Kind.Plus:
                case Kind.Minus: return ParseUnary();
                case Kind.Number: return ParseNumber();
                case Kind.String: return ParseString();
                case Kind.Open: return ParseObject();
                case Kind.OpenBracket: return ParseArray();
                case Kind.Identifier: return ParseIdentifier();
                case Kind.OpenParens: return ParseParens();
                default:
                    throw new Exception(Expected("value"));
            }
        }
    }
}