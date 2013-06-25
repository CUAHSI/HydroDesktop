\name{saveDataSeries}
\alias{saveDataSeries}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{Takes a Data Series List and saves it to the database}
\description{
%%  ~~ A concise (1-5 lines) description of what the function does. ~~
}
\usage{
saveDataSeries(connectionString, newSeries, SQLite = TRUE, overwrite = TRUE)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{connectionString}{
%%     ~~Describe \code{connectionString} here~~
}
  \item{newSeries}{
%%     ~~Describe \code{newSeries} here~~
}
  \item{SQLite}{
%%     ~~Describe \code{SQLite} here~~
}
  \item{overwrite}{
%%     ~~Describe \code{overwrite} here~~
}
}
\details{
%%  ~~ If necessary, more details than the description above ~~
}
\value{
%%  ~Describe the value returned
%%  If it is a LIST, use
%%  \item{comp1 }{Description of 'comp1'}
%%  \item{comp2 }{Description of 'comp2'}
%% ...
}
\references{
%% ~put references to the literature/web site here ~
}
\author{
%%  ~~who you are~~
}
\note{
%%  ~~further notes~~
}

%% ~Make other sections like Warning with \section{Warning }{....} ~

\seealso{
%% ~~objects to See Also as \code{\link{help}}, ~~~
}
\examples{
##---- Should be DIRECTLY executable !! ----
##-- ==>  Define data, use random,
##--	or do  help(data=index)  for the standard data sets.

## The function is currently defined as
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
#print( paste("INSERT INTO DataValues VALUES(",getNextValueID(con),", ",SeriesID,", ", ser$DataValue[i],", ",ser$ValueAccuracy[i],", '", format(ser$LocalDateTime[i],"\%Y-\%m-\%d \%H:\%M:\%S"),"', ",ser$UTCOffset[i],", '",format(ser$DateTimeUTC[i],"\%Y-\%m-\%d \%H:\%M:\%S"),"', ", ser$OffsetValue[i],", '", ser$OffsetTypeID[i],"', '", ser$CensorCode[i],"', '", ser$QualifierID[i],"', '", ser$SampleID[i],"', '", ser$FileID[i], "')", sep=""))
dbGetQuery(con,paste("INSERT INTO DataValues VALUES(",getNextValueID(con),", ",SeriesID,", ", ser$DataValue[i],", ",ser$ValueAccuracy[i],", '", format(ser$LocalDateTime[i],"\%Y-\%m-\%d \%H:\%M:\%S"),"', ",ser$UTCOffset[i],", '",format(ser$DateTimeUTC[i],"\%Y-\%m-\%d \%H:\%M:\%S"),"', ", ser$OffsetValue[i],", '", ser$OffsetTypeID[i],"', '", ser$CensorCode[i],"', '", ser$QualifierID[i],"', '", ser$SampleID[i],"', '", ser$FileID[i], "')", sep=""))
  }
else{
count<-dbGetQuery(con, paste("SELECT COUNT(DataValue) FROM DataValues WHERE SeriesID=",SeriesID, " AND LocalDateTime = '", format(newSeries$DataValues$LocalDateTime[i],"\%Y-\%m-\%d \%H:\%M:\%S"),"'", sep=""))
if (count==0){
#print(paste("INSERT INTO DataValues VALUES(",getNextValueID(con),", ",SeriesID,", ", ser$DataValue[i],", ",ser$ValueAccuracy[i],", '", format(ser$LocalDateTime[i],"\%Y-\%m-\%d \%H:\%M:\%S"),"', ",ser$UTCOffset[i],", '",format(ser$DateTimeUTC[i],"\%Y-\%m-\%d \%H:\%M:\%S"),"', ", ser$OffsetValue[i],", '", ser$OffsetTypeID[i],"', '", ser$CensorCode[i],"', '", ser$QualifierID[i],"', '", ser$SampleID[i],"', '", ser$FileID[i], "')", sep=""))
dbGetQuery(con,paste("INSERT INTO DataValues VALUES(",getNextValueID(con),", ",SeriesID,", ", ser$DataValue[i],", ",ser$ValueAccuracy[i],", '", format(ser$LocalDateTime[i],"\%Y-\%m-\%d \%H:\%M:\%S"),"', ",ser$UTCOffset[i],", '",format(ser$DateTimeUTC[i],"\%Y-\%m-\%d \%H:\%M:\%S"),"', ", ser$OffsetValue[i],", '", ser$OffsetTypeID[i],"', '", ser$CensorCode[i],"', '", ser$QualifierID[i],"', '", ser$SampleID[i],"', '", ser$FileID[i], "')", sep=""))
  }
else{
#print(paste("UPDATE DataValues SET DataValue=", newSeries$DataValues$DataValue[i], " WHERE SeriesID=",SeriesID, " AND LocalDateTime ='", format(newSeries$DataValues$LocalDateTime[i],"\%Y-\%m-\%d \%H:\%M:\%S"),"'", sep=""))
dbGetQuery(con, paste("UPDATE DataValues SET DataValue=", newSeries$DataValues$DataValue[i], " WHERE SeriesID=",SeriesID, " AND LocalDateTime ='", format(newSeries$DataValues$LocalDateTime[i],"\%Y-\%m-\%d \%H:\%M:\%S"),"'", sep=""))
  }
  } 
  }
dbCommit(con)  
    dbDisconnect(con)
  }
  }
}
% Add one or more standard keywords, see file 'KEYWORDS' in the
% R documentation directory.
\keyword{ ~kwd1 }
\keyword{ ~kwd2 }% __ONLY ONE__ keyword per line
