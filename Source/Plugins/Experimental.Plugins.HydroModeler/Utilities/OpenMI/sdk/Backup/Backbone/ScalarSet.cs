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
using OpenMI.Standard;

namespace Oatc.OpenMI.Sdk.Backbone
{

	/// <summary>
	/// The ScalarSet class contains a list of scalar values.
    /// <para>This is a trivial implementation of OpenMI.Standard.IScalarSet, refer there for further details.</para>
    /// </summary>
	[Serializable]
	public class ScalarSet  : IScalarSet
	{
        double[] _values;
        double _missingValueDefinition;
        private double _compareDoublesEpsilon;

        /// <summary>
        /// Constructor
        /// </summary>
        public ScalarSet()
        {
            _values = new double[0];
            _missingValueDefinition = -999.0;
            _compareDoublesEpsilon = 0.000001;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="values">The list of values</param>
        public ScalarSet(double[] values)
        {
            _values = values;
            _missingValueDefinition = -999.0;	// TODO call empty constructor
            _compareDoublesEpsilon = 0.000001;
        }

        /// <summary>
        /// Returns if a certain element is valid
        /// </summary>
        /// <param name="elementIndex">Element index</param>
        /// <returns>True if element is valid</returns>
        public virtual bool IsValid(int elementIndex)
        {
            return !isEqual(_values[elementIndex], _missingValueDefinition);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source">The scalar set to copy</param>
        public ScalarSet(IScalarSet source)
        {
            if ( source == null )
            {
                throw new Exception("ScalarSet Constructor from \"Source\": source == null");
            }
            _values = new double[source.Count];
            for (int i = 0; i < source.Count; i++)
            {
                _values[i] = source.GetScalar(i);
            }
        }

        /// <summary>
        /// Gets a value from the list of values
        /// </summary>
        /// <param name="ElementIndex">The element index</param>
        /// <returns>The scalar value</returns>
        public double GetScalar(int ElementIndex)
        {
            if ( _values == null )
            {
                throw new Exception("Values null");
            }
            else
            {
                if ( _values.Length < ElementIndex+1 || ElementIndex < 0 )
                {
                    throw new Exception("Invalid ElementIndex");
                }
            }
            return _values[ElementIndex];
        }

        /// <summary>
        /// Gives direct access to the data
        /// </summary>
        public double[] data
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
            }
        }

        /// <summary>
        /// Returns the number of values
        /// </summary>
        public int Count
        {
            get
            {
                return _values.Length;
            }
        }

        ///<summary>
        /// Check if the current instance equals another instance of this class.
        ///</summary>
		///<param name="obj">The instance to compare the current instance with.</param>
        ///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
		public override bool Equals(Object obj)
        {
			if (obj is ScalarSet)
            {
				ScalarSet sourceSet = (ScalarSet)obj;
                if (sourceSet.Count != Count)
                    return false;
                for (int i = 0; i < Count; i++)
                {
                    if (sourceSet.GetScalar(i) != GetScalar(i))
                        return false;
                }
                return true;
            }

			return base.Equals(obj);
        }


	    ///<summary>
		/// Getter and Setter for the missing value definition.
	    ///</summary>
	    public double MissingValueDefinition
        {
            get { return _missingValueDefinition; }
            set { _missingValueDefinition = value; }
        }


        ///<summary>
        /// The epsilon (i.e. the small double value) that should be used when
        /// checking
        /// 
        ///</summary>
        public double CompareDoublesEpsilon
        {
            get { return _compareDoublesEpsilon; }
            set { _compareDoublesEpsilon = value; }
        }

        /// <summary>
        /// Will compare two doubles, using _doubleEpsilon.
        /// </summary>
        /// <param name="double1">First double</param>
        /// <param name="double2">Second double</param>
        /// <returns>True if double1 and double2 are equal.</returns>
        protected bool isEqual(double double1, double double2)
        {
            bool isEqual = false;

            if (double1 >= double2 - _compareDoublesEpsilon &&
                double1 <= double2 + _compareDoublesEpsilon)
            {
                isEqual = true;
            }

            return isEqual;
        }

		///<summary>
		/// Get Hash Code.
		///</summary>
		///<returns>Hash Code for the current instance.</returns>
		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			if (_values != null) hashCode += _values.GetHashCode();
			return hashCode;
		}
    }
}
 
