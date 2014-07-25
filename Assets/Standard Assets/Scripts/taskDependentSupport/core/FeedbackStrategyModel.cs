
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
			if (messageID == 2) feedbackMessage = "The way you worked that out was very good. Well done! Now click the DONE button.";
			if (messageID == 3) feedbackMessage = "Drag two fractions here to test equivalence";

			if (messageID == 4) feedbackMessage = "You have forgotten to change the numerator as well";
			if (messageID == 5) feedbackMessage = "Use the partition tool, what do you notice? Can you make a denominator of 12?";
			if (messageID == 6) feedbackMessage = "Make a fraction equivalent to 3/4. It should have 12 as a denominator";
			if (messageID == 7) feedbackMessage = "You have made 12 the numerator. Make 12 the denominator";

			if (messageID == 8) feedbackMessage = "It seems like you haven't completed the task";
			if (messageID == 9) feedbackMessage = "If you need more help to finish the task, ask your teacher";
			if (messageID == 10) feedbackMessage = "What do you notice about the relationship between 4 and 12?";
			if (messageID == 11) feedbackMessage = "What did you notice about the coloured parts and the uncoloured parts?";// What stayed the same and what changed?";

			if (messageID == 12) feedbackMessage = "Make a fraction with 12 as denominator. The fraction should be equivalent to 3/4.";
			if (messageID == 13) feedbackMessage = "Click the up arrow next to the empty fraction, to make the denominator 12.";
			if (messageID == 14) feedbackMessage = "Check that your denominator is 12";
			if (messageID == 15) feedbackMessage = "You have made the numerator 12. Please make the denominator 12 instead.";
			if (messageID == 16) feedbackMessage = "How can you remember which part of the fraction is the denominator, and which part is the numerator?";
			if (messageID == 17) feedbackMessage = "Check that your fraction equals 3/4. Make another fraction, this time 3/4. Then compare the two fractions.";
			if (messageID == 18) feedbackMessage = "Compare your two fractions using the comparison box.";
			if (messageID == 19) feedbackMessage = "What do you notice about these two fractions?";
			if (messageID == 20) feedbackMessage = "Now change the numerator to make a fraction that is equivalent to 3/4.";
			if (messageID == 21) feedbackMessage = "Use the partition tool to make the denominator 12.";
			if (messageID == 22) feedbackMessage = "Right-click the representation and choose 'Show/hide partition'";
			if (messageID == 23) feedbackMessage = "What has happened to the numerator and denominator? Have they been affected the same or differently?";
			if (messageID == 24) feedbackMessage = "Why did you make the fraction 9/12? What did you do to the numerator and denominator of 3/4?";
		
			//feedback to click next button if 30 seconds were spend on the reflective prompt
			//send only as text no voice.
			if (messageID == 100) feedbackMessage = "When you have finished click the arrow button to go to the next task.";

			return feedbackMessage;
		}

	}
}

