'If _MethodID = Nothing, it means the user selected create new in fDeriveNewDataSeries

Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces


Public Class fMethodTableManagement

    Private connString = HydroDesktop.Configuration.Settings.Instance.DataRepositoryConnectionString
    Private dbTools As New DbOperations(connString, DatabaseTypes.SQLite)

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub CloseMe() Handles Me.Leave, btnCancel.Click
        Me.Close()
        If _fDeriveNewDataSeries.ddlMethods.SelectedIndex = _fDeriveNewDataSeries.ddlMethods.Items.Count - 1 Then
            _fDeriveNewDataSeries.SetDefaultMethods()
        End If
    End Sub

    Public Sub initialize()

        If _MethodID = Nothing Then
            txtID.Text = dbTools.GetNextID("Methods", "MethodID").ToString
            btnSubmit.Text = "Add"
        Else
            txtID.Text = _MethodID.ToString
            Dim repo = RepositoryFactory.Instance.Get(Of IMethodsRepository)(dbTools)
            Dim method = repo.GetMethod(_MethodID)
            txtDescription.Text = method.Description
            txtLink.Text = method.Link
            btnSubmit.Text = "Edit"
        End If

    End Sub

    Private Sub InsertNewMethod()
        Try
            Dim repo = RepositoryFactory.Instance.Get(Of IMethodsRepository)(dbTools)
            repo.InsertMethod(txtDescription.Text.ToString, txtLink.Text.ToString, Int32.Parse(txtID.Text))
        Catch ex As Exception
            Throw New Exception("Error Occurred when Inserting new methods." & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub UpdateMethod()
        Try
            Dim repo = RepositoryFactory.Instance.Get(Of IMethodsRepository)(dbTools)
            repo.UpdateMethod(Int32.Parse(txtID.Text), txtDescription.Text.ToString, txtLink.Text.ToString)
        Catch ex As Exception
            Throw New Exception("Error Occured when Updating methods." & vbCrLf & ex.Message)
        End Try
    End Sub

    Private Sub btnSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        Dim count As Integer

        'If the user did not enter things in text boxes, some action will be acted before making any changes in the database
        If txtDescription.Text = Nothing Then
            txtDescription.Text = "unknown"
        End If
        If txtLink.Text = Nothing Then
            txtLink.Text = "unknown"
        End If

        If _MethodID = Nothing Then
            InsertNewMethod()
        Else
            UpdateMethod()
        End If

        _fDeriveNewDataSeries.FillMethods()

        While Not (_fDeriveNewDataSeries.ddlMethods.SelectedValue = Val(txtID.Text))
            _fDeriveNewDataSeries.ddlMethods.SelectedItem = _fDeriveNewDataSeries.ddlMethods.Items.Item(count)
            count += 1
        End While

        Me.Close()

    End Sub

End Class