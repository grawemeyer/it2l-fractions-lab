using UnityEngine;
using System.Collections;


namespace taskDependentSupport.core
{

	public class Reasoning 
	{

		private string taskID="";

		public void setTaskID(string value){
			taskID = value;
		}

		public void processEvent()
		{

			if (taskID.Equals("EQUIValence1")){

				bool correctSolution = false;
				bool correctDenominator = false;
				bool misconception1 = false;
				bool misconception2 = false;
				bool misconception3 = false;
				bool misconception4 = false;
				for (int i = 0; i < StudentModel.getCurrentFractions().Count; i++){
					Fraction currentFraction = StudentModel.getCurrentFractions()[i];
					int nominator = currentFraction.getNominator();
					int denominator = currentFraction.getDenominator();
					int partition = currentFraction.getPartition();

					if (partition != 0){
						nominator = nominator * partition;
						denominator = denominator * partition;
					}

					if((denominator == 0) && (nominator == 0)){
						misconception3 = true;
					}

					else if (denominator == 12) {
						correctDenominator = true;
						if (nominator == 9){
							correctSolution = true;
						}
						else if (nominator == 3){
							misconception1 = true;
						}
					}
					else if (nominator == 12){
						misconception4 = true;
					}
					else {
						misconception2 = true;
					}


					if (correctSolution){
						FeedbackStrategyModel.setMessage(2, "high");
					}
					else if (misconception1){
						FeedbackStrategyModel.setMessage(4, "high");
					}
					else if (misconception2){
						FeedbackStrategyModel.setMessage(5, "high");
					}
					else if (misconception3){
						FeedbackStrategyModel.setMessage(6, "high");
					}
					else if (misconception4){
						FeedbackStrategyModel.setMessage(7, "high");
					}
					else {
						FeedbackStrategyModel.setMessage(0, "low");
					}
				}
			}
		}
	}
}
