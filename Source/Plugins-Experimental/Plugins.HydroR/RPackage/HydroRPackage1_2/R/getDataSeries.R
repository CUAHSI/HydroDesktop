getDataSeries <-
function(connectionString, seriesID, SQLite= TRUE, startDate="1900-01-01", endDate="2050-12-31")
{     
    if(SQLite==TRUE) 
    {      
        driver<-dbDriver("SQLite")
        con<- dbConnect(driver, dbname=connectionString)
		    dbBeginTransaction(con)                                 
	      dValues<-getDataValues(con, seriesID, SQLite ,startDate, endDate)     
        dSeries<- dbGetQuery(con, paste("SELECT * FROM DataSeries WHERE SeriesID = " ,seriesID, sep="" ))    
        #format(dSeries$LocalDateTime, format= "%Y-%m-%d %H:%M:%S", usetz= FALSE)
        #dSeries$BeginDateTime<-as.Date(dSeries$BeginDateTime, "%Y-%m-%d %H:%M:%S")
        dSeries$BeginDateTime<-strptime(dSeries$BeginDateTime, format= "%Y-%m-%d %H:%M:%S")
        dSeries$BeginDateTimeUTC<-strptime(dSeries$BeginDateTimeUTC, format= "%Y-%m-%d %H:%M:%S")
        dSeries$EndDateTime<-strptime(dSeries$EndDateTime, format= "%Y-%m-%d %H:%M:%S")
        dSeries$EndDateTimeUTC<-strptime(dSeries$EndDateTimeUTC, format= "%Y-%m-%d %H:%M:%S")       
        dSeries$CreationDateTime<-strptime(dSeries$CreationDateTime, format= "%Y-%m-%d %H:%M:%S")  
		    dSeries$UpdateDateTime<-strptime(dSeries$UpdateDateTime, format= "%Y-%m-%d %H:%M:%S")
		    dSeries$LastcheckedDateTime<-strptime(dSeries$LastcheckedDateTime, format= "%Y-%m-%d %H:%M:%S")
       	variableData<-dbGetQuery(con, paste("SELECT * FROM Variables WHERE VariableID=", dSeries$VariableID)) 
	      siteData<-dbGetQuery(con, paste("SELECT * FROM Sites WHERE SiteID=", dSeries$SiteID))       
        variableUnits<-dbGetQuery(con, paste("SELECT * FROM Units WHERE UnitsID=", variableData$VariableUnitsID))
        timeUnits<-dbGetQuery(con, paste("SELECT * FROM Units WHERE UnitsID=", variableData$TimeUnitsID ))
        methodData<-dbGetQuery(con, paste("SELECT * FROM Methods WHERE MethodID=", dSeries$MethodID)) 
        sourceData<-dbGetQuery(con, paste("SELECT * FROM Sources WHERE SourceID=", dSeries$SourceID)) 
        qualityControl<-dbGetQuery(con, paste("SELECT * FROM QualityControlLevels WHERE QualityControlLevelID=", dSeries$QualityControlLevelID)) 
       dbCommit(con)	
       dbDisconnect(con) 
	 }	  
   list(DataValues=dValues, DataSeries=dSeries, Site=siteData, Variable=variableData, Method=methodData, Source=sourceData, VariableUnits=variableUnits, TimeUnits=timeUnits, QualityControlLevel=qualityControl)
}

   getDataValues<-function(con, seriesID, SQLite ,startDate, endDate)
{ 
    dataFrame<-dbGetQuery(con, paste("SELECT * FROM DataValues WHERE SeriesID = " ,seriesID, " AND  LocalDateTime BETWEEN '", startDate,"' AND '", endDate,"'", sep="" ))         
    #dataFrame$LocalDateTime<-as.Date(dataFrame$LocalDateTime, "%Y-%m-%d %H:%M:%S")
    #format(dSeries$LocalDateTime, format= "%Y-%m-%d %H:%M:%S", usetz= FALSE)
    dataFrame$LocalDateTime<-strptime(dataFrame$LocalDateTime, format= "%Y-%m-%d %H:%M:%S")
    dataFrame$DateTimeUTC<-strptime(dataFrame$DateTimeUTC, format= "%Y-%m-%d %H:%M:%S")
    dataFrame 
}