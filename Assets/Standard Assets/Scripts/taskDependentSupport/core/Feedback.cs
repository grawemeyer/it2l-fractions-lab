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

		private List<String> getFeedbackAsList(FeedbackElem currentFeedback){
			List<String> result = new List<String>();
			FeedbackMessage feedback = currentFeedback.getFeedbackMessage ();
			String guidance = feedback.getGuidance ();
			String socratic = feedback.getSocratic ();
			String didacticConceptual = feedback.getDidacticConceptual ();
			String didacticProcedural = feedback.getDidacticProcedural ();

			result.Add (socratic);
			result.Add (guidance);
			result.Add (didacticConceptual);
			result.Add (didacticProcedural);
		
			return result;
		}

		public void generateFeedbackMessage(){
			Debug.Log ("generateFeedbackMessage");
			string feedbackMessage = FeedbackStrategyModel.getFeedbackMessage();
			Debug.Log ("feedbackMessage: "+feedbackMessage);

			long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
			ticks /= 10000000; //Convert windows ticks to seconds

			FeedbackElem currentFeedbackTest = studentModel.getCurrentFeedback();
			Debug.Log (" :::: feedback ID in feedback ::: "+currentFeedbackTest.getID());

			if (!feedbackMessage.Equals ("")) {
				if (taskDependentSupport.TDSWrapper.TIS){
					List<bool> feedbackFollowed = studentModel.getFeedbackFollowed();
					bool followed = feedbackFollowed[feedbackFollowed.Count-1];
					bool previousViewed = studentModel.getPreviousViewed();
					int level = studentModel.getCurrentFeedbackLevel();
					FeedbackElem currentFeedback = studentModel.getCurrentFeedback();
					List<String> feedback = getFeedbackAsList(currentFeedback);
					String feedbackType = getFeedbackTypeAsString(currentFeedback.getFeedbackType());

					taskDependentSupport.TDSWrapper.sendMessageToTIS(feedback, feedbackType, level, followed, previousViewed);
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
				studentModel.setPreviousViewed (false);
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

	}
}
