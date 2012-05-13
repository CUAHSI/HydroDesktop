
Namespace Controls
    Public Class PlotOptionsControl
        'Inherits Windows.Forms.UserControl
        Public tsType As PlotOptions.TimeSeriesType = PlotOptions.TimeSeriesType.Both
        Public bwType As PlotOptions.BoxWhiskerType = PlotOptions.BoxWhiskerType.Monthly
        Public hpType As PlotOptions.HistogramType = PlotOptions.HistogramType.Count
        Public hpAlgo As PlotOptions.HistorgramAlgorithms = PlotOptions.HistorgramAlgorithms.Sturges
        Public IsShowLegend As Boolean = True

        Public Property Options() As PlotOptions
            Get
                Return New PlotOptions(tsType, hpType, hpAlgo, 0, 0, 0, 0, 0, 0, 0, bwType, Drawing.Color.Black, btnSetPointColor.BackColor, IsShowLegend, Now, Now, True, False)
            End Get
            Set(ByVal value As PlotOptions)
                Select Case value.TimeSeriesMethod
                    Case PlotOptions.TimeSeriesType.Line

                    Case PlotOptions.TimeSeriesType.Point

                    Case PlotOptions.TimeSeriesType.Both

                End Select

                Select Case value.BoxWhiskerMethod
                    Case PlotOptions.BoxWhiskerType.Monthly

                    Case PlotOptions.BoxWhiskerType.Seasonal

                    Case PlotOptions.BoxWhiskerType.Yearly

                    Case PlotOptions.BoxWhiskerType.Overall

                End Select

                Select Case value.HistTypeMethod
                    Case PlotOptions.HistogramType.Count

                    Case PlotOptions.HistogramType.Probability

                    Case PlotOptions.HistogramType.Relative

                End Select

                Select Case value.HistAlgorothmsMethod
                    Case PlotOptions.HistorgramAlgorithms.Scott

                    Case PlotOptions.HistorgramAlgorithms.Sturges

                    Case PlotOptions.HistorgramAlgorithms.Freedman

                End Select

                'btnSetLineColor.BackColor = value.GetLineColor
                'btnSetPointColor.BackColor = value.GetPointColor
                'dtpStartDatePicker.Value = value.StartDate
                'dtpEndDatePicker.Value = value.EndDate
                'ckbDateRangeChange.Checked = value.ChangeDateRange
                'ckbShowLegend.Checked = value.ShowLegend
                'txtLineccNumber.Text = value.LineColorList
                'txtPointccNumber.Text = value.PointColorList
            End Set
        End Property

        Private Sub btnSetLineColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetLineColor.Click
            Dim newColor As System.Drawing.Color = PromptForColor(btnSetLineColor.BackColor)

            If Not IsDBNull(newColor) Then
                btnSetLineColor.BackColor = newColor
            End If
        End Sub

        Private Sub btnSetPointColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetPointColor.Click
            Dim newColor As System.Drawing.Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                btnSetPointColor.BackColor = newColor
            End If
        End Sub

        Private Function PromptForColor(ByVal defaultColor As System.Drawing.Color) As System.Drawing.Color
            Dim dlgColor As System.Windows.Forms.ColorDialog = New System.Windows.Forms.ColorDialog()

            If Not IsDBNull(defaultColor) Then
                dlgColor.Color = defaultColor
            End If

            If (dlgColor.ShowDialog() = Windows.Forms.DialogResult.OK) Then
                Return dlgColor.Color
            Else
                Return Nothing
            End If
        End Function


        Private Sub OptionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnTSLine.Click, rbtnTSPoint.Click, rbtnTSBoth.Click, rbtnBPMonthly.Click, rbtnBPOverall.Click, rbtnBPSeasonal.Click, rbtnBPYearly.Click, rbtnHPCount.Click, rbtnHPFreedman.Click, rbtnHPProbability.Click, rbtnHPRelative.Click, rbtnHPScotts.Click, rbtnHPSturges.Click, ckbShowLegend.CheckedChanged, btnplot.Click, txtLineccNumber.Leave, txtPointccNumber.Leave

        End Sub

        Private Sub btnColorConnections_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnColorConnections.Click

        End Sub

        Public Sub ColorCollectionChosen(ByVal _lineColorList As Integer, ByVal _pointColorList As Integer)
            If Not _lineColorList = Nothing Then
                txtLineccNumber.Text = _lineColorList
            End If
            If Not _pointColorList = Nothing Then
                txtPointccNumber.Text = _pointColorList
            End If
            btnSetLineColor.BackColor = Drawing.Color.Black
            btnSetPointColor.BackColor = Drawing.Color.Black
        End Sub

        Private Sub ckbDateRangeChange_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ckbDateRangeChange.CheckedChanged
            If ckbDateRangeChange.Checked Then
                dtpStartDatePicker.Enabled = False
                dtpEndDatePicker.Enabled = False
            Else
                dtpStartDatePicker.Enabled = True
                dtpEndDatePicker.Enabled = True
            End If
        End Sub
    End Class
End Namespace