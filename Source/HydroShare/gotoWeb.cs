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
    public partial class gotoWeb : Form
    {
        public gotoWeb()
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

        private void webBrowser1_DocumentCompleted_1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // "q" is the name of the Google search textbox

            SetText("name", "username", "martinn");
            SetText("name", "password", "");
            passwordEntry psw = new passwordEntry();
            psw.StartPosition = FormStartPosition.CenterScreen;
            psw.Visible = true;
            SetText("name", "password", psw.password);
            //ClickButton("type", "submit");
            
            //SetText("name", "search", "dog");
            // "btnK" is the name of the Google search submit button
            // "go" is the name of the Wiki search submit button
            //ClickButton("name", "go");
            //this.Close();
            return;
        }
    }
}