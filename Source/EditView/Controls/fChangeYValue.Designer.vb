Imports EditView

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class fChangeYValue
    Inherits System.Windows.Forms.Form
    Public _cEditView As cEditView
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
        Me.txtValue = New System.Windows.Forms.TextBox
        Me.btnApplyChange = New System.Windows.Forms.Button
        Me.ddlMethod = New System.Windows.Forms.ComboBox
        Me.btnCancel = New System.Windows.Forms.Button
        CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ConfigBindingSource
        '
        Me.ConfigBindingSource.DataSource = GetType(HydroDesktop.Configuration.Settings)
        '
        'txtValue
        '
        Me.txtValue.Location = New System.Drawing.Point(122, 12)
        Me.txtValue.Name = "txtValue"
        Me.txtValue.Size = New System.Drawing.Size(100, 20)
        Me.txtValue.TabIndex = 2
        '
        'btnApplyChange
        '
        Me.btnApplyChange.Location = New System.Drawing.Point(13, 46)
        Me.btnApplyChange.Name = "btnApplyChange"
        Me.btnApplyChange.Size = New System.Drawing.Size(103, 23)
        Me.btnApplyChange.TabIndex = 4
        Me.btnApplyChange.Text = "Apply"
        Me.btnApplyChange.UseVisualStyleBackColor = True
        '
        'ddlMethod
        '
        Me.ddlMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlMethod.FormattingEnabled = True
        Me.ddlMethod.Items.AddRange(New Object() {"Add", "Subtract", "Multiply", "Set value to"})
        Me.ddlMethod.Location = New System.Drawing.Point(13, 11)
        Me.ddlMethod.Name = "ddlMethod"
        Me.ddlMethod.Size = New System.Drawing.Size(103, 21)
        Me.ddlMethod.TabIndex = 9
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(122, 46)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 23)
        Me.btnCancel.TabIndex = 10
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'fChangeYValue
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(234, 83)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.ddlMethod)
        Me.Controls.Add(Me.btnApplyChange)
        Me.Controls.Add(Me.txtValue)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "fChangeYValue"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Change Y Value"
        CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ConfigBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents txtValue As System.Windows.Forms.TextBox
    Friend WithEvents btnApplyChange As System.Windows.Forms.Button
    Friend WithEvents ddlMethod As System.Windows.Forms.ComboBox
    Friend WithEvents btnCancel As System.Windows.Forms.Button
End Class
