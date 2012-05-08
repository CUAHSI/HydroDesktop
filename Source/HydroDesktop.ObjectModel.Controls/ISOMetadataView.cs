using System.ComponentModel;
using System.Windows.Forms;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.ObjectModel.Controls
{
    /// <summary>
    /// View of <see cref="ISOMetadata"/>
    /// </summary>
    public partial class ISOMetadataView : UserControl
    {
        #region Fields

        private ISOMetadata _entity;
        private bool _readOnly;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="QualityControlLevelView"/>
        /// </summary>
        public ISOMetadataView()
        {
            InitializeComponent();

            if (this.IsDesignMode()) return;

            Entity = null;

            // Set bindings
            tbTopicCategory.AddBinding<TextBox, ISOMetadata>(x => x.Text, bindingSource1, x => x.TopicCategory);
            tbTitle.AddBinding<TextBox, ISOMetadata>(x => x.Text, bindingSource1, x => x.Title);
            tbAbstract.AddBinding<TextBox, ISOMetadata>(x => x.Text, bindingSource1, x => x.Abstract);
            tbProfileVersion.AddBinding<TextBox, ISOMetadata>(x => x.Text, bindingSource1, x => x.ProfileVersion);
            tblMetadataLink.AddBinding<TextBox, ISOMetadata>(x => x.Text, bindingSource1, x => x.MetadataLink);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current Entity
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISOMetadata Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
                bindingSource1.DataSource = value ?? (object)typeof(ISOMetadata);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether control is read-only.
        /// </summary>
        [Browsable(false)]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;

                tbTopicCategory.ReadOnly = value;
                tbTitle.ReadOnly = value;
                tbAbstract.ReadOnly = value;
                tbProfileVersion.ReadOnly = value;
                tblMetadataLink.ReadOnly = value;
            }
        }

        #endregion
    }
}
