<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class cPlotOptions
    Inherits System.Windows.Forms.UserControl

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
        Me.gboxOtherOptions = New System.Windows.Forms.GroupBox
        Me.gboxLegend = New System.Windows.Forms.GroupBox
        Me.ckbShowLegend = New System.Windows.Forms.CheckBox
        Me.gboxColors = New System.Windows.Forms.GroupBox
        Me.gBoxColorCollection = New System.Windows.Forms.GroupBox
        Me.txtPointccNumber = New System.Windows.Forms.TextBox
        Me.txtLineccNumber = New System.Windows.Forms.TextBox
        Me.lblPointccNumber = New System.Windows.Forms.Label
        Me.lblLineccNumber = New System.Windows.Forms.Label
        Me.btnColorConnections = New System.Windows.Forms.Button
        Me.btnSetPointColor = New System.Windows.Forms.Button
        Me.btnSetLineColor = New System.Windows.Forms.Button
        Me.lblPointColor = New System.Windows.Forms.Label
        Me.lblLineColor = New System.Windows.Forms.Label
        Me.gboxTSPlotOptions = New System.Windows.Forms.GroupBox
        Me.gboxTSPlotType = New System.Windows.Forms.GroupBox
        Me.FlowLayoutPanel3 = New System.Windows.Forms.FlowLayoutPanel
        Me.rbtnTSLine = New System.Windows.Forms.RadioButton
        Me.rbtnTSPoint = New System.Windows.Forms.RadioButton
        Me.rbtnTSBoth = New System.Windows.Forms.RadioButton
        Me.gboxBoxPlotOptions = New System.Windows.Forms.GroupBox
        Me.gboxBPPlotType = New System.Windows.Forms.GroupBox
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel
        Me.rbtnBPMonthly = New System.Windows.Forms.RadioButton
        Me.rbtnBPSeasonal = New System.Windows.Forms.RadioButton
        Me.rbtnBPYearly = New System.Windows.Forms.RadioButton
        Me.rbtnBPOverall = New System.Windows.Forms.RadioButton
        Me.gboxHistPlotOptions = New System.Windows.Forms.GroupBox
        Me.gboxHPAlgorithms = New System.Windows.Forms.GroupBox
        Me.FlowLayoutPanel4 = New System.Windows.Forms.FlowLayoutPanel
        Me.rbtnHPSturges = New System.Windows.Forms.RadioButton
        Me.rbtnHPScotts = New System.Windows.Forms.RadioButton
        Me.rbtnHPFreedman = New System.Windows.Forms.RadioButton
        Me.gboxHPHistogramType = New System.Windows.Forms.GroupBox
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel
        Me.rbtnHPCount = New System.Windows.Forms.RadioButton
        Me.rbtnHPProbability = New System.Windows.Forms.RadioButton
        Me.rbtnHPRelative = New System.Windows.Forms.RadioButton
        Me.gboxDateRange = New System.Windows.Forms.GroupBox
        Me.ckbDateRangeChange = New System.Windows.Forms.CheckBox
        Me.dtpEndDatePicker = New System.Windows.Forms.DateTimePicker
        Me.dtpStartDatePicker = New System.Windows.Forms.DateTimePicker
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.btnplot = New System.Windows.Forms.Button
        Me.gboxStatisticOptions = New System.Windows.Forms.GroupBox
        Me.ckboxUseCensoredData = New System.Windows.Forms.CheckBox
        Me.gboxOtherOptions.SuspendLayout()
        Me.gboxLegend.SuspendLayout()
        Me.gboxColors.SuspendLayout()
        Me.gBoxColorCollection.SuspendLayout()
        Me.gboxTSPlotOptions.SuspendLayout()
        Me.gboxTSPlotType.SuspendLayout()
        Me.FlowLayoutPanel3.SuspendLayout()
        Me.gboxBoxPlotOptions.SuspendLayout()
        Me.gboxBPPlotType.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.gboxHistPlotOptions.SuspendLayout()
        Me.gboxHPAlgorithms.SuspendLayout()
        Me.FlowLayoutPanel4.SuspendLayout()
        Me.gboxHPHistogramType.SuspendLayout()
        Me.FlowLayoutPanel2.SuspendLayout()
        Me.gboxDateRange.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.gboxStatisticOptions.SuspendLayout()
        Me.SuspendLayout()
        '
        'gboxOtherOptions
        '
        Me.gboxOtherOptions.AutoSize = True
        Me.gboxOtherOptions.Controls.Add(Me.gboxLegend)
        Me.gboxOtherOptions.Controls.Add(Me.gboxColors)
        Me.gboxOtherOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gboxOtherOptions.Location = New System.Drawing.Point(3, 343)
        Me.gboxOtherOptions.Name = "gboxOtherOptions"
        Me.gboxOtherOptions.Size = New System.Drawing.Size(224, 182)
        Me.gboxOtherOptions.TabIndex = 22
        Me.gboxOtherOptions.TabStop = False
        Me.gboxOtherOptions.Text = "Options Apply to All Plots"
        '
        'gboxLegend
        '
        Me.gboxLegend.Controls.Add(Me.ckbShowLegend)
        Me.gboxLegend.Dock = System.Windows.Forms.DockStyle.Top
        Me.gboxLegend.Location = New System.Drawing.Point(3, 136)
        Me.gboxLegend.Name = "gboxLegend"
        Me.gboxLegend.Size = New System.Drawing.Size(218, 43)
        Me.gboxLegend.TabIndex = 1
        Me.gboxLegend.TabStop = False
        Me.gboxLegend.Text = "Legend"
        '
        'ckbShowLegend
        '
        Me.ckbShowLegend.AutoSize = True
        Me.ckbShowLegend.Checked = True
        Me.ckbShowLegend.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ckbShowLegend.Location = New System.Drawing.Point(7, 20)
        Me.ckbShowLegend.Name = "ckbShowLegend"
        Me.ckbShowLegend.Size = New System.Drawing.Size(92, 17)
        Me.ckbShowLegend.TabIndex = 0
        Me.ckbShowLegend.Text = "Show Legend"
        Me.ckbShowLegend.UseVisualStyleBackColor = True
        '
        'gboxColors
        '
        Me.gboxColors.Controls.Add(Me.gBoxColorCollection)
        Me.gboxColors.Controls.Add(Me.btnSetPointColor)
        Me.gboxColors.Controls.Add(Me.btnSetLineColor)
        Me.gboxColors.Controls.Add(Me.lblPointColor)
        Me.gboxColors.Controls.Add(Me.lblLineColor)
        Me.gboxColors.Dock = System.Windows.Forms.DockStyle.Top
        Me.gboxColors.Location = New System.Drawing.Point(3, 16)
        Me.gboxColors.Name = "gboxColors"
        Me.gboxColors.Size = New System.Drawing.Size(218, 120)
        Me.gboxColors.TabIndex = 0
        Me.gboxColors.TabStop = False
        Me.gboxColors.Text = "Plot Colors"
        '
        'gBoxColorCollection
        '
        Me.gBoxColorCollection.Controls.Add(Me.txtPointccNumber)
        Me.gBoxColorCollection.Controls.Add(Me.txtLineccNumber)
        Me.gBoxColorCollection.Controls.Add(Me.lblPointccNumber)
        Me.gBoxColorCollection.Controls.Add(Me.lblLineccNumber)
        Me.gBoxColorCollection.Controls.Add(Me.btnColorConnections)
        Me.gBoxColorCollection.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.gBoxColorCollection.Location = New System.Drawing.Point(3, 44)
        Me.gBoxColorCollection.Name = "gBoxColorCollection"
        Me.gBoxColorCollection.Size = New System.Drawing.Size(212, 73)
        Me.gBoxColorCollection.TabIndex = 5
        Me.gBoxColorCollection.TabStop = False
        Me.gBoxColorCollection.Text = "Use Color Collection number:"
        '
        'txtPointccNumber
        '
        Me.txtPointccNumber.Location = New System.Drawing.Point(130, 17)
        Me.txtPointccNumber.Name = "txtPointccNumber"
        Me.txtPointccNumber.Size = New System.Drawing.Size(20, 20)
        Me.txtPointccNumber.TabIndex = 8
        Me.txtPointccNumber.Text = "0"
        '
        'txtLineccNumber
        '
        Me.txtLineccNumber.Location = New System.Drawing.Point(41, 17)
        Me.txtLineccNumber.Name = "txtLineccNumber"
        Me.txtLineccNumber.Size = New System.Drawing.Size(20, 20)
        Me.txtLineccNumber.TabIndex = 7
        Me.txtLineccNumber.Text = "0"
        '
        'lblPointccNumber
        '
        Me.lblPointccNumber.AutoSize = True
        Me.lblPointccNumber.Location = New System.Drawing.Point(88, 20)
        Me.lblPointccNumber.Name = "lblPointccNumber"
        Me.lblPointccNumber.Size = New System.Drawing.Size(36, 13)
        Me.lblPointccNumber.TabIndex = 6
        Me.lblPointccNumber.Text = "Points"
        '
        'lblLineccNumber
        '
        Me.lblLineccNumber.AutoSize = True
        Me.lblLineccNumber.Location = New System.Drawing.Point(6, 20)
        Me.lblLineccNumber.Name = "lblLineccNumber"
        Me.lblLineccNumber.Size = New System.Drawing.Size(32, 13)
        Me.lblLineccNumber.TabIndex = 5
        Me.lblLineccNumber.Text = "Lines"
        '
        'btnColorConnections
        '
        Me.btnColorConnections.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.btnColorConnections.Location = New System.Drawing.Point(3, 45)
        Me.btnColorConnections.Name = "btnColorConnections"
        Me.btnColorConnections.Size = New System.Drawing.Size(206, 25)
        Me.btnColorConnections.TabIndex = 4
        Me.btnColorConnections.Text = "Get Color Collections Number"
        Me.btnColorConnections.UseVisualStyleBackColor = True
        '
        'btnSetPointColor
        '
        Me.btnSetPointColor.BackColor = System.Drawing.Color.Black
        Me.btnSetPointColor.Location = New System.Drawing.Point(133, 17)
        Me.btnSetPointColor.Name = "btnSetPointColor"
        Me.btnSetPointColor.Size = New System.Drawing.Size(18, 18)
        Me.btnSetPointColor.TabIndex = 3
        Me.btnSetPointColor.UseVisualStyleBackColor = False
        '
        'btnSetLineColor
        '
        Me.btnSetLineColor.BackColor = System.Drawing.Color.Black
        Me.btnSetLineColor.Location = New System.Drawing.Point(44, 17)
        Me.btnSetLineColor.Name = "btnSetLineColor"
        Me.btnSetLineColor.Size = New System.Drawing.Size(18, 18)
        Me.btnSetLineColor.TabIndex = 2
        Me.btnSetLineColor.UseVisualStyleBackColor = False
        '
        'lblPointColor
        '
        Me.lblPointColor.AutoSize = True
        Me.lblPointColor.Location = New System.Drawing.Point(91, 20)
        Me.lblPointColor.Name = "lblPointColor"
        Me.lblPointColor.Size = New System.Drawing.Size(36, 13)
        Me.lblPointColor.TabIndex = 1
        Me.lblPointColor.Text = "Points"
        '
        'lblLineColor
        '
        Me.lblLineColor.AutoSize = True
        Me.lblLineColor.Location = New System.Drawing.Point(6, 20)
        Me.lblLineColor.Name = "lblLineColor"
        Me.lblLineColor.Size = New System.Drawing.Size(32, 13)
        Me.lblLineColor.TabIndex = 0
        Me.lblLineColor.Text = "Lines"
        '
        'gboxTSPlotOptions
        '
        Me.gboxTSPlotOptions.AutoSize = True
        Me.gboxTSPlotOptions.Controls.Add(Me.gboxTSPlotType)
        Me.gboxTSPlotOptions.Dock = System.Windows.Forms.DockStyle.Top
        Me.gboxTSPlotOptions.Location = New System.Drawing.Point(3, 3)
        Me.gboxTSPlotOptions.Name = "gboxTSPlotOptions"
        Me.gboxTSPlotOptions.Size = New System.Drawing.Size(224, 68)
        Me.gboxTSPlotOptions.TabIndex = 19
        Me.gboxTSPlotOptions.TabStop = False
        Me.gboxTSPlotOptions.Text = "Time Series Plot"
        '
        'gboxTSPlotType
        '
        Me.gboxTSPlotType.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.gboxTSPlotType.Controls.Add(Me.FlowLayoutPanel3)
        Me.gboxTSPlotType.Dock = System.Windows.Forms.DockStyle.Top
        Me.gboxTSPlotType.Location = New System.Drawing.Point(3, 16)
        Me.gboxTSPlotType.Name = "gboxTSPlotType"
        Me.gboxTSPlotType.Size = New System.Drawing.Size(218, 49)
        Me.gboxTSPlotType.TabIndex = 0
        Me.gboxTSPlotType.TabStop = False
        Me.gboxTSPlotType.Text = "Plot Type"
        '
        'FlowLayoutPanel3
        '
        Me.FlowLayoutPanel3.AutoSize = True
        Me.FlowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel3.Controls.Add(Me.rbtnTSLine)
        Me.FlowLayoutPanel3.Controls.Add(Me.rbtnTSPoint)
        Me.FlowLayoutPanel3.Controls.Add(Me.rbtnTSBoth)
        Me.FlowLayoutPanel3.Location = New System.Drawing.Point(6, 19)
        Me.FlowLayoutPanel3.Name = "FlowLayoutPanel3"
        Me.FlowLayoutPanel3.Size = New System.Drawing.Size(159, 23)
        Me.FlowLayoutPanel3.TabIndex = 6
        '
        'rbtnTSLine
        '
        Me.rbtnTSLine.AutoSize = True
        Me.rbtnTSLine.Location = New System.Drawing.Point(3, 3)
        Me.rbtnTSLine.Name = "rbtnTSLine"
        Me.rbtnTSLine.Size = New System.Drawing.Size(45, 17)
        Me.rbtnTSLine.TabIndex = 3
        Me.rbtnTSLine.Text = "Line"
        '
        'rbtnTSPoint
        '
        Me.rbtnTSPoint.AutoSize = True
        Me.rbtnTSPoint.Location = New System.Drawing.Point(54, 3)
        Me.rbtnTSPoint.Name = "rbtnTSPoint"
        Me.rbtnTSPoint.Size = New System.Drawing.Size(49, 17)
        Me.rbtnTSPoint.TabIndex = 4
        Me.rbtnTSPoint.Text = "Point"
        '
        'rbtnTSBoth
        '
        Me.rbtnTSBoth.AutoSize = True
        Me.rbtnTSBoth.Checked = True
        Me.rbtnTSBoth.Location = New System.Drawing.Point(109, 3)
        Me.rbtnTSBoth.Name = "rbtnTSBoth"
        Me.rbtnTSBoth.Size = New System.Drawing.Size(47, 17)
        Me.rbtnTSBoth.TabIndex = 5
        Me.rbtnTSBoth.TabStop = True
        Me.rbtnTSBoth.Text = "Both"
        '
        'gboxBoxPlotOptions
        '
        Me.gboxBoxPlotOptions.Controls.Add(Me.gboxBPPlotType)
        Me.gboxBoxPlotOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gboxBoxPlotOptions.Location = New System.Drawing.Point(3, 77)
        Me.gboxBoxPlotOptions.Name = "gboxBoxPlotOptions"
        Me.gboxBoxPlotOptions.Size = New System.Drawing.Size(224, 85)
        Me.gboxBoxPlotOptions.TabIndex = 20
        Me.gboxBoxPlotOptions.TabStop = False
        Me.gboxBoxPlotOptions.Text = "Box/Whisker Plot"
        '
        'gboxBPPlotType
        '
        Me.gboxBPPlotType.AutoSize = True
        Me.gboxBPPlotType.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.gboxBPPlotType.Controls.Add(Me.FlowLayoutPanel1)
        Me.gboxBPPlotType.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gboxBPPlotType.Location = New System.Drawing.Point(3, 16)
        Me.gboxBPPlotType.Name = "gboxBPPlotType"
        Me.gboxBPPlotType.Size = New System.Drawing.Size(218, 66)
        Me.gboxBPPlotType.TabIndex = 13
        Me.gboxBPPlotType.TabStop = False
        Me.gboxBPPlotType.Text = "Plot Type"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.FlowLayoutPanel1.Controls.Add(Me.rbtnBPMonthly)
        Me.FlowLayoutPanel1.Controls.Add(Me.rbtnBPSeasonal)
        Me.FlowLayoutPanel1.Controls.Add(Me.rbtnBPYearly)
        Me.FlowLayoutPanel1.Controls.Add(Me.rbtnBPOverall)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(3, 16)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(173, 47)
        Me.FlowLayoutPanel1.TabIndex = 8
        '
        'rbtnBPMonthly
        '
        Me.rbtnBPMonthly.AutoSize = True
        Me.rbtnBPMonthly.Checked = True
        Me.rbtnBPMonthly.Location = New System.Drawing.Point(3, 3)
        Me.rbtnBPMonthly.Name = "rbtnBPMonthly"
        Me.rbtnBPMonthly.Size = New System.Drawing.Size(62, 17)
        Me.rbtnBPMonthly.TabIndex = 3
        Me.rbtnBPMonthly.TabStop = True
        Me.rbtnBPMonthly.Text = "Monthly"
        '
        'rbtnBPSeasonal
        '
        Me.rbtnBPSeasonal.AutoSize = True
        Me.rbtnBPSeasonal.Location = New System.Drawing.Point(71, 3)
        Me.rbtnBPSeasonal.Name = "rbtnBPSeasonal"
        Me.rbtnBPSeasonal.Size = New System.Drawing.Size(69, 17)
        Me.rbtnBPSeasonal.TabIndex = 4
        Me.rbtnBPSeasonal.Text = "Seasonal"
        '
        'rbtnBPYearly
        '
        Me.rbtnBPYearly.AutoSize = True
        Me.rbtnBPYearly.Location = New System.Drawing.Point(3, 26)
        Me.rbtnBPYearly.Name = "rbtnBPYearly"
        Me.rbtnBPYearly.Size = New System.Drawing.Size(54, 17)
        Me.rbtnBPYearly.TabIndex = 5
        Me.rbtnBPYearly.Text = "Yearly"
        '
        'rbtnBPOverall
        '
        Me.rbtnBPOverall.AutoSize = True
        Me.rbtnBPOverall.Location = New System.Drawing.Point(63, 26)
        Me.rbtnBPOverall.Name = "rbtnBPOverall"
        Me.rbtnBPOverall.Size = New System.Drawing.Size(58, 17)
        Me.rbtnBPOverall.TabIndex = 6
        Me.rbtnBPOverall.Text = "Overall"
        '
        'gboxHistPlotOptions
        '
        Me.gboxHistPlotOptions.AutoSize = True
        Me.gboxHistPlotOptions.Controls.Add(Me.gboxHPAlgorithms)
        Me.gboxHistPlotOptions.Controls.Add(Me.gboxHPHistogramType)
        Me.gboxHistPlotOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gboxHistPlotOptions.Location = New System.Drawing.Point(3, 168)
        Me.gboxHistPlotOptions.Name = "gboxHistPlotOptions"
        Me.gboxHistPlotOptions.Size = New System.Drawing.Size(224, 169)
        Me.gboxHistPlotOptions.TabIndex = 21
        Me.gboxHistPlotOptions.TabStop = False
        Me.gboxHistPlotOptions.Text = "Histogram  Plot"
        '
        'gboxHPAlgorithms
        '
        Me.gboxHPAlgorithms.Controls.Add(Me.FlowLayoutPanel4)
        Me.gboxHPAlgorithms.Dock = System.Windows.Forms.DockStyle.Top
        Me.gboxHPAlgorithms.Location = New System.Drawing.Point(3, 85)
        Me.gboxHPAlgorithms.Name = "gboxHPAlgorithms"
        Me.gboxHPAlgorithms.Size = New System.Drawing.Size(218, 81)
        Me.gboxHPAlgorithms.TabIndex = 1
        Me.gboxHPAlgorithms.TabStop = False
        Me.gboxHPAlgorithms.Text = "Binning Algorithms"
        '
        'FlowLayoutPanel4
        '
        Me.FlowLayoutPanel4.Controls.Add(Me.rbtnHPSturges)
        Me.FlowLayoutPanel4.Controls.Add(Me.rbtnHPScotts)
        Me.FlowLayoutPanel4.Controls.Add(Me.rbtnHPFreedman)
        Me.FlowLayoutPanel4.Location = New System.Drawing.Point(3, 19)
        Me.FlowLayoutPanel4.Name = "FlowLayoutPanel4"
        Me.FlowLayoutPanel4.Size = New System.Drawing.Size(191, 56)
        Me.FlowLayoutPanel4.TabIndex = 1
        '
        'rbtnHPSturges
        '
        Me.rbtnHPSturges.AutoSize = True
        Me.rbtnHPSturges.Checked = True
        Me.rbtnHPSturges.Location = New System.Drawing.Point(3, 3)
        Me.rbtnHPSturges.Name = "rbtnHPSturges"
        Me.rbtnHPSturges.Size = New System.Drawing.Size(63, 17)
        Me.rbtnHPSturges.TabIndex = 0
        Me.rbtnHPSturges.TabStop = True
        Me.rbtnHPSturges.Text = "Sturges'"
        Me.rbtnHPSturges.UseVisualStyleBackColor = True
        '
        'rbtnHPScotts
        '
        Me.rbtnHPScotts.AutoSize = True
        Me.rbtnHPScotts.Location = New System.Drawing.Point(72, 3)
        Me.rbtnHPScotts.Name = "rbtnHPScotts"
        Me.rbtnHPScotts.Size = New System.Drawing.Size(57, 17)
        Me.rbtnHPScotts.TabIndex = 1
        Me.rbtnHPScotts.Text = "Scott's"
        Me.rbtnHPScotts.UseVisualStyleBackColor = True
        '
        'rbtnHPFreedman
        '
        Me.rbtnHPFreedman.AutoSize = True
        Me.rbtnHPFreedman.Location = New System.Drawing.Point(3, 26)
        Me.rbtnHPFreedman.Name = "rbtnHPFreedman"
        Me.rbtnHPFreedman.Size = New System.Drawing.Size(119, 17)
        Me.rbtnHPFreedman.TabIndex = 2
        Me.rbtnHPFreedman.Text = "Freedman-Diaconis’"
        Me.rbtnHPFreedman.UseVisualStyleBackColor = True
        '
        'gboxHPHistogramType
        '
        Me.gboxHPHistogramType.Controls.Add(Me.FlowLayoutPanel2)
        Me.gboxHPHistogramType.Dock = System.Windows.Forms.DockStyle.Top
        Me.gboxHPHistogramType.Location = New System.Drawing.Point(3, 16)
        Me.gboxHPHistogramType.Name = "gboxHPHistogramType"
        Me.gboxHPHistogramType.Size = New System.Drawing.Size(218, 69)
        Me.gboxHPHistogramType.TabIndex = 0
        Me.gboxHPHistogramType.TabStop = False
        Me.gboxHPHistogramType.Text = "Histogram Type"
        '
        'FlowLayoutPanel2
        '
        Me.FlowLayoutPanel2.Controls.Add(Me.rbtnHPCount)
        Me.FlowLayoutPanel2.Controls.Add(Me.rbtnHPProbability)
        Me.FlowLayoutPanel2.Controls.Add(Me.rbtnHPRelative)
        Me.FlowLayoutPanel2.Location = New System.Drawing.Point(3, 19)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        Me.FlowLayoutPanel2.Size = New System.Drawing.Size(191, 45)
        Me.FlowLayoutPanel2.TabIndex = 1
        '
        'rbtnHPCount
        '
        Me.rbtnHPCount.AutoSize = True
        Me.rbtnHPCount.Checked = True
        Me.rbtnHPCount.Location = New System.Drawing.Point(3, 3)
        Me.rbtnHPCount.Name = "rbtnHPCount"
        Me.rbtnHPCount.Size = New System.Drawing.Size(53, 17)
        Me.rbtnHPCount.TabIndex = 0
        Me.rbtnHPCount.TabStop = True
        Me.rbtnHPCount.Text = "Count"
        Me.rbtnHPCount.UseVisualStyleBackColor = True
        '
        'rbtnHPProbability
        '
        Me.rbtnHPProbability.AutoSize = True
        Me.rbtnHPProbability.Location = New System.Drawing.Point(62, 3)
        Me.rbtnHPProbability.Name = "rbtnHPProbability"
        Me.rbtnHPProbability.Size = New System.Drawing.Size(111, 17)
        Me.rbtnHPProbability.TabIndex = 1
        Me.rbtnHPProbability.Text = "Probability Density"
        Me.rbtnHPProbability.UseVisualStyleBackColor = True
        '
        'rbtnHPRelative
        '
        Me.rbtnHPRelative.AutoSize = True
        Me.rbtnHPRelative.Location = New System.Drawing.Point(3, 26)
        Me.rbtnHPRelative.Name = "rbtnHPRelative"
        Me.rbtnHPRelative.Size = New System.Drawing.Size(125, 17)
        Me.rbtnHPRelative.TabIndex = 2
        Me.rbtnHPRelative.Text = "Relative Frequencies"
        Me.rbtnHPRelative.UseVisualStyleBackColor = True
        '
        'gboxDateRange
        '
        Me.gboxDateRange.Controls.Add(Me.ckbDateRangeChange)
        Me.gboxDateRange.Controls.Add(Me.dtpEndDatePicker)
        Me.gboxDateRange.Controls.Add(Me.dtpStartDatePicker)
        Me.gboxDateRange.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gboxDateRange.Location = New System.Drawing.Point(3, 579)
        Me.gboxDateRange.Name = "gboxDateRange"
        Me.gboxDateRange.Size = New System.Drawing.Size(224, 92)
        Me.gboxDateRange.TabIndex = 26
        Me.gboxDateRange.TabStop = False
        Me.gboxDateRange.Text = "Date Range"
        '
        'ckbDateRangeChange
        '
        Me.ckbDateRangeChange.AutoSize = True
        Me.ckbDateRangeChange.Checked = True
        Me.ckbDateRangeChange.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ckbDateRangeChange.Location = New System.Drawing.Point(6, 71)
        Me.ckbDateRangeChange.Name = "ckbDateRangeChange"
        Me.ckbDateRangeChange.Size = New System.Drawing.Size(130, 17)
        Me.ckbDateRangeChange.TabIndex = 2
        Me.ckbDateRangeChange.Text = "Display full date range"
        Me.ckbDateRangeChange.UseVisualStyleBackColor = True
        '
        'dtpEndDatePicker
        '
        Me.dtpEndDatePicker.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtpEndDatePicker.Location = New System.Drawing.Point(3, 45)
        Me.dtpEndDatePicker.Name = "dtpEndDatePicker"
        Me.dtpEndDatePicker.Size = New System.Drawing.Size(219, 20)
        Me.dtpEndDatePicker.TabIndex = 1
        Me.dtpEndDatePicker.Value = New Date(2010, 3, 2, 0, 0, 0, 0)
        '
        'dtpStartDatePicker
        '
        Me.dtpStartDatePicker.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dtpStartDatePicker.Location = New System.Drawing.Point(3, 19)
        Me.dtpStartDatePicker.Name = "dtpStartDatePicker"
        Me.dtpStartDatePicker.Size = New System.Drawing.Size(219, 20)
        Me.dtpStartDatePicker.TabIndex = 0
        Me.dtpStartDatePicker.Value = New Date(2010, 3, 2, 0, 0, 0, 0)
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.TableLayoutPanel1.Controls.Add(Me.gboxDateRange, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.btnplot, 0, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.gboxHistPlotOptions, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.gboxBoxPlotOptions, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.gboxTSPlotOptions, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.gboxOtherOptions, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.gboxStatisticOptions, 0, 4)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 7
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle)
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(230, 707)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'btnplot
        '
        Me.btnplot.Dock = System.Windows.Forms.DockStyle.Top
        Me.btnplot.Location = New System.Drawing.Point(3, 677)
        Me.btnplot.Name = "btnplot"
        Me.btnplot.Size = New System.Drawing.Size(224, 27)
        Me.btnplot.TabIndex = 23
        Me.btnplot.Text = "Apply Options"
        Me.btnplot.UseVisualStyleBackColor = True
        '
        'gboxStatisticOptions
        '
        Me.gboxStatisticOptions.AutoSize = True
        Me.gboxStatisticOptions.Controls.Add(Me.ckboxUseCensoredData)
        Me.gboxStatisticOptions.Dock = System.Windows.Forms.DockStyle.Top
        Me.gboxStatisticOptions.Location = New System.Drawing.Point(3, 531)
        Me.gboxStatisticOptions.Name = "gboxStatisticOptions"
        Me.gboxStatisticOptions.Size = New System.Drawing.Size(224, 42)
        Me.gboxStatisticOptions.TabIndex = 27
        Me.gboxStatisticOptions.TabStop = False
        Me.gboxStatisticOptions.Text = "Statistic Options"
        '
        'ckboxUseCensoredData
        '
        Me.ckboxUseCensoredData.AutoSize = True
        Me.ckboxUseCensoredData.Dock = System.Windows.Forms.DockStyle.Top
        Me.ckboxUseCensoredData.Location = New System.Drawing.Point(3, 16)
        Me.ckboxUseCensoredData.Name = "ckboxUseCensoredData"
        Me.ckboxUseCensoredData.Padding = New System.Windows.Forms.Padding(3)
        Me.ckboxUseCensoredData.Size = New System.Drawing.Size(218, 23)
        Me.ckboxUseCensoredData.TabIndex = 39
        Me.ckboxUseCensoredData.Text = "Use censored data in summary statistics."
        '
        'cPlotOptions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.AutoSize = True
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "cPlotOptions"
        Me.Size = New System.Drawing.Size(233, 710)
        Me.gboxOtherOptions.ResumeLayout(False)
        Me.gboxLegend.ResumeLayout(False)
        Me.gboxLegend.PerformLayout()
        Me.gboxColors.ResumeLayout(False)
        Me.gboxColors.PerformLayout()
        Me.gBoxColorCollection.ResumeLayout(False)
        Me.gBoxColorCollection.PerformLayout()
        Me.gboxTSPlotOptions.ResumeLayout(False)
        Me.gboxTSPlotType.ResumeLayout(False)
        Me.gboxTSPlotType.PerformLayout()
        Me.FlowLayoutPanel3.ResumeLayout(False)
        Me.FlowLayoutPanel3.PerformLayout()
        Me.gboxBoxPlotOptions.ResumeLayout(False)
        Me.gboxBoxPlotOptions.PerformLayout()
        Me.gboxBPPlotType.ResumeLayout(False)
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.FlowLayoutPanel1.PerformLayout()
        Me.gboxHistPlotOptions.ResumeLayout(False)
        Me.gboxHPAlgorithms.ResumeLayout(False)
        Me.FlowLayoutPanel4.ResumeLayout(False)
        Me.FlowLayoutPanel4.PerformLayout()
        Me.gboxHPHistogramType.ResumeLayout(False)
        Me.FlowLayoutPanel2.ResumeLayout(False)
        Me.FlowLayoutPanel2.PerformLayout()
        Me.gboxDateRange.ResumeLayout(False)
        Me.gboxDateRange.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.gboxStatisticOptions.ResumeLayout(False)
        Me.gboxStatisticOptions.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents gboxOtherOptions As System.Windows.Forms.GroupBox
    Friend WithEvents gboxLegend As System.Windows.Forms.GroupBox
    Friend WithEvents ckbShowLegend As System.Windows.Forms.CheckBox
    Friend WithEvents gboxColors As System.Windows.Forms.GroupBox
    Friend WithEvents gBoxColorCollection As System.Windows.Forms.GroupBox
    Friend WithEvents txtPointccNumber As System.Windows.Forms.TextBox
    Friend WithEvents txtLineccNumber As System.Windows.Forms.TextBox
    Friend WithEvents lblPointccNumber As System.Windows.Forms.Label
    Friend WithEvents lblLineccNumber As System.Windows.Forms.Label
    Friend WithEvents btnColorConnections As System.Windows.Forms.Button
    Friend WithEvents btnSetPointColor As System.Windows.Forms.Button
    Friend WithEvents btnSetLineColor As System.Windows.Forms.Button
    Friend WithEvents lblPointColor As System.Windows.Forms.Label
    Friend WithEvents lblLineColor As System.Windows.Forms.Label
    Friend WithEvents gboxTSPlotOptions As System.Windows.Forms.GroupBox
    Friend WithEvents gboxTSPlotType As System.Windows.Forms.GroupBox
    Friend WithEvents FlowLayoutPanel3 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents rbtnTSLine As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnTSPoint As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnTSBoth As System.Windows.Forms.RadioButton
    Friend WithEvents gboxBoxPlotOptions As System.Windows.Forms.GroupBox
    Friend WithEvents gboxBPPlotType As System.Windows.Forms.GroupBox
    Friend WithEvents FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents rbtnBPMonthly As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnBPSeasonal As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnBPYearly As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnBPOverall As System.Windows.Forms.RadioButton
    Friend WithEvents gboxHistPlotOptions As System.Windows.Forms.GroupBox
    Friend WithEvents gboxHPAlgorithms As System.Windows.Forms.GroupBox
    Friend WithEvents FlowLayoutPanel4 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents rbtnHPSturges As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnHPScotts As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnHPFreedman As System.Windows.Forms.RadioButton
    Friend WithEvents gboxHPHistogramType As System.Windows.Forms.GroupBox
    Friend WithEvents FlowLayoutPanel2 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents rbtnHPCount As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnHPProbability As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnHPRelative As System.Windows.Forms.RadioButton
    Friend WithEvents gboxDateRange As System.Windows.Forms.GroupBox
    Friend WithEvents ckbDateRangeChange As System.Windows.Forms.CheckBox
    Friend WithEvents dtpEndDatePicker As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpStartDatePicker As System.Windows.Forms.DateTimePicker
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnplot As System.Windows.Forms.Button
    Friend WithEvents gboxStatisticOptions As System.Windows.Forms.GroupBox
    Friend WithEvents ckboxUseCensoredData As System.Windows.Forms.CheckBox


End Class
