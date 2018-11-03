﻿'Namespace EditView
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class cEditView
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.pTimeSeriesPlot = New cTimeSeriesPlot()
        Me.panel1 = New System.Windows.Forms.Panel()
        Me.dgvDataValues = New System.Windows.Forms.DataGridView()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlFilter = New System.Windows.Forms.Panel()
        Me.gboxDataFilter = New System.Windows.Forms.GroupBox()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.btnClearFilter = New System.Windows.Forms.Button()
        Me.btnApplyFilter = New System.Windows.Forms.Button()
        Me.txtEditDFVTChange = New System.Windows.Forms.TextBox()
        Me.rbtnEditDFVTChange = New System.Windows.Forms.RadioButton()
        Me.rbtnDate = New System.Windows.Forms.RadioButton()
        Me.rbtnValueThreshold = New System.Windows.Forms.RadioButton()
        Me.rbtnDataGap = New System.Windows.Forms.RadioButton()
        Me.gboxDate = New System.Windows.Forms.GroupBox()
        Me.lblAfter = New System.Windows.Forms.Label()
        Me.lblDateBefore = New System.Windows.Forms.Label()
        Me.dtpAfter = New System.Windows.Forms.DateTimePicker()
        Me.dtpBefore = New System.Windows.Forms.DateTimePicker()
        Me.gboxDataGap = New System.Windows.Forms.GroupBox()
        Me.ddlTimePeriod = New System.Windows.Forms.ComboBox()
        Me.lblDataGapTime = New System.Windows.Forms.Label()
        Me.txtDataGapValue = New System.Windows.Forms.TextBox()
        Me.lblDataGapValue = New System.Windows.Forms.Label()
        Me.gboxValueThreshold = New System.Windows.Forms.GroupBox()
        Me.txtValueLess = New System.Windows.Forms.TextBox()
        Me.txtValueLarger = New System.Windows.Forms.TextBox()
        Me.lblValueLess = New System.Windows.Forms.Label()
        Me.lblValueLarger = New System.Windows.Forms.Label()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.pbProgressBar = New System.Windows.Forms.ProgressBar()
        Me.lblstatus = New System.Windows.Forms.Label()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.panel1.SuspendLayout()
        CType(Me.dgvDataValues, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.pnlFilter.SuspendLayout()
        Me.gboxDataFilter.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.gboxDate.SuspendLayout()
        Me.gboxDataGap.SuspendLayout()
        Me.gboxValueThreshold.SuspendLayout()
        Me.TableLayoutPanel5.SuspendLayout()
        Me.TableLayoutPanel4.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 2
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 237.0!))
        Me.TableLayoutPanel3.Controls.Add(Me.TableLayoutPanel1, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.Panel2, 1, 0)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 1
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(898, 443)
        Me.TableLayoutPanel3.TabIndex = 3
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.pTimeSeriesPlot, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.panel1, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 64.2!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35.8!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(655, 437)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'pTimeSeriesPlot
        '
        Me.pTimeSeriesPlot.AutoSize = True
        Me.pTimeSeriesPlot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pTimeSeriesPlot.Location = New System.Drawing.Point(3, 3)
        Me.pTimeSeriesPlot.Name = "pTimeSeriesPlot"
        Me.pTimeSeriesPlot.Size = New System.Drawing.Size(649, 274)
        Me.pTimeSeriesPlot.TabIndex = 0
        '
        'panel1
        '
        Me.panel1.Controls.Add(Me.dgvDataValues)
        Me.panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.panel1.Location = New System.Drawing.Point(3, 283)
        Me.panel1.Name = "panel1"
        Me.panel1.Size = New System.Drawing.Size(649, 151)
        Me.panel1.TabIndex = 5
        '
        'dgvDataValues
        '
        Me.dgvDataValues.AllowDrop = True
        Me.dgvDataValues.AllowUserToAddRows = False
        Me.dgvDataValues.AllowUserToDeleteRows = False
        Me.dgvDataValues.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.MistyRose
        Me.dgvDataValues.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.dgvDataValues.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.dgvDataValues.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvDataValues.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
        Me.dgvDataValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Arial", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgvDataValues.DefaultCellStyle = DataGridViewCellStyle2
        Me.dgvDataValues.EnableHeadersVisualStyles = False
        Me.dgvDataValues.Location = New System.Drawing.Point(-3, 6)
        Me.dgvDataValues.Name = "dgvDataValues"
        Me.dgvDataValues.RowHeadersVisible = False
        Me.dgvDataValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvDataValues.Size = New System.Drawing.Size(649, 151)
        Me.dgvDataValues.TabIndex = 4
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.TableLayoutPanel2)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(664, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(231, 437)
        Me.Panel2.TabIndex = 0
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.AutoScroll = True
        Me.TableLayoutPanel2.ColumnCount = 1
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.pnlFilter, 0, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 437.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(231, 437)
        Me.TableLayoutPanel2.TabIndex = 1
        '
        'pnlFilter
        '
        Me.pnlFilter.AutoScroll = True
        Me.pnlFilter.Controls.Add(Me.gboxDataFilter)
        Me.pnlFilter.Location = New System.Drawing.Point(3, 3)
        Me.pnlFilter.Name = "pnlFilter"
        Me.pnlFilter.Size = New System.Drawing.Size(225, 338)
        Me.pnlFilter.TabIndex = 6
        '
        'gboxDataFilter
        '
        Me.gboxDataFilter.Controls.Add(Me.Panel4)
        Me.gboxDataFilter.Controls.Add(Me.txtEditDFVTChange)
        Me.gboxDataFilter.Controls.Add(Me.rbtnEditDFVTChange)
        Me.gboxDataFilter.Controls.Add(Me.rbtnDate)
        Me.gboxDataFilter.Controls.Add(Me.rbtnValueThreshold)
        Me.gboxDataFilter.Controls.Add(Me.rbtnDataGap)
        Me.gboxDataFilter.Controls.Add(Me.gboxDate)
        Me.gboxDataFilter.Controls.Add(Me.gboxDataGap)
        Me.gboxDataFilter.Controls.Add(Me.gboxValueThreshold)
        Me.gboxDataFilter.Cursor = System.Windows.Forms.Cursors.Default
        Me.gboxDataFilter.Location = New System.Drawing.Point(0, 0)
        Me.gboxDataFilter.Name = "gboxDataFilter"
        Me.gboxDataFilter.Size = New System.Drawing.Size(209, 333)
        Me.gboxDataFilter.TabIndex = 1
        Me.gboxDataFilter.TabStop = False
        Me.gboxDataFilter.Text = "Data Filters"
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.btnClearFilter)
        Me.Panel4.Controls.Add(Me.btnApplyFilter)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel4.Location = New System.Drawing.Point(3, 300)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(203, 30)
        Me.Panel4.TabIndex = 7
        '
        'btnClearFilter
        '
        Me.btnClearFilter.Location = New System.Drawing.Point(3, 4)
        Me.btnClearFilter.Name = "btnClearFilter"
        Me.btnClearFilter.Size = New System.Drawing.Size(99, 23)
        Me.btnClearFilter.TabIndex = 6
        Me.btnClearFilter.Text = "Clear Filter"
        Me.btnClearFilter.UseVisualStyleBackColor = True
        '
        'btnApplyFilter
        '
        Me.btnApplyFilter.Location = New System.Drawing.Point(101, 4)
        Me.btnApplyFilter.Name = "btnApplyFilter"
        Me.btnApplyFilter.Size = New System.Drawing.Size(99, 23)
        Me.btnApplyFilter.TabIndex = 5
        Me.btnApplyFilter.Text = "Apply Filter"
        Me.btnApplyFilter.UseVisualStyleBackColor = True
        '
        'txtEditDFVTChange
        '
        Me.txtEditDFVTChange.Location = New System.Drawing.Point(163, 274)
        Me.txtEditDFVTChange.Name = "txtEditDFVTChange"
        Me.txtEditDFVTChange.Size = New System.Drawing.Size(40, 20)
        Me.txtEditDFVTChange.TabIndex = 4
        '
        'rbtnEditDFVTChange
        '
        Me.rbtnEditDFVTChange.ForeColor = System.Drawing.SystemColors.ControlText
        Me.rbtnEditDFVTChange.Location = New System.Drawing.Point(9, 274)
        Me.rbtnEditDFVTChange.Name = "rbtnEditDFVTChange"
        Me.rbtnEditDFVTChange.Size = New System.Drawing.Size(168, 20)
        Me.rbtnEditDFVTChange.TabIndex = 3
        Me.rbtnEditDFVTChange.TabStop = True
        Me.rbtnEditDFVTChange.Text = "Value Change Threshold >= "
        Me.rbtnEditDFVTChange.UseVisualStyleBackColor = True
        '
        'rbtnDate
        '
        Me.rbtnDate.AutoSize = True
        Me.rbtnDate.Location = New System.Drawing.Point(9, 168)
        Me.rbtnDate.Name = "rbtnDate"
        Me.rbtnDate.Size = New System.Drawing.Size(14, 13)
        Me.rbtnDate.TabIndex = 3
        Me.rbtnDate.TabStop = True
        Me.rbtnDate.UseVisualStyleBackColor = True
        '
        'rbtnValueThreshold
        '
        Me.rbtnValueThreshold.AutoSize = True
        Me.rbtnValueThreshold.Location = New System.Drawing.Point(9, 16)
        Me.rbtnValueThreshold.Name = "rbtnValueThreshold"
        Me.rbtnValueThreshold.Size = New System.Drawing.Size(14, 13)
        Me.rbtnValueThreshold.TabIndex = 1
        Me.rbtnValueThreshold.TabStop = True
        Me.rbtnValueThreshold.UseVisualStyleBackColor = True
        '
        'rbtnDataGap
        '
        Me.rbtnDataGap.AutoSize = True
        Me.rbtnDataGap.Location = New System.Drawing.Point(9, 90)
        Me.rbtnDataGap.Name = "rbtnDataGap"
        Me.rbtnDataGap.Size = New System.Drawing.Size(14, 13)
        Me.rbtnDataGap.TabIndex = 2
        Me.rbtnDataGap.TabStop = True
        Me.rbtnDataGap.UseVisualStyleBackColor = True
        '
        'gboxDate
        '
        Me.gboxDate.Controls.Add(Me.lblAfter)
        Me.gboxDate.Controls.Add(Me.lblDateBefore)
        Me.gboxDate.Controls.Add(Me.dtpAfter)
        Me.gboxDate.Controls.Add(Me.dtpBefore)
        Me.gboxDate.Dock = System.Windows.Forms.DockStyle.Top
        Me.gboxDate.Location = New System.Drawing.Point(3, 168)
        Me.gboxDate.Name = "gboxDate"
        Me.gboxDate.Size = New System.Drawing.Size(203, 100)
        Me.gboxDate.TabIndex = 2
        Me.gboxDate.TabStop = False
        Me.gboxDate.Text = "    Date"
        '
        'lblAfter
        '
        Me.lblAfter.AutoSize = True
        Me.lblAfter.Location = New System.Drawing.Point(4, 58)
        Me.lblAfter.Name = "lblAfter"
        Me.lblAfter.Size = New System.Drawing.Size(32, 13)
        Me.lblAfter.TabIndex = 7
        Me.lblAfter.Text = "After:"
        '
        'lblDateBefore
        '
        Me.lblDateBefore.AutoSize = True
        Me.lblDateBefore.Location = New System.Drawing.Point(4, 20)
        Me.lblDateBefore.Name = "lblDateBefore"
        Me.lblDateBefore.Size = New System.Drawing.Size(41, 13)
        Me.lblDateBefore.TabIndex = 6
        Me.lblDateBefore.Text = "Before:"
        '
        'dtpAfter
        '
        Me.dtpAfter.CustomFormat = "MM/dd/yyyy hh:mm:ss"
        Me.dtpAfter.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpAfter.Location = New System.Drawing.Point(3, 74)
        Me.dtpAfter.Name = "dtpAfter"
        Me.dtpAfter.Size = New System.Drawing.Size(197, 20)
        Me.dtpAfter.TabIndex = 5
        '
        'dtpBefore
        '
        Me.dtpBefore.CustomFormat = "MM/dd/yyyy hh:mm:ss"
        Me.dtpBefore.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpBefore.Location = New System.Drawing.Point(3, 34)
        Me.dtpBefore.Name = "dtpBefore"
        Me.dtpBefore.Size = New System.Drawing.Size(197, 20)
        Me.dtpBefore.TabIndex = 4
        '
        'gboxDataGap
        '
        Me.gboxDataGap.Controls.Add(Me.ddlTimePeriod)
        Me.gboxDataGap.Controls.Add(Me.lblDataGapTime)
        Me.gboxDataGap.Controls.Add(Me.txtDataGapValue)
        Me.gboxDataGap.Controls.Add(Me.lblDataGapValue)
        Me.gboxDataGap.Dock = System.Windows.Forms.DockStyle.Top
        Me.gboxDataGap.Location = New System.Drawing.Point(3, 90)
        Me.gboxDataGap.Name = "gboxDataGap"
        Me.gboxDataGap.Size = New System.Drawing.Size(203, 78)
        Me.gboxDataGap.TabIndex = 1
        Me.gboxDataGap.TabStop = False
        Me.gboxDataGap.Text = "    Data Gaps"
        '
        'ddlTimePeriod
        '
        Me.ddlTimePeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlTimePeriod.FormattingEnabled = True
        Me.ddlTimePeriod.Items.AddRange(New Object() {"Second", "Minute", "Hour", "Day"})
        Me.ddlTimePeriod.Location = New System.Drawing.Point(76, 47)
        Me.ddlTimePeriod.Name = "ddlTimePeriod"
        Me.ddlTimePeriod.Size = New System.Drawing.Size(106, 21)
        Me.ddlTimePeriod.TabIndex = 6
        '
        'lblDataGapTime
        '
        Me.lblDataGapTime.AutoSize = True
        Me.lblDataGapTime.Location = New System.Drawing.Point(6, 50)
        Me.lblDataGapTime.Name = "lblDataGapTime"
        Me.lblDataGapTime.Size = New System.Drawing.Size(69, 13)
        Me.lblDataGapTime.TabIndex = 5
        Me.lblDataGapTime.Text = "Time Period: "
        '
        'txtDataGapValue
        '
        Me.txtDataGapValue.Location = New System.Drawing.Point(56, 17)
        Me.txtDataGapValue.Name = "txtDataGapValue"
        Me.txtDataGapValue.Size = New System.Drawing.Size(126, 20)
        Me.txtDataGapValue.TabIndex = 4
        '
        'lblDataGapValue
        '
        Me.lblDataGapValue.AutoSize = True
        Me.lblDataGapValue.Location = New System.Drawing.Point(6, 20)
        Me.lblDataGapValue.Name = "lblDataGapValue"
        Me.lblDataGapValue.Size = New System.Drawing.Size(37, 13)
        Me.lblDataGapValue.TabIndex = 3
        Me.lblDataGapValue.Text = "Value:"
        '
        'gboxValueThreshold
        '
        Me.gboxValueThreshold.Controls.Add(Me.txtValueLess)
        Me.gboxValueThreshold.Controls.Add(Me.txtValueLarger)
        Me.gboxValueThreshold.Controls.Add(Me.lblValueLess)
        Me.gboxValueThreshold.Controls.Add(Me.lblValueLarger)
        Me.gboxValueThreshold.Dock = System.Windows.Forms.DockStyle.Top
        Me.gboxValueThreshold.Location = New System.Drawing.Point(3, 16)
        Me.gboxValueThreshold.Name = "gboxValueThreshold"
        Me.gboxValueThreshold.Size = New System.Drawing.Size(203, 74)
        Me.gboxValueThreshold.TabIndex = 0
        Me.gboxValueThreshold.TabStop = False
        Me.gboxValueThreshold.Text = "    Value Threshold"
        '
        'txtValueLess
        '
        Me.txtValueLess.Location = New System.Drawing.Point(56, 46)
        Me.txtValueLess.Name = "txtValueLess"
        Me.txtValueLess.Size = New System.Drawing.Size(126, 20)
        Me.txtValueLess.TabIndex = 5
        '
        'txtValueLarger
        '
        Me.txtValueLarger.Location = New System.Drawing.Point(56, 21)
        Me.txtValueLarger.Name = "txtValueLarger"
        Me.txtValueLarger.Size = New System.Drawing.Size(126, 20)
        Me.txtValueLarger.TabIndex = 4
        '
        'lblValueLess
        '
        Me.lblValueLess.AutoSize = True
        Me.lblValueLess.Location = New System.Drawing.Point(6, 49)
        Me.lblValueLess.Name = "lblValueLess"
        Me.lblValueLess.Size = New System.Drawing.Size(43, 13)
        Me.lblValueLess.TabIndex = 3
        Me.lblValueLess.Text = "Value <"
        '
        'lblValueLarger
        '
        Me.lblValueLarger.AutoSize = True
        Me.lblValueLarger.Location = New System.Drawing.Point(6, 24)
        Me.lblValueLarger.Name = "lblValueLarger"
        Me.lblValueLarger.Size = New System.Drawing.Size(43, 13)
        Me.lblValueLarger.TabIndex = 2
        Me.lblValueLarger.Text = "Value >"
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.ColumnCount = 2
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel5.Controls.Add(Me.pbProgressBar, 1, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.lblstatus, 0, 0)
        Me.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(3, 452)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 1
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(898, 20)
        Me.TableLayoutPanel5.TabIndex = 4
        '
        'pbProgressBar
        '
        Me.pbProgressBar.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbProgressBar.Location = New System.Drawing.Point(452, 3)
        Me.pbProgressBar.Name = "pbProgressBar"
        Me.pbProgressBar.Size = New System.Drawing.Size(443, 14)
        Me.pbProgressBar.TabIndex = 5
        '
        'lblstatus
        '
        Me.lblstatus.AutoSize = True
        Me.lblstatus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblstatus.Location = New System.Drawing.Point(3, 0)
        Me.lblstatus.Name = "lblstatus"
        Me.lblstatus.Size = New System.Drawing.Size(443, 20)
        Me.lblstatus.TabIndex = 0
        '
        'TableLayoutPanel4
        '
        Me.TableLayoutPanel4.ColumnCount = 1
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel4.Controls.Add(Me.TableLayoutPanel5, 0, 1)
        Me.TableLayoutPanel4.Controls.Add(Me.TableLayoutPanel3, 0, 0)
        Me.TableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 2
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(904, 475)
        Me.TableLayoutPanel4.TabIndex = 2
        '
        'cEditView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(191, Byte), Integer), CType(CType(219, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.Controls.Add(Me.TableLayoutPanel4)
        Me.Name = "cEditView"
        Me.Size = New System.Drawing.Size(904, 475)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.panel1.ResumeLayout(False)
        CType(Me.dgvDataValues, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.pnlFilter.ResumeLayout(False)
        Me.gboxDataFilter.ResumeLayout(False)
        Me.gboxDataFilter.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.gboxDate.ResumeLayout(False)
        Me.gboxDate.PerformLayout()
        Me.gboxDataGap.ResumeLayout(False)
        Me.gboxDataGap.PerformLayout()
        Me.gboxValueThreshold.ResumeLayout(False)
        Me.gboxValueThreshold.PerformLayout()
        Me.TableLayoutPanel5.ResumeLayout(False)
        Me.TableLayoutPanel5.PerformLayout()
        Me.TableLayoutPanel4.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pTimeSeriesPlot As cTimeSeriesPlot
    Friend WithEvents panel1 As System.Windows.Forms.Panel
    Friend WithEvents dgvDataValues As System.Windows.Forms.DataGridView
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pnlFilter As System.Windows.Forms.Panel
    Friend WithEvents gboxDataFilter As System.Windows.Forms.GroupBox
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents btnClearFilter As System.Windows.Forms.Button
    Friend WithEvents btnApplyFilter As System.Windows.Forms.Button
    Friend WithEvents txtEditDFVTChange As System.Windows.Forms.TextBox
    Friend WithEvents rbtnEditDFVTChange As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnDate As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnValueThreshold As System.Windows.Forms.RadioButton
    Friend WithEvents rbtnDataGap As System.Windows.Forms.RadioButton
    Friend WithEvents gboxDate As System.Windows.Forms.GroupBox
    Friend WithEvents lblAfter As System.Windows.Forms.Label
    Friend WithEvents lblDateBefore As System.Windows.Forms.Label
    Friend WithEvents dtpAfter As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpBefore As System.Windows.Forms.DateTimePicker
    Friend WithEvents gboxDataGap As System.Windows.Forms.GroupBox
    Friend WithEvents ddlTimePeriod As System.Windows.Forms.ComboBox
    Friend WithEvents lblDataGapTime As System.Windows.Forms.Label
    Friend WithEvents txtDataGapValue As System.Windows.Forms.TextBox
    Friend WithEvents lblDataGapValue As System.Windows.Forms.Label
    Friend WithEvents gboxValueThreshold As System.Windows.Forms.GroupBox
    Friend WithEvents txtValueLess As System.Windows.Forms.TextBox
    Friend WithEvents txtValueLarger As System.Windows.Forms.TextBox
    Friend WithEvents lblValueLess As System.Windows.Forms.Label
    Friend WithEvents lblValueLarger As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel5 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pbProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents lblstatus As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel4 As System.Windows.Forms.TableLayoutPanel

End Class
'End Namespace
