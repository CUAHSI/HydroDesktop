Imports HydroDesktop.Plugins.EditView

Namespace HydroDesktop.Plugins.Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class DeriveNewDataSeries
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
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
            Me.components = New System.ComponentModel.Container()
            Me.btnNewSeries = New System.Windows.Forms.Button()
            Me.ConfigBindingSource = New System.Windows.Forms.BindingSource(Me.components)
            Me.lblMethods = New System.Windows.Forms.Label()
            Me.ddlMethods = New System.Windows.Forms.ComboBox()
            Me.btnMethods = New System.Windows.Forms.Button()
            Me.lblComment = New System.Windows.Forms.Label()
            Me.txtComment = New System.Windows.Forms.TextBox()
            Me.gboxgeneral = New System.Windows.Forms.GroupBox()
            Me.lblQualityControlLevel = New System.Windows.Forms.Label()
            Me.ddlQualityControlLevel = New System.Windows.Forms.ComboBox()
            Me.btnQualityControlLevel = New System.Windows.Forms.Button()
            Me.lblVariable = New System.Windows.Forms.Label()
            Me.btnVariable = New System.Windows.Forms.Button()
            Me.ddlVariable = New System.Windows.Forms.ComboBox()
            Me.btnBackToOriginal = New System.Windows.Forms.Button()
            Me.gboxDeriveOption = New System.Windows.Forms.GroupBox()
            Me.rbtnAlgebraic = New System.Windows.Forms.RadioButton()
            Me.gboxAlgebraic = New System.Windows.Forms.GroupBox()
            Me.lblE = New System.Windows.Forms.Label()
            Me.txtF = New System.Windows.Forms.TextBox()
            Me.lblD = New System.Windows.Forms.Label()
            Me.txtE = New System.Windows.Forms.TextBox()
            Me.lblC = New System.Windows.Forms.Label()
            Me.txtD = New System.Windows.Forms.TextBox()
            Me.lblB = New System.Windows.Forms.Label()
            Me.txtC = New System.Windows.Forms.TextBox()
            Me.lblA = New System.Windows.Forms.Label()
            Me.txtB = New System.Windows.Forms.TextBox()
            Me.lbl1 = New System.Windows.Forms.Label()
            Me.txtA = New System.Windows.Forms.TextBox()
            Me.lblY = New System.Windows.Forms.Label()
            Me.rbtnAggregate = New System.Windows.Forms.RadioButton()
            Me.gboxAggregate = New System.Windows.Forms.GroupBox()
            Me.Panel2 = New System.Windows.Forms.Panel()
            Me.rbtnDaily = New System.Windows.Forms.RadioButton()
            Me.rbtnQuarterly = New System.Windows.Forms.RadioButton()
            Me.rbtnMonthly = New System.Windows.Forms.RadioButton()
            Me.Panel1 = New System.Windows.Forms.Panel()
            Me.rbtnSum = New System.Windows.Forms.RadioButton()
            Me.rbtnMaximum = New System.Windows.Forms.RadioButton()
            Me.rbtnAverage = New System.Windows.Forms.RadioButton()
            Me.rbtnMinimum = New System.Windows.Forms.RadioButton()
            Me.rbtnCopy = New System.Windows.Forms.RadioButton()
            Me.btnCancel = New System.Windows.Forms.Button()
            CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.gboxgeneral.SuspendLayout()
            Me.gboxDeriveOption.SuspendLayout()
            Me.gboxAlgebraic.SuspendLayout()
            Me.gboxAggregate.SuspendLayout()
            Me.Panel2.SuspendLayout()
            Me.Panel1.SuspendLayout()
            Me.SuspendLayout()
            '
            'btnNewSeries
            '
            Me.btnNewSeries.Location = New System.Drawing.Point(12, 350)
            Me.btnNewSeries.Name = "btnNewSeries"
            Me.btnNewSeries.Size = New System.Drawing.Size(340, 37)
            Me.btnNewSeries.TabIndex = 0
            Me.btnNewSeries.Text = "New Data Series"
            Me.btnNewSeries.UseVisualStyleBackColor = True
            '
            'ConfigBindingSource
            '
            Me.ConfigBindingSource.DataSource = GetType(HydroDesktop.Configuration.Settings)
            '
            'lblMethods
            '
            Me.lblMethods.AutoSize = True
            Me.lblMethods.Location = New System.Drawing.Point(6, 16)
            Me.lblMethods.Name = "lblMethods"
            Me.lblMethods.Size = New System.Drawing.Size(76, 13)
            Me.lblMethods.TabIndex = 3
            Me.lblMethods.Text = "Using Method:"
            '
            'ddlMethods
            '
            Me.ddlMethods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ddlMethods.FormattingEnabled = True
            Me.ddlMethods.Location = New System.Drawing.Point(9, 33)
            Me.ddlMethods.Name = "ddlMethods"
            Me.ddlMethods.Size = New System.Drawing.Size(133, 21)
            Me.ddlMethods.TabIndex = 4
            '
            'btnMethods
            '
            Me.btnMethods.Location = New System.Drawing.Point(9, 59)
            Me.btnMethods.Name = "btnMethods"
            Me.btnMethods.Size = New System.Drawing.Size(133, 23)
            Me.btnMethods.TabIndex = 6
            Me.btnMethods.Text = "Edit Method"
            Me.btnMethods.UseVisualStyleBackColor = True
            '
            'lblComment
            '
            Me.lblComment.AutoSize = True
            Me.lblComment.Location = New System.Drawing.Point(456, 16)
            Me.lblComment.Name = "lblComment"
            Me.lblComment.Size = New System.Drawing.Size(54, 13)
            Me.lblComment.TabIndex = 7
            Me.lblComment.Text = "Comment:"
            '
            'txtComment
            '
            Me.txtComment.Location = New System.Drawing.Point(459, 32)
            Me.txtComment.Multiline = True
            Me.txtComment.Name = "txtComment"
            Me.txtComment.Size = New System.Drawing.Size(218, 50)
            Me.txtComment.TabIndex = 8
            '
            'gboxgeneral
            '
            Me.gboxgeneral.Controls.Add(Me.lblQualityControlLevel)
            Me.gboxgeneral.Controls.Add(Me.ddlQualityControlLevel)
            Me.gboxgeneral.Controls.Add(Me.btnQualityControlLevel)
            Me.gboxgeneral.Controls.Add(Me.lblVariable)
            Me.gboxgeneral.Controls.Add(Me.btnVariable)
            Me.gboxgeneral.Controls.Add(Me.ddlVariable)
            Me.gboxgeneral.Controls.Add(Me.btnBackToOriginal)
            Me.gboxgeneral.Controls.Add(Me.txtComment)
            Me.gboxgeneral.Controls.Add(Me.lblComment)
            Me.gboxgeneral.Controls.Add(Me.lblMethods)
            Me.gboxgeneral.Controls.Add(Me.btnMethods)
            Me.gboxgeneral.Controls.Add(Me.ddlMethods)
            Me.gboxgeneral.Location = New System.Drawing.Point(12, 214)
            Me.gboxgeneral.Name = "gboxgeneral"
            Me.gboxgeneral.Size = New System.Drawing.Size(683, 130)
            Me.gboxgeneral.TabIndex = 9
            Me.gboxgeneral.TabStop = False
            Me.gboxgeneral.Text = "General"
            '
            'lblQualityControlLevel
            '
            Me.lblQualityControlLevel.AutoSize = True
            Me.lblQualityControlLevel.Location = New System.Drawing.Point(285, 16)
            Me.lblQualityControlLevel.Name = "lblQualityControlLevel"
            Me.lblQualityControlLevel.Size = New System.Drawing.Size(103, 13)
            Me.lblQualityControlLevel.TabIndex = 14
            Me.lblQualityControlLevel.Text = "Quality Control level:"
            '
            'ddlQualityControlLevel
            '
            Me.ddlQualityControlLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ddlQualityControlLevel.FormattingEnabled = True
            Me.ddlQualityControlLevel.Location = New System.Drawing.Point(288, 32)
            Me.ddlQualityControlLevel.Name = "ddlQualityControlLevel"
            Me.ddlQualityControlLevel.Size = New System.Drawing.Size(136, 21)
            Me.ddlQualityControlLevel.TabIndex = 13
            '
            'btnQualityControlLevel
            '
            Me.btnQualityControlLevel.Location = New System.Drawing.Point(288, 59)
            Me.btnQualityControlLevel.Name = "btnQualityControlLevel"
            Me.btnQualityControlLevel.Size = New System.Drawing.Size(140, 23)
            Me.btnQualityControlLevel.TabIndex = 15
            Me.btnQualityControlLevel.Text = "Edit Quality Control Level"
            Me.btnQualityControlLevel.UseVisualStyleBackColor = True
            '
            'lblVariable
            '
            Me.lblVariable.AutoSize = True
            Me.lblVariable.Location = New System.Drawing.Point(144, 16)
            Me.lblVariable.Name = "lblVariable"
            Me.lblVariable.Size = New System.Drawing.Size(48, 13)
            Me.lblVariable.TabIndex = 10
            Me.lblVariable.Text = "Variable:"
            '
            'btnVariable
            '
            Me.btnVariable.Location = New System.Drawing.Point(147, 59)
            Me.btnVariable.Name = "btnVariable"
            Me.btnVariable.Size = New System.Drawing.Size(133, 23)
            Me.btnVariable.TabIndex = 12
            Me.btnVariable.Text = "Edit Variable"
            Me.btnVariable.UseVisualStyleBackColor = True
            '
            'ddlVariable
            '
            Me.ddlVariable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ddlVariable.FormattingEnabled = True
            Me.ddlVariable.Location = New System.Drawing.Point(147, 33)
            Me.ddlVariable.Name = "ddlVariable"
            Me.ddlVariable.Size = New System.Drawing.Size(133, 21)
            Me.ddlVariable.TabIndex = 11
            '
            'btnBackToOriginal
            '
            Me.btnBackToOriginal.Location = New System.Drawing.Point(7, 101)
            Me.btnBackToOriginal.Name = "btnBackToOriginal"
            Me.btnBackToOriginal.Size = New System.Drawing.Size(138, 23)
            Me.btnBackToOriginal.TabIndex = 9
            Me.btnBackToOriginal.Text = "Change back to original"
            Me.btnBackToOriginal.UseVisualStyleBackColor = True
            '
            'gboxDeriveOption
            '
            Me.gboxDeriveOption.Controls.Add(Me.rbtnAlgebraic)
            Me.gboxDeriveOption.Controls.Add(Me.gboxAlgebraic)
            Me.gboxDeriveOption.Controls.Add(Me.rbtnAggregate)
            Me.gboxDeriveOption.Controls.Add(Me.gboxAggregate)
            Me.gboxDeriveOption.Controls.Add(Me.rbtnCopy)
            Me.gboxDeriveOption.Location = New System.Drawing.Point(12, 12)
            Me.gboxDeriveOption.Name = "gboxDeriveOption"
            Me.gboxDeriveOption.Size = New System.Drawing.Size(683, 196)
            Me.gboxDeriveOption.TabIndex = 10
            Me.gboxDeriveOption.TabStop = False
            Me.gboxDeriveOption.Text = "Derive Options"
            '
            'rbtnAlgebraic
            '
            Me.rbtnAlgebraic.AutoSize = True
            Me.rbtnAlgebraic.Location = New System.Drawing.Point(15, 132)
            Me.rbtnAlgebraic.Name = "rbtnAlgebraic"
            Me.rbtnAlgebraic.Size = New System.Drawing.Size(194, 17)
            Me.rbtnAlgebraic.TabIndex = 4
            Me.rbtnAlgebraic.TabStop = True
            Me.rbtnAlgebraic.Text = "Derive Using An Algebraic Equation"
            Me.rbtnAlgebraic.UseVisualStyleBackColor = True
            '
            'gboxAlgebraic
            '
            Me.gboxAlgebraic.Controls.Add(Me.lblE)
            Me.gboxAlgebraic.Controls.Add(Me.txtF)
            Me.gboxAlgebraic.Controls.Add(Me.lblD)
            Me.gboxAlgebraic.Controls.Add(Me.txtE)
            Me.gboxAlgebraic.Controls.Add(Me.lblC)
            Me.gboxAlgebraic.Controls.Add(Me.txtD)
            Me.gboxAlgebraic.Controls.Add(Me.lblB)
            Me.gboxAlgebraic.Controls.Add(Me.txtC)
            Me.gboxAlgebraic.Controls.Add(Me.lblA)
            Me.gboxAlgebraic.Controls.Add(Me.txtB)
            Me.gboxAlgebraic.Controls.Add(Me.lbl1)
            Me.gboxAlgebraic.Controls.Add(Me.txtA)
            Me.gboxAlgebraic.Controls.Add(Me.lblY)
            Me.gboxAlgebraic.Location = New System.Drawing.Point(9, 132)
            Me.gboxAlgebraic.Name = "gboxAlgebraic"
            Me.gboxAlgebraic.Size = New System.Drawing.Size(655, 55)
            Me.gboxAlgebraic.TabIndex = 3
            Me.gboxAlgebraic.TabStop = False
            Me.gboxAlgebraic.Text = "                                        "
            '
            'lblE
            '
            Me.lblE.AutoSize = True
            Me.lblE.Location = New System.Drawing.Point(572, 24)
            Me.lblE.Name = "lblE"
            Me.lblE.Size = New System.Drawing.Size(24, 13)
            Me.lblE.TabIndex = 12
            Me.lblE.Text = "x^5"
            '
            'txtF
            '
            Me.txtF.Location = New System.Drawing.Point(512, 21)
            Me.txtF.Name = "txtF"
            Me.txtF.Size = New System.Drawing.Size(54, 20)
            Me.txtF.TabIndex = 11
            Me.txtF.Text = "0"
            Me.txtF.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'lblD
            '
            Me.lblD.AutoSize = True
            Me.lblD.Location = New System.Drawing.Point(470, 24)
            Me.lblD.Name = "lblD"
            Me.lblD.Size = New System.Drawing.Size(36, 13)
            Me.lblD.TabIndex = 10
            Me.lblD.Text = "x^4 + "
            '
            'txtE
            '
            Me.txtE.Location = New System.Drawing.Point(410, 21)
            Me.txtE.Name = "txtE"
            Me.txtE.Size = New System.Drawing.Size(54, 20)
            Me.txtE.TabIndex = 9
            Me.txtE.Text = "0"
            Me.txtE.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'lblC
            '
            Me.lblC.AutoSize = True
            Me.lblC.Location = New System.Drawing.Point(368, 24)
            Me.lblC.Name = "lblC"
            Me.lblC.Size = New System.Drawing.Size(36, 13)
            Me.lblC.TabIndex = 8
            Me.lblC.Text = "x^3 + "
            '
            'txtD
            '
            Me.txtD.Location = New System.Drawing.Point(308, 21)
            Me.txtD.Name = "txtD"
            Me.txtD.Size = New System.Drawing.Size(54, 20)
            Me.txtD.TabIndex = 7
            Me.txtD.Text = "0"
            Me.txtD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'lblB
            '
            Me.lblB.AutoSize = True
            Me.lblB.Location = New System.Drawing.Point(266, 24)
            Me.lblB.Name = "lblB"
            Me.lblB.Size = New System.Drawing.Size(36, 13)
            Me.lblB.TabIndex = 6
            Me.lblB.Text = "x^2 + "
            '
            'txtC
            '
            Me.txtC.Location = New System.Drawing.Point(206, 21)
            Me.txtC.Name = "txtC"
            Me.txtC.Size = New System.Drawing.Size(54, 20)
            Me.txtC.TabIndex = 5
            Me.txtC.Text = "0"
            Me.txtC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'lblA
            '
            Me.lblA.AutoSize = True
            Me.lblA.Location = New System.Drawing.Point(176, 24)
            Me.lblA.Name = "lblA"
            Me.lblA.Size = New System.Drawing.Size(24, 13)
            Me.lblA.TabIndex = 4
            Me.lblA.Text = "x + "
            '
            'txtB
            '
            Me.txtB.Location = New System.Drawing.Point(116, 21)
            Me.txtB.Name = "txtB"
            Me.txtB.Size = New System.Drawing.Size(54, 20)
            Me.txtB.TabIndex = 3
            Me.txtB.Text = "0"
            Me.txtB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'lbl1
            '
            Me.lbl1.AutoSize = True
            Me.lbl1.Location = New System.Drawing.Point(97, 24)
            Me.lbl1.Name = "lbl1"
            Me.lbl1.Size = New System.Drawing.Size(13, 13)
            Me.lbl1.TabIndex = 2
            Me.lbl1.Text = "+"
            '
            'txtA
            '
            Me.txtA.Location = New System.Drawing.Point(36, 21)
            Me.txtA.Name = "txtA"
            Me.txtA.Size = New System.Drawing.Size(54, 20)
            Me.txtA.TabIndex = 1
            Me.txtA.Text = "0"
            Me.txtA.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
            '
            'lblY
            '
            Me.lblY.AutoSize = True
            Me.lblY.Location = New System.Drawing.Point(7, 24)
            Me.lblY.Name = "lblY"
            Me.lblY.Size = New System.Drawing.Size(26, 13)
            Me.lblY.TabIndex = 0
            Me.lblY.Text = "Y = "
            '
            'rbtnAggregate
            '
            Me.rbtnAggregate.AutoSize = True
            Me.rbtnAggregate.Location = New System.Drawing.Point(15, 43)
            Me.rbtnAggregate.Name = "rbtnAggregate"
            Me.rbtnAggregate.Size = New System.Drawing.Size(197, 17)
            Me.rbtnAggregate.TabIndex = 1
            Me.rbtnAggregate.TabStop = True
            Me.rbtnAggregate.Text = "Derive Using an Aggregate Function"
            Me.rbtnAggregate.UseVisualStyleBackColor = True
            '
            'gboxAggregate
            '
            Me.gboxAggregate.Controls.Add(Me.Panel2)
            Me.gboxAggregate.Controls.Add(Me.Panel1)
            Me.gboxAggregate.Location = New System.Drawing.Point(9, 43)
            Me.gboxAggregate.Name = "gboxAggregate"
            Me.gboxAggregate.Size = New System.Drawing.Size(655, 83)
            Me.gboxAggregate.TabIndex = 2
            Me.gboxAggregate.TabStop = False
            Me.gboxAggregate.Text = "                                        "
            '
            'Panel2
            '
            Me.Panel2.Controls.Add(Me.rbtnDaily)
            Me.Panel2.Controls.Add(Me.rbtnQuarterly)
            Me.Panel2.Controls.Add(Me.rbtnMonthly)
            Me.Panel2.Location = New System.Drawing.Point(7, 23)
            Me.Panel2.Name = "Panel2"
            Me.Panel2.Size = New System.Drawing.Size(457, 23)
            Me.Panel2.TabIndex = 4
            '
            'rbtnDaily
            '
            Me.rbtnDaily.AutoSize = True
            Me.rbtnDaily.Location = New System.Drawing.Point(7, 3)
            Me.rbtnDaily.Name = "rbtnDaily"
            Me.rbtnDaily.Size = New System.Drawing.Size(48, 17)
            Me.rbtnDaily.TabIndex = 0
            Me.rbtnDaily.TabStop = True
            Me.rbtnDaily.Text = "Daily"
            Me.rbtnDaily.UseVisualStyleBackColor = True
            '
            'rbtnQuarterly
            '
            Me.rbtnQuarterly.AutoSize = True
            Me.rbtnQuarterly.Location = New System.Drawing.Point(234, 3)
            Me.rbtnQuarterly.Name = "rbtnQuarterly"
            Me.rbtnQuarterly.Size = New System.Drawing.Size(67, 17)
            Me.rbtnQuarterly.TabIndex = 2
            Me.rbtnQuarterly.TabStop = True
            Me.rbtnQuarterly.Text = "Quarterly"
            Me.rbtnQuarterly.UseVisualStyleBackColor = True
            '
            'rbtnMonthly
            '
            Me.rbtnMonthly.AutoSize = True
            Me.rbtnMonthly.Location = New System.Drawing.Point(120, 3)
            Me.rbtnMonthly.Name = "rbtnMonthly"
            Me.rbtnMonthly.Size = New System.Drawing.Size(62, 17)
            Me.rbtnMonthly.TabIndex = 1
            Me.rbtnMonthly.TabStop = True
            Me.rbtnMonthly.Text = "Monthly"
            Me.rbtnMonthly.UseVisualStyleBackColor = True
            '
            'Panel1
            '
            Me.Panel1.Controls.Add(Me.rbtnSum)
            Me.Panel1.Controls.Add(Me.rbtnMaximum)
            Me.Panel1.Controls.Add(Me.rbtnAverage)
            Me.Panel1.Controls.Add(Me.rbtnMinimum)
            Me.Panel1.Location = New System.Drawing.Point(7, 52)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(457, 23)
            Me.Panel1.TabIndex = 3
            '
            'rbtnSum
            '
            Me.rbtnSum.AutoSize = True
            Me.rbtnSum.Location = New System.Drawing.Point(347, 3)
            Me.rbtnSum.Name = "rbtnSum"
            Me.rbtnSum.Size = New System.Drawing.Size(46, 17)
            Me.rbtnSum.TabIndex = 3
            Me.rbtnSum.TabStop = True
            Me.rbtnSum.Text = "Sum"
            Me.rbtnSum.UseVisualStyleBackColor = True
            '
            'rbtnMaximum
            '
            Me.rbtnMaximum.AutoSize = True
            Me.rbtnMaximum.Location = New System.Drawing.Point(7, 3)
            Me.rbtnMaximum.Name = "rbtnMaximum"
            Me.rbtnMaximum.Size = New System.Drawing.Size(69, 17)
            Me.rbtnMaximum.TabIndex = 0
            Me.rbtnMaximum.TabStop = True
            Me.rbtnMaximum.Text = "Maximum"
            Me.rbtnMaximum.UseVisualStyleBackColor = True
            '
            'rbtnAverage
            '
            Me.rbtnAverage.AutoSize = True
            Me.rbtnAverage.Location = New System.Drawing.Point(234, 3)
            Me.rbtnAverage.Name = "rbtnAverage"
            Me.rbtnAverage.Size = New System.Drawing.Size(65, 17)
            Me.rbtnAverage.TabIndex = 2
            Me.rbtnAverage.TabStop = True
            Me.rbtnAverage.Text = "Average"
            Me.rbtnAverage.UseVisualStyleBackColor = True
            '
            'rbtnMinimum
            '
            Me.rbtnMinimum.AutoSize = True
            Me.rbtnMinimum.Location = New System.Drawing.Point(120, 3)
            Me.rbtnMinimum.Name = "rbtnMinimum"
            Me.rbtnMinimum.Size = New System.Drawing.Size(66, 17)
            Me.rbtnMinimum.TabIndex = 1
            Me.rbtnMinimum.TabStop = True
            Me.rbtnMinimum.Text = "Minimum"
            Me.rbtnMinimum.UseVisualStyleBackColor = True
            '
            'rbtnCopy
            '
            Me.rbtnCopy.AutoSize = True
            Me.rbtnCopy.Location = New System.Drawing.Point(15, 20)
            Me.rbtnCopy.Name = "rbtnCopy"
            Me.rbtnCopy.Size = New System.Drawing.Size(186, 17)
            Me.rbtnCopy.TabIndex = 0
            Me.rbtnCopy.TabStop = True
            Me.rbtnCopy.Text = "Derive A Copy Of Data For Editing"
            Me.rbtnCopy.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            Me.btnCancel.Location = New System.Drawing.Point(358, 350)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(337, 37)
            Me.btnCancel.TabIndex = 11
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'fDeriveNewDataSeries
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(707, 394)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.gboxDeriveOption)
            Me.Controls.Add(Me.gboxgeneral)
            Me.Controls.Add(Me.btnNewSeries)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "fDeriveNewDataSeries"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Derive New Series"
            CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
            Me.gboxgeneral.ResumeLayout(False)
            Me.gboxgeneral.PerformLayout()
            Me.gboxDeriveOption.ResumeLayout(False)
            Me.gboxDeriveOption.PerformLayout()
            Me.gboxAlgebraic.ResumeLayout(False)
            Me.gboxAlgebraic.PerformLayout()
            Me.gboxAggregate.ResumeLayout(False)
            Me.Panel2.ResumeLayout(False)
            Me.Panel2.PerformLayout()
            Me.Panel1.ResumeLayout(False)
            Me.Panel1.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents btnNewSeries As System.Windows.Forms.Button
        Friend WithEvents ConfigBindingSource As System.Windows.Forms.BindingSource
        Friend WithEvents lblMethods As System.Windows.Forms.Label
        Friend WithEvents ddlMethods As System.Windows.Forms.ComboBox
        Friend WithEvents btnMethods As System.Windows.Forms.Button
        Friend WithEvents lblComment As System.Windows.Forms.Label
        Friend WithEvents txtComment As System.Windows.Forms.TextBox
        Friend WithEvents gboxgeneral As System.Windows.Forms.GroupBox
        Friend WithEvents btnBackToOriginal As System.Windows.Forms.Button
        Friend WithEvents gboxDeriveOption As System.Windows.Forms.GroupBox
        Friend WithEvents rbtnCopy As System.Windows.Forms.RadioButton
        Friend WithEvents rbtnAlgebraic As System.Windows.Forms.RadioButton
        Friend WithEvents gboxAlgebraic As System.Windows.Forms.GroupBox
        Friend WithEvents rbtnAggregate As System.Windows.Forms.RadioButton
        Friend WithEvents gboxAggregate As System.Windows.Forms.GroupBox
        Friend WithEvents rbtnAverage As System.Windows.Forms.RadioButton
        Friend WithEvents rbtnMinimum As System.Windows.Forms.RadioButton
        Friend WithEvents rbtnMaximum As System.Windows.Forms.RadioButton
        Friend WithEvents lblA As System.Windows.Forms.Label
        Friend WithEvents txtB As System.Windows.Forms.TextBox
        Friend WithEvents lbl1 As System.Windows.Forms.Label
        Friend WithEvents txtA As System.Windows.Forms.TextBox
        Friend WithEvents lblY As System.Windows.Forms.Label
        Friend WithEvents lblE As System.Windows.Forms.Label
        Friend WithEvents txtF As System.Windows.Forms.TextBox
        Friend WithEvents lblD As System.Windows.Forms.Label
        Friend WithEvents txtE As System.Windows.Forms.TextBox
        Friend WithEvents lblC As System.Windows.Forms.Label
        Friend WithEvents txtD As System.Windows.Forms.TextBox
        Friend WithEvents lblB As System.Windows.Forms.Label
        Friend WithEvents txtC As System.Windows.Forms.TextBox
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents Panel2 As System.Windows.Forms.Panel
        Friend WithEvents rbtnDaily As System.Windows.Forms.RadioButton
        Friend WithEvents rbtnQuarterly As System.Windows.Forms.RadioButton
        Friend WithEvents rbtnMonthly As System.Windows.Forms.RadioButton
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents lblQualityControlLevel As System.Windows.Forms.Label
        Friend WithEvents ddlQualityControlLevel As System.Windows.Forms.ComboBox
        Friend WithEvents btnQualityControlLevel As System.Windows.Forms.Button
        Friend WithEvents lblVariable As System.Windows.Forms.Label
        Friend WithEvents btnVariable As System.Windows.Forms.Button
        Friend WithEvents ddlVariable As System.Windows.Forms.ComboBox
        Friend WithEvents rbtnSum As System.Windows.Forms.RadioButton
    End Class
End Namespace