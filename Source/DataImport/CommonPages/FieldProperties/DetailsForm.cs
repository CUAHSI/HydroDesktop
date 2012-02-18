using System;
using System.Diagnostics.Contracts;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;

namespace DataImport.CommonPages
{
    /// <summary>
    /// Allows to edit Source/Method/QualityControlLevel
    /// </summary>
    public partial class DetailsForm : Form
    {
        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="DetailsForm"/>
        /// </summary>
        public DetailsForm()
        {
            InitializeComponent();

            if (DesignMode) return;
            // Set bindings.......
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current QualityControlLevel
        /// </summary>
        public QualityControlLevel QualityControlLevel
        {
            get { return qualityControlLevelView.Entity; }
        }

        /// <summary>
        /// Current Method
        /// </summary>
        public Method Method
        {
            get { return methodView.Entity; }
        }

        /// <summary>
        /// Current Source
        /// </summary>
        public Source Source
        {
            get { return sourceView.Entity; }
        }

        #endregion
    }
}
