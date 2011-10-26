Imports EditView

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class fQualityControlLevelTableManagement
    Inherits System.Windows.Forms.Form
    Public _fDeriveNewDataSeries As fDeriveNewDataSeries
    Public _QualityControlLevelID As Integer

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
        Me.components = New System.ComponentModel.Container
        Me.ConfigBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.lblID = New System.Windows.Forms.Label
        Me.txtID = New System.Windows.Forms.TextBox
        Me.lblCode = New System.Windows.Forms.Label
        Me.txtCode = New System.Windows.Forms.TextBox
        Me.lblDefinition = New System.Windows.Forms.Label
        Me.txtDefinition = New System.Windows.Forms.TextBox
        Me.lblExplanation = New System.Windows.Forms.Label
        Me.txtExplanation = New System.Windows.Forms.TextBox
        Me.btnSubmit = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ConfigBindingSource
        '
        Me.ConfigBindingSource.DataSource = GetType(HydroDesktop.Configuration.Settings)
        '
        'lblID
        '
        Me.lblID.AutoSize = True
        Me.lblID.Location = New System.Drawing.Point(13, 13)
        Me.lblID.Name = "lblID"
        Me.lblID.Size = New System.Drawing.Size(21, 13)
        Me.lblID.TabIndex = 0
        Me.lblID.Text = "ID:"
        '
        'txtID
        '
        Me.txtID.Location = New System.Drawing.Point(40, 10)
        Me.txtID.Name = "txtID"
        Me.txtID.ReadOnly = True
        Me.txtID.Size = New System.Drawing.Size(82, 20)
        Me.txtID.TabIndex = 1
        '
        'lblCode
        '
        Me.lblCode.AutoSize = True
        Me.lblCode.Location = New System.Drawing.Point(143, 13)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.Size = New System.Drawing.Size(107, 13)
        Me.lblCode.TabIndex = 2
        Me.lblCode.Text = "Quality Control Level:"
        '
        'txtCode
        '
        Me.txtCode.Location = New System.Drawing.Point(256, 10)
        Me.txtCode.Name = "txtCode"
        Me.txtCode.Size = New System.Drawing.Size(232, 20)
        Me.txtCode.TabIndex = 3
        '
        'lblDefinition
        '
        Me.lblDefinition.AutoSize = True
        Me.lblDefinition.Location = New System.Drawing.Point(13, 49)
        Me.lblDefinition.Name = "lblDefinition"
        Me.lblDefinition.Size = New System.Drawing.Size(54, 13)
        Me.lblDefinition.TabIndex = 4
        Me.lblDefinition.Text = "Definition:"
        '
        'txtDefinition
        '
        Me.txtDefinition.Location = New System.Drawing.Point(73, 46)
        Me.txtDefinition.Name = "txtDefinition"
        Me.txtDefinition.Size = New System.Drawing.Size(415, 20)
        Me.txtDefinition.TabIndex = 5
        '
        'lblExplanation
        '
        Me.lblExplanation.AutoSize = True
        Me.lblExplanation.Location = New System.Drawing.Point(13, 82)
        Me.lblExplanation.Name = "lblExplanation"
        Me.lblExplanation.Size = New System.Drawing.Size(65, 13)
        Me.lblExplanation.TabIndex = 6
        Me.lblExplanation.Text = "Explanation:"
        '
        'txtExplanation
        '
        Me.txtExplanation.Location = New System.Drawing.Point(84, 79)
        Me.txtExplanation.Multiline = True
        Me.txtExplanation.Name = "txtExplanation"
        Me.txtExplanation.Size = New System.Drawing.Size(404, 94)
        Me.txtExplanation.TabIndex = 7
        '
        'btnSubmit
        '
        Me.btnSubmit.Location = New System.Drawing.Point(16, 179)
        Me.btnSubmit.Name = "btnSubmit"
        Me.btnSubmit.Size = New System.Drawing.Size(234, 31)
        Me.btnSubmit.TabIndex = 8
        Me.btnSubmit.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(256, 179)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(232, 31)
        Me.btnCancel.TabIndex = 9
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'fQualityControlLevelTableManagement
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(500, 210)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSubmit)
        Me.Controls.Add(Me.txtExplanation)
        Me.Controls.Add(Me.lblExplanation)
        Me.Controls.Add(Me.txtDefinition)
        Me.Controls.Add(Me.lblDefinition)
        Me.Controls.Add(Me.txtCode)
        Me.Controls.Add(Me.lblCode)
        Me.Controls.Add(Me.txtID)
        Me.Controls.Add(Me.lblID)
        Me.Name = "fQualityControlLevelTableManagement"
        Me.Text = "Quality Control Level Table Management"
        CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ConfigBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents lblID As System.Windows.Forms.Label
    Friend WithEvents txtID As System.Windows.Forms.TextBox
    Friend WithEvents lblCode As System.Windows.Forms.Label
    Friend WithEvents txtCode As System.Windows.Forms.TextBox
    Friend WithEvents lblDefinition As System.Windows.Forms.Label
    Friend WithEvents txtDefinition As System.Windows.Forms.TextBox
    Friend WithEvents lblExplanation As System.Windows.Forms.Label
    Friend WithEvents txtExplanation As System.Windows.Forms.TextBox
    Friend WithEvents btnSubmit As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
End Class
