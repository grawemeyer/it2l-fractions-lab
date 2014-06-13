using UnityEngine;
using System.Collections;
using System;
using taskDependentSupport.core;
using System.Threading;

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
		public static bool intelligentSupportOff = true;

		private static Counter counter; 
		#endregion
		
		#region Public Static Methods
		public static void SendBrowserMessage(params object[] args)
		{
			Application.ExternalCall("newEvent", args);
		}

		private static Thread t1;

		public static void SendMessageToSupport(params object[] args)
		{
			if (intelligentSupportOff) return;


			string eventType = "";
			string eventName = "";
			string objectID = "";
			string objectValue = "";
			string objectPosition = "";
			int objectValueInt = 0;

			if (args.Length>0) eventType = args [0].ToString ();
			if (args.Length>1) eventName = args [1].ToString ();
			if (args.Length>2) objectID = args [2].ToString ();

			if (args.Length > 3){
				objectValue = args [3].ToString ();
				try {
					objectValueInt = System.Convert.ToInt32(objectValue);	
				} catch (Exception ex) {
						
				};
			}
			if (args.Length>4) objectPosition = args [4].ToString ();

			long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
			ticks /= 10000000; //Convert windows ticks to seconds
			//Debug.Log ("EVENT: "+eventType+" name: "+eventName+" id: "+objectID+"objectValue: "+objectValue+" objectValueInt: "+objectValueInt);
			Analysis analyse = new Analysis();
			analyse.analyseEvent(eventType, eventName, objectID, objectValue, objectValueInt, objectPosition, ticks);


			if (counter == null) {
				counter = new Counter ();
				Thread counterThread = new Thread(new ThreadStart(counter.increaseCounter));
				counterThread.Start();
			}

			counter.resetCounter();

			if (eventType.Equals("FractionGenerated") || eventType.Equals("FractionChange")){
				Thread responseThread = new Thread (new ThreadStart (handleEvent));
				responseThread.Start (); 
			}

		}


		private static void handleEvent()
		{
			try {
				while (counter.getValue ()< 400) {}
				if (counter.getValue () >= 400) {
					Reasoning reasoning = new Reasoning();
					reasoning.processEvent();
				
					Feedback feedback = new Feedback();
					feedback.generateFeedbackMessage();
				}
			} 
			catch (ThreadAbortException e){}
		}
		#endregion
		
		#region Protected Methods
		#endregion
		
		#region Public Methods
		#endregion
	}
}
