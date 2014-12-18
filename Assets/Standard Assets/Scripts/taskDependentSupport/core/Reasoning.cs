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

		public Reasoning(string taskIDvalue){
			Debug.Log ("hier in REASONING and generate new feedbackdata!!!");
			taskID = taskIDvalue;
			feedbackData = new FeedbackData (taskID);
			taskID = taskIDvalue;
		}

		public void setStudentModel(StudentModel elem){
			studentModel = elem;
			//studentModel.setFeedbackData (new FeedbackData ());
		}

	
		public void processDoneEvent(){
			if (studentModel.isTaskCompleted ()) {
				if (taskID.Equals ("EQUIValence1") || taskID.Equals("EQUIValence2")) {
					if (studentModel.getParticitionUsed()){
						Debug.Log (" currentFeedback = feedbackData.R1");
						currentFeedback = feedbackData.R1;
					}
					else {
						Debug.Log (" currentFeedback = feedbackData.R1");
						currentFeedback = feedbackData.R2;
					}
					setNewFeedback();
				}
				else if (taskID.Equals ("Comp1")){
					currentFeedback = feedbackData.CE2;
					setNewFeedback();
				}
				TDSWrapper.ArrowButtonEnable (true);
			} 
			else {
				if (studentModel.firstDoneButtonPressed()){
					currentFeedback = feedbackData.O1;
				}
				else {
					currentFeedback = feedbackData.O2;
					TDSWrapper.ArrowButtonEnable (true);
				}
				studentModel.setDoneButtonPressed ();
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
				if (socratic.Length>0) currentCounter = 1;
				else if (guidance.Length>0) currentCounter =2;
				else if (didacticConceptual.Length>0) currentCounter=3;
				else if (didacticProcedural.Length>0) currentCounter = 4;
			}
			else if (studentCounter == 1){
				if (guidance.Length>0) currentCounter =2;
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
			else if (studentCounter ==4){
				currentCounter = 1;
			}

			studentFeedbackElem.setCounter (currentCounter);
			studentModel.setPreviousFeedback (currentFeedback);
			studentModel.addFeedbackProvided (currentFeedback);
			FeedbackStrategyModel.setCurrentFeedback (currentFeedback, currentCounter);
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

		private bool currentSetIncludesFraction(int numerator, int denominator){
			for (int i = 0; i< studentModel.getCurrentFractions().Count; i++){
				Fraction current = studentModel.getCurrentFractions()[i];
				if ((current.getNumerator() == numerator) && (current.getDenominator() == denominator)){
					return true;
				}
			}
			return false;
		}

		private bool sameRepresentations(){
			if (studentModel.getCurrentFractions ().Count > 0) {
				string firstRepresentation = studentModel.getCurrentFractions()[0].getName();
				for (int i = 1; i< studentModel.getCurrentFractions().Count; i++){
					string currentRepresentation = studentModel.getCurrentFractions()[i].getName();
					if (!firstRepresentation.Equals(currentRepresentation)) return false;
				}
			}
			return true;
		}

		public void processEvent()
		{
			Debug.Log ("processEvent");

			if (taskID.Equals("Comp1")){
				checkForFeedbackFollowed();

				int firstNumerator = 1;
				int firstDenominator = 3;
				int secondNumerator = 1;
				int secondDenominator = 5;

				bool compared = false;

				Fraction currentFraction =studentModel.getCurrentFraction();

				if (currentFraction != null){
					int numerator = currentFraction.getNumerator();
					int denominator = currentFraction.getDenominator();
					int partition = currentFraction.getPartition();
					
					if (partition != 0){
						numerator = numerator * partition;
						denominator = denominator * partition;
					}

					if (!sameRepresentations()){
						currentFeedback = feedbackData.CM7;
					}

					else if (studentModel.getComparedFractions() && currentSetIncludesFraction(firstNumerator, firstDenominator)
					         && currentSetIncludesFraction(secondNumerator, secondDenominator)){
						studentModel.setTaskCompleted(true);
						currentFeedback = feedbackData.E1;
					}
					else if (!studentModel.getComparedFractions() && currentSetIncludesFraction(firstNumerator, firstDenominator)
								&& currentSetIncludesFraction(secondNumerator, secondDenominator)){
						//currentFeedback = feedbackData.M11;
						currentFeedback = feedbackData.CM8;
					}
					else if ((numerator ==0) && (denominator ==0)){
						currentFeedback = feedbackData.S3;
					}
					else if ((denominator != firstDenominator) && (denominator != secondDenominator) 
					         && (numerator == 0)){
						currentFeedback = feedbackData.M1;
					}
					else if ((denominator != firstDenominator) && (denominator != secondDenominator)
					         && (numerator != firstNumerator) && (numerator != secondNumerator)){
						currentFeedback = feedbackData.M3;
					}
					else if (numerator == firstDenominator){
						currentFeedback = feedbackData.M2;
					}
					else if (numerator == secondDenominator){
						currentFeedback = feedbackData.CM2;
					}
					else if ((denominator == firstDenominator)
					         && (studentModel.getPreviousFeedback().getID().Equals(feedbackData.M1.getID ())
					    || studentModel.getPreviousFeedback().getID().Equals(feedbackData.M2.getID ()))){
						currentFeedback = feedbackData.M4;	
					}
					else if ((denominator == secondDenominator)
					         && (studentModel.getPreviousFeedback().getID().Equals(feedbackData.M1.getID ())
					    || studentModel.getPreviousFeedback().getID().Equals(feedbackData.CM2.getID ()))){
						currentFeedback = feedbackData.M4;	
					}
					else if ((numerator == firstDenominator) && studentModel.getPreviousFeedback().getID().Equals(feedbackData.M1.getID ())){
						currentFeedback = feedbackData.M5;	
					}
					else if ((numerator == secondDenominator) && studentModel.getPreviousFeedback().getID().Equals(feedbackData.M1.getID ())){
						currentFeedback = feedbackData.CM5;	
					}
					else if ((numerator==firstNumerator) && (denominator == firstDenominator) 
					        && !currentSetIncludesFraction(secondNumerator, secondDenominator)){
						currentFeedback = feedbackData.CM6Second;	
					}
					else if ((numerator==secondNumerator) && (denominator == secondDenominator) 
					         && !currentSetIncludesFraction(firstNumerator, firstDenominator)){
						currentFeedback = feedbackData.CM6;	
					}
					else if (denominator==secondDenominator 
					         && currentSetIncludesFraction(firstNumerator, firstDenominator)){
						currentFeedback = feedbackData.CM12;	
					}
					else if (denominator==firstDenominator){
						currentFeedback = feedbackData.CM11;	
					}

					else {
						currentFeedback = new FeedbackElem();
					}
				}
				else {
					currentFeedback = new FeedbackElem();
				}
				setNewFeedback();
			}

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
			


				Fraction currentFraction =studentModel.getCurrentFraction();

				Debug.Log ("currentFraction: "+currentFraction);

				if (currentFraction != null){
						
					int numerator = currentFraction.getNumerator();
					int denominator = currentFraction.getDenominator();
					int partition = currentFraction.getPartition();


						
					if (partition != 0){
						numerator = numerator * partition;
						denominator = denominator * partition;
					}

					Debug.Log ("numerator: "+numerator+" denominator: "+denominator);

					if (!studentModel.getComparedResult() && currentSetIncludesFraction(endNumerator,endDenominator) && currentSetIncludesFraction(startNumerator,startDenominator)){
						currentFeedback = feedbackData.M11;
					}

					else if (studentModel.getComparedResult() && currentSetIncludesFraction(endNumerator,endDenominator) && currentSetIncludesFraction(startNumerator,startDenominator)){
						studentModel.setTaskCompleted(true);
						currentFeedback = feedbackData.E1;
					}

					else if ((numerator == endNumerator) && (denominator == endDenominator)) {
						Debug.Log ("solution found ");
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
				else {
					currentFeedback = new FeedbackElem();
				}

				setNewFeedback();
			
			}
		}
	}
}
