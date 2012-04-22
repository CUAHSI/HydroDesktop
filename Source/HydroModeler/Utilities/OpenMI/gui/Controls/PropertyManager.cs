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
using System.ComponentModel;
using OpenMI.Standard;
using Oatc.OpenMI.Sdk.DevelopmentSupport;

namespace Oatc.OpenMI.Gui.Controls
{
	/// <summary>
	/// This class is used to hold pairs of <c>string</c> key-value pairs.
	/// These pairs are than published like typical .NET class properties (ie. { get; set; } ) 
	/// using overrides of <see cref="ICustomTypeDescriptor">ICustomTypeDescriptor</see>
	/// methods.
	/// Main goal of this class is to be able to modify properties shown in
	/// <see cref="System.Windows.Forms.PropertyGrid">PropertyGrid</see>
	/// in run-time.
	/// </summary>
	public class PropertyManager: ICustomTypeDescriptor
	{
		Hashtable _properties;
		object _tag;

		/// <summary>
		/// Creates a new instance if <see cref="PropertyManager">PropertyManager</see> class.
		/// </summary>
		public PropertyManager()
		{
			_properties = new Hashtable();
			_tag = null;
		}
	

		private class MyPropertyDescriptor: PropertyDescriptor
		{
			private PropertyManager _enclosingInstance;
			private bool _readOnly;
			private string _value;
						
			public MyPropertyDescriptor(
				PropertyManager enclosingInstance,
				string name,
				string value,
				bool readOnly,
				Attribute[] attrs) :
				base(name, attrs)
			{
				_enclosingInstance = enclosingInstance;
				_readOnly = readOnly;
				_value = value;			
			}


			public override Type ComponentType
			{
				get { return typeof(PropertyManager); }
			}

			public override bool IsReadOnly
			{
				get { return (_readOnly); }
			}

			public override Type PropertyType
			{
				get { return typeof(string); }
			}

			public override bool CanResetValue(object component)
			{
				return( false );
			}

			public override object GetValue(object component)
			{
				return( _value );				
			}

			public override void ResetValue(object component)
			{				
			}

			public override void SetValue(object component, object value)
			{
				if( !_readOnly )
					_value = (string)value;
			}

			public override bool ShouldSerializeValue(object component)
			{
				return( false );
			}
		}


		#region ICustomTypeDescriptor explicit interface definitions
		

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			MyPropertyDescriptor[] ret = new MyPropertyDescriptor[ _properties.Count ];

			int i=0;
			foreach( MyPropertyDescriptor desc in _properties.Values )
				ret[i++] = desc;		

			return( new PropertyDescriptorCollection(ret) );
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}
		#endregion


		#region Public members

		/// <summary>
		/// Creates new or sets existing property of this instance.
		/// </summary>
		/// <param name="Name">Name of the property.</param>
		/// <param name="Value">Value of the property.</param>
		/// <param name="ReadOnly">Determines whether the property is read-only.</param>		
		public void SetProperty( string Name, string Value, bool ReadOnly )
		{
			SetProperty( Name, Value, ReadOnly, null, null );
		}

		/// <summary>
		/// Creates new or sets existing property of this instance.
		/// </summary>
		/// <param name="Name">Name of the property.</param>
		/// <param name="Value">Value of the property.</param>
		/// <param name="ReadOnly">Determines whether the property is read-only.</param>
		/// <param name="Description">Description of the property or <c>null</c> if no description is needed.</param>		
		public void SetProperty( string Name, string Value, bool ReadOnly, string Description )
		{
			SetProperty( Name, Value, ReadOnly, Description, null );
		}

		/// <summary>
		/// Creates new or sets existing property of this instance.
		/// </summary>
		/// <param name="Name">Name of the property.</param>
		/// <param name="Value">Value of the property.</param>
		/// <param name="ReadOnly">Determines whether the property is read-only.</param>
		/// <param name="Description">Description of the property or <c>null</c> if no description is needed.</param>
		/// <param name="Category">Category of the property or <c>null</c> if no category is needed.</param>
		public void SetProperty( string Name, string Value, bool ReadOnly, string Description, string Category )
		{
			// Set attributes of this property
			ArrayList attributes = new ArrayList();
			attributes.Add( new ReadOnlyAttribute(ReadOnly) );
			if( Description!= null )
				attributes.Add( new DescriptionAttribute(Description) );
			if( Category!=null )
				attributes.Add( new CategoryAttribute(Category) );				

			MyPropertyDescriptor desc = new MyPropertyDescriptor( this, Name, Value, ReadOnly, (Attribute[])attributes.ToArray(typeof(Attribute)) );

			_properties[Name] = desc;
		}

		/// <summary>
		/// Gets value of some property.
		/// </summary>
		/// <param name="Name">Name of the property.</param>
		/// <returns>Returns value of the property.</returns>
		public string GetProperty( string Name )
		{
			if( !_properties.Contains(Name) )
				throw( new ArgumentException("There's no property with this name: "+Name) );
			return( (string)((MyPropertyDescriptor)_properties[Name]).GetValue(null) );
		}

		/// <summary>
		/// Removes property from this instance.
		/// </summary>
		/// <param name="Name">Name of the property to be removed.</param>
		public void RemoveProperty( string Name )
		{
			_properties.Remove(Name);
		}


		/// <summary>
		/// Gets or sets custom user object.
		/// </summary>
		/// <remarks>
		/// This object can be used by the user for any purpose and all non-static methods of this class
		/// ignores it.
		/// Static method <see cref="ConstructPropertyManager">ConstructPropertyManager</see> sets 
		/// new value into it.
		/// </remarks>
		public object Tag 
		{
			get { return( _tag ); }
			set { _tag = value; }
		}


		/// <summary>
		/// Constructs a new <see cref="PropertyManager">PropertyManager</see> from <c>object</c>
		/// of known type.
		/// </summary>
		/// <param name="obj">Object of known type.</param>
		/// <param name="allReadOnly">If true, all properties are readonly.</param>
		/// <returns>Returns new <see cref="PropertyManager">PropertyManager</see>
		/// or <c>null</c> if object's type isn't known.</returns>
		/// <remarks>A this time this method knowns following types:
		/// <list>
		/// <item><see cref="IQuantity">IQuantity</see></item>
		/// <item><see cref="IElementSet">IElementSet</see></item>
		/// <item><see cref="IDataOperation">IDataOperation</see></item>
		/// <item><see cref="ILinkableComponent">ILinkableComponent</see></item>
		/// </list>
		/// Method saves <c>obj</c> parameter to <see cref="Tag">Tag</see> property, but you can
		/// use it for any purpose.
		/// </remarks>
		public static PropertyManager ConstructPropertyManager( object obj, bool allReadOnly )
		{
			PropertyManager prop = null;

			if( obj is IQuantity )
			{
				IQuantity quantity = (IQuantity)obj; 
				prop = new PropertyManager();

				// General
				prop.SetProperty("Description", quantity.Description,          true, "Description of this Quantity.",  "General");
				prop.SetProperty("ID",          quantity.ID,                   true, "ID of this Quantity.",           "General");
				prop.SetProperty("ValueType",   quantity.ValueType.ToString(), true, "Type of this Quantity's value.", "General" );

				// Dimensions
				prop.SetProperty("AmountOfSubstance", quantity.Dimension.GetPower(DimensionBase.AmountOfSubstance).ToString(), true, "The amount of substance in mole.",   "Dimensions");
				prop.SetProperty("Currency",          quantity.Dimension.GetPower(DimensionBase.Currency).ToString(),          true, "Currency in Euro.",              "Dimensions");
				prop.SetProperty("ElectricCurrent",   quantity.Dimension.GetPower(DimensionBase.ElectricCurrent).ToString(),   true, "Electric current in ampere.",    "Dimensions");
				prop.SetProperty("Length",            quantity.Dimension.GetPower(DimensionBase.Length).ToString(),            true, "Length in meter.",               "Dimensions");
				prop.SetProperty("LuminousIntensity", quantity.Dimension.GetPower(DimensionBase.LuminousIntensity).ToString(), true, "Luminous intensity in candela.", "Dimensions");
				prop.SetProperty("Mass",              quantity.Dimension.GetPower(DimensionBase.Mass).ToString(),              true, "Mass in kilogram.",              "Dimensions");
				prop.SetProperty("Temperature",       quantity.Dimension.GetPower(DimensionBase.Temperature).ToString(),       true, "Temperature in kelvin.",         "Dimensions");
				prop.SetProperty("Time",              quantity.Dimension.GetPower(DimensionBase.Time).ToString(),              true, "Time in second.",                "Dimensions");

				// Unit
				prop.SetProperty("ConversionFactorToSI", quantity.Unit.ConversionFactorToSI.ToString(), true, "Multiplicative coefficient used to convert this quantity to SI (SiUnit = Unit*ConversionFactorToSI + OffSetToSI).", "Unit");
				prop.SetProperty("OffSetToSI",           quantity.Unit.OffSetToSI.ToString(),           true, "Additive coefficient used to convert this quantity to SI (SiUnit = Unit*ConversionFactorToSI + OffSetToSI).",       "Unit");
				prop.SetProperty("UnitDescription",      quantity.Unit.Description,                     true, "Description of this unit.",                                                                                         "Unit");
				prop.SetProperty("UnitID",               quantity.Unit.ID,                              true, "ID of this unit.",                                                                                                  "Unit");
			}
			else if( obj is IElementSet )
			{
				IElementSet elementSet = (IElementSet)obj;
				prop = new PropertyManager();

				// General
				prop.SetProperty("ID",                 elementSet.ID,                      true, "ID of this ElementSet",                    "General" );
				prop.SetProperty("Version",            elementSet.Version.ToString(),      true, "Version of this ElementSet.",              "General" );
				prop.SetProperty("SpatialReferenceID", elementSet.SpatialReference.ID,     true, "ID of this ElementSet's SpatialReference", "General" );
				prop.SetProperty("Description",        elementSet.Description,             true, "Description of this ElementSet.",          "General" );
				prop.SetProperty("ElementCount",       elementSet.ElementCount.ToString(), true, "Count of elements of this ElementSet.",    "General" );
				prop.SetProperty("ElementType",        elementSet.ElementType.ToString(),  true, "Type of elements in this ElementSet.",     "General" );
			}
			else if( obj is IDataOperation )
			{
				IDataOperation dataOperation = (IDataOperation)obj;
				prop = new PropertyManager();

				string DataOperationID = "DataOperationID";

				// small trick to avoid that some argument's name is same as DataOperationID.
				// it's not quite pure, but it works:-)				
				bool conflict;
				do
				{
					conflict = false;
					for( int i=0; i<dataOperation.ArgumentCount; i++ )
						if( dataOperation.GetArgument(i).Key == DataOperationID )
						{
							DataOperationID += " ";
							conflict = true;
							break;
						}
				}
				while( conflict );	

				// General
				prop.SetProperty(DataOperationID, dataOperation.ID, true, "ID of this DataOperation", "General");
					
				// Arguments
				for( int i=0; i<dataOperation.ArgumentCount; i++ )
				{
					IArgument arg = dataOperation.GetArgument(i);
					prop.SetProperty(arg.Key, arg.Value, arg.ReadOnly || allReadOnly, arg.Description, "Arguments");
				}		
			}
			else if( obj is ILinkableComponent )
			{
				ILinkableComponent linkableComponent = (ILinkableComponent)obj;
				prop = new PropertyManager();

				DateTime
					timeHorizonStart = CalendarConverter.ModifiedJulian2Gregorian( linkableComponent.TimeHorizon.Start.ModifiedJulianDay ),
                    timeHorizonEnd = CalendarConverter.ModifiedJulian2Gregorian( linkableComponent.TimeHorizon.End.ModifiedJulianDay ),
					earliestInputTime = CalendarConverter.ModifiedJulian2Gregorian( linkableComponent.EarliestInputTime.ModifiedJulianDay );
				
				// General
				prop.SetProperty("ComponentID",             linkableComponent.ComponentID,                        true, "ID the component.",                                                        "General" );
				prop.SetProperty("ComponentDescription",    linkableComponent.ComponentDescription,               true, "Description of this component.",                                           "General" );
				prop.SetProperty("InputExchangeItemCount",  linkableComponent.InputExchangeItemCount.ToString(),  true, "Number of input exchange items.",                                          "General" );
				prop.SetProperty("OutputExchangeItemCount", linkableComponent.OutputExchangeItemCount.ToString(), true, "Number of output exchange items.",                                         "General" );
				prop.SetProperty("ModelID",                 linkableComponent.ModelID,                            true, "ID of the model (model=component+data).",                                  "General" );
				prop.SetProperty("ModelDescription",        linkableComponent.ModelDescription,                   true, "Description of the model.",                                                "General" );
				prop.SetProperty("TimeHorizonStart",        timeHorizonStart.ToString(),                          true, "Start of component's timehorizon.",                                        "General" );
				prop.SetProperty("TimeHorizonEnd",          timeHorizonEnd.ToString(),                            true, "End of component's timehorizon.",                                          "General" );
				prop.SetProperty("ValidationMessage",       linkableComponent.Validate(),                         true, "Validation string generated by component. No error ocured if it's empty.", "General" );
				prop.SetProperty("EarliestInputTime",       earliestInputTime.ToString(),                         true, "Earliest time for which component needs next input.",                      "General" );

				string implementsIManageState = obj is IManageState ? "yes" : "no";
				prop.SetProperty("ImplementsIManageState",  implementsIManageState,                               true, "Describes whether model implements IManageState interface.",               "General" );
			}

			if( prop!=null )
				prop.Tag = obj;

			return( prop );
		}


		#endregion
		

	}
}
