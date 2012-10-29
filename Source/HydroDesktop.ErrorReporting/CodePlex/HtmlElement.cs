using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HydroDesktop.ErrorReporting.CodePlex
{
    internal class HtmlElement
    {
        private readonly Dictionary<string, string> _attributes = new Dictionary<string, string>();
        
        public HtmlElement(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public HtmlElement(string text)
        {
            var regex = new Regex("(\\w*)\\s*=\\s*\"([^\"]*)\"|(\\w*)\\s*=\\s*'([^\"]*)'",
                                  RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var matchCollection = regex.Matches(text);
            foreach (Match match in matchCollection)
            {
                var text2 = match.Value.Replace("\"", "").Replace("'", "");
                var num = text2.IndexOf('=');
                var key = text2.Substring(0, num);
                var value = text2.Substring(num + 1);
                _attributes[key] = value;
            }
        }

        public string ID
        {
            get { return GetValueOrDefault("id"); }
        }

        public string Name
        {
            get { return GetValueOrDefault("name"); }
            set { _attributes["name"] = value; }
        }

        public string Value
        {
            get { return GetValueOrDefault("value"); }
            set { _attributes["value"] = value; }
        }

        public string Type
        {
            get { return GetValueOrDefault("type"); }
        }

        public bool IsFile { get; set; }
        public string FileName { get; set; }
        public byte[] File { get; set; }
        public string ContentType { get; set; }

        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Value);
        }

        private string GetValueOrDefault(string name)
        {
            string value;
            return _attributes.TryGetValue(name, out value) ? value : null;
        }
    }
}