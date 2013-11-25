using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HydroShare
{
    public partial class inputBoxTest : Form
    {
        public inputBoxTest()
        {
            InitializeComponent();
        }

        void SetText(string attribute, string attName, string value)
        {
            HtmlElementCollection col = webBrowser1.Document.GetElementsByTagName("input");

            foreach (HtmlElement element in col)
            {
                if (element.GetAttribute(attribute).Equals(attName))
                    element.SetAttribute("value", value);
            }
        }

        void ClickButton(string attribute, string attName)
        {
            HtmlElementCollection col = webBrowser1.Document.GetElementsByTagName("input");

            foreach (HtmlElement element in col)
            {
                if (element.GetAttribute(attribute).Equals(attName))
                    element.InvokeMember("click");
            }

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // "q" is the name of the Google search textbox

            SetText("name", "q", "Dog");

            // "btnG" is the name of the Google search submit button

            ClickButton("name", "btnK");
        }
    }
}
