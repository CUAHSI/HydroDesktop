Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class MainControl
        Inherits System.Windows.Forms.UserControl

        Public frmCC As ColorSettingsDialog

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.summaryStatistics = New SummaryStatistics()
            Me.tpDataSummary = New System.Windows.Forms.TabPage()
            Me.dataSummary = New DataSummary()
            Me.tpBoxWhisker = New System.Windows.Forms.TabPage()
            Me.boxWhisker = New BoxWhiskerPlot()
            Me.tpHistogram = New System.Windows.Forms.TabPage()
            Me.histogramPlot = New HistogramPlot()
            Me.tpProbability = New System.Windows.Forms.TabPage()
            Me.probabilityPlot = New ProbabilityPlot()
            Me.tpTimeSeries = New System.Windows.Forms.TabPage()
            Me.ProgressBar = New System.Windows.Forms.ProgressBar()
            Me.timeSeriesPlot = New TimeSeriesPlot()
            Me.tcPlots = New System.Windows.Forms.TabControl()
            Me.tpDataSummary.SuspendLayout()
            Me.tpBoxWhisker.SuspendLayout()
            Me.tpHistogram.SuspendLayout()
            Me.tpProbability.SuspendLayout()
            Me.tpTimeSeries.SuspendLayout()
            Me.tcPlots.SuspendLayout()
            Me.SuspendLayout()
            '
            'CSummaryStatistics1
            '
            Me.summaryStatistics.AutoScroll = True
            Me.summaryStatistics.Dock = System.Windows.Forms.DockStyle.Fill
            Me.summaryStatistics.Location = New System.Drawing.Point(0, 0)
            Me.summaryStatistics.Name = "CSummaryStatistics1"
            Me.summaryStatistics.Padding = New System.Windows.Forms.Padding(3)
            Me.summaryStatistics.Size = New System.Drawing.Size(230, 375)
            Me.summaryStatistics.TabIndex = 0
            '
            'tpDataSummary
            '
            Me.tpDataSummary.Controls.Add(Me.dataSummary)
            Me.tpDataSummary.Location = New System.Drawing.Point(4, 5)
            Me.tpDataSummary.Name = "tpDataSummary"
            Me.tpDataSummary.Padding = New System.Windows.Forms.Padding(3)
            Me.tpDataSummary.Size = New System.Drawing.Size(987, 491)
            Me.tpDataSummary.TabIndex = 5
            Me.tpDataSummary.Text = "Summary Statistics"
            Me.tpDataSummary.UseVisualStyleBackColor = True
            '
            'pDataSummary
            '
            Me.dataSummary.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dataSummary.Location = New System.Drawing.Point(3, 3)
            Me.dataSummary.Name = "pDataSummary"
            Me.dataSummary.Size = New System.Drawing.Size(981, 485)
            Me.dataSummary.TabIndex = 0
            '
            'tpBoxWhisker
            '
            Me.tpBoxWhisker.Controls.Add(Me.boxWhisker)
            Me.tpBoxWhisker.Location = New System.Drawing.Point(4, 5)
            Me.tpBoxWhisker.Name = "tpBoxWhisker"
            Me.tpBoxWhisker.Size = New System.Drawing.Size(987, 491)
            Me.tpBoxWhisker.TabIndex = 4
            Me.tpBoxWhisker.Text = "Box/Whisker"
            Me.tpBoxWhisker.UseVisualStyleBackColor = True
            '
            'pBoxWhisker
            '
            Me.boxWhisker.Dock = System.Windows.Forms.DockStyle.Fill
            Me.boxWhisker.Location = New System.Drawing.Point(0, 0)
            Me.boxWhisker.Name = "pBoxWhisker"
            Me.boxWhisker.ShowPointValues = False
            Me.boxWhisker.Size = New System.Drawing.Size(987, 491)
            Me.boxWhisker.TabIndex = 0
            '
            'tpHistogram
            '
            Me.tpHistogram.Controls.Add(Me.histogramPlot)
            Me.tpHistogram.Location = New System.Drawing.Point(4, 5)
            Me.tpHistogram.Name = "tpHistogram"
            Me.tpHistogram.Size = New System.Drawing.Size(987, 491)
            Me.tpHistogram.TabIndex = 3
            Me.tpHistogram.Text = "Histogram"
            Me.tpHistogram.UseVisualStyleBackColor = True
            '
            'pHistogram
            '
            Me.histogramPlot.Dock = System.Windows.Forms.DockStyle.Fill
            Me.histogramPlot.Location = New System.Drawing.Point(0, 0)
            Me.histogramPlot.Name = "pHistogram"
            Me.histogramPlot.ShowPointValues = False
            Me.histogramPlot.Size = New System.Drawing.Size(987, 491)
            Me.histogramPlot.TabIndex = 0
            '
            'tpProbability
            '
            Me.tpProbability.Controls.Add(Me.probabilityPlot)
            Me.tpProbability.Location = New System.Drawing.Point(4, 5)
            Me.tpProbability.Name = "tpProbability"
            Me.tpProbability.Size = New System.Drawing.Size(987, 491)
            Me.tpProbability.TabIndex = 2
            Me.tpProbability.Text = "Probability"
            Me.tpProbability.UseVisualStyleBackColor = True
            '
            'pProbability
            '
            Me.probabilityPlot.Dock = System.Windows.Forms.DockStyle.Fill
            Me.probabilityPlot.Location = New System.Drawing.Point(0, 0)
            Me.probabilityPlot.Name = "pProbability"
            Me.probabilityPlot.SeriesSelector = Nothing
            Me.probabilityPlot.ShowPointValues = False
            Me.probabilityPlot.Size = New System.Drawing.Size(987, 491)
            Me.probabilityPlot.TabIndex = 0
            '
            'tpTimeSeries
            '
            Me.tpTimeSeries.Controls.Add(Me.ProgressBar)
            Me.tpTimeSeries.Controls.Add(Me.timeSeriesPlot)
            Me.tpTimeSeries.Location = New System.Drawing.Point(4, 5)
            Me.tpTimeSeries.Name = "tpTimeSeries"
            Me.tpTimeSeries.Size = New System.Drawing.Size(987, 491)
            Me.tpTimeSeries.TabIndex = 1
            Me.tpTimeSeries.Text = "Time Series"
            Me.tpTimeSeries.UseVisualStyleBackColor = True
            '
            'ProgressBar
            '
            Me.ProgressBar.Dock = System.Windows.Forms.DockStyle.Bottom
            Me.ProgressBar.Location = New System.Drawing.Point(0, 468)
            Me.ProgressBar.Name = "ProgressBar"
            Me.ProgressBar.Size = New System.Drawing.Size(987, 23)
            Me.ProgressBar.TabIndex = 2
            '
            'pTimeSeries
            '
            Me.timeSeriesPlot.Dock = System.Windows.Forms.DockStyle.Fill
            Me.timeSeriesPlot.Location = New System.Drawing.Point(0, 0)
            Me.timeSeriesPlot.Name = "pTimeSeries"
            Me.timeSeriesPlot.SeriesSelector = Nothing
            Me.timeSeriesPlot.ShowPointValues = False
            Me.timeSeriesPlot.Size = New System.Drawing.Size(987, 491)
            Me.timeSeriesPlot.TabIndex = 0
            '
            'tcPlots
            '
            Me.tcPlots.Appearance = System.Windows.Forms.TabAppearance.Buttons
            Me.tcPlots.Controls.Add(Me.tpTimeSeries)
            Me.tcPlots.Controls.Add(Me.tpProbability)
            Me.tcPlots.Controls.Add(Me.tpHistogram)
            Me.tcPlots.Controls.Add(Me.tpBoxWhisker)
            Me.tcPlots.Controls.Add(Me.tpDataSummary)
            Me.tcPlots.Dock = System.Windows.Forms.DockStyle.Fill
            Me.tcPlots.ItemSize = New System.Drawing.Size(0, 1)
            Me.tcPlots.Location = New System.Drawing.Point(0, 0)
            Me.tcPlots.Name = "tcPlots"
            Me.tcPlots.SelectedIndex = 0
            Me.tcPlots.Size = New System.Drawing.Size(995, 500)
            Me.tcPlots.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
            Me.tcPlots.TabIndex = 1
            '
            'MainControl
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(191, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(255, Byte), Integer))
            Me.Controls.Add(Me.tcPlots)
            Me.Name = "MainControl"
            Me.Size = New System.Drawing.Size(995, 500)
            Me.tpDataSummary.ResumeLayout(False)
            Me.tpBoxWhisker.ResumeLayout(False)
            Me.tpHistogram.ResumeLayout(False)
            Me.tpProbability.ResumeLayout(False)
            Me.tpTimeSeries.ResumeLayout(False)
            Me.tcPlots.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents plotOptionsControl As New PlotOptionsControl
        Friend WithEvents summaryStatistics As SummaryStatistics
        Friend WithEvents dataSummary As DataSummary
        Friend WithEvents boxWhisker As BoxWhiskerPlot
        Friend WithEvents histogramPlot As HistogramPlot
        Friend WithEvents probabilityPlot As ProbabilityPlot
        Friend WithEvents ProgressBar As System.Windows.Forms.ProgressBar
        Friend WithEvents timeSeriesPlot As TimeSeriesPlot
        Private WithEvents tcPlots As System.Windows.Forms.TabControl
        Private WithEvents tpDataSummary As System.Windows.Forms.TabPage
        Private WithEvents tpBoxWhisker As System.Windows.Forms.TabPage
        Private WithEvents tpHistogram As System.Windows.Forms.TabPage
        Private WithEvents tpProbability As System.Windows.Forms.TabPage
        Private WithEvents tpTimeSeries As System.Windows.Forms.TabPage
    End Class
End Namespace