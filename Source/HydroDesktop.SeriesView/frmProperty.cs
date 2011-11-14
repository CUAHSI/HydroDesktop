using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Database;

namespace SeriesView
{
    public partial class frmProperty : Form
    {
        // Fields
        //private IContainer components = null;
        //private PropertyGrid proGridSeries1;
        private DataRow currentRow = null;

        // Methods
        public frmProperty()
        {
            this.InitializeComponent();

            this.Load +=new EventHandler(frmProperty_Load);
        }

        public frmProperty(DataRow propertyRow)
        {
            InitializeComponent();
            currentRow = propertyRow;
            this.Load +=new EventHandler(frmProperty_Load);
        }


        private void frmProperty_Load(object sender, EventArgs e)
        {
            if (currentRow == null) return;
            
            try
            { 
                SeriesInfo dataValues = new SeriesInfo();
               
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
                
                this.proGridSeries1.SelectedObject = dataValues;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception cy)
            {
                MessageBox.Show(cy.Message, "Failed Query", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, false);
                this.DialogResult = DialogResult.No;
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
            private string _siteCode;//
            private string _siteName;//

            private string _variableCode;//
            private string _variableName;//
            private string _speciation;//
            private string _variableUnitsName;//
            private string _sampleMedium;//
            private string _valueType;//
            private string _timeSupport;//
            private string _timeUnitsName;//
            private string _dataType;//
            private string _generalCategory;//
            private string _noDataValue;//

            private string _methodDescription;//

            private string _organization;//
            private string _sourceDescription;//
            private string _citation;//

            private string _qualityControlLevelCode;//

            private DateTime _beginDateTime;
            private DateTime _beginDateTimeUTC;
            private DateTime _endDateTime;
            private DateTime _endDateTimeUTC;
            private int _valueCount;
            #endregion
            
            #region Sites: SiteCode, SiteName
            [Category("Sites"), Description("SiteCode")]
            public string SiteCode
            {
                get
                {
                    return this._siteCode;
                }
                set
                {
                    this._siteCode = value;
                }
            }
            [Category("Sites"), Description("SiteName")]
            public string SiteName
            {
                get
                {
                    return this._siteName;
                }
                set
                {
                    this._siteName = value;
                }
            }
            #endregion

            #region Variables
            [Category("Variables"), Description("VariableCode")]
            public string VariableCode
            {
                get
                {
                    return this._variableCode;
                }
                set
                {
                    this._variableCode = value;
                }
            }
            [Category("Variables"), Description("VariableName")]
            public string VariableName
            {
                get
                {
                    return this._variableName;
                }
                set
                {
                    this._variableName = value;
                }
            }
            [Category("Variables"), Description("Speciation")]
            public string Speciation
            {
                get
                {
                    return this._speciation;
                }
                set
                {
                    this._speciation = value;
                }
            }
            [Category("Variables"), Description("VariableUnitsName")]
            public string VariableUnitsName
            {
                get
                {
                    return this._variableUnitsName;
                }
                set
                {
                    this._variableUnitsName = value;
                }
            }
            [Category("Variables"), Description("SampleMedium")]
            public string SampleMedium
            {
                get
                {
                    return this._sampleMedium;
                }
                set
                {
                    this._sampleMedium = value;
                }
            }
            [Category("Variables"), Description("ValueType")]
            public string ValueType
            {
                get
                {
                    return this._valueType;
                }
                set
                {
                    this._valueType = value;
                }
            }
            [Category("Variables"), Description("TimeSupport")]
            public string TimeSupport
            {
                get
                {
                    return this._timeSupport;
                }
                set
                {
                    this._timeSupport = value;
                }
            }
            [Category("Variables"), Description("TimeUnitsName")]
            public string TimeUnitsName
            {
                get
                {
                    return this._timeUnitsName;
                }
                set
                {
                    this._timeUnitsName = value;
                }
            }
            [Category("Variables"), Description("DataType")]
            public string DataType
            {
                get
                {
                    return this._dataType;
                }
                set
                {
                    this._dataType = value;
                }
            }
            [Category("Variables"), Description("GeneralCategory")]
            public string GeneralCategory
            {
                get
                {
                    return this._generalCategory;
                }
                set
                {
                    this._generalCategory = value;
                }
            }
            [Category("Variables"), Description("NoDataValue")]
            public string NoDataValue
            {
                get
                {
                    return this._noDataValue;
                }
                set
                {
                    this._noDataValue = value;
                }
            }
            #endregion

            #region Methods
            [Category("Methods"), Description("MethodDescription")]
            public string MethodDescription
            {
                get
                {
                    return this._methodDescription;
                }
                set
                {
                    this._methodDescription = value;
                }
            }
            #endregion

            #region Sources
            [Category("Sources"), Description("Organization")]
            public string Organization
            {
                get
                {
                    return this._organization;
                }
                set
                {
                    this._organization = value;
                }
            }
            [Category("Sources"), Description("SourceDescription")]
            public string SourceDescription
            {
                get
                {
                    return this._sourceDescription;
                }
                set
                {
                    this._sourceDescription = value;
                }
            }
            [Category("Sources"), Description("Citation")]
            public string Citation
            {
                get
                {
                    return this._citation;
                }
                set
                {
                    this._citation = value;
                }
            }
            #endregion

            #region QualityControlLevels
            [Category("Variables"), Description("QualityControlLevelCode")]
            public string QualityControlLevelCode
            {
                get
                {
                    return this._qualityControlLevelCode;
                }
                set
                {
                    this._qualityControlLevelCode = value;
                }
            }
            #endregion

            #region DataSeries
            [Description("The BeginDateTime of this Series"), Category("DataSeries")]
            public DateTime BeginDateTime
            {
                get
                {
                    return this._beginDateTime;
                }
                set
                {
                    this._beginDateTime = value;
                }
            }
            [Description("The BeginDateTimeUTC of this Series"), Category("DataSeries")]
            public DateTime BeginDateTimeUTC
            {
                get
                {
                    return this._beginDateTimeUTC;
                }
                set
                {
                    this._beginDateTimeUTC = value;
                }
            }
            [Description("The EndDateTime of this Series"), Category("DataSeries")]
            public DateTime EndDateTime
            {
                get
                {
                    return this._endDateTime;
                }
                set
                {
                    this._endDateTime = value;
                }
            }
            [Description("The EndDateTimeUTC of this Series"), Category("DataSeries")]
            public DateTime EndDateTimeUTC
            {
                get
                {
                    return this._endDateTimeUTC;
                }
                set
                {
                    this._endDateTimeUTC = value;
                }
            }
            [Category("DataSeries"), Description("The Count of Values that Series contains")]
            public int ValueCount
            {
                get
                {
                    return this._valueCount;
                }
                set
                {
                    this._valueCount = value;
                }
            }
            #endregion
        }
    }
}