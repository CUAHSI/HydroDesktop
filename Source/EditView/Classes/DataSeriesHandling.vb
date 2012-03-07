Imports HydroDesktop.Database
Imports HydroDesktop.Configuration
Imports HydroDesktop.Interfaces
Imports System.Globalization

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
        BeginDateTime = Convert.ToDateTime(dbTools.ExecuteSingleOutput(SQLstring), CultureInfo.InvariantCulture)
        SQLstring = "SELECT LocalDateTime FROM DataValues WHERE SeriesID = " + SeriesID.ToString + " ORDER BY LocalDateTime DESC"
        EndDateTime = Convert.ToDateTime(dbTools.ExecuteSingleOutput(SQLstring), CultureInfo.InvariantCulture)
        SQLstring = "SELECT DateTimeUTC FROM DataValues WHERE SeriesID = " + SeriesID.ToString + " ORDER BY LocalDateTime ASC"
        BeginDateTimeUTC = Convert.ToDateTime(dbTools.ExecuteSingleOutput(SQLstring), CultureInfo.InvariantCulture)
        SQLstring = "SELECT DateTimeUTC FROM DataValues WHERE SeriesID = " + SeriesID.ToString + " ORDER BY LocalDateTime DESC"
        EndDateTimeUTC = Convert.ToDateTime(dbTools.ExecuteSingleOutput(SQLstring), CultureInfo.InvariantCulture)
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
