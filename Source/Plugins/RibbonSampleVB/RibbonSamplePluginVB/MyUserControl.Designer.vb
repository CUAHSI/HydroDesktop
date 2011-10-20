<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MyUserControl
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
        Me.Label1 = New System.Windows.Forms.Label
        Me.lstCheckedSeries = New System.Windows.Forms.ListBox
        Me.lblCheckedSeries = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.DataGridView1 = New System.Windows.Forms.DataGridView
        Me.Label3 = New System.Windows.Forms.Label
        Me.txtSelectedID = New System.Windows.Forms.TextBox
        Me.btnSaveEdits = New System.Windows.Forms.Button
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(19, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(252, 18)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Added by VB.NET Sample Plugin"
        '
        'lstCheckedSeries
        '
        Me.lstCheckedSeries.FormattingEnabled = True
        Me.lstCheckedSeries.Location = New System.Drawing.Point(22, 68)
        Me.lstCheckedSeries.Name = "lstCheckedSeries"
        Me.lstCheckedSeries.Size = New System.Drawing.Size(90, 108)
        Me.lstCheckedSeries.TabIndex = 2
        '
        'lblCheckedSeries
        '
        Me.lblCheckedSeries.AutoSize = True
        Me.lblCheckedSeries.Location = New System.Drawing.Point(19, 52)
        Me.lblCheckedSeries.Name = "lblCheckedSeries"
        Me.lblCheckedSeries.Size = New System.Drawing.Size(80, 13)
        Me.lblCheckedSeries.TabIndex = 3
        Me.lblCheckedSeries.Text = "CheckedIDList:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(126, 52)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(92, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "SelectedSeriesID:"
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(22, 201)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(703, 249)
        Me.DataGridView1.TabIndex = 6
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(19, 185)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(160, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Edit the DataValues Table:"
        '
        'txtSelectedID
        '
        Me.txtSelectedID.Location = New System.Drawing.Point(129, 68)
        Me.txtSelectedID.Name = "txtSelectedID"
        Me.txtSelectedID.Size = New System.Drawing.Size(90, 20)
        Me.txtSelectedID.TabIndex = 8
        '
        'btnSaveEdits
        '
        Me.btnSaveEdits.Location = New System.Drawing.Point(609, 456)
        Me.btnSaveEdits.Name = "btnSaveEdits"
        Me.btnSaveEdits.Size = New System.Drawing.Size(116, 23)
        Me.btnSaveEdits.TabIndex = 9
        Me.btnSaveEdits.Text = "Save Edits"
        Me.btnSaveEdits.UseVisualStyleBackColor = True
        '
        'MyUserControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.Controls.Add(Me.btnSaveEdits)
        Me.Controls.Add(Me.txtSelectedID)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblCheckedSeries)
        Me.Controls.Add(Me.lstCheckedSeries)
        Me.Controls.Add(Me.Label1)
        Me.Name = "MyUserControl"
        Me.Size = New System.Drawing.Size(758, 504)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lstCheckedSeries As System.Windows.Forms.ListBox
    Friend WithEvents lblCheckedSeries As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtSelectedID As System.Windows.Forms.TextBox
    Friend WithEvents btnSaveEdits As System.Windows.Forms.Button

End Class
