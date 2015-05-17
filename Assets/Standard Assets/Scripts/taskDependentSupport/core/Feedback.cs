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
			FeedbackElem feedback = studentModel.getCurrentFeedback ();
			String feedbackID = feedback.getID ();
			String lastReflectiveIdentifier = "E";
			Boolean lastReflectivePrompt = feedbackID.Contains (lastReflectiveIdentifier);
			Debug.Log ("feedbackType: "+feedbackType+" feedbackID: "+feedbackID+ " lastReflectivePrompt: "+lastReflectivePrompt);

			if (lastReflectivePrompt || (feedbackType == FeedbackType.affirmation) || (feedbackType == FeedbackType.taskNotFinished)) {
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
			String feedbackMessage = FeedbackStrategyModel.getFeedbackMessage();


			Debug.Log ("feedbackMessage: "+feedbackMessage);

			long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
			ticks /= 10000000; //Convert windows ticks to seconds

			FeedbackElem currentFeedbackTest = studentModel.getCurrentFeedback();
			Debug.Log (" :::: feedback ID in feedback ::: "+currentFeedbackTest.getID());

			FeedbackElem currentFeedback = studentModel.getCurrentFeedback();
			String feedbackType = getFeedbackTypeAsString(currentFeedback.getFeedbackType());


			if (!feedbackMessage.Equals ("")) {
				int level = studentModel.getCurrentFeedbackLevel();
				String feedbackID = currentFeedback.getID();
				String feedbackMessgageRule = studentModel.getMessageRule ();
				String feedbackFollowedRule = studentModel.getFeedbackFollowedRule ();
				String feedbackCounterRule = studentModel.getFeedbackCounterRule ();

				taskDependentSupport.TDSWrapper.SaveEvent ("TDS.feedbackMessageRule", feedbackMessgageRule);
				taskDependentSupport.TDSWrapper.SaveEvent ("TDS.feedbackFollowedRule", feedbackFollowedRule);
				taskDependentSupport.TDSWrapper.SaveEvent ("TDS.feedbackCounterRule", feedbackCounterRule);
				taskDependentSupport.TDSWrapper.SaveEvent ("TDS.message", feedbackMessage);
				taskDependentSupport.TDSWrapper.SaveEvent ("TDS.messageID", feedbackID);
				taskDependentSupport.TDSWrapper.SaveEvent ("TDS.messageType", feedbackType);
				taskDependentSupport.TDSWrapper.SaveEvent ("TDS.level", level.ToString());
				if (taskDependentSupport.TDSWrapper.TIS){
					List<bool> feedbackFollowed = studentModel.getFeedbackFollowed();
					bool followed = feedbackFollowed[feedbackFollowed.Count-1];
					bool previousViewed = studentModel.getPreviousViewed();
					taskDependentSupport.TDSWrapper.SaveEvent ("TDS.followed", followed.ToString ());
					taskDependentSupport.TDSWrapper.SaveEvent ("TDS.previousViewed", previousViewed.ToString ());
					taskDependentSupport.TDSWrapper.SaveEvent ("TDS.MessageSend","true");
					List<String> feedback = getFeedbackAsList(currentFeedback);
					taskDependentSupport.TDSWrapper.sendMessageToTIS(feedback, feedbackType, feedbackID, level, followed, previousViewed);
				}
				else {
					calculatePresentationOfFeedback (studentModel.getlastDisplayedMessageType());

					if (presentationMode.Equals ("lightBulb")){
						taskDependentSupport.TDSWrapper.sendFeedbackTypeToSNA(feedbackType);
						taskDependentSupport.TDSWrapper.SaveEvent ("TDS.presentation","lightBulbMessage");
						taskDependentSupport.TDSWrapper.SaveEvent ("TDS.MessageSend","true");
						taskDependentSupport.TDSWrapper.SendMessageToLightBulb(feedbackMessage);
					}

					else if (studentModel.getPopUpClosed()){
						taskDependentSupport.TDSWrapper.sendFeedbackTypeToSNA(feedbackType);
						if (presentationMode.Equals ("low")) {
							taskDependentSupport.TDSWrapper.SaveEvent ("TDS.presentation","low");
							taskDependentSupport.TDSWrapper.SaveEvent ("TDS.MessageSend","true");
							sendLowMessage (feedbackMessage);

						} else if (presentationMode.Equals ("high")) {
							taskDependentSupport.TDSWrapper.SaveEvent ("TDS.presentation","popUp");
							taskDependentSupport.TDSWrapper.SaveEvent ("TDS.MessageSend","true");
							Debug.Log ("send HIGH message");
							sendHighMessage (feedbackMessage);
							studentModel.setPopUpClosed(false);
						}
					}
					else if (!studentModel.getPopUpClosed()){
						taskDependentSupport.TDSWrapper.SaveEvent ("TDS.NOTsend","popUpIsNotClosed");
					
					}
				}
				//check when this should be called
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
