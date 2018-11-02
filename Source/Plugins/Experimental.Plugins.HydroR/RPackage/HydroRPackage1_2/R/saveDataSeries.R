saveDataSeries <-
function(connectionString, newSeries, SQLite = TRUE, overwrite= TRUE)
{
	if(SQLite==TRUE)
  {
		driver<-dbDriver("SQLite")
    con<- dbConnect(driver, dbname=connectionString)
    dbBeginTransaction(con)
		
		siteID<-getSiteID(con, newSeries$Site, overwrite)
		vunitsID<-getUnitsID(con, newSeries$VariableUnits,overwrite)
		tunitsID<-getUnitsID(con, newSeries$TimeUnits,overwrite)	
		variableID<-getVariableID(con, newSeries$Variable, overwrite)		
		methodID<-getMethodID(con, newSeries$Method, overwrite)		
		sourceID<-getSourceID(con, newSeries$Source, overwrite)		
		qclID<-getQualityControlLevelID(con, newSeries$QualityControlLevel, overwrite)
			
		SeriesID<-getSeriesID(con, list(site=siteID, variable=variableID, method=methodID, qcl=qclID, sources=sourceID), newSeries$DataSeries, newSeries$DataValues , overwrite)
		   
       
    ser<-newSeries$DataValues
		for(i in 1:length(newSeries$DataValues$DataValue))
    {		                                               			
		    if(is.null(ser$DataValue[i])|| is.null(ser$LocalDateTime[i])|| is.null(ser$UTCOffset[i])||  is.null(ser$CensorCode[i])||is.null(ser$DateTimeUTC[i])|| !nchar(ser$DataValue[i])|| !nchar(ser$LocalDateTime[i])|| !nchar(ser$UTCOffset[i])|| !nchar(ser$CensorCode[i])|| !nchar(ser$DateTimeUTC[i]))
			   {
			     dbDisconnect(con) 
				    stop(paste("All of the Mandatory Values information was not filled out for row #", i, sep= ""))                    				
			   }
        else
        {
			     if(overwrite==FALSE)
            {			
				    #print( paste("INSERT INTO DataValues VALUES(",getNextValueID(con),", ",SeriesID,", ", ser$DataValue[i],", ",dbFormat(ser$ValueAccuracy[i]),", '", format(ser$LocalDateTime[i],"%Y-%m-%d %H:%M:%S"),"', ",dbFormat(ser$UTCOffset[i]),", '",format(ser$DateTimeUTC[i],"%Y-%m-%d %H:%M:%S"),"', ", dbFormat(ser$OffsetValue[i]),", ", dbFormat(ser$OffsetTypeID[i]),", ", dbFormat(ser$CensorCode[i]),", ", dbFormat(ser$QualifierID[i]),", ", dbFormat(ser$SampleID[i]),", ", dbFormat(ser$FileID[i]), ")", sep=""))
				    dbGetQuery(con,paste("INSERT INTO DataValues VALUES(",getNextValueID(con),", ",SeriesID,", ", ser$DataValue[i],", ",dbFormat(ser$ValueAccuracy[i]),", '",ser$LocalDateTime[i],"', ",dbFormat(ser$UTCOffset[i]),", '",ser$DateTimeUTC[i],"', ", dbFormat(ser$OffsetValue[i]),", ", dbFormat(ser$OffsetTypeID[i]),", ", dbFormat(ser$CensorCode[i]),", ", dbFormat(ser$QualifierID[i]),", ", dbFormat(ser$SampleID[i]),", ", dbFormat(ser$FileID[i]), ")", sep=""))
			     }	
			     else
          {	
				    count<-dbGetQuery(con, paste("SELECT COUNT(DataValue) FROM DataValues WHERE SeriesID=",SeriesID, " AND LocalDateTime = '", newSeries$DataValues$LocalDateTime[i],"'", sep=""))			
				    if (count==0)
            {
					     #print(paste("INSERT INTO DataValues VALUES(",getNextValueID(con),", ",dbFormat(SeriesID),", ", dbFormat(ser$DataValue[i]),", ",	dbFormat(ser$ValueAccuracy[i]),", '", format(ser$LocalDateTime[i],"%Y-%m-%d %H:%M:%S"),"', ",dbFormat(ser$UTCOffset[i]),", '",format(ser$DateTimeUTC[i],"%Y-%m-%d %H:%M:%S"),"', ", dbFormat(ser$OffsetValue[i]),", '", dbFormat(ser$OffsetTypeID[i]),"', '", dbFormat(ser$CensorCode[i]),"', '", dbFormat(ser$QualifierID[i]),"', '", dbFormat(ser$SampleID[i]),"', '", dbFormat(ser$FileID[i]), "')", sep=""))
					     dbGetQuery(con,paste("INSERT INTO DataValues VALUES(",getNextValueID(con),", ",dbFormat(SeriesID),", ", dbFormat(ser$DataValue[i]),", ",	dbFormat(ser$ValueAccuracy[i]),", '", ser$LocalDateTime[i],"', ",dbFormat(ser$UTCOffset[i]),", '",ser$DateTimeUTC[i],"', ", dbFormat(ser$OffsetValue[i]),", '", dbFormat(ser$OffsetTypeID[i]),"', '", dbFormat(ser$CensorCode[i]),"', '", dbFormat(ser$QualifierID[i]),"', '", dbFormat(ser$SampleID[i]),"', '", dbFormat(ser$FileID[i]), "')", sep=""))
				    }
		        else
            {
					     #print(paste("UPDATE DataValues SET DataValue=", newSeries$DataValues$DataValue[i], " WHERE SeriesID=",SeriesID, " AND LocalDateTime ='", format(newSeries$DataValues$LocalDateTime[i],"%Y-%m-%d %H:%M:%S"),"'", sep=""))		
					     dbGetQuery(con, paste("UPDATE DataValues SET DataValue=", newSeries$DataValues$DataValue[i], " WHERE SeriesID=",SeriesID, " AND LocalDateTime ='", newSeries$DataValues$LocalDateTime[i],"'", sep=""))		
		        }
			    } 
        }		
		  }			
		  dbCommit(con)	  
  	  dbDisconnect(con)		
	}    
	
	getDataSeries(connectionString, SeriesID, SQLite, startDate=min(as.character(newSeries$DataValues$LocalDateTime)), endDate=max(as.character(newSeries$DataValues$LocalDateTime)))
}

                  dbFormat<-function(val){
	if(is.null(val)|| val== ''|| is.na(val))
  {
	 "NULL"	
	}
	else 
  {
		if(is.character(val))
    {
			paste("'", val,"'", sep="") 
		}
		else
    {
			val
		}	
	}
}



getNextValueID<-function(con){	
	dbGetQuery(con, paste("SELECT MAX(ValueID) FROM DataValues"))+1
}

getNextSeriesID<-function(con){
	dbGetQuery(con, paste("SELECT MAX(SeriesID) FROM DataSeries"))+1	
}

getSeriesID<-function(con, S, Series, Values, overwrite){
	if(overwrite==TRUE )
  {
		seriesID<-dbGetQuery(con, paste("SELECT SeriesID FROM DataSeries WHERE SiteID=",Series$SiteID, " AND VariableID=", Series$VariableID, " AND MethodID=",Series$MethodID," AND QualityControlLevelID=",Series$QualityControlLevelID ," AND SourceID=",Series$SourceID, sep="" ))
		dbGetQuery(con, paste("UPDATE DataSeries SET UpdateDateTime= '", strptime(Sys.time(),"%Y-%m-%d %H:%M:%S"), "', ValueCount=", nrow(Values),", BeginDateTime='",min(Values$LocalDateTime),"', EndDateTime='",max(Values$LocalDateTime),"', BeginDateTimeUTC='",min(Values$DateTimeUTC),"',EndDateTimeUTC='",max(Values$DateTimeUTC),"' WHERE SeriesID=", seriesID, sep="" ))
	}
	else{
		#print(paste("SELECT SeriesID FROM DataSeries WHERE SiteID=",S$site, " AND VariableID=", S$variable, " AND MethodID=",S$method," AND QualityControlLevelID=",S$qcl," AND SourceID=",S$sources, sep="" ))
		seriesID<-dbGetQuery(con, paste("SELECT SeriesID FROM DataSeries WHERE SiteID=",S$site, " AND VariableID=", S$variable, " AND MethodID=",S$method," AND QualityControlLevelID=",S$qcl," AND SourceID=",S$sources, sep="" ))
			if(is.null(S$site)|| is.null(S$variable)|| is.null(S$method)|| is.null(S$qcl)|| is.null(S$sources)|| !nchar(S$site)|| !nchar(S$variable)|| !nchar(S$method)|| !nchar(S$qcl) || !nchar(S$sources))
      {
				dbDisconnect(con) 
        #tkmessageBox(message="All of the Mandatory Series information was not filled out, the Series data will not be written to the database",icon="error",type="ok")
				stop("All of the Mandatory Series information was not filled out")
				
        #print(paste(S$site, S$variable, S$method, S$qcl, S$source,nchar(S$site), nchar(S$variable),nchar(S$method),nchar(S$qcl) ,nchar(S$sources), sep=", "))
			}
			else
      {
				#if(S$site==Series$SiteID && S$variable==Series$VariableID && S$method==Series$MethodID && S$sources==Series$SourceID && S$qcl==Series$QualityControlLevelID )
        #{
				#	tkmessageBox(message="You have Not edited your Metadata but have set overwrite==FALSE. You may end up with duplicate entries in your database.",icon="error",type="ok")
				#}
        #else
        #{
        
        Values$LocalDateTime<-as.character(Values$LocalDateTime)
        Values$DateTimeUTC<-as.character(Values$DateTimeUTC)
				  if(nrow(seriesID)==0)
          {
					 # print(paste("INSERT INTO DataSeries VALUES(",getNextSeriesID(con),", ",dbFormat(S$site),", ", dbFormat(S$variable),", ", dbFormat(Series$IsCategorical),", ", dbFormat(S$method),", ", dbFormat(S$sources),", ", dbFormat(S$qcl),", '", format(min(Values$LocalDateTime),"%Y-%m-%d %H:%M:%S"),"', '", format(max(Values$LocalDateTime),"%Y-%m-%d %H:%M:%S"),"', '", format(min(Values$DateTimeUTC),"%Y-%m-%d %H:%M:%S"),"', '", format(max(Values$DateTimeUTC),"%Y-%m-%d %H:%M:%S"),"', ", dbFormat(nrow(Values)),", '", format(Sys.time(),"%Y-%m-%d %H:%M:%S"),"', ", dbFormat(Series$Subscribed),", '", format(Sys.time(),"%Y-%m-%d %H:%M:%S"),"', '", format(Sys.time(),"%Y-%m-%d %H:%M:%S"),"')",sep=""))
					 dbGetQuery(con,paste("INSERT INTO DataSeries VALUES(",getNextSeriesID(con),", ",dbFormat(S$site),", ", dbFormat(S$variable),", ",dbFormat(Series$IsCategorical),", ", dbFormat(S$method),", ", dbFormat(S$sources),", ", dbFormat(S$qcl),", '", min(Values$LocalDateTime),"', '", max(Values$LocalDateTime),"', '", min(Values$DateTimeUTC),"', '", max(Values$DateTimeUTC),"', ", dbFormat(nrow(Values)),", '", strptime(Sys.time(),"%Y-%m-%d %H:%M:%S"),"', ", dbFormat(Series$Subscribed),", '", strptime(Sys.time(),"%Y-%m-%d %H:%M:%S"),"', '", strptime(Sys.time(),"%Y-%m-%d %H:%M:%S"),"')",sep=""))
			 	   }
			 	   else
            {
            query= paste("UPDATE DataSeries SET BeginDateTime='", min(Values$LocalDateTime),"', EndDateTime='", max(Values$LocalDateTime),"', BeginDateTimeUTC='", min(Values$DateTimeUTC),"', EndDateTimeUTC='", max(Values$DateTimeUTC),"', ValueCount=", dbFormat(nrow(Values)),", UpdateDateTime='", strptime(Sys.time(),"%Y-%m-%d %H:%M:%S"),"',  LastCheckedDateTime= '", strptime(Sys.time(),"%Y-%m-%d %H:%M:%S"),"' where SeriesID = " ,seriesID ,sep="")
            			 	   dbGetQuery(con,paste("UPDATE DataSeries SET BeginDateTime='", min(Values$LocalDateTime),"', EndDateTime='", max(Values$LocalDateTime),"', BeginDateTimeUTC='", min(Values$DateTimeUTC),"', EndDateTimeUTC='", max(Values$DateTimeUTC),"', ValueCount=", dbFormat(nrow(Values)),", UpdateDateTime='", strptime(Sys.time(),"%Y-%m-%d %H:%M:%S"),"',  LastCheckedDateTime= '", strptime(Sys.time(),"%Y-%m-%d %H:%M:%S"),"' where SeriesID = " ,seriesID ,sep=""))
			 	   
			 	   }
				#}
				#print(paste("SELECT SeriesID FROM DataSeries WHERE SiteID=",S$site, " AND VariableID=", S$variable, " AND MethodID=",S$method," AND QualityControlLevelID=",S$qcl," AND SourceID=",S$sources, sep="" ))
				seriesID<-dbGetQuery(con, paste("SELECT SeriesID FROM DataSeries WHERE SiteID=",S$site, " AND VariableID=", S$variable, " AND MethodID=",S$method," AND QualityControlLevelID=",S$qcl," AND SourceID=",S$sources, sep="" ))
			}   			
	}
	seriesID
}

getSiteID<-function(con, Site, overwrite){
#Mandatory: siteID, siteCode, siteName, Latitude, longitude, LatLongDatumID
	#print(paste("SELECT SiteID FROM Sites WHERE SiteCode='",Site$SiteCode, "' AND SiteName='", Site$SiteName, "'", sep="" ))
	siteID<-dbGetQuery(con, paste("SELECT SiteID FROM Sites WHERE SiteCode='",Site$SiteCode, "'AND SiteName='", Site$SiteName, "'", sep="" ))
	if(nrow(siteID)==0)
  {
		if(overwrite==TRUE){
			#print("You have edited your site information but have set overwrite==TRUE so the data is not being written to the database. Overwrite=TRUE means you want to change the datavalues but keep the rest of the data the same")
			tkmessageBox(message="You have edited your site information but have set overwrite==TRUE so the data is not being written to the database.",icon="error",type="ok")
		}
		else{
			if(is.null(Site$SiteCode)|| is.null(Site$SiteName)|| is.null(Site$Latitude)|| is.null(Site$Longitude)|| is.null(Site$LatLongDatumID)|| !nchar(Site$SiteCode)|| !nchar(Site$SiteName)|| !nchar(Site$Latitude)|| !nchar(Site$Longitude) || !nchar(Site$LatLongDatumID))
      {
				#print(paste(Site$SiteCode, Site$SiteName, Site$Latitude, Site$Longitude, Site$LatLongDatumID,nchar(Site$SiteCode), nchar(Site$SiteName),nchar(Site$Latitude),nchar(Site$Longitude) ,nchar(Site$LatLongDatumID), sep=", "))
				#tkmessageBox(message="All of the Mandatory site information was not filled out, the Site data will not be written to the database",icon="error",type="ok")
				dbDisconnect(con) 
        stop("All of the Mandatory Site information was not filled out")
			}
      else
      {
				#print(paste("INSERT INValueO Sites VALUES(",dbGetQuery(con, paste("SELECT MAX(SiteID) FROM Sites"))+1,", ", dbFormat(Site$SiteCode),", ", dbFormat(Site$SiteName),", ", dbFormat(Site$Latitude),", ", dbFormat(Site$Longitude),", ",dbFormat(Site$LatLongDatumID),", ", dbFormat(Site$Elevation_m),", ", dbFormat(Site$VerticalDatum),", ", dbFormat(Site$LocalX),", ", dbFormat(Site$LocalY),", ",dbFormat(Site$LocalProjectionID),", ", dbFormat(Site$PosAccuracy_m),", ", dbFormat(Site$State),", ", dbFormat(Site$County),", ", dbFormat(Site$Comments) , ")", sep = ""))
				dbGetQuery(con, paste("INSERT INTO Sites VALUES(",dbGetQuery(con, paste("SELECT MAX(SiteID) FROM Sites"))+1,", ", dbFormat(Site$SiteCode),", ", dbFormat(Site$SiteName),", ", dbFormat(Site$Latitude),", ", dbFormat(Site$Longitude),", ",dbFormat(Site$LatLongDatumID),", ", dbFormat(Site$Elevation_m),", ", dbFormat(Site$VerticalDatum),", ", dbFormat(Site$LocalX),", ", dbFormat(Site$LocalY),", ",dbFormat(Site$LocalProjectionID),", ", dbFormat(Site$PosAccuracy_m),", ", dbFormat(Site$State),", ", dbFormat(Site$County),", ", dbFormat(Site$Comments) , ")", sep = ""))
				siteID<-dbGetQuery(con, paste("SELECT SiteID FROM Sites WHERE SiteCode='",Site$SiteCode, "'AND SiteName='", Site$SiteName, "'", sep="" ))
				
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
			if( is.null(Variable$VariableCode)|| !nchar(Variable$VariableCode)|| is.null(Variable$VariableName)|| !nchar(Variable$VariableName)|| is.null(Variable$Speciation)|| !nchar(Variable$Speciation)|| is.null(Variable$VariableUnitsID)|| !nchar(Variable$VariableUnitsID)|| is.null(Variable$SampleMedium)|| !nchar(Variable$SampleMedium)|| is.null(Variable$ValueType)|| !nchar(Variable$ValueType)|| is.null(Variable$IsRegular)|| !nchar(Variable$IsRegular)|| is.null(Variable$TimeSupport)|| !nchar(Variable$TimeSupport)|| is.null(Variable$TimeUnitsID)|| !nchar(Variable$TimeUnitsID)|| is.null(Variable$DataType)|| !nchar(Variable$DataType)|| is.null(Variable$GeneralCategory)|| !nchar(Variable$GeneralCategory)|| is.null(Variable$NoDataValue)|| !nchar(Variable$NoDataValue))
      {
				#tkmessageBox(message="All of the Mandatory Variable information was not filled out, the Variable data will not be written to the database",icon="error",type="ok")
				dbDisconnect(con) 
        stop("All of the Mandatory Variable information was not filled out")
			}
      else
      {
				#print(paste("INSERT INTO Variables VALUES(",dbGetQuery(con, paste("SELECT MAX(VariableID) FROM Variables"))+1,", ", dbFormat(Variable$VariableCode),", ", dbFormat(Variable$VariableName),", ", dbFormat(Variable$Speciation),", ", dbFormat(Variable$VariableUnitsID),", ",dbFormat(Variable$SampleMedium),", ", dbFormat(Variable$ValueType),", ", dbFormat(Variable$IsRegular),", ", dbFormat(Variable$IsCategorical),", ", dbFormat(Variable$TimeSupport),", ", dbFormat(Variable$TimeUnitsID),", ", dbFormat(Variable$DataType),", ", dbFormat(Variable$GeneralCategory),", ", dbFormat(Variable$NoDataValue),", ", dbFormat(Variable$ConceptID) , ")", sep = ""))
				dbGetQuery(con, paste("INSERT INTO Variables VALUES(",dbGetQuery(con, paste("SELECT MAX(VariableID) FROM Variables"))+1,", ", dbFormat(Variable$VariableCode),", ", 
        dbFormat(Variable$VariableName),", ", dbFormat(Variable$Speciation),", ", dbFormat(Variable$VariableUnitsID),", ",dbFormat(Variable$SampleMedium),", ", dbFormat(Variable$ValueType),", ", 
        dbFormat(Variable$IsRegular),", ", dbFormat(Variable$IsCategorical),", ", dbFormat(Variable$TimeSupport),", ", dbFormat(Variable$TimeUnitsID),", ", dbFormat(Variable$DataType),", ", 
        dbFormat(Variable$GeneralCategory),", ", dbFormat(Variable$NoDataValue),", ", dbFormat(Variable$ConceptID) , ")", sep = ""))
				variableID<-dbGetQuery(con, paste("SELECT VariableID FROM Variables WHERE VariableCode='", Variable$VariableCode, "' AND VariableName='",Variable$VariableName,"' AND VariableUnitsID=",Variable$VariableUnitsID, sep=""))		
			
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
			if(is.null(Method$MethodDescription)|| !nchar(Method$MethodDescription)||is.null(Method$MethodLink)|| !nchar(Method$MethodLink)){
				#tkmessageBox(message="All of the Mandatory Method information was not filled out, the Method data will not be written to the database",icon="error",type="ok")
				dbDisconnect(con) 
        stop("All of the Mandatory Method information was not filled out")
			}else{
				#print(paste("INSERT INTO Methods VALUES(",dbGetQuery(con, paste("SELECT MAX(MethodID) FROM Methods"))+1,", '", Method$MethodDescription,"', '", Method$MethodLink,"')", sep = ""))
			    query<- paste("INSERT INTO Methods VALUES(",dbGetQuery(con, paste("SELECT MAX(MethodID) FROM Methods"))+1,", ", dbFormat(Method$MethodDescription),", ", dbFormat(Method$MethodLink),")", sep = "")
      	dbGetQuery(con, paste("INSERT INTO Methods VALUES(",dbGetQuery(con, paste("SELECT MAX(MethodID) FROM Methods"))+1,", ", dbFormat(Method$MethodDescription),", ", dbFormat(Method$MethodLink),")", sep = ""))
				methodID<-dbGetQuery(con, paste("SELECT MethodID FROM Methods WHERE MethodLink=", dbFormat(Method$MethodLink), " AND MethodDescription=",dbFormat(Method$MethodDescription), "", sep=""))				
				
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
		else
    {	
			if(is.null(Sources$Organization)|| !nchar(Sources$Organization)||  is.null(Sources$SourceDescription) || !nchar(Sources$SourceDescription)|| is.null(Sources$ContactName)|| !nchar(Sources$ContactName)|| is.null(Sources$Phone)|| !nchar(Sources$Phone)|| is.null(Sources$Email)|| !nchar(Sources$Email)|| is.null(Sources$Address)|| !nchar(Sources$Address)||  is.null(Sources$City)|| !nchar(Sources$City)|| is.null(Sources$State)|| !nchar(Sources$State)|| is.null(Sources$ZipCode)|| !nchar(Sources$ZipCode)|| is.null(Sources$Citation)|| !nchar(Sources$Citation)|| is.null(Sources$MetadataID)|| !nchar(Sources$MetadataID))
      {
				#tkmessageBox(message="All of the Mandatory Source information was not filled out, the Source data will not be written to the database",icon="error",type="ok")
				dbDisconnect(con) 
        stop("All of the Mandatory Source information was not filled out")
			}
      else
      {	
				#print(paste("INSERT INTO Sources VALUES(",dbGetQuery(con, paste("SELECT MAX(SourceID) FROM Sources"))+1,", ",dbFormat(Sources$Organization),", ",dbFormat(Sources$SourceDescription),", ",dbFormat(Sources$SourceLink),", ",dbFormat(Sources$ContactName),", ",dbFormat(Sources$Phone),", ",dbFormat(Sources$Email),", ",dbFormat(Sources$Address),", ",dbFormat(Sources$City),", ",dbFormat(Sources$State),", ",dbFormat(Sources$ZipCode),", ",dbFormat(Sources$Citation),", ",dbFormat(Sources$MetadataID) ,")", sep = ""))
				dbGetQuery(con, paste("INSERT INTO Sources VALUES(",dbGetQuery(con, paste("SELECT MAX(SourceID) FROM Sources"))+1,", ",dbFormat(Sources$Organization),", ",dbFormat(Sources$SourceDescription),", ",dbFormat(Sources$SourceLink),", ",dbFormat(Sources$ContactName),", ",dbFormat(Sources$Phone),", ",dbFormat(Sources$Email),", ",dbFormat(Sources$Address),", ",dbFormat(Sources$City),", ",dbFormat(Sources$State),", ",dbFormat(Sources$ZipCode),", ",dbFormat(Sources$Citation),", ",dbFormat(Sources$MetadataID) ,")", sep = ""))
				sourceID<-dbGetQuery(con, paste("SELECT SourceID FROM Sources WHERE Organization='", Sources$Organization, "' AND SourceLink='",Sources$SourceLink, "' AND ContactName='",Sources$ContactName,"' AND City='",Sources$City,"'", sep=""))				
				
			}
		}			
	}
	sourceID
}

getQualityControlLevelID<-function(con, QCL, overwrite){
#Mandatory: QualityControlLevelID, QualityControlLevelCode, Definition, Explanation
	#print(paste("SELECT QualityControlLevelID FROM QualityControlLevels WHERE QualityControlLevelCode='", QCL$QualityControlLevelCode, "' AND Definition='",QCL$Definition, "'", sep=""))				
	qclID<-dbGetQuery(con, paste("SELECT QualityControlLevelID FROM QualityControlLevels WHERE QualityControlLevelCode='", QCL$QualityControlLevelCode, "' AND Definition='",QCL$Definition, "'", sep=""))				
	if(nrow(qclID)==0){
		if(overwrite==TRUE){
			tkmessageBox(message="You have edited your Quality Control information but have set overwrite==TRUE so the data is not being written to the database.",icon="error",type="ok")
		}
		else{
			if(is.null(QCL$QualityControlLevelCode)|| !nchar(QCL$QualityControlLevelCode)||is.null(QCL$Definition)|| !nchar(QCL$Definition)|| is.null(QCL$Explanation)|| !nchar(QCL$Explanation))
      { 
				#tkmessageBox(message="All of the Mandatory Quality Control information was not filled out, the Qualty Control data will not be written to the database",icon="error",type="ok")
				dbDisconnect(con) 
        stop("All of the Mandatory Quality Control information was not filled out")
			}
      else
      {	
				#print(paste("INSERT INTO QualityControlLevels VALUES(",dbGetQuery(con, paste("SELECT MAX(QualityControlLevelID) FROM QualityControlLevels"))+1,", ", dbFormat(QCL$QualityControlLevelCode),", ", dbFormat(QCL$Definition),", ", dbFormat(QCL$Explanation),")", sep = ""))
				dbGetQuery(con, paste("INSERT INTO QualityControlLevels VALUES(",dbGetQuery(con, paste("SELECT MAX(QualityControlLevelID) FROM QualityControlLevels"))+1,", ", dbFormat(QCL$QualityControlLevelCode),", ", dbFormat(QCL$Definition),", ", dbFormat(QCL$Explanation),")", sep = ""))
				qclID<-dbGetQuery(con, paste("SELECT QualityControlLevelID FROM QualityControlLevels WHERE QualityControlLevelCode='", QCL$QualityControlLevelCode, "' AND Definition='",QCL$Definition, "'", sep=""))				
		
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
				dbDisconnect(con) 
        stop("All of the Mandatory Units information was not filled out")
			}
      else
      {	
				#print(paste("INSERT INTO Units VALUES(",dbGetQuery(con, paste("SELECT MAX(UnitsID) FROM Units"))+1,", ",dbFormat(Units$UnitsName),", ",dbFormat(Units$UnitsType),", ",dbFormat(Units$UnitsAbbreviation),")", sep = "")) 
				dbGetQuery(con,paste("INSERT INTO Units VALUES(",dbGetQuery(con, paste("SELECT MAX(UnitsID) FROM Units"))+1,", ",dbFormat(Units$UnitsName),", ",dbFormat(Units$UnitsType),", ",dbFormat(Units$UnitsAbbreviation),")", sep = "")) 
				unitID<-dbGetQuery(con, paste("SELECT UnitsID FROM Units WHERE UnitsName='", Units$UnitsName, "' AND UnitsType='",Units$UnitsType, "' AND UnitsAbbreviation='",Units$UnitsAbbreviation,"'", sep=""))					
				
			}
		}			
	}	
	unitID	
}

