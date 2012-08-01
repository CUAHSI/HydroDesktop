using System.ComponentModel;
using System.Data;
using System.IO;

namespace HydroDesktop.ImportExport
{
	/// <summary>
	/// Helper class for CSV file parsing
	/// </summary>
    public static class CsvFileParser
	{
		#region Private Members

		/// <summary>
		/// Creates a name for a column that has not been used for any existing columns in the given data table
		/// </summary>
		/// <param name="dataTable">The data table for which a unique column name is to be created</param>
		/// <returns>A unique column name for the data table</returns>
		private static string GetUniqueColumnName ( DataTable dataTable )
		{
			return GetUniqueColumnName ( dataTable, "Column" );
		}

		/// <summary>
		/// Creates a name for a column that has not been used for any existing columns in the given data table
		/// </summary>
		/// <param name="dataTable">The data table for which a unique column name is to be created</param>
		/// <param name="baseColumnName">The base column name to start with when creating a unique column name. A number will be appended to the base column name until a unique column name is found.</param>
		/// <returns>A unique column name for the data table</returns>
		private static string GetUniqueColumnName ( DataTable dataTable, string baseColumnName )
		{
			// Check the input column name
			if ( string.IsNullOrEmpty(baseColumnName) )
			{
				baseColumnName = "Column";
			}

			if ( dataTable.Columns.Contains ( baseColumnName ) == false )
			{
				return baseColumnName;
			}

			// Add a number to the column name until we find a unique name
			int counter = 1;
			while ( true )
			{
				string columnName = baseColumnName + counter++;
				if ( dataTable.Columns.Contains ( columnName ) == false )
				{
					return columnName;
				}
			}
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Counts the number of lines in a file 
		/// </summary>
		/// <param name="fileName">The full path to and name of the file to count lines in</param>
		/// <returns>The number of lines in the file</returns>
		private static long CountLinesInFile ( string fileName )
		{
			long lineCount = 0;
			using ( var reader = new StreamReader ( fileName ) )
			{
			    while ( (reader.ReadLine ()) != null )
				{
					lineCount++;
				}
			}
			return lineCount;
		}

	    /// <summary>
	    /// Parses a comma separated file into a DataTable
	    /// </summary>
	    /// <param name="fileToParse">The full path to and name of the CSV file to parse</param>
	    /// <param name="hasHeaders">True if the file has column headers; false otherwise</param>
	    /// <param name="bgWorker">BackgroundWorker (may be null), in order to show progress</param>
	    /// <param name="e">Arguments from a BackgroundWorker (may be null), in order to support canceling</param>
	    /// <param name="maxRowsCount">Max rows count in result DataTable </param>
	    /// <returns>DataTable of the parsed data</returns>
	    public static DataTable ParseFileToDataTable ( string fileToParse, bool hasHeaders, BackgroundWorker bgWorker = null, DoWorkEventArgs e = null, int maxRowsCount = -1)
		{
			var dataTable = new DataTable ();

			// Get the number of lines in the file
			long totalSteps = 0;
			long currentStep = 0;
	        int previousPercentComplete = 0;

			if ( e != null && bgWorker != null )
			{
				// Check for cancel
				if ( bgWorker.CancellationPending )
				{
					e.Cancel = true;
					return dataTable;
				}

				// Report progress
				if ( bgWorker.WorkerReportsProgress )
				{
					bgWorker.ReportProgress ( 0, "Opening file..." );
					totalSteps = CountLinesInFile ( fileToParse );
				}

			}

			using ( TextReader stream = new StreamReader ( fileToParse ) )
			{
				var csv = new CsvStreamReader ( stream );
				string[] line = csv.ReadLine ();
				if ( line == null )
				{
					return dataTable;
				}

				// Get the column headers
			    int percentComplete;
			    if ( hasHeaders )
				{
					// Check for cancel
					if ( e != null && bgWorker != null )
					{
						if ( bgWorker.CancellationPending )
						{
							e.Cancel = true;
							return dataTable;
						}

						// Report progress
						if ( bgWorker.WorkerReportsProgress )
						{
							currentStep++;
							percentComplete = (int)(100 * currentStep / totalSteps);
							if ( percentComplete > previousPercentComplete )
							{
								bgWorker.ReportProgress ( percentComplete, "Reading data header..." );
								previousPercentComplete = percentComplete;
							}
						}
					}

					foreach ( string part in line )
					{
						// Get a unique column header
						string columnHeader = part;

						if ( string.IsNullOrEmpty(columnHeader) )
						{
							columnHeader = GetUniqueColumnName ( dataTable );
						}
						else if ( dataTable.Columns.Contains ( columnHeader ) )
						{
							columnHeader = GetUniqueColumnName ( dataTable, columnHeader );
						}

						// Add the column to the table
						dataTable.Columns.Add ( columnHeader, typeof ( string ) );
					}

					line = csv.ReadLine ();
				}

				// Parse the rest of the file
				while ( line != null )
				{
					// Check for cancel
					if ( e != null && bgWorker != null )
					{
						if ( bgWorker.CancellationPending )
						{
							e.Cancel = true;
							return dataTable;
						}

						// Report progress
						if ( bgWorker.WorkerReportsProgress )
						{
							currentStep++;
							percentComplete = (int)(100 * currentStep / totalSteps);
							if ( percentComplete > previousPercentComplete )
							{
								bgWorker.ReportProgress ( percentComplete, "Reading line " + currentStep + " of " + totalSteps + "..." );
								previousPercentComplete = percentComplete;
							}
						}
					}

					// Add columns if the current line has more columns than the data table
					while ( line.Length > dataTable.Columns.Count )
					{
						dataTable.Columns.Add ( GetUniqueColumnName ( dataTable ), typeof ( string ) );
					}

					// Add the row to the data table
                    if (maxRowsCount < 0 ||
                        dataTable.Rows.Count < maxRowsCount)
                    {
                        dataTable.Rows.Add(line);
                    }
                    else
                    {
                        break;
                    }
				    line = csv.ReadLine ();
				}

			}

			// Report progress
			if ( e != null && bgWorker != null )
			{
				if ( bgWorker.CancellationPending )
				{
					e.Cancel = true;
					return dataTable;
				}

				if ( bgWorker.WorkerReportsProgress )
				{
					bgWorker.ReportProgress ( 100, "All lines read from file" );
				}
			}

			return dataTable;
		}

		#endregion
	}
}
