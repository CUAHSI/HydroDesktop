Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class DataSummary
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
            Me.dgvStatSummary = New System.Windows.Forms.DataGridView
            Me.stat1 = New System.Windows.Forms.DataGridViewTextBoxColumn
            Me.stat2 = New System.Windows.Forms.DataGridViewTextBoxColumn
            CType(Me.dgvStatSummary, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'dgvStatSummary
            '
            Me.dgvStatSummary.AllowUserToAddRows = False
            Me.dgvStatSummary.AllowUserToDeleteRows = False
            Me.dgvStatSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dgvStatSummary.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.stat1, Me.stat2})
            Me.dgvStatSummary.Dock = System.Windows.Forms.DockStyle.Fill
            Me.dgvStatSummary.Location = New System.Drawing.Point(0, 0)
            Me.dgvStatSummary.Name = "dgvStatSummary"
            Me.dgvStatSummary.ReadOnly = True
            Me.dgvStatSummary.Size = New System.Drawing.Size(770, 324)
            Me.dgvStatSummary.TabIndex = 0
            '
            'stat1
            '
            Me.stat1.HeaderText = ""
            Me.stat1.Name = "stat1"
            Me.stat1.ReadOnly = True
            '
            'stat2
            '
            Me.stat2.HeaderText = ""
            Me.stat2.Name = "stat2"
            Me.stat2.ReadOnly = True
            Me.stat2.Width = 105
            '
            'cDataSummary
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.dgvStatSummary)
            Me.Name = "cDataSummary"
            Me.Size = New System.Drawing.Size(770, 324)
            CType(Me.dgvStatSummary, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents dgvStatSummary As System.Windows.Forms.DataGridView
        Friend WithEvents stat1 As System.Windows.Forms.DataGridViewTextBoxColumn
        Friend WithEvents stat2 As System.Windows.Forms.DataGridViewTextBoxColumn

    End Class
End Namespace