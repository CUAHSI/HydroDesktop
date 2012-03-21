'If _QualityControlLevelID = Nothing, it means the user selected create new in fDeriveNewDataSeries

Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports HydroDesktop.Interfaces.ObjectModel


Public Class fQualityControlLevelTableManagement

    Private ReadOnly _repo As IQualityControlLevelsRepository = RepositoryFactory.Instance.Get(Of IQualityControlLevelsRepository)()

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
            btnSubmit.Text = "Add"
        Else 'else means the user clicked "Edit Quality Control Level" button
            Dim entity = _repo.GetByKey(_QualityControlLevelID)

            txtCode.Text = entity.Code
            txtDefinition.Text = entity.Definition
            txtExplanation.Text = entity.Explanation

            btnSubmit.Text = "Edit"
        End If
    End Sub

    Private Sub btnSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        Dim count As Integer

        If txtCode.Text = Nothing Then
            MsgBox("Please at least enter the Quality Control Level")
            Return
        End If
        If txtDefinition.Text = Nothing Then
            txtDefinition.Text = "unknown"
        End If
        If txtExplanation.Text = Nothing Then
            txtExplanation.Text = "unknown"
        End If

        Dim entity = New QualityControlLevel()
        entity.Id = _QualityControlLevelID
        entity.Code = txtCode.Text
        entity.Explanation = txtExplanation.Text
        entity.Definition = txtDefinition.Text

        If _QualityControlLevelID = Nothing Then
            _repo.AddNew(entity)
            _QualityControlLevelID = entity.Id
        Else
            _repo.Update(entity)
        End If

        _fDeriveNewDataSeries.FillQualityControlLevel()

        While Not (_fDeriveNewDataSeries.ddlQualityControlLevel.SelectedValue = Val(_QualityControlLevelID))
            _fDeriveNewDataSeries.ddlQualityControlLevel.SelectedItem = _fDeriveNewDataSeries.ddlQualityControlLevel.Items.Item(count)
            count += 1
        End While

        Me.Close()

    End Sub


End Class