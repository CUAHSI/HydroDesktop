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
using DotSpatial.Modeling;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

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
        GraphicsPath _arrowPath = new GraphicsPath();

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
		public Point[] Draw(Point windowPosition, Graphics g)
		{


            Pen arrowPen = new Pen(Color.Black, 1.6F);

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

            //draw curved line
            Point[] lineArray = new Point[4];
            lineArray[0] = new Point((int)startX, (int)startY);
            lineArray[1] = new Point((int)startX - (((int)startX - (int)endX) / 3), (int)startY);
            lineArray[2] = new Point((int)endX - (((int)endX - (int)startX) / 3), (int)endY);
            lineArray[3] = new Point((int)endX, (int)endY);

            //Point[] lineArray2 = new Point[4];
            //lineArray2[0] = new Point((int)startX, (int)startY);
            //lineArray2[1] = new Point((int)startX + (int)((endX - startX) / 4), (int)startY);
            //lineArray2[2] = new Point((int)startX + (int)((endX - startX) / 2), (int)startY + (int)((endY - startY)/2));
            //lineArray2[2] = new Point((int)startX + (int)((endX - startX) * 0.75), (int)endY);
            //lineArray2[3] = new Point((int)startX + (int)((endX - startX)), (int)endY);
            //g.DrawCurve(new Pen(Color.Red, 1.5F), lineArray2);
            //_arrowPath.AddCurve(lineArray2);

            LinearGradientBrush brush = new LinearGradientBrush(lineArray[0], lineArray[3], Color.WhiteSmoke, Color.Black);
            //Blend blend = new Blend();
            //blend.Factors = new float[] { 0.0f, 0.1f, 0.3f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f, 1f, 1f };
            //blend.Positions = new float[] { 0, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f };
            //brush.Blend = blend;

            arrowPen = new Pen(brush, 1.6F);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawBeziers(arrowPen, lineArray);
            _arrowPath.AddBeziers(lineArray);
            _arrowPath.Flatten();

			//g.DrawLine(linePen, startX, startY, endX, endY);

            if (Math.Abs(startX - endX) + Math.Abs(startY - endY) > 10)
            {
                return windowTrianglePoints;
            }
            else
                return new Point[0];



		}
        public void FillArrows(List<Point[]> points, Graphics g)
        {
            

            for (int i = 0; i <= points.Count - 1; i++)
            {
                Point[] windowTrianglePoints = points[i];
                Pen arrowPen = new Pen(Color.Gray, 1.5F);


                g.FillPolygon(Brushes.Black, windowTrianglePoints, System.Drawing.Drawing2D.FillMode.Alternate);
                g.DrawPolygon(arrowPen, windowTrianglePoints);

            }
        }
        //public void Draw(Point windowPosition, Graphics graph, ModelShapes Shape)
        //{
        //    if (Shape == ModelShapes.Arrow)
        //    {
        //        _arrowPath = new GraphicsPath();

        //        //Draws the basic shape
        //        Pen arrowPen;
        //        //if (Highlight < 1)
        //        //    arrowPen = new Pen(Color.Cyan, 3F);
        //        //else
        //        //    arrowPen = new Pen(Color.Black, 3F);

        //        arrowPen = new Pen(Color.Black, 1F);

        //        int startX = _providingModel.GetMidPoint().X;
        //        int startY = _providingModel.GetMidPoint().Y;
        //        int endX = _acceptingModel.GetMidPoint().X;
        //        int endY = _acceptingModel.GetMidPoint().Y;

        //        Point start = new Point(startX, startY);
        //        Point end = new Point(endX, endY);

        //        //Draws the curved arrow
        //        Point[] lineArray = new Point[4];
        //        lineArray[0] = new Point(startX, startY);
        //        lineArray[1] = new Point(startX - ((startX - endX) / 3), startY);
        //        lineArray[2] = new Point(endX - ((endX - startX) / 3), endY);
        //        lineArray[3] = new Point(endX, endY);
        //        graph.DrawBeziers(arrowPen, lineArray);
        //        _arrowPath.AddBeziers(lineArray);
        //        _arrowPath.Flatten();

        //        //Draws the arrow head
        //        Point midPt = new Point((endX + startX) / 2, (endY + startY) / 2);
        //        float dx = 10;// endX - startX;
        //        float dy = 10;// endY - startY;

        //        const double cos = 0.866;
        //        const double sin = 0.500;
        //        //PointF end1 = new PointF(
        //        //    (float)(midPt.X + (dx * cos + dy * -sin)),
        //        //    (float)(midPt.Y + (dx * sin + dy * cos)));
        //        //PointF end2 = new PointF(
        //        //    (float)(midPt.X + (dx * cos + dy * sin)),
        //        //    (float)(midPt.Y + (dx * -sin + dy * cos)));
        //        //graph.DrawLine(arrowPen, midPt, end1);
        //        //graph.DrawLine(arrowPen, midPt, end2);
        //        arrowPen.EndCap = LineCap.ArrowAnchor;

        //        //Point[] arrowArray = new Point[3];
                
        //        //arrowArray[0] = midPt;
        //        //arrowArray[1] = new Point(midPt.X, midPt.Y - 10);
        //        //arrowArray[2] = new Point(midPt.X - 10, midPt.Y + 10);
        //        //graph.DrawPolygon(arrowPen, arrowArray);

        //        //Garbage collection
        //        arrowPen.Dispose();
        //    }
        //}
	
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
