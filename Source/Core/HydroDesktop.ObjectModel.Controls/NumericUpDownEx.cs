using System.Windows.Forms;

namespace HydroDesktop.ObjectModel.Controls
{
    /// <summary>
    /// NumericUpDown with fixed ReadOnly behavior
    /// </summary>
    public class NumericUpDownEx : NumericUpDown
    {
        /// <summary>
        /// Create new instance of <see cref="NumericUpDownEx"/>
        /// </summary>
        public NumericUpDownEx()
        {
            Maximum = decimal.MaxValue;
            Minimum = decimal.MinValue;
        }

        public override void DownButton()
        {
            if (FullReadOnly)
                return;
            base.DownButton();
        }

        public override void UpButton()
        {
            if (FullReadOnly)
                return;
            base.UpButton();
        }

        private bool _fullReadOnly;
        /// <summary>
        /// Gets or sets value indicating whether Value can be changed by the User
        /// </summary>
        public bool FullReadOnly
        {
            get { return _fullReadOnly; }
            set
            {
                _fullReadOnly = value;
                ReadOnly = value;
            }
        }
    }
}