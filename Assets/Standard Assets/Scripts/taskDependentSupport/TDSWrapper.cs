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
		public static bool intelligentSupportOff = false;
		public static String taskID = "";
		public static String studentID = "";
		public static Thread responseThread;
		public static bool doneButtonEnabled = false;
		public static bool arrowButtonEnabled = true;
		public static bool needsNewThread = true;

		private static StudentModel studentModel;

		public static Counter counter; 

		//for testing:
		private static Counter testCounter; 
		#endregion
		
		#region Public Static Methods
		public static void StopThreads(){
			Debug.Log ("STOP THREADS");
			responseThread = null;
			counter = null;
		}


		public static void SendBrowserMessage(params object[] args)
		{
			Application.ExternalCall("newEvent", args);
		}

		public static void SendMessageToLightBulb(String feedbacktext){
			Debug.Log ("sendMessageToLightBulb: "+feedbacktext);
			Application.ExternalCall("sendMessageToLightBulb", feedbacktext);
		}

		public static void PlaySound(String message){
			Debug.Log ("playSound: "+message);
			Application.ExternalCall("playSound", message);
		}

		public static void SaveEvent(String message){
			Debug.Log ("saveEvent: "+message);
			Application.ExternalCall("saveEvent", message);
		}

		public static void setTaskID(object arg){
			Debug.Log ("!!!!!!!!!!! setTaskID: "+arg);
			Debug.Log ("setTaskID: "+arg);
			//Comp1, EQUIValence1 or EQUIValence2
			String elem = arg.ToString ();
			String checkTaskID = elem.Substring(0,4);
			if (checkTaskID.Equals ("Comp")) {
				taskID = elem.Substring(0,5);
				studentID = elem.Substring(5);
			}
			else if (checkTaskID.Equals ("EQUI")){
				taskID = elem.Substring(0,12);
				studentID = elem.Substring(12);
			}

			//taskID = arg.ToString();
			Debug.Log ("taskID: "+taskID);
			Debug.Log ("studentID: "+studentID);
			//if (studentModel == null) 
			studentModel = new StudentModel (taskID);
			studentModel.resetDoneButtonPressed();
			//Debug.Log ("studentModel setFeedbackData 1 ");
			//studentModel.setFeedbackData (new FeedbackData ());

			if (taskID.Equals ("EQUIValence1")) {
				DoneButtonEnable(true);
				ArrowButtonEnable(false);
			}
		}

		public static void DoneButtonEnable(bool value){
			Debug.Log ("DoneButtonEnable: "+value);
			Application.ExternalCall("doneButtonEnable", value.ToString ());
			doneButtonEnabled = value;
		}

		public static void ArrowButtonEnable(bool value){
			Debug.Log ("ArrowButtonEnable: "+value);
			Application.ExternalCall("arrowButtonEnable", value.ToString ());
			arrowButtonEnabled = value;
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

			SaveEvent (ticks+";eventType:"+eventType+";eventName:"+eventName+";objectID:"+objectID+";objectValue:"+objectValue+";objectValueInt:"+objectValueInt+";objectPosition:"+objectPosition+";");

			Debug.Log ("taskID: "+taskID);

			if (studentModel == null) {
				studentModel = new StudentModel (taskID);
				//Debug.Log ("studentModel setFeedbackData 2 ");
				//studentModel.setFeedbackData (new FeedbackData ());
			}

			Analysis analyse = new Analysis ();
			analyse.analyseEvent (studentModel, eventType, eventName, objectID, objectValue, objectValueInt, objectPosition, ticks);

			if (counter == null) {
				Debug.Log ("counter == null");
				counter = new Counter ();
				Thread counterThread = new Thread (new ThreadStart (counter.increaseCounter));
				counterThread.Start ();
			}

			counter.resetCounter ();

			//for testing
			/*if (taskID.Equals("EQUIValence1")){
				if (testCounter == null) {
					Debug.Log ("testCounter == null");
					testCounter = new Counter ();
					Thread testCounterThread = new Thread (new ThreadStart (testCounter.sendMessage));
					testCounterThread.Start ();
				}
			}*/

			if (eventType.Equals ("FractionGenerated") || eventType.Equals ("FractionChange") || eventType.Equals ("OperationResult")) {
				Debug.Log ("FractionGenerated ||  FractionChange");
				Debug.Log ("needsNewThread "+needsNewThread);
				Debug.Log ("responseThread "+responseThread);
				Debug.Log ("counter "+counter);
				Debug.Log ("counter "+counter.getValue());
				if (needsNewThread || (responseThread == null)){
					responseThread = new Thread (new ThreadStart (handleEvent));
					responseThread.Start (); 
					needsNewThread = false;
				}
			}

			else if (doneButtonEnabled && eventType.Equals ("PlatformEvent") && 
			         (eventName.Equals ("doneButtonPressed") || eventName.Equals ("*doneButtonPressed*"))){
				Debug.Log ("doneButtonPressed");
				studentModel.setDoneButtonPressed ();
				Reasoning reasoning = new Reasoning(taskID);
				reasoning.setStudentModel(studentModel);
				reasoning.processEvent();
				reasoning.processDoneEvent();
				
				Feedback feedback = new Feedback();
				feedback.setStudentModel(studentModel);
				feedback.setStudentID(studentID);
				feedback.generateFeedbackMessage();
			}
			else if (eventType.Equals ("PlatformEvent") && (eventName.Equals ("lightBulbPressed") || eventName.Equals ("*lightBulbPressed*"))){
				Debug.Log (":::: Light bulb pressed ::::");
			}

		}

		private static void handleEvent()
		{
			try {
				while (counter.getValue ()< 400) {}
				if (counter.getValue () >= 400) {
					Debug.Log("counter > 400");
					Reasoning reasoning = new Reasoning(taskID);
					reasoning.setStudentModel(studentModel);
					reasoning.processEvent();

					Feedback feedback = new Feedback();
					feedback.setStudentModel(studentModel);
					feedback.setStudentID(studentID);
					feedback.generateFeedbackMessage();

					needsNewThread = true;
					responseThread = null;
				}
			} 
			catch (ThreadAbortException e){
				needsNewThread = true;
				Debug.Log (e);
			}
		}

		#endregion
		
		#region Protected Methods
		#endregion
		
		#region Public Methods
		#endregion
	}
}
