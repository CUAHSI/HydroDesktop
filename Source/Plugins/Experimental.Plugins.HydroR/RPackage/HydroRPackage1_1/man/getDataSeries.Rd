\name{getDataSeries}
\alias{getDataSeries}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{get data and Metadata from database and stores it in a List}
\description{
%%  ~~ A concise (1-5 lines) description of what the function does. ~~
}
\usage{
getDataSeries(connectionString, seriesID, SQLite = TRUE, startDate = "1900-01-01", endDate = "2050-12-31")
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{connectionString}{
%%     ~~Describe \code{connectionString} here~~
}
  \item{seriesID}{
%%     ~~Describe \code{seriesID} here~~
}
  \item{SQLite}{
%%     ~~Describe \code{SQLite} here~~
}
  \item{startDate}{
%%     ~~Describe \code{startDate} here~~
}
  \item{endDate}{
%%     ~~Describe \code{endDate} here~~
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
function(connectionString, seriesID, SQLite= TRUE, startDate="1900-01-01", endDate="2050-12-31"){     
    if(SQLite==TRUE) {      
        driver<-dbDriver("SQLite")
        con<- dbConnect(driver, dbname=connectionString)
dbBeginTransaction(con)
dValues<-getDataValues(con, seriesID, SQLite ,startDate, endDate) 
        dSeries<- dbGetQuery(con, paste("SELECT * FROM DataSeries WHERE SeriesID = " ,seriesID, sep="" )) 
        dSeries$BeginDateTime<-as.Date(dSeries$BeginDateTime, "\%Y-\%m-\%d \%H:\%M:\%S")
        dSeries$BeginDateTimeUTC<-as.Date(dSeries$BeginDateTimeUTC, "\%Y-\%m-\%d \%H:\%M:\%S")
        dSeries$EndDateTime<-as.Date(dSeries$EndDateTime, "\%Y-\%m-\%d \%H:\%M:\%S")
        dSeries$EndDateTimeUTC<-as.Date(dSeries$EndDateTimeUTC, "\%Y-\%m-\%d \%H:\%M:\%S")         
        dSeries$CreationDateTime<-as.Date(dSeries$CreationDateTime, "\%Y-\%m-\%d \%H:\%M:\%S")  
dSeries$UpdateDateTime<-as.Date(dSeries$UpdateDateTime, "\%Y-\%m-\%d \%H:\%M:\%S")  
dSeries$LastcheckedDateTime<-as.Date(dSeries$LastcheckedDateTime, "\%Y-\%m-\%d \%H:\%M:\%S")  
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
}
% Add one or more standard keywords, see file 'KEYWORDS' in the
% R documentation directory.
\keyword{ ~kwd1 }
\keyword{ ~kwd2 }% __ONLY ONE__ keyword per line
