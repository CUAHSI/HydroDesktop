Imports EditView

Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class QualifiersTableManagement
        Inherits System.Windows.Forms.Form
        Public _cEditView As EditView

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
            Me.txtQualifierCode = New System.Windows.Forms.TextBox()
            Me.lblDescription = New System.Windows.Forms.Label()
            Me.lblLink = New System.Windows.Forms.Label()
            Me.lblSetTo = New System.Windows.Forms.Label()
            Me.ddlQualifiers = New System.Windows.Forms.ComboBox()
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
            Me.btnSubmit.Location = New System.Drawing.Point(15, 157)
            Me.btnSubmit.Name = "btnSubmit"
            Me.btnSubmit.Size = New System.Drawing.Size(201, 31)
            Me.btnSubmit.TabIndex = 17
            Me.btnSubmit.Text = "Set"
            Me.btnSubmit.UseVisualStyleBackColor = True
            '
            'txtDescription
            '
            Me.txtDescription.Location = New System.Drawing.Point(94, 87)
            Me.txtDescription.Multiline = True
            Me.txtDescription.Name = "txtDescription"
            Me.txtDescription.Size = New System.Drawing.Size(331, 59)
            Me.txtDescription.TabIndex = 16
            '
            'txtQualifierCode
            '
            Me.txtQualifierCode.Location = New System.Drawing.Point(94, 51)
            Me.txtQualifierCode.Name = "txtQualifierCode"
            Me.txtQualifierCode.Size = New System.Drawing.Size(331, 20)
            Me.txtQualifierCode.TabIndex = 14
            '
            'lblDescription
            '
            Me.lblDescription.AutoSize = True
            Me.lblDescription.Location = New System.Drawing.Point(12, 87)
            Me.lblDescription.Name = "lblDescription"
            Me.lblDescription.Size = New System.Drawing.Size(63, 13)
            Me.lblDescription.TabIndex = 13
            Me.lblDescription.Text = "Description:"
            '
            'lblLink
            '
            Me.lblLink.AutoSize = True
            Me.lblLink.Location = New System.Drawing.Point(12, 54)
            Me.lblLink.Name = "lblLink"
            Me.lblLink.Size = New System.Drawing.Size(76, 13)
            Me.lblLink.TabIndex = 18
            Me.lblLink.Text = "Qualifier Code:"
            '
            'lblSetTo
            '
            Me.lblSetTo.AutoSize = True
            Me.lblSetTo.Location = New System.Drawing.Point(15, 13)
            Me.lblSetTo.Name = "lblSetTo"
            Me.lblSetTo.Size = New System.Drawing.Size(97, 13)
            Me.lblSetTo.TabIndex = 19
            Me.lblSetTo.Text = "Set the Qualifier to:"
            '
            'ddlQualifiers
            '
            Me.ddlQualifiers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.ddlQualifiers.FormattingEnabled = True
            Me.ddlQualifiers.Location = New System.Drawing.Point(119, 13)
            Me.ddlQualifiers.Name = "ddlQualifiers"
            Me.ddlQualifiers.Size = New System.Drawing.Size(306, 21)
            Me.ddlQualifiers.TabIndex = 20
            '
            'btnCancel
            '
            Me.btnCancel.Location = New System.Drawing.Point(222, 157)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(203, 31)
            Me.btnCancel.TabIndex = 21
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'fQualifiersTableManagement
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(437, 188)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.ddlQualifiers)
            Me.Controls.Add(Me.lblSetTo)
            Me.Controls.Add(Me.lblLink)
            Me.Controls.Add(Me.btnSubmit)
            Me.Controls.Add(Me.txtDescription)
            Me.Controls.Add(Me.txtQualifierCode)
            Me.Controls.Add(Me.lblDescription)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "fQualifiersTableManagement"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "Qualifiers Table Management"
            CType(Me.ConfigBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents ConfigBindingSource As System.Windows.Forms.BindingSource
        Friend WithEvents btnSubmit As System.Windows.Forms.Button
        Friend WithEvents txtDescription As System.Windows.Forms.TextBox
        Friend WithEvents txtQualifierCode As System.Windows.Forms.TextBox
        Friend WithEvents lblDescription As System.Windows.Forms.Label
        Friend WithEvents lblLink As System.Windows.Forms.Label
        Friend WithEvents lblSetTo As System.Windows.Forms.Label
        Friend WithEvents ddlQualifiers As System.Windows.Forms.ComboBox
        Friend WithEvents btnCancel As System.Windows.Forms.Button
    End Class
End Namespace