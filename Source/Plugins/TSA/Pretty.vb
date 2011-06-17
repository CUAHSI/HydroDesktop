Public Class Pretty
    Shared Function PrettyP(ByVal min As Double, ByVal max As Double, Optional ByVal interval As Integer = 6, Optional ByVal buffer As Double = 0.01)

        Dim range As Double = max - min
        max = max + range * buffer
        If (min >= 0) And (min - range * buffer < 0) Then
            min = 0
        Else
            min = min - range * buffer
        End If

        Dim shrink_sml As Double = 0.25
        Dim high_u_fact() As Double = {0.8, 1.7}
        Dim eps_correction As Integer = 2
        Dim return_bounds As Integer = 0
        Dim dx, cell, unit, base, U, ns, nu As Double
        Dim k As Integer, min_n As Integer = 1
        Dim i_small As Boolean
        Dim list1 As List(Of Double) = New List(Of Double)


        Const rounding_eps As Double = 0.0000001
        Const dbl_epsilon As Double = Double.Epsilon
        Const dbl_max As Double = Double.MaxValue
        Const dbl_min As Double = Double.MinValue

        'calculating the range
        dx = max - min

        'cell is the scale
        If (dx = 0 And max = 0) Then

            cell = 1
            i_small = True

        Else
            cell = fmax(Math.Abs(min), Math.Abs(max))

            ' U is upper bound
            If (high_u_fact(1) >= (1.5 * high_u_fact(0) + 0.5)) Then

                U = 1 + (1 / (1 + high_u_fact(0)))
            Else
                U = 1 + (1.5 / (1 + high_u_fact(1)))
            End If

            i_small = dx < cell * U * imax(1, interval) * dbl_epsilon * 3

        End If

        If (i_small) Then
            If (cell > 10) Then
                cell = 9 + cell / 10
                cell *= shrink_sml
            End If

            If (min_n > 1) Then
                cell /= min_n
            End If
        Else
            cell = dx

            If (interval > 1) Then
                cell /= interval
            End If

        End If

        If (cell < 20 * dbl_min) Then
            cell = 20 * dbl_min

        ElseIf (cell * 10 > dbl_max) Then
            cell = 0.1 * dbl_max
        End If

        base = Math.Pow(10.0, Math.Floor(Math.Log10(cell)))

        unit = base
        U = 2 * base
        If (U - cell < high_u_fact(0) * (cell - unit)) Then
            unit = U
            U = 4 * base
            If (U - cell < high_u_fact(0) * (cell - unit)) Then
                unit = U
                U = 5 * base
                If (U - cell < high_u_fact(0) * (cell - unit)) Then
                    unit = U
                    U = 6 * base
                    If (U - cell < high_u_fact(0) * (cell - unit)) Then
                        unit = U
                        U = 8 * base
                        If (U - cell < high_u_fact(0) * (cell - unit)) Then
                            unit = U
                            U = 10 * base
                            If (U - cell < high_u_fact(0) * (cell - unit)) Then
                                unit = U
                            End If
                        End If
                    End If
                End If
            End If
        End If

        If unit = 0 Then
            unit = 1
        End If

        ns = Math.Floor(min / unit + rounding_eps)
        nu = Math.Ceiling(max / unit - rounding_eps)

        If (eps_correction And (eps_correction > 1 Or Not (i_small))) Then

            If (min) Then
                min *= (1 - dbl_epsilon)
            Else
                min = -dbl_min
            End If

            If (max) Then
                max *= (1 + dbl_epsilon)
            Else
                max = +dbl_min
            End If

        End If

        While (ns * unit > min + rounding_eps * unit)
            ns -= 1
        End While

        While (nu * unit < max - rounding_eps * unit)
            nu += 1
        End While

        k = 0.5 + nu - ns

        If (k < min_n) Then

            k = min_n - k

            If (ns >= 0) Then
                nu += k / 2
                ns -= k / 2 + k Mod 2
            Else
                ns -= k / 2
                nu += k / 2 + k Mod 2
            End If
            interval = min_n
        Else
            interval = k
        End If

        If (return_bounds) Then
            If (ns * unit < min) Then
                min = ns * unit
            End If

            If (nu * unit < max) Then
                max = nu * unit
            End If
        Else
            min = ns
            max = nu
        End If

        min = ns * unit
        max = nu * unit

        If (nu >= ns + 1) Then
            If (ns * unit < min - rounding_eps * unit) Then
                ns += 1
            End If
            If (nu > ns + 1 And nu * unit > max + rounding_eps * unit) Then
                nu -= 1
            End If
            interval = nu - ns
        End If

        list1.Add(min)
        list1.Add(max)
        list1.Add(unit)
        Return list1
    End Function

    Shared Function fmax(ByVal lo As Double, ByVal hi As Double)

        If (lo < hi) Then
            Return hi
        Else
            Return lo
        End If

    End Function

    Shared Function imax(ByVal lo As Integer, ByVal hi As Integer)

        If (lo < hi) Then
            Return hi
        Else
            Return lo
        End If

    End Function
End Class
