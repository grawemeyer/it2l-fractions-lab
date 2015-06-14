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
		public static String languageString = "";

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
			SaveEvent ("TDS.sendFeedbackTypeToSNA", feedbackType);
			Debug.Log ("sendFeedbackTypeToSNA:: "+feedbackType);
			Application.ExternalCall("sendFeedbackTypeToSNA", feedbackType);
		}

		public static void sendRepresentationTypeToSNA(String representation){
			SaveEvent ("TDS.sendRepresentationTypeToSNA", representation);
			Debug.Log ("sendRepresentationTypeToSNA:: "+representation);
			Application.ExternalCall("sendRepresentationTypeToSNA", representation);
		}

		public static void sendMessageToTIS(List<String> feedback, String currentFeedbackType, String feedbackID, int level, bool followed, bool viewed){
			String feedbackString = feedback [0];
			Debug.Log ("TDSWRAPPER: sendMessageToTIS:"+ feedbackString+" currentFeedbackType: "+currentFeedbackType+" feedbackID: "+feedbackID+" level: "+level+" followed: "+followed+" viewed: "+viewed);
			Application.ExternalCall("sendMessageToTIS", feedback, currentFeedbackType, feedbackID, level, followed, viewed);
		}

		public static void sendDoneButtonPressedToTIS(bool value){
			SaveEvent ("TDS.sendDoneButtonPressedToTIS", value.ToString());
			Debug.Log ("sendDoneButtonPressedToTIS:: "+value);
			Application.ExternalCall("sendDoneButtonPressedToTIS", value);
			if (studentModel == null) {
				studentModel = new StudentModel (taskID);
			}
			studentModel.setPopUpClosed (value);
		}

		public static void SendMessageToLightBulb(String feedbacktext){
			Debug.Log ("TDSWRAPPER: sendMessageToLightBulb: "+feedbacktext);
			Application.ExternalCall("sendMessageToLightBulb", feedbacktext);
		}

		public static void PlaySound(String message){
			Debug.Log ("playSound: "+message);
			SaveEvent ("TDS.PlaySound", message);
			Application.ExternalCall("playSound", message);
		}

		public static void SaveEvent(String id, String message){
			String result = id+":"+message;
			Debug.Log ("saveEvent: "+result);
			Application.ExternalCall("saveEvent", result);
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
			SaveEvent ("TDS.taskID", taskID);
			SaveEvent ("TDS.studentID", studentID);
			Debug.Log ("taskID: "+taskID);
			Debug.Log ("studentID: "+studentID);
			studentModel = new StudentModel (taskID);
			setLanguageInStudentModel(studentModel);
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
			SaveEvent ("TDS.DoneButtonEnable", value.ToString ());
			Debug.Log ("TDSWRAPPER: DoneButtonEnable: "+value);
			Application.ExternalCall("doneButtonEnable", value.ToString ());
			doneButtonEnabled = value;
		}

		public static void ArrowButtonEnable(bool value){
			Debug.Log ("TDSWRAPPER: ArrowButtonEnable: "+value);
			SaveEvent ("TDS.ArrowButtonEnable", value.ToString ());
			Application.ExternalCall("arrowButtonEnable", value.ToString ());
			arrowButtonEnabled = value;
		}

		private static void switchTISon(){
			SaveEvent ("TDS.TIS", "true");
			TIS = true;
		}

		private static void switchTISoff(){
			SaveEvent ("TDS.TIS", "false");
			TIS = false;
		}

		public static void setLanguage(String value){
			Debug.Log ("::::: setLanguage:::: "+value);
			languageString = value;
		}

		private static void setLanguageInStudentModel(StudentModel student){
			if (languageString.Equals ("de")) {
				switchGermanOn(student);
			}
			else if (languageString.Equals ("es")){
				switchSpanishOn(student);
			}
			else {
				switchEnghlishOn(student);
			}
		}

		private static void switchEnghlishOn(StudentModel student){
			student.setLanguageEnglish (true);
			student.setLanguageGerman (false);
			student.setLanguageSpanish (false);
		}

		private static void switchGermanOn(StudentModel student){
			student.setLanguageEnglish (false);
			student.setLanguageGerman (true);
			student.setLanguageSpanish (false);
		}

		private static void switchSpanishOn(StudentModel student){
			student.setLanguageEnglish (false);
			student.setLanguageGerman (false);
			student.setLanguageSpanish (true);
		}

		public static void SendMessageToSupport(params object[] args)
		{
				if (intelligentSupportOff)
						return;

				string eventType = "";
				string eventName = "";
				string objectID = "";
				string objectValue = "";
				string objectPosition = "";
				int objectValueInt = 0;

				if (args.Length > 0)
						eventType = args [0].ToString ();
				if (args.Length > 1)
						eventName = args [1].ToString ();
				if (args.Length > 2)
						objectID = args [2].ToString ();

				if (args.Length > 3) {
						objectValue = args [3].ToString ();
						try {
								objectValueInt = System.Convert.ToInt32 (objectValue);	
						} catch (Exception ex) {
						
						}
						;
				}
				if (args.Length > 4)
						objectPosition = args [4].ToString ();

				SaveEvent ("TDS.eventType", eventType);
				SaveEvent ("TDS.eventName", eventName);
				SaveEvent ("TDS.objectID", objectID);
				SaveEvent ("TDS.objectValue", objectValue);
				SaveEvent ("TDS.objectValueInt", objectValueInt.ToString());
				SaveEvent ("TDS.objectPosition", objectPosition);
				

				Debug.Log ("taskID: " + taskID);

				if (studentModel == null) {
						studentModel = new StudentModel (taskID);
						setLanguageInStudentModel (studentModel);
				}

				Analysis analyse = new Analysis ();
				analyse.analyseEvent (studentModel, eventType, eventName, objectID, objectValue, objectValueInt, objectPosition);

				Debug.Log ("eventType: " + eventType + " eventName:" + eventName);


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

				if (eventType.Equals ("FractionGenerated") || eventType.Equals ("FractionChange") 
						|| eventType.Equals ("OperationResult") || (eventType.Equals ("EquivalenceGenerated"))
			    		|| (eventType.Equals ("ClickButton") && !eventName.Equals ("CloseFeedbackPopup"))
			    		|| eventType.Equals ("FractionTrashed")) {
						Debug.Log ("FractionGenerated ||  FractionChange");
						Debug.Log ("needsNewThread " + needsNewThread);
						Debug.Log ("responseThread " + responseThread);
						Debug.Log ("counter " + counter);
						Debug.Log ("counter " + counter.getValue ());
						
					if (needsNewThread || (responseThread == null)) {
						responseThread = new Thread (new ThreadStart (handleEvent));
						responseThread.Start (); 
						needsNewThread = false;
					}
					

				} else if (doneButtonEnabled && eventType.Equals ("PlatformEvent") && 
						(eventName.Equals ("doneButtonPressed") || eventName.Equals ("*doneButtonPressed*"))) {
						Debug.Log (":::: doneButtonPressed ::::: ");
						Reasoning reasoning = new Reasoning (taskID, studentModel);
						reasoning.processDoneEvent ();

						FeedbackElem currentFeedback = studentModel.getCurrentFeedback ();
						Debug.Log (" :::: feedback ID after reasoning ::: " + currentFeedback.getID ());
				
						Feedback feedback = new Feedback ();
						feedback.setStudentModel (studentModel);
						feedback.setStudentID (studentID);
						feedback.generateFeedbackMessage ();
				} else if (eventType.Equals ("PlatformEvent") && eventName.Equals ("*lightBulbPressedON*")) {
						Debug.Log (":::: Light bulb pressed ON::::");
						messageViewed();
						studentModel.setPreviousViewed (true);
						sendDoneButtonPressedToTIS (false);
			
				} else if (eventType.Equals ("PlatformEvent") && eventName.Equals ("*lightBulbPressedOFF*")) {
						Debug.Log (":::: Light bulb pressed OFF::::");

						/*Reasoning reasoning = new Reasoning(taskID);
				reasoning.setStudentModel(studentModel);
				reasoning.processEvent();
				
				Feedback feedback = new Feedback();
				feedback.setStudentModel(studentModel);
				feedback.setStudentID(studentID);
				feedback.generateFeedbackMessage();

				needsNewThread = true;
				responseThread = null;*/
				} else if (eventType.Equals ("PlatformEvent") && eventName.Equals ("*switchTISOFF*")) {
						switchTISoff ();
				} else if (eventType.Equals ("PlatformEvent") && eventName.Equals ("*switchTISON*")) {
						switchTISon ();
				} else if (eventType.Equals ("ClickButton") && eventName.Equals ("CloseFeedbackPopup")) {
						sendDoneButtonPressedToTIS (true);
				} else if (eventType.Equals ("PlatformEvent") && eventName.Equals ("*switchEnglishON*")) {
						switchEnghlishOn (studentModel);	
				} else if (eventType.Equals ("PlatformEvent") && eventName.Equals ("*switchGermanON*")) {
						switchGermanOn (studentModel);	
				} else if (eventType.Equals ("PlatformEvent") && eventName.Equals ("*switchSpanishON*")) {
						switchSpanishOn (studentModel);	
				} else if (eventType.Equals ("PlatformEvent") && eventName.Equals ("*closeFeedbackPopup*")) {
						sendDoneButtonPressedToTIS (true);
				}
		

		}

		private static String getFeedbackTypeAsString(int feedbackType){
			String feedbackTypeString = "";
			if (feedbackType == FeedbackType.problemSolving) {
				feedbackTypeString = FeedbackType.problemSolvingString;
			}
			else if (feedbackType == FeedbackType.nextStep) {
				feedbackTypeString = FeedbackType.nextStepString;
			}
			else if (feedbackType == FeedbackType.affirmation) {
				feedbackTypeString = FeedbackType.affirmationString;
			}
			else if (feedbackType == FeedbackType.reflection) {
				feedbackTypeString = FeedbackType.reflectionString;
			}
			else if (feedbackType == FeedbackType.other) {
				feedbackTypeString = FeedbackType.otherString;
			}
			else if (feedbackType == FeedbackType.taskNotFinished) {
				feedbackTypeString = FeedbackType.taskNotFinishedString;
			}
			return feedbackTypeString;
		}


		public static void messageViewed(){
			if (!TIS) {
				FeedbackElem viewedMessageElem = studentModel.getFeedbackElemViewed ();
				String feedbackID = viewedMessageElem.getID ();
				int type = viewedMessageElem.getFeedbackType ();
				String feedbackType = getFeedbackTypeAsString (type);
				String message = studentModel.getViewedMessage ();
				SaveEvent ("TDS.wiewedMessage.id", feedbackID);
				SaveEvent ("TDS.wiewedMessage.type", feedbackType);
				SaveEvent ("TDS.wiewedMessage.message", message);
			}
		}

		private static void handleEvent()
		{
			try {
				while (counter.getValue ()< 400) {}
				if (counter.getValue () >= 400) {
					Debug.Log("counter > 400");
					Reasoning reasoning = new Reasoning(taskID, studentModel);
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
