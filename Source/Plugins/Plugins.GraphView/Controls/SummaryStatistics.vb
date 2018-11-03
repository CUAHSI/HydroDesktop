Imports HydroDesktop.Interfaces.ObjectModel

Namespace Controls
    Public Class SummaryStatistics
        Public Property NumberOfObservations() As Integer
        Public Property NumberOfCensoredObservations() As Integer
        Public Property ArithmeticMean() As Double
        Public Property GeometricMean() As Double
        Public Property Maximum() As Double
        Public Property Minimum() As Double
        Public Property StandardDeviation() As Double
        Public Property CoefficientOfVariation() As Double
        Public Property Percentile10() As Double
        Public Property Percentile25() As Double
        Public Property Percentile50() As Double
        Public Property Percentile75() As Double
        Public Property Percentile90() As Double

        Public Shared Function Create(ByVal table As DataTable, ByVal useCensoredData As Boolean) As SummaryStatistics
            Dim result = New SummaryStatistics()

            result.NumberOfObservations = Statistics.Count(table)
            result.NumberOfCensoredObservations = Statistics.CountCensored(table)

            Dim data = table
            If (Not useCensoredData) Then
                Dim temp As DataTable = table.Copy
                Dim censoredRows() As DataRow = temp.Rows.Cast(Of DataRow).Where(Function(row) DataValue.IsCensored(row("CensorCode"))).ToArray()

                For Each censoredRow As DataRow In censoredRows
                    temp.Rows.Remove(censoredRow)
                Next censoredRow

                data = temp
            End If

            result.ArithmeticMean = Statistics.ArithmeticMean(data)
            result.GeometricMean = Statistics.GeometricMean(data)
            result.Maximum = Statistics.Maximum(data)
            result.Minimum = Statistics.Minimum(data)
            result.StandardDeviation = Statistics.StandardDeviation(data)
            result.CoefficientOfVariation = Statistics.CoefficientOfVariation(data)

            result.Percentile10 = Statistics.Percentile(data, 10)
            result.Percentile25 = Statistics.Percentile(data, 25)
            result.Percentile50 = Statistics.Percentile(data, 50)
            result.Percentile75 = Statistics.Percentile(data, 75)
            result.Percentile90 = Statistics.Percentile(data, 90)

            Return result
        End Function
    End Class
End NameSpace