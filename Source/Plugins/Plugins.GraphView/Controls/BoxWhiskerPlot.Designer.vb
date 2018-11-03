Imports HydroDesktop.ZedGraphEx

Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class BoxWhiskerPlot
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
            Me.components = New System.ComponentModel.Container
            Me.zgBoxWhiskerPlot = New ZedGraphControlEx
            Me.SuspendLayout()
            '
            'zgBoxWhiskerPlot
            '
            Me.zgBoxWhiskerPlot.AutoScroll = True
            Me.zgBoxWhiskerPlot.Dock = System.Windows.Forms.DockStyle.Fill
            Me.zgBoxWhiskerPlot.Location = New System.Drawing.Point(0, 0)
            Me.zgBoxWhiskerPlot.Name = "zgBoxWhiskerPlot"
            Me.zgBoxWhiskerPlot.ScrollMaxX = 0
            Me.zgBoxWhiskerPlot.ScrollMaxY = 0
            Me.zgBoxWhiskerPlot.ScrollMaxY2 = 0
            Me.zgBoxWhiskerPlot.ScrollMinX = 0
            Me.zgBoxWhiskerPlot.ScrollMinY = 0
            Me.zgBoxWhiskerPlot.ScrollMinY2 = 0
            Me.zgBoxWhiskerPlot.Size = New System.Drawing.Size(200, 200)
            Me.zgBoxWhiskerPlot.TabIndex = 0
            '
            'cBoxWhiskerPlot
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.zgBoxWhiskerPlot)
            Me.Name = "cBoxWhiskerPlot"
            Me.Size = New System.Drawing.Size(200, 200)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents zgBoxWhiskerPlot As ZedGraphControlEx

    End Class
End Namespace