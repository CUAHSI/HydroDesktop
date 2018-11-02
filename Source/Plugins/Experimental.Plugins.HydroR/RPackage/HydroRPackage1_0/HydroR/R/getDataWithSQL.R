getDataWithSQL <-
function(connectionString, SQLString, SQLite= TRUE)   {
    if(SQLite==TRUE) {
        driver<-dbDriver("SQLite")
        con <- dbConnect(driver, dbname=connectionString)
        return(dbGetQuery(con, SQLString) )
      }      
}

