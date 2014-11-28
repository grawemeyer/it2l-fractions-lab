using System;
using UnityEngine;
using System.Collections;
namespace taskDependentSupport.core

{
		public class FeedbackElem
		{
			private String id;
			private FeedbackMessage message;
			private int type;
			private Fraction nextSteps;
			private int counter;
			
			public FeedbackElem (){
				id = "";
				message = new FeedbackMessage();
				nextSteps = new Fraction();
				type = 0;
				counter = 0;
			}


			public void setCounter(int elem){
				counter = elem;
				Debug.Log ("setCounter: "+id+" "+counter);
			}

			public int getCounter(){
				Debug.Log ("getCounter: "+id+" "+counter);
				return counter;
			}

			public void setID(String value){
				id = value;
			}

			public void setFeedbackMessage(FeedbackMessage value){
				message = value;
			}
			
			public void setFeedbackType (int value){
				type = value;
			}

			public void setNextStep (Fraction value){
				nextSteps = value;
			}

			public String getID(){
				return id;
			}

			public FeedbackMessage getFeedbackMessage(){
				return message;
			}
		
			public int getFeedbackType (){
				return type;
			}
		
			public Fraction getNextStep (){
				return nextSteps;
			}

		}
}

