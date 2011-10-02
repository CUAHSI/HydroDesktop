Imports System.Windows.Forms
Imports DotSpatial.Controls
Imports DotSpatial.Controls.RibbonControls
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces

Namespace EditView
    <Plugin("Edit", Author:="Utah State University", UniqueName:="ODM_Tools_Edit", Version:="1.0")> _
    Public Class Main
        Inherits Extension
        Implements IMapPlugin

#Region "Variables"

        '//reference to the main application and it's UI items
        Private _mapArgs As IMapPluginArgs

        '//the tab page which will be added to the tab control by the plugin
        Private _tabPage As TabPage = Nothing

        Private _seriesView As ISeriesView = Nothing

        Private _mainControl As cEditView

        Private Const _pluginName As String = "Edit"

        Private _EditView As RibbonTab

        Private rbSelectSeries As New RibbonButton("Edit Series")

        Private rbDeriveNewDataSeries As New RibbonButton("Derive Series")

        Private rbRestoreData As New RibbonButton("Restore Data")

        Private rbApplyToDatabase As New RibbonButton("Save To Database")

        Private ckbShowLegend As New RibbonCheckBox()

        Private rbChangeYValue As New RibbonButton("Change Value")

        Private rbInterpolate As New RibbonButton("Interpolate")

        Private rbFlag As New RibbonButton("Flag")

        Private rbAddNewPoint As New RibbonButton("Add Point")

        Private rbDeletePoint As New RibbonButton("Delete Point")

#End Region

#Region "IExtension Members"
        '/// <summary>
        '/// Fires when the plugin should become inactive
        '/// </summary>
        Protected Overrides Sub OnDeactivate()

        End Sub
#End Region

#Region "IMapPlugin Members"

        '/// <summary>
        '/// Initialize the mapWindow 6 plugin
        '/// </summary>
        '/// <param name="args">The plugin arguments to access the main application</param>
        Public Sub Initialize(ByVal args As IMapPluginArgs) Implements IMapPlugin.Initialize
            _mapArgs = args


            'To Add Items to the ribbon menu
            If Not _mapArgs.Ribbon Is Nothing Then
                'The main application has by default theree ribbon tabs: Home, Map View and Data.
                'Home is always the first ribbon Tab. 'Views' is always the first ribbon panel in
                'the Home ribbon tab.

                'Remove helpTab
                Dim helpTab As RibbonTab = FindHelpTab()
                If Not helpTab Is Nothing Then
                    args.Ribbon.Tabs.Remove(helpTab)
                End If

                '**************************************************************************************
                'Adding the tab
                _EditView = New RibbonTab(args.Ribbon, _pluginName)
                _mapArgs.Ribbon.Tabs.Add(_EditView)
                AddHandler _EditView.ActiveChanged, AddressOf EditViewTab_ActiveChanged
                'AddHandler _mapArgs.Ribbon.ActiveTabChanged, AddressOf Ribbon_ActiveTabChanged

                '**************************************************************************************

                'To add a ribbon panel with a dropDown and regular button

                'To add a new View to the main application window
                If Not _mapArgs.PanelManager Is Nothing Then

                    Dim manager As IHydroAppManager = CType(_mapArgs.AppManager, IHydroAppManager)
                    If Not manager Is Nothing Then

                        _seriesView = manager.SeriesView
                        _mainControl = New cEditView(_seriesView.SeriesSelector)
                        _seriesView.AddPanel(_pluginName, _mainControl)

                        AddHandler _seriesView.SeriesSelector.Refreshed, AddressOf SeriesView_Refreshed

                    End If
                End If

                InitializeRibbonButtons()

                'add the help tab again
                If Not helpTab Is Nothing Then
                    _mapArgs.Ribbon.Tabs.Add(helpTab)
                End If
            End If

        End Sub

        Private Function FindHelpTab() As RibbonTab
            For Each t As RibbonTab In _mapArgs.Ribbon.Tabs
                If t.Text = "Help" Then
                    Return t
                End If
            Next
            Return Nothing
        End Function

        Private Sub InitializeRibbonButtons()
            'Main Function Panel
            Dim rpMainFunction As New RibbonPanel("Main Functions")

			rpMainFunction.ButtonMoreVisible = False

            rpMainFunction.Items.Add(rbSelectSeries)
            AddHandler rbSelectSeries.Click, AddressOf btnSelectSeries_Click
            rbSelectSeries.Image = My.Resources.Edit

            rpMainFunction.Items.Add(rbDeriveNewDataSeries)
            AddHandler rbDeriveNewDataSeries.Click, AddressOf _mainControl.btnDeriveNewDataSeries_Click
            rbDeriveNewDataSeries.Image = My.Resources.DeriveNewSeries

            rpMainFunction.Items.Add(rbRestoreData)
            AddHandler rbRestoreData.Click, AddressOf _mainControl.btnRestoreData_Click
            rbRestoreData.Image = My.Resources.Restore

            rpMainFunction.Items.Add(rbApplyToDatabase)
            AddHandler rbApplyToDatabase.Click, AddressOf _mainControl.btnApplyToDatabase_Click
            rbApplyToDatabase.Image = My.Resources.Save

            'Plot Function Panel
			Dim rpEditFunction As New RibbonPanel("Edit Functions")
			rpEditFunction.ButtonMoreVisible = False

            rpEditFunction.Items.Add(rbChangeYValue)
            AddHandler rbChangeYValue.Click, AddressOf _mainControl.btnChangeYValue_Click
            rbChangeYValue.Image = My.Resources.ChangeValue

            rpEditFunction.Items.Add(rbInterpolate)
            AddHandler rbInterpolate.Click, AddressOf _mainControl.btnInterpolate_Click
            rbInterpolate.Image = My.Resources.Interpolate

            rpEditFunction.Items.Add(rbFlag)
            AddHandler rbFlag.Click, AddressOf _mainControl.btnFlag_Click
            rbFlag.Image = My.Resources.Flag

            rpEditFunction.Items.Add(rbAddNewPoint)
            AddHandler rbAddNewPoint.Click, AddressOf _mainControl.btnAddNewPoint_Click
            rbAddNewPoint.Image = My.Resources.Add

            rpEditFunction.Items.Add(rbDeletePoint)
            AddHandler rbDeletePoint.Click, AddressOf _mainControl.btnDeletePoint_Click
            rbDeletePoint.Image = My.Resources.Delete

            'Main Function Panel
			Dim rpPlotFunction As New RibbonPanel("Plot Function")
			rpPlotFunction.ButtonMoreVisible = False

            rpPlotFunction.Items.Add(ckbShowLegend)
            ckbShowLegend.Text = "Show Legend"
            ckbShowLegend.Checked = True
            _mainControl.ShowLegend = True
            AddHandler ckbShowLegend.CheckBoxCheckChanged, AddressOf ckbShowLegend_CheckBoxCheckChanged


            _EditView.Panels.Add(rpMainFunction)
            _EditView.Panels.Add(rpEditFunction)
            _EditView.Panels.Add(rpPlotFunction)


            rbAddNewPoint.Enabled = False
            rbApplyToDatabase.Enabled = False
            rbChangeYValue.Enabled = False
            rbDeletePoint.Enabled = False
            rbFlag.Enabled = False
            rbInterpolate.Enabled = False
            rbRestoreData.Enabled = False

        End Sub

        Public Overloads Sub Activate() Implements IMapPlugin.Activate
            MyBase.OnActivate()
        End Sub

        'added by Jiri - remove the 'graph view' tab control when the plug-in
        'is deactivated
        Public Overloads Sub Deactivate() Implements IMapPlugin.Deactivate

            If Not _mapArgs.Ribbon Is Nothing Then
                _mapArgs.PanelManager.RemoveTab(_pluginName)
                _mapArgs.Ribbon.Tabs.Remove(_EditView)
                _seriesView.RemovePanel(_pluginName)
                '_mapArgs.Ribbon.Tabs(0).Panels(0).Items.Remove(_ribbonButton1)

                'important line to deactivate the plugin
                MyBase.OnDeactivate()

            Else
                'MyBase.Deactivate()
                'If Not _tabPage Is Nothing Then
                '    _mainTabControl.TabPages.Remove(_tabPage)
                '    RemoveHandler _mainTabControl.TabIndexChanged, AddressOf TabChanged
                'End If
            End If
        End Sub

        Public Overloads Property IsEnabled() As Boolean Implements IMapPlugin.IsEnabled
            Get
                Return MyBase.IsEnabled
            End Get
            Set(ByVal value As Boolean)
                MyBase.IsEnabled = value
            End Set
        End Property

        Private Sub LeavingEditView()
            _mainControl.pTimeSeriesPlot.Clear()
            _mainControl.dgvDataValues.DataSource = Nothing
            _mainControl.Editing = False
            _mainControl.Canceled = False
            _mainControl.newseriesID = 0
            _mainControl.gboxDataFilter.Enabled = False
            rbAddNewPoint.Enabled = False
            rbApplyToDatabase.Enabled = False
            rbChangeYValue.Enabled = False
            rbDeletePoint.Enabled = False
            rbFlag.Enabled = False
            rbInterpolate.Enabled = False
            rbRestoreData.Enabled = False
            rbSelectSeries.Text = "Edit Series"
        End Sub

#End Region

#Region "Event Handlers"

        Sub SeriesView_Refreshed()
            LeavingEditView()
        End Sub

        Sub EditViewTab_ActiveChanged()
            _mapArgs.PanelManager.SelectedTabName = "Series View"

            If Not _mainControl.Editing Then
                If Not _seriesView Is Nothing Then
                    _seriesView.VisiblePanelName = _pluginName
                End If
            Else
                _mapArgs.PanelManager.SelectedTabName = "MapView"
                If _mainControl.Canceled Then
                    _mainControl.Canceled = False
                    _mapArgs.PanelManager.SelectedTabName = "Series View"
                Else
                    LeavingEditView()
                End If
            End If




            'If Not _mainControl.Editing Then
            '    If _EditView.Active Then
            '        _mapArgs.PanelManager.SelectedTabName = _pluginName
            '        _mainControl.RefreshSelection()
            '    Else
            '        _mapArgs.PanelManager.SelectedTabName = "MapView"
            '        LeavingEditView()
            '    End If
            'Else
            '    If Not _EditView.Active Then
            '        _mapArgs.PanelManager.SelectedTabName = "MapView"
            '        If _mainControl.Canceled Then
            '            _mainControl.Canceled = False
            '            _mapArgs.Ribbon.ActiveTab = _EditView

            '            '_mainControl.pTimeSeriesPlot.Clear()
            '            '_mapArgs.Ribbon.ActiveTab = _EditView
            '            'With _mainControl.pTimeSeriesPlot.zgTimeSeries.GraphPane
            '            '    For i As Integer = 0 To .CurveList.Count - 1
            '            '        If _mainControl.newseriesID = _mainControl.pTimeSeriesPlot.CurveID(i) Then
            '            '            _mainControl.pTimeSeriesPlot.EnterEditMode(i)
            '            '        End If
            '            '    Next
            '            'End With
            '        Else
            '            LeavingEditView()
            '        End If
            '    End If
            'End If
        End Sub

        Sub btnSelectSeries_Click()
            If Not _mainControl.Editing Then
                If Not _seriesView.SeriesSelector.SelectedSeriesID = 0 Then
                    _mainControl.btnSelectSeries_Click()
                    rbSelectSeries.Text = "Stop Editing"
                    _mainControl.gboxDataFilter.Enabled = True
                    rbAddNewPoint.Enabled = True
                    rbApplyToDatabase.Enabled = True
                    rbChangeYValue.Enabled = True
                    rbDeletePoint.Enabled = True
                    rbFlag.Enabled = True
                    rbInterpolate.Enabled = True
                    rbRestoreData.Enabled = True
                End If
            Else
                If _mainControl.Editing Then
                    Dim result As Integer

                    result = MsgBox("You are editing a series. Do you want to save your edits?", MsgBoxStyle.YesNoCancel, "Save?")
                    If result = MsgBoxResult.Yes Then
                        _mainControl.SaveGraphChangesToDatabase()
                    End If
                End If

                rbAddNewPoint.Enabled = False
                rbApplyToDatabase.Enabled = False
                rbChangeYValue.Enabled = False
                rbDeletePoint.Enabled = False
                rbFlag.Enabled = False
                rbInterpolate.Enabled = False
                rbRestoreData.Enabled = False
                _mainControl.gboxDataFilter.Enabled = False

                rbSelectSeries.Text = "Edit Series"
                _mainControl.Editing = False
                _mainControl.newseriesID = 0
                _mainControl.pTimeSeriesPlot.ClearEditMode()
                _mainControl.dgvDataValues.DataSource = Nothing
            End If
        End Sub

        Sub ckbShowLegend_CheckBoxCheckChanged()
            If ckbShowLegend.Checked Then
                _mainControl.ShowLegend = True
            Else
                _mainControl.ShowLegend = False
            End If
            _mainControl.ckbShowLegend_CheckedChanged()
        End Sub

#End Region

    End Class
End Namespace
