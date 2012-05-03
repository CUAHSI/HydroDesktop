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
            Me.CSummaryStatistics1 = New cSummaryStatistics
            Me.tpDataSummary = New System.Windows.Forms.TabPage
            Me.pDataSummary = New cDataSummary
            Me.tpBoxWhisker = New System.Windows.Forms.TabPage
            Me.pBoxWhisker = New cBoxWhiskerPlot
            Me.tpHistogram = New System.Windows.Forms.TabPage
            Me.pHistogram = New cHistogramPlot
            Me.tpProbability = New System.Windows.Forms.TabPage
            Me.pProbability = New cProbabilityPlot
            Me.tpTimeSeries = New System.Windows.Forms.TabPage
            Me.ProgressBar = New System.Windows.Forms.ProgressBar
            Me.TabControl2 = New System.Windows.Forms.TabControl
            Me.pTimeSeries = New cTimeSeriesPlot
            Me.tpDataSummary.SuspendLayout()
            Me.tpBoxWhisker.SuspendLayout()
            Me.tpHistogram.SuspendLayout()
            Me.tpProbability.SuspendLayout()
            Me.tpTimeSeries.SuspendLayout()
            Me.TabControl2.SuspendLayout()
            Me.SuspendLayout()
            '
            'CSummaryStatistics1
            '
            Me.CSummaryStatistics1.AutoScroll = True
            Me.CSummaryStatistics1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.CSummaryStatistics1.Location = New System.Drawing.Point(0, 0)
            Me.CSummaryStatistics1.Name = "CSummaryStatistics1"
            Me.CSummaryStatistics1.Padding = New System.Windows.Forms.Padding(3)
            Me.CSummaryStatistics1.Size = New System.Drawing.Size(230, 375)
            Me.CSummaryStatistics1.TabIndex = 0
            '
            'tpDataSummary
            '
            Me.tpDataSummary.Controls.Add(Me.pDataSummary)
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
            Me.pDataSummary.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pDataSummary.Location = New System.Drawing.Point(3, 3)
            Me.pDataSummary.Name = "pDataSummary"
            Me.pDataSummary.Size = New System.Drawing.Size(981, 485)
            Me.pDataSummary.TabIndex = 0
            '
            'tpBoxWhisker
            '
            Me.tpBoxWhisker.Controls.Add(Me.pBoxWhisker)
            Me.tpBoxWhisker.Location = New System.Drawing.Point(4, 5)
            Me.tpBoxWhisker.Name = "tpBoxWhisker"
            Me.tpBoxWhisker.Size = New System.Drawing.Size(987, 491)
            Me.tpBoxWhisker.TabIndex = 4
            Me.tpBoxWhisker.Text = "Box/Whisker"
            Me.tpBoxWhisker.UseVisualStyleBackColor = True
            '
            'pBoxWhisker
            '
            Me.pBoxWhisker.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pBoxWhisker.Location = New System.Drawing.Point(0, 0)
            Me.pBoxWhisker.Name = "pBoxWhisker"
            Me.pBoxWhisker.Size = New System.Drawing.Size(987, 491)
            Me.pBoxWhisker.TabIndex = 0
            '
            'tpHistogram
            '
            Me.tpHistogram.Controls.Add(Me.pHistogram)
            Me.tpHistogram.Location = New System.Drawing.Point(4, 5)
            Me.tpHistogram.Name = "tpHistogram"
            Me.tpHistogram.Size = New System.Drawing.Size(987, 491)
            Me.tpHistogram.TabIndex = 3
            Me.tpHistogram.Text = "Histogram"
            Me.tpHistogram.UseVisualStyleBackColor = True
            '
            'pHistogram
            '
            Me.pHistogram.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pHistogram.Location = New System.Drawing.Point(0, 0)
            Me.pHistogram.Name = "pHistogram"
            Me.pHistogram.Size = New System.Drawing.Size(987, 491)
            Me.pHistogram.TabIndex = 0
            '
            'tpProbability
            '
            Me.tpProbability.Controls.Add(Me.pProbability)
            Me.tpProbability.Location = New System.Drawing.Point(4, 5)
            Me.tpProbability.Name = "tpProbability"
            Me.tpProbability.Size = New System.Drawing.Size(987, 491)
            Me.tpProbability.TabIndex = 2
            Me.tpProbability.Text = "Probability"
            Me.tpProbability.UseVisualStyleBackColor = True
            '
            'pProbability
            '
            Me.pProbability.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pProbability.Location = New System.Drawing.Point(0, 0)
            Me.pProbability.Name = "pProbability"
            Me.pProbability.SeriesSelector = Nothing
            Me.pProbability.Size = New System.Drawing.Size(987, 491)
            Me.pProbability.TabIndex = 0
            '
            'tpTimeSeries
            '
            Me.tpTimeSeries.Controls.Add(Me.ProgressBar)
            Me.tpTimeSeries.Controls.Add(Me.pTimeSeries)
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
            'TabControl2
            '
            Me.TabControl2.Appearance = System.Windows.Forms.TabAppearance.Buttons
            Me.TabControl2.Controls.Add(Me.tpTimeSeries)
            Me.TabControl2.Controls.Add(Me.tpProbability)
            Me.TabControl2.Controls.Add(Me.tpHistogram)
            Me.TabControl2.Controls.Add(Me.tpBoxWhisker)
            Me.TabControl2.Controls.Add(Me.tpDataSummary)
            Me.TabControl2.Dock = System.Windows.Forms.DockStyle.Fill
            Me.TabControl2.ItemSize = New System.Drawing.Size(0, 1)
            Me.TabControl2.Location = New System.Drawing.Point(0, 0)
            Me.TabControl2.Name = "TabControl2"
            Me.TabControl2.SelectedIndex = 0
            Me.TabControl2.Size = New System.Drawing.Size(995, 500)
            Me.TabControl2.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
            Me.TabControl2.TabIndex = 1
            '
            'pTimeSeries
            '
            Me.pTimeSeries.Dock = System.Windows.Forms.DockStyle.Fill
            Me.pTimeSeries.Location = New System.Drawing.Point(0, 0)
            Me.pTimeSeries.Name = "pTimeSeries"
            Me.pTimeSeries.Size = New System.Drawing.Size(987, 491)
            Me.pTimeSeries.TabIndex = 0
            '
            'cTSA
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(191, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(255, Byte), Integer))
            Me.Controls.Add(Me.TabControl2)
            Me.Name = "cTSA"
            Me.Size = New System.Drawing.Size(995, 500)
            Me.tpDataSummary.ResumeLayout(False)
            Me.tpBoxWhisker.ResumeLayout(False)
            Me.tpHistogram.ResumeLayout(False)
            Me.tpProbability.ResumeLayout(False)
            Me.tpTimeSeries.ResumeLayout(False)
            Me.TabControl2.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents abc As cPlotOptions.ControlCollection
        Friend WithEvents CPlotOptions1 As New cPlotOptions
        Friend WithEvents CSummaryStatistics1 As cSummaryStatistics
        Friend WithEvents tpDataSummary As System.Windows.Forms.TabPage
        Friend WithEvents pDataSummary As cDataSummary
        Friend WithEvents tpBoxWhisker As System.Windows.Forms.TabPage
        Friend WithEvents pBoxWhisker As cBoxWhiskerPlot
        Friend WithEvents tpHistogram As System.Windows.Forms.TabPage
        Friend WithEvents pHistogram As cHistogramPlot
        Friend WithEvents tpProbability As System.Windows.Forms.TabPage
        Friend WithEvents pProbability As cProbabilityPlot
        Friend WithEvents tpTimeSeries As System.Windows.Forms.TabPage
        Friend WithEvents ProgressBar As System.Windows.Forms.ProgressBar
        Friend WithEvents pTimeSeries As cTimeSeriesPlot
        Friend WithEvents TabControl2 As System.Windows.Forms.TabControl
        'Friend WithEvents tpSummaryPlot As System.Windows.Forms.TabPage
        'Friend WithEvents pSummaryPlot As cSummaryPlot
    End Class
End Namespace