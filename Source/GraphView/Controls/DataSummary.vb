Option Strict On

Namespace Controls

    Public Class DataSummary

        Private _seriesPlotInfo As SeriesPlotInfo

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            AddHandler VisibleChanged, AddressOf OnDataSummaryVisibleChanged
        End Sub

        Public Sub Plot(ByVal seriesPlotInfo As SeriesPlotInfo)

            _seriesPlotInfo = Nothing
            If Not Visible Then
                _seriesPlotInfo = seriesPlotInfo
                Return
            End If

            ClearStatTables()
            For Each seriesInfo In seriesPlotInfo.GetSeriesInfo()
                Plot(seriesInfo)
            Next
            StatTableStyling()
        End Sub

#Region "Private methods"

        Private Sub OnDataSummaryVisibleChanged(ByVal sender As Object, ByVal e As EventArgs)
            If Not Visible Then Return
            If _seriesPlotInfo Is Nothing Then Return
            Plot(_seriesPlotInfo)
        End Sub

        Private Sub Plot(ByRef options As OneSeriesPlotInfo)
            Dim siteName = options.SiteName
            Dim variableName = options.VariableName
            Dim siteAndVariable = siteName + ", " + variableName
            Dim statistics = options.Statistics

            dgvStatSummary.Rows.Add(siteAndVariable, "ID " + options.SeriesID.ToString())
            dgvStatSummary.Rows.Add("# Of Observations", statistics.NumberOfObservations)
            dgvStatSummary.Rows.Add("# Of Censored Obs.", statistics.NumberOfCensoredObservations)
            dgvStatSummary.Rows.Add("Arithmetic Mean", statistics.ArithmeticMean)
            dgvStatSummary.Rows.Add("Geometric Mean", statistics.GeometricMean)
            dgvStatSummary.Rows.Add("Maximum", statistics.Maximum)
            dgvStatSummary.Rows.Add("Minimum", statistics.Minimum)
            dgvStatSummary.Rows.Add("Standard Deviation", statistics.StandardDeviation)
            dgvStatSummary.Rows.Add("Coefficient of Variation", statistics.CoefficientOfVariation)
            dgvStatSummary.Rows.Add("Percentiles 10%", statistics.Percentile10)
            dgvStatSummary.Rows.Add("Percentiles 25%", statistics.Percentile25)
            dgvStatSummary.Rows.Add("Percentiles 50%(median)", statistics.Percentile50)
            dgvStatSummary.Rows.Add("Percentiles 75%", statistics.Percentile75)
            dgvStatSummary.Rows.Add("Percentiles 90%", statistics.Percentile90)
            dgvStatSummary.Rows.Add()
            dgvStatSummary.Columns(0).Width = siteAndVariable.Length * 7
            dgvStatSummary.AutoResizeColumns()
        End Sub

        Private Sub ClearStatTables()
            dgvStatSummary.Rows.Clear()
        End Sub

        Private Sub StatTableStyling()
            Dim count As Integer = 0
            Dim sizecount As Integer = dgvStatSummary.Rows.Count
            For Each i In DirectCast(dgvStatSummary.Rows, IEnumerable)
                dgvStatSummary.Rows(count).Cells(0).Style.BackColor = Drawing.Color.Yellow
                If (count Mod 15 = 0) And Not (count = sizecount) Then
                    dgvStatSummary.Rows(count).Cells(0).Style.BackColor = Drawing.Color.Aqua
                    dgvStatSummary.Rows(count).Cells(1).Style.BackColor = Drawing.Color.Aqua
                End If
                If count Mod 15 = 14 Then
                    dgvStatSummary.Rows(count).Cells(0).Style.BackColor = Drawing.Color.White
                End If
                count += 1
            Next

        End Sub

#End Region

    End Class
End Namespace