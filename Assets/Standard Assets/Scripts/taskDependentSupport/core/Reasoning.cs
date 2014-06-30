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

		public void processDoneEvent(){
			Debug.Log ("processDoneEvent");
			if (StudentModel.isTaskCompleted ()) {
				if (taskID.Equals ("EQUIValence1")) {
					if (StudentModel.isMisconceptionNominatorForgotten ()) {
						FeedbackStrategyModel.setMessage (10, "high");
					} else {
						FeedbackStrategyModel.setMessage (11, "high");
					}
				}
				TDSWrapper.ArrowButtonEnable (true);
				TDSWrapper.DoneButtonEnable (false);
			} 
			else {
				if (StudentModel.firstDoneButtonPressed()){
					FeedbackStrategyModel.setMessage (8, "high");
				}
				else {
					FeedbackStrategyModel.setMessage (9, "high");
					TDSWrapper.ArrowButtonEnable (true);
				}
			}
		}

		public void processEvent()
		{
			Debug.Log ("processEvent");
			if (taskID.Equals("EQUIValence1")){
				Debug.Log ("EQUIValence1");
				bool correctSolution = false;
				bool correctDenominator = false;
				bool misconception1 = false;
				bool misconception2 = false;
				bool misconception3 = false;
				bool misconception4 = false;
			
				//check if there is already a correct solution
				Fraction inUseFraction = StudentModel.getCurrentFraction();
				bool correctSolutionFound = false;
				Debug.Log ("inUseFraction: "+inUseFraction);
				Debug.Log ("getCurrentFractions: "+StudentModel.getCurrentFractions());
				Debug.Log ("count: "+StudentModel.getCurrentFractions().Count;);
				for (int j = 0; j < StudentModel.getCurrentFractions().Count; j++){

					Fraction thisFraction = StudentModel.getCurrentFractions()[j];
					Debug.Log ("thisFraction: "+thisFraction);
					if ((inUseFraction == null) || !inUseFraction.getID().Equals(thisFraction.getID())){
						Debug.Log ("inUseFraction == null or inUse not current ");
						int nominator = thisFraction.getNominator();
						int denominator = thisFraction.getDenominator();
						int partition = thisFraction.getPartition();
						
						if (partition != 0){
							Debug.Log ("partition was used ");
							nominator = nominator * partition;
							denominator = denominator * partition;
						}

						if ((denominator == 12) && (nominator == 9)){
							Debug.Log ("solution found ");
							correctSolutionFound = true;
							FeedbackStrategyModel.setMessage(0, "low");
						}
					}

				}

				if (!correctSolutionFound) {
					Debug.Log ("NOT correctSolutionFound: ");
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
								StudentModel.setTaskCompleted(true);
							}
							else if (nominator == 3){
								misconception1 = true;
								StudentModel.setMisconceptionNominatorForgotten();
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
}
