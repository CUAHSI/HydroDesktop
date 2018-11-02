getDataSeries <-
function(connectionString, seriesID, SQLite= TRUE, startDate="1900-01-01", endDate="2050-12-31"){     
    if(SQLite==TRUE) {      
        driver<-dbDriver("SQLite")
        con<- dbConnect(driver, dbname=connectionString)
dbBeginTransaction(con)
dValues<-getDataValues(con, seriesID, SQLite ,startDate, endDate) 
        dSeries<- dbGetQuery(con, paste("SELECT * FROM DataSeries WHERE SeriesID = " ,seriesID, sep="" )) 
        dSeries$BeginDateTime<-as.Date(dSeries$BeginDateTime, "%Y-%m-%d %H:%M:%S")
        dSeries$BeginDateTimeUTC<-as.Date(dSeries$BeginDateTimeUTC, "%Y-%m-%d %H:%M:%S")
        dSeries$EndDateTime<-as.Date(dSeries$EndDateTime, "%Y-%m-%d %H:%M:%S")
        dSeries$EndDateTimeUTC<-as.Date(dSeries$EndDateTimeUTC, "%Y-%m-%d %H:%M:%S")         
        dSeries$CreationDateTime<-as.Date(dSeries$CreationDateTime, "%Y-%m-%d %H:%M:%S")  
dSeries$UpdateDateTime<-as.Date(dSeries$UpdateDateTime, "%Y-%m-%d %H:%M:%S")  
dSeries$LastcheckedDateTime<-as.Date(dSeries$LastcheckedDateTime, "%Y-%m-%d %H:%M:%S")  
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
    list(DataValues=dValues, DataSeries=dSeries, Site=siteData, Variable=variableData, VariableUnits=variableUnits, TimeUnits=timeUnits, Method=methodData, Source=sourceData, QualityControlLevel=qualityControl)
}

