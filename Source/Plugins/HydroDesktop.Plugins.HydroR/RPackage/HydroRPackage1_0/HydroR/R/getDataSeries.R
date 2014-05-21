getDataSeries <-
function(connectionString, seriesID, SQLite= TRUE ,startDate="1900-01-01", endDate="2050-12-31")     
{       
      if(SQLite==TRUE) {
        driver<-dbDriver("SQLite")
        con <- dbConnect(driver, dbname=connectionString)
        return(dbGetQuery(con, paste("SELECT * FROM DataValues WHERE SeriesID = " ,seriesID, " AND  LocalDateTime BETWEEN \'", startDate,"\' AND \'", endDate,"\'", sep="" )) )
      }      
}

