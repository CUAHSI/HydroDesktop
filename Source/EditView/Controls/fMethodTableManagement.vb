'If _MethodID = Nothing, it means the user selected create new in fDeriveNewDataSeries

Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces


Public Class fMethodTableManagement

    ReadOnly repo As IMethodsRepository = RepositoryFactory.Instance.Get(Of IMethodsRepository)()

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub CloseMe() Handles Me.Leave, btnCancel.Click
        Close()
        If _fDeriveNewDataSeries.ddlMethods.SelectedIndex = _fDeriveNewDataSeries.ddlMethods.Items.Count - 1 Then
            _fDeriveNewDataSeries.SetDefaultMethods()
        End If
    End Sub

    Public Sub initialize()

        If _MethodID = Nothing Then
            btnSubmit.Text = "Add"
        Else
            Dim method = repo.GetByKey(_MethodID)
            txtDescription.Text = method.Description
            txtLink.Text = method.Link
            btnSubmit.Text = "Edit"
        End If

    End Sub

    Private Sub InsertNewMethod()
        _MethodID = repo.InsertMethod(txtDescription.Text.ToString, txtLink.Text.ToString)
    End Sub

    Private Sub UpdateMethod()
        repo.UpdateMethod(_MethodID, txtDescription.Text.ToString, txtLink.Text.ToString)
    End Sub

    Private Sub btnSubmit_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSubmit.Click
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

        While Not (_fDeriveNewDataSeries.ddlMethods.SelectedValue = _MethodID)
            _fDeriveNewDataSeries.ddlMethods.SelectedItem = _fDeriveNewDataSeries.ddlMethods.Items.Item(count)
            count += 1
        End While

        Close()

    End Sub

End Class