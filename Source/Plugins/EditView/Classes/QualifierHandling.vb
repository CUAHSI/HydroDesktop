Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports HydroDesktop.Configuration

Public Class QualifierHandling
    'Qualifier Handling
    Public Shared Function GetQualifierID(ByVal QualifierCode As String) As Integer
        Dim connString = Settings.Instance.DataRepositoryConnectionString
        Dim dbTools As New DbOperations(connString, DatabaseTypes.SQLite)
        Dim dt As New DataTable()
        Dim SQLstring As String
        Dim QualifierID As Integer = dbTools.GetNextID("Qualifiers", "QualifierID")

        dt = dbTools.LoadTable("Qualifiers", "SELECT * FROM Qualifiers")

        For i As Integer = 0 To dt.Rows.Count - 1
            If dt.Rows(i)("QualifierCode").ToString.ToLower = QualifierCode.ToLower Then
                QualifierID = dt.Rows(i)("QualifierID")
                Return QualifierID
            End If
        Next

        SQLstring = "INSERT INTO Qualifiers(QualifierID, QualifierCode) VALUES ("
        SQLstring += QualifierID.ToString + ",'" + QualifierCode + "')"
        dbTools.ExecuteNonQuery(SQLstring)
        Return QualifierID

    End Function


    Public Shared Function GetQualifierCode(ByVal QualifierID As Integer)
        Dim connString = Settings.Instance.DataRepositoryConnectionString
        Dim dbTools As New DbOperations(connString, DatabaseTypes.SQLite)
        Dim QualifierCode As String

        QualifierCode = dbTools.ExecuteSingleOutput("SELECT QualifierCode FROM Qualifiers WHERE QualifierID = " + QualifierID.ToString)
        Return QualifierCode

    End Function

End Class
