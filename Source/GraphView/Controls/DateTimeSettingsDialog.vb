Namespace Controls

    Public Class DateTimeSettingsDialog

        Private ReadOnly _plotOptions As PlotOptions

        Public Event DatesApplied As EventHandler

        Public Sub New(plotOptions As PlotOptions)

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            _plotOptions = plotOptions

            dtpStartDate.MinDate = _plotOptions.StartDateLimit
            dtpStartDate.MaxDate = _plotOptions.EndDateLimit
            dtpEndDate.MinDate = _plotOptions.StartDateLimit
            dtpEndDate.MaxDate = _plotOptions.EndDateLimit

            dtpStartDate.Value = _plotOptions.StartDateTime
            dtpEndDate.Value = _plotOptions.EndDateTime

            AddHandler dtpStartDate.ValueChanged, AddressOf dtpDate_ValueChanged
            AddHandler dtpEndDate.ValueChanged, AddressOf dtpDate_ValueChanged

            btnApply.Enabled = False
        End Sub

        Private Sub dtpDate_ValueChanged(ByVal sender As Object, ByVal e As EventArgs)
            btnApply.Enabled = True
        End Sub

        Private Sub btnApply_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnApply.Click
            ApplyDates()
        End Sub

        Private Sub btnOK_Click(sender As System.Object, e As EventArgs) Handles btnOK.Click
            ApplyDates()
            Close()
        End Sub

        Private Sub ApplyDates()
            If Not btnApply.Enabled Then Return

            _plotOptions.StartDateTime = dtpStartDate.Value
            _plotOptions.EndDateTime = dtpEndDate.Value

            btnApply.Enabled = False

            RaiseEvent DatesApplied(Me, EventArgs.Empty)
        End Sub

    End Class
End Namespace