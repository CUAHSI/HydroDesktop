'If _QualityControlLevelID = Nothing, it means the user selected create new in fDeriveNewDataSeries

Imports HydroDesktop.Database
Imports System.Globalization
Imports System.Threading
Imports System.Text
Imports HydroDesktop.Interfaces


Public Class fQualityControlLevelTableManagement

    Private connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
    Private dbTools As New DbOperations(connString, DatabaseTypes.SQLite)

    Public Sub New()

        InitializeComponent()

    End Sub

    Private Sub CloseMe() Handles Me.Leave, btnCancel.Click
        Me.Close()
        If _fDeriveNewDataSeries.ddlQualityControlLevel.SelectedIndex = _fDeriveNewDataSeries.ddlQualityControlLevel.Items.Count - 1 Then
            _fDeriveNewDataSeries.SetDefaultQualityControlLevel()
        End If
    End Sub

    Public Sub initialize()
        'if there are no values in _QualityControlLevelID, that means the user clicked "New Qulity Control Level..."
        If _QualityControlLevelID = Nothing Then
            txtID.Text = dbTools.GetNextID("QualityControlLevels", "QualityControlLevelID").ToString
            btnSubmit.Text = "Add"
        Else 'else means the user clicked "Edit Quality Control Level" button
            txtID.Text = _QualityControlLevelID.ToString
            txtCode.Text = dbTools.ExecuteSingleOutput("SELECT QualityControlLevelCode FROM QualityControlLevels WHERE QualityControlLevelID = " + _QualityControlLevelID.ToString).ToString
            txtDefinition.Text = dbTools.ExecuteSingleOutput("SELECT Definition FROM QualityControlLevels WHERE QualityControlLevelID = " + _QualityControlLevelID.ToString).ToString
            txtExplanation.Text = dbTools.ExecuteSingleOutput("SELECT Explanation FROM QualityControlLevels WHERE QualityControlLevelID = " + _QualityControlLevelID.ToString).ToString
            btnSubmit.Text = "Edit"
        End If
    End Sub

    Private Sub InsertNewQualityControlLevel()
        Dim SQLstring As StringBuilder = New StringBuilder()

        Try
            If txtCode.Text = Nothing Then
                MsgBox("Please at least enter the Quality Control Level")
            Else
                SQLstring.Append("INSERT INTO QualityControlLevels(QualityControlLevelID, QualityControlLevelCode, Definition, Explanation) VALUES (")
                SQLstring.Append(txtID.Text.ToString + ",'" + txtCode.Text.ToString + "','" + txtDefinition.Text.ToString + "','" + txtExplanation.Text.ToString + "')")
                dbTools.ExecuteNonQuery(SQLstring.ToString)
            End If
        Catch ex As Exception
            Throw New Exception("Error Occured when Inserting new quality control level" & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub UpdateQualityControlLevel()
        Dim SQLstring As StringBuilder = New StringBuilder()
        Try
            If txtCode.Text = Nothing Then
                MsgBox("Please at least enter the Quality Control Level")
            Else
                SQLstring.Append("UPDATE QualityControlLevels SET QualityControlLevelCode='" + txtCode.Text.ToString + "',")
                SQLstring.Append("Definition='" + txtDefinition.Text.ToString + "',Explanation='" + txtExplanation.Text.ToString + "' ")
                SQLstring.Append("WHERE QualityControlLevelID = " + txtID.Text.ToString)
                dbTools.ExecuteNonQuery(SQLstring.ToString)
            End If
        Catch ex As Exception
            Throw New Exception("Error Occured when Updating quality control level" & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub btnSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        Dim count As Integer

        If txtCode.Text = Nothing Then
            Return
        End If
        If txtDefinition.Text = Nothing Then
            txtDefinition.Text = "unknown"
        End If
        If txtExplanation.Text = Nothing Then
            txtExplanation.Text = "unknown"
        End If

        If _QualityControlLevelID = Nothing Then
            InsertNewQualityControlLevel()
        Else
            UpdateQualityControlLevel()
        End If

        _fDeriveNewDataSeries.FillQualityControlLevel()

        While Not (_fDeriveNewDataSeries.ddlQualityControlLevel.SelectedValue = Val(txtID.Text))
            _fDeriveNewDataSeries.ddlQualityControlLevel.SelectedItem = _fDeriveNewDataSeries.ddlQualityControlLevel.Items.Item(count)
            count += 1
        End While

        Me.Close()

    End Sub


End Class