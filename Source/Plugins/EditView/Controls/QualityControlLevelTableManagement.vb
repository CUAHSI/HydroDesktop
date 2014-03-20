Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports HydroDesktop.Interfaces.ObjectModel

Namespace Controls


    Public Class QualityControlLevelTableManagement

        Private ReadOnly _repo As IQualityControlLevelsRepository = RepositoryFactory.Instance.Get(Of IQualityControlLevelsRepository)()
        Private _QualityControlLevelID As Int64?

        Public Sub New()

            InitializeComponent()

        End Sub

        Private Sub CloseMe() Handles Me.Leave, btnCancel.Click
            Close()
        End Sub

        Public ReadOnly Property QualityControlLevelID() As Int64?
            Get
                Return _QualityControlLevelID
            End Get
        End Property

        Public Sub initialize(ByVal qId As Int64?)
            _QualityControlLevelID = qId

            If Not _QualityControlLevelID.HasValue Then
                btnSubmit.Text = "Add"
            Else 'else means the user clicked "Edit Quality Control Level" button
                Dim entity = _repo.GetByKey(_QualityControlLevelID.Value)

                txtCode.Text = entity.Code
                txtDefinition.Text = entity.Definition
                txtExplanation.Text = entity.Explanation

                btnSubmit.Text = "Edit"
            End If
        End Sub

        Private Sub btnSubmit_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSubmit.Click
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
            entity.Code = txtCode.Text
            entity.Explanation = txtExplanation.Text
            entity.Definition = txtDefinition.Text

            If Not _QualityControlLevelID.HasValue Then
                _repo.AddNew(entity)
                _QualityControlLevelID = entity.Id
            Else
                entity.Id = _QualityControlLevelID.Value
                _repo.Update(entity)
            End If

            Close()
        End Sub


    End Class
End Namespace