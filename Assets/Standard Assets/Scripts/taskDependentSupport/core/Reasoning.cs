using UnityEngine;
using System.Collections;


namespace taskDependentSupport.core
{

	public class Reasoning 
	{



		private string taskID="";
		private StudentModel studentModel;
		private FeedbackData feedbackData;
		private FeedbackElem currentFeedback;

		public Reasoning(){
			Debug.Log ("hier in REASONING and generate new feedbackdata!!!");
			feedbackData = new FeedbackData ();
			
		}

		public void setStudentModel(StudentModel elem){
			studentModel = elem;
			//studentModel.setFeedbackData (new FeedbackData ());
		}


		public void setTaskID(string value){
			taskID = value;
		}

		public void processDoneEvent(){
			Debug.Log ("processDoneEvent");
			if (studentModel.isTaskCompleted ()) {
				if (taskID.Equals ("EQUIValence1")) {
					if (studentModel.getParticitionUsed()){
						currentFeedback = feedbackData.R1;
					}
					else {
						currentFeedback = feedbackData.R2;
					}
					setNewFeedback();
				}
				TDSWrapper.ArrowButtonEnable (true);
				TDSWrapper.DoneButtonEnable (false);
			} 
			else {
				if (studentModel.firstDoneButtonPressed()){
					currentFeedback = feedbackData.O1;
				}
				else {
					currentFeedback = feedbackData.O2;
					TDSWrapper.ArrowButtonEnable (true);
				}
				setNewFeedback();
			}

		}

		private void setNewFeedback(){
			int currentCounter = 0;
			string currentFeedbackID = currentFeedback.getID();
			string guidance = currentFeedback.getFeedbackMessage ().getGuidance ();
			string socratic = currentFeedback.getFeedbackMessage ().getSocratic ();
			string didacticConceptual = currentFeedback.getFeedbackMessage ().getDidacticConceptual ();
			string didacticProcedural = currentFeedback.getFeedbackMessage ().getDidacticProcedural();

			Debug.Log ("setNewFeedback: "+currentFeedbackID);

			FeedbackElem studentFeedbackElem = studentModel.getFeedbackData ().getFeedbackElem (currentFeedbackID);
			Debug.Log ("studentFeedbackElem: "+studentFeedbackElem.getID());
			int studentCounter = studentFeedbackElem.getCounter();
			currentCounter = studentCounter;

			Debug.Log ("studentCounter: "+studentCounter);

			if (studentCounter == 0) {
				if (guidance.Length>0) currentCounter = 1;
				else if (socratic.Length>0) currentCounter =2;
				else if (didacticConceptual.Length>0) currentCounter=3;
				else if (didacticProcedural.Length>0) currentCounter = 4;
			}
			else if (studentCounter == 1){
				if (socratic.Length>0) currentCounter =2;
				else if (didacticConceptual.Length>0) currentCounter=3;
				else if (didacticProcedural.Length>0) currentCounter = 4;
			}
			else if (studentCounter == 2){
				if (didacticConceptual.Length>0) currentCounter=3;
				else if (didacticProcedural.Length>0) currentCounter = 4;
			}
			else if (studentCounter == 3){
				if (didacticProcedural.Length>0) currentCounter = 4;
			}
			Debug.Log ("currentCounter: "+currentCounter);
			studentFeedbackElem.setCounter (currentCounter);
			studentModel.setPreviousFeedback (currentFeedback);
			studentModel.addFeedbackProvided (currentFeedback);
			FeedbackStrategyModel.setCurrentFeedback (currentFeedback, currentCounter);
			Debug.Log (" test elem: "+studentFeedbackElem.getID ()+" "+studentFeedbackElem.getCounter ());
		}

		private void checkForFeedbackFollowed(){
			bool wasFeedbackFollowed = feedbackFollowed ();
			studentModel.addFeedbackFollowed (wasFeedbackFollowed);
		}

		private bool feedbackFollowed(){
			FeedbackElem previousFeedback = studentModel.getPreviousFeedback ();
			Fraction feedbackNextSteps = previousFeedback.getNextStep ();
			int feedbackNumerator = feedbackNextSteps.getNumerator();
			int feedbackDenominator = feedbackNextSteps.getDenominator ();;
			bool feedbackAnyValue = feedbackNextSteps.getAnyValye();
			bool feedbackSpeech = feedbackNextSteps.getSpeech();
			bool feedbackComparison = feedbackNextSteps.getComparison();

			for (int i = 0; i < studentModel.getCurrentFractions().Count; i++) {
				Fraction thisFraction = studentModel.getCurrentFractions()[i];
				int numerator = thisFraction.getNumerator();
				int denominator = thisFraction.getDenominator();
				int partition = thisFraction.getPartition();
				
				if (partition != 0){
					numerator = numerator * partition;
					denominator = denominator * partition;
					studentModel.setPartitionUsed(true);
				}

				if (feedbackAnyValue){
					if ((numerator != 0) || (denominator != 0)) return true;
					else return false;
				}

				else if ((feedbackNumerator != 0) && (feedbackDenominator !=0) && feedbackComparison){
					if ((feedbackNumerator == numerator) && (feedbackDenominator == denominator) && studentModel.getCompared()) return true;
					else return false;
				}

				else if (feedbackNumerator != 0){
					if (feedbackDenominator !=0){
						if ((feedbackNumerator == numerator) && (feedbackDenominator == denominator)) return true;
						else return false;
					}
					if (feedbackNumerator == numerator) return true;
					else return false;
				}

				else if (feedbackDenominator !=0){
					if (feedbackDenominator == denominator) return true;
					else return false;
				}

				else if (feedbackSpeech){
					//need task-independent support for this
					return true;
				}
				else if (feedbackComparison) {
					return studentModel.getCompared();
				}
			}
			return true;
		}



		public void processEvent()
		{
			Debug.Log ("processEvent");
			if (taskID.Equals("EQUIValence1") || taskID.Equals("EQUIValence2")){
				checkForFeedbackFollowed();

				Debug.Log ("EQUIValence1 or EQUIValence2");
				bool correctSolution = false;
				bool correctDenominator = false;

				int startNumerator = 0;
				int endNumerator = 0;
				int startDenominator = 0;
				int endDenominator = 0;

				if (taskID.Equals("EQUIValence1")){
					startNumerator = 3;
					endNumerator = 9;
					startDenominator = 4;
					endDenominator = 12;
				}
				else if (taskID.Equals("EQUIValence2")){
					startNumerator = 1;
					endNumerator = 2;
					startDenominator = 2;
					endDenominator = 4;
				}
			
				//check if there is already a correct solution
				Fraction inUseFraction = studentModel.getCurrentFraction();
				bool correctSolutionFound = false;
				Debug.Log ("inUseFraction: "+inUseFraction);
				Debug.Log ("getCurrentFractions: "+studentModel.getCurrentFractions());
				Debug.Log ("count: "+studentModel.getCurrentFractions().Count);
				for (int j = 0; j < studentModel.getCurrentFractions().Count; j++){

					Fraction thisFraction = studentModel.getCurrentFractions()[j];
					Debug.Log ("thisFraction: "+thisFraction);
					if ((inUseFraction == null) || !inUseFraction.getID().Equals(thisFraction.getID())){
						Debug.Log ("inUseFraction == null or inUse not current ");
						int numerator = thisFraction.getNumerator();
						int denominator = thisFraction.getDenominator();
						int partition = thisFraction.getPartition();

						Debug.Log ("numerator: "+numerator+" denominator: "+denominator+" partition: "+partition);
						
						if (partition != 0){
							Debug.Log ("partition was used ");
							numerator = numerator * partition;
							denominator = denominator * partition;
							studentModel.setPartitionUsed(true);
						}

						if ((denominator == endDenominator) && (numerator == endNumerator)){
							Debug.Log ("solution found ");
							correctSolutionFound = true;
							studentModel.setTaskCompleted(true);
							currentFeedback = feedbackData.E1;
						}
					}

				}


				if (!correctSolutionFound) {

					for (int i = 0; i < studentModel.getCurrentFractions().Count; i++){
						Fraction currentFraction = studentModel.getCurrentFractions()[i];
						
						int numerator = currentFraction.getNumerator();
						int denominator = currentFraction.getDenominator();
						int partition = currentFraction.getPartition();
						
						if (partition != 0){
							numerator = numerator * partition;
							denominator = denominator * partition;
						}

						if ((numerator == endNumerator) && (denominator == endDenominator)) {
							Debug.Log ("solution found ");
							correctSolutionFound = true;
							studentModel.setTaskCompleted(true);
							currentFeedback = feedbackData.E1;
						}
						else if ((denominator == 0) && (numerator == 0)){
							currentFeedback = feedbackData.S3;
						}

						else if ((numerator != endNumerator) && (denominator == endDenominator) && 
						         (studentModel.getPreviousFeedback().getID().Equals(feedbackData.M1.getID()) || 
						 studentModel.getPreviousFeedback().getID().Equals(feedbackData.M2.getID ()))){
							currentFeedback = feedbackData.M4;
						}
						
						else if ((numerator != endNumerator) && (numerator == endDenominator) && studentModel.getPreviousFeedback().getID().Equals(feedbackData.M1.getID())){
							currentFeedback = feedbackData.M5;
						}

						else if ((numerator == startNumerator) && (denominator == startDenominator)){
							currentFeedback = feedbackData.M7;
						}

						else if ((numerator != endNumerator) && (denominator == endDenominator) && 
						         studentModel.getPreviousFeedback().getID().Equals(feedbackData.M8.getID ())){
							currentFeedback = feedbackData.M9;
						}

						else if ((numerator != endNumerator) && (denominator == endDenominator) && 
						         (!studentModel.getReflectionForDenominatorShown())){
							currentFeedback = feedbackData.M10;
							studentModel.setReflectionForDenominatorShown(true);
						}

						else if ((numerator != endNumerator) && (denominator == endDenominator) && studentModel.getReflectionForDenominatorShown()){
							currentFeedback = feedbackData.M8;
						}

						else if ((numerator != endNumerator) && 
						         ((denominator == endDenominator) || (denominator == startDenominator))){
							currentFeedback = feedbackData.M6;
						}

						else if ((denominator != endDenominator) && (denominator != startDenominator)){
							currentFeedback = feedbackData.M1;
						}

						else if ((numerator == endDenominator) || (numerator == startDenominator)){
							currentFeedback = feedbackData.M2;
						}

						else if (((numerator != startNumerator) || (denominator != startDenominator)) && ((numerator != endNumerator) || (denominator != endDenominator))){
							currentFeedback = feedbackData.M3;
						}
					
						else {
							currentFeedback = new FeedbackElem();
						}


					}
				}
				setNewFeedback();
			}
		}
	}
}
