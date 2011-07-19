Imports System.Windows.Forms
Imports DotSpatial.Controls
Imports DotSpatial.Controls.RibbonControls
Imports HydroDesktop.Database
Imports HydroDesktop.Interfaces
Imports DotSpatial.Controls.Header

Namespace EditView

    Public Class Main
        Inherits Extension
        Implements IMapPlugin

#Region "Variables"

        'reference to the main application and it's UI items
        Private _mapArgs As IMapPluginArgs

        'the tab page which will be added to the tab control by the plugin
        'Private _tabPage As TabPage = Nothing

        Private _seriesView As ISeriesView = Nothing

        Private _mainControl As cEditView

        Private Const _pluginName As String = "Edit"
        Private Const _editTabKey As String = "kEdit"

        'Private _EditView As RibbonTab
        Private _EditView As New RootItem(_editTabKey, _pluginName)

        Private btnSelectSeries As SimpleActionItem

        Private btnDeriveNewDataSeries As SimpleActionItem

        Private btnRestoreData As SimpleActionItem

        Private btnApplyToDatabase As SimpleActionItem

        'TODO add support for checkbox
        Private ckbShowLegend As SimpleActionItem

        Private btnChangeYValue As SimpleActionItem

        Private btnInterpolate As SimpleActionItem

        Private btnFlag As SimpleActionItem
        Private btnAddNewPoint As SimpleActionItem

        Private btnDeletePoint As SimpleActionItem

#End Region

#Region "IExtension Members"
        ''' <summary>
        ''' Fires when the plugin should become inactive
        ''' </summary>
        Protected Overrides Sub OnDeactivate()

        End Sub
#End Region

#Region "IMapPlugin Members"

        ''' <summary>
        ''' Initialize the  plugin
        ''' </summary>
        ''' <param name="args">The plugin arguments to access the main application</param>
        Public Sub Initialize(ByVal args As IMapPluginArgs) Implements IMapPlugin.Initialize
            _mapArgs = args



            '**************************************************************************************
            'Adding the tab
            _mapArgs.AppManager.HeaderControl.Add(_EditView)

            'TODO handle this using DockManager
            'AddHandler _EditView.ActiveChanged, AddressOf EditViewTab_ActiveChanged
            AddHandler _mapArgs.Ribbon.ActiveTabChanged, AddressOf Ribbon_ActiveChanged

            '**************************************************************************************

            'To add a new main panel View to the main application window
            If Not _mapArgs.PanelManager Is Nothing Then

                Dim manager As IHydroAppManager = CType(_mapArgs.AppManager, IHydroAppManager)
                If Not manager Is Nothing Then

                    _seriesView = manager.SeriesView
                    _mainControl = New cEditView(_seriesView.SeriesSelector)
                    _seriesView.AddPanel(_pluginName, _mainControl)
                End If
            End If

            InitializeRibbonButtons()

            'opening project event
            AddHandler _mapArgs.AppManager.SerializationManager.Deserializing, AddressOf SerializationManager_Deserializing

        End Sub


        Private Sub InitializeRibbonButtons()

            'Main Function Panel
            Dim mainFunctionGroup As String = "Main Functions"

            btnSelectSeries = New SimpleActionItem("Edit Series", AddressOf btnSelectSeries_Click)
            btnSelectSeries.RootKey = _editTabKey
            btnSelectSeries.LargeImage = My.Resources.Edit
            btnSelectSeries.GroupCaption = mainFunctionGroup
            _mapArgs.AppManager.HeaderControl.Add(btnSelectSeries)

            btnDeriveNewDataSeries = New SimpleActionItem("Derive Series", AddressOf _mainControl.btnDeriveNewDataSeries_Click)
            btnDeriveNewDataSeries.RootKey = _editTabKey
            btnDeriveNewDataSeries.LargeImage = My.Resources.DeriveNewSeries
            btnDeriveNewDataSeries.GroupCaption = mainFunctionGroup
            _mapArgs.AppManager.HeaderControl.Add(btnDeriveNewDataSeries)

            btnRestoreData = New SimpleActionItem("Restore Data", AddressOf _mainControl.btnRestoreData_Click)
            btnRestoreData.RootKey = _editTabKey
            btnRestoreData.LargeImage = My.Resources.Restore
            btnRestoreData.GroupCaption = mainFunctionGroup
            _mapArgs.AppManager.HeaderControl.Add(btnRestoreData)

            btnApplyToDatabase = New SimpleActionItem("Save To Database", AddressOf _mainControl.btnApplyToDatabase_Click)
            btnApplyToDatabase.RootKey = _editTabKey
            btnApplyToDatabase.LargeImage = My.Resources.Save
            btnApplyToDatabase.GroupCaption = mainFunctionGroup
            _mapArgs.AppManager.HeaderControl.Add(btnApplyToDatabase)

            'Plot Function Panel
            Dim editFunctionGroup As String = "Edit Functions"

            btnChangeYValue = New SimpleActionItem("Change Value", AddressOf _mainControl.btnChangeYValue_Click)
            btnChangeYValue.RootKey = _editTabKey
            btnChangeYValue.LargeImage = My.Resources.ChangeValue
            btnChangeYValue.GroupCaption = editFunctionGroup
            _mapArgs.AppManager.HeaderControl.Add(btnChangeYValue)

            btnInterpolate = New SimpleActionItem("Interpolate", AddressOf _mainControl.btnInterpolate_Click)
            btnInterpolate.RootKey = _editTabKey
            btnInterpolate.LargeImage = My.Resources.Interpolate
            btnInterpolate.GroupCaption = editFunctionGroup
            _mapArgs.AppManager.HeaderControl.Add(btnInterpolate)

            btnFlag = New SimpleActionItem("Flag", AddressOf _mainControl.btnFlag_Click)
            btnFlag.RootKey = _editTabKey
            btnFlag.LargeImage = My.Resources.Flag
            btnFlag.GroupCaption = editFunctionGroup
            _mapArgs.AppManager.HeaderControl.Add(btnFlag)

            btnAddNewPoint = New SimpleActionItem("Add Point", AddressOf _mainControl.btnAddNewPoint_Click)
            btnAddNewPoint.RootKey = _editTabKey
            btnAddNewPoint.LargeImage = My.Resources.Add
            btnAddNewPoint.GroupCaption = editFunctionGroup
            _mapArgs.AppManager.HeaderControl.Add(btnAddNewPoint)

            btnDeletePoint = New SimpleActionItem("Delete Point", AddressOf _mainControl.btnDeletePoint_Click)
            btnDeletePoint.RootKey = _editTabKey
            btnDeletePoint.LargeImage = My.Resources.Delete
            btnDeletePoint.GroupCaption = editFunctionGroup
            _mapArgs.AppManager.HeaderControl.Add(btnDeletePoint)

            'Main Function Panel
            Dim plotFunctionGroup As String = "Plot Function"

            ckbShowLegend = New SimpleActionItem("Show Legend", AddressOf _mainControl.ckbShowLegend_Click)
            ckbShowLegend.RootKey = _editTabKey
            ckbShowLegend.LargeImage = My.Resources.Legend
            ckbShowLegend.GroupCaption = plotFunctionGroup
            ckbShowLegend.ToggleGroupKey = "kEditLegend"
            _mapArgs.AppManager.HeaderControl.Add(ckbShowLegend)

            _mainControl.ShowLegend = False

            'disable buttons by default
            _mapArgs.AppManager.HeaderControl.DisableItem(btnAddNewPoint.Key)
            _mapArgs.AppManager.HeaderControl.DisableItem(btnApplyToDatabase.Key)
            _mapArgs.AppManager.HeaderControl.DisableItem(btnChangeYValue.Key)
            _mapArgs.AppManager.HeaderControl.DisableItem(btnDeletePoint.Key)
            _mapArgs.AppManager.HeaderControl.DisableItem(btnFlag.Key)
            _mapArgs.AppManager.HeaderControl.DisableItem(btnInterpolate.Key)
            _mapArgs.AppManager.HeaderControl.DisableItem(btnRestoreData.Key)
            'rbAddNewPoint.Enabled = False
            'rbApplyToDatabase.Enabled = False
            'rbChangeYValue.Enabled = False
            'rbDeletePoint.Enabled = False
            'rbFlag.Enabled = False
            'rbInterpolate.Enabled = False
            'rbRestoreData.Enabled = False

        End Sub

        Public Overloads Sub Activate() Implements IMapPlugin.Activate
            MyBase.OnActivate()
        End Sub

        'added by Jiri - remove the 'graph view' tab control when the plug-in
        'is deactivated
        Public Overloads Sub Deactivate() Implements IMapPlugin.Deactivate

            _mapArgs.AppManager.HeaderControl.RemoveItems()

            'important line to deactivate the plugin
            MyBase.OnDeactivate()

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
            _mapArgs.AppManager.HeaderControl.DisableItem(btnAddNewPoint.Key)
            _mapArgs.AppManager.HeaderControl.DisableItem(btnApplyToDatabase.Key)
            _mapArgs.AppManager.HeaderControl.DisableItem(btnChangeYValue.Key)
            _mapArgs.AppManager.HeaderControl.DisableItem(btnDeletePoint.Key)
            _mapArgs.AppManager.HeaderControl.DisableItem(btnFlag.Key)
            _mapArgs.AppManager.HeaderControl.DisableItem(btnInterpolate.Key)
            _mapArgs.AppManager.HeaderControl.DisableItem(btnRestoreData.Key)

            'TODO allow changing of button caption
            btnSelectSeries.Caption = "Edit Series"
        End Sub

#End Region

#Region "Event Handlers"

        Sub Ribbon_ActiveChanged()

            Dim myTab As RibbonTab = _mapArgs.Ribbon.Tabs.Find(Function(t) t.Text = _pluginName)
            Dim homeTab As RibbonTab = _mapArgs.Ribbon.Tabs.Find(Function(t) t.Text = "Home")

            If myTab.Active Then
                _mapArgs.PanelManager.SelectedTabName = "Series View"
                _seriesView.VisiblePanelName = _pluginName
            ElseIf homeTab.Active Then
                _mapArgs.PanelManager.SelectedTabName = "Map View"
            Else
                LeavingEditView()
            End If



            'If Not _mainControl.Editing Then
            '    If Not _seriesView Is Nothing Then
            '        _seriesView.VisiblePanelName = _pluginName
            '    End If
            'Else
            '    _mapArgs.PanelManager.SelectedTabName = "MapView"
            '    If _mainControl.Canceled Then
            '        _mainControl.Canceled = False
            '        _mapArgs.PanelManager.SelectedTabName = "Series View"
            '    Else
            '        LeavingEditView()
            '    End If
            'End If

        End Sub

        'old code from Ribbon_ActiveChanged..
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

        Sub btnSelectSeries_Click()
            If Not _mainControl.Editing Then
                If Not _seriesView.SeriesSelector.SelectedSeriesID = 0 Then
                    _mainControl.btnSelectSeries_Click()

                    'TODO : allow to change button caption
                    btnSelectSeries.Caption = "Stop Editing"
                    _mainControl.gboxDataFilter.Enabled = True

                    _mapArgs.AppManager.HeaderControl.EnableItem(btnAddNewPoint.Key)
                    _mapArgs.AppManager.HeaderControl.EnableItem(btnApplyToDatabase.Key)
                    _mapArgs.AppManager.HeaderControl.EnableItem(btnChangeYValue.Key)
                    _mapArgs.AppManager.HeaderControl.EnableItem(btnDeletePoint.Key)
                    _mapArgs.AppManager.HeaderControl.EnableItem(btnFlag.Key)
                    _mapArgs.AppManager.HeaderControl.EnableItem(btnInterpolate.Key)
                    _mapArgs.AppManager.HeaderControl.EnableItem(btnRestoreData.Key)
                End If
            Else
                If _mainControl.Editing Then
                    Dim result As Integer

                    result = MsgBox("You are editing a series. Do you want to save your edits?", MsgBoxStyle.YesNoCancel, "Save?")
                    If result = MsgBoxResult.Yes Then
                        _mainControl.SaveGraphChangesToDatabase()
                    End If
                End If

                _mapArgs.AppManager.HeaderControl.DisableItem(btnAddNewPoint.Key)
                _mapArgs.AppManager.HeaderControl.DisableItem(btnApplyToDatabase.Key)
                _mapArgs.AppManager.HeaderControl.DisableItem(btnChangeYValue.Key)
                _mapArgs.AppManager.HeaderControl.DisableItem(btnDeletePoint.Key)
                _mapArgs.AppManager.HeaderControl.DisableItem(btnFlag.Key)
                _mapArgs.AppManager.HeaderControl.DisableItem(btnInterpolate.Key)
                _mapArgs.AppManager.HeaderControl.DisableItem(btnRestoreData.Key)
                _mainControl.gboxDataFilter.Enabled = False

                'Change caption back to Edit Series (TODO support changing of caption)
                btnSelectSeries.Caption = "Edit Series"

                _mainControl.Editing = False
                _mainControl.newseriesID = 0
                _mainControl.pTimeSeriesPlot.ClearEditMode()
                _mainControl.dgvDataValues.DataSource = Nothing
            End If
        End Sub

        'Sub ckbShowLegend_CheckBoxCheckChanged()
        '    'TODO support check box
        '    'If ckbShowLegend.Checked Then
        '    '    _mainControl.ShowLegend = True
        '    'Else
        '    '    _mainControl.ShowLegend = False
        '    'End If
        '    _mainControl.ckbShowLegend_CheckedChanged()
        'End Sub

        Private Sub SerializationManager_Deserializing(ByVal sender As Object, ByVal e As SerializingEventArgs)
            _mainControl.RefreshSelection()
        End Sub

#End Region

    End Class
End Namespace
