using UnityEngine;
using System.Collections;
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
			string evenType = "";
			
			if (args.Length>0) eventType = (string) args [0];
			if (args.Length>1) eventName = (string) args [1];
			if (args.Length>2) eventType = (string) args [2];

			Debug.Log (" hier in SendMessageToSupport");
			Debug.Log (" hier in SendMessageToSupport eventType: "+eventType);
			Debug.Log (" hier in SendMessageToSupport eventName: "+eventName);
			Debug.Log (" hier in SendMessageToSupport eventType: "+eventType);
			
			Analysis analyse = new Analysis();
			analyse.analyseEvent(eventType, eventName, eventType);

			Reasoning reasoning = new Reasoning();
			reasoning.processEvent();

			Feedback feedback = new Feedback();
			feedback.generateFeedbackMessage();


			//string result = "";
			//string test = (string) args [args.Length - 1];

			//for (int i = 0; i < args.Length; i++) {
			//	result += " "+args[i]; 
				//result =args[i]; 
			//}
			//if (test.Equals ("Equivalence")) {
				//System.Console.WriteLine ("Equivalence!!! ");
				//if (eventManager != null) {
				//	string message = "testing";
				//	string json = "{\"method\": \"HighFeedback\", \"parameters\": {\"message\": \"" + message +"\"}}";
					//u.getUnity().SendMessage("ExternalInterface", "SendEvent", json);
				//	eventManager.SendMessage("SendEvent", json);
				//}
				//else {
				//	System.Console.WriteLine ("eventManager == null");
				//}
			//}
			

			//System.Console.WriteLine("hier in wrapper "+result);
		}
		#endregion
		
		#region Protected Methods
		#endregion
		
		#region Public Methods
		#endregion
	}
}
