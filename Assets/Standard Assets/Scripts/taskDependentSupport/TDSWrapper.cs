using UnityEngine;
using System.Collections;
using System;
using taskDependentSupport.core;
using System.Threading;
using System.Collections.Generic;



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
		public static bool TIS = false;
		public static String taskID = "";
		public static String studentID = "";
        public static String taskInformationPackage;
		public static Thread responseThread;
		public static bool doneButtonEnabled = true;
		public static bool arrowButtonEnabled = false;
		public static bool needsNewThread = true;

		private static StudentModel studentModel;

		public static Counter counter; 

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

		public static void SendHighMessage(String feedbacktext, String currentFeedbackType, String previousFeedbackType, bool followed){
			Debug.Log ("SendHighMessage: SendHighMessage: "+feedbacktext);
			Application.ExternalCall("SendHighMessage", feedbacktext);
		}

		public static void sendFeedbackTypeToSNA(String feedbackType){
			Debug.Log ("sendFeedbackTypeToSNA:: "+feedbackType);
			Application.ExternalCall("sendFeedbackTypeToSNA", feedbackType);
		}

		public static void sendMessageToTIS(List<String> feedback, String currentFeedbackType, int level, bool followed, bool viewed){
			String feedbackString = feedback [0];
			Debug.Log ("TDSWRAPPER: sendMessageToTIS:"+ feedbackString+" currentFeedbackType: "+currentFeedbackType+" level: "+level+" followed: "+followed+" viewed: "+viewed);
			Application.ExternalCall("sendMessageToTIS", feedback, currentFeedbackType, level, followed, viewed);
		}

		public static void SendMessageToLightBulb(String feedbacktext){
			Debug.Log ("TDSWRAPPER: sendMessageToLightBulb: "+feedbacktext);
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

			String elem = arg.ToString ();
				String checkTaskID = elem.Substring(0,7);
			if (checkTaskID.Equals ("task1.1")) {
				taskID = elem.Substring(0,11);
				studentID = elem.Substring(11);
			}
			else if (checkTaskID.Equals ("task2.1")) {
				taskID = elem.Substring(0,7);
				studentID = elem.Substring(7);
			}
			else if (checkTaskID.Equals ("task2.2")){
				taskID = elem.Substring(0,7);
				studentID = elem.Substring(7);
			}
			else if (checkTaskID.Equals ("task2.4")){
				taskID = elem.Substring(0,17);
				studentID = elem.Substring(17);
			}
			else if (checkTaskID.Equals ("task2.6")){
				taskID = elem.Substring(0,12);
				studentID = elem.Substring(12);
			}
			else if (checkTaskID.Equals ("task2.7")){
				taskID = elem.Substring(0,12);
				studentID = elem.Substring(12);
			}
			else if (checkTaskID.Equals ("task3aP")){
				taskID = elem.Substring(0,22);
				studentID = elem.Substring(22);
			}
			//task3aPlus.1.setA.area
			Debug.Log ("taskID: "+taskID);
			Debug.Log ("studentID: "+studentID);
			studentModel = new StudentModel (taskID);
			studentModel.resetDoneButtonPressed();
		
			if (checkTaskID.Equals ("task2.7")) {
				//needs to be checked if these buttons have to be set for all tasks
				DoneButtonEnable(true);
				ArrowButtonEnable(false);
			}
		}

		public static void SetTaskInformationPackage(String _tip) 
		{
			taskInformationPackage = _tip;
		}
		
		public static void DoneButtonEnable(bool value){
			Debug.Log ("TDSWRAPPER: DoneButtonEnable: "+value);
			Application.ExternalCall("doneButtonEnable", value.ToString ());
			doneButtonEnabled = value;
		}

		public static void ArrowButtonEnable(bool value){
			Debug.Log ("TDSWRAPPER: ArrowButtonEnable: "+value);
			Application.ExternalCall("arrowButtonEnable", value.ToString ());
			arrowButtonEnabled = value;
		}

		private static void switchTISon(){
			TIS = true;
		}

		private static void switchTISoff(){
			TIS = false;
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

			SaveEvent (ticks+";eventType:"+eventType+";eventName:"+eventName+";objectID:"+objectID+";objectValue:"+objectValue+";objectValueInt:"+objectValueInt+";objectPosition:"+objectPosition+";");

			Debug.Log ("taskID: "+taskID);

			if (studentModel == null) {
				studentModel = new StudentModel (taskID);
			}

			Analysis analyse = new Analysis ();
			analyse.analyseEvent (studentModel, eventType, eventName, objectID, objectValue, objectValueInt, objectPosition, ticks);

			Debug.Log ("eventType: "+eventType+" eventName:"+eventName);

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
				Debug.Log (":::: doneButtonPressed ::::: ");
				Reasoning reasoning = new Reasoning(taskID);
				reasoning.setStudentModel(studentModel);
				reasoning.processEvent();
				reasoning.processDoneEvent();

				FeedbackElem currentFeedback = studentModel.getCurrentFeedback();
				Debug.Log (" :::: feedback ID after reasoning ::: "+currentFeedback.getID());
				
				Feedback feedback = new Feedback();
				feedback.setStudentModel(studentModel);
				feedback.setStudentID(studentID);
				feedback.generateFeedbackMessage();
			}
			else if (eventType.Equals ("PlatformEvent") && eventName.Equals ("*lightBulbPressedON*")){
				Debug.Log (":::: Light bulb pressed ON::::");
				studentModel.setPreviousViewed(true);
			
			}
			else if (eventType.Equals ("PlatformEvent") && eventName.Equals ("*lightBulbPressedOFF*")){
				Debug.Log (":::: Light bulb pressed OFF::::");

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
			else if (eventType.Equals ("PlatformEvent") && eventName.Equals ("*switchTISOFF*")){
				switchTISoff();
			}
			else if (eventType.Equals ("PlatformEvent") && eventName.Equals ("*switchTISON*")){
				switchTISon();
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
