using System.Collections.Generic;
using System.Text;

namespace meta_store
{
    public class TreeLevel : ILevel
    {
        private Dictionary<string, TreeLevel> children;
        private TreeLevel parent;
        private string key;
        public object a, b;
        public Sigo sa, sb;
        public object r;
        public bool rc;
        public int action = -1;
        public string debug = "";

        public void Ret(bool rc, object r, string debug)
        {
            this.rc = rc;
            this.r = r;
            this.debug += debug;
        }

        private TreeLevel(TreeLevel parent, string key)
        {
            this.parent = parent;
            this.key = key;
            if (parent != null)
            {
                this.parent.children.Add(key, this);             
            }
            children = new Dictionary<string, TreeLevel>();
        }

        public static TreeLevel Create()
        {
            return new TreeLevel(null, null);
        }

        public TreeLevel Root
        {
            get {
                var root = this;
                while (root.parent != null)
                {
                    root = root.parent;
                }
                return root;
            }            
        }

        public TreeLevel Next(string key)
        {
            return new TreeLevel(this, key);
        }

        public TreeLevel Parent => parent;

        public override string ToString()
        {
            var ls = new List<string>();
            var t = this;
            while(t != null)
            {
                ls.Add(t.key);
                t = t.parent;
            }
            ls.Reverse();
            return string.Join("/", ls);
        }

        public string ToDebugString()
        {
            var sb = new StringBuilder();
            sb.Append(ToString());
            sb.Append(":");
            
            return Build(sb).ToString();
        }

        public StringBuilder Build(StringBuilder sb = null, int indent = 0)
        {
            sb = sb ?? new StringBuilder();
            void AppendIndent(int level)
            {
                sb.Append(' ', level * 2);
            }

            sb.Append('{');
            var first = true;
            foreach(var child in this.children)
            {
                if (first)
                {
                    sb.AppendLine();
                } else
                {
                    first = false;
                }                
                AppendIndent(indent + 1);
                switch(action)
                {
                    case -1: sb.Append(" "); break;
                    case 0: sb.Append(" "); break;
                    case 1: sb.Append("+"); break;
                    case 2: sb.Append("-"); break;
                    case 3: sb.Append("="); break;
                    default:
                        throw new System.Exception("unknown action");
                };
                sb.Append(child.Key);
                sb.Append(": ");
                child.Value.Build(sb, indent + 1);
                sb.AppendLine(",");
            }

            AppendIndent(indent);
            sb.Append('}');
            return sb;
        }

        ILevel ILevel.Root => Root;

        ILevel ILevel.Parent => Parent;

        ILevel ILevel.Next(string name) => Next(name);
    }
}
