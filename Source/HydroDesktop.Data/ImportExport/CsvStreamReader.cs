using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace HydroDesktop.ImportExport
{
	class CsvStreamReader
	{
		#region Variables

		private TextReader _csvStream;
		private char[] _buffer = new char[4096];
		private int _currentPositionInBuffer = 0;
		private int _lengthReadFromStream = 0;
		private bool _endOfStream = false;
		private bool _endOfLine = false;

		#endregion

		#region Constructor

		public CsvStreamReader ( TextReader textStream )
		{
			_csvStream = textStream;
		}

		#endregion

		#region Private Members

		/// <summary>
		/// Reads one data item (an item delimited by commas) in a line from a CSV stream
		/// </summary>
		/// <returns>A string representing the data item, or null if the end of the line was already reached in the stream</returns>
		private string ReadData ()
		{
			StringBuilder data = new StringBuilder ();

			// See if we're at the end of a line
			if ( _endOfLine == true )
			{
				_endOfLine = false;
				return null;
			}

			// Step through each character until we've found the end of the data item
			bool isQuoted = false; // True if data item is surrounded by quotes
			bool enteringData = true; // True if we're about to enter the beginning of a data item in the stream
			bool exitingData = false; // True if we're about to leave a quoted data item in the stream

			while ( true )
			{
				// Read the next character from the buffer.  This is more efficient than reading characters one-by-one from the stream.
				char currentCharacter = ReadCharacter ();

				// Check for end of stream
				if ( _endOfStream == true )
				{
					if ( data.Length > 0 )
					{
						return data.ToString ();
					}
					else
					{
						return null;
					}
				}

				// Check for end of data item
				if ( (exitingData == true || isQuoted == false)
					&& (currentCharacter == ',') )
				{
					return data.ToString ();
				}

				// Check for end of line
				if ( (enteringData == true || exitingData == true || isQuoted == false)
					&& (currentCharacter == '\x0A' || currentCharacter == '\x0D') )
				{
					_endOfLine = true;
					// If the stream uses the carriage return - new line combination, then advance the reader past the new line character
					if ( currentCharacter == '\x0D' && Peek () == '\x0A' )
					{
						ReadCharacter ();
					}
					return data.ToString ();
				}

				// See if we're entering a data item
				if ( enteringData == true )
				{
					// See if data item begins with quotes
					if ( currentCharacter == '"' )
					{
						isQuoted = true;
					}
					else
					{
						data.Append ( currentCharacter );
					}
					enteringData = false;
					continue;
				}

				// Check for end of quoted data item
				if ( currentCharacter == '"' && isQuoted == true )
				{
					if ( Peek () == '"' )
					{
						// If a data item is surrounded by quotes, then back to back double quotes within the item represents a one double quote.
						// Add one quote, and skip the other.
						data.Append ( currentCharacter );
						ReadCharacter ();
					}
					else
					{
						exitingData = true;
					}

					continue;
				}

				// If we made it this far, then this character is part of the data item
				data.Append ( currentCharacter );
			}
		}

		/// <summary>
		/// Returns the next available character but does not consume it
		/// </summary>
		/// <returns>The next character to be read</returns>
		private char Peek ()
		{
			// If we're at the end of the buffer, replace its contents with the next block of data
			if ( _currentPositionInBuffer >= _lengthReadFromStream )
			{
				_lengthReadFromStream = _csvStream.ReadBlock ( _buffer, 0, _buffer.Length );

				// Check for end of stream
				if ( _lengthReadFromStream == 0 )
				{
					_endOfStream = true;
					return '\a'; // ('Alert' character)  Doesn't matter what we return, because we'll exit when we see that we're at the end of the stream.
				}
				_currentPositionInBuffer = 0;
			}

			return _buffer[_currentPositionInBuffer];
		}

		/// <summary>
		/// Returns the next available character
		/// </summary>
		/// <returns>The next character to be read</returns>
		private char ReadCharacter ()
		{
			// If we're at the end of the buffer, replace its contents with the next block of data
			if ( _currentPositionInBuffer >= _lengthReadFromStream )
			{
				_lengthReadFromStream = _csvStream.ReadBlock ( _buffer, 0, _buffer.Length );

				// Check for end of stream
				if ( _lengthReadFromStream == 0 )
				{
					_endOfStream = true;
					return 'a'; // Doesn't matter what we return, because we'll exit when we see that we're at the end of the stream.
				}
				_currentPositionInBuffer = 0;
			}

			return _buffer[_currentPositionInBuffer++];
		}

		#endregion

		#region Public Members

		/// <summary>
		/// Reads a line from the CSV stream, and parses it into a string array
		/// </summary>
		/// <returns>String array of data items parsed from the stream, or null if the end of the stream has been reached</returns>
		public string[] ReadLine ()
		{
			var line = new List<string> ();
			string data = ReadData ();

			while ( data != null )
			{
				line.Add ( data );
				data = ReadData ();
			}

			if ( _endOfStream && line.Count == 0 )
			{
				return null;
			}

		    return line.ToArray();
		}

		#endregion
	}
}
