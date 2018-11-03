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
using System.Runtime.Remoting; 
using OpenMI.Standard;  
using Oatc.OpenMI.Sdk.Backbone;  
namespace Oatc.OpenMI.Sdk.Wrapper
{
  /// <summary>
  /// The SmartInputLink contains the ILink object. The smart input link 
  /// has a reference to the engine, which enables the SmartInputLink to update input.
  /// </summary>
  [Serializable]
  public class SmartInputLink  : SmartLink
  {

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="engine">Reference to the engine</param>
      /// <param name="link">Reference to the link</param>
      public SmartInputLink(IRunEngine engine, ILink link)
      {
          this._link   = link;
          this._engine = engine;
      }

	  /// <summary>
	  /// Retrieves data from the providing LinkableComponent as defined 
	  /// in the Link and sets this data in the engine
	  /// </summary>
	  public virtual void UpdateInput()
	  { 
		  ITime inputTime = this._engine.GetInputTime(link.TargetQuantity.ID, link.TargetElementSet.ID);

		  if (inputTime != null) 
		  {
              SendEvent(EventType.TargetBeforeGetValuesCall, this.link.TargetComponent);
              IScalarSet sourceValueSet = (IScalarSet)link.SourceComponent.GetValues(inputTime, link.ID);

              //The input values set is copied in ordet to avoid the risk that it is changed be the provider.

              double targetMissValDef = this._engine.GetMissingValueDefinition();
              ScalarSet targetValueSet = new ScalarSet(sourceValueSet);

              for (int i = 0; i < sourceValueSet.Count; i++)
              {
                  if (!sourceValueSet.IsValid(i))
                  {
                      targetValueSet.data[i] = targetMissValDef;
                  }
              }

              targetValueSet.MissingValueDefinition = targetMissValDef;
              targetValueSet.CompareDoublesEpsilon = targetMissValDef / 1.0e+10;
              SendEvent(EventType.TargetAfterGetValuesReturn, this.link.TargetComponent);
              this.Engine.SetValues(link.TargetQuantity.ID, link.TargetElementSet.ID, targetValueSet);
          }
	  }

	  /// <summary>
	  /// Send event
	  /// </summary>
	  /// <param name="eventType">the event type to send</param>
	  /// <param name="sender">reference to the sender (this)</param>
      public void SendEvent( EventType eventType, ILinkableComponent sender)
	  {
          if (((Oatc.OpenMI.Sdk.Backbone.LinkableComponent)this.link.TargetComponent).HasListeners())
          {
              Oatc.OpenMI.Sdk.Backbone.Event eventD = new Oatc.OpenMI.Sdk.Backbone.Event(eventType);
              eventD.Description = eventType.ToString();
              eventD.Sender = sender;
              ITime t = this._engine.GetCurrentTime();
              if (t is ITimeStamp)
              {
                  eventD.SimulationTime = t as ITimeStamp;
              }
              else
              {
                  eventD.SimulationTime = ((ITimeSpan)this._engine.GetCurrentTime()).End;
              }
              this.link.TargetComponent.SendEvent(eventD);
          }
	  }
  }
}
