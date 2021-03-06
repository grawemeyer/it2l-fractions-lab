
using System;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace taskDependentSupport.core
{

	public class FeedbackStrategyModel
	{

		private static FeedbackElem currentFeedback = new FeedbackElem ();
		private static int currentFeedbackPointer = 0;

		public static void setCurrentFeedback(FeedbackElem feedback, int pointer){
			currentFeedback = feedback;
			currentFeedbackPointer = pointer;
		}

		public static string getFeedbackMessage()
		{
			Debug.Log (" :::: feedback ID in strategy model ::: "+currentFeedback.getID()+" pointer::: "+currentFeedbackPointer);

			if (currentFeedbackPointer == 1) {
				return currentFeedback.getFeedbackMessage ().getSocratic ();
			}
			else if (currentFeedbackPointer == 2) {
				return currentFeedback.getFeedbackMessage ().getGuidance ();

			}
			else if (currentFeedbackPointer == 3){
				return currentFeedback.getFeedbackMessage ().getDidacticConceptual ();
			}
			else return currentFeedback.getFeedbackMessage ().getDidacticProcedural();
		}
	}
}

