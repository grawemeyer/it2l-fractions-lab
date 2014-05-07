using UnityEngine;
using System.Collections;

namespace taskDependentSupport.core
{

	public class Feedback 
	{

		public void generateFeedbackMessage(){
			string feedbackMessage = FeedbackStrategyModel.getFeedbackMessage();
			string messageType = FeedbackStrategyModel.getMessageType();
			
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
			//test
			var json = "{\"method\": \"HighFeedback\", \"parameters\": {\"message\": \"" + message +"\"}}";
			taskDependentSupport.TDSWrapper.eventManager.SendMessage("SendEvent", json);
		}
		
		
		private void sendLowMessage(string message) 
		{
			var json = "{\"method\": \"LowFeedback\", \"parameters\": {\"message\": \"" + message +"\"}}";
			taskDependentSupport.TDSWrapper.eventManager.SendMessage("SendEvent", json);
		}

	}
}
