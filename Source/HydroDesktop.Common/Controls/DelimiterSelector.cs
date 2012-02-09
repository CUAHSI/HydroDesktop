using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace HydroDesktop.Common.Controls
{
    /// <summary>
    /// Delimiter selector allows to select delimiter
    /// </summary>
    public partial class DelimiterSelector : UserControl
    {
        #region Fields

        private readonly List<RadioButton> _delimiterControls;
        private string _currentDelimiter;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="DelimiterSelector"/>
        /// </summary>
        public DelimiterSelector()
        {
            InitializeComponent();

            rdoComma.Tag = ",";
            rdoTab.Tag = "\t";
            rdoSpace.Tag = " ";
            rdoPipe.Tag = "|";
            rdoSemicolon.Tag = ";";
            rdoOthers.Tag = string.Empty;

            _delimiterControls = new List<RadioButton> { rdoComma, rdoTab, rdoSpace, rdoPipe, rdoSemicolon, rdoOthers };
            foreach(var rb in _delimiterControls)
            {
                var rb1 = rb;
                rb.CheckedChanged += delegate
                                         {
                                             if (rb1.Checked)
                                             {
                                                 CurrentDelimiter = (string) rb1.Tag;
                                             }
                                         };
            }
            rdoComma.Checked = true;
        }

        #endregion

        #region Events 

        /// <summary>
        /// Raised when <see cref="CurrentDelimiter"/> changed.
        /// </summary>
        public event EventHandler CurrentDelimiterChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Current Delimiter
        /// </summary>
        [Browsable(false)]
        public string CurrentDelimiter
        {
            get { return _currentDelimiter; }
            set
            {
                _currentDelimiter = value;

                var findedButton = false;
                foreach(var rb in _delimiterControls)
                {
                    var delimiter = (string) rb.Tag;
                    if (string.Equals(delimiter, value))
                    {
                        rb.Checked = true;
                        findedButton = true;
                        break;
                    }
                }
                if (!findedButton)
                {
                    tbOther.Text = value;
                }

                RaiseCurrentDelimiterChanged();
            }
        }

        #endregion

        #region Private methods

        private void RaiseCurrentDelimiterChanged()
        {
            var handler = CurrentDelimiterChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void tbOther_TextChanged(object sender, EventArgs e)
        {
            if (tbOther.Text.Length != 0)
            {
                rdoOthers.Tag = tbOther.Text;
                CurrentDelimiter = tbOther.Text;
            }
        }

        #endregion
    }
}
