using System;
using System.Drawing;
using System.Windows.Forms;

namespace HydroR.Controls
{
    public partial class REditor : RichTextBox
    {
        private int line = 0;

        public int Line
        {
            get { return line; }
            set { line = value; }
        }
        private int highlightlength;

        public int Highlightlength
        {
            get { return highlightlength; }
            set { highlightlength = value; }
        }
        private bool generateCode;

        public bool GenerateCode
        {
            get { return generateCode; }
            set { generateCode = value; }
        }
        private SyntaxColoring syn = new SyntaxColoring();
        public REditor()
        {
            InitializeComponent();
            transp.Changed += new cHighlight.EventHandler(highlighterClick);
        }
        public void SelectLine(int lineNumber)
        {
            //if (isNewLine(lineNumber))
            if(lineNumber>=0)
            {
                Controls.Remove(transp);

                if (lineNumber < Lines.Length && lineNumber>=0)
                {
                    highlightlength = Math.Max(Math.Max(Width, Lines[lineNumber].Length * Font.Height), highlightlength);
                }
                else { highlightlength = Math.Max(Width, highlightlength); }
                currLine = lineNumber;
                transp.Location = GetPositionFromCharIndex(GetFirstCharIndexFromLine(lineNumber));
                transp.Size = new Size(highlightlength, Font.Height + 5);
                
                Controls.Add(transp);
            }
        }
        int currLine;
        private bool isNewLine(int l)
        {           
            
            if (currLine != l)
                return true;
            else return false;

        }
       
        
        private void highlighterClick(Point Location)
        {
            Select(this.GetCharIndexFromPosition(new Point(Location.X, GetPositionFromCharIndex(this.SelectionStart).Y)), 0);           
        }
        private void rTextChanged(object sender, EventArgs e)
        {
            if (!generateCode)
            {
                int p = SelectionStart;
                syn.TextChange(this, GetLineFromCharIndex(p));
                SelectionStart = p;
                SelectLine(line);
            }
        }
        private void rMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                linechanged(new Point(e.X, e.Y));
        }
        private void rKeyUp(object sender, KeyEventArgs e)
        {
            int p = SelectionStart;
            if (line != GetLineFromCharIndex(p))
            {
                Point po = GetPositionFromCharIndex(p);
                linechanged(new Point(po.X, po.Y));
            }
        }
        private void linechanged(Point p)
        {
            if (SelectionLength == 0)
            {
                //set current line by the selected line
                int position = GetCharIndexFromPosition(p);
                line = GetLineFromCharIndex(position);
                SelectLine(line);
                Select(position, 0);
            }
        }        
        private void Scroll(object sender, EventArgs e)
        {
            SelectLine(line);
        }
       public void TextChange()
       {
           syn.TextChange(this);
       }        
       public void TextChange(int editline)
       {
            syn.TextChange(this, editline);
       }
       public void TextChange(int start, int end)
       {
            syn.TextChanged(this, start, end);
       }

    }
}
