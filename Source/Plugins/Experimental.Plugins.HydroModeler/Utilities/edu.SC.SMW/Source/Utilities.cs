using System;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Geometries;
using System.Data.SqlClient;

namespace SMW
{
    public class ODM
    {
        //Define all of the necessary ODM parameters within this class (with default values) and then reference these
        // within the Utilities class.  The developer can choose to set the parameters they want before calling the 
        // ODM utiity methods (within their engine class).
        
        //The developer must enter Values, DateTimes, and StreamWriter object to create the .csv file


        #region Accessors for CreateODMcsv

        /// <summary>
        /// Name of the site. Required
        /// </summary>
        /// 
        private string _SiteName = "unknown";
        public string SiteName
        {
            get { return _SiteName; }
            set
            {
                //remove spaces from name
                _SiteName = value.Replace(" ", String.Empty);
            }
        }

        /// <summary>
        /// Corresponding code associated with the SiteName.  If one doesn't exist this is can be used to assign one. Required
        /// </summary>
        private string _siteCode = "0";
        public string SiteCode
        {
            get { return _siteCode; }
            set { _siteCode = value; }
        }

        /// <summary>
        /// Site latitude in degrees. 
        /// </summary>
        /// <example> 41.7 </example>
        private string _Latitude = "41.7";
        public string Latitude
        {
            get { return _Latitude; }
            set { _Latitude = value; }
        }

        /// <summary>
        /// Site longitude in degrees. 
        /// </summary>
        /// <example> -111.9 </example>
        private string _Longitude = "-111.9";
        public string Longitude
        {
            get { return _Longitude; }
            set { _Longitude = value; }
        }

        /// <summary>
        /// Value accuracy
        /// </summary>
        private string _valAccuracy = "1";
        public string ValAccuracy
        {
            get { return _valAccuracy; }
            set { _valAccuracy = value; }
        }

        /// <summary>
        /// Coordinate datum. 
        /// </summary>
        /// <example> 2 </example>
        private string _datum = "2";
        public string Datum
        {
            get { return _datum; }
            set { _datum = value; }
        }

        /// <summary>
        /// Site elevation
        /// </summary>
        /// <example> 0 </example>
        private string _elevation = "0";
        public string Elevation
        {
            get { return _elevation; }
            set { _elevation = value; }
        }

        /// <summary>
        /// Vertical Datum
        /// </summary>
        /// <example> NGVD29 </example>
        private string _verticalDatum = "NGVD29";
        public string VerticalDatum
        {
            get { return _verticalDatum; }
            set { _verticalDatum = value; }
        }

        /// <summary>
        /// Site's 'X' coordinate
        /// </summary>
        /// <example> 421276.323 </example>
        private string _localX = "421276.323";
        public string LocalX
        {
            get { return _localX; }
            set { _localX = value; }
        }

        /// <summary>
        /// Sites 'Y' coordinate
        /// </summary>
        /// <example> 4618952.04 </example>
        private string _localY = "4618952.04";
        public string LocalY
        {
            get { return _localY; }
            set { _localY = value; }
        }

        private string _localProjectionID = "105";
        public string LocalProjectionID
        {
            get { return _localProjectionID; }
            set { _localProjectionID = value; }
        }

        private string _PosAccuracy = "1";
        public string PosAccuracy
        {
            get { return _PosAccuracy; }
            set { _PosAccuracy = value; }
        }

        /// <summary>
        /// State in which the site is located
        /// </summary>
        /// <example> SC </example>
        private string _siteState = "SC";
        public string SiteState
        {
            get { return _siteState; }
            set { _siteState = value; }
        }

        /// <summary>
        /// County in which the site is located
        /// </summary>
        /// <example> Richland </example>
        private string _county = "Richland";
        public string County
        {
            get { return _county; }
            set { _county = value; }
        }


        private string _comment = "none";
        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        /// <summary>
        /// Description of modeling technique
        /// </summary>
        private string _methodDesc = "none";
        public string MethodDesc
        {
            get { return _methodDesc; }
            set { _methodDesc = value; }
        }

        /// <summary>
        /// Web link that explains the methods used
        /// </summary>
        private string _methodLink = "none";
        public string MethodLink
        {
            get { return _methodLink; }
            set { _methodLink = value; }
        }

        private string _qualifierCode = "";
        public string QualifierCode
        {
            get { return _qualifierCode; }
            set { _qualifierCode = value; }
        }

        private string _qualifierDesc = "";
        public string QualifierDesc
        {
            get { return _qualifierDesc; }
            set { _qualifierDesc = value; }
        }

        private string _speciation = "Not Applicable";
        public string Speciation
        {
            get { return _speciation; }
            set { _speciation = value; }
        }

        private string _sampleMedium = "Other";
        public string SampleMedium
        {
            get { return _sampleMedium; }
            set { _sampleMedium = value; }
        }

        private string _valueType = "Field Observation";
        public string ValueType
        {
            get { return _valueType; }
            set { _valueType = value; }
        }

        private string _isRegular = "1";
        public string IsRegular
        {
            get { return _isRegular; }
            set { _isRegular = value; }
        }

        /// <summary>
        /// The time increment which is supported
        /// </summary>
        private string _timeSupport = "30";
        public string TimeSupport
        {
            get { return _timeSupport; }
            set { _timeSupport = value; }
        }

        /// <summary>
        /// Units of time support
        /// </summary>
        private string _timeUnitsID = "102";
        public string TimeUnitsID
        {
            get { return _timeUnitsID; }
            set { _timeUnitsID = value; }
        }

        /// <summary>
        /// Type of data used
        /// </summary>
        private string _dataType = "Incremental";
        public string DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        private string _generalCategory = "Hydrology";
        public string GeneralCategory
        {
            get { return _generalCategory; }
            set { _generalCategory = value; }
        }

        private string _noDataValue = "-9999";
        public string NoDataValue
        {
            get { return _noDataValue; }
            set { _noDataValue = value; }
        }

        private string _offsetUnitsID = "";
        public string OffsetUnitsID
        {
            get { return _offsetUnitsID; }
            set { _offsetUnitsID = value; }
        }

        private string _offsetDesc = "";
        public string OffsetDesc
        {
            get { return _offsetDesc; }
            set { _offsetDesc = value; }
        }

        private string _qualityControlCode = "0";
        public string QualityControlCode
        {
            get { return _qualityControlCode; }
            set { _qualityControlCode = value; }
        }

        private string _definition = "none";
        public string Definition
        {
            get { return _definition; }
            set { _definition = value; }
        }

        private string _explaination = "none";
        public string Explaination
        {
            get { return _explaination; }
            set { _explaination = value; }
        }

        private string _organization = "University of South Carolina";
        public string Organization
        {
            get { return _organization; }
            set { _organization = value; }
        }

        private string _sourceDesc = "Continuous";
        public string SourceDesc
        {
            get { return _sourceDesc; }
            set { _sourceDesc = value; }
        }

        private string _sourceLink = "none";
        public string SourceLink
        {
            get { return _sourceLink; }
            set { _sourceLink = value; }
        }

        private string _contactName = "unknown";
        public string ContactName
        {
            get { return _contactName; }
            set { _contactName = value; }
        }

        private string _phone = "unknown";
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        private string _email = "unknown";
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        private string _address = "unknown";
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private string _city = "unknown";
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        private string _sourceState = "unknown";
        public string SourceState
        {
            get { return _sourceState; }
            set { _sourceState = value; }
        }

        private string _zipCode = "unknown";
        public string ZipCode
        {
            get { return _zipCode; }
            set { _zipCode = value; }
        }

        private string _citation = "none";
        public string Citation
        {
            get { return _citation; }
            set { _citation = value; }
        }

        private string _topicCategory = "inlandWaters";
        public string TopicCategory
        {
            get { return _topicCategory; }
            set { _topicCategory = value; }
        }

        private string _title = "none";
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _abstract = "none";
        public string Abstract
        {
            get { return _abstract; }
            set { _abstract = value; }
        }

        private string _profileVersion = "none";
        public string ProfileVersion
        {
            get { return _profileVersion; }
            set { _profileVersion = value; }
        }

        private string _metadata = "";
        public string Metadata
        {
            get { return _metadata; }
            set { _metadata = value; }
        }

        private string _sampleType = "";
        public string SampleType
        {
            get { return _sampleType; }
            set { _sampleType = value; }
        }

        private string _labSampleCode = "";
        public string LabSampleCode
        {
            get { return _labSampleCode; }
            set { _labSampleCode = value; }
        }

        private string _labName = "";
        public string LabName
        {
            get { return _labName; }
            set { _labName = value; }
        }

        private string _labOrganization = "";
        public string LabOrganization
        {
            get { return _labOrganization; }
            set { _labOrganization = value; }
        }

        private string _labMethodName = "";
        public string LabMethodName
        {
            get { return _labMethodName; }
            set { _labMethodName = value; }
        }

        private string _labMethodDesc = "";
        public string LabMethodDesc
        {
            get { return _labMethodDesc; }
            set { _labMethodDesc = value; }
        }

        private string _labMethodLink = "";
        public string LabMethodLink
        {
            get { return _labMethodLink; }
            set { _labMethodLink = value; }
        }

        private string _offsetValue = "";
        public string OffsetValue
        {
            get { return _offsetValue; }
            set { _offsetValue = value; }
        }

        private string _censorCode = "nc";
        public string CensorCode
        {
            get { return _censorCode; }
            set { _censorCode = value; }
        }

        private string _variableCode = "3";
        public string VariableCode
        {
            get { return _variableCode; }
            set { _variableCode = value; }
        }

        private string _variableName = "Streamflow";
        public string VariableName
        {
            get { return _variableName; }
            set { _variableName = value; }
        }

        private string _variableUnitsID = "35";
        public string VariableUnitsID
        {
            get { return _variableUnitsID; }
            set { _variableUnitsID = value; }
        }

        /// <summary>
        /// Object that holds all of the values that will be entered into ODM
        /// </summary>
        private System.Collections.ArrayList _values = null;
        public System.Collections.ArrayList Values
        {
            get { return _values; }
            set { _values = value; }
        }

        /// <summary>
        /// Object that holds the corresponding times associated with the 'Values' object.  Required
        /// </summary>
        private System.Collections.ArrayList _dateTimes = null;
        public System.Collections.ArrayList DateTimes
        {
            get { return _dateTimes; }
            set { _dateTimes = value; }
        }

        #endregion





        /// <summary>
        /// This method is used to add definitions (i.e. units, methods, variabletype, etc...) into ODM database.
        /// </summary>
        /// <param name="odm_path">Full path to ODM database</param>
        /// <param name="tablename">Name of table in which definition will be appended</param>
        /// <param name="parameter_names">array of parameter names to be entered in database.   Must be in same order as existing tables.  
        /// NOTE: This method will fail if manditory data is omitted, see ODM database for required table entries.</param>
        /// /// <param name="parameter_values">array of parameter values to be entered in database.   Must be in same order as 'parameter_names'.</param>
        public void Add_ODM_Definition(string odm_path, string tablename, string[] parameter_names, string[] parameter_values)
        {
            //C:\\Research\\CUAHSI\\ODM\\Blank Template ODM 1.1\\OD.mdf

            //HACK: The modeler should be responsible for creating the connection to the OD database
            //open Sql Connection
            SqlConnection myconn = new SqlConnection(
                "Data Source=.\\SQLEXPRESS;" +
                "Initial Catalog = OD;" +
                "User ID = sa" +
                "Password = sa");
            //System.Data.SqlClient.SqlConnection myconn = 
            //new System.Data.SqlClient.SqlConnection("Data Source=.\\SQLEXPRESS;" +
            //                                        "AttachDbFilename="+odm_path+";" +
            //                                        "Integrated Security=True;" +
            //                                        "Connect Timeout=30;" +
            //                                        "user id = ODM_Loader"+
            //                                        "Pwd=ODM_Loader");

            try { myconn.Open(); }
            catch (Exception) { };

            //Create insert string
            string insert = "INSERT INTO" + tablename + "(";
            foreach (string param in parameter_names)
            {
                insert += (param + ",");
            }
            insert += ") Values (";
            foreach (string param in parameter_values)
            {
                insert += (param + ",");
            }
            insert += ")";

            //Create SQL command
            System.Data.SqlClient.SqlCommand myCommand = new System.Data.SqlClient.SqlCommand(insert, myconn);

            //Load data into ODM
            try { myCommand.ExecuteNonQuery(); }
            catch (Exception) { };
        }






        /// <summary>
        /// Writes the header information needed prior to writing data
        /// </summary>
        /// <param name="SW"> StreamWriter object pointing to the location where the .csv file will be created</param>
        public void CreateODMcsv()
        {
            if(!System.IO.Directory.Exists(CSVPath))
                System.IO.Directory.CreateDirectory(CSVPath);  
            
           
                
            System.IO.StreamWriter SW = new System.IO.StreamWriter(CSVPath + "\\" + SiteName + ".csv");
            //Write Header Info
            SW.WriteLine("DataValue,ValueAccuracy,LocalDateTime,DateTimeUTC,SiteCode,SiteName," +
                         "Latitude,Longitude,LatLongDatumID,Elevation_m,VerticalDatum,LocalX," +
                         "LocalY,LocalProjectionID,PosAccuracy_m,SiteState,County,Comments," +
                         "MethodDescription,MethodLink,QualifierCode,QualifierDescription," +
                         "VariableCode,VariableName,Speciation,VariableUnitsID,SampleMedium," +
                         "ValueType,IsRegular,TimeSupport,TimeUnitsID,DataType,GeneralCategory," +
                         "NoDataValue,OffsetUnitsID,OffsetDescription,QualityControlLevelCode," +
                         "Definition,Explanation,Organization,SourceDescription,SourceLink,ContactName," +
                         "Phone,Email,Address,City,SourceState,ZipCode,Citation,TopicCategory,Title," +
                         "Abstract,ProfileVersion,MetadataLink,SampleType,LabSampleCode,LabName,LabOrganization," +
                         "LabMethodName,LabMethodDescription,LabMethodLink,OffsetValue,CensorCode");


            //Write out values


            //loop through all the values stored in the Values arraylist
            int i = 0;

            foreach (double value in Values)
            {
                DateTime UTCDateTime = Convert.ToDateTime(DateTimes[i]);

                SW.Write(   value.ToString()    + "," +                             //Data Values
                                ValAccuracy   +"," +                          //Value Accuracy
                                DateTimes[i] + ", " +                         //LocalDateTime
                                UTCDateTime.ToUniversalTime().ToString() + "," +    //DateTimeUTC
                                SiteCode  + " ," +                            //SiteCode
                                SiteName  + "," +                             //SiteName
                                Latitude + "," +                              //Latitude
                                Longitude + "," +                             //Longitude
                                Datum + "," +                                 //Lat Lon Datum
                                Elevation + "," +                             //Elevation
                                VerticalDatum + "," +                         //Vertical Datum
                                LocalX + "," +                                //Local X
                                LocalY + "," +                                //Local Y
                                LocalProjectionID + "," +                     //Local Projection ID
                                PosAccuracy + "," +                              //Pos Accuracy_m
                                SiteState + "," +                             //SiteState
                                County + "," +                                //County
                                Comment + "," +                               //Comment
                                MethodDesc + "," +                            //Method Description
                                MethodLink + "," +                            //MethodLink
                                QualifierCode + "," +                         //Qualifier Code
                                QualifierDesc +"," +                          //Qualifier Desc
                                VariableCode + "," +                          //Variable Code
                                VariableName + "," +                          //Variable Name
                                Speciation + "," +                            //Speciation
                                VariableUnitsID + "," +                       //VariableUnitsID
                                SampleMedium + "," +                          //Sample Medium
                                ValueType + "," +                             //ValueType
                                IsRegular + "," +                             //IsRegular
                                TimeSupport + "," +                           //TimeSupport
                                TimeUnitsID + "," +                           //TimeUnitsID
                                DataType + "," +                              //DataType
                                GeneralCategory + "," +                       //General Category
                                NoDataValue + "," +                           //NoDataValue
                                OffsetUnitsID + "," +                         //OffsetUnitsID
                                OffsetDesc + "," +                            //Offset Desciption
                                QualityControlCode + "," +                    //Quality Control Level Code
                                Definition + "," +                            //Definition
                                Explaination + "," +                          //Explaination
                                Organization + "," +                          //Organization
                                SourceDesc + "," +                            //Source Desc
                                SourceLink + "," +                            //SourceLink
                                ContactName + "," +                           //ContactName
                                Phone + "," +                                 //Phone
                                Email + "," +                                 //Email
                                Address + "," +                               //Address
                                City + "," +                                  //City
                                SourceState + "," +                           //Source State
                                ZipCode + "," +                               //Zip Code
                                Citation + "," +                              //Citation
                                TopicCategory + "," +                         //Topic Category
                                Title + "," +                                 //Title
                                Abstract + "," +                              //Abstract
                                ProfileVersion + "," +                        //Profile version
                                Metadata + "," +                              //Metadata
                                SampleType + "," +                            //SampleType
                                LabSampleCode + "," +                         //LabSampleCode
                                LabName + "," +                               //LabName
                                LabOrganization + "," +                       //LabOrganization
                                LabMethodName + "," +                         //LabMethodName
                                LabMethodDesc + "," +                         //LabMethodDescription
                                LabMethodLink + "," +                         //LabMethodLink
                                OffsetValue + "," +                           //Offset Value
                                CensorCode + "\n");                           //Censor Code
                         
                i++;

            }
            SW.Close();
        }




        //HACK: Remove this because it has phased out
        /// <summary>
        /// Writes model data to csv file, in a format compatible with ODM.Loader (use Utilites.CreateODMcsv to write header info first)
        /// </summary>
        /// <param name="SW">Stream Writer instance </param>
        /// <param name="UTCDateTimes">Array of DateTimes, UTC format</param>
        /// <param name="values">Array of output values, should be organized to match with UTCDateTimes[]</param>
        /// <param name="UnitCode">integer code corresponding to the Unit ID, from ODM 'Units' table </param>
        /// <param name="VariableCode">integer code corresponding to the Variable ID, from ODM 'Variables' table</param>
        /// <param name="variableName">Name of variable in string format</param>
        /// <param name="SiteName">Name of Site in string format</param>
        public void WriteToODMcsv(System.IO.StreamWriter SW, System.Collections.ArrayList DateTimes, System.Collections.ArrayList values, int UnitCode,
            int VariableCode, string variableName, string Sitename)
        {
            ODM Params = new ODM();
            

            int i = 0;
            foreach (double value in values)
            {
                DateTime UTCDateTime = Convert.ToDateTime(DateTimes[i]);

                SW.Write(value.ToString() + "," +                       //Data Values
                         "1," +                                         //Value Accuracy
                         DateTimes[i] + ", " +      //LocalDateTime
                         UTCDateTime.ToUniversalTime().ToString() + "," +                        //DateTimeUTC
                         "1 ," +                           //SiteCode
                         Sitename + "," +                                 //SiteName
                         "41.718473," +                                 //Latitude
                          "-111.946402," +                              //Longitude
                          "2," +                                        //Lat Lon Datum
                          "0," +                                        //Elevation
                          "NGVD29," +                                   //Vertical Datum
                          "421276.323," +                               //Local X
                          "4618952.04," +                               //Local Y
                          "105," +                                      //Local Projection ID
                          "," +                                         //Pos Accuracy_m
                          "SC," +                                       //SiteState
                          "Richland," +                                 //County
                          "none," +                                     //Comment
                          "Component Modeling," +                       //Method Description
                          "http://www.campbellsci.com," +               //MethodLink
                          "," +                                         //Qualifier Code
                          "," +                                         //Qualifier Desc
                          VariableCode.ToString() + "," +                //Variable Code
                          variableName + "," +                           //Variable Name
                          "Not Applicable," +                           //Speciation
                          UnitCode.ToString() + "," +                     //VariableUnitsID
                          "Other," +                                    //Sample Medium
                          "Field Observation," +                 //ValueType
                          "1," +                                        //IsRegular
                          "30," +                                       //TimeSupport
                          "102," +                                      //TimeUnitsID
                          "Incremental," +                              //DataType
                          "Hydrology," +                                //General Category
                          "-9999," +                                    //NoDataValue
                          "," +                                         //OffsetUnitsID
                          "," +                                         //Offset Desciption
                          "0," +                                        //Quality Control Level Code
                          "none," +                                     //Definition
                          "none," +                                     //Explaination
                          "Universit of South Carolina," +              //Organization
                          "Continuous," +                               //Source Desc
                          "none," +                                      //SourceLink
                          "unknown," +                                  //ContactName
                          "unknown," +                                  //Phone
                          "unknown," +                                  //Email
                          "unknown," +                                  //Address
                          "unknown," +                                  //City
                          "unknown," +                                  //Source State
                          "unknown," +                                  //Zip Code
                          "none," +                                     //Citation
                          "inlandWaters," +                             //Topic Category
                          "none," +                                     //Title
                          "none," +                                     //Abstract
                          "none," +                                     //Profile version
                          "," +                                         //Metadata
                          "," +                                         //SampleType
                          "," +                                         //LabSampleCode
                          "," +                                         //LabName
                          "," +                                         //LabOrganization
                          "," +                                         //LabMethodName
                          "," +                                         //LabMethodDescription
                          "," +                                         //LabMethodLink
                          "," +                                         //Offset Value
                          "nc" +                                        //Censor Code
                         "\n");
                i++;
            }

        }







        //TODO: accept relative paths
        /// <summary>
        /// This method loads a .csv file into ODM.  
        /// </summary>
        /// <param name="path">path to the odm .bat file</param>
        public void LoadIntoODM()
        {
            
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.EnableRaisingEvents = false;

            p.StartInfo.FileName = CSVPath + "//" + SiteName + ".bat";
            p.Start();
            p.Close();

        }


        //TODO: accept relative paths
        //HACK: Put into separate class, so that all of the accessors for this method are in the same location
        /// <summary>
        /// This method creates a .bat file is necessary for the LoadODMcsv method.  Returns the full path of the bat file.
        /// </summary>
        /// <param name="path">path to save the .bat file</param>
        /// 
        /// <remarks>Before calling this method, alter the values of Server, database. user, password, file, and log</remarks>
        public void CreateBat()
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(CSVPath + "\\"+SiteName+".bat",false);

            sw.WriteLine("odmloader.exe -server {0} -db {1} -user {2} -password {3} -file {4} -log {5}",Server,Database,User,Password,SiteName+".csv",Log);
            //TODO: For debugging only, Remove.
            sw.WriteLine("pause");
            sw.Close();

        }


        #region Accessors for CreateBAT


        private string _server = "CE-51\\SQLEXPRESS";
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        private string _database = "OD";
        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        private string _user = "sa";
        public string User
        {
            get { return _user; }
            set { _user = value; }
        }

        private string _password = "sa";
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _file = "";
        public string CSVPath
        {
            get { return _file; }
            set { _file = value; }
        }

        private string _log = "log.txt";
        public string Log
        {
            get { return _log; }
            set { _log = value; }
        }

        #endregion


    }


    //HACK: change this class name to GIS
    public class Utilities
    {

        #region GIS Utility Methods
        

        /// <summary>
        /// This method adds vertices to the omiElementSet.  It provides the spatial representation for
        /// the element set.  The vertices are extracted from the components input shapefile.
        /// </summary>
        /// <param name="omiElementSet">the components element set object</param>
        /// <param name="shapefilePath">path to a shapefile, spatially defining the elementset</param>
        /// <returns>the elementset with vertices added to it from the shapefile</returns>
        public ElementSet AddElementsFromShapefile(ElementSet omiElementSet, string shapefilePath)
        {
            //this uses the free SharpMap API for reading a shapefile
            VectorLayer myLayer = new VectorLayer("elements_layer");
            myLayer.DataSource = new ShapeFile(shapefilePath);
            myLayer.DataSource.Open();

            //set spatial reference from shapefile
            SpatialReference sprf = new SpatialReference();
            sprf.ID = myLayer.DataSource.SRID.ToString();
            omiElementSet.SpatialReference = sprf;

            //add elements to elementset from shapefile
            for (uint i = 0; i < myLayer.DataSource.GetFeatureCount(); ++i)
            {

                FeatureDataRow feat = myLayer.DataSource.GetFeature(i);
                string GeometryType = Convert.ToString(
                    feat.Geometry.AsText().Substring(
                    0, feat.Geometry.AsText().IndexOf(' ')));

                Element e = new Element();

                if (feat.Table.Columns.IndexOf("HydroCode") != -1)
                    e.ID = feat.ItemArray[feat.Table.Columns.IndexOf("HydroCode")].ToString();

                if (GeometryType == "POINT")
                {
                    omiElementSet.ElementType = ElementType.XYPoint;
                    Point p = (Point)feat.Geometry;
                    Vertex v = new Vertex();
                    v.x = p.X;
                    v.y = p.Y;
                    e.AddVertex(v);
                }
                if (GeometryType == "POLYGON")
                {
                    omiElementSet.ElementType = ElementType.XYPolygon;
                    Polygon p = (Polygon)feat.Geometry;
                    LinearRing lr = p.ExteriorRing;

                    //Only loop until lr.Vertices.Count-2 b/c the first element is the same
                    // as the last element within the exterior ring.  This will thrown an error
                    // within the OATC element mapper, when trying to map elements.  Also this
                    // loop arranges the vertices of the exterior ring in counter clockwise order
                    // as needed for the element mapping.
                    for (int j = lr.Vertices.Count-2; j >=0 ; j--)
                    {
                        Vertex v = new Vertex();
                        v.x = lr.Vertices[j].X;
                        v.y = lr.Vertices[j].Y;
                        e.AddVertex(v);
                    }
                }
                if (GeometryType == "LINESTRING")
                {
                    omiElementSet.ElementType = ElementType.XYPolyLine;
                    LineString ls = (LineString)feat.Geometry;
                    //Point endpt = ls.EndPoint;
                    //Point startpt = ls.StartPoint;
                    for(int j=0; j< ls.Vertices.Count;j++)
                    {
                        Vertex v = new Vertex();
                        v.x = ls.Vertices[j].X;
                        v.y = ls.Vertices[j].Y;
                        e.AddVertex(v);
                    }
                   
                }
                omiElementSet.AddElement(e);
            }
            return omiElementSet;
        }

        #endregion

    }
}
