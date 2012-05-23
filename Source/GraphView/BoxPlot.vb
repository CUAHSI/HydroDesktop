Option Strict On


''' <summary>
''' Used to calculate statistics used in box plots
''' </summary>
''' <remarks></remarks>
Public Class BoxPlot

#Region " Member Variables "

    'point location variables
    Private m_xVal As Double 'xvalue location of the boxplot
    Private m_yVal_Med As Double 'yvalue of the boxplot -> median value

    'box stats variables
    Private m_mean As Double 'mean value for the boxplot
    Private m_median As Double 'median value for the boxplot
    Private m_quantile_25 As Double '25th quantile value
    Private m_quantile_75 As Double '75th quantile value
    Private m_adjLevel_Lower As Double 'lower whisker
    Private m_adjLevel_Upper As Double 'upper whisker
    Private m_confIntvl95_Lower As Double 'the lower 95% confidence interval on the mean
    Private m_confIntvl95_Upper As Double 'the upper 95% confidence interval on the mean
    Private m_confLimit95_Lower As Double 'the lower 95% confidence limit on the median
    Private m_confLimit95_Upper As Double 'the upper 95% confidence limit on the median

    'other variables
    Private m_outliers_Lower As Double() 'holds the y-value only -> NOTE: use m_xVal for the x-value when plotting
    Private m_outliers_Upper As Double() 'holds the y-value only -> NOTE: use m_xVal for the x-value when plotting
    Private m_numOutliers_Lower As Integer 'number of lower outliers
    Private m_numOutliers_Upper As Integer 'number of upper outliers

#End Region

#Region " Functions "

    ''' <summary>
    ''' 'creates a new instance of the class, initializes member variables
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        m_xVal = 0
        m_yVal_Med = 0
        m_numOutliers_Lower = 0
        m_numOutliers_Upper = 0
    End Sub

    ''' <summary>
    ''' adds a lower outlier value to m_outliers_Lower, updates m_numOutliers_Lower
    ''' </summary>
    ''' <param name="value">the lower outlier value to add</param>
    ''' <remarks></remarks>
    Public Sub AddOutlier_Lower(ByVal value As Double)
        Try
            'resize the array
            ReDim Preserve m_outliers_Lower(m_numOutliers_Lower)
            'add the new outlier
            m_outliers_Lower(m_numOutliers_Lower) = value
            'update the count
            m_numOutliers_Lower += 1
        Catch ex As Exception
            Throw New Exception("Error Occured in BoxPlot.AddOutlier_Lower" & vbCrLf & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' add an upper outlier value to m_outliers_Upper, update m_numOutliers_Upper
    ''' </summary>
    ''' <param name="value">the upper outlier value to add</param>
    ''' <remarks></remarks>
    Public Sub AddOutlier_Upper(ByVal value As Double)
        Try
            'resize the array
            ReDim Preserve m_outliers_Upper(m_numOutliers_Upper)
            'add the new outlier
            m_outliers_Upper(m_numOutliers_Upper) = value
            'update the count
            m_numOutliers_Upper += 1
        Catch ex As Exception
            Throw New Exception("Error Occured in BoxPlot.AddOutlier_Lower" & vbCrLf & ex.Message)
        End Try
    End Sub

#End Region

#Region " Properties "

    ''' <summary>
    ''' gets or sets the xValue for the boxplot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property xValue() As Double
        Get
            Return m_xVal
        End Get
        Set(ByVal value As Double)
            m_xVal = value
        End Set
    End Property

    ''' <summary>
    ''' gets or sets the yValue for the boxplot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property yValue() As Double
        Get
            Return m_yVal_Med
        End Get
        Set(ByVal value As Double)
            m_yVal_Med = value
        End Set
    End Property

    ''' <summary>
    ''' gets or sets the mean value for the boxplot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property mean() As Double
        Get
            Return m_mean
        End Get
        Set(ByVal value As Double)
            m_mean = value
        End Set
    End Property

    ''' <summary>
    ''' gets or sets the median value for the boxplot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property median() As Double
        Get
            Return m_median
        End Get
        Set(ByVal value As Double)
            m_median = value
        End Set
    End Property

    ''' <summary>
    ''' gets or sets the 25th quantile value for the boxplot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property quantile_25th() As Double
        Get
            Return m_quantile_25
        End Get
        Set(ByVal value As Double)
            m_quantile_25 = value
        End Set
    End Property

    ''' <summary>
    ''' gets or sets the 75th quantile value for the boxplot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property quantile_75th() As Double
        Get
            Return m_quantile_75
        End Get
        Set(ByVal value As Double)
            m_quantile_75 = value
        End Set
    End Property

    ''' <summary>
    ''' gets or sets the lower whisker (Lower adjacent level) value for the boxplot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property adjacentLevel_Lower() As Double
        Get
            Return m_adjLevel_Lower
        End Get
        Set(ByVal value As Double)
            m_adjLevel_Lower = value
        End Set
    End Property

    ''' <summary>
    ''' gets or sets the upper whisker (Upper adjacent level) value for the boxplot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property adjacentLevel_Upper() As Double
        Get
            Return m_adjLevel_Upper
        End Get
        Set(ByVal value As Double)
            m_adjLevel_Upper = value
        End Set
    End Property

    ''' <summary>
    ''' gets or sets the Lower 95% Confidence Interval value for the boxplot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property confidenceInterval95_Lower() As Double
        Get
            Return m_confIntvl95_Lower
        End Get
        Set(ByVal value As Double)
            m_confIntvl95_Lower = value
        End Set
    End Property

    ''' <summary>
    ''' gets or sets the Upper 95% Confidence Interval value for the boxplot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property confidenceInterval95_Upper() As Double
        Get
            Return m_confIntvl95_Upper
        End Get
        Set(ByVal value As Double)
            m_confIntvl95_Upper = value
        End Set
    End Property

    ''' <summary>
    ''' gets or sets the Lower 95% Confidence Limit value for the boxplot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property confidenceLimit95_Lower() As Double
        Get
            Return m_confLimit95_Lower
        End Get
        Set(ByVal value As Double)
            m_confLimit95_Lower = value
        End Set
    End Property

    ''' <summary>
    ''' gets or sets the Upper 95% Confidnece Limit value for the boxplot
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property confidenceLimit95_Upper() As Double
        Get
            Return m_confLimit95_Upper
        End Get
        Set(ByVal value As Double)
            m_confLimit95_Upper = value
        End Set
    End Property

    ''' <summary>
    ''' gets the number of lower outliers in m_outliers_Lower
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property numOutliers_Lower() As Integer
        Get
            Return m_numOutliers_Lower
        End Get
    End Property

    ''' <summary>
    ''' gets the number of upper outliers in m_outliers_Upper
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property numOutliers_Upper() As Integer
        Get
            Return m_numOutliers_Upper
        End Get
    End Property

    ''' <summary>
    ''' gets the collection of lower outliers
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property collectionOfOutliers_Lower() As Double()
        Get
            Return m_outliers_Lower
        End Get
    End Property

    ''' <summary>
    ''' gets the collection of upper outliers
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property collectionOfOutliers_Upper() As Double()
        Get
            Return m_outliers_Upper
        End Get
    End Property

    ''' <summary>
    ''' gets or sets the lower outlier value at the given index
    ''' </summary>
    ''' <param name="index">the index into m_outliers_Lower to get or set the value for</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property outlierValue_Lower(ByVal index As Integer) As Double
        Get
            If index >= 0 And index < m_numOutliers_Lower Then
                Return m_outliers_Lower(index)
            Else
                Return -1
            End If
        End Get
        Set(ByVal value As Double)
            If index > 0 And index < m_numOutliers_Lower Then
                m_outliers_Lower(index) = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' gets or sets the upper outlier value at the given index
    ''' </summary>
    ''' <param name="index">the index into m_outliers_Upper to get or set the value for</param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property outlierValue_Upper(ByVal index As Integer) As Double
        Get
            If index >= 0 And index < m_numOutliers_Upper Then
                Return m_outliers_Upper(index)
            Else
                Return -1
            End If
        End Get
        Set(ByVal value As Double)
            If index > 0 And index < m_numOutliers_Upper Then
                m_outliers_Upper(index) = value
            End If
        End Set
    End Property

#End Region

End Class
