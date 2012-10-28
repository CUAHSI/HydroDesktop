using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HydroDesktop.ErrorReporting.CodePlex
{
    internal class HtmlForm : HtmlElement
    {
        private List<HtmlElement> _elements = new List<HtmlElement>();

        internal HtmlForm(string text)
            : base(text)
        {
        }

        public IList<HtmlElement> Elements
        {
            get
            {
                return _elements;
            }
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var htmlElement in _elements)
            {
                if (htmlElement.Name == null) continue;
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append('&');
                }
                stringBuilder.Append(htmlElement);
            }
            return stringBuilder.ToString();
        }

        public static HtmlForm GetForm(string doc, string id)
        {
            HtmlForm htmlForm = null;
            var arrayList = new List<HtmlElement>();
            var regex = new Regex("<form\\s*([^>]*)>|<input\\s*([^>]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var matchCollection = regex.Matches(doc);
            foreach (Match match in matchCollection)
            {
                if (match.Value.StartsWith("<form", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (htmlForm != null)
                    {
                        break;
                    }
                    htmlForm = new HtmlForm(match.Value);
                    var flag = (String.Compare(htmlForm.ID, id, StringComparison.OrdinalIgnoreCase) == 0);
                    if (!flag)
                    {
                        htmlForm = null;
                    }
                }
                else
                {
                    if (htmlForm != null)
                    {
                        var value = new HtmlElement(match.Value);
                        arrayList.Add(value);
                    }
                }
            }
            if (htmlForm != null)
            {
                htmlForm._elements = new List<HtmlElement>(arrayList.Count);
                htmlForm._elements.AddRange(arrayList);
            }

            return htmlForm;
        }
    }
}