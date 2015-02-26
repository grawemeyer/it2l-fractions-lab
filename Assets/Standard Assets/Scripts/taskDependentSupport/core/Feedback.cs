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

		public void calculatePresentationOfFeedback(int feedbackType){
			presentationMode = "lightBulb";

			if ((feedbackType == FeedbackType.affirmation) || (feedbackType == FeedbackType.taskNotFinished)) {
				presentationMode = "high";
			}
		}


		private String getFeedbackTypeAsString(int feedbackType){
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
					calculatePresentationOfFeedback (studentModel.getlastDisplayedMessageType());
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
