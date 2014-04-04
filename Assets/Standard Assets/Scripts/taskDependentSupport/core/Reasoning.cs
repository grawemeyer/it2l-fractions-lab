using UnityEngine;
using System.Collections;

namespace taskDependentSupport.core
{

	public class Reasoning 
	{
	
		public void processEvent()
		{
			if ((StudentModel.getCurrentFractions() == 2) && (! StudentModel.getCompared())){
				FeedbackStrategyModel.setMessage(1, "low");
			}
			
			else if (StudentModel.getComparedResult()){
				FeedbackStrategyModel.setMessage(2, "high");
			}
			
			else if (StudentModel.getEquivalenceOpen() == 1){
				FeedbackStrategyModel.setMessage(3, "low");
			}
			
			else {
				FeedbackStrategyModel.setMessage(0, "low");
			}
		}
	}
}
