\name{getDataSeries}
\alias{getDataSeries}
%- Also NEED an '\alias' for EACH other topic documented here.
\title{get data from database and stores it in a dataframe}
\description{
%%  ~~ A concise (1-5 lines) description of what the function does. ~~
}
\usage{
getDataSeries(connectionString, seriesID, SQLite, startDate = "1900-01-01", endDate = "2050-12-31")
}
%- maybe also 'usage' for other objects documented here.
\arguments{
  \item{connectionString}{
%%     ~~Describe \code{connectionString} if the Database is SQL lite this is the full file path to the Database File. Otherwise it should containt he connection string to connect to the Database.~~
}
  \item{seriesID}{
%%     ~~Describe \code{seriesID} The Series ID of the Data to collect from the HydroDesktop Database ~~
}
  \item{SQLite}{
%%     ~~Describe \code{SQLite}  Indicates wether the database being accessed is an SQLite Database or not. ~~
}
  \item{startDate}{
%%     ~~Describe \code{startDate} The Date you want to begin collecting data from the series.  Format: "yyyy-MM-dd hh:mm:ss" ~~
}
  \item{endDate}{
%%     ~~Describe \code{endDate} the Date you want to end collecting data from the series. Format: "yyyy-MM-dd hh:mm:ss"   ~~
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
function(connectionString, seriesID, SQLite ,startDate="2009-09-01", endDate="2009-09-02")     
{       
      if(SQLite==TRUE) {
        driver<-dbDriver("SQLite")
        con <- dbConnect(driver, dbname=connectionString)
        return(dbGetQuery(con, paste("SELECT * FROM DataValues WHERE SeriesID = " ,seriesID, " AND  LocalDateTime BETWEEN \'", startDate,"\' AND \'", endDate,"\'", sep="" )) )
      }      
  }
}
% Add one or more standard keywords, see file 'KEYWORDS' in the
% R documentation directory.
\keyword{ ~kwd1 }
\keyword{ ~kwd2 }% __ONLY ONE__ keyword per line
