Option Strict On
Public Class Statistics
    Public Const NotCensored As String = "nc"
    Public Const Unknown As String = "unknown"

    Shared Function ArithmeticMean(ByRef objDataTable As Data.DataTable) As Double
        Try
            Return Convert.ToDouble(objDataTable.Compute("Avg(DataValue)", ""))
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.ArithmeticMean" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function GeometricMean(ByRef objDataTable As Data.DataTable) As Double
        Try
            Dim dblTotal As Double = 0
            Dim objDataRow As DataRow
            For Each objDataRow In objDataTable.Rows
                If Convert.ToInt32(objDataRow.Item("DataValue")) > 0 Then
                    dblTotal += Math.Log10(Convert.ToDouble(objDataRow.Item("DataValue")))
                End If
            Next
            Return 10 ^ (dblTotal / Count(objDataTable))
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.GeometricMean" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function Mean(ByRef objDataTable As Data.DataTable) As Double
        Try
            Return ArithmeticMean(objDataTable)
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.Mean" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function Median(ByRef objDataTable As Data.DataTable) As Double
        Try
            If (Count(objDataTable) Mod 2 = 0) Then
                Dim intRow As Integer = Convert.ToInt32(Math.Floor(Count(objDataTable) * 0.5))
                Return (Convert.ToDouble(objDataTable.Rows(intRow).Item("DataValue")) + Convert.ToDouble(objDataTable.Rows(intRow - 1).Item("DataValue"))) / 2
            Else
                Return Percentile(objDataTable, 50)
            End If
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.Median" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function Minimum(ByRef objDataTable As Data.DataTable) As Double
        Try
            Return Convert.ToDouble(objDataTable.Compute("MIN(DataValue)", ""))
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.Minimum" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function Maximum(ByRef objDataTable As Data.DataTable) As Double
        Try
            Return Convert.ToDouble(objDataTable.Compute("MAX(DataValue)", ""))
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.Maximum" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function Range(ByRef objDataTable As Data.DataTable) As Double
        Try
            Return Maximum(objDataTable) - Minimum(objDataTable)
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.Range" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function UpperQuartile(ByRef objDataTable As Data.DataTable) As Double
        Try
            Return Percentile(objDataTable, 75)
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.UpperQuartile" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function LowerQuartile(ByRef objDataTable As Data.DataTable) As Double
        Try
            Return Percentile(objDataTable, 25)
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.LowerQuartile" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function InterquartileRange(ByRef objDataTable As Data.DataTable) As Double
        Try
            Return UpperQuartile(objDataTable) - LowerQuartile(objDataTable)
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.InterquartileRange" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function UpperAdjacent(ByRef objDataTable As Data.DataTable) As Double
        Try
            If (UpperQuartile(objDataTable) + InterquartileRange(objDataTable) * 1.5 > Maximum(objDataTable)) Then
                Return Maximum(objDataTable)
            Else
                Return UpperQuartile(objDataTable) + InterquartileRange(objDataTable) * 1.5
            End If
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.UpperAdjacent" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function LowerAdjacent(ByRef objDataTable As Data.DataTable) As Double
        Try
            If (LowerQuartile(objDataTable) - InterquartileRange(objDataTable) * 1.5 < Minimum(objDataTable)) Then
                Return Minimum(objDataTable)
            Else
                Return LowerQuartile(objDataTable) - InterquartileRange(objDataTable) * 1.5
            End If
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.LowerAdjacent" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function UpperConfidenceLimit(ByRef objDataTable As Data.DataTable) As Double
        Try
            Return Median(objDataTable) + StandardDeviation(objDataTable) / Math.Sqrt(Count(objDataTable))
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.UpperConfidenceLimit" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function LowerConfidenceLimit(ByRef objDataTable As Data.DataTable) As Double
        Try
            Return Median(objDataTable) - StandardDeviation(objDataTable) / Math.Sqrt(Count(objDataTable))
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.LowerConfidenceLimit" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function UpperConfidenceInterval(ByRef objDataTable As Data.DataTable) As Double
        Try
            Return Mean(objDataTable) + StandardDeviation(objDataTable) / Math.Sqrt(Count(objDataTable))
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.UpperConfidenceInterval" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function LowerConfidenceInterval(ByRef objDataTable As Data.DataTable) As Double
        Try
            Return Mean(objDataTable) - StandardDeviation(objDataTable) / Math.Sqrt(Count(objDataTable))
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.LowerConfidenceInterval" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function Percentile(ByRef objDataTable As Data.DataTable, ByVal intPercentile As Integer) As Double
        Try
            Dim intRow As Integer = Convert.ToInt32(Math.Floor(Count(objDataTable) * (intPercentile / 100)))
            Dim rows() As DataRow = objDataTable.Select("", "DataValue ASC")
            Return Convert.ToDouble(rows(intRow).Item("DataValue"))
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.Percentile" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function StandardDeviation(ByRef objDataTable As DataTable) As Double
        Try
            If objDataTable.Rows.Count > 1 Then
                Return Convert.ToDouble(objDataTable.Compute("STDEV(DataValue)", ""))
            Else
                Return 0
            End If
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.StandardDeviation" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function CoefficientOfVariation(ByRef objDataTable As Data.DataTable) As Double
        Try
            If ArithmeticMean(objDataTable) <> 0 Then
                Return StandardDeviation(objDataTable) / ArithmeticMean(objDataTable)
            Else
                Return 0
            End If
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.CoefficientofVariation" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function Count(ByRef objDataTable As Data.DataTable) As Integer
        Try
            Return objDataTable.Rows.Count
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.Count" & vbCrLf & ex.Message)
        End Try
    End Function

    Shared Function CountCensored(ByRef objDataTable As Data.DataTable) As Integer
        Try
            objDataTable.CaseSensitive = False
            Dim rows() As DataRow = objDataTable.Select("(CensorCode <> '" & Statistics.NotCensored & "') AND (CensorCode <> '" & Statistics.Unknown & "')")
            Return rows.Count
        Catch ex As Exception
            Throw New Exception("Error Occured in Statistics.Count" & vbCrLf & ex.Message)
        End Try
    End Function
End Class