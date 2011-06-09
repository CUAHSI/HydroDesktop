#region Copyright
/*
* Copyright (c) 2005,2006,2007, OpenMI Association
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenMI Association nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "OpenMI Association" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "OpenMI Association" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion 
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Spatial;
using Oatc.OpenMI.Gui.Core;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
	/// <summary>
	/// Window for ElementSetViewer tool.
	/// </summary>
	public class ElementSetViewer : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panelViewer;
		private System.Windows.Forms.Button buttonClose;
	

		ArrayList _elementSets;
		double _maxX;
		double _maxY;
		double _minX;
		double _minY;
		double _scale;
		Font _font;
		int _margin;
		float _lineWidth;
		private System.Windows.Forms.NumericUpDown numericMarginWidth;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numericLineWidth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
		
		
		int _penIndex;

		/// <summary>
		/// Creates a new instance of <see cref="ElementSetViewer">ElementSetViewer</see> dialog.
		/// </summary>
		public ElementSetViewer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			//  Workarround to handle a bug from Microsoft
			//  A bug, see http://dturini.blogspot.com/2004_08_01_dturini_archive.html
			//  or  http://support.microsoft.com/default.aspx?scid=KB;EN-US;q326219#appliesto
			try
			{
				_font = new Font("Arial", 11, FontStyle.Bold);
			}
			catch (ArithmeticException)
			{
				Utils.ResetFPU(); 
				_font = new Font("Arial", 11, FontStyle.Bold);
			} 
		}



		private Pen GetNextPen(float width)
		{
			Color color;
			switch( _penIndex )
			{
				case 0:  color = Color.Blue;  break;
				case 1:  color = Color.Red;   break;
				case 2:  color = Color.Green; break;
				default:
					Random rand = new Random(12345);
					color = Color.FromArgb( rand.Next(255), rand.Next(255), rand.Next(255) );
					break;
			}

			Pen pen;
			if( width<0 )
				pen = new Pen( color );
			else
				pen = new Pen( color, width );

			_penIndex++;

			return( pen );
		}

		private Pen GetNextPen()
		{
			return( GetNextPen(-1) );
		}
		private void InitPenGetter()
		{
			_penIndex = 0;
		}


		/// <summary>
		/// Populates dialog with element sets.
		/// </summary>
		/// <param name="elementSets"><see cref="ArrayList">ArrayList</see> of element sets, ie. 
		/// <see cref="IElementSet">IElementSet</see> objects.</param>
		public void PopulateDialog( ArrayList elementSets )
		{
			_elementSets = elementSets;

			_maxX = double.MinValue;
			_maxY = double.MinValue;
			_minX = double.MaxValue;
			_minY = double.MaxValue;
			_scale = 1;
			_margin = 10;
			_lineWidth = 2;

			// find borders
			foreach(IElementSet elementSet in _elementSets)			
				for (int i = 0; i < elementSet.ElementCount; i++)				
					for (int n = 0; n < elementSet.GetVertexCount(i); n++)
					{
						if (_maxX < elementSet.GetXCoordinate(i,n))						
							_maxX = elementSet.GetXCoordinate(i,n);
						if (_maxY < elementSet.GetYCoordinate(i,n))						
							_maxY = elementSet.GetYCoordinate(i,n);						
						if (_minX > elementSet.GetXCoordinate(i,n))						
							_minX = elementSet.GetXCoordinate(i,n);
						if (_minY > elementSet.GetYCoordinate(i,n))						
							_minY = elementSet.GetYCoordinate(i,n);						
					}			
		}

		private void panelViewer_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			// get values from numericUpDowns
			_margin = (int)numericMarginWidth.Value;
			_lineWidth = (float)numericLineWidth.Value;


			// calculate scale
			double a = double.MaxValue;
			double b = double.MaxValue;
			if (_maxY > _minY)			
				a = (panelViewer.ClientSize.Height - 2*_margin) / (_maxY - _minY);						
			if (_maxX > _minX)			
				b = (panelViewer.ClientSize.Width - 2*_margin) /  (_maxX - _minX);
			_scale = Math.Min(a,b);

			InitPenGetter();


			int x1,x2,y1,y2;
			Pen pen;

			int idBasedCount = 0;
		
			// draw element sets
			foreach(IElementSet elementSet in _elementSets)
			{
				switch( elementSet.ElementType )
				{
					case ElementType.IDBased:
						pen = GetNextPen();
						e.Graphics.DrawString( "ID: "+elementSet.ID, _font, pen.Brush, _margin, _margin + idBasedCount*20 );
						idBasedCount++;
						break;


					case ElementType.XYPoint:
						pen = GetNextPen();
						
						for( int i = 0; i < elementSet.ElementCount; i++ )
						{
							x1 = TransX(elementSet.GetXCoordinate(i,0));
							y1 = TransY(elementSet.GetYCoordinate(i,0));
							e.Graphics.FillEllipse( pen.Brush, x1, y1, 5, 5 );						
						}						
						break;

					case ElementType.XYPolygon:
						pen = GetNextPen( _lineWidth );
						for (int i = 0; i < elementSet.ElementCount; i++)
						{
							for (int n = 0; n < elementSet.GetVertexCount(i); n++)
							{
								if (n == 0)
								{
									x1 = TransX(elementSet.GetXCoordinate(i,elementSet.GetVertexCount(i)-1));
									y1 = TransY(elementSet.GetYCoordinate(i,elementSet.GetVertexCount(i)-1));
									x2 = TransX(elementSet.GetXCoordinate(i,0));
									y2 = TransY(elementSet.GetYCoordinate(i,0));
								}
								else
								{
									x1 = TransX(elementSet.GetXCoordinate(i,n-1));
									y1 = TransY(elementSet.GetYCoordinate(i,n-1));
									x2 = TransX(elementSet.GetXCoordinate(i,n));
									y2 = TransY(elementSet.GetYCoordinate(i,n));
								}
																
								e.Graphics.DrawLine(pen,x1,y1,x2,y2);														
							}
						}
						
						break;

					case ElementType.XYPolyLine:
					case ElementType.XYLine:
						pen = GetNextPen( _lineWidth );

						for (int i = 0; i < elementSet.ElementCount; i++)
						{
							for (int n = 1; n < elementSet.GetVertexCount(i); n++)
							{
							
								x1 = TransX(elementSet.GetXCoordinate(i,n-1));
								y1 = TransY(elementSet.GetYCoordinate(i,n-1));
								x2 = TransX(elementSet.GetXCoordinate(i,n));
								y2 = TransY(elementSet.GetYCoordinate(i,n));

								e.Graphics.DrawLine(pen,x1,y1,x2,y2);
								
							}
						}					

						break;
				}				
			}
		}

		private int TransX(double x)
		{
			int x1 = (int)((x - _minX) * _scale) + _margin;

			return x1;
		}

		private int TransY(double y)
		{
			int y1 = (int)((y - _minY) * _scale);
			y1 = panelViewer.ClientSize.Height - y1 - _margin ;

			return y1;
		}

		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void panelViewer_Resize(object sender, System.EventArgs e)
		{
			panelViewer.Invalidate();
		}

		

		#region .NET generated code
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ElementSetViewer));
			this.panelViewer = new System.Windows.Forms.Panel();
			this.buttonClose = new System.Windows.Forms.Button();
			this.numericMarginWidth = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numericLineWidth = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.numericMarginWidth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericLineWidth)).BeginInit();
			this.SuspendLayout();
			// 
			// panelViewer
			// 
			this.panelViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panelViewer.BackColor = System.Drawing.Color.White;
			this.panelViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelViewer.Location = new System.Drawing.Point(4, 4);
			this.panelViewer.Name = "panelViewer";
			this.panelViewer.Size = new System.Drawing.Size(648, 368);
			this.panelViewer.TabIndex = 0;
			this.panelViewer.Resize += new System.EventHandler(this.panelViewer_Resize);
			this.panelViewer.Paint += new System.Windows.Forms.PaintEventHandler(this.panelViewer_Paint);
			this.panelViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelViewer_MouseMove);
			this.panelViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelViewer_MouseDown);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.buttonClose.Location = new System.Drawing.Point(544, 404);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(92, 28);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// numericMarginWidth
			// 
			this.numericMarginWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numericMarginWidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericMarginWidth.Increment = new System.Decimal(new int[] {
																				 2,
																				 0,
																				 0,
																				 0});
			this.numericMarginWidth.Location = new System.Drawing.Point(92, 416);
			this.numericMarginWidth.Name = "numericMarginWidth";
			this.numericMarginWidth.Size = new System.Drawing.Size(52, 20);
			this.numericMarginWidth.TabIndex = 2;
			this.numericMarginWidth.Value = new System.Decimal(new int[] {
																			 10,
																			 0,
																			 0,
																			 0});
			this.numericMarginWidth.ValueChanged += new System.EventHandler(this.panelViewer_Resize);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(8, 416);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 20);
			this.label1.TabIndex = 3;
			this.label1.Text = "Margin width:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(8, 388);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(76, 20);
			this.label2.TabIndex = 5;
			this.label2.Text = "Line width:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericLineWidth
			// 
			this.numericLineWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numericLineWidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numericLineWidth.DecimalPlaces = 1;
			this.numericLineWidth.Increment = new System.Decimal(new int[] {
																			   25,
																			   0,
																			   0,
																			   131072});
			this.numericLineWidth.Location = new System.Drawing.Point(92, 388);
			this.numericLineWidth.Minimum = new System.Decimal(new int[] {
																			 25,
																			 0,
																			 0,
																			 131072});
			this.numericLineWidth.Name = "numericLineWidth";
			this.numericLineWidth.Size = new System.Drawing.Size(52, 20);
			this.numericLineWidth.TabIndex = 4;
			this.numericLineWidth.Value = new System.Decimal(new int[] {
																		   2,
																		   0,
																		   0,
																		   0});
			this.numericLineWidth.ValueChanged += new System.EventHandler(this.panelViewer_Resize);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(504, 380);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(144, 16);
			this.label3.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.Location = new System.Drawing.Point(160, 400);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(160, 16);
			this.label4.TabIndex = 7;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(0, 0);
			this.label5.Name = "label5";
			this.label5.TabIndex = 0;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(0, 0);
			this.label6.Name = "label6";
			this.label6.TabIndex = 0;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.label7.Location = new System.Drawing.Point(160, 380);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(160, 16);
			this.label7.TabIndex = 8;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label8.Location = new System.Drawing.Point(160, 420);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(160, 16);
			this.label8.TabIndex = 9;
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label9.Location = new System.Drawing.Point(328, 380);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(168, 16);
			this.label9.TabIndex = 10;
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label10.Location = new System.Drawing.Point(328, 400);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(168, 16);
			this.label10.TabIndex = 11;
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label11.Location = new System.Drawing.Point(328, 420);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(168, 16);
			this.label11.TabIndex = 12;
			// 
			// ElementSetViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(656, 441);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.numericLineWidth);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numericMarginWidth);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.panelViewer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(664, 468);
			this.Name = "ElementSetViewer";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "ElementSetViewer";
			((System.ComponentModel.ISupportInitialize)(this.numericMarginWidth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericLineWidth)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

     
        private void panelViewer_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            
        }

		#endregion

        private void panelViewer_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // write x and y coordinates			
			double x = ((e.X - _margin) /  _scale) + _minX;
			double y = _minY - (e.Y + _margin - panelViewer.ClientSize.Height) / _scale;
			if( _scale != double.MaxValue )
				this.label3.Text = "(" + x.ToString("F3") +", " + y.ToString("F3") + ")";			
			else
				this.label3.Text = "";


            // write elementSet ID and element index
            this.label4.Text = " ";
            this.label7.Text = " ";
            this.label8.Text = " ";
            this.label9.Text = " ";
            this.label10.Text = " ";
            this.label11.Text = " ";

            for (int elementSetNumber = 0; elementSetNumber < this._elementSets.Count; elementSetNumber++)
            {
                string elementID = " ";
                int elementIndex = -9;
                double distance = 10e30;

                IElementSet elementSet = (IElementSet) _elementSets[elementSetNumber];
                
                if (elementSetNumber == 0)
                {
                    this.label7.Text = elementSet.ID.Substring(0, Math.Min(20, elementSet.ID.Length));
                }
                if (elementSetNumber == 1)
                {
                    this.label9.Text = elementSet.ID.Substring(0, Math.Min(20, elementSet.ID.Length));
                }

                for (int index = 0; index < elementSet.ElementCount; index++)
                {
                    if (((IElementSet) _elementSets[elementSetNumber]).ElementType == ElementType.XYPolygon)
                    {
                        XYPolygon xyPolygon = new XYPolygon();

                        for (int i = 0; i < elementSet.GetVertexCount(index); i++)
                        {
                            xyPolygon.Points.Add(new XYPoint(elementSet.GetXCoordinate(index,i), elementSet.GetYCoordinate(index,i)));
                        }

                        if (XYGeometryTools.IsPointInPolygon(x,y,xyPolygon))
                        {
                            elementID = elementSet.GetElementID(index);
                            elementIndex = index;
                        }
                    }

                    
                    if (((IElementSet) _elementSets[elementSetNumber]).ElementType == ElementType.XYPolyLine)
                    {
                        XYPolyline xyPolyline = new XYPolyline();
                        for (int i = 0; i < elementSet.GetVertexCount(index); i++)
                        {
                            xyPolyline.Points.Add(new XYPoint(elementSet.GetXCoordinate(index,i), elementSet.GetYCoordinate(index,i)));
                        }
                        double xx =  XYGeometryTools.CalculatePolylineToPointDistance(xyPolyline, new XYPoint(x,y));
                        if (xx < distance)
                        {
                            distance = xx;
                            if (xx < 0.3 * xyPolyline.GetLength())
                            {
                                elementIndex = index;
                                elementID = elementSet.GetElementID(index);
                            }
                        }

                    }

                    if (elementSetNumber == 0 && elementIndex >= 0)
                    {
                        this.label4.Text = "Index: " + elementIndex.ToString();
                        this.label8.Text = "ID: " + elementID.Substring(0, Math.Min(17, elementID.Length));
                    }
                    if (elementSetNumber == 1 && elementIndex >= 0)
                    {
                        this.label10.Text = "Index: " + elementIndex.ToString();
                        this.label11.Text = "ID: " + elementID.Substring(0, Math.Min(17, elementID.Length));
                    }
                }
            }
        }
	}
}
