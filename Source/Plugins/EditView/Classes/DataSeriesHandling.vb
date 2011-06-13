Imports HydroDesktop.Database
Imports HydroDesktop.Configuration
Imports HydroDesktop.Interfaces

Public Class DataSeriesHandling

    Public Shared Sub UpdateDataSeriesFromDataValues(ByVal SeriesID As Integer)
        Dim connString = Settings.Instance.DataRepositoryConnectionString
        Dim dbTools As New DbOperations(connString, DatabaseTypes.SQLite)
        Dim SQLstring As String

        Dim BeginDateTime As DateTime
        Dim EndDateTime As DateTime
        Dim BeginDateTimeUTC As DateTime
        Dim EndDateTimeUTC As DateTime
        Dim ValueCount As Integer

        SQLstring = "SELECT LocalDateTime FROM DataValues WHERE SeriesID = " + SeriesID.ToString + " ORDER BY LocalDateTime ASC"
        BeginDateTime = dbTools.ExecuteSingleOutput(SQLstring)
        SQLstring = "SELECT LocalDateTime FROM DataValues WHERE SeriesID = " + SeriesID.ToString + " ORDER BY LocalDateTime DESC"
        EndDateTime = dbTools.ExecuteSingleOutput(SQLstring)
        SQLstring = "SELECT DateTimeUTC FROM DataValues WHERE SeriesID = " + SeriesID.ToString + " ORDER BY LocalDateTime ASC"
        BeginDateTimeUTC = dbTools.ExecuteSingleOutput(SQLstring)
        SQLstring = "SELECT DateTimeUTC FROM DataValues WHERE SeriesID = " + SeriesID.ToString + " ORDER BY LocalDateTime DESC"
        EndDateTimeUTC = dbTools.ExecuteSingleOutput(SQLstring)
        SQLstring = "SELECT COUNT(*) FROM DataValues WHERE SeriesID = " + SeriesID.ToString
        ValueCount = dbTools.ExecuteSingleOutput(SQLstring)


        SQLstring = "UPDATE DataSeries SET BeginDateTime = '" + BeginDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', "
        SQLstring += "EndDateTime = '" + EndDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "', "
        SQLstring += "BeginDateTimeUTC = '" + BeginDateTimeUTC.ToString("yyyy-MM-dd HH:mm:ss") + "', "
        SQLstring += "EndDateTimeUTC = '" + EndDateTimeUTC.ToString("yyyy-MM-dd HH:mm:ss") + "', "
        SQLstring += "ValueCount = " + ValueCount.ToString + " WHERE SeriesID = " + SeriesID.ToString
        dbTools.ExecuteNonQuery(SQLstring)


    End Sub
End Class
