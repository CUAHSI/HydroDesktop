Imports EditView

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class fMethodTableManagement
    Inherits System.Windows.Forms.Form
    Public _fDeriveNewDataSeries As fDeriveNewDataSeries
    Public _MethodID As Long
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
        Me.ConfigBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.btnSubmit = New System.Windows.Forms.Button()
        Me.txtDescription = New System.Windows.Forms.TextBox()
        Me.txtLink = New System.Windows.Forms.TextBox()
        Me.lblDescription = New System.Windows.Forms.Label()
        Me.lblLink = New System.Windows.Forms.Label()
        Me.btnCancel = New System.Windows.Forms.Button()
        CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ConfigBindingSource
        '
        Me.ConfigBindingSource.DataSource = GetType(HydroDesktop.Configuration.Settings)
        '
        'btnSubmit
        '
        Me.btnSubmit.Location = New System.Drawing.Point(12, 129)
        Me.btnSubmit.Name = "btnSubmit"
        Me.btnSubmit.Size = New System.Drawing.Size(202, 31)
        Me.btnSubmit.TabIndex = 17
        Me.btnSubmit.UseVisualStyleBackColor = True
        '
        'txtDescription
        '
        Me.txtDescription.Location = New System.Drawing.Point(82, 17)
        Me.txtDescription.Multiline = True
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.Size = New System.Drawing.Size(343, 59)
        Me.txtDescription.TabIndex = 16
        '
        'txtLink
        '
        Me.txtLink.Location = New System.Drawing.Point(82, 92)
        Me.txtLink.Name = "txtLink"
        Me.txtLink.Size = New System.Drawing.Size(343, 20)
        Me.txtLink.TabIndex = 14
        '
        'lblDescription
        '
        Me.lblDescription.AutoSize = True
        Me.lblDescription.Location = New System.Drawing.Point(13, 20)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(63, 13)
        Me.lblDescription.TabIndex = 13
        Me.lblDescription.Text = "Description:"
        '
        'lblLink
        '
        Me.lblLink.AutoSize = True
        Me.lblLink.Location = New System.Drawing.Point(13, 95)
        Me.lblLink.Name = "lblLink"
        Me.lblLink.Size = New System.Drawing.Size(30, 13)
        Me.lblLink.TabIndex = 18
        Me.lblLink.Text = "Link:"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(220, 129)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(205, 31)
        Me.btnCancel.TabIndex = 19
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'fMethodTableManagement
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(437, 170)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.lblLink)
        Me.Controls.Add(Me.btnSubmit)
        Me.Controls.Add(Me.txtDescription)
        Me.Controls.Add(Me.txtLink)
        Me.Controls.Add(Me.lblDescription)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "fMethodTableManagement"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Method Table Management"
        CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ConfigBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents btnSubmit As System.Windows.Forms.Button
    Friend WithEvents txtDescription As System.Windows.Forms.TextBox
    Friend WithEvents txtLink As System.Windows.Forms.TextBox
    Friend WithEvents lblDescription As System.Windows.Forms.Label
    Friend WithEvents lblLink As System.Windows.Forms.Label
    Friend WithEvents btnCancel As System.Windows.Forms.Button
End Class
