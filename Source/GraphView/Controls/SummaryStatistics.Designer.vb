Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class SummaryStatistics
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
            Me.gboxStatistics = New System.Windows.Forms.GroupBox
            Me.lblPercentiles = New System.Windows.Forms.Label
            Me.tbox90Perc = New System.Windows.Forms.TextBox
            Me.tbox75Perc = New System.Windows.Forms.TextBox
            Me.tbox50Perc = New System.Windows.Forms.TextBox
            Me.lbl90Perc = New System.Windows.Forms.Label
            Me.lbl75Perc = New System.Windows.Forms.Label
            Me.tbox10Perc = New System.Windows.Forms.TextBox
            Me.tbox25Perc = New System.Windows.Forms.TextBox
            Me.lbl50Perc = New System.Windows.Forms.Label
            Me.lbl25Perc = New System.Windows.Forms.Label
            Me.lbl10Perc = New System.Windows.Forms.Label
            Me.lblStdDev = New System.Windows.Forms.Label
            Me.lblMax = New System.Windows.Forms.Label
            Me.lblCoeffVar = New System.Windows.Forms.Label
            Me.tboxGeoMean = New System.Windows.Forms.TextBox
            Me.lblAMean = New System.Windows.Forms.Label
            Me.tboxAMean = New System.Windows.Forms.TextBox
            Me.lblGeoMean = New System.Windows.Forms.Label
            Me.tboxCoeffVar = New System.Windows.Forms.TextBox
            Me.tboxMin = New System.Windows.Forms.TextBox
            Me.gboxDivider = New System.Windows.Forms.GroupBox
            Me.tboxMax = New System.Windows.Forms.TextBox
            Me.tboxStdDev = New System.Windows.Forms.TextBox
            Me.lblMin = New System.Windows.Forms.Label
            Me.lblNumCensoredObs = New System.Windows.Forms.Label
            Me.lblNumObs = New System.Windows.Forms.Label
            Me.tboxNumObs = New System.Windows.Forms.TextBox
            Me.tboxNumCensoredObs = New System.Windows.Forms.TextBox
            Me.gboxStatistics.SuspendLayout()
            Me.SuspendLayout()
            '
            'gboxStatistics
            '
            Me.gboxStatistics.Controls.Add(Me.lblPercentiles)
            Me.gboxStatistics.Controls.Add(Me.tbox90Perc)
            Me.gboxStatistics.Controls.Add(Me.tbox75Perc)
            Me.gboxStatistics.Controls.Add(Me.tbox50Perc)
            Me.gboxStatistics.Controls.Add(Me.lbl90Perc)
            Me.gboxStatistics.Controls.Add(Me.lbl75Perc)
            Me.gboxStatistics.Controls.Add(Me.tbox10Perc)
            Me.gboxStatistics.Controls.Add(Me.tbox25Perc)
            Me.gboxStatistics.Controls.Add(Me.lbl50Perc)
            Me.gboxStatistics.Controls.Add(Me.lbl25Perc)
            Me.gboxStatistics.Controls.Add(Me.lbl10Perc)
            Me.gboxStatistics.Controls.Add(Me.lblStdDev)
            Me.gboxStatistics.Controls.Add(Me.lblMax)
            Me.gboxStatistics.Controls.Add(Me.lblCoeffVar)
            Me.gboxStatistics.Controls.Add(Me.tboxGeoMean)
            Me.gboxStatistics.Controls.Add(Me.lblAMean)
            Me.gboxStatistics.Controls.Add(Me.tboxAMean)
            Me.gboxStatistics.Controls.Add(Me.lblGeoMean)
            Me.gboxStatistics.Controls.Add(Me.tboxCoeffVar)
            Me.gboxStatistics.Controls.Add(Me.tboxMin)
            Me.gboxStatistics.Controls.Add(Me.gboxDivider)
            Me.gboxStatistics.Controls.Add(Me.tboxMax)
            Me.gboxStatistics.Controls.Add(Me.tboxStdDev)
            Me.gboxStatistics.Controls.Add(Me.lblMin)
            Me.gboxStatistics.Controls.Add(Me.lblNumCensoredObs)
            Me.gboxStatistics.Controls.Add(Me.lblNumObs)
            Me.gboxStatistics.Controls.Add(Me.tboxNumObs)
            Me.gboxStatistics.Controls.Add(Me.tboxNumCensoredObs)
            Me.gboxStatistics.Dock = System.Windows.Forms.DockStyle.Top
            Me.gboxStatistics.Location = New System.Drawing.Point(3, 3)
            Me.gboxStatistics.Name = "gboxStatistics"
            Me.gboxStatistics.Size = New System.Drawing.Size(194, 342)
            Me.gboxStatistics.TabIndex = 38
            Me.gboxStatistics.TabStop = False
            Me.gboxStatistics.Text = "Statistics"
            '
            'lblPercentiles
            '
            Me.lblPercentiles.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.lblPercentiles.Location = New System.Drawing.Point(8, 216)
            Me.lblPercentiles.Name = "lblPercentiles"
            Me.lblPercentiles.Size = New System.Drawing.Size(72, 16)
            Me.lblPercentiles.TabIndex = 60
            Me.lblPercentiles.Text = "Percentiles"
            Me.lblPercentiles.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            '
            'tbox90Perc
            '
            Me.tbox90Perc.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                          Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tbox90Perc.BackColor = System.Drawing.SystemColors.Window
            Me.tbox90Perc.Location = New System.Drawing.Point(124, 312)
            Me.tbox90Perc.Name = "tbox90Perc"
            Me.tbox90Perc.ReadOnly = True
            Me.tbox90Perc.Size = New System.Drawing.Size(64, 20)
            Me.tbox90Perc.TabIndex = 53
            Me.tbox90Perc.Text = "0"
            Me.tbox90Perc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'tbox75Perc
            '
            Me.tbox75Perc.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                          Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tbox75Perc.BackColor = System.Drawing.SystemColors.Window
            Me.tbox75Perc.Location = New System.Drawing.Point(124, 288)
            Me.tbox75Perc.Name = "tbox75Perc"
            Me.tbox75Perc.ReadOnly = True
            Me.tbox75Perc.Size = New System.Drawing.Size(64, 20)
            Me.tbox75Perc.TabIndex = 52
            Me.tbox75Perc.Text = "0"
            Me.tbox75Perc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'tbox50Perc
            '
            Me.tbox50Perc.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                          Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tbox50Perc.BackColor = System.Drawing.SystemColors.Window
            Me.tbox50Perc.Location = New System.Drawing.Point(124, 264)
            Me.tbox50Perc.Name = "tbox50Perc"
            Me.tbox50Perc.ReadOnly = True
            Me.tbox50Perc.Size = New System.Drawing.Size(64, 20)
            Me.tbox50Perc.TabIndex = 51
            Me.tbox50Perc.Text = "0"
            Me.tbox50Perc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'lbl90Perc
            '
            Me.lbl90Perc.Location = New System.Drawing.Point(96, 314)
            Me.lbl90Perc.Name = "lbl90Perc"
            Me.lbl90Perc.Size = New System.Drawing.Size(32, 16)
            Me.lbl90Perc.TabIndex = 59
            Me.lbl90Perc.Text = "90%"
            '
            'lbl75Perc
            '
            Me.lbl75Perc.Location = New System.Drawing.Point(96, 290)
            Me.lbl75Perc.Name = "lbl75Perc"
            Me.lbl75Perc.Size = New System.Drawing.Size(32, 16)
            Me.lbl75Perc.TabIndex = 58
            Me.lbl75Perc.Text = "75%"
            '
            'tbox10Perc
            '
            Me.tbox10Perc.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                          Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tbox10Perc.BackColor = System.Drawing.SystemColors.Window
            Me.tbox10Perc.Location = New System.Drawing.Point(124, 216)
            Me.tbox10Perc.Name = "tbox10Perc"
            Me.tbox10Perc.ReadOnly = True
            Me.tbox10Perc.Size = New System.Drawing.Size(64, 20)
            Me.tbox10Perc.TabIndex = 49
            Me.tbox10Perc.Text = "0"
            Me.tbox10Perc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'tbox25Perc
            '
            Me.tbox25Perc.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                          Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tbox25Perc.BackColor = System.Drawing.SystemColors.Window
            Me.tbox25Perc.Location = New System.Drawing.Point(124, 240)
            Me.tbox25Perc.Name = "tbox25Perc"
            Me.tbox25Perc.ReadOnly = True
            Me.tbox25Perc.Size = New System.Drawing.Size(64, 20)
            Me.tbox25Perc.TabIndex = 50
            Me.tbox25Perc.Text = "0"
            Me.tbox25Perc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'lbl50Perc
            '
            Me.lbl50Perc.Location = New System.Drawing.Point(50, 266)
            Me.lbl50Perc.Name = "lbl50Perc"
            Me.lbl50Perc.Size = New System.Drawing.Size(78, 16)
            Me.lbl50Perc.TabIndex = 57
            Me.lbl50Perc.Text = "(Median)  50%"
            '
            'lbl25Perc
            '
            Me.lbl25Perc.Location = New System.Drawing.Point(96, 242)
            Me.lbl25Perc.Name = "lbl25Perc"
            Me.lbl25Perc.Size = New System.Drawing.Size(32, 16)
            Me.lbl25Perc.TabIndex = 56
            Me.lbl25Perc.Text = "25%"
            '
            'lbl10Perc
            '
            Me.lbl10Perc.Location = New System.Drawing.Point(96, 218)
            Me.lbl10Perc.Name = "lbl10Perc"
            Me.lbl10Perc.Size = New System.Drawing.Size(32, 16)
            Me.lbl10Perc.TabIndex = 55
            Me.lbl10Perc.Text = "10%"
            '
            'lblStdDev
            '
            Me.lblStdDev.Location = New System.Drawing.Point(4, 162)
            Me.lblStdDev.Name = "lblStdDev"
            Me.lblStdDev.Size = New System.Drawing.Size(120, 16)
            Me.lblStdDev.TabIndex = 45
            Me.lblStdDev.Text = "Standard Deviation :"
            Me.lblStdDev.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'lblMax
            '
            Me.lblMax.Location = New System.Drawing.Point(4, 114)
            Me.lblMax.Name = "lblMax"
            Me.lblMax.Size = New System.Drawing.Size(120, 16)
            Me.lblMax.TabIndex = 43
            Me.lblMax.Text = "Maximum :"
            Me.lblMax.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'lblCoeffVar
            '
            Me.lblCoeffVar.Location = New System.Drawing.Point(2, 186)
            Me.lblCoeffVar.Name = "lblCoeffVar"
            Me.lblCoeffVar.Size = New System.Drawing.Size(122, 16)
            Me.lblCoeffVar.TabIndex = 46
            Me.lblCoeffVar.Text = "Coefficient of Variation:"
            Me.lblCoeffVar.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'tboxGeoMean
            '
            Me.tboxGeoMean.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                           Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tboxGeoMean.BackColor = System.Drawing.SystemColors.Window
            Me.tboxGeoMean.Location = New System.Drawing.Point(124, 88)
            Me.tboxGeoMean.Name = "tboxGeoMean"
            Me.tboxGeoMean.ReadOnly = True
            Me.tboxGeoMean.Size = New System.Drawing.Size(64, 20)
            Me.tboxGeoMean.TabIndex = 37
            Me.tboxGeoMean.Text = "0"
            Me.tboxGeoMean.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'lblAMean
            '
            Me.lblAMean.Location = New System.Drawing.Point(4, 66)
            Me.lblAMean.Name = "lblAMean"
            Me.lblAMean.Size = New System.Drawing.Size(120, 16)
            Me.lblAMean.TabIndex = 42
            Me.lblAMean.Text = "Arithmetic Mean :"
            Me.lblAMean.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'tboxAMean
            '
            Me.tboxAMean.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                         Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tboxAMean.BackColor = System.Drawing.SystemColors.Window
            Me.tboxAMean.Location = New System.Drawing.Point(124, 64)
            Me.tboxAMean.Name = "tboxAMean"
            Me.tboxAMean.ReadOnly = True
            Me.tboxAMean.Size = New System.Drawing.Size(64, 20)
            Me.tboxAMean.TabIndex = 36
            Me.tboxAMean.Text = "0"
            Me.tboxAMean.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'lblGeoMean
            '
            Me.lblGeoMean.Location = New System.Drawing.Point(4, 90)
            Me.lblGeoMean.Name = "lblGeoMean"
            Me.lblGeoMean.Size = New System.Drawing.Size(120, 16)
            Me.lblGeoMean.TabIndex = 48
            Me.lblGeoMean.Text = "Geometric Mean :"
            Me.lblGeoMean.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'tboxCoeffVar
            '
            Me.tboxCoeffVar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tboxCoeffVar.BackColor = System.Drawing.SystemColors.Window
            Me.tboxCoeffVar.Location = New System.Drawing.Point(124, 184)
            Me.tboxCoeffVar.Name = "tboxCoeffVar"
            Me.tboxCoeffVar.ReadOnly = True
            Me.tboxCoeffVar.Size = New System.Drawing.Size(64, 20)
            Me.tboxCoeffVar.TabIndex = 41
            Me.tboxCoeffVar.Text = "0"
            Me.tboxCoeffVar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'tboxMin
            '
            Me.tboxMin.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                       Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tboxMin.BackColor = System.Drawing.SystemColors.Window
            Me.tboxMin.Location = New System.Drawing.Point(124, 136)
            Me.tboxMin.Name = "tboxMin"
            Me.tboxMin.ReadOnly = True
            Me.tboxMin.Size = New System.Drawing.Size(64, 20)
            Me.tboxMin.TabIndex = 39
            Me.tboxMin.Text = "0"
            Me.tboxMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'gboxDivider
            '
            Me.gboxDivider.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                           Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.gboxDivider.Location = New System.Drawing.Point(7, 204)
            Me.gboxDivider.Name = "gboxDivider"
            Me.gboxDivider.Size = New System.Drawing.Size(181, 8)
            Me.gboxDivider.TabIndex = 47
            Me.gboxDivider.TabStop = False
            '
            'tboxMax
            '
            Me.tboxMax.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                       Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tboxMax.BackColor = System.Drawing.SystemColors.Window
            Me.tboxMax.Location = New System.Drawing.Point(124, 112)
            Me.tboxMax.Name = "tboxMax"
            Me.tboxMax.ReadOnly = True
            Me.tboxMax.Size = New System.Drawing.Size(64, 20)
            Me.tboxMax.TabIndex = 38
            Me.tboxMax.Text = "0"
            Me.tboxMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'tboxStdDev
            '
            Me.tboxStdDev.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                          Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tboxStdDev.BackColor = System.Drawing.SystemColors.Window
            Me.tboxStdDev.Location = New System.Drawing.Point(124, 160)
            Me.tboxStdDev.Name = "tboxStdDev"
            Me.tboxStdDev.ReadOnly = True
            Me.tboxStdDev.Size = New System.Drawing.Size(64, 20)
            Me.tboxStdDev.TabIndex = 40
            Me.tboxStdDev.Text = "0"
            Me.tboxStdDev.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'lblMin
            '
            Me.lblMin.Location = New System.Drawing.Point(4, 138)
            Me.lblMin.Name = "lblMin"
            Me.lblMin.Size = New System.Drawing.Size(120, 16)
            Me.lblMin.TabIndex = 44
            Me.lblMin.Text = "Minimum :"
            Me.lblMin.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'lblNumCensoredObs
            '
            Me.lblNumCensoredObs.Location = New System.Drawing.Point(4, 42)
            Me.lblNumCensoredObs.Name = "lblNumCensoredObs"
            Me.lblNumCensoredObs.Size = New System.Drawing.Size(120, 16)
            Me.lblNumCensoredObs.TabIndex = 35
            Me.lblNumCensoredObs.Text = "# Of Censored Obs. :"
            Me.lblNumCensoredObs.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'lblNumObs
            '
            Me.lblNumObs.Location = New System.Drawing.Point(4, 18)
            Me.lblNumObs.Name = "lblNumObs"
            Me.lblNumObs.Size = New System.Drawing.Size(120, 16)
            Me.lblNumObs.TabIndex = 33
            Me.lblNumObs.Text = "# Of Observations :"
            Me.lblNumObs.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            '
            'tboxNumObs
            '
            Me.tboxNumObs.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                          Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tboxNumObs.BackColor = System.Drawing.SystemColors.Window
            Me.tboxNumObs.Location = New System.Drawing.Point(124, 16)
            Me.tboxNumObs.Name = "tboxNumObs"
            Me.tboxNumObs.ReadOnly = True
            Me.tboxNumObs.Size = New System.Drawing.Size(64, 20)
            Me.tboxNumObs.TabIndex = 32
            Me.tboxNumObs.Text = "0"
            Me.tboxNumObs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'tboxNumCensoredObs
            '
            Me.tboxNumCensoredObs.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                                                  Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.tboxNumCensoredObs.BackColor = System.Drawing.SystemColors.Window
            Me.tboxNumCensoredObs.Location = New System.Drawing.Point(124, 40)
            Me.tboxNumCensoredObs.Name = "tboxNumCensoredObs"
            Me.tboxNumCensoredObs.ReadOnly = True
            Me.tboxNumCensoredObs.Size = New System.Drawing.Size(64, 20)
            Me.tboxNumCensoredObs.TabIndex = 34
            Me.tboxNumCensoredObs.Text = "0"
            Me.tboxNumCensoredObs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'cSummaryStatistics
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.AutoScroll = True
            Me.Controls.Add(Me.gboxStatistics)
            Me.Name = "cSummaryStatistics"
            Me.Padding = New System.Windows.Forms.Padding(3)
            Me.Size = New System.Drawing.Size(200, 375)
            Me.gboxStatistics.ResumeLayout(False)
            Me.gboxStatistics.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents gboxStatistics As System.Windows.Forms.GroupBox
        Friend WithEvents lblPercentiles As System.Windows.Forms.Label
        Friend WithEvents tbox90Perc As System.Windows.Forms.TextBox
        Friend WithEvents tbox75Perc As System.Windows.Forms.TextBox
        Friend WithEvents tbox50Perc As System.Windows.Forms.TextBox
        Friend WithEvents lbl90Perc As System.Windows.Forms.Label
        Friend WithEvents lbl75Perc As System.Windows.Forms.Label
        Friend WithEvents tbox10Perc As System.Windows.Forms.TextBox
        Friend WithEvents tbox25Perc As System.Windows.Forms.TextBox
        Friend WithEvents lbl50Perc As System.Windows.Forms.Label
        Friend WithEvents lbl25Perc As System.Windows.Forms.Label
        Friend WithEvents lbl10Perc As System.Windows.Forms.Label
        Friend WithEvents lblStdDev As System.Windows.Forms.Label
        Friend WithEvents lblMax As System.Windows.Forms.Label
        Friend WithEvents lblCoeffVar As System.Windows.Forms.Label
        Friend WithEvents tboxGeoMean As System.Windows.Forms.TextBox
        Friend WithEvents lblAMean As System.Windows.Forms.Label
        Friend WithEvents tboxAMean As System.Windows.Forms.TextBox
        Friend WithEvents lblGeoMean As System.Windows.Forms.Label
        Friend WithEvents tboxCoeffVar As System.Windows.Forms.TextBox
        Friend WithEvents tboxMin As System.Windows.Forms.TextBox
        Friend WithEvents gboxDivider As System.Windows.Forms.GroupBox
        Friend WithEvents tboxMax As System.Windows.Forms.TextBox
        Friend WithEvents tboxStdDev As System.Windows.Forms.TextBox
        Friend WithEvents lblMin As System.Windows.Forms.Label
        Friend WithEvents lblNumCensoredObs As System.Windows.Forms.Label
        Friend WithEvents lblNumObs As System.Windows.Forms.Label
        Friend WithEvents tboxNumObs As System.Windows.Forms.TextBox
        Friend WithEvents tboxNumCensoredObs As System.Windows.Forms.TextBox

    End Class
End Namespace