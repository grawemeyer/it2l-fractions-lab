
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

			if (messageID == 4) feedbackMessage = "You have forgotten to change the numerator as well";
			if (messageID == 5) feedbackMessage = "If you use the partition tool, what do you notice?";// Can you make a denominator of 12 using the partition tool?";
			if (messageID == 6) feedbackMessage = "Make a fraction equivalent to 3/4. It should have 12 as a denominator";
			if (messageID == 7) feedbackMessage = "You have made 12 the numerator. Make 12 the denominator";


			return feedbackMessage;
		}

	}
}

