using UnityEngine;
using System.Collections;

namespace taskDependentSupport.core
{

	public class Feedback 
	{

		public void generateFeedbackMessage(){
			string feedbackMessage = FeedbackStrategyModel.getFeedbackMessage();
			string messageType = FeedbackStrategyModel.getMessageType();
			int messageID = FeedbackStrategyModel.getMessageID();

			if (messageID != 0) {
				StudentModel.setDisplaydMessageID(messageID);
				StudentModel.setDisplayedMessageType(messageType);
			}

			if (messageType.Equals("low"))
			{
				sendLowMessage(feedbackMessage);
			}
			else if (messageType.Equals("high"))
			{
				sendHighMessage(feedbackMessage);
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
