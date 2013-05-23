createDataSeries <-
function(connectionString, SQLite= TRUE)
{     
    if(SQLite==TRUE) 
    {      
        driver<-dbDriver("SQLite")
        con<- dbConnect(driver, dbname=connectionString)
		    dbBeginTransaction(con)                                 
		    dValues<-createDataValues(con, SQLite)     
		    dSeries<- dbGetQuery(con, "SELECT * FROM DataSeries WHERE SeriesID = -99999999")
		    #dSeries$BeginDateTime<-strptime(dSeries$BeginDateTime, format= "%Y-%m-%d %H:%M:%S")
        #dSeries$BeginDateTimeUTC<-strptime(dSeries$BeginDateTimeUTC, format= "%Y-%m-%d %H:%M:%S")
        #dSeries$EndDateTime<-strptime(dSeries$EndDateTime, format= "%Y-%m-%d %H:%M:%S")
        #dSeries$EndDateTimeUTC<-strptime(dSeries$EndDateTimeUTC, format= "%Y-%m-%d %H:%M:%S")       
        #dSeries$CreationDateTime<-strptime(dSeries$CreationDateTime, format= "%Y-%m-%d %H:%M:%S")  
		    #dSeries$UpdateDateTime<-strptime(dSeries$UpdateDateTime, format= "%Y-%m-%d %H:%M:%S")
		    #dSeries$LastcheckedDateTime<-strptime(dSeries$LastCheckedDateTime, format= "%Y-%m-%d %H:%M:%S")
            
        variableData<-dbGetQuery(con, "SELECT * FROM Variables WHERE VariableID=-99999999") 
        siteData<-dbGetQuery(con, "SELECT * FROM Sites WHERE SiteID=-99999999")       
        variableUnits<-dbGetQuery(con, "SELECT * FROM Units WHERE UnitsID=-99999999")
        timeUnits<-dbGetQuery(con, "SELECT * FROM Units WHERE UnitsID=-99999999")
        methodData<-dbGetQuery(con, "SELECT * FROM Methods WHERE MethodID=-99999999") 
        sourceData<-dbGetQuery(con, "SELECT * FROM Sources WHERE SourceID=-99999999") 
	      qualityControl<-dbGetQuery(con, "SELECT * FROM QualityControlLevels WHERE QualityControlLevelID=-99999999") 
	       dbCommit(con)	
        dbDisconnect(con) 
	   }	  
    list(DataValues=dValues, DataSeries=dSeries, Site=siteData, Variable=variableData, Method=methodData, Sources=sourceData, VariableUnits=variableUnits, TimeUnits=timeUnits, QualityControlLevel=qualityControl)
}

 createDataValues<-function(con, SQLite)
{
    if(SQLite==TRUE) 
    { 
      dataFrame<-dbGetQuery(con, paste("SELECT * FROM DataValues WHERE SeriesID = -99999999 AND LocalDateTime BETWEEN '1800-01-01' AND '1800-01-01'", sep="" )) 
      dataFrame$LocalDateTime<-strptime(dataFrame$LocalDateTime, format= "%Y-%m-%d %H:%M:%S")
      dataFrame$DateTimeUTC<-strptime(dataFrame$DateTimeUTC, format= "%Y-%m-%d %H:%M:%S")
    }        
    dataFrame 
}