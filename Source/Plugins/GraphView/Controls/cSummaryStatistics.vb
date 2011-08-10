Public Class cSummaryStatistics


    Public Sub ClearStatistics()
        Statistic_NumberOfObservations = 0
        Statistic_NumberOfCensoredObservations = 0
        Statistic_ArithmeticMean = 0
        Statistic_GeometricMean = 0
        Statistic_Maximum = 0
        Statistic_Minimum = 0
        Statistic_StandardDeviation = 0
        Statistic_CoefficientOfVariation = 0

        Statistic_10thPercentile = 0
        Statistic_25thPercentile = 0
        Statistic_50thPercentile = 0
        Statistic_75thPercentile = 0
        Statistic_90thPercentile = 0
    End Sub
    Public Sub GetStatistics(ByVal data As DataTable, ByVal options As PlotOptions)

        Statistic_NumberOfObservations = Statistics.Count(data)
        Statistic_NumberOfCensoredObservations = Statistics.CountCensored(data)
        If options.UseCensoredData = True Then
            Statistic_ArithmeticMean = Statistics.ArithmeticMean(data)
            Statistic_GeometricMean = Statistics.GeometricMean(data)
            Statistic_Maximum = Statistics.Maximum(data)
            Statistic_Minimum = Statistics.Minimum(data)
            Statistic_StandardDeviation = Statistics.StandardDeviation(data)
            Statistic_CoefficientOfVariation = Statistics.CoefficientOfVariation(data)

            Statistic_10thPercentile = Statistics.Percentile(data, 10)
            Statistic_25thPercentile = Statistics.Percentile(data, 25)
            Statistic_50thPercentile = Statistics.Percentile(data, 50)
            Statistic_75thPercentile = Statistics.Percentile(data, 75)
            Statistic_90thPercentile = Statistics.Percentile(data, 90)
        Else
            Dim temp As DataTable = data.Copy
            If (Not options.UseCensoredData) Then
                temp.CaseSensitive = False
                Dim censoredRows() As DataRow = temp.Select("(CensorCode <> '" & Statistics.NotCensored & "') AND (CensorCode <> '" & Statistics.Unknown & "')")

                For Each censoredRow As DataRow In censoredRows
                    temp.Rows.Remove(censoredRow)
                Next censoredRow
            End If
            If temp.Rows.Count > 0 Then
                Statistic_ArithmeticMean = Statistics.ArithmeticMean(temp)
                Statistic_GeometricMean = Statistics.GeometricMean(temp)
                Statistic_Maximum = Statistics.Maximum(temp)
                Statistic_Minimum = Statistics.Minimum(temp)
                Statistic_StandardDeviation = Statistics.StandardDeviation(temp)
                Statistic_CoefficientOfVariation = Statistics.CoefficientOfVariation(temp)

                Statistic_10thPercentile = Statistics.Percentile(temp, 10)
                Statistic_25thPercentile = Statistics.Percentile(temp, 25)
                Statistic_50thPercentile = Statistics.Percentile(temp, 50)
                Statistic_75thPercentile = Statistics.Percentile(temp, 75)
                Statistic_90thPercentile = Statistics.Percentile(temp, 90)
            End If
        End If
    End Sub

    'Public Property Statistic_UseCensoredData() As Boolean
    '    Get
    '        Return ckboxUseCensoredData.Checked
    '    End Get
    '    Set(ByVal value As Boolean)
    '        ckboxUseCensoredData.Checked = value
    '    End Set
    'End Property

    <ComponentModel.DefaultValue(0)> _
    Public Property Statistic_NumberOfObservations() As Integer
        Get
            Dim value As Integer
            If Integer.TryParse(tboxNumObs.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            tboxNumObs.Text = value
        End Set
    End Property
    <ComponentModel.DefaultValue(0)> _
    Public Property Statistic_NumberOfCensoredObservations() As Integer
        Get
            Dim value As Integer
            If Integer.TryParse(tboxNumCensoredObs.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            tboxNumCensoredObs.Text = value
        End Set
    End Property
    <ComponentModel.DefaultValue(0.0)> _
    Public Property Statistic_ArithmeticMean() As Double
        Get
            Dim value As Integer
            If Integer.TryParse(tboxAMean.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Double)
            tboxAMean.Text = value
        End Set
    End Property
    <ComponentModel.DefaultValue(0.0)> _
    Public Property Statistic_GeometricMean() As Double
        Get
            Dim value As Integer
            If Integer.TryParse(tboxGeoMean.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Double)
            tboxGeoMean.Text = value
        End Set
    End Property
    <ComponentModel.DefaultValue(0.0)> _
    Public Property Statistic_Maximum() As Double
        Get
            Dim value As Integer
            If Integer.TryParse(tboxMax.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Double)
            tboxMax.Text = value
        End Set
    End Property
    <ComponentModel.DefaultValue(0.0)> _
    Public Property Statistic_Minimum() As Double
        Get
            Dim value As Integer
            If Integer.TryParse(tboxMin.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Double)
            tboxMin.Text = value
        End Set
    End Property
    <ComponentModel.DefaultValue(0.0)> _
    Public Property Statistic_StandardDeviation() As Double
        Get
            Dim value As Integer
            If Integer.TryParse(tboxStdDev.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Double)
            tboxStdDev.Text = value
        End Set
    End Property
    <ComponentModel.DefaultValue(0.0)> _
    Public Property Statistic_CoefficientOfVariation() As Double
        Get
            Dim value As Integer
            If Integer.TryParse(tboxCoeffVar.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Double)
            tboxCoeffVar.Text = value
        End Set
    End Property

    <ComponentModel.DefaultValue(0.0)> _
    Public Property Statistic_10thPercentile() As Double
        Get
            Dim value As Integer
            If Integer.TryParse(tbox10Perc.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Double)
            tbox10Perc.Text = value
        End Set
    End Property
    <ComponentModel.DefaultValue(0.0)> _
    Public Property Statistic_25thPercentile() As Double
        Get
            Dim value As Integer
            If Integer.TryParse(tbox25Perc.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Double)
            tbox25Perc.Text = value
        End Set
    End Property
    <ComponentModel.DefaultValue(0.0)> _
    Public Property Statistic_50thPercentile() As Double
        Get
            Dim value As Integer
            If Integer.TryParse(tbox50Perc.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Double)
            tbox50Perc.Text = value
        End Set
    End Property
    <ComponentModel.DefaultValue(0.0)> _
    Public Property Statistic_75thPercentile() As Double
        Get
            Dim value As Integer
            If Integer.TryParse(tbox75Perc.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Double)
            tbox75Perc.Text = value
        End Set
    End Property
    <ComponentModel.DefaultValue(0.0)> _
    Public Property Statistic_90thPercentile() As Double
        Get
            Dim value As Integer
            If Integer.TryParse(tbox90Perc.Text, value) Then
                Return value
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Double)
            tbox90Perc.Text = value
        End Set
    End Property
End Class
