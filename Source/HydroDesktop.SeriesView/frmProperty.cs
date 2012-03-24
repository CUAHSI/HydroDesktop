using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace SeriesView
{
    public partial class frmProperty : Form
    {
        // Fields
        //private IContainer components = null;
        //private PropertyGrid proGridSeries1;
        private DataRow currentRow;

        // Methods
        public frmProperty()
        {
            InitializeComponent();

            Load +=frmProperty_Load;
        }

        public frmProperty(DataRow propertyRow)
        {
            InitializeComponent();
            currentRow = propertyRow;
            Load +=frmProperty_Load;
        }


        private void frmProperty_Load(object sender, EventArgs e)
        {
            if (currentRow == null) return;
            
            try
            { 
                var dataValues = new SeriesInfo();
               
                //Sites: SiteCode, SiteName
                dataValues.SiteCode = currentRow["SiteCode"].ToString();
                dataValues.SiteName = currentRow["SiteName"].ToString();
                //Variables: VariableCode, VariableName, Speciation, (VariableUnitsName), 
                //SampleMedium, ValueType, TimeSupport, (TimeUnitsName),
                //DataType, GeneralCategory, NoDataValue
                dataValues.VariableCode = currentRow["VariableCode"].ToString();
                dataValues.VariableName = currentRow["VariableName"].ToString();
                dataValues.Speciation = currentRow["Speciation"].ToString();
                dataValues.VariableUnitsName = currentRow["VariableUnitsName"].ToString();
                dataValues.SampleMedium = currentRow["SampleMedium"].ToString();
                dataValues.ValueType = currentRow["ValueType"].ToString();
                dataValues.TimeSupport = currentRow["TimeSupport"].ToString();
                dataValues.TimeUnitsName = currentRow["TimeUnitsName"].ToString();
                dataValues.DataType = currentRow["DataType"].ToString();
                dataValues.GeneralCategory = currentRow["GeneralCategory"].ToString();
                dataValues.NoDataValue = currentRow["NoDataValue"].ToString();
                //Methods: MethodDescription
                dataValues.MethodDescription = currentRow["MethodDescription"].ToString();
                //Sources: Organization, SourceDescription, Citation 
                dataValues.Organization = currentRow["Organization"].ToString();
                dataValues.SourceDescription = currentRow["SourceDescription"].ToString();
                dataValues.Citation = currentRow["Citation"].ToString();
                //QualityControlLevels: QualityControlLevelCode
                dataValues.QualityControlLevelCode = currentRow["QualityControlLevelCode"].ToString();
                //DataSeries: BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ValueCount 
                //Convert.ToDateTime(seriesRow[1].ToString());
                dataValues.BeginDateTime = Convert.ToDateTime(currentRow["BeginDateTime"].ToString());
                dataValues.EndDateTime = Convert.ToDateTime(currentRow["EndDateTime"].ToString());
                dataValues.BeginDateTimeUTC = Convert.ToDateTime(currentRow["BeginDateTimeUTC"].ToString());
                dataValues.EndDateTimeUTC = Convert.ToDateTime(currentRow["EndDateTimeUTC"].ToString());
                dataValues.ValueCount =Convert.ToInt32( currentRow["ValueCount"].ToString());
                
                proGridSeries1.SelectedObject = dataValues;
                DialogResult = DialogResult.OK;
            }
            catch (Exception cy)
            {
                MessageBox.Show(cy.Message, "Failed Query", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                DialogResult = DialogResult.No;
            }

        }

        // Nested Types
        [DefaultProperty("Detailed Property")]
        public class SeriesInfo
        {
            /* Sites: SiteCode, SiteName,
             * 
             * Variables: VariableCode, VariableName, Speciation, (VariableUnitsName), 
             *            SampleMedium, ValueType, TimeSupport, (TimeUnitsName),
             *            DataType, GeneralCategory, NoDataValue
             *            
             * Methods: MethodDescription,     
             * 
             * Sources: Organization, SourceDescription, Citation 
             * 
             * QualityControlLevels: QualityControlLevelCode
             * 
             * DataSeries: BeginDateTime, EndDateTime, BeginDateTimeUTC, EndDateTimeUTC, ValueCount 
             */

            #region Fields

            #endregion
            
            #region Sites: SiteCode, SiteName

            [Category("Sites"), Description("SiteCode")]
            public string SiteCode { get; set; }

            [Category("Sites"), Description("SiteName")]
            public string SiteName { get; set; }

            #endregion

            #region Variables

            [Category("Variables"), Description("VariableCode")]
            public string VariableCode { get; set; }

            [Category("Variables"), Description("VariableName")]
            public string VariableName { get; set; }

            [Category("Variables"), Description("Speciation")]
            public string Speciation { get; set; }

            [Category("Variables"), Description("VariableUnitsName")]
            public string VariableUnitsName { get; set; }

            [Category("Variables"), Description("SampleMedium")]
            public string SampleMedium { get; set; }

            [Category("Variables"), Description("ValueType")]
            public string ValueType { get; set; }

            [Category("Variables"), Description("TimeSupport")]
            public string TimeSupport { get; set; }

            [Category("Variables"), Description("TimeUnitsName")]
            public string TimeUnitsName { get; set; }

            [Category("Variables"), Description("DataType")]
            public string DataType { get; set; }

            [Category("Variables"), Description("GeneralCategory")]
            public string GeneralCategory { get; set; }

            [Category("Variables"), Description("NoDataValue")]
            public string NoDataValue { get; set; }

            #endregion

            #region Methods

            [Category("Methods"), Description("MethodDescription")]
            public string MethodDescription { get; set; }

            #endregion

            #region Sources

            [Category("Sources"), Description("Organization")]
            public string Organization { get; set; }

            [Category("Sources"), Description("SourceDescription")]
            public string SourceDescription { get; set; }

            [Category("Sources"), Description("Citation")]
            public string Citation { get; set; }

            #endregion

            #region QualityControlLevels

            [Category("Variables"), Description("QualityControlLevelCode")]
            public string QualityControlLevelCode { get; set; }

            #endregion

            #region DataSeries

            [Description("The BeginDateTime of this Series"), Category("DataSeries")]
            public DateTime BeginDateTime { get; set; }

            [Description("The BeginDateTimeUTC of this Series"), Category("DataSeries")]
            public DateTime BeginDateTimeUTC { get; set; }

            [Description("The EndDateTime of this Series"), Category("DataSeries")]
            public DateTime EndDateTime { get; set; }

            [Description("The EndDateTimeUTC of this Series"), Category("DataSeries")]
            public DateTime EndDateTimeUTC { get; set; }

            [Category("DataSeries"), Description("The Count of Values that Series contains")]
            public int ValueCount { get; set; }

            #endregion
        }
    }
}