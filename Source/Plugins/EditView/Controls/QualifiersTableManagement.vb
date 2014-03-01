Imports System.Windows.Forms
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports HydroDesktop.Interfaces.ObjectModel

Namespace Controls


    Public Class QualifiersTableManagement
        Private ReadOnly _qualifierRepo As IQualifiersRepository = RepositoryFactory.Instance.Get(Of IQualifiersRepository)()

        Public Sub New()
            InitializeComponent()
            initialize()
        End Sub

        Private Sub initialize()
            Dim dt = _qualifierRepo.AsDataTable()
            dt.Rows.Add()
            dt.Rows(dt.Rows.Count - 1).Item("QualifierID") = 0
            dt.Rows(dt.Rows.Count - 1).Item("QualifierCode") = "New Qualifier..."

            ddlQualifiers.DataSource = dt
            ddlQualifiers.DisplayMember = "QualifierCode"
            ddlQualifiers.ValueMember = "QualifierID"

            ddlQualifiers.SelectedItem = ddlQualifiers.Items(ddlQualifiers.Items.Count - 1)

        End Sub

        Private Sub btnSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            Dim entity = New Qualifier()
            entity.Id = ddlQualifiers.SelectedValue
            entity.Code = txtQualifierCode.Text
            entity.Description = txtDescription.Text

            If ddlQualifiers.SelectedIndex = ddlQualifiers.Items.Count - 1 Then
                _qualifierRepo.AddQualifier(entity)
            Else
                _qualifierRepo.Update(entity)
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
                txtDescription.Text = DirectCast(ddlQualifiers.DataSource, DataTable).Rows(ddlQualifiers.SelectedIndex)("QualifierDescription")
                txtQualifierCode.Text = DirectCast(ddlQualifiers.DataSource, DataTable).Rows(ddlQualifiers.SelectedIndex)("QualifierCode")
            End If
        End Sub

        Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
            Me.Close()
        End Sub


    End Class
End Namespace