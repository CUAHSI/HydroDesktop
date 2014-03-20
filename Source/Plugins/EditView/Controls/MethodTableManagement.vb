Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces

Namespace Controls

    Public Class MethodTableManagement

        ReadOnly repo As IMethodsRepository = RepositoryFactory.Instance.Get(Of IMethodsRepository)()
        Private _MethodID As Long?

        Public Sub New()
            InitializeComponent()
        End Sub

        Private Sub CloseMe() Handles Me.Leave, btnCancel.Click
            Close()
        End Sub

        Public Sub initialize(ByVal mId As Int64?)
            _MethodID = mId
            If Not _MethodID.HasValue Then
                btnSubmit.Text = "Add"
            Else
                Dim method = repo.GetByKey(_MethodID.Value)
                txtDescription.Text = method.Description
                txtLink.Text = method.Link
                btnSubmit.Text = "Edit"
            End If
        End Sub

        Public ReadOnly Property MethodID() As Long?
            Get
                Return _MethodID
            End Get
        End Property

        Private Sub InsertNewMethod()
            _MethodID = repo.InsertMethod(txtDescription.Text.ToString, txtLink.Text.ToString)
        End Sub

        Private Sub UpdateMethod()
            repo.UpdateMethod(_MethodID, txtDescription.Text.ToString, txtLink.Text.ToString)
        End Sub

        Private Sub btnSubmit_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSubmit.Click
            'If the user did not enter things in text boxes, some action will be acted before making any changes in the database
            If txtDescription.Text = Nothing Then
                txtDescription.Text = "unknown"
            End If
            If txtLink.Text = Nothing Then
                txtLink.Text = "unknown"
            End If

            If Not _MethodID.HasValue Then
                InsertNewMethod()
            Else
                UpdateMethod()
            End If

            Close()
        End Sub
    End Class
End Namespace