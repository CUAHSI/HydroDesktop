Imports System.Windows.Forms
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces


Public Class fQualifiersTableManagement

    Private connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
    Private dbTools As New DbOperations(connString, DatabaseTypes.SQLite)

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub initialize()
        Dim dt As New DataTable
        dt = dbTools.LoadTable("Qualifiers", "SELECT * FROM Qualifiers")
        dt.Rows.Add()
        dt.Rows(dt.Rows.Count - 1).Item("QualifierID") = dbTools.GetNextID("Qualifiers", "QualifierID").ToString
        dt.Rows(dt.Rows.Count - 1).Item("QualifierCode") = "New Qualifier..."

        ddlQualifiers.DataSource = dt
        ddlQualifiers.DisplayMember = "QualifierCode"
        ddlQualifiers.ValueMember = "QualifierID"

        ddlQualifiers.SelectedItem = ddlQualifiers.Items(ddlQualifiers.Items.Count - 1)

    End Sub

    Private Sub InsertNewQualifier()
        Dim SQLstring As String

        If ddlQualifiers.SelectedValue Is Nothing Then
            SQLstring = "INSERT INTO Qualifiers (QualifierCode, QualifierDescription) VALUES ('"
            SQLstring += txtQualifierCode.Text + "','" + txtDescription.Text + "')"
        Else
            SQLstring = "INSERT INTO Qualifiers (QualifierID, QualifierCode, QualifierDescription) VALUES ("
            SQLstring += ddlQualifiers.SelectedValue.ToString + ",'" + txtQualifierCode.Text + "','" + txtDescription.Text + "')"
        End If

        dbTools.ExecuteNonQuery(SQLstring)
    End Sub

    Private Sub UpdateQualifier()
        Dim SQLstring As String
        SQLstring = "UPDATE Qualifiers SET QualifierCode = '" + txtQualifierCode.Text + "', QualifierDescription = '" + txtDescription.Text + "' WHERE "
        SQLstring += "QualifierID = " + ddlQualifiers.SelectedValue.ToString
        dbTools.ExecuteNonQuery(SQLstring)
    End Sub

    Private Sub btnSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If ddlQualifiers.SelectedIndex = ddlQualifiers.Items.Count - 1 Then
            InsertNewQualifier()
        Else
            UpdateQualifier()
        End If

        For Each row As DataGridViewRow In _cEditView.GetSelectedRows()
            row.Cells("QualifierCode").Value = txtQualifierCode.Text
            If Not row.Cells("Other").Value = -1 And Not row.Cells("Other").Value = 1 Then
                row.Cells("Other").Value = 2
            End If
        Next

        _cEditView.RefreshDataGridView()
        _cEditView.pTimeSeriesPlot.ReplotEditingCurve(_cEditView)

        Me.Close()
    End Sub

    Private Sub ddlQualifiers_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ddlQualifiers.SelectedIndexChanged
        If ddlQualifiers.SelectedIndex = ddlQualifiers.Items.Count - 1 Then
        Else
            txtDescription.Text = dbTools.ExecuteSingleOutput("SELECT QualifierDescription FROM Qualifiers WHERE QualifierID = " + ddlQualifiers.SelectedValue.ToString)
            txtQualifierCode.Text = dbTools.ExecuteSingleOutput("SELECT QualifierCode FROM Qualifiers WHERE QualifierID = " + ddlQualifiers.SelectedValue.ToString)
        End If
    End Sub

    'Private Sub Leaving() Handles Me.Deactivate
    '    Me.Close()
    'End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub


End Class