using UnityEngine;
using System.Collections;
using System;

namespace taskDependentSupport.core
{

	public class Feedback 
	{

		public void generateFeedbackMessage(){
			string feedbackMessage = FeedbackStrategyModel.getFeedbackMessage();
			string messageType = FeedbackStrategyModel.getMessageType();
			int messageID = FeedbackStrategyModel.getMessageID();

			long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
			ticks /= 10000000; //Convert windows ticks to seconds

			if (messageID != 0) {
				StudentModel.setDisplaydMessageID(messageID);
				StudentModel.setDisplayedMessageType(messageType);
			}


			if (!feedbackMessage.Equals ("")) {
				if (messageType.Equals ("low")) {
					taskDependentSupport.TDSWrapper.SaveEvent (ticks + ";lowMessage:" + feedbackMessage + ";");
					sendLowMessage (feedbackMessage);
				} else if (messageType.Equals ("high")) {
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
