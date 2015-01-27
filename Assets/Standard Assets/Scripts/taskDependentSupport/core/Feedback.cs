using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace taskDependentSupport.core
{

	public class Feedback 
	{

		private string studentID="";
		private StudentModel studentModel;
		private String presentationMode = ";lightBulbMessage:";
		
		public void setStudentModel(StudentModel elem){
			studentModel = elem;
		}

		public void setStudentID(string value){
			studentID = value;
		}

		public void calculatePresentationOfFeedback(){
			//needs to be set by the task-independent support
			//presentationMode = "lightBulb";

			presentationMode = "high";

			//if the student has completed the exercise then the affect boost should be 
			//provided in pop-up

			//the reflective prompt at the end of the task should be
			//provided as pop-up

		}

		private String getFeedbackTypeAsString(int feedbackType){
			String feedbackTypeString = "";
			if ((feedbackType == 1) || (feedbackType == 3)) feedbackTypeString="PROBLEM_SOLVING";
			else if (feedbackType == 2) feedbackTypeString="REFLECTION";
			else if (feedbackType == 4) feedbackTypeString="AFFIRMATION";
			else if (feedbackType == 5) feedbackTypeString="OTHER";
			return feedbackTypeString;
		}

		public void generateFeedbackMessage(){
			Debug.Log ("generateFeedbackMessage");
			string feedbackMessage = FeedbackStrategyModel.getFeedbackMessage();
			Debug.Log ("feedbackMessage: "+feedbackMessage);



			long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
			ticks /= 10000000; //Convert windows ticks to seconds

			if (!feedbackMessage.Equals ("")) {
				if (taskDependentSupport.TDSWrapper.TIS){
					List<bool> feedbackFollowed = studentModel.getFeedbackFollowed();
					bool followed = feedbackFollowed[feedbackFollowed.Count-1];

					List<FeedbackElem> feedbackProvided = studentModel.getFeedbackProvided();
					int feedbackProvidedLength = feedbackProvided.Count;
					int previousFeedbackType =0;
					if (feedbackProvidedLength > 1){
						FeedbackElem previousFeedback = feedbackProvided[feedbackProvided.Count-2];
						previousFeedbackType = previousFeedback.getFeedbackType();
					}
					FeedbackElem currentFeedback = feedbackProvided[feedbackProvided.Count-1];
					int currentFeedbackType = currentFeedback.getFeedbackType();
					String previousFeedbackTypeString = getFeedbackTypeAsString(previousFeedbackType);
					String currentFeedbackTypeString = getFeedbackTypeAsString(currentFeedbackType);

					taskDependentSupport.TDSWrapper.sendMessageToTIS(feedbackMessage, currentFeedbackTypeString, previousFeedbackTypeString, followed);
				}
				else {
					calculatePresentationOfFeedback ();
					if (presentationMode.Equals ("lightBulb")){
						taskDependentSupport.TDSWrapper.SaveEvent (ticks + ";lightBulbMessage:" + feedbackMessage + ";");
						taskDependentSupport.TDSWrapper.SendMessageToLightBulb(feedbackMessage);
					}

					else if (presentationMode.Equals ("low")) {
						taskDependentSupport.TDSWrapper.SaveEvent (ticks + ";lowMessage:" + feedbackMessage + ";");
						sendLowMessage (feedbackMessage);
					} else if (presentationMode.Equals ("high")) {
						taskDependentSupport.TDSWrapper.SaveEvent (ticks + ";highMessage:" + feedbackMessage + ";");
						Debug.Log ("send HIGH message");
						sendHighMessage (feedbackMessage);
					}
				}
			}
		}

		private void sendHighMessage(string message) 
		{
			var json = "{\"method\": \"HighFeedback\", \"parameters\": {\"message\": \"" + message +"\"}}";

			taskDependentSupport.TDSWrapper.eventManager.SendMessage("SendEvent", json);
			taskDependentSupport.TDSWrapper.PlaySound(message);
		}
		
		
		private void sendLowMessage(string message) 
		{
			var json = "{\"method\": \"LowFeedback\", \"parameters\": {\"message\": \"" + message +"\"}}";
			taskDependentSupport.TDSWrapper.eventManager.SendMessage("SendEvent", json);
			taskDependentSupport.TDSWrapper.PlaySound(message);
			Debug.Log ("::::: sendLowMessage");
		}

		private void highlightItem(string itemID)
		{
		
		}

		private void playSound(string message)
		{
		
		}

	}
}
