saveDataSeries <-
function(connectionString, newSeries, SQLite = TRUE, overwrite= TRUE){
if(SQLite==TRUE){
driver<-dbDriver("SQLite")
        con<- dbConnect(driver, dbname=connectionString)
        dbBeginTransaction(con)

siteID<-getSiteID(con, newSeries$Site, overwrite)
variableID<-getVariableID(con, newSeries$Variable, overwrite)
methodID<-getMethodID(con, newSeries$Method, overwrite)
qclID<-getQualityControlLevelID(con, newSeries$QualityControlLevel, overwrite)
vunitsID<-getUnitsID(con, newSeries$VariableUnits,overwrite)
tunitsID<-getUnitsID(con, newSeries$TimeUnits,overwrite)
sourceID<-getSourceID(con, newSeries$Source, overwrite)
SeriesID<-getSeriesID(con, list(site=siteID, variable=variableID, method=methodID, qcl=qclID, source=sourceID), newSeries$DataSeries, newSeries$DataValues , overwrite)
 
#dataValues Mandatory:ValueID, DataValue, LocalDateTime, UTCOffcet, DateTimeUTC, SiteID, VariableID, CensorCode, MethodID, SourceID, QualityControlLevelID
for(i in 1:length(newSeries$DataValues$DataValue)){
ser<-newSeries$DataValues
if(overwrite==FALSE){
#print( paste("INSERT INTO DataValues VALUES(",getNextValueID(con),", ",SeriesID,", ", ser$DataValue[i],", ",ser$ValueAccuracy[i],", '", format(ser$LocalDateTime[i],"%Y-%m-%d %H:%M:%S"),"', ",ser$UTCOffset[i],", '",format(ser$DateTimeUTC[i],"%Y-%m-%d %H:%M:%S"),"', ", ser$OffsetValue[i],", '", ser$OffsetTypeID[i],"', '", ser$CensorCode[i],"', '", ser$QualifierID[i],"', '", ser$SampleID[i],"', '", ser$FileID[i], "')", sep=""))
dbGetQuery(con,paste("INSERT INTO DataValues VALUES(",getNextValueID(con),", ",SeriesID,", ", ser$DataValue[i],", ",ser$ValueAccuracy[i],", '", format(ser$LocalDateTime[i],"%Y-%m-%d %H:%M:%S"),"', ",ser$UTCOffset[i],", '",format(ser$DateTimeUTC[i],"%Y-%m-%d %H:%M:%S"),"', ", ser$OffsetValue[i],", '", ser$OffsetTypeID[i],"', '", ser$CensorCode[i],"', '", ser$QualifierID[i],"', '", ser$SampleID[i],"', '", ser$FileID[i], "')", sep=""))
}
else{
count<-dbGetQuery(con, paste("SELECT COUNT(DataValue) FROM DataValues WHERE SeriesID=",SeriesID, " AND LocalDateTime = '", format(newSeries$DataValues$LocalDateTime[i],"%Y-%m-%d %H:%M:%S"),"'", sep=""))
if (count==0){
#print(paste("INSERT INTO DataValues VALUES(",getNextValueID(con),", ",SeriesID,", ", ser$DataValue[i],", ",ser$ValueAccuracy[i],", '", format(ser$LocalDateTime[i],"%Y-%m-%d %H:%M:%S"),"', ",ser$UTCOffset[i],", '",format(ser$DateTimeUTC[i],"%Y-%m-%d %H:%M:%S"),"', ", ser$OffsetValue[i],", '", ser$OffsetTypeID[i],"', '", ser$CensorCode[i],"', '", ser$QualifierID[i],"', '", ser$SampleID[i],"', '", ser$FileID[i], "')", sep=""))
dbGetQuery(con,paste("INSERT INTO DataValues VALUES(",getNextValueID(con),", ",SeriesID,", ", ser$DataValue[i],", ",ser$ValueAccuracy[i],", '", format(ser$LocalDateTime[i],"%Y-%m-%d %H:%M:%S"),"', ",ser$UTCOffset[i],", '",format(ser$DateTimeUTC[i],"%Y-%m-%d %H:%M:%S"),"', ", ser$OffsetValue[i],", '", ser$OffsetTypeID[i],"', '", ser$CensorCode[i],"', '", ser$QualifierID[i],"', '", ser$SampleID[i],"', '", ser$FileID[i], "')", sep=""))
}
else{
#print(paste("UPDATE DataValues SET DataValue=", newSeries$DataValues$DataValue[i], " WHERE SeriesID=",SeriesID, " AND LocalDateTime ='", format(newSeries$DataValues$LocalDateTime[i],"%Y-%m-%d %H:%M:%S"),"'", sep=""))
dbGetQuery(con, paste("UPDATE DataValues SET DataValue=", newSeries$DataValues$DataValue[i], " WHERE SeriesID=",SeriesID, " AND LocalDateTime ='", format(newSeries$DataValues$LocalDateTime[i],"%Y-%m-%d %H:%M:%S"),"'", sep=""))
}
} 
}
dbCommit(con)  
    dbDisconnect(con)
}
}

          getDataValues<-function(con, seriesID, SQLite ,startDate, endDate){ 
    #sep = separator between each comma
    dataFrame<-dbGetQuery(con, paste("SELECT * FROM DataValues WHERE SeriesID = " ,seriesID, " AND  LocalDateTime BETWEEN '", startDate,"' AND '", endDate,"'", sep="" ))         
    dataFrame$LocalDateTime<-as.Date(dataFrame$LocalDateTime, "%Y-%m-%d %H:%M:%S")
    dataFrame$DateTimeUTC<-as.Date(dataFrame$DateTimeUTC, "%Y-%m-%d %H:%M:%S")
    dataFrame 
}  

getNextValueID<-function(con){	
	dbGetQuery(con, paste("SELECT MAX(ValueID) FROM DataValues"))+1
}

getNextSeriesID<-function(con){
	dbGetQuery(con, paste("SELECT MAX(SeriesID) FROM DataSeries"))+1	
}

getSeriesID<-function(con, S, Series, Values, overwrite){
	#print(paste("SELECT SeriesID FROM DataSeries WHERE SiteID=",S$site, " AND VariableID=", S$variable, " AND MethodID=",S$method," AND QualityControlLevelID= ",S$qcl," AND SourceID=",S$source, sep="" ))
	seriesID<-dbGetQuery(con, paste("SELECT SeriesID FROM DataSeries WHERE SiteID=",S$site, " AND VariableID=", S$variable, " AND MethodID=",S$method," AND QualityControlLevelID= ",S$qcl," AND SourceID=",S$source, sep="" ))
	#print(seriesID)
	if(overwrite==TRUE ){
			#print(paste("UPDATE DataSeries SET UpdateDateTime= '", format(Sys.time(),"%Y-%m-%d %H:%M:%S"), "', ValueCount=", nrow(Values),", BeginDateTime='",format(min(Values$LocalDateTime),"%Y-%m-%d %H:%M:%S"),"',EndDateTime='",format(max(Values$LocalDateTime),"%Y-%m-%d %H:%M:%S"),"',BeginDateTimeUTC='",format(min(Values$DateTimeUTC),"%Y-%m-%d %H:%M:%S"),"',EndDateTimeUTC='",format(max(Values$DateTimeUTC),"%Y-%m-%d %H:%M:%S"),"' WHERE SeriesID=", seriesID, sep="" ))
			dbGetQuery(con, paste("UPDATE DataSeries SET UpdateDateTime= '", format(Sys.time(),"%Y-%m-%d %H:%M:%S"), "', ValueCount=", nrow(Values),", BeginDateTime='",format(min(Values$LocalDateTime),"%Y-%m-%d %H:%M:%S"),"', EndDateTime='",format(max(Values$LocalDateTime),"%Y-%m-%d %H:%M:%S"),"', BeginDateTimeUTC='",format(min(Values$DateTimeUTC),"%Y-%m-%d %H:%M:%S"),"',EndDateTimeUTC='",format(max(Values$DateTimeUTC),"%Y-%m-%d %H:%M:%S"),"' WHERE SeriesID=", seriesID, sep="" ))
	}
	else{
	#list(site=siteID, variable=variableID, method=methodID, qcl=qclID, source=sourceID)
		if(is.null(S$site)|| is.null(S$variable)|| is.null(S$method)|| is.null(S$qcl)|| is.null(S$source)||!nchar(S$site)|| !nchar(S$variable)|| !nchar(S$method)|| !nchar(S$qcl) || !nchar(S$source)){
			#tkmessageBox(message="All of the Mandatory Series information was not filled out, the Series data will not be written to the database",icon="error",type="ok")
			stop("All of the Mandatory Series information was not filled out")
			#print(paste(S$site, S$variable, S$method, S$qcl, S$source,nchar(S$site), nchar(S$variable),nchar(S$method),nchar(S$qcl) ,nchar(S$source), sep=", "))
		}
		else{	
			if(S$site==Series$SiteID && S$variable==Series$VariableID && S$method==Series$MethodID && S$source==Series$SourceID && S$qcl==Series$QualityControlLevelID ){
				tkmessageBox(message="You have Not edited your Metadata but have set overwrite==FALSE. You may end up with duplicate entries in your database.",icon="error",type="ok")
			}else{
				#print(paste("INSERT INTO DataSeries VALUES(",getNextSeriesID(con),", ",S$site,", ", S$variable,", ", Series$IsCategorical,", ", S$method,", ", Series$SourceID,", ", Series$QualityControlLevelID,", '", format(min(Values$LocalDateTime),"%Y-%m-%d %H:%M:%S"),"', '", format(max(Values$LocalDateTime),"%Y-%m-%d %H:%M:%S"),"', '", format(min(Values$DateTimeUTC),"%Y-%m-%d %H:%M:%S"),"', '", format(max(Values$DateTimeUTC),"%Y-%m-%d %H:%M:%S"),"', ", nrow(Values),", '", format(Sys.time(),"%Y-%m-%d %H:%M:%S"),"', ", Series$Subscribed,", '", format(Sys.time(),"%Y-%m-%d %H:%M:%S"),"', '", format(Sys.time(),"%Y-%m-%d %H:%M:%S"),"')",sep=""))
				dbGetQuery(con,paste("INSERT INTO DataSeries VALUES(",getNextSeriesID(con),", ",S$site,", ", S$variable,", ", Series$IsCategorical,", ", S$method,", ", Series$SourceID,", ", Series$QualityControlLevelID,", '", format(min(Values$LocalDateTime),"%Y-%m-%d %H:%M:%S"),"', '", format(max(Values$LocalDateTime),"%Y-%m-%d %H:%M:%S"),"', '", format(min(Values$DateTimeUTC),"%Y-%m-%d %H:%M:%S"),"', '", format(max(Values$DateTimeUTC),"%Y-%m-%d %H:%M:%S"),"', ", nrow(Values),", '", format(Sys.time(),"%Y-%m-%d %H:%M:%S"),"', ", Series$Subscribed,", '", format(Sys.time(),"%Y-%m-%d %H:%M:%S"),"', '", format(Sys.time(),"%Y-%m-%d %H:%M:%S"),"')",sep=""))
			}
			#print(paste("SELECT SeriesID FROM DataSeries WHERE SiteID=",S$site, " AND VariableID=", S$variable, " AND MethodID=",S$method," AND QualityControlLevelID= ",S$qcl," AND SourceID=",S$source, sep="" ))
			seriesID<-dbGetQuery(con, paste("SELECT SeriesID FROM DataSeries WHERE SiteID=",S$site, " AND VariableID=", S$variable, " AND MethodID=",S$method," AND QualityControlLevelID= ",S$qcl," AND SourceID=",S$source, sep="" ))
			#print(seriesID)
		}	
	}	
	seriesID
}

getSiteID<-function(con, Site, overwrite){
#Mandatory: siteID, siteCode, siteName, Latitude, longitude, LatLongDatumID
	#print(paste("SELECT SiteID FROM Sites WHERE SiteCode='",Site$SiteCode, "' AND SiteName='", Site$SiteName, "'", sep="" ))
	siteID<-dbGetQuery(con, paste("SELECT SiteID FROM Sites WHERE SiteCode='",Site$SiteCode, "'AND SiteName='", Site$SiteName, "'", sep="" ))
	#print(siteID)
	if(nrow(siteID)==0){
		if(overwrite==TRUE){
			#print("You have edited your site information but have set overwrite==TRUE so the data is not being written to the database. Overwrite=TRUE means you want to change the datavalues but keep the rest of the data the same")
			tkmessageBox(message="You have edited your site information but have set overwrite==TRUE so the data is not being written to the database.",icon="error",type="ok")
		}
		else{
			if(is.null(Site$SiteCode)|| is.null(Site$SiteName)|| is.null(Site$Latitude)|| is.null(Site$Longitude)|| is.null(Site$LatLongDatumID)||!nchar(Site$SiteCode)|| !nchar(Site$SiteName)|| !nchar(Site$Latitude)|| !nchar(Site$Longitude) || !nchar(Site$LatLongDatumID)){
			#if(TRUE){
				#print(paste(Site$SiteCode, Site$SiteName, Site$Latitude, Site$Longitude, Site$LatLongDatumID,nchar(Site$SiteCode), nchar(Site$SiteName),nchar(Site$Latitude),nchar(Site$Longitude) ,nchar(Site$LatLongDatumID), sep=", "))
				#tkmessageBox(message="All of the Mandatory site information was not filled out, the Site data will not be written to the database",icon="error",type="ok")
				stop("All of the Mandatory Site information was not filled out")
			}else{
				#print(paste("INSERT INTO Sites VALUES(",dbGetQuery(con, paste("SELECT MAX(SiteID) FROM Sites"))+1,", '", Site$SiteCode,"', '", Site$SiteName,"', ", Site$Latitude,", ", Site$Longitude,", ",Site$LatLongDatumID,", ", Site$Elevation_m,", '", Site$VerticalDatum,"', ", Site$LocalX,", ", Site$LocalY,", '",Site$LocalProjectionID,"', ", Site$PosAccuracy_m,", '", Site$State,"', '", Site$County,"', '", Site$Comments , "')", sep = ""))
				dbGetQuery(con, paste("INSERT INTO Sites VALUES(",dbGetQuery(con, paste("SELECT MAX(SiteID) FROM Sites"))+1,", '", Site$SiteCode,"', '", Site$SiteName,"', ", Site$Latitude,", ", Site$Longitude,", ",Site$LatLongDatumID,", ", Site$Elevation_m,", '", Site$VerticalDatum,"', ", Site$LocalX,", ", Site$LocalY,", '",Site$LocalProjectionID,"', ", Site$PosAccuracy_m,", '", Site$State,"', '", Site$County,"', '", Site$Comments , "')", sep = ""))
				siteID<-dbGetQuery(con, paste("SELECT SiteID FROM Sites WHERE SiteCode='",Site$SiteCode, "'AND SiteName='", Site$SiteName, "'", sep="" ))
				#print(siteID)
			}
		}			
	}	
	siteID	
}

getVariableID<-function(con, Variable, overwrite){
#Mandatory: VariableID, VariableCode, VariableName, Speciation, VariableUnitsID, SampleMedium, ValueType, IsRegular, TimeSupport, TimeUnitsID, DataType, GeneralCategory, NoDataValue
	#print(paste("SELECT VariableID FROM Variables WHERE VariableCode='", Variable$VariableCode, "' AND VariableName='",Variable$VariableName,"' AND VariableUnitsID=",Variable$VariableUnitsID, sep=""))		
	variableID<-dbGetQuery(con, paste("SELECT VariableID FROM Variables WHERE VariableCode='", Variable$VariableCode, "' AND VariableName='",Variable$VariableName,"' AND VariableUnitsID=",Variable$VariableUnitsID, sep=""))		
	if(nrow(variableID)==0){
		if(overwrite==TRUE){
			tkmessageBox(message="You have edited your variable information but have set overwrite==TRUE so the data is not being written to the database.",icon="error",type="ok")
		}
		else{
			if( is.null(Variable$VariableCode)|| !nchar(Variable$VariableCode)|| is.null(Variable$VariableName)|| !nchar(Variable$VariableName)|| is.null(Variable$Speciation)|| !nchar(Variable$Speciation)|| is.null(Variable$VariableUnitsID)|| !nchar(Variable$VariableUnitsID)|| is.null(Variable$SampleMedium)|| !nchar(Variable$SampleMedium)|| is.null(Variable$ValueType)|| !nchar(Variable$ValueType)|| is.null(Variable$IsRegular)|| !nchar(Variable$IsRegular)|| is.null(Variable$TimeSupport)|| !nchar(Variable$TimeSupport)|| is.null(Variable$TimeUnitsID)|| !nchar(Variable$TimeUnitsID)|| is.null(Variable$DataType)|| !nchar(Variable$DataType)|| is.null(Variable$GeneralCategory)|| !nchar(Variable$GeneralCategory)|| is.null(Variable$NoDataValue)|| !nchar(Variable$NoDataValue)){
				#tkmessageBox(message="All of the Mandatory Variable information was not filled out, the Variable data will not be written to the database",icon="error",type="ok")
				stop("All of the Mandatory Variable information was not filled out")
			}else{
				#print(paste("INSERT INTO Variables VALUES(",dbGetQuery(con, paste("SELECT MAX(VariableID) FROM Variables"))+1,", '", Variable$VariableCode,"', '", Variable$VariableName,"', '", Variable$Speciation,"', ", Variable$VariableUnitsID,", '",Variable$SampleMedium,"', '", Variable$ValueType,"', ", Variable$IsRegular,", ", Variable$IsCategorical,", ", Variable$TimeSupport,", ", Variable$TimeUnitsID,", '", Variable$DataType,"', '", Variable$GeneralCategory,"', ", Variable$NoDataValue,", '", Variable$ConceptID , "')", sep = ""))
				dbGetQuery(con, paste("INSERT INTO Variables VALUES(",dbGetQuery(con, paste("SELECT MAX(VariableID) FROM Variables"))+1,", '", Variable$VariableCode,"', '", Variable$VariableName,"', '", Variable$Speciation,"', ", Variable$VariableUnitsID,", '",Variable$SampleMedium,"', '", Variable$ValueType,"', ", Variable$IsRegular,", ", Variable$IsCategorical,", ", Variable$TimeSupport,", ", Variable$TimeUnitsID,", '", Variable$DataType,"', '", Variable$GeneralCategory,"', ", Variable$NoDataValue,", '", Variable$ConceptID , "')", sep = ""))
				variableID<-dbGetQuery(con, paste("SELECT VariableID FROM Variables WHERE VariableCode='", Variable$VariableCode, "' AND VariableName='",Variable$VariableName,"' AND VariableUnitsID=",Variable$VariableUnitsID, sep=""))		
				#print(variableID)
			}
		}			
	}	
	variableID
}

getMethodID<-function(con, Method, overwrite){
#Mandatory: MethodID, MethodDescription
	#print(paste("SELECT MethodID FROM Methods WHERE MethodLink='", Method$MethodLink, "' AND MethodDescription='",Method$MethodDescription, "'", sep=""))				
	methodID<-dbGetQuery(con, paste("SELECT MethodID FROM Methods WHERE MethodLink='", Method$MethodLink, "' AND MethodDescription='",Method$MethodDescription, "'", sep=""))				
	if(nrow(methodID)==0){
		if(overwrite==TRUE){
			tkmessageBox(message="You have edited your method information but have set overwrite==TRUE so the data is not being written to the database.",icon="error",type="ok")
		}
		else{		
			if(is.null(Method$MethodDescription)|| !nchar(Method$MethodDescription)){
				#tkmessageBox(message="All of the Mandatory Method information was not filled out, the Method data will not be written to the database",icon="error",type="ok")
				stop("All of the Mandatory Method information was not filled out")
			}else{
				#print(paste("INSERT INTO Methods VALUES(",dbGetQuery(con, paste("SELECT MAX(MethodID) FROM Methods"))+1,", '", Method$MethodDescription,"', '", Method$MethodLink,"')", sep = ""))
				dbGetQuery(con, paste("INSERT INTO Methods VALUES(",dbGetQuery(con, paste("SELECT MAX(MethodID) FROM Methods"))+1,", '", Method$MethodDescription,"', '", Method$MethodLink,"')", sep = ""))
				methodID<-dbGetQuery(con, paste("SELECT MethodID FROM Methods WHERE MethodLink='", Method$MethodLink, "' AND MethodDescription='",Method$MethodDescription, "'", sep=""))				
				#print(methodID)
			}
		}			
	}	
	methodID
}

getSourceID<-function(con, Sources, overwrite){
#Mandatory: SourceID, Organization, sourceDescription, ContactName, Phone, Email, Address, City, state, ZipCode, Citation, MetadataID
	#print(paste("SELECT SourceID FROM Sources WHERE Organization='", Sources$Organization, "' AND SourceLink='",Sources$SourceLink, "' AND ContactName='",Sources$ContactName,"' AND City='",Sources$City,"'", sep=""))				
	sourceID<-dbGetQuery(con, paste("SELECT SourceID FROM Sources WHERE Organization='", Sources$Organization, "' AND SourceLink='",Sources$SourceLink, "' AND ContactName='",Sources$ContactName,"' AND City='",Sources$City,"'", sep=""))				
	if(nrow(sourceID)==0){
		if(overwrite==TRUE){
			tkmessageBox(message="You have edited your Source information but have set overwrite==TRUE so the data is not being written to the database.",icon="error",type="ok")
		}
		else{	
			if(is.null(Sources$Organization)|| !nchar(Sources$Organization)|| is.null(Sources$SourceDescription) || !nchar(Sources$SourceDescription)|| is.null(Sources$ContactName)|| !nchar(Sources$ContactName)|| is.null(Sources$Phone)|| !nchar(Sources$Phone)|| is.null(Sources$Email)|| !nchar(Sources$Email)|| is.null(Sources$Address)|| !nchar(Sources$Address)|| is.null(Sources$City)|| !nchar(Sources$City)|| is.null(Sources$State)|| !nchar(Sources$State)|| is.null(Sources$ZipCode)|| !nchar(Sources$ZipCode)|| is.null(Sources$Citation)|| !nchar(Sources$Citation)|| is.null(Sources$MetadataID)|| !nchar(Sources$MetadatID)){
				#tkmessageBox(message="All of the Mandatory Source information was not filled out, the Source data will not be written to the database",icon="error",type="ok")
				stop("All of the Mandatory Source information was not filled out")
			}else{	
				#print(paste("INSERT INTO Sources VALUES(",dbGetQuery(con, paste("SELECT MAX(SourceID) FROM Sources"))+1,", '",Sources$Organization,"', '",Sources$SourceDescription,"', '",Sources$SourceLink,"', '",Sources$ContactName,"', '",Sources$Phone,"', '",Sources$Email,"', '",Sources$Address,"', '",Sources$City,"', '",Sources$State,"', '",Sources$ZipCode,"', '",Sources$Citation,"', '",Sources$MetadataID ,"')", sep = ""))
				dbGetQuery(con, paste("INSERT INTO Sources VALUES(",dbGetQuery(con, paste("SELECT MAX(SourceID) FROM Sources"))+1,", '",Sources$Organization,"', '",Sources$SourceDescription,"', '",Sources$SourceLink,"', '",Sources$ContactName,"', '",Sources$Phone,"', '",Sources$Email,"', '",Sources$Address,"', '",Sources$City,"', '",Sources$State,"', '",Sources$ZipCode,"', '",Sources$Citation,"', '",Sources$MetadataID ,"')", sep = ""))
				sourceID<-dbGetQuery(con, paste("SELECT SourceID FROM Sources WHERE Organization='", Sources$Organization, "' AND SourceLink='",Sources$SourceLink, "' AND ContactName='",Sources$ContactName,"' AND City='",Sources$City,"'", sep=""))				
				#print(sourceID)
			}
		}			
	}
	sourceID
}

getQualityControlLevelID<-function(con, QCL, overwrite){
#Mandatory: QualityControlLevelID, QualityControlLevelCode, Definition, Explanation
	#print(paste("SELECT QualityControlLevelID FROM QualityControlLevels WHERE QualityControlLevelCode='", QCL$QualityControlLevelCode, "' AND Definition='",QCL$Definition, "'", sep=""))				
	qclID<-dbGetQuery(con, paste("SELECT QualityControlLevelID FROM QualityControlLevels WHERE QualityControlLevelCode='", QCL$QualityControlLevelCode, "' AND Definition='",QCL$Definition, "'" , sep=""))				
	if(nrow(qclID)==0){
		if(overwrite==TRUE){
			tkmessageBox(message="You have edited your Quality Control information but have set overwrite==TRUE so the data is not being written to the database.",icon="error",type="ok")
		}
		else{
			if(is.null(QCL$QualityControlLevelCode)|| !nchar(QCL$QualityControlLevelCode)||is.null(QCL$Definition)|| !nchar(QCL$Definition)|| is.null(QCL$Explanation)|| !nchar(QCL$Explanation)){ 
				#tkmessageBox(message="All of the Mandatory Quality Control information was not filled out, the Qualty Control data will not be written to the database",icon="error",type="ok")
				stop("All of the Mandatory Quality Control information was not filled out")
			}else{	
				#print(paste("INSERT INTO QualityControlLevels VALUES(",dbGetQuery(con, paste("SELECT MAX(QualityControlLevelID) FROM QualityControlLevels"))+1,", '", QCL$QualityControlLevelCode,"', '", QCL$Definition,"', '", QCL$Explanation,"')", sep = ""))
				dbGetQuery(con, paste("INSERT INTO QualityControlLevels VALUES(",dbGetQuery(con, paste("SELECT MAX(QualityControlLevelID) FROM QualityControlLevels"))+1,", '", QCL$QualityControlLevelCode,"', '", QCL$Definition,"', '", QCL$Explanation,"')", sep = ""))
				qclID<-dbGetQuery(con, paste("SELECT QualityControlLevelID FROM QualityControlLevels QualityControlLevelCode='", QCL$QualityControlLevelCode, "' AND Definition='",QCL$Definition, "'", sep=""))				
				#print(qclID)
			}
		}			
	}
	qclID
}

getUnitsID<-function(con, Units, overwrite){
#Mandatory: UnitsID, UnitsName, UnitsType, UnitsAbbreviation
	#print(paste("SELECT UnitsID FROM Units WHERE UnitsName='", Units$UnitsName, "' AND UnitsType='",Units$UnitsType, "' AND UnitsAbbreviation='",Units$UnitsAbbreviation,"'", sep=""))				
	unitID<-dbGetQuery(con, paste("SELECT UnitsID FROM Units WHERE UnitsName='", Units$UnitsName, "' AND UnitsType='",Units$UnitsType, "' AND UnitsAbbreviation='",Units$UnitsAbbreviation,"'", sep=""))				
	if(nrow(unitID)==0){
		if(overwrite==TRUE){
			tkmessageBox(message="You have edited your Units information but have set overwrite==TRUE so the data is not being written to the database.",icon="error",type="ok")
		}
		else{	
			if(is.null(Units$UnitsName)|| !nchar(Units$UnitsName)|| is.null(Units$UnitsType)|| !nchar(Units$UnitsType)||is.null(Units$UnitsAbbreviation)|| !nchar(Units$UnitsAbbreviation)){
				#tkmessageBox(message="All of the Units information was not filled out, the Qualty Control data will not be written to the database",icon="error",type="ok")
				stop("All of the Mandatory Units information was not filled out")
			}else{	
				#print(paste("INSERT INTO Units VALUES(",dbGetQuery(con, paste("SELECT MAX(UnitsID) FROM Units"))+1,", '",Units$UnitsName,"', '",Units$UnitsType,"', '",Units$UnitsAbbreviation,"')", sep = ""))
				dbGetQuery(con,paste("INSERT INTO Units VALUES(",dbGetQuery(con, paste("SELECT MAX(UnitsID) FROM Units"))+1,", '",Units$UnitsName,"', '",Units$UnitsType,"', '",Units$UnitsAbbreviation,"')", sep = "")) 
				unitID<-dbGetQuery(con, paste("SELECT UnitsID FROM Units WHERE UnitsName='", Units$UnitsName, "' AND UnitsType='",Units$UnitsType, "' AND UnitsAbbreviation='",Units$UnitsAbbreviation,"'", sep=""))					
				#print(unitID)
			}
		}			
	}	
	unitID	
}
