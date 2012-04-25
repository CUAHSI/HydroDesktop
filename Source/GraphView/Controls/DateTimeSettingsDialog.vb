
Namespace Controls

    Public Class DateTimeSettingsDialog

        Private ReadOnly _ctsa As MainControl

        Public Sub New(ctsa As MainControl)

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            _ctsa = ctsa

            dtpStartDate.MinDate = _ctsa.StartDateLimit
            dtpStartDate.MaxDate = _ctsa.EndDateLimit
            dtpEndDate.MinDate = _ctsa.StartDateLimit
            dtpEndDate.MaxDate = _ctsa.EndDateLimit

            dtpStartDate.Value = _ctsa.StartDateTime
            dtpEndDate.Value = _ctsa.EndDateTime

            AddHandler dtpStartDate.ValueChanged, AddressOf dtpDate_ValueChanged
            AddHandler dtpEndDate.ValueChanged, AddressOf dtpDate_ValueChanged

            btnApply.Enabled = False
        End Sub

        Private Sub dtpDate_ValueChanged(ByVal sender As Object, ByVal e As EventArgs)
            btnApply.Enabled = True
        End Sub

        Private Sub btnApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApply.Click
            ApplyDates()
        End Sub

        Private Sub btnOK_Click(sender As System.Object, e As System.EventArgs) Handles btnOK.Click
            ApplyDates()
            Close()
        End Sub

        Private Sub ApplyDates()
            If Not btnApply.Enabled Then Return

            _ctsa.StartDateTime = dtpStartDate.Value
            _ctsa.EndDateTime = dtpEndDate.Value
            _ctsa.ApplyOptions()

            btnApply.Enabled = False
        End Sub

    End Class
End Namespace