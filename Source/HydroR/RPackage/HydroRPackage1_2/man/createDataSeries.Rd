\name{createDataSeries}
\alias{createDataSeries}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{create a blank Data Series List}
\description{
%%  ~~ A concise (1-5 lines) description of what the function does. ~~
}
\usage{
createDataSeries(connectionString, SQLite = TRUE)
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{connectionString}{
%%     ~~Describe \code{connectionString} here~~
}
  \item{SQLite}{
%%     ~~Describe \code{SQLite} here~~
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
function(connectionString, SQLite= TRUE)
{     
    if(SQLite==TRUE) 
    {      
        driver<-dbDriver("SQLite")
        con<- dbConnect(driver, dbname=connectionString)
		    dbBeginTransaction(con)                                 
		    dValues<-createDataValues(con, SQLite)     
        dSeries<- dbGetQuery(con, "SELECT * FROM DataSeries WHERE SeriesID = -99999999")    
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
}
% Add one or more standard keywords, see file 'KEYWORDS' in the
% R documentation directory.
\keyword{ ~kwd1 }
\keyword{ ~kwd2 }% __ONLY ONE__ keyword per line
