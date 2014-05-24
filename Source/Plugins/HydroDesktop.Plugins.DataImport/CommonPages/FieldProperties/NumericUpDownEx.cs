using System.Windows.Forms;

namespace HydroDesktop.Plugins.DataImport.CommonPages
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

        /// <summary>
        /// Decrements the value of the spin box (also known as an up-down control).
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public override void DownButton()
        {
            if (FullReadOnly)
                return;
            base.DownButton();
        }

        /// <summary>
        /// Increments the value of the spin box (also known as an up-down control).
        /// </summary>
        /// <filterpriority>1</filterpriority>
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