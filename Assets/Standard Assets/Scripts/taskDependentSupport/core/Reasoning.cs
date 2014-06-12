using UnityEngine;
using System.Collections;


namespace taskDependentSupport.core
{

	public class Reasoning 
	{
	
		public void processEvent()
		{

			bool correctSolution = false;
			bool correctDenominator = false;
			for (int i = 0; i < StudentModel.getCurrentFractions().Count; i++){
				Fraction currentFraction = StudentModel.getCurrentFractions()[i];
				int nominator = currentFraction.getNominator();
				int denominator = currentFraction.getDenominator();
				int partition = currentFraction.getPartition();

				if (partition != 0){
					nominator = nominator * partition;
					denominator = denominator * partition;
				}
				if (denominator == 12) {
					correctDenominator = true;
					if (nominator == 9){
						correctSolution = true;
					}
				}


				if (correctSolution){
					FeedbackStrategyModel.setMessage(2, "high");
				}
				else {
					FeedbackStrategyModel.setMessage(0, "low");
				}

			}
		}
	}
}
