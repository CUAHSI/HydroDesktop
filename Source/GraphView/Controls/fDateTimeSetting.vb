Public Class fDateTimeSetting

    Public Sub initialize()

        dtpStartDate.MinDate = _CTSA.StartDateLimit
        dtpEndDate.MaxDate = _CTSA.EndDateLimit

        dtpStartDate.SelectionStart = _CTSA.StartDateTime
        dtpEndDate.SelectionStart = _CTSA.EndDateTime

        dtpEndDate.MinDate = dtpStartDate.SelectionStart
        dtpStartDate.MaxDate = dtpEndDate.SelectionStart

    End Sub


    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub dtpStartDate_DateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DateRangeEventArgs) Handles dtpStartDate.DateChanged
        dtpEndDate.MinDate = dtpStartDate.SelectionStart
    End Sub

    Private Sub dtpEndDate_DateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DateRangeEventArgs) Handles dtpEndDate.DateChanged
        dtpStartDate.MaxDate = dtpEndDate.SelectionStart
    End Sub

    Private Sub btnApply_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApply.Click
        _CTSA.StartDateTime = dtpStartDate.SelectionStart
        _CTSA.EndDateTime = dtpEndDate.SelectionStart
        _CTSA.ApplyOptions()
        Me.Close()
    End Sub
End Class