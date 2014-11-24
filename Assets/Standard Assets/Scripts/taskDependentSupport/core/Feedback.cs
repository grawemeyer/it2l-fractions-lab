using UnityEngine;
using System.Collections;
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
			presentationMode = "lightBulb";

			//if student did not follow the advice AND did not click on the light bulb 2 x
			//then FRUSTEATION -> reflective prompt in pop-up window -> consequtive messages in pop-up as well?

			//if student quickly went on light bulb several times (3x) 
			//then CONFUSION -> prompts in pop-up window

			//if student follows advice 2 x  
			//then ENJOYMENT -> prompts as light bulb

		}

		public void generateFeedbackMessage(){
			Debug.Log ("generateFeedbackMessage");
			string feedbackMessage = FeedbackStrategyModel.getFeedbackMessage();
			Debug.Log ("feedbackMessage: "+feedbackMessage);

			calculatePresentationOfFeedback ();

			long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
			ticks /= 10000000; //Convert windows ticks to seconds

			if (!feedbackMessage.Equals ("")) {
				if (presentationMode.Equals ("lightBulb") || studentID.Equals("student1") || studentID.Equals("Student1")){
					taskDependentSupport.TDSWrapper.SaveEvent (ticks + ";lightBulbMessage:" + feedbackMessage + ";");
					taskDependentSupport.TDSWrapper.SendMessageToLightBulb(feedbackMessage);
				}

				else if (presentationMode.Equals ("low")) {
					taskDependentSupport.TDSWrapper.SaveEvent (ticks + ";lowMessage:" + feedbackMessage + ";");
					sendLowMessage (feedbackMessage);
				} else if (presentationMode.Equals ("high")) {
					taskDependentSupport.TDSWrapper.SaveEvent (ticks + ";highMessage:" + feedbackMessage + ";");
					sendHighMessage (feedbackMessage);
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
		}

		private void highlightItem(string itemID)
		{
		
		}

		private void playSound(string message)
		{
		
		}

	}
}
