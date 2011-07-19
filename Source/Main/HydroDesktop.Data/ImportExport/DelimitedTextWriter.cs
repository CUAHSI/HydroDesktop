using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace HydroDesktop.ImportExport
{
	public enum BackgroundWorkerReportingOptions
	{
		UserStateAndProgress,
		ProgressOnly,
		None
	}

	/// <summary>
	/// Writes items to a text file in which items should be separated by a delimiter
	/// </summary>
	public class DelimitedTextWriter : IDisposable
	{
		#region Variables

		private TextWriter _textStream;
		private bool _isDisposed = false;
		private string _delimiter;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the DelimitedTextWriter class for the specified stream, using the specified delimiter and the default encoding and buffer size.
		/// </summary>
		/// <param name="textStream">The stream to write to.</param>
		/// <param name="delimiter">The delimiter to be used to separate data items in a given data row.</param>
		public DelimitedTextWriter ( TextWriter textStream, string delimiter )
		{
			if ( textStream == null )
			{
				throw new ArgumentNullException ( "Null text stream provided to DelimitedTextWriter" );
			}
			else
			{
				_textStream = textStream;
			}

			if ( delimiter == null )
			{
				throw new ArgumentNullException ( "Null delimiter provided to DelimitedTextWriter" );
			}
			else if ( delimiter == String.Empty )
			{
				throw new ArgumentException ( "Empty delimiter provided to DelimitedTextWriter" );
			}
			else
			{
				_delimiter = delimiter;
			}
		}

		/// <summary>
		/// Initializes a new instance of the DelimitedTextWriter class for the specified file on the specified path, using the specified delimiter and the default encoding and buffer size.
		/// </summary>
		/// <param name="path">The complete file path to write to.</param>
		/// <param name="delimiter">The delimiter to be used to separate data items in a given data row.</param>
		public DelimitedTextWriter ( string path, string delimiter )
		{
			_textStream = OpenStream ( path, false );

			if ( delimiter == null )
			{
				throw new ArgumentNullException ( "Null delimiter provided to DelimitedTextWriter" );
			}
			else if ( delimiter == String.Empty )
			{
				throw new ArgumentException ( "Empty delimiter provided to DelimitedTextWriter" );
			}
			else
			{
				_delimiter = delimiter;
			}
		}

		/// <summary>
		/// Initializes a new instance of the DelimitedTextWriter class for the specified file on the specified path, using the specified delimiter and the default encoding and buffer size. If the file exists, it can be either overwritten or appended to. If the file does not exist, this constructor creates a new file.
		/// </summary>
		/// <param name="path">The complete file path to write to.</param>
		/// <param name="delimiter">The delimiter to be used to separate data items in a given data row.</param>
		/// <param name="append">Determines whether data is to be appended to the file. If the file exists and append is false, the file is overwritten. If the file exists and append is true, the data is appended to the file. Otherwise, a new file is created.</param>
		public DelimitedTextWriter ( string path, string delimiter, bool append )
		{
			_textStream = OpenStream ( path, append );

			if ( delimiter == null )
			{
				throw new ArgumentNullException ( "Null delimiter provided to DelimitedTextWriter" );
			}
			else if ( delimiter == String.Empty )
			{
				throw new ArgumentException ( "Empty delimiter provided to DelimitedTextWriter" );
			}
			else
			{
				_delimiter = delimiter;
			}
		}

		#endregion

		#region Destructor

		~DelimitedTextWriter ()
		{
			//Indicate that the GC called Dispose, not the user
			Dispose ( false );
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Formats the input string so that it can be parsed properly in a delimited file, by enclosing the string in double quotes if the string contains characters that would make parsing difficult
		/// </summary>
		/// <param name="dataItem">The input string to format</param>
		/// <param name="delimiter">The delimiter to be used to separate data items in a given data row.</param>
		/// <returns>Properly formatted string for writing to a delimited file</returns>
		private static string FormatDataItem ( string dataItem, string delimiter )
		{
			char[] charactersRequiringQuotes = new char[3];
			charactersRequiringQuotes[0] = '\"';
			charactersRequiringQuotes[1] = '\x0A';
			charactersRequiringQuotes[2] = '\x0D';

			if ( dataItem.IndexOfAny ( charactersRequiringQuotes ) > -1 || dataItem.IndexOf ( delimiter ) > -1 )
			{
				dataItem = "\"" + dataItem.Replace ( "\"", "\"\"" ) + "\"";
			}

			return dataItem;
		}

		/// <summary>
		/// Opens a TextWriter to the output file
		/// </summary>
		/// <param name="outputFilename">Full path and filename for the output file</param>
		/// <param name="append">Determines whether data is to be appended to the file. If the file exists and append is false, the file is overwritten. If the file exists and append is true, the data is appended to the file. Otherwise, a new file is created.</param>
		/// <returns>TextWriter for the file at the path specified</returns>
		private static TextWriter OpenStream ( string outputFilename, bool append )
		{
			// Check that the folder exists
			try
			{
				string parentFolder = Path.GetDirectoryName ( outputFilename );
				if ( Directory.Exists ( parentFolder ) == false )
				{
					throw new Exception ( "The folder where the output file is to be written does not exist" );
				}
			}
			catch ( Exception ex )
			{
				throw new Exception ( "Could not identify folder where output file should be written.\n" + ex.Message );
			}

			// Attempt to create the output file
			TextWriter outputStream = null;
			try
			{
				outputStream = new StreamWriter ( outputFilename, append ) as TextWriter;
			}
			catch ( Exception ex )
			{
				throw new Exception ( "Could not create output file.\n" + ex.Message );
			}

			return outputStream;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Closes the current DelimitedTextWriter and the underlying stream
		/// </summary>
		public void Close ()
		{
			Dispose ();
		}

		/// <summary>
		/// Converts items from a data table row to a delimited list of items, writes the list to the text stream in a single line, and then writes a line terminator to the stream
		/// </summary>
		/// <param name="row">Data row containing the items to be written as a delimited list in a single line</param>
		/// <param name="formatItems">True if each item should be formatted for delimited file compatibility</param>
		public void WriteLine ( DataRow row, bool formatItems )
		{
			ArrayList itemArray = new ArrayList ( row.ItemArray.Length );

			object[] rowItems = row.ItemArray;

			for ( int i = 0; i < rowItems.Length; i++ )
			{
				object rowItem = rowItems[i];
				if ( rowItem == null )
				{
					itemArray.Add ( "" );
				}
				else
				{
					itemArray.Add ( rowItem.ToString () );
				}
			}

			string[] items = (string[])itemArray.ToArray ( typeof ( string ) );

			WriteLine ( items, formatItems );
		}

		/// <summary>
		/// Converts input items to a delimited list of items, writes the list to the text stream in a single line, and then writes a line terminator to the stream
		/// </summary>
		/// <param name="items">The items which should be written as a delimited list in a single line</param>
		/// <param name="formatItems">True if each item should be formatted for delimited file compatibility</param>
		public void WriteLine ( string[] items, bool formatItems )
		{
			// Write data from each column for the current row
			for ( int i = 0; i < items.Length; i++ )
			{
				string item = items[i];
				if ( item == null )
				{
					item = String.Empty;
				}

				if ( formatItems == true )
				{
					item = FormatDataItem ( item, _delimiter );
				}

				if ( i > 0 )
				{
					_textStream.Write ( _delimiter + item );
				}
				else
				{
					_textStream.Write ( item );
				}
			}

			_textStream.Write ( System.Environment.NewLine );
		}

		/// <summary>
		/// Writes data from a data table to a text stream, formatted as delimited values
		/// </summary>
		/// <param name="dataTable">The data table with the data to write</param>
		/// <param name="outputStream">The text stream to which the delimited data will be written</param>
		/// <param name="delimiter">The delimiter to be used to separate data items in a given data row.</param>
		/// <param name="includeHeaders">True if the column names from the data table should be included as headers in the output stream</param>
		public static void DataTableToStream ( DataTable dataTable, TextWriter outputStream, string delimiter, bool includeHeaders )
		{
			DataTableToStream ( dataTable, outputStream, delimiter, includeHeaders, null, null, BackgroundWorkerReportingOptions.None );
		}

		/// <summary>
		/// Writes data from a data table to a text stream, formatted as delimited values
		/// </summary>
		/// <param name="dataTable">The data table with the data to write</param>
		/// <param name="outputStream">The text stream to which the delimited data will be written</param>
		/// <param name="delimiter">The delimiter to be used to separate data items in a given data row.</param>
		/// <param name="includeHeaders">True if the column names from the data table should be included as headers in the output stream</param>
		/// <param name="bgWorker">BackgroundWorker (may be null), in order to show progress</param>
		/// <param name="e">Arguments from a BackgroundWorker (may be null), in order to support canceling</param>
		/// <param name="reportingOption">Indicates how the BackgroundWorker should report progress</param>
		public static void DataTableToStream ( DataTable dataTable, TextWriter outputStream, string delimiter, bool includeHeaders, BackgroundWorker bgWorker, DoWorkEventArgs e, BackgroundWorkerReportingOptions reportingOption )
		{
			// Check that columns are present
			int columnCount = dataTable.Columns.Count;
			if ( columnCount == 0 )
			{
				return;
			}

			// Get the number of rows in the table
			long totalSteps = 0;
			long currentStep = 0;
			int percentComplete = 0;
			int previousPercentComplete = 0;

			// Background worker updates
			if ( bgWorker != null )
			{
				// Check for cancel
				if ( e != null && bgWorker.CancellationPending )
				{
					e.Cancel = true;
					return;
				}

				// Report progress
				if ( bgWorker.WorkerReportsProgress == true && reportingOption != BackgroundWorkerReportingOptions.None )
				{
					if ( reportingOption == BackgroundWorkerReportingOptions.UserStateAndProgress )
					{
						bgWorker.ReportProgress ( 0, "Reading data..." );
					}
					else if ( reportingOption == BackgroundWorkerReportingOptions.ProgressOnly )
					{
						bgWorker.ReportProgress ( 0 );
					}

					totalSteps = dataTable.Rows.Count;
				}
			}

			// Write the column headers
			if ( includeHeaders == true )
			{
				// Background worker updates
				if ( bgWorker != null )
				{
					// Check for cancel
					if ( e != null && bgWorker.CancellationPending )
					{
						e.Cancel = true;
						return;
					}

					// Report progress
					if ( bgWorker.WorkerReportsProgress == true && reportingOption != BackgroundWorkerReportingOptions.None )
					{
						if ( reportingOption == BackgroundWorkerReportingOptions.UserStateAndProgress )
						{
							bgWorker.ReportProgress ( 0, "Reading column headers..." );
						}
						else if ( reportingOption == BackgroundWorkerReportingOptions.ProgressOnly )
						{
							bgWorker.ReportProgress ( 0 );
						}

						totalSteps = dataTable.Rows.Count;
					}
				}

				// Write each column name from the data table
				for ( int i = 0; i < columnCount; i++ )
				{
					string item = FormatDataItem ( dataTable.Columns[i].ColumnName, delimiter );
					if ( i > 0 )
					{
						outputStream.Write ( delimiter + item );
					}
					else
					{
						outputStream.Write ( item );
					}
				}

				outputStream.Write ( System.Environment.NewLine );
			}

			// Write the data
			foreach ( DataRow row in dataTable.Rows )
			{
				// Background worker updates
				if ( bgWorker != null )
				{
					// Check for cancel
					if ( e != null && bgWorker.CancellationPending )
					{
						e.Cancel = true;
						return;
					}

					// Report progress
					if ( bgWorker.WorkerReportsProgress == true && reportingOption != BackgroundWorkerReportingOptions.None )
					{
						currentStep++;
						percentComplete = (int)(100 * currentStep / totalSteps);
						if ( percentComplete > previousPercentComplete )
						{
							if ( reportingOption == BackgroundWorkerReportingOptions.UserStateAndProgress )
							{
								bgWorker.ReportProgress ( percentComplete, "Writing line " + currentStep + " of " + totalSteps + "..." );
							}
							else if ( reportingOption == BackgroundWorkerReportingOptions.ProgressOnly )
							{
								bgWorker.ReportProgress ( percentComplete );
							}

							previousPercentComplete = percentComplete;
						}
					}
				}

				// Write data from each column for the current row
				for ( int i = 0; i < columnCount; i++ )
				{
					object rowValue = row[i];

					if ( rowValue == null )
					{
						rowValue = String.Empty;
					}

					string item = FormatDataItem ( rowValue.ToString (), delimiter );

					if ( i > 0 )
					{
						outputStream.Write ( delimiter + item );
					}
					else
					{
						outputStream.Write ( item );
					}
				}

				outputStream.Write ( System.Environment.NewLine );
			}

			// Background worker updates
			if ( bgWorker != null )
			{
				// Check for cancel
				if ( e != null && bgWorker.CancellationPending )
				{
					e.Cancel = true;
					return;
				}

				// Report progress
				if ( bgWorker.WorkerReportsProgress == true && reportingOption != BackgroundWorkerReportingOptions.None )
				{
					if ( reportingOption == BackgroundWorkerReportingOptions.UserStateAndProgress )
					{
						bgWorker.ReportProgress ( 100, "All lines written" );
					}
					else if ( reportingOption == BackgroundWorkerReportingOptions.ProgressOnly )
					{
						bgWorker.ReportProgress ( 100 );
					}

					totalSteps = dataTable.Rows.Count;
				}
			}
		}

		/// <summary>
		/// Writes data from a data table to a delimited text file
		/// </summary>
		/// <param name="dataTable">The data table with the data to write</param>
		/// <param name="outputFilename">Full path and filename for the output file</param>
		/// <param name="delimiter">The delimiter to be used to separate data items in a given data row.</param>
		/// <param name="includeHeaders">True if the column names from the data table should be included as headers in the output stream</param>
		public static void DataTableToDelimitedFile ( DataTable dataTable, string outputFilename, string delimiter, bool includeHeaders )
		{
			DataTableToDelimitedFile ( dataTable, outputFilename, delimiter, includeHeaders, false, null, null, BackgroundWorkerReportingOptions.None );
		}

		/// <summary>
		/// Writes data from a data table to a delimited text file
		/// </summary>
		/// <param name="dataTable">The data table with the data to write</param>
		/// <param name="outputFilename">Full path and filename for the output  file</param>
		/// <param name="delimiter">The delimiter to be used to separate data items in a given data row.</param>
		/// <param name="includeHeaders">True if the column names from the data table should be included as headers in the output stream</param>
		/// <param name="append">Determines whether data is to be appended to the file. If the file exists and append is false, the file is overwritten. If the file exists and append is true, the data is appended to the file. Otherwise, a new file is created.</param>
		public static void DataTableToDelimitedFile ( DataTable dataTable, string outputFilename, string delimiter, bool includeHeaders, bool append )
		{
			DataTableToDelimitedFile ( dataTable, outputFilename, delimiter, includeHeaders, append, null, null, BackgroundWorkerReportingOptions.None );
		}

		/// <summary>
		/// Writes data from a data table to a delimited text file
		/// </summary>
		/// <param name="dataTable">The data table with the data to write</param>
		/// <param name="outputFilename">Full path and filename for the output  file</param>
		/// <param name="delimiter">The delimiter to be used to separate data items in a given data row.</param>
		/// <param name="includeHeaders">True if the column names from the data table should be included as headers in the output stream</param>
		/// <param name="bgWorker">BackgroundWorker (may be null), in order to show progress</param>
		/// <param name="e">Arguments from a BackgroundWorker (may be null), in order to support canceling</param>
		/// <param name="reportingOption">Indicates how the BackgroundWorker should report progress</param>
		public static void DataTableToDelimitedFile ( DataTable dataTable, string outputFilename, string delimiter, bool includeHeaders, BackgroundWorker bgWorker, DoWorkEventArgs e, BackgroundWorkerReportingOptions reportingOption )
		{
			DataTableToDelimitedFile ( dataTable, outputFilename, delimiter, includeHeaders, false, bgWorker, e, reportingOption );
		}

		/// <summary>
		/// Writes data from a data table to a delimited text file
		/// </summary>
		/// <param name="dataTable">The data table with the data to write</param>
		/// <param name="outputFilename">Full path and filename for the output  file</param>
		/// <param name="delimiter">The delimiter to be used to separate data items in a given data row.</param>
		/// <param name="includeHeaders">True if the column names from the data table should be included as headers in the output stream</param>
		/// <param name="append">Determines whether data is to be appended to the file. If the file exists and append is false, the file is overwritten. If the file exists and append is true, the data is appended to the file. Otherwise, a new file is created.</param>
		/// <param name="bgWorker">BackgroundWorker (may be null), in order to show progress</param>
		/// <param name="e">Arguments from a BackgroundWorker (may be null), in order to support canceling</param>
		/// <param name="reportingOption">Indicates how the BackgroundWorker should report progress</param>
		public static void DataTableToDelimitedFile ( DataTable dataTable, string outputFilename, string delimiter, bool includeHeaders, bool append, BackgroundWorker bgWorker, DoWorkEventArgs e, BackgroundWorkerReportingOptions reportingOption )
		{
			// Check that the folder exists
			try
			{
				string parentFolder = Path.GetDirectoryName ( outputFilename );
				if ( Directory.Exists ( parentFolder ) == false )
				{
					throw new Exception ( "The folder where the output file is to be written does not exist" );
				}
			}
			catch ( Exception ex )
			{
				throw new Exception ( "Could not identify folder where output file should be written.\n" + ex.Message );
			}

			// Attempt to create the output file
			TextWriter outputStream = null;
			try
			{
				using ( outputStream = new StreamWriter ( outputFilename, append ) as TextWriter )
				{
					try
					{
						// Write the data
						DelimitedTextWriter.DataTableToStream ( dataTable, outputStream, delimiter, includeHeaders, bgWorker, e, reportingOption );
					}
					catch ( Exception ex )
					{
						throw new Exception ( "Could not write to file.\n" + ex.Message );
					}

					// Close the file
					outputStream.Close ();
				}
			}
			catch ( Exception ex )
			{
				throw new Exception ( "Could not create output file.\n" + ex.Message );
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		protected void Dispose ( bool disposing )
		{
			if ( _isDisposed == false )
			{
				if ( disposing == true )
				{
					// Code to dispose the managed resources of the class
				}

				// Code to dispose the un-managed resources of the class
				if ( _textStream != null )
				{
					_textStream.Dispose ();
					_textStream = null;
				}
			}

			_isDisposed = true;
		}


		#endregion
	}
}
