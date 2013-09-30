# -*- coding: utf-8 -*- 

###########################################################################
## Python code generated with wxFormBuilder (version Aug 25 2009)
## http://www.wxformbuilder.org/
##
## PLEASE DO "NOT" EDIT THIS FILE!
###########################################################################

import wx

###########################################################################
## Class MyFrame1
###########################################################################

class MyFrame1 ( wx.Frame ):
	
	def __init__( self, parent ):
		wx.Frame.__init__  ( self, parent, id = wx.ID_ANY, title = u"Find Shapefile on HydroShare", pos = wx.DefaultPosition, size = wx.Size( 289,381 ), style = wx.DEFAULT_FRAME_STYLE|wx.TAB_TRAVERSAL )
		
		self.SetSizeHintsSz( wx.DefaultSize, wx.DefaultSize )
		
		bSizer1 = wx.BoxSizer( wx.VERTICAL )
		
		self.btn_refresh = wx.Button( self, wx.ID_ANY, u"Refresh List", wx.DefaultPosition, wx.DefaultSize, 0 )
		bSizer1.Add( self.btn_refresh, 0, wx.ALL|wx.EXPAND, 5 )
		
		rdo_FilterSearchChoices = [ u"Spatial Data", u"Time Series" ]
		self.rdo_FilterSearch = wx.RadioBox( self, wx.ID_ANY, u"Filter Search", wx.DefaultPosition, wx.DefaultSize, rdo_FilterSearchChoices, 2, wx.RA_SPECIFY_COLS )
		self.rdo_FilterSearch.SetSelection( 0 )
		bSizer1.Add( self.rdo_FilterSearch, 0, wx.ALIGN_CENTER|wx.EXPAND, 1 )
		
		lst_AvailableItemsChoices = []
		self.lst_AvailableItems = wx.ListBox( self, wx.ID_ANY, wx.DefaultPosition, wx.DefaultSize, lst_AvailableItemsChoices, wx.LB_MULTIPLE|wx.LB_NEEDED_SB )
		bSizer1.Add( self.lst_AvailableItems, 1, wx.ALL|wx.EXPAND, 5 )
		
		bSizer3 = wx.BoxSizer( wx.HORIZONTAL )
		
		self.btn_cancel = wx.Button( self, wx.ID_ANY, u"Cancel", wx.DefaultPosition, wx.DefaultSize, 0 )
		bSizer3.Add( self.btn_cancel, 1, wx.ALL, 5 )
		
		self.btn_GetData = wx.Button( self, wx.ID_ANY, u"Get Data", wx.DefaultPosition, wx.DefaultSize, 0 )
		bSizer3.Add( self.btn_GetData, 1, wx.ALL, 5 )
		
		bSizer1.Add( bSizer3, 0, wx.EXPAND, 5 )
		
		self.SetSizer( bSizer1 )
		self.Layout()
		
		# Connect Events
		self.btn_refresh.Bind( wx.EVT_BUTTON, self.clk_Refresh )
		self.rdo_FilterSearch.Bind( wx.EVT_RADIOBOX, self.clk_FilterSearch )
		self.btn_cancel.Bind( wx.EVT_BUTTON, self.clk_Cancel )
		self.btn_GetData.Bind( wx.EVT_BUTTON, self.clk_GetData )
	
	def __del__( self ):
		pass
	
	
	# Virtual event handlers, overide them in your derived class
	def clk_Refresh( self, event ):
		event.Skip()
	
	def clk_FilterSearch( self, event ):
		event.Skip()
	
	def clk_Cancel( self, event ):
		event.Skip()
	
	def clk_GetData( self, event ):
		event.Skip()
	

