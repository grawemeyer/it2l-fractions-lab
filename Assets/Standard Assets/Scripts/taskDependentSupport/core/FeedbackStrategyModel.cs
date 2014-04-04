
using System;
namespace taskDependentSupport.core
{

	public class FeedbackStrategyModel
	{
			
		private static int messageID = 0;
		private static string messageType = ""; 


		public static void setMessage(int id, string type)
		{
			messageID = id;
			messageType = type;
		}

		public static int getMessageID()
		{
			return messageID;
		}

		public static void setMessageID (int value)
		{
			messageID = value;
		}

		public static string getMessageType()
		{
			return messageType;
		}
		
		public static void setMessageType(string value)
		{
			messageType = value;
		}

		public static string getFeedbackMessage()
		{
			string feedbackMessage = "";
			
			if (messageID == 1) feedbackMessage = "Please use the quivalent box to compare the fractions.";
			if (messageID == 2) feedbackMessage = "Great! Well done.";
			if (messageID == 3) feedbackMessage = "Drag two fractions here to test equivalence";

			return feedbackMessage;
		}

	}
}

