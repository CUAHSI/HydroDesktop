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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.Backbone;

namespace Oatc.OpenMI.Gui.Core
{
	/// <summary>
	/// This class describes connection between two OpenMI models in one direction,
	/// which consists of many links in same direction.
	/// </summary>
	public class UIConnection
	{
		UIModel _providingModel;
		UIModel _acceptingModel;
		Point[] _trianglePoints;
		ArrayList _links;
		Pen linePen;

		/// <summary>
		/// Creates a new instance of <see cref="UIConnection">UIConnection</see> class.
		/// </summary>
		/// <param name="providingModel">Model providing data.</param>
		/// <param name="acceptingModel">Model accepting data.</param>
		public UIConnection(UIModel providingModel, UIModel acceptingModel)
		{
			_providingModel = providingModel;
			_acceptingModel = acceptingModel;

			_links = new ArrayList();
			_trianglePoints = new Point[3];
		
			linePen = new Pen(Color.Blue, 2);
		}

		/// <summary>
		/// Gets providing model.
		/// </summary>
		public UIModel ProvidingModel
		{
			get { return(_providingModel); }
		}

		/// <summary>
		/// Gets accepting model.
		/// </summary>
		public UIModel AcceptingModel
		{
			get { return(_acceptingModel); }
		}

		/// <summary>
		/// Gets list of all links in this connection.
		/// </summary>
		public ArrayList Links
		{
			get
			{
				return _links;
			}
		}
		
		/// <summary>
		/// Draw connection (i.e. line with triangle) to specific graphics object.
		/// </summary>
		/// <param name="windowPosition">Position of window described by graphics object in composition area.</param>
		/// <param name="g">Graphics where connection should be drawn.</param>		
		public void Draw(Point windowPosition, Graphics g)
		{
			float startX = _providingModel.GetMidPoint().X;
			float startY = _providingModel.GetMidPoint().Y ;
			float endX   = _acceptingModel.GetMidPoint().X;
			float endY   = _acceptingModel.GetMidPoint().Y;

			// calculate triangle point in area points and store them internally
			_trianglePoints = GetTrianglePoints( startX, startY, endX, endY );

			// recalculate trinagle points so they correspond to window and can be draw
			Point[] windowTrianglePoints = new Point[3];
			for( int i=0; i<3; i++ )
			{
				windowTrianglePoints[i].X = _trianglePoints[i].X - windowPosition.X;
				windowTrianglePoints[i].Y = _trianglePoints[i].Y - windowPosition.Y;
			}

			// modify start and end so they correspond to window
			startX -= windowPosition.X;
			startY -= windowPosition.Y;
			endX -= windowPosition.X;
			endY -= windowPosition.Y;
		
			g.DrawLine(linePen, startX, startY, endX, endY);
			
			// we draw the triangle only the link is at least 10 pixels
			if( Math.Abs(startX-endX) + Math.Abs(startY-endY) > 10 )
			{				
				g.FillPolygon( Brushes.Blue, windowTrianglePoints, System.Drawing.Drawing2D.FillMode.Alternate );
				g.DrawPolygon( Pens.Red, windowTrianglePoints );
			}
		}
	
		private static Point[] GetTrianglePoints(float startX, float startY, float endX, float endY)
		{
			Point[] trianglePoints = new Point[3];

			const float size = 10; // size of triangles

			/*float startX = _providingModel.GetMidPoint().X;
			float startY = _providingModel.GetMidPoint().Y;

			float endX   = _acceptingModel.GetMidPoint().X;
			float endY   = _acceptingModel.GetMidPoint().Y;*/

			float midX   = (endX + startX) / 2;
			float midY   = (endY + startY) / 2;			

			float length = (float) Math.Sqrt(Math.Pow((startX-midX),2) + Math.Pow((startY-midY),2));
		
			float pX = midX + size *(startX - midX)/length;
			float pY = midY + size *(startY - midY)/length;

			float vX = midX - pX;
			float vY = midY - pY;

			float t1X = pX - vY;
			float t1Y = pY + vX;

			float t2X = pX + vY;
			float t2Y = pY - vX;

			trianglePoints[0] = new Point((int) midX,(int) midY);
			trianglePoints[1] = new Point((int) t1X,(int) t1Y);
			trianglePoints[2] = new Point((int) t2X,(int) t2Y);

			return( trianglePoints );
		}


		/// <summary>
		/// Determines, whether point is on connection line, i.e. in the triangle.
		/// </summary>
		/// <param name="point">Point</param>
		/// <returns>Returns <c>true</c> if point is inside the triangle, otherwise returns <c>false</c>.</returns>
		public bool IsOnConnectionLine( Point point )
		{
			bool isOnConnectionLine = true;
			int m;

			for (int i = 0; i < 3; i++)
			{
				m = i + 1;
				if (m == 3)
					m = 0;			

				if(0 < (point.X - _trianglePoints[i].X)*(_trianglePoints[m].Y-_trianglePoints[i].Y) - ( _trianglePoints[m].X - _trianglePoints[i].X )*(point.Y-_trianglePoints[i].Y))
					isOnConnectionLine = false;				
			}

			return isOnConnectionLine;				
		}

		/*private Point getMidPoint()
		{
			return new Point(
			(int)( (_providingModel.GetMidPoint().X + _acceptingModel.GetMidPoint().X) / 2 ),
			(int)( (_providingModel.GetMidPoint().Y + _acceptingModel.GetMidPoint().Y) / 2) );
		}*/
		
	}
}
