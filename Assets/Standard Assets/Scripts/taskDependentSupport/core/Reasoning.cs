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
					if (StudentModel.getParticitionUsed()){
						FeedbackStrategyModel.setMessage (23, "high");
					}
					else {
						FeedbackStrategyModel.setMessage (24, "high");
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

		private bool problem1 = false;
		private bool problem2 = false;
		private bool problem3 = false;
		private bool problem4 = false;
		private bool problem5 = false;
		private bool problem6 = false;
		private bool problem7 = false;
		private bool problem8 = false;
		private bool reflect1 = false;
		private bool reflect2 = false;

		private void reset(){
			problem1 = false;
			problem2 = false;
			problem3 = false;
			problem4 = false;
			problem5 = false;
			problem6 = false;
			problem7 = false;
			problem8 = false;
		}

		public void processEvent()
		{
			Debug.Log ("processEvent");
			if (taskID.Equals("EQUIValence1")){
				Debug.Log ("EQUIValence1");
				bool correctSolution = false;
				bool correctDenominator = false;

			
				//check if there is already a correct solution
				Fraction inUseFraction = StudentModel.getCurrentFraction();
				bool correctSolutionFound = false;
				Debug.Log ("inUseFraction: "+inUseFraction);
				Debug.Log ("getCurrentFractions: "+StudentModel.getCurrentFractions());
				Debug.Log ("count: "+StudentModel.getCurrentFractions().Count);
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
							StudentModel.setPartitionUsed(true);
						}

						if ((denominator == 12) && (nominator == 9)){
							Debug.Log ("solution found ");
							correctSolutionFound = true;
							//FeedbackStrategyModel.setMessage(0, "low");
							StudentModel.setTaskCompleted(true);
						}
					}

				}

				if (!correctSolutionFound) {
					Debug.Log ("NOT correctSolutionFound: ");
					bool denominatorOf12 = false;
					bool threeQuaters = false;
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
							problem1 = true;
						}
						else if ((denominator == 4) && (nominator == 3)){
							problem8=true;
							threeQuaters = true;
							if (denominatorOf12) problem6=true;
						}

						else if (denominator == 12) {
							correctDenominator = true;
							denominatorOf12 = true;

							if (StudentModel.getNominatorDenominatorMisconception()){
								reflect1=true;
								StudentModel.setNominatorDenominatorMisconception(false);
							}

							if (nominator == 9){
								correctSolution = true;
								StudentModel.setTaskCompleted(true);
							}
							if (threeQuaters)problem6=true;
							else if ((nominator == 3) || (nominator == 0)) problem7=true;
							else{
								problem5 = true;
								StudentModel.setAskForComparison(true);
							}

						}
						else if (nominator == 12){
							problem4 = true;
							StudentModel.setNominatorDenominatorMisconception(true);
						}
						else {
							//denominator not 12
							if (denominator ==0) problem2 = true;
							else problem3=true;
						}

						if (denominatorOf12 && threeQuaters){
							reset();
							problem6=true;
							StudentModel.setAskForComparison(true);
						}

						if (StudentModel.getAskForComparison() && StudentModel.getCompared()){
							reflect1=false;
							reflect2=true;
							StudentModel.setAskForComparison(false);
						}

						if (correctSolution){
							FeedbackStrategyModel.setMessage(2, "high");
						}
						else if (reflect1){
							FeedbackStrategyModel.setMessage(16, "high");
						}
						else if (reflect2){
							FeedbackStrategyModel.setMessage(19, "high");
						}
						else if (problem1){
							FeedbackStrategyModel.setMessage(12, "high");
						}
						else if (problem2){
							FeedbackStrategyModel.setMessage(13, "high");
						}
						else if (problem3){
							FeedbackStrategyModel.setMessage(14, "high");
						}
						else if (problem4){
							FeedbackStrategyModel.setMessage(15, "high");
						}
						else if (problem5){
							FeedbackStrategyModel.setMessage(17, "high");
						}
						else if (problem6){
							FeedbackStrategyModel.setMessage(18, "high");
						}
						else if (problem7){
							FeedbackStrategyModel.setMessage(20, "high");
						}
						else if (problem8){
							FeedbackStrategyModel.setMessage(21, "high");
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
