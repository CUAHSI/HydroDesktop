Imports HydroDesktop.Plugins.EditView

Namespace HydroDesktop.Plugins.Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class VariablesTableManagement
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
            Me.btnSubmit = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.lblVariableCode = New System.Windows.Forms.Label()
            Me.txtVariableCode = New System.Windows.Forms.TextBox()
            Me.txtVariableName = New System.Windows.Forms.TextBox()
            Me.lblVariableName = New System.Windows.Forms.Label()
            Me.txtSpeciation = New System.Windows.Forms.TextBox()
            Me.lblSpeciation = New System.Windows.Forms.Label()
            Me.txtSampleMedium = New System.Windows.Forms.TextBox()
            Me.lblSampleMedium = New System.Windows.Forms.Label()
            Me.gboxVariableUnits = New System.Windows.Forms.GroupBox()
            Me.ddlUnitsName = New System.Windows.Forms.ComboBox()
            Me.lblVUnitsAbbreviation = New System.Windows.Forms.Label()
            Me.lblVUnitsType = New System.Windows.Forms.Label()
            Me.lblValueType = New System.Windows.Forms.Label()
            Me.ddlValueType = New System.Windows.Forms.ComboBox()
            Me.lblRegular = New System.Windows.Forms.Label()
            Me.Panel1 = New System.Windows.Forms.Panel()
            Me.rbtnRegularNo = New System.Windows.Forms.RadioButton()
            Me.rbtnRegularYes = New System.Windows.Forms.RadioButton()
            Me.gboxTimeSupport = New System.Windows.Forms.GroupBox()
            Me.txtTValue = New System.Windows.Forms.TextBox()
            Me.lblTValue = New System.Windows.Forms.Label()
            Me.ddlTUnitsName = New System.Windows.Forms.ComboBox()
            Me.lblTUnitsAbbreviation = New System.Windows.Forms.Label()
            Me.lblTUnitsType = New System.Windows.Forms.Label()
            Me.ConfigBindingSource = New System.Windows.Forms.BindingSource(Me.components)
            Me.ddlDataType = New System.Windows.Forms.ComboBox()
            Me.lblDataType = New System.Windows.Forms.Label()
            Me.txtNoDataValue = New System.Windows.Forms.TextBox()
            Me.lblNoDataValue = New System.Windows.Forms.Label()
            Me.txtGeneralCategory = New System.Windows.Forms.TextBox()
            Me.lblGeneralCategory = New System.Windows.Forms.Label()
            Me.gboxVariableUnits.SuspendLayout()
            Me.Panel1.SuspendLayout()
            Me.gboxTimeSupport.SuspendLayout()
            CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'btnSubmit
            '
            Me.btnSubmit.Location = New System.Drawing.Point(12, 227)
            Me.btnSubmit.Name = "btnSubmit"
            Me.btnSubmit.Size = New System.Drawing.Size(202, 31)
            Me.btnSubmit.TabIndex = 17
            Me.btnSubmit.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            Me.btnCancel.Location = New System.Drawing.Point(220, 227)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(205, 31)
            Me.btnCancel.TabIndex = 19
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'lblVariableCode
            '
            Me.lblVariableCode.AutoSize = True
            Me.lblVariableCode.Location = New System.Drawing.Point(13, 13)
            Me.lblVariableCode.Name = "lblVariableCode"
            Me.lblVariableCode.Size = New System.Drawing.Size(76, 13)
            Me.lblVariableCode.TabIndex = 20
            Me.lblVariableCode.Text = "Variable Code:"
            '
            'txtVariableCode
            '
            Me.txtVariableCode.Location = New System.Drawing.Point(95, 10)
            Me.txtVariableCode.Name = "txtVariableCode"
            Me.txtVariableCode.Size = New System.Drawing.Size(80, 20)
            Me.txtVariableCode.TabIndex = 21
            '
            'txtVariableName
            '
            Me.txtVariableName.Location = New System.Drawing.Point(272, 10)
            Me.txtVariableName.Name = "txtVariableName"
            Me.txtVariableName.Size = New System.Drawing.Size(153, 20)
            Me.txtVariableName.TabIndex = 23
            '
            'lblVariableName
            '
            Me.lblVariableName.AutoSize = True
            Me.lblVariableName.Location = New System.Drawing.Point(194, 13)
            Me.lblVariableName.Name = "lblVariableName"
            Me.lblVariableName.Size = New System.Drawing.Size(79, 13)
            Me.lblVariableName.TabIndex = 22
            Me.lblVariableName.Text = "Variable Name:"
            '
            'txtSpeciation
            '
            Me.txtSpeciation.Location = New System.Drawing.Point(272, 36)
            Me.txtSpeciation.Name = "txtSpeciation"
            Me.txtSpeciation.Size = New System.Drawing.Size(153, 20)
            Me.txtSpeciation.TabIndex = 25
            '
            'lblSpeciation
            '
            Me.lblSpeciation.AutoSize = True
            Me.lblSpeciation.Location = New System.Drawing.Point(213, 39)
            Me.lblSpeciation.Name = "lblSpeciation"
            Me.lblSpeciation.Size = New System.Drawing.Size(60, 13)
            Me.lblSpeciation.TabIndex = 24
            Me.lblSpeciation.Text = "Speciation:"
            '
            'txtSampleMedium
            '
            Me.txtSampleMedium.Location = New System.Drawing.Point(272, 62)
            Me.txtSampleMedium.Name = "txtSampleMedium"
            Me.txtSampleMedium.Size = New System.Drawing.Size(153, 20)
            Me.txtSampleMedium.TabIndex = 27
            '
            'lblSampleMedium
            '
            Me.lblSampleMedium.AutoSize = True
            Me.lblSampleMedium.Location = New System.Drawing.Point(188, 65)
            Me.lblSampleMedium.Name = "lblSampleMedium"
            Me.lblSampleMedium.Size = New System.Drawing.Size(85, 13)
            Me.lblSampleMedium.TabIndex = 26
            Me.lblSampleMedium.Text = "Sample Medium:"
            '
            'gboxVariableUnits
            '
            Me.gboxVariableUnits.Controls.Add(Me.ddlUnitsName)
            Me.gboxVariableUnits.Controls.Add(Me.lblVUnitsAbbreviation)
            Me.gboxVariableUnits.Controls.Add(Me.lblVUnitsType)
            Me.gboxVariableUnits.Location = New System.Drawing.Point(12, 36)
            Me.gboxVariableUnits.Name = "gboxVariableUnits"
            Me.gboxVariableUnits.Size = New System.Drawing.Size(163, 71)
            Me.gboxVariableUnits.TabIndex = 28
            Me.gboxVariableUnits.TabStop = False
            Me.gboxVariableUnits.Text = "Variable Units:"
            '
            'ddlUnitsName
            '
            Me.ddlUnitsName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ddlUnitsName.FormattingEnabled = True
            Me.ddlUnitsName.Location = New System.Drawing.Point(7, 20)
            Me.ddlUnitsName.Name = "ddlUnitsName"
            Me.ddlUnitsName.Size = New System.Drawing.Size(150, 21)
            Me.ddlUnitsName.TabIndex = 2
            '
            'lblVUnitsAbbreviation
            '
            Me.lblVUnitsAbbreviation.AutoSize = True
            Me.lblVUnitsAbbreviation.Location = New System.Drawing.Point(6, 57)
            Me.lblVUnitsAbbreviation.Name = "lblVUnitsAbbreviation"
            Me.lblVUnitsAbbreviation.Size = New System.Drawing.Size(99, 13)
            Me.lblVUnitsAbbreviation.TabIndex = 1
            Me.lblVUnitsAbbreviation.Text = "Units Abbreviation: "
            '
            'lblVUnitsType
            '
            Me.lblVUnitsType.AutoSize = True
            Me.lblVUnitsType.Location = New System.Drawing.Point(6, 44)
            Me.lblVUnitsType.Name = "lblVUnitsType"
            Me.lblVUnitsType.Size = New System.Drawing.Size(64, 13)
            Me.lblVUnitsType.TabIndex = 0
            Me.lblVUnitsType.Text = "Units Type: "
            '
            'lblValueType
            '
            Me.lblValueType.AutoSize = True
            Me.lblValueType.Location = New System.Drawing.Point(209, 89)
            Me.lblValueType.Name = "lblValueType"
            Me.lblValueType.Size = New System.Drawing.Size(64, 13)
            Me.lblValueType.TabIndex = 29
            Me.lblValueType.Text = "Value Type:"
            '
            'ddlValueType
            '
            Me.ddlValueType.FormattingEnabled = True
            Me.ddlValueType.Items.AddRange(New Object() {"Field Observation", "Derived Value", "Unknown", "Others..."})
            Me.ddlValueType.Location = New System.Drawing.Point(272, 86)
            Me.ddlValueType.Name = "ddlValueType"
            Me.ddlValueType.Size = New System.Drawing.Size(153, 21)
            Me.ddlValueType.TabIndex = 31
            '
            'lblRegular
            '
            Me.lblRegular.AutoSize = True
            Me.lblRegular.Location = New System.Drawing.Point(226, 119)
            Me.lblRegular.Name = "lblRegular"
            Me.lblRegular.Size = New System.Drawing.Size(47, 13)
            Me.lblRegular.TabIndex = 32
            Me.lblRegular.Text = "Regular:"
            '
            'Panel1
            '
            Me.Panel1.Controls.Add(Me.rbtnRegularNo)
            Me.Panel1.Controls.Add(Me.rbtnRegularYes)
            Me.Panel1.Location = New System.Drawing.Point(272, 113)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(153, 23)
            Me.Panel1.TabIndex = 33
            '
            'rbtnRegularNo
            '
            Me.rbtnRegularNo.AutoSize = True
            Me.rbtnRegularNo.Location = New System.Drawing.Point(63, 4)
            Me.rbtnRegularNo.Name = "rbtnRegularNo"
            Me.rbtnRegularNo.Size = New System.Drawing.Size(39, 17)
            Me.rbtnRegularNo.TabIndex = 1
            Me.rbtnRegularNo.TabStop = True
            Me.rbtnRegularNo.Text = "No"
            Me.rbtnRegularNo.UseVisualStyleBackColor = True
            '
            'rbtnRegularYes
            '
            Me.rbtnRegularYes.AutoSize = True
            Me.rbtnRegularYes.Location = New System.Drawing.Point(4, 4)
            Me.rbtnRegularYes.Name = "rbtnRegularYes"
            Me.rbtnRegularYes.Size = New System.Drawing.Size(43, 17)
            Me.rbtnRegularYes.TabIndex = 0
            Me.rbtnRegularYes.TabStop = True
            Me.rbtnRegularYes.Text = "Yes"
            Me.rbtnRegularYes.UseVisualStyleBackColor = True
            '
            'gboxTimeSupport
            '
            Me.gboxTimeSupport.Controls.Add(Me.txtTValue)
            Me.gboxTimeSupport.Controls.Add(Me.lblTValue)
            Me.gboxTimeSupport.Controls.Add(Me.ddlTUnitsName)
            Me.gboxTimeSupport.Controls.Add(Me.lblTUnitsAbbreviation)
            Me.gboxTimeSupport.Controls.Add(Me.lblTUnitsType)
            Me.gboxTimeSupport.Location = New System.Drawing.Point(12, 113)
            Me.gboxTimeSupport.Name = "gboxTimeSupport"
            Me.gboxTimeSupport.Size = New System.Drawing.Size(163, 96)
            Me.gboxTimeSupport.TabIndex = 29
            Me.gboxTimeSupport.TabStop = False
            Me.gboxTimeSupport.Text = "Time Support:"
            '
            'txtTValue
            '
            Me.txtTValue.Location = New System.Drawing.Point(50, 17)
            Me.txtTValue.Name = "txtTValue"
            Me.txtTValue.Size = New System.Drawing.Size(107, 20)
            Me.txtTValue.TabIndex = 4
            '
            'lblTValue
            '
            Me.lblTValue.AutoSize = True
            Me.lblTValue.Location = New System.Drawing.Point(7, 20)
            Me.lblTValue.Name = "lblTValue"
            Me.lblTValue.Size = New System.Drawing.Size(37, 13)
            Me.lblTValue.TabIndex = 3
            Me.lblTValue.Text = "Value:"
            '
            'ddlTUnitsName
            '
            Me.ddlTUnitsName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ddlTUnitsName.FormattingEnabled = True
            Me.ddlTUnitsName.Location = New System.Drawing.Point(7, 43)
            Me.ddlTUnitsName.Name = "ddlTUnitsName"
            Me.ddlTUnitsName.Size = New System.Drawing.Size(150, 21)
            Me.ddlTUnitsName.TabIndex = 2
            '
            'lblTUnitsAbbreviation
            '
            Me.lblTUnitsAbbreviation.AutoSize = True
            Me.lblTUnitsAbbreviation.Location = New System.Drawing.Point(6, 80)
            Me.lblTUnitsAbbreviation.Name = "lblTUnitsAbbreviation"
            Me.lblTUnitsAbbreviation.Size = New System.Drawing.Size(99, 13)
            Me.lblTUnitsAbbreviation.TabIndex = 1
            Me.lblTUnitsAbbreviation.Text = "Units Abbreviation: "
            '
            'lblTUnitsType
            '
            Me.lblTUnitsType.AutoSize = True
            Me.lblTUnitsType.Location = New System.Drawing.Point(6, 67)
            Me.lblTUnitsType.Name = "lblTUnitsType"
            Me.lblTUnitsType.Size = New System.Drawing.Size(64, 13)
            Me.lblTUnitsType.TabIndex = 0
            Me.lblTUnitsType.Text = "Units Type: "
            '
            'ConfigBindingSource
            '
            Me.ConfigBindingSource.DataSource = GetType(HydroDesktop.Configuration.Settings)
            '
            'ddlDataType
            '
            Me.ddlDataType.FormattingEnabled = True
            Me.ddlDataType.Items.AddRange(New Object() {"Average", "Maximum", "Minimum", "Sum", "Continuous", "Unknown", "Others..."})
            Me.ddlDataType.Location = New System.Drawing.Point(272, 142)
            Me.ddlDataType.Name = "ddlDataType"
            Me.ddlDataType.Size = New System.Drawing.Size(153, 21)
            Me.ddlDataType.TabIndex = 35
            '
            'lblDataType
            '
            Me.lblDataType.AutoSize = True
            Me.lblDataType.Location = New System.Drawing.Point(213, 145)
            Me.lblDataType.Name = "lblDataType"
            Me.lblDataType.Size = New System.Drawing.Size(60, 13)
            Me.lblDataType.TabIndex = 34
            Me.lblDataType.Text = "Data Type:"
            '
            'txtNoDataValue
            '
            Me.txtNoDataValue.Location = New System.Drawing.Point(272, 169)
            Me.txtNoDataValue.Name = "txtNoDataValue"
            Me.txtNoDataValue.Size = New System.Drawing.Size(153, 20)
            Me.txtNoDataValue.TabIndex = 37
            '
            'lblNoDataValue
            '
            Me.lblNoDataValue.AutoSize = True
            Me.lblNoDataValue.Location = New System.Drawing.Point(190, 172)
            Me.lblNoDataValue.Name = "lblNoDataValue"
            Me.lblNoDataValue.Size = New System.Drawing.Size(83, 13)
            Me.lblNoDataValue.TabIndex = 36
            Me.lblNoDataValue.Text = "No Data Value: "
            '
            'txtGeneralCategory
            '
            Me.txtGeneralCategory.Location = New System.Drawing.Point(272, 195)
            Me.txtGeneralCategory.Name = "txtGeneralCategory"
            Me.txtGeneralCategory.Size = New System.Drawing.Size(153, 20)
            Me.txtGeneralCategory.TabIndex = 39
            '
            'lblGeneralCategory
            '
            Me.lblGeneralCategory.AutoSize = True
            Me.lblGeneralCategory.Location = New System.Drawing.Point(181, 198)
            Me.lblGeneralCategory.Name = "lblGeneralCategory"
            Me.lblGeneralCategory.Size = New System.Drawing.Size(92, 13)
            Me.lblGeneralCategory.TabIndex = 38
            Me.lblGeneralCategory.Text = "GeneralCategory: "
            '
            'fVariablesTableManagement
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(437, 268)
            Me.Controls.Add(Me.txtGeneralCategory)
            Me.Controls.Add(Me.lblGeneralCategory)
            Me.Controls.Add(Me.txtNoDataValue)
            Me.Controls.Add(Me.lblNoDataValue)
            Me.Controls.Add(Me.ddlDataType)
            Me.Controls.Add(Me.lblDataType)
            Me.Controls.Add(Me.gboxTimeSupport)
            Me.Controls.Add(Me.Panel1)
            Me.Controls.Add(Me.lblRegular)
            Me.Controls.Add(Me.ddlValueType)
            Me.Controls.Add(Me.lblValueType)
            Me.Controls.Add(Me.gboxVariableUnits)
            Me.Controls.Add(Me.txtSampleMedium)
            Me.Controls.Add(Me.lblSampleMedium)
            Me.Controls.Add(Me.txtSpeciation)
            Me.Controls.Add(Me.lblSpeciation)
            Me.Controls.Add(Me.txtVariableName)
            Me.Controls.Add(Me.lblVariableName)
            Me.Controls.Add(Me.txtVariableCode)
            Me.Controls.Add(Me.lblVariableCode)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnSubmit)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "fVariablesTableManagement"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Variables Table Management"
            Me.gboxVariableUnits.ResumeLayout(False)
            Me.gboxVariableUnits.PerformLayout()
            Me.Panel1.ResumeLayout(False)
            Me.Panel1.PerformLayout()
            Me.gboxTimeSupport.ResumeLayout(False)
            Me.gboxTimeSupport.PerformLayout()
            CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents ConfigBindingSource As System.Windows.Forms.BindingSource
        Friend WithEvents btnSubmit As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents lblVariableCode As System.Windows.Forms.Label
        Friend WithEvents txtVariableCode As System.Windows.Forms.TextBox
        Friend WithEvents txtVariableName As System.Windows.Forms.TextBox
        Friend WithEvents lblVariableName As System.Windows.Forms.Label
        Friend WithEvents txtSpeciation As System.Windows.Forms.TextBox
        Friend WithEvents lblSpeciation As System.Windows.Forms.Label
        Friend WithEvents txtSampleMedium As System.Windows.Forms.TextBox
        Friend WithEvents lblSampleMedium As System.Windows.Forms.Label
        Friend WithEvents gboxVariableUnits As System.Windows.Forms.GroupBox
        Friend WithEvents ddlUnitsName As System.Windows.Forms.ComboBox
        Friend WithEvents lblVUnitsAbbreviation As System.Windows.Forms.Label
        Friend WithEvents lblVUnitsType As System.Windows.Forms.Label
        Friend WithEvents lblValueType As System.Windows.Forms.Label
        Friend WithEvents ddlValueType As System.Windows.Forms.ComboBox
        Friend WithEvents lblRegular As System.Windows.Forms.Label
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents rbtnRegularNo As System.Windows.Forms.RadioButton
        Friend WithEvents rbtnRegularYes As System.Windows.Forms.RadioButton
        Friend WithEvents gboxTimeSupport As System.Windows.Forms.GroupBox
        Friend WithEvents ddlTUnitsName As System.Windows.Forms.ComboBox
        Friend WithEvents lblTUnitsAbbreviation As System.Windows.Forms.Label
        Friend WithEvents lblTUnitsType As System.Windows.Forms.Label
        Friend WithEvents txtTValue As System.Windows.Forms.TextBox
        Friend WithEvents lblTValue As System.Windows.Forms.Label
        Friend WithEvents ddlDataType As System.Windows.Forms.ComboBox
        Friend WithEvents lblDataType As System.Windows.Forms.Label
        Friend WithEvents txtNoDataValue As System.Windows.Forms.TextBox
        Friend WithEvents lblNoDataValue As System.Windows.Forms.Label
        Friend WithEvents txtGeneralCategory As System.Windows.Forms.TextBox
        Friend WithEvents lblGeneralCategory As System.Windows.Forms.Label
    End Class
End Namespace