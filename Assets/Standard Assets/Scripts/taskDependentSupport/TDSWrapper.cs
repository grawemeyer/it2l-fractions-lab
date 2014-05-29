using UnityEngine;
using System.Collections;
using System;
using taskDependentSupport.core;

namespace taskDependentSupport
{
	public class TDSWrapper
	{
		#region Internal Data Structures
		#endregion
		
		#region Protected Fields
		#endregion
		
		#region Public Fields
		#endregion
		
		#region Public Get/Set
		#endregion
		
		#region Public Static Fields
		public static GameObject eventManager = null;
		public static bool intelligentSupportOff = false;
		#endregion
		
		#region Public Static Methods
		public static void SendBrowserMessage(params object[] args)
		{
			Application.ExternalCall("newEvent", args);
		}
		
		public static void SendMessageToSupport(params object[] args)
		{
			if (intelligentSupportOff) return;


			string eventType = "";
			string eventName = "";
			string objectID = "";
			string objectValue = "";
			string objectPosition = "";
			int objectValueInt = 0;

			if (args.Length>0) eventType = (string) args [0];
			if (args.Length>1) eventName = (string) args [1];
			if (args.Length>2) objectID = (string) args [2];

			if (args.Length > 3){
				try {
					objectValue = (string)args [3];
				} catch (Exception ex) {
					objectValueInt = (int)args[3];			
				};
			}
			if (args.Length>4) objectPosition = (string) args [4];

			long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
			ticks /= 10000000; //Convert windows ticks to seconds
				
			Analysis analyse = new Analysis();
			analyse.analyseEvent(eventType, eventName, objectID, objectValue, objectValueInt, objectPosition, ticks);

			Reasoning reasoning = new Reasoning();
			reasoning.processEvent();

			Feedback feedback = new Feedback();
			feedback.generateFeedbackMessage();

		}
		#endregion
		
		#region Protected Methods
		#endregion
		
		#region Public Methods
		#endregion
	}
}
