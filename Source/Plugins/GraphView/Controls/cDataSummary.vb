Public Class cDataSummary

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub

    Public Sub CreateStatTable(ByVal site As String, ByVal seriesId As Integer, ByVal data As DataTable, ByVal options As PlotOptions)

        If options.UseCensoredData = True Then
            dgvStatSummary.Rows.Add(site, "ID " + seriesId.ToString)
            dgvStatSummary.Rows.Add("# Of Observations", Statistics.Count(data))
            dgvStatSummary.Rows.Add("# Of Censored Obs.", Statistics.CountCensored(data))
            dgvStatSummary.Rows.Add("Arithmetic Mean", Statistics.ArithmeticMean(data))
            dgvStatSummary.Rows.Add("Geometric Mean", Statistics.GeometricMean(data))
            dgvStatSummary.Rows.Add("Maximum", Statistics.Maximum(data))
            dgvStatSummary.Rows.Add("Minimum", Statistics.Minimum(data))
            dgvStatSummary.Rows.Add("Standard Deviation", Statistics.StandardDeviation(data))
            dgvStatSummary.Rows.Add("Coefficiant of Variation", Statistics.CoefficientOfVariation(data))
            dgvStatSummary.Rows.Add("Percentiles 10%", Statistics.Percentile(data, 10))
            dgvStatSummary.Rows.Add("Percentiles 25%", Statistics.Percentile(data, 25))
            dgvStatSummary.Rows.Add("Percentiles 50%(median)", Statistics.Percentile(data, 50))
            dgvStatSummary.Rows.Add("Percentiles 75%", Statistics.Percentile(data, 75))
            dgvStatSummary.Rows.Add("Percentiles 90%", Statistics.Percentile(data, 90))
            dgvStatSummary.Rows.Add()
            dgvStatSummary.Columns(0).Width = site.Length * 7
            dgvStatSummary.AutoResizeColumns()
        Else
            Dim temp As DataTable = data.Copy
            If (Not options.UseCensoredData) Then
                temp.CaseSensitive = False
                Dim censoredRows() As DataRow = temp.Select("(CensorCode <> '" & Statistics.NotCensored & "') AND (CensorCode <> '" & Statistics.Unknown & "')")

                For Each censoredRow As DataRow In censoredRows
                    temp.Rows.Remove(censoredRow)
                Next censoredRow
            End If
            If temp.Rows.Count > 0 Then
                dgvStatSummary.Rows.Add(site, "ID " + seriesId.ToString)
                dgvStatSummary.Rows.Add("# Of Observations", Statistics.Count(temp))
                dgvStatSummary.Rows.Add("# Of Censored Obs.", Statistics.CountCensored(temp))
                dgvStatSummary.Rows.Add("Arithmetic Mean", Statistics.ArithmeticMean(temp))
                dgvStatSummary.Rows.Add("Geometric Mean", Statistics.GeometricMean(temp))
                dgvStatSummary.Rows.Add("Maximum", Statistics.Maximum(temp))
                dgvStatSummary.Rows.Add("Minimum", Statistics.Minimum(temp))
                dgvStatSummary.Rows.Add("Standard Deviation", Statistics.StandardDeviation(temp))
                dgvStatSummary.Rows.Add("Coefficiant of Variation", Statistics.CoefficientOfVariation(temp))
                dgvStatSummary.Rows.Add("Percentiles 10%", Statistics.Percentile(temp, 10))
                dgvStatSummary.Rows.Add("Percentiles 25%", Statistics.Percentile(temp, 25))
                dgvStatSummary.Rows.Add("Percentiles 50%(median)", Statistics.Percentile(temp, 50))
                dgvStatSummary.Rows.Add("Percentiles 75%", Statistics.Percentile(temp, 75))
                dgvStatSummary.Rows.Add("Percentiles 90%", Statistics.Percentile(temp, 90))
                dgvStatSummary.Rows.Add()
                dgvStatSummary.Columns(0).Width = site.Length * 7
                dgvStatSummary.AutoResizeColumns()
            End If
        End If
    End Sub

    Public Sub ClearStatTables()
        dgvStatSummary.Rows.Clear()

    End Sub

    Public Sub StatTableStyling()
        Dim count As Integer = 0
        Dim sizecount As Integer = 0
        For Each i In dgvStatSummary.Rows
            sizecount += 1
        Next
        For Each i In dgvStatSummary.Rows
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

    Public Sub RemoveStatTable(ByVal SeriesID As Integer)
        Dim row As Integer = 0

        Do Until (row > dgvStatSummary.RowCount - 1)
            If dgvStatSummary.Rows(row).Cells(1).Value = "ID " + SeriesID.ToString Then
                For i = 0 To 14
                    dgvStatSummary.Rows.Remove(dgvStatSummary.Rows(row))
                Next
            Else
                row += 15
            End If

        Loop
    End Sub

End Class
