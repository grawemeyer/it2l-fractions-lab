using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;


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
			Debug.Log ("::: processDoneEvent ::: ");
			processEvent ();
			Debug.Log ("::: isTaskCompleted ::: "+studentModel.isTaskCompleted());
			if (studentModel.isTaskCompleted ()) {
				if (taskID.Equals ("task1.1setA")){
					currentFeedback = feedbackData.CE2;
					setNewFeedback();
				}
				else if (taskID.Equals("task2.1")){
					Debug.Log (":: task 2.1 :: feebdack FE2");
					currentFeedback = feedbackData.FE2;
					setNewFeedback();
				}
				else if (taskID.Equals ("task2.2")) {
					currentFeedback = feedbackData.F2E2;
					setNewFeedback();
				}
				else if (taskID.Equals("task2.4.setA.area") || taskID.Equals("task2.4.setB.area") || taskID.Equals("task2.4.setC.area") ||
				         taskID.Equals("task2.4.setA.numb") || taskID.Equals("task2.4.setB.numb") || taskID.Equals("task2.4.setC.numb") ||
				         taskID.Equals("task2.4.setA.sets") || taskID.Equals("task2.4.setB.sets") || taskID.Equals("task2.4.setC.sets") ||
				         taskID.Equals("task2.4.setA.liqu") || taskID.Equals("task2.4.setB.liqu") || taskID.Equals("task2.4.setC.liqu")){
					currentFeedback = feedbackData.T24E2;
					setNewFeedback();
				}
				else if (taskID.Equals("task2.6.setA") || taskID.Equals("task2.6.setB") || taskID.Equals("task2.6.setC")){
					currentFeedback = feedbackData.T26E2;
					setNewFeedback();
				}
				else if (taskID.Equals("task2.7.setA") ||taskID.Equals("task2.7.setB") || taskID.Equals("task2.7.setC")) {
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
				else if (taskID.Equals("task3aPlus.1.setA.area") || taskID.Equals("task3aPlus.1.setB.area") || taskID.Equals("task3aPlus.1.setC.area") ||
				         taskID.Equals("task3aPlus.1.setA.numb") || taskID.Equals("task3aPlus.1.setB.numb") || taskID.Equals("task3aPlus.1.setC.numb") ||
				         taskID.Equals("task3aPlus.1.setA.sets") || taskID.Equals("task3aPlus.1.setB.sets") || taskID.Equals("task3aPlus.1.setC.sets") ||
				         taskID.Equals("task3aPlus.1.setA.liqu") || taskID.Equals("task3aPlus.1.setB.liqu") || taskID.Equals("task3aPlus.1.setC.liqu") ){
					currentFeedback = feedbackData.T3aP1E2;
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
			studentModel.setCurrentFeedback (currentFeedback);
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

			studentModel.setCurrentFeedbackLevel (currentCounter);
			studentFeedbackElem.setCounter (currentCounter);
			studentModel.setPreviousFeedback (currentFeedback);
			studentModel.addFeedbackProvided (currentFeedback);
			studentModel.setDisplayedMessageType (currentFeedback.getFeedbackType());
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
			int feedbackDenominator = feedbackNextSteps.getDenominator ();
			int feedbackPartitionValue = feedbackNextSteps.getPartition();
			bool feedbackAnyValue = feedbackNextSteps.getAnyValye();
			bool feedbackSpeech = feedbackNextSteps.getSpeech();
			bool feedbackComparison = feedbackNextSteps.getComparison();
			bool differntRepresentation = feedbackNextSteps.getDifferentRepresentation ();
			bool allSameValue = feedbackNextSteps.getAllSameValue ();
			bool numeratorAnyValue = feedbackNextSteps.getNumeratorAnyValue ();
			bool equivalentFraction = feedbackNextSteps.getEquivalentFraction ();
			bool partitionBool = feedbackNextSteps.getPartitionBool ();
			int[] denominators = feedbackNextSteps.getDenominators ();
			int[] numerators = feedbackNextSteps.getNumerators ();
			int numeratorAdd = feedbackNextSteps.getNumeratorForAdditionTask ();
			int denominatorAdd = feedbackNextSteps.getDenominatorForAdditionTask ();
			int numeratorAddEnd = feedbackNextSteps.getNumeratorForAdditionTaskEnd ();
			int denominatorAddEnd = feedbackNextSteps.getDenominatorForAdditionTaskEnd ();
			bool additionBox = feedbackNextSteps.getAdditionBox ();

			bool result = false;

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
					if ((numerator != 0) || (denominator != 0)) result =true;
				}

				else if (numeratorAnyValue){
					if (numerator != 0) result = true;
				}
				else if (feedbackPartitionValue != 0){
					if (feedbackPartitionValue == partition) result=true;
				}
				else if (partitionBool){
					if (partition != 0) result= true;
				}

				else if (numeratorAdd != 0){
					if ((denominator == denominatorAdd) && (numerator < numeratorAdd)) result = true;
				}

				else if ((feedbackNumerator != 0) && (feedbackDenominator !=0) && feedbackComparison){
					if ((feedbackNumerator == numerator) && (feedbackDenominator == denominator) && studentModel.getCompared()) result= true;
				}

				else if (equivalentFraction){
					result= equivalent(numerator, denominator, feedbackNumerator, feedbackDenominator);
				}

				else if (feedbackNumerator != 0){
					if (feedbackDenominator !=0){
						if ((feedbackNumerator == numerator) && (feedbackDenominator == denominator)) result= true;
					}
					if (feedbackNumerator == numerator) result= true;
				}

				else if (feedbackDenominator !=0){
					if (feedbackDenominator == denominator) result= true;
				}

				else if (feedbackSpeech){
					//need task-independent support for this
					return true;
				}
				else if (feedbackComparison) {
					result= studentModel.getCompared();
				}

				else if ((numerators != null) && (denominators != null)){
					for (int j = 0; j < denominators.Length; j++){
						int valueNum = numerators[j];
						int valueDen = denominators[j];
						if ((numerator == valueNum) && (denominator == valueDen)) result = true;
					}
				}

				else if (denominators != null){
					for (int j = 0; j < denominators.Length; j++){
						int value = denominators[j];
						if (denominator == value) result = true;
					}
				}
			}

			if (additionBox) {
				result = studentModel.getAdditionBox();
			}

			if (numeratorAddEnd != 0) {
				result = checkForAddedFraction(numeratorAddEnd, denominatorAddEnd);
			}

			if (differntRepresentation) {
				result = !sameRepresentations();
			}
			if (allSameValue){
				result = sameValues();
			}
			return result;
		}

		private List<int> getNumeratorsWithCorrectDenominator(int finalNumerator, int finalDenominator){
			List<int> result = new List<int>();

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

				if ((denominator == finalDenominator) && (numerator != finalNumerator)){
					result.Add(numerator);
				}
			}
			return result;
		}


		private bool checkForAdditionResult(int numerator, List<int> numeratorsWithCorrectDenominator, int finalNumerator){
			for (int i = 0; i< numeratorsWithCorrectDenominator.Count; i++){
				int currentNumerator = numeratorsWithCorrectDenominator[i];
				if ((numerator+currentNumerator) == finalNumerator){
					return true;
				}
			}
			return false;
		}

		private bool correctNumeratorsAsAdditionAnswers(List<int> numeratorsWithCorrectDenominator, int finalNumerator){
			if (numeratorsWithCorrectDenominator.Count == 1)return false;
			int firstNumerator = numeratorsWithCorrectDenominator [0];
			numeratorsWithCorrectDenominator.RemoveAt(0);
			if (checkForAdditionResult (firstNumerator, numeratorsWithCorrectDenominator, finalNumerator)) {
				return true;
			} 
			else {
				correctNumeratorsAsAdditionAnswers(numeratorsWithCorrectDenominator, finalNumerator);
			}
			return false;
		}

		private bool checkForAddedFraction(int finalNumerator, int finalDenominator){
			int addedNumerators = 0;
			List<int> numeratorsWithCorrectDenominator = getNumeratorsWithCorrectDenominator (finalNumerator, finalDenominator);
			int countNumerators = numeratorsWithCorrectDenominator.Count;

			if (countNumerators < 2) {
				return false;
			}
			else if (correctNumeratorsAsAdditionAnswers (numeratorsWithCorrectDenominator, finalNumerator)){
				return true;
			}
			else {
				return false;
			}
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

		private bool sameValues(){
			if (studentModel.getCurrentFractions ().Count > 0) {
				Fraction firstFraction = studentModel.getCurrentFractions()[0];
				int firstNumerator = firstFraction.getNumerator();
				int firstDenominator = firstFraction.getDenominator();
				int firstPartition = firstFraction.getPartition();
				
				if (firstPartition != 0){
					firstNumerator = firstNumerator * firstPartition;
					firstDenominator = firstDenominator * firstPartition;
				}
				for (int i = 1; i< studentModel.getCurrentFractions().Count; i++){
					Fraction currentfraction = studentModel.getCurrentFractions()[i];
					int numerator = currentfraction.getNumerator();
					int denominator = currentfraction.getDenominator();
					int partition = currentfraction.getPartition();
					
					if (partition != 0){
						numerator = numerator * partition;
						denominator = denominator * partition;
					}
					if ((firstNumerator != numerator) || (firstDenominator != denominator)) return false;
				}
			}
			return true;
		}

		private List<Fraction> getFractionsWithSameValues(int checkNumerator, int checkDenominator){
			List<Fraction> result = new List<Fraction>();
			for (int i = 0; i< studentModel.getCurrentFractions().Count; i++) {
				Fraction currentfraction = studentModel.getCurrentFractions()[i];
				int numerator = currentfraction.getNumerator();
				int denominator = currentfraction.getDenominator();
				int partition = currentfraction.getPartition();
				
				if (partition != 0){
					numerator = numerator * partition;
					denominator = denominator * partition;
				}

				if ((checkNumerator == numerator) && (checkDenominator == denominator)){
					result.Add(currentfraction);
				}
			}
			return result;
		}

		private List<Fraction> deleteFractionsWithSameValues(int checkNumerator, int checkDenominator){
			List<Fraction> result = new List<Fraction>();
			for (int i = 0; i< studentModel.getCurrentFractions().Count; i++) {
				Fraction currentfraction = studentModel.getCurrentFractions()[i];
				int numerator = currentfraction.getNumerator();
				int denominator = currentfraction.getDenominator();
				int partition = currentfraction.getPartition();
				
				if (partition != 0){
					numerator = numerator * partition;
					denominator = denominator * partition;
				}
				
				if ((checkNumerator != numerator) || (checkDenominator != denominator)){
					result.Add(currentfraction);
				}
			}
			return result;
		}

		private List<Fraction> getFractionsWithDiffRep(String name, List<Fraction> sameValueFractions){
			List<Fraction> result  = new List<Fraction>();
			for (int i = 0; i< sameValueFractions.Count; i++) {
				String currentName = sameValueFractions[i].getName ();
				if (!currentName.Equals(name)){
					result.Add (sameValueFractions[i]);
				}
			}
			return result;
		}

		private bool fourWithDifferentRep(List<Fraction> sameValueFractions){
			string currentRepresentation = sameValueFractions[0].getName();
			List<Fraction> diffRep = getFractionsWithDiffRep(currentRepresentation, sameValueFractions);
			diffRep.Add (sameValueFractions[0]);

			if (diffRep.Count >= 4) {
				return true;
			}
			return false;
		}

		private bool fourWithDiffRepAndSameValues(){
			int amountOfReps = createdReps ();
			if (amountOfReps >= 4) {
				//get all with the same values
				List<Fraction> currentFractions = studentModel.getCurrentFractions();
				while (currentFractions.Count > 0){
					Fraction firstFraction = currentFractions [0];
					int firstNumerator = firstFraction.getNumerator();
					int firstDenominator = firstFraction.getDenominator();
					int firstPartition = firstFraction.getPartition();
					
					if (firstPartition != 0){
						firstNumerator = firstNumerator * firstPartition;
						firstDenominator = firstDenominator * firstPartition;
					}

					List<Fraction> sameValues = getFractionsWithSameValues(firstNumerator, firstDenominator);
					currentFractions = deleteFractionsWithSameValues(firstNumerator, firstDenominator);

					if (sameValues.Count >= 4){
						if (fourWithDifferentRep(sameValues)){
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool onlyOneFraction(){
			if (studentModel.getCurrentFractions ().Count == 1) {
				return true;
			}
			return false;
		}

		private int createdReps(){
			return studentModel.getCurrentFractions ().Count;
		}

		private bool multiple(int value, int multipleOf){
			double doubleValue = System.Convert.ToDouble(value);
			double doubleMultipleOf = System.Convert.ToDouble(multipleOf);
			double result = doubleValue / doubleMultipleOf;

			string stringResult = result.ToString ();
			string[] splitResult = stringResult.Split ('.');

			if (stringResult.IndexOf (".") > 0) {
				return false;
			}
			return true;
		}

		private bool equivalent(int numerator, int denominator, int multipleNumerator, int multipleDenominator){

			if ((numerator != multipleNumerator) && (denominator != multipleDenominator)) {
				double mainNumerator = System.Convert.ToDouble (multipleNumerator);
				double mainDenominator = System.Convert.ToDouble (multipleDenominator);
				double mainResult = (mainNumerator / mainDenominator);
			
				double thisNumerator = System.Convert.ToDouble (numerator);
				double thisDenominator = System.Convert.ToDouble (denominator);
				double thisResult = (thisNumerator / thisDenominator);

				if (mainResult == thisResult) return true;
			}
			return false;
		}

		private bool currentSetContainsFraction(int checkNumerator, int checkDenominator){
			if (studentModel.getCurrentFractions ().Count > 0) {
				for (int i = 0; i< studentModel.getCurrentFractions().Count; i++){
					Fraction currentfraction = studentModel.getCurrentFractions()[i];
					int numerator = currentfraction.getNumerator();
					int denominator = currentfraction.getDenominator();
					int partition = currentfraction.getPartition();
					
					if (partition != 0){
						numerator = numerator * partition;
						denominator = denominator * partition;
					}
					if ((numerator == checkNumerator) && (denominator == checkDenominator)){
						return true;
					}
				}
			}
			return false;
		}

		private bool currentSetContainsEqivalentFraction(int checkNumerator, int checkDenominator){
			if (studentModel.getCurrentFractions ().Count > 0) {
				for (int i = 0; i< studentModel.getCurrentFractions().Count; i++){
				Fraction multiplCurrentfraction = studentModel.getCurrentFractions()[i];
					int multipleNumerator = multiplCurrentfraction.getNumerator();
					int multipleDenominator = multiplCurrentfraction.getDenominator();
					int multiplePartition = multiplCurrentfraction.getPartition();
				
					if (multiplePartition != 0){
						multipleNumerator = multipleNumerator * multiplePartition;
						multipleDenominator = multipleDenominator * multiplePartition;
					}
		
					if (equivalent(multipleNumerator, multipleDenominator, checkNumerator, checkDenominator)){
						return true;
					}
				}
			}
			return false;
		}

		private bool checkCurrentFractionsForEquivalence(int equivalentNumerator, int equivalentDenominator){
			if (studentModel.getCurrentFractions ().Count > 0) {
				bool containsMainFraction = false;
				bool containsMultipleFraction = false;

				for (int i = 0; i< studentModel.getCurrentFractions().Count; i++){
					Fraction currentfraction = studentModel.getCurrentFractions()[i];
					int numerator = currentfraction.getNumerator();
					int denominator = currentfraction.getDenominator();
					int partition = currentfraction.getPartition();
					
					if (partition != 0){
						numerator = numerator * partition;
						denominator = denominator * partition;
					}
					if ((numerator == equivalentNumerator) && (denominator == equivalentDenominator)){
						containsMainFraction = true;
						i = studentModel.getCurrentFractions().Count;
					}
				}
				for (int j = 0; j< studentModel.getCurrentFractions().Count; j++){
					Fraction multiplCurrentfraction = studentModel.getCurrentFractions()[j];
					int multipleNumerator = multiplCurrentfraction.getNumerator();
					int multipleDenominator = multiplCurrentfraction.getDenominator();
					int multiplePartition = multiplCurrentfraction.getPartition();
					
					if (multiplePartition != 0){
						multipleNumerator = multipleNumerator * multiplePartition;
						multipleDenominator = multipleDenominator * multiplePartition;
					}

					if (equivalent(multipleNumerator, multipleDenominator, equivalentNumerator, equivalentDenominator)){
						containsMultipleFraction = true;
						j = studentModel.getCurrentFractions().Count;
					}
				}

				if (containsMainFraction && containsMultipleFraction){
					return true;
				}
			}
			return false;
		}

		private bool onlyOneFractionCreatedForAddition(int finalNumerator, int finalDenominator){
			if (studentModel.getCurrentFractions ().Count > 1) {
				return false;
			}
			else {
				Fraction currentfraction = studentModel.getCurrentFractions ()[0];
				int numerator = currentfraction.getNumerator();
				int denominator = currentfraction.getDenominator();
				int partition = currentfraction.getPartition();
				
				if (partition != 0){
					numerator = numerator * partition;
					denominator = denominator * partition;
				}

				if ((numerator < finalNumerator) && (denominator == finalDenominator)) return true;
			}
			return false;
		}


		public void processEvent()
		{
			Debug.Log ("processEvent");
			Debug.Log ("taskID: "+taskID);
			Fraction testCurrentFraction = studentModel.getCurrentFraction ();
			Debug.Log ("testCurrentFraction: "+testCurrentFraction);

			if (taskID.Equals("task3aPlus.1.setA.area") || taskID.Equals("task3aPlus.1.setB.area") || taskID.Equals("task3aPlus.1.setC.area") ||
			    taskID.Equals("task3aPlus.1.setA.numb") || taskID.Equals("task3aPlus.1.setB.numb") || taskID.Equals("task3aPlus.1.setC.numb") ||
			    taskID.Equals("task3aPlus.1.setA.sets") || taskID.Equals("task3aPlus.1.setB.sets") || taskID.Equals("task3aPlus.1.setC.sets") ||
			    taskID.Equals("task3aPlus.1.setA.liqu") || taskID.Equals("task3aPlus.1.setB.liqu") || taskID.Equals("task3aPlus.1.setC.liqu") ){

				checkForFeedbackFollowed ();

				int startNumerator = 0;
				int startDenominator = 0;
				string representation = "area";

				if (taskID.Equals("task3aPlus.1.setA.area")){
					startNumerator = 3;
					startDenominator = 5;
					representation = "area";
				}
				else if (taskID.Equals("task3aPlus.1.setB.area")){
					startNumerator = 4;
					startDenominator = 7;
					representation = "area";
				}
				else if (taskID.Equals("task3aPlus.1.setC.area")){
					startNumerator = 12;
					startDenominator = 9;
					representation = "number line";
				}
				else if (taskID.Equals("task3aPlus.1.setA.numb")){
					startNumerator = 3;
					startDenominator = 5;
					representation = "number line";
				}
				else if (taskID.Equals("task3aPlus.1.setB.numb")){
					startNumerator = 4;
					startDenominator = 7;
					representation = "number line";
				}
				else if (taskID.Equals("task3aPlus.1.setC.numb")){
					startNumerator = 12;
					startDenominator = 9;
					representation = "number line";
				}
				else if (taskID.Equals("task3aPlus.1.setA.sets")){
					startNumerator = 3;
					startDenominator = 5;
					representation = "sets";
				}
				else if (taskID.Equals("task3aPlus.1.setB.sets")){
					startNumerator = 4;
					startDenominator = 7;
					representation = "sets";
				}
				else if (taskID.Equals("task3aPlus.1.setC.sets")){
					startNumerator = 12;
					startDenominator = 9;
					representation = "sets";
				}
				else if (taskID.Equals("task3aPlus.1.setA.liqu")){
					startNumerator = 3;
					startDenominator = 5;
					representation = "liquid measures";
				}
				else if (taskID.Equals("task3aPlus.1.setB.liqu")){
					startNumerator = 4;
					startDenominator = 7;
					representation = "liquid measures";
				}
				else if (taskID.Equals("task3aPlus.1.setC.liqu")){
					startNumerator = 12;
					startDenominator = 9;
					representation = "liquid measures";
				}

				Fraction currentFraction = studentModel.getCurrentFraction ();
				
				if (currentFraction != null) {
					int numerator = currentFraction.getNumerator ();
					int denominator = currentFraction.getDenominator ();
					int partition = currentFraction.getPartition ();


					if (partition != 0) {
						numerator = numerator * partition;
						denominator = denominator * partition;
					}

					if ((numerator == 0) && (denominator == 0)) {
						currentFeedback = feedbackData.S3;
					}

					//needs to be removed when new rules that include a third fraction of the solution fraction has been added
					else if (checkForAddedFraction(startNumerator, startDenominator)){
						studentModel.setTaskCompleted(true);
						currentFeedback = feedbackData.T3aP1E1;
					}

					else if (checkForAddedFraction(startNumerator, startDenominator) && studentModel.getAdditionBox()){
						studentModel.setTaskCompleted(true);
						currentFeedback = feedbackData.T3aP1E1;
					}
					else if (checkForAddedFraction(startNumerator, startDenominator) && (!studentModel.getAdditionBox())){
						currentFeedback = feedbackData.T3aP1M11;
					}

					else if ((numerator == startDenominator) &&
					         studentModel.getPreviousFeedback().getID().Equals(feedbackData.T3aP1M1.getID ())){
						currentFeedback = feedbackData.T3aP1M5;
					}

					else if ((denominator == startDenominator) &&
					         (studentModel.getPreviousFeedback().getID().Equals(feedbackData.T3aP1M1.getID ()) ||
					 studentModel.getPreviousFeedback().getID().Equals(feedbackData.T3aP1M2.getID ()))){
						currentFeedback = feedbackData.T3aP1M4;
					}

					else if ((numerator == 0) && (denominator == startDenominator)){
						currentFeedback = feedbackData.T3aP1M6;
					}

					else if (onlyOneFractionCreatedForAddition(startNumerator, startDenominator)){
						currentFeedback = feedbackData.T3aP1M7;
					}

					//(numerator >= startNumerato) needs to get changed (>) for new rules that include a third fraction of the solution fraction
					else if ((denominator == startDenominator) && (numerator >= startNumerator)){
						currentFeedback = feedbackData.T3aP1M8;
					}
					//(numerator >= startNumerato) needs to get changed (>) for new rules that include a third fraction of the solution fraction
					else if ((denominator != startDenominator) && (numerator >= startNumerator)){
						currentFeedback = feedbackData.T3aP1M3;
					}

					else if (numerator == startDenominator){
						currentFeedback = feedbackData.T3aP1M2;
					}

					else if (denominator != startDenominator){
						currentFeedback = feedbackData.T3aP1M1;
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

			else if (taskID.Equals("task2.6.setA") || taskID.Equals("task2.6.setB") || taskID.Equals("task2.6.setC")){

				checkForFeedbackFollowed ();

				int startNumerator = 0;
				int startDenominator = 0;
				int endNumerator = 0;
				int endDenominator = 0;

				if (taskID.Equals("task2.6.setA")){
					startNumerator = 3;
					startDenominator = 4;
					endNumerator = 1;
					endDenominator = 12;
				}
				else if (taskID.Equals("task2.6.setB")){
					startNumerator = 2;
					startDenominator = 5;
					endNumerator = 1;
					endDenominator = 10;
				}
				else if (taskID.Equals("task2.6.setC")){
					startNumerator = 7;
					startDenominator = 3;
					endNumerator = 1;
					endDenominator = 21;
				}

				Fraction currentFraction = studentModel.getCurrentFraction ();
				
				if (currentFraction != null) {
					int numerator = currentFraction.getNumerator ();
					int denominator = currentFraction.getDenominator ();
					int partition = currentFraction.getPartition ();
					
					if (partition != 0) {
						numerator = numerator * partition;
						denominator = denominator * partition;
					}

					if (sameRepresentations() == false){
						currentFeedback = feedbackData.T26M8;
					}

					else if ((numerator == 0) && (denominator == 0)) {
						currentFeedback = feedbackData.S3;
					}
					else if (currentSetIncludesFraction(startNumerator, startDenominator) && 
					         currentSetIncludesFraction(endNumerator, endDenominator) && studentModel.getComparedFractions()){
						studentModel.setTaskCompleted(true);
						currentFeedback = feedbackData.T26E1;
					}
					else if (currentSetIncludesFraction(startNumerator, startDenominator) && 
					         currentSetIncludesFraction(endNumerator, endDenominator) && (studentModel.getComparedFractions() == false)){
						currentFeedback = feedbackData.T26M11;
					}
					else if (currentSetIncludesFraction(startNumerator, startDenominator) || 
					         currentSetIncludesFraction(endNumerator, endDenominator)){
						currentFeedback = feedbackData.T26M7;
					}

					else if (((denominator == startDenominator) || (denominator == endDenominator)) && 
					         (studentModel.getPreviousFeedback().getID().Equals(feedbackData.T26M1.getID ()) ||
					         studentModel.getPreviousFeedback().getID().Equals(feedbackData.T26M1.getID ()))){
						currentFeedback = feedbackData.T26M4;
					}
					else if (((numerator == startDenominator) || (numerator == endDenominator)) &&
					         studentModel.getPreviousFeedback().getID().Equals(feedbackData.T26M1.getID ())){
						currentFeedback = feedbackData.T26M5;
					}
					else if ((denominator == startDenominator) || (denominator == endDenominator)){
						currentFeedback = feedbackData.T26M6;
					}

					else if ((numerator == 0) && ((denominator != startDenominator) || (denominator != endDenominator))){
						currentFeedback = feedbackData.T26M1;
					}
					else if ((numerator == startDenominator) || (numerator == endDenominator)){
						currentFeedback = feedbackData.T26M2;
					}
					else if ((numerator != 0) && 
					         (((numerator!=startNumerator) && (denominator!=startDenominator)) ||
					 ((numerator!=endNumerator) && (denominator!=endDenominator)))){
						currentFeedback = feedbackData.T26M3;
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


			else if (taskID.Equals("task2.4.setA.area") || taskID.Equals("task2.4.setB.area") || taskID.Equals("task2.4.setC.area") ||
			    taskID.Equals("task2.4.setA.numb") || taskID.Equals("task2.4.setB.numb") || taskID.Equals("task2.4.setC.numb") ||
			    taskID.Equals("task2.4.setA.sets") || taskID.Equals("task2.4.setB.sets") || taskID.Equals("task2.4.setC.sets") ||
			    taskID.Equals("task2.4.setA.liqu") || taskID.Equals("task2.4.setB.liqu") || taskID.Equals("task2.4.setC.liqu")){
			
				checkForFeedbackFollowed ();

				int startNumerator = 0;
				int startDenominator = 0;
				string representation = "area";

				if (taskID.Equals("task2.4.setA.area")){
					startNumerator = 1;
					startDenominator = 2;
				}
				else if (taskID.Equals("task2.4.setB.area")){
					startNumerator = 3;
					startDenominator = 4;
				}
				else if (taskID.Equals("task2.4.setC.area")){
					startNumerator = 7;
					startDenominator = 3;
				}
				else if (taskID.Equals("task2.4.setA.numb")){
					startNumerator = 1;
					startDenominator = 2;
					representation = "number line";
				}
				else if (taskID.Equals("task2.4.setB.numb")){
					startNumerator = 3;
					startDenominator = 4;
					representation = "number line";
				}
				else if (taskID.Equals("task2.4.setC.numb")){
					startNumerator = 7;
					startDenominator = 3;
					representation = "number line";
				}
				else if (taskID.Equals("task2.4.setA.sets")){
					startNumerator = 1;
					startDenominator = 2;
					representation = "sets";
				}
				else if (taskID.Equals("task2.4.setB.sets")){
					startNumerator = 3;
					startDenominator = 4;
					representation = "sets";
				}
				else if (taskID.Equals("task2.4.setC.sets")){
					startNumerator = 7;
					startDenominator = 3;
					representation = "sets";
				}
				else if (taskID.Equals("task2.4.setA.liqu")){
					startNumerator = 1;
					startDenominator = 2;
					representation = "liquid measures";
				}
				else if (taskID.Equals("task2.4.setB.liqu")){
					startNumerator = 3;
					startDenominator = 4;
					representation = "liquid measures";
				}
				else if (taskID.Equals("task2.4.setC.liqu")){
					startNumerator = 7;
					startDenominator = 3;
					representation = "liquid measures";
				}

				Fraction currentFraction = studentModel.getCurrentFraction ();
				
				if (currentFraction != null) {
					int numerator = currentFraction.getNumerator ();
					int denominator = currentFraction.getDenominator ();
					int partition = currentFraction.getPartition ();
					
					if (partition != 0) {
						numerator = numerator * partition;
						denominator = denominator * partition;
					}


					Debug.Log (":: numerator: "+numerator);
					Debug.Log (":: denominator: "+denominator);
					Debug.Log (":: equivalent: "+equivalent(numerator, denominator, startNumerator, startDenominator));
					Debug.Log (":: checkCurrentFractionsForEquivalence: "+checkCurrentFractionsForEquivalence(startNumerator, startDenominator));
					Debug.Log (":: getComparedFractions: "+studentModel.getComparedFractions());
					Debug.Log (":: getComparedResult: "+studentModel.getComparedResult());

					if ((numerator == 0) && (denominator == 0)) {
						currentFeedback = feedbackData.S3;
					}



					else if (equivalent(numerator, denominator, startNumerator, startDenominator) && 
					         (studentModel.getCurrentFractions().Count == 1)){
						//studentModel.setTaskCompleted(true);
						currentFeedback = feedbackData.T24M12;
					}

					else if (checkCurrentFractionsForEquivalence(startNumerator, startDenominator) && studentModel.getComparedResult()){
						studentModel.setTaskCompleted(true);
						currentFeedback = feedbackData.T24E1;
					}
					else if (checkCurrentFractionsForEquivalence(startNumerator, startDenominator) && !studentModel.getComparedResult()){
						currentFeedback = feedbackData.T24M11;
					}
					else if (currentSetContainsEqivalentFraction(startNumerator, startDenominator) && 
					         (!currentSetContainsFraction(startNumerator, startDenominator))){
						currentFeedback = feedbackData.T24M13;
					}
					else if ((numerator == startNumerator) && (denominator == startDenominator)) {
						currentFeedback = feedbackData.T24M7;
					}
					else if ((numerator == 0) && ((denominator == startDenominator) || multiple(denominator, startDenominator))){
						currentFeedback = feedbackData.T24M6;
					}

					else if (!equivalent(numerator, denominator, startNumerator, startDenominator) &&
					         ((numerator != 0) && (!multiple(numerator, startNumerator))) && 
					         ((denominator == startDenominator) || multiple(denominator, startDenominator))){
						currentFeedback = feedbackData.T24M9;
					} 
					else if (studentModel.getPreviousFeedback().getID().Equals(feedbackData.T24M8.getID ())){
						currentFeedback = feedbackData.T24M5;
					}
					else if (!equivalent(numerator, denominator, startNumerator, startDenominator) &&
					         ((numerator != 0) && (multiple(numerator, startNumerator))) && 
					         ((denominator == startDenominator) || multiple(denominator, startDenominator))){
						currentFeedback = feedbackData.T24M8;
					} 

					else if (!equivalent(numerator, denominator, startNumerator, startDenominator) &&
					         (numerator == 0)  && 
					         ((denominator == startDenominator) || multiple(denominator, startDenominator))){
						currentFeedback = feedbackData.T24M10;
					} 

					else if (studentModel.getPreviousFeedback().getID().Equals(feedbackData.T24M1.getID ()) &&
					         (numerator == startDenominator)){
						currentFeedback = feedbackData.T24M5;
					}
					else if (((denominator == startDenominator) || (multiple(denominator, startDenominator))) &&
							(studentModel.getPreviousFeedback().getID().Equals(feedbackData.T24M1.getID ()) ||
					         studentModel.getPreviousFeedback().getID().Equals(feedbackData.T24M2.getID ()))){
						currentFeedback = feedbackData.T24M4;
					}
					else if ((denominator != startDenominator) && (!multiple(denominator, startDenominator))){
						currentFeedback = feedbackData.T24M1;
					}
					else if (numerator == startDenominator){
						currentFeedback = feedbackData.T24M2;
					}
					else if (((numerator != startNumerator) || (denominator != startDenominator)) && 
					         !equivalent(numerator, denominator, startNumerator, startDenominator)){
						currentFeedback = feedbackData.T24M3;
					}
					else if ((denominator == startDenominator) || (multiple(denominator, startDenominator))){
						currentFeedback = feedbackData.T24M6;
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



			if (taskID.Equals ("task2.2")) {
				checkForFeedbackFollowed ();
				
				Fraction currentFraction = studentModel.getCurrentFraction ();

				if (currentFraction != null) {
					int numerator = currentFraction.getNumerator ();
					int denominator = currentFraction.getDenominator ();
					int partition = currentFraction.getPartition ();
					
					if (partition != 0) {
						numerator = numerator * partition;
						denominator = denominator * partition;
					}
					
					if ((numerator == 0) && (denominator == 0)) {
						currentFeedback = feedbackData.S3;
					}
					else if ((denominator != 0) && (numerator == 0)){
						currentFeedback = feedbackData.F2M1;
					}
					else if ((partition == 0)  && (denominator != 0) && (numerator != 0) && 
					         studentModel.getPreviousFeedback().getID().Equals(feedbackData.F2M1.getID ())){
						currentFeedback = feedbackData.F2M4;
					}
					else if ((partition == 0)  && (denominator != 0) && (numerator != 0)){
						currentFeedback = feedbackData.F2M6;
					}
					else if ((partition == 2)  && (denominator != 0) && (numerator != 0)){
						currentFeedback = feedbackData.F2M7;
					}

					else if ((partition == 3)  && (denominator != 0) && (numerator != 0)){
						currentFeedback = feedbackData.F2M7b;
					}

					else if ((partition == 4)  && (denominator != 0) && (numerator != 0)){
						currentFeedback = feedbackData.F2M7c;
					}

					else if ((partition == 5)  && (denominator != 0) && (numerator != 0)){
						studentModel.setTaskCompleted(true);
						currentFeedback = feedbackData.F2E1;
					}
					else if ((partition != 2) && (partition != 3) && (partition != 4) 
					         && (partition != 5) && (partition != 0)){
						currentFeedback = feedbackData.F2M11;
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

			if (taskID.Equals("task2.1")){
				checkForFeedbackFollowed();
				Debug.Log (":::: Hier in Task 2.1 ::: ");

				Fraction currentFraction =studentModel.getCurrentFraction();
				if (currentFraction != null){
					int numerator = currentFraction.getNumerator();
					int denominator = currentFraction.getDenominator();
					int partition = currentFraction.getPartition();

					if (partition != 0){
						numerator = numerator * partition;
						denominator = denominator * partition;
					}

					Debug.Log (":::: numerator: "+numerator);
					Debug.Log (":::: denominator: "+denominator);
					Debug.Log (":::: sameRepresentations: "+sameRepresentations());
					Debug.Log (":::: sameValues: "+sameValues());
					Debug.Log (":::: createdReps: "+createdReps());
					Debug.Log (":::: fourWithDiffRepAndSameValues: "+fourWithDiffRepAndSameValues());

					if ((numerator ==0) && (denominator ==0)){
						currentFeedback = feedbackData.S3;
					}
					//else if (!sameRepresentations() && sameValues() && (createdReps() == 2)){
					//	currentFeedback = feedbackData.FM10;
					//}

					else if (!sameRepresentations() && sameValues() && (createdReps() < 4)){
						currentFeedback = feedbackData.FM11;
					}
					else if (!sameRepresentations() && (!sameValues() && (createdReps() > 1))){
						currentFeedback = feedbackData.FM12;
					}
					else if (fourWithDiffRepAndSameValues()){
						studentModel.setTaskCompleted(true);
						currentFeedback = feedbackData.FE1;
					}
					//else if (!sameRepresentations() && sameValues() && (createdReps() >= 4)){
					//	studentModel.setTaskCompleted(true);
					//	currentFeedback = feedbackData.FE1;
					//}

					else if ((numerator !=0) || (denominator !=0)) {
						currentFeedback = feedbackData.FM6;
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

			if (taskID.Equals("task1.1setA")){
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
						//currentFeedback = feedbackData.M13;
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

			if (taskID.Equals("task2.7.setA") ||taskID.Equals("task2.7.setB") || taskID.Equals("task2.7.setC")){
				checkForFeedbackFollowed();

				Debug.Log ("task2.7.setB or EQUIValence2");
				bool correctSolution = false;
				bool correctDenominator = false;

				int startNumerator = 0;
				int endNumerator = 0;
				int startDenominator = 0;
				int endDenominator = 0;

				if (taskID.Equals("task2.7.setA")){
					startNumerator = 1;
					endNumerator = 3;
					startDenominator = 6;
					endDenominator = 18;
				}
				else if (taskID.Equals("task2.7.setB")){
					startNumerator = 3;
					endNumerator = 9;
					startDenominator = 4;
					endDenominator = 12;
				}
				else if (taskID.Equals("task2.7.setC")){
					startNumerator = 7;
					endNumerator = 28;
					startDenominator = 3;
					endDenominator = 12;
				}
			


				Fraction currentFraction =studentModel.getCurrentFraction();

				Debug.Log (":::: currentFraction: "+currentFraction);

				if (currentFraction != null){
						
					int numerator = currentFraction.getNumerator();
					int denominator = currentFraction.getDenominator();
					int partition = currentFraction.getPartition();


						
					if (partition != 0){
						numerator = numerator * partition;
						denominator = denominator * partition;
					}

					Debug.Log (":::: numerator: "+numerator+" denominator: "+denominator);
					Debug.Log (":::: getComparedResult: "+studentModel.getComparedResult());
					Debug.Log (":::: currentSetIncludesFraction END: "+currentSetIncludesFraction(endNumerator,endDenominator));
					Debug.Log (":::: currentSetIncludesFraction START: "+currentSetIncludesFraction(startNumerator,startDenominator));

					if (!studentModel.getComparedResult() && currentSetIncludesFraction(endNumerator,endDenominator) && currentSetIncludesFraction(startNumerator,startDenominator)){
						currentFeedback = feedbackData.M13;
					}

					else if (studentModel.getComparedResult() && currentSetIncludesFraction(endNumerator,endDenominator) && currentSetIncludesFraction(startNumerator,startDenominator)){
						studentModel.setTaskCompleted(true);
						currentFeedback = feedbackData.E1;
					}

					//else if ((numerator == endNumerator) && (denominator == endDenominator)) {
					//	Debug.Log ("solution found ");
					//	studentModel.setTaskCompleted(true);
					//	currentFeedback = feedbackData.E1;
					//}

					else if (currentSetIncludesFraction(endNumerator,endDenominator) && 
					         (onlyOneFraction() || (!currentSetIncludesFraction(startNumerator,startDenominator)))){
						currentFeedback = feedbackData.M8;
					}
					else if (currentSetIncludesFraction(startNumerator,startDenominator) && 
					         ((!onlyOneFraction()) || (!currentSetIncludesFraction(endNumerator,endDenominator)))){
						currentFeedback = feedbackData.M9;
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
				         studentModel.getPreviousFeedback().getID().Equals(feedbackData.M10.getID ())){
						currentFeedback = feedbackData.M11;
					}

					else if ((numerator != endNumerator) && (denominator == endDenominator) && 
				         (!studentModel.getReflectionForDenominatorShown())){
						currentFeedback = feedbackData.M12;
						studentModel.setReflectionForDenominatorShown(true);
					}

					else if ((numerator != endNumerator) && (denominator == endDenominator) && studentModel.getReflectionForDenominatorShown()){
						currentFeedback = feedbackData.M10;
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
