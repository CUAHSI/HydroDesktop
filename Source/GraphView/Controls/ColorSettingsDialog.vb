Imports System.Drawing

Namespace Controls

    Public Class ColorSettingsDialog

        Private ReadOnly ccList0 As New List(Of Color)
        Private ReadOnly ccList1 As New List(Of Color)
        Private ReadOnly ccList2 As New List(Of Color)
        Private ReadOnly ccList3 As New List(Of Color)
        Private ReadOnly ccList4 As New List(Of Color)
        Private ReadOnly ccList5 As New List(Of Color)
        Private ReadOnly ccList6 As New List(Of Color)
        Private ReadOnly ccList7 As New List(Of Color)
        Private ReadOnly ccList8 As New List(Of Color)
        Private ReadOnly ccList9 As New List(Of Color)

        Private ReadOnly _linecolorlist As New List(Of Color)
        Private ReadOnly _pointcolorlist As New List(Of Color)

        Private Property linecolorlist() As List(Of Color)
            Get
                Return _linecolorlist
            End Get
            Set(ByVal value As List(Of Color))
                lblslc0.BackColor = value(0)
                lblslc1.BackColor = value(1)
                lblslc2.BackColor = value(2)
                lblslc3.BackColor = value(3)
                lblslc4.BackColor = value(4)
                lblslc5.BackColor = value(5)
                lblslc6.BackColor = value(6)
                lblslc7.BackColor = value(7)
                lblslc8.BackColor = value(8)
                lblslc9.BackColor = value(9)

                _linecolorlist.Clear()

                For i As Integer = 0 To 9
                    _linecolorlist.Add(value(i))
                Next

            End Set
        End Property

        Private Property pointcolorlist() As List(Of Color)
            Get
                Return _pointcolorlist
            End Get
            Set(ByVal value As List(Of Color))
                lblspc0.BackColor = value(0)
                lblspc1.BackColor = value(1)
                lblspc2.BackColor = value(2)
                lblspc3.BackColor = value(3)
                lblspc4.BackColor = value(4)
                lblspc5.BackColor = value(5)
                lblspc6.BackColor = value(6)
                lblspc7.BackColor = value(7)
                lblspc8.BackColor = value(8)
                lblspc9.BackColor = value(9)

                _pointcolorlist.Clear()

                For i As Integer = 0 To 9
                    _pointcolorlist.Add(value(i))
                Next

            End Set
        End Property

        Public Sub New(ByVal _lcolorlist As List(Of Color), ByVal _pcolorlist As List(Of Color))
            InitializeComponent()

            linecolorlist = _lcolorlist
            pointcolorlist = _pcolorlist

            ccList0.Clear()
            ccList0.Add(Color.FromArgb(106, 61, 154))
            ccList0.Add(Color.FromArgb(202, 178, 214))
            ccList0.Add(Color.FromArgb(255, 127, 0))
            ccList0.Add(Color.FromArgb(253, 191, 111))
            ccList0.Add(Color.FromArgb(227, 26, 28))
            ccList0.Add(Color.FromArgb(251, 154, 153))
            ccList0.Add(Color.FromArgb(51, 160, 44))
            ccList0.Add(Color.FromArgb(178, 223, 138))
            ccList0.Add(Color.FromArgb(31, 120, 180))
            ccList0.Add(Color.FromArgb(166, 206, 227))

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


            lblc10.BackColor = ccList1(0)
            lblc11.BackColor = ccList1(1)
            lblc12.BackColor = ccList1(2)
            lblc13.BackColor = ccList1(3)
            lblc14.BackColor = ccList1(4)
            lblc15.BackColor = ccList1(5)
            lblc16.BackColor = ccList1(6)
            lblc17.BackColor = ccList1(7)
            lblc18.BackColor = ccList1(8)
            lblc19.BackColor = ccList1(9)

            lblc20.BackColor = ccList2(0)
            lblc21.BackColor = ccList2(1)
            lblc22.BackColor = ccList2(2)
            lblc23.BackColor = ccList2(3)
            lblc24.BackColor = ccList2(4)
            lblc25.BackColor = ccList2(5)
            lblc26.BackColor = ccList2(6)
            lblc27.BackColor = ccList2(7)
            lblc28.BackColor = ccList2(8)
            lblc29.BackColor = ccList2(9)

            lblc30.BackColor = ccList3(0)
            lblc31.BackColor = ccList3(1)
            lblc32.BackColor = ccList3(2)
            lblc33.BackColor = ccList3(3)
            lblc34.BackColor = ccList3(4)
            lblc35.BackColor = ccList3(5)
            lblc36.BackColor = ccList3(6)
            lblc37.BackColor = ccList3(7)
            lblc38.BackColor = ccList3(8)
            lblc39.BackColor = ccList3(9)

            lblc40.BackColor = ccList4(0)
            lblc41.BackColor = ccList4(1)
            lblc42.BackColor = ccList4(2)
            lblc43.BackColor = ccList4(3)
            lblc44.BackColor = ccList4(4)
            lblc45.BackColor = ccList4(5)
            lblc46.BackColor = ccList4(6)
            lblc47.BackColor = ccList4(7)
            lblc48.BackColor = ccList4(8)
            lblc49.BackColor = ccList4(9)

            lblc50.BackColor = ccList5(0)
            lblc51.BackColor = ccList5(1)
            lblc52.BackColor = ccList5(2)
            lblc53.BackColor = ccList5(3)
            lblc54.BackColor = ccList5(4)
            lblc55.BackColor = ccList5(5)
            lblc56.BackColor = ccList5(6)
            lblc57.BackColor = ccList5(7)
            lblc58.BackColor = ccList5(8)
            lblc59.BackColor = ccList5(9)

            lblc60.BackColor = ccList6(0)
            lblc61.BackColor = ccList6(1)
            lblc62.BackColor = ccList6(2)
            lblc63.BackColor = ccList6(3)
            lblc64.BackColor = ccList6(4)
            lblc65.BackColor = ccList6(5)
            lblc66.BackColor = ccList6(6)
            lblc67.BackColor = ccList6(7)
            lblc68.BackColor = ccList6(8)
            lblc69.BackColor = ccList6(9)

            lblc70.BackColor = ccList7(0)
            lblc71.BackColor = ccList7(1)
            lblc72.BackColor = ccList7(2)
            lblc73.BackColor = ccList7(3)
            lblc74.BackColor = ccList7(4)
            lblc75.BackColor = ccList7(5)
            lblc76.BackColor = ccList7(6)
            lblc77.BackColor = ccList7(7)
            lblc78.BackColor = ccList7(8)
            lblc79.BackColor = ccList7(9)

            lblc80.BackColor = ccList8(0)
            lblc81.BackColor = ccList8(1)
            lblc82.BackColor = ccList8(2)
            lblc83.BackColor = ccList8(3)
            lblc84.BackColor = ccList8(4)
            lblc85.BackColor = ccList8(5)
            lblc86.BackColor = ccList8(6)
            lblc87.BackColor = ccList8(7)
            lblc88.BackColor = ccList8(8)
            lblc89.BackColor = ccList8(9)

            lblc90.BackColor = ccList9(0)
            lblc91.BackColor = ccList9(1)
            lblc92.BackColor = ccList9(2)
            lblc93.BackColor = ccList9(3)
            lblc94.BackColor = ccList9(4)
            lblc95.BackColor = ccList9(5)
            lblc96.BackColor = ccList9(6)
            lblc97.BackColor = ccList9(7)
            lblc98.BackColor = ccList9(8)
            lblc99.BackColor = ccList9(9)

            lblc00.BackColor = ccList0(0)
            lblc01.BackColor = ccList0(1)
            lblc02.BackColor = ccList0(2)
            lblc03.BackColor = ccList0(3)
            lblc04.BackColor = ccList0(4)
            lblc05.BackColor = ccList0(5)
            lblc06.BackColor = ccList0(6)
            lblc07.BackColor = ccList0(7)
            lblc08.BackColor = ccList0(8)
            lblc09.BackColor = ccList0(9)

            'ccList2.Add(Color.FromArgb())
            'ccList2.Add(Color.FromArgb())
            'ccList2.Add(Color.FromArgb())
            'ccList2.Add(Color.FromArgb())
            'ccList2.Add(Color.FromArgb())
            'ccList2.Add(Color.FromArgb())
            'ccList2.Add(Color.FromArgb())
            'ccList2.Add(Color.FromArgb())
            'ccList2.Add(Color.FromArgb())
            'ccList2.Add(Color.FromArgb())


        End Sub

        'Private Sub Leaving() Handles Me.Deactivate
        '    Me.Close()

        'End Sub

        'Color Collection 1
        Private Sub btnlinec1_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnlinec1.Click
            linecolorlist = ccList1
        End Sub
        Private Sub btnpointc1_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnpointc1.Click
            pointcolorlist = ccList1
        End Sub
        Private Sub btnbothc1_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnbothc1.Click
            pointcolorlist = ccList1
            linecolorlist = ccList1
        End Sub
        'Color Collection 2
        Private Sub btnlinec2_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnlinec2.Click
            linecolorlist = ccList2
        End Sub
        Private Sub btnpointc2_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnpointc2.Click
            pointcolorlist = ccList2
        End Sub
        Private Sub btnbothc2_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnbothc2.Click
            pointcolorlist = ccList2
            linecolorlist = ccList2
        End Sub
        'Color Collection 3
        Private Sub btnlinec3_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnlinec3.Click
            linecolorlist = ccList3
        End Sub
        Private Sub btnpointc3_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnpointc3.Click
            pointcolorlist = ccList3
        End Sub
        Private Sub btnbothc3_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnbothc3.Click
            pointcolorlist = ccList3
            linecolorlist = ccList3
        End Sub
        'Color Collection 4
        Private Sub btnlinec4_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnlinec4.Click
            linecolorlist = ccList4
        End Sub
        Private Sub btnpointc4_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnpointc4.Click
            pointcolorlist = ccList4
        End Sub
        Private Sub btnbothc4_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnbothc4.Click
            pointcolorlist = ccList4
            linecolorlist = ccList4
        End Sub
        'Color Collection 5
        Private Sub btnlinec5_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnlinec5.Click
            linecolorlist = ccList5
        End Sub
        Private Sub btnpointc5_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnpointc5.Click
            pointcolorlist = ccList5
        End Sub
        Private Sub btnbothc5_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnbothc5.Click
            pointcolorlist = ccList5
            linecolorlist = ccList5
        End Sub
        'Color Collection 6
        Private Sub btnlinec6_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnlinec6.Click
            linecolorlist = ccList6
        End Sub
        Private Sub btnpointc6_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnpointc6.Click
            pointcolorlist = ccList6
        End Sub
        Private Sub btnbothc6_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnbothc6.Click
            pointcolorlist = ccList6
            linecolorlist = ccList6
        End Sub
        'Color Collection 7
        Private Sub btnlinec7_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnlinec7.Click
            linecolorlist = ccList7
        End Sub
        Private Sub btnpointc7_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnpointc7.Click
            pointcolorlist = ccList7
        End Sub
        Private Sub btnbothc7_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnbothc7.Click
            pointcolorlist = ccList7
            linecolorlist = ccList7
        End Sub
        'Color Collection 8
        Private Sub btnlinec8_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnlinec8.Click
            linecolorlist = ccList8
        End Sub
        Private Sub btnpointc8_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnpointc8.Click
            pointcolorlist = ccList8
        End Sub
        Private Sub btnbothc8_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnbothc8.Click
            pointcolorlist = ccList8
            linecolorlist = ccList8
        End Sub
        'Color Collection 9
        Private Sub btnlinec9_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnlinec9.Click
            linecolorlist = ccList9
        End Sub
        Private Sub btnpointc9_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnpointc9.Click
            pointcolorlist = ccList9
        End Sub
        Private Sub btnbothc9_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnbothc9.Click
            pointcolorlist = ccList9
            linecolorlist = ccList9
        End Sub
        'Color Collection 0
        Private Sub btnlinec0_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnlinec0.Click
            linecolorlist = ccList0
        End Sub
        Private Sub btnpointc0_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnpointc0.Click
            pointcolorlist = ccList0
        End Sub
        Private Sub btnbothc0_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnbothc0.Click
            pointcolorlist = ccList0
            linecolorlist = ccList0
        End Sub
        'Custom Color Collection
        Private Sub btnlinecc_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnlinecc.Click
            Dim cclist As New List(Of Color)

            cclist.Add(lblcc0.BackColor)
            cclist.Add(lblcc1.BackColor)
            cclist.Add(lblcc2.BackColor)
            cclist.Add(lblcc3.BackColor)
            cclist.Add(lblcc4.BackColor)
            cclist.Add(lblcc5.BackColor)
            cclist.Add(lblcc6.BackColor)
            cclist.Add(lblcc7.BackColor)
            cclist.Add(lblcc8.BackColor)
            cclist.Add(lblcc9.BackColor)

            linecolorlist = cclist
        End Sub
        Private Sub btnpointcc_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnpointcc.Click
            Dim cclist As New List(Of Color)

            cclist.Add(lblcc0.BackColor)
            cclist.Add(lblcc1.BackColor)
            cclist.Add(lblcc2.BackColor)
            cclist.Add(lblcc3.BackColor)
            cclist.Add(lblcc4.BackColor)
            cclist.Add(lblcc5.BackColor)
            cclist.Add(lblcc6.BackColor)
            cclist.Add(lblcc7.BackColor)
            cclist.Add(lblcc8.BackColor)
            cclist.Add(lblcc9.BackColor)

            pointcolorlist = cclist
        End Sub
        Private Sub btnbothcc_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnbothcc.Click
            Dim cclist As New List(Of Color)

            cclist.Add(lblcc0.BackColor)
            cclist.Add(lblcc1.BackColor)
            cclist.Add(lblcc2.BackColor)
            cclist.Add(lblcc3.BackColor)
            cclist.Add(lblcc4.BackColor)
            cclist.Add(lblcc5.BackColor)
            cclist.Add(lblcc6.BackColor)
            cclist.Add(lblcc7.BackColor)
            cclist.Add(lblcc8.BackColor)
            cclist.Add(lblcc9.BackColor)

            pointcolorlist = cclist
            linecolorlist = cclist
        End Sub


        Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnCancel.Click
            Close()
        End Sub


        Private Sub btnApply_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnApply.Click

            _CTSA.pointcolorlist.Clear()
            _CTSA.linecolorlist.Clear()
            For i As Integer = 0 To 9
                _CTSA.pointcolorlist.Add(pointcolorlist(i))
            Next
            For i As Integer = 0 To 9
                _CTSA.linecolorlist.Add(linecolorlist(i))
            Next

            _CTSA.ApplyOptions()

        End Sub

        Private Function PromptForColor(ByVal defaultColor As Color) As Color
            Dim dlgColor As Windows.Forms.ColorDialog = New Windows.Forms.ColorDialog()

            If Not IsDBNull(defaultColor) Then
                dlgColor.Color = defaultColor
            End If

            If (dlgColor.ShowDialog() = Windows.Forms.DialogResult.OK) Then
                Return dlgColor.Color
            Else
                Return Nothing
            End If
        End Function

        Private Sub btnSetLineColor_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btnSetLineColor.Click
            Dim newColor As Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                btnSetLineColor.BackColor = newColor
            End If

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
            Dim newColor As Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                btnSetPointColor.BackColor = newColor
            End If

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

        Private Sub lblcc0_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lblcc0.Click
            Dim newColor As Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                lblcc0.BackColor = newColor
            End If
        End Sub
        Private Sub lblcc1_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lblcc1.Click
            Dim newColor As Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                lblcc1.BackColor = newColor
            End If
        End Sub
        Private Sub lblcc2_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lblcc2.Click
            Dim newColor As Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                lblcc2.BackColor = newColor
            End If
        End Sub
        Private Sub lblcc3_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lblcc3.Click
            Dim newColor As Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                lblcc3.BackColor = newColor
            End If
        End Sub
        Private Sub lblcc4_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lblcc4.Click
            Dim newColor As Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                lblcc4.BackColor = newColor
            End If
        End Sub
        Private Sub lblcc5_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lblcc5.Click
            Dim newColor As Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                lblcc5.BackColor = newColor
            End If
        End Sub
        Private Sub lblcc6_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lblcc6.Click
            Dim newColor As Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                lblcc6.BackColor = newColor
            End If
        End Sub
        Private Sub lblcc7_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lblcc7.Click
            Dim newColor As Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                lblcc7.BackColor = newColor
            End If
        End Sub
        Private Sub lblcc8_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lblcc8.Click
            Dim newColor As Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                lblcc8.BackColor = newColor
            End If
        End Sub
        Private Sub lblcc9_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lblcc9.Click
            Dim newColor As Color = PromptForColor(btnSetPointColor.BackColor)

            If Not IsDBNull(newColor) Then
                lblcc9.BackColor = newColor
            End If
        End Sub
    End Class
End Namespace