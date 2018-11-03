Namespace Controls

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ColorSettingsDialog
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ColorSettingsDialog))
            Me.lblCurrentLineColor = New System.Windows.Forms.Label()
            Me.lblCurrentPointColor = New System.Windows.Forms.Label()
            Me.btnApply = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.btnSetPointColor = New System.Windows.Forms.Button()
            Me.btnSetLineColor = New System.Windows.Forms.Button()
            Me.lblAutoColor = New System.Windows.Forms.Label()
            Me.lblLine = New System.Windows.Forms.Label()
            Me.lblPoint = New System.Windows.Forms.Label()
            Me.lblCustom = New System.Windows.Forms.Label()
            Me.ColorPalettePoint = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.ColorPaletteLine = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.ColorPaletteCustom = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.ColorPalette10 = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.ColorPalette9 = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.ColorPalette8 = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.ColorPalette7 = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.ColorPalette6 = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.ColorPalette5 = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.ColorPalette4 = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.ColorPalette3 = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.ColorPalette2 = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.ColorPalette1 = New HydroDesktop.Plugins.GraphView.ColorPalette()
            Me.SuspendLayout()
            '
            'lblCurrentLineColor
            '
            Me.lblCurrentLineColor.Location = New System.Drawing.Point(523, 254)
            Me.lblCurrentLineColor.Name = "lblCurrentLineColor"
            Me.lblCurrentLineColor.Size = New System.Drawing.Size(75, 27)
            Me.lblCurrentLineColor.TabIndex = 24
            Me.lblCurrentLineColor.Text = "Current Line Color Set:"
            '
            'lblCurrentPointColor
            '
            Me.lblCurrentPointColor.Location = New System.Drawing.Point(624, 254)
            Me.lblCurrentPointColor.Name = "lblCurrentPointColor"
            Me.lblCurrentPointColor.Size = New System.Drawing.Size(75, 27)
            Me.lblCurrentPointColor.TabIndex = 25
            Me.lblCurrentPointColor.Text = "Current Point Color Set:"
            '
            'btnApply
            '
            Me.btnApply.Location = New System.Drawing.Point(523, 443)
            Me.btnApply.Name = "btnApply"
            Me.btnApply.Size = New System.Drawing.Size(176, 23)
            Me.btnApply.TabIndex = 26
            Me.btnApply.Text = "Apply Changes"
            Me.btnApply.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            Me.btnCancel.Location = New System.Drawing.Point(523, 472)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(176, 23)
            Me.btnCancel.TabIndex = 27
            Me.btnCancel.Text = "Close"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'btnSetPointColor
            '
            Me.btnSetPointColor.BackColor = System.Drawing.Color.Black
            Me.btnSetPointColor.Location = New System.Drawing.Point(374, 375)
            Me.btnSetPointColor.Name = "btnSetPointColor"
            Me.btnSetPointColor.Size = New System.Drawing.Size(18, 18)
            Me.btnSetPointColor.TabIndex = 28
            Me.btnSetPointColor.UseVisualStyleBackColor = False
            '
            'btnSetLineColor
            '
            Me.btnSetLineColor.BackColor = System.Drawing.Color.Black
            Me.btnSetLineColor.Location = New System.Drawing.Point(374, 318)
            Me.btnSetLineColor.Name = "btnSetLineColor"
            Me.btnSetLineColor.Size = New System.Drawing.Size(18, 18)
            Me.btnSetLineColor.TabIndex = 29
            Me.btnSetLineColor.UseVisualStyleBackColor = False
            '
            'lblAutoColor
            '
            Me.lblAutoColor.Location = New System.Drawing.Point(314, 254)
            Me.lblAutoColor.Name = "lblAutoColor"
            Me.lblAutoColor.Size = New System.Drawing.Size(78, 27)
            Me.lblAutoColor.TabIndex = 30
            Me.lblAutoColor.Text = "Auto Color Generator:"
            '
            'lblLine
            '
            Me.lblLine.AutoSize = True
            Me.lblLine.Location = New System.Drawing.Point(317, 321)
            Me.lblLine.Name = "lblLine"
            Me.lblLine.Size = New System.Drawing.Size(30, 13)
            Me.lblLine.TabIndex = 31
            Me.lblLine.Text = "Line:"
            '
            'lblPoint
            '
            Me.lblPoint.AutoSize = True
            Me.lblPoint.Location = New System.Drawing.Point(317, 378)
            Me.lblPoint.Name = "lblPoint"
            Me.lblPoint.Size = New System.Drawing.Size(34, 13)
            Me.lblPoint.TabIndex = 32
            Me.lblPoint.Text = "Point:"
            '
            'lblCustom
            '
            Me.lblCustom.Location = New System.Drawing.Point(417, 254)
            Me.lblCustom.Name = "lblCustom"
            Me.lblCustom.Size = New System.Drawing.Size(66, 27)
            Me.lblCustom.TabIndex = 35
            Me.lblCustom.Text = "Custom Color Set:"
            '
            'ColorPalettePoint
            '
            Me.ColorPalettePoint.CanChangeColors = False
            Me.ColorPalettePoint.Colors = CType(resources.GetObject("ColorPalettePoint.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPalettePoint.DefaultColorFunc = Nothing
            Me.ColorPalettePoint.Location = New System.Drawing.Point(627, 284)
            Me.ColorPalettePoint.Name = "ColorPalettePoint"
            Me.ColorPalettePoint.ShowButtons = False
            Me.ColorPalettePoint.Size = New System.Drawing.Size(75, 153)
            Me.ColorPalettePoint.TabIndex = 48
            '
            'ColorPaletteLine
            '
            Me.ColorPaletteLine.CanChangeColors = False
            Me.ColorPaletteLine.Colors = CType(resources.GetObject("ColorPaletteLine.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPaletteLine.DefaultColorFunc = Nothing
            Me.ColorPaletteLine.Location = New System.Drawing.Point(526, 284)
            Me.ColorPaletteLine.Name = "ColorPaletteLine"
            Me.ColorPaletteLine.ShowButtons = False
            Me.ColorPaletteLine.Size = New System.Drawing.Size(75, 153)
            Me.ColorPaletteLine.TabIndex = 47
            '
            'ColorPaletteCustom
            '
            Me.ColorPaletteCustom.CanChangeColors = True
            Me.ColorPaletteCustom.Colors = CType(resources.GetObject("ColorPaletteCustom.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPaletteCustom.DefaultColorFunc = Nothing
            Me.ColorPaletteCustom.Location = New System.Drawing.Point(420, 284)
            Me.ColorPaletteCustom.Name = "ColorPaletteCustom"
            Me.ColorPaletteCustom.ShowButtons = True
            Me.ColorPaletteCustom.Size = New System.Drawing.Size(75, 211)
            Me.ColorPaletteCustom.TabIndex = 46
            '
            'ColorPalette10
            '
            Me.ColorPalette10.CanChangeColors = False
            Me.ColorPalette10.Colors = CType(resources.GetObject("ColorPalette10.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPalette10.DefaultColorFunc = Nothing
            Me.ColorPalette10.Location = New System.Drawing.Point(215, 284)
            Me.ColorPalette10.Name = "ColorPalette10"
            Me.ColorPalette10.ShowButtons = True
            Me.ColorPalette10.Size = New System.Drawing.Size(75, 211)
            Me.ColorPalette10.TabIndex = 45
            '
            'ColorPalette9
            '
            Me.ColorPalette9.CanChangeColors = False
            Me.ColorPalette9.Colors = CType(resources.GetObject("ColorPalette9.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPalette9.DefaultColorFunc = Nothing
            Me.ColorPalette9.Location = New System.Drawing.Point(114, 284)
            Me.ColorPalette9.Name = "ColorPalette9"
            Me.ColorPalette9.ShowButtons = True
            Me.ColorPalette9.Size = New System.Drawing.Size(75, 211)
            Me.ColorPalette9.TabIndex = 44
            '
            'ColorPalette8
            '
            Me.ColorPalette8.CanChangeColors = False
            Me.ColorPalette8.Colors = CType(resources.GetObject("ColorPalette8.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPalette8.DefaultColorFunc = Nothing
            Me.ColorPalette8.Location = New System.Drawing.Point(14, 284)
            Me.ColorPalette8.Name = "ColorPalette8"
            Me.ColorPalette8.ShowButtons = True
            Me.ColorPalette8.Size = New System.Drawing.Size(75, 211)
            Me.ColorPalette8.TabIndex = 43
            '
            'ColorPalette7
            '
            Me.ColorPalette7.CanChangeColors = False
            Me.ColorPalette7.Colors = CType(resources.GetObject("ColorPalette7.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPalette7.DefaultColorFunc = Nothing
            Me.ColorPalette7.Location = New System.Drawing.Point(624, 21)
            Me.ColorPalette7.Name = "ColorPalette7"
            Me.ColorPalette7.ShowButtons = True
            Me.ColorPalette7.Size = New System.Drawing.Size(75, 211)
            Me.ColorPalette7.TabIndex = 42
            '
            'ColorPalette6
            '
            Me.ColorPalette6.CanChangeColors = False
            Me.ColorPalette6.Colors = CType(resources.GetObject("ColorPalette6.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPalette6.DefaultColorFunc = Nothing
            Me.ColorPalette6.Location = New System.Drawing.Point(523, 21)
            Me.ColorPalette6.Name = "ColorPalette6"
            Me.ColorPalette6.ShowButtons = True
            Me.ColorPalette6.Size = New System.Drawing.Size(75, 211)
            Me.ColorPalette6.TabIndex = 41
            '
            'ColorPalette5
            '
            Me.ColorPalette5.CanChangeColors = False
            Me.ColorPalette5.Colors = CType(resources.GetObject("ColorPalette5.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPalette5.DefaultColorFunc = Nothing
            Me.ColorPalette5.Location = New System.Drawing.Point(420, 21)
            Me.ColorPalette5.Name = "ColorPalette5"
            Me.ColorPalette5.ShowButtons = True
            Me.ColorPalette5.Size = New System.Drawing.Size(75, 211)
            Me.ColorPalette5.TabIndex = 40
            '
            'ColorPalette4
            '
            Me.ColorPalette4.CanChangeColors = False
            Me.ColorPalette4.Colors = CType(resources.GetObject("ColorPalette4.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPalette4.DefaultColorFunc = Nothing
            Me.ColorPalette4.Location = New System.Drawing.Point(317, 21)
            Me.ColorPalette4.Name = "ColorPalette4"
            Me.ColorPalette4.ShowButtons = True
            Me.ColorPalette4.Size = New System.Drawing.Size(75, 211)
            Me.ColorPalette4.TabIndex = 39
            '
            'ColorPalette3
            '
            Me.ColorPalette3.CanChangeColors = False
            Me.ColorPalette3.Colors = CType(resources.GetObject("ColorPalette3.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPalette3.DefaultColorFunc = Nothing
            Me.ColorPalette3.Location = New System.Drawing.Point(215, 21)
            Me.ColorPalette3.Name = "ColorPalette3"
            Me.ColorPalette3.ShowButtons = True
            Me.ColorPalette3.Size = New System.Drawing.Size(75, 211)
            Me.ColorPalette3.TabIndex = 38
            '
            'ColorPalette2
            '
            Me.ColorPalette2.CanChangeColors = False
            Me.ColorPalette2.Colors = CType(resources.GetObject("ColorPalette2.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPalette2.DefaultColorFunc = Nothing
            Me.ColorPalette2.Location = New System.Drawing.Point(114, 21)
            Me.ColorPalette2.Name = "ColorPalette2"
            Me.ColorPalette2.ShowButtons = True
            Me.ColorPalette2.Size = New System.Drawing.Size(75, 211)
            Me.ColorPalette2.TabIndex = 37
            '
            'ColorPalette1
            '
            Me.ColorPalette1.CanChangeColors = False
            Me.ColorPalette1.Colors = CType(resources.GetObject("ColorPalette1.Colors"), System.Collections.Generic.IList(Of System.Drawing.Color))
            Me.ColorPalette1.DefaultColorFunc = Nothing
            Me.ColorPalette1.Location = New System.Drawing.Point(14, 21)
            Me.ColorPalette1.Name = "ColorPalette1"
            Me.ColorPalette1.ShowButtons = True
            Me.ColorPalette1.Size = New System.Drawing.Size(75, 211)
            Me.ColorPalette1.TabIndex = 36
            '
            'ColorSettingsDialog
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(727, 525)
            Me.Controls.Add(Me.ColorPalettePoint)
            Me.Controls.Add(Me.ColorPaletteLine)
            Me.Controls.Add(Me.ColorPaletteCustom)
            Me.Controls.Add(Me.ColorPalette10)
            Me.Controls.Add(Me.ColorPalette9)
            Me.Controls.Add(Me.ColorPalette8)
            Me.Controls.Add(Me.ColorPalette7)
            Me.Controls.Add(Me.ColorPalette6)
            Me.Controls.Add(Me.ColorPalette5)
            Me.Controls.Add(Me.ColorPalette4)
            Me.Controls.Add(Me.ColorPalette3)
            Me.Controls.Add(Me.ColorPalette2)
            Me.Controls.Add(Me.ColorPalette1)
            Me.Controls.Add(Me.lblCustom)
            Me.Controls.Add(Me.lblPoint)
            Me.Controls.Add(Me.lblLine)
            Me.Controls.Add(Me.lblAutoColor)
            Me.Controls.Add(Me.btnSetLineColor)
            Me.Controls.Add(Me.btnSetPointColor)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnApply)
            Me.Controls.Add(Me.lblCurrentPointColor)
            Me.Controls.Add(Me.lblCurrentLineColor)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ColorSettingsDialog"
            Me.Text = "Color Setting"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents lblCurrentLineColor As System.Windows.Forms.Label
        Friend WithEvents lblCurrentPointColor As System.Windows.Forms.Label
        Friend WithEvents btnApply As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents btnSetPointColor As System.Windows.Forms.Button
        Friend WithEvents btnSetLineColor As System.Windows.Forms.Button
        Friend WithEvents lblAutoColor As System.Windows.Forms.Label
        Friend WithEvents lblLine As System.Windows.Forms.Label
        Friend WithEvents lblPoint As System.Windows.Forms.Label
        Friend WithEvents lblCustom As System.Windows.Forms.Label
        Friend WithEvents ColorPalette1 As HydroDesktop.Plugins.GraphView.ColorPalette
        Friend WithEvents ColorPalette2 As HydroDesktop.Plugins.GraphView.ColorPalette
        Friend WithEvents ColorPalette3 As HydroDesktop.Plugins.GraphView.ColorPalette
        Friend WithEvents ColorPalette4 As HydroDesktop.Plugins.GraphView.ColorPalette
        Friend WithEvents ColorPalette5 As HydroDesktop.Plugins.GraphView.ColorPalette
        Friend WithEvents ColorPalette6 As HydroDesktop.Plugins.GraphView.ColorPalette
        Friend WithEvents ColorPalette7 As HydroDesktop.Plugins.GraphView.ColorPalette
        Friend WithEvents ColorPalette8 As HydroDesktop.Plugins.GraphView.ColorPalette
        Friend WithEvents ColorPalette9 As HydroDesktop.Plugins.GraphView.ColorPalette
        Friend WithEvents ColorPalette10 As HydroDesktop.Plugins.GraphView.ColorPalette
        Friend WithEvents ColorPaletteCustom As HydroDesktop.Plugins.GraphView.ColorPalette
        Friend WithEvents ColorPaletteLine As HydroDesktop.Plugins.GraphView.ColorPalette
        Friend WithEvents ColorPalettePoint As HydroDesktop.Plugins.GraphView.ColorPalette
        'Friend WithEvents CTSA1 As cTSA
    End Class
End Namespace