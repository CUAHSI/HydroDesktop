Imports System.Drawing
Imports HydroDesktop.Common.Tools

Namespace Controls

    Public Class ColorSettingsDialog

        Public Sub New(ByVal _lcolorlist As List(Of Color), ByVal _pcolorlist As List(Of Color))
            InitializeComponent()

            linecolorlist = _lcolorlist
            pointcolorlist = _pcolorlist

            Dim ccList1 As New List(Of Color)
            ccList1.Add(Color.FromArgb(165, 0, 38))
            ccList1.Add(Color.FromArgb(215, 48, 39))
            ccList1.Add(Color.FromArgb(244, 109, 67))
            ccList1.Add(Color.FromArgb(253, 174, 97))
            ccList1.Add(Color.FromArgb(254, 224, 139))
            ccList1.Add(Color.FromArgb(217, 239, 139))
            ccList1.Add(Color.FromArgb(166, 217, 106))
            ccList1.Add(Color.FromArgb(102, 189, 99))
            ccList1.Add(Color.FromArgb(26, 152, 80))
            ccList1.Add(Color.FromArgb(0, 104, 55))

            Dim ccList2 As New List(Of Color)
            ccList2.Add(Color.FromArgb(158, 1, 66))
            ccList2.Add(Color.FromArgb(213, 62, 79))
            ccList2.Add(Color.FromArgb(244, 109, 67))
            ccList2.Add(Color.FromArgb(253, 174, 97))
            ccList2.Add(Color.FromArgb(254, 224, 139))
            ccList2.Add(Color.FromArgb(230, 245, 152))
            ccList2.Add(Color.FromArgb(171, 221, 164))
            ccList2.Add(Color.FromArgb(102, 194, 165))
            ccList2.Add(Color.FromArgb(50, 136, 189))
            ccList2.Add(Color.FromArgb(94, 79, 162))

            Dim ccList3 As New List(Of Color)
            ccList3.Add(Color.FromArgb(165, 0, 38))
            ccList3.Add(Color.FromArgb(215, 48, 39))
            ccList3.Add(Color.FromArgb(244, 109, 67))
            ccList3.Add(Color.FromArgb(253, 174, 97))
            ccList3.Add(Color.FromArgb(254, 224, 144))
            ccList3.Add(Color.FromArgb(224, 243, 248))
            ccList3.Add(Color.FromArgb(171, 217, 233))
            ccList3.Add(Color.FromArgb(116, 173, 209))
            ccList3.Add(Color.FromArgb(69, 117, 180))
            ccList3.Add(Color.FromArgb(49, 54, 149))

            Dim ccList4 As New List(Of Color)
            ccList4.Add(Color.FromArgb(103, 0, 31))
            ccList4.Add(Color.FromArgb(178, 24, 43))
            ccList4.Add(Color.FromArgb(214, 96, 77))
            ccList4.Add(Color.FromArgb(244, 165, 130))
            ccList4.Add(Color.FromArgb(253, 219, 199))
            ccList4.Add(Color.FromArgb(224, 224, 224))
            ccList4.Add(Color.FromArgb(186, 186, 186))
            ccList4.Add(Color.FromArgb(135, 135, 135))
            ccList4.Add(Color.FromArgb(77, 77, 77))
            ccList4.Add(Color.FromArgb(26, 26, 26))

            Dim ccList5 As New List(Of Color)
            ccList5.Add(Color.FromArgb(103, 0, 31))
            ccList5.Add(Color.FromArgb(178, 24, 43))
            ccList5.Add(Color.FromArgb(214, 96, 77))
            ccList5.Add(Color.FromArgb(244, 165, 130))
            ccList5.Add(Color.FromArgb(253, 219, 199))
            ccList5.Add(Color.FromArgb(209, 229, 240))
            ccList5.Add(Color.FromArgb(146, 197, 222))
            ccList5.Add(Color.FromArgb(67, 147, 195))
            ccList5.Add(Color.FromArgb(33, 102, 172))
            ccList5.Add(Color.FromArgb(5, 48, 97))

            Dim ccList6 As New List(Of Color)
            ccList6.Add(Color.FromArgb(142, 1, 82))
            ccList6.Add(Color.FromArgb(197, 27, 125))
            ccList6.Add(Color.FromArgb(222, 119, 174))
            ccList6.Add(Color.FromArgb(241, 182, 218))
            ccList6.Add(Color.FromArgb(253, 224, 239))
            ccList6.Add(Color.FromArgb(230, 245, 208))
            ccList6.Add(Color.FromArgb(184, 225, 134))
            ccList6.Add(Color.FromArgb(127, 188, 65))
            ccList6.Add(Color.FromArgb(77, 146, 33))
            ccList6.Add(Color.FromArgb(39, 100, 25))

            Dim ccList7 As New List(Of Color)
            ccList7.Add(Color.FromArgb(64, 0, 75))
            ccList7.Add(Color.FromArgb(118, 42, 131))
            ccList7.Add(Color.FromArgb(153, 112, 171))
            ccList7.Add(Color.FromArgb(194, 165, 207))
            ccList7.Add(Color.FromArgb(231, 212, 232))
            ccList7.Add(Color.FromArgb(217, 240, 211))
            ccList7.Add(Color.FromArgb(168, 216, 183))
            ccList7.Add(Color.FromArgb(90, 174, 97))
            ccList7.Add(Color.FromArgb(27, 120, 55))
            ccList7.Add(Color.FromArgb(0, 68, 27))

            Dim ccList8 As New List(Of Color)
            ccList8.Add(Color.FromArgb(84, 48, 5))
            ccList8.Add(Color.FromArgb(140, 81, 10))
            ccList8.Add(Color.FromArgb(191, 129, 45))
            ccList8.Add(Color.FromArgb(223, 194, 125))
            ccList8.Add(Color.FromArgb(246, 232, 195))
            ccList8.Add(Color.FromArgb(199, 234, 229))
            ccList8.Add(Color.FromArgb(128, 205, 193))
            ccList8.Add(Color.FromArgb(53, 151, 143))
            ccList8.Add(Color.FromArgb(1, 102, 94))
            ccList8.Add(Color.FromArgb(0, 60, 48))

            Dim ccList9 As New List(Of Color)
            ccList9.Add(Color.FromArgb(127, 59, 8))
            ccList9.Add(Color.FromArgb(179, 88, 6))
            ccList9.Add(Color.FromArgb(224, 130, 20))
            ccList9.Add(Color.FromArgb(253, 184, 99))
            ccList9.Add(Color.FromArgb(254, 224, 182))
            ccList9.Add(Color.FromArgb(216, 218, 235))
            ccList9.Add(Color.FromArgb(178, 171, 210))
            ccList9.Add(Color.FromArgb(128, 115, 172))
            ccList9.Add(Color.FromArgb(84, 39, 136))
            ccList9.Add(Color.FromArgb(45, 0, 75))

            Dim ccList10 As New List(Of Color)
            ccList10.Add(Color.FromArgb(106, 61, 154))
            ccList10.Add(Color.FromArgb(202, 178, 214))
            ccList10.Add(Color.FromArgb(255, 127, 0))
            ccList10.Add(Color.FromArgb(253, 191, 111))
            ccList10.Add(Color.FromArgb(227, 26, 28))
            ccList10.Add(Color.FromArgb(251, 154, 153))
            ccList10.Add(Color.FromArgb(51, 160, 44))
            ccList10.Add(Color.FromArgb(178, 223, 138))
            ccList10.Add(Color.FromArgb(31, 120, 180))
            ccList10.Add(Color.FromArgb(166, 206, 227))

            InitPalette(ColorPalette1, ccList1)
            InitPalette(ColorPalette2, ccList2)
            InitPalette(ColorPalette3, ccList3)
            InitPalette(ColorPalette4, ccList4)
            InitPalette(ColorPalette5, ccList5)
            InitPalette(ColorPalette6, ccList6)
            InitPalette(ColorPalette7, ccList7)
            InitPalette(ColorPalette8, ccList8)
            InitPalette(ColorPalette9, ccList9)
            InitPalette(ColorPalette10, ccList10)

            ColorPaletteCustom.DefaultColorFunc = Function()
                                                      Return btnSetPointColor.BackColor
                                                  End Function
            InitPalette(ColorPaletteCustom, Nothing)
        End Sub

        Private Sub InitPalette(ByVal palette As ColorPalette, ByVal colors As IList(Of Color))
            palette.Colors = colors
            AddHandler palette.OnLineClicked, AddressOf line_Click
            AddHandler palette.OnPointClicked, AddressOf point_Click
        End Sub

        Public Property linecolorlist() As IList(Of Color)
            Get
                Return ColorPaletteLine.Colors
            End Get
            Private Set(ByVal value As IList(Of Color))
                ColorPaletteLine.Colors = value
            End Set
        End Property

        Public Property pointcolorlist() As List(Of Color)
            Get
                Return ColorPalettePoint.Colors
            End Get
            Private Set(ByVal value As List(Of Color))
                ColorPalettePoint.Colors = value
            End Set
        End Property

        Private Sub line_Click(ByVal sender As System.Object, ByVal e As ColorPaletteButtonEventArgs)
            linecolorlist = e.Colors
        End Sub
        Private Sub point_Click(ByVal sender As System.Object, ByVal e As ColorPaletteButtonEventArgs)
            pointcolorlist = e.Colors
        End Sub

        Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnCancel.Click
            Close()
        End Sub

        Public Event ColorsApplied As EventHandler

        Private Sub btnApply_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnApply.Click
            RaiseEvent ColorsApplied(Me, EventArgs.Empty)
        End Sub

        Private Sub btnSetLineColor_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSetLineColor.Click
            Dim newColor = DrawingHelper.PromptForColor(btnSetLineColor.BackColor)
            If newColor Is Nothing Then Return

            btnSetLineColor.BackColor = newColor.Value
            Dim colorlist As New List(Of Color)
            Dim x As Integer

            x = btnSetLineColor.BackColor.ToArgb()
            For i = 0 To 9
                If x > -18001 Then
                    x = -16777216
                End If
                colorlist.Add(Color.FromArgb(x))
                x += 18000
            Next i

            linecolorlist = colorlist

        End Sub

        Private Sub btnSetPointColor_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSetPointColor.Click
            Dim newColor = DrawingHelper.PromptForColor(btnSetPointColor.BackColor)
            If newColor Is Nothing Then Return

            btnSetPointColor.BackColor = newColor.Value
            Dim colorlist As New List(Of Color)()
            Dim x As Integer

            x = btnSetPointColor.BackColor.ToArgb()
            For i = 0 To 9
                If x > -18001 Then
                    x = -16777216
                End If
                colorlist.Add(Color.FromArgb(x))
                x += 18000
            Next i

            pointcolorlist = colorlist
        End Sub
    End Class
End Namespace