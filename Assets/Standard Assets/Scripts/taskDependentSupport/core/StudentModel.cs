using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace taskDependentSupport.core
{
	public class StudentModel 
	{
		private int equivalenceOpen = 0;
		private List<Fraction> currentFractions = new List<Fraction>();
		private Fraction currentFraction; 
		private bool compared = false;
		private bool comparedResult = false;
		private bool comparedFractions = false;
		private long time = 0;
		private int lastDisplayedMessageID = 0;
		private string lastDisplayedMessageType = ""; 
		private int misconception = 0;
		private bool completed = false;
		private int doneButtonPressed = 0;
		private bool partitionUsed = false;
		private bool nominatorDenominatorMisconception = false;
		private bool askForComparison = false;
		private FeedbackData feedbackData;
		private FeedbackElem previous = new FeedbackElem();
		private bool reflectionForDenominatorShown = false;
		private List<FeedbackElem> feedbackProvided = new List<FeedbackElem>();
		private List<bool> feedbackFollowed = new List<bool>();
		private bool additionBox = false;
		private bool substractionBox = false;

		public StudentModel(String taskID){
			feedbackData = new FeedbackData (taskID); 
		}


		public void setSubstractionBox(bool value){
			substractionBox = value;
		}
		
		public bool getSubstrationBox(){
			return substractionBox;
		}


		public void setAdditionBox(bool value){
			additionBox = value;
		}

		public bool getAdditionBox(){
			return additionBox;
		}

		public void setComparedFractions(bool elem){
			comparedFractions = elem;
		}

		public bool getComparedFractions(){
			return comparedFractions;
		}

		public List<bool> getFeedbackFollowed(){
			return feedbackFollowed;
		}
		
		public void addFeedbackFollowed (bool elem){
			feedbackFollowed.Add (elem);
		}

		public List<FeedbackElem> getFeedbackProvided(){
			return feedbackProvided;
		}

		public void addFeedbackProvided (FeedbackElem elem){
			feedbackProvided.Add (elem);
		}

		public void setReflectionForDenominatorShown(bool elem){
			reflectionForDenominatorShown = elem;
		}

		public bool getReflectionForDenominatorShown(){
			return reflectionForDenominatorShown;
		}

		public void setPreviousFeedback(FeedbackElem elem){
			previous = elem;
		}

		public FeedbackElem getPreviousFeedback(){
			return previous;
		}

		public void setFeedbackData(FeedbackData elem){
			feedbackData = elem;
		}

		public FeedbackData getFeedbackData(){
			return feedbackData;
		}

		public void setAskForComparison(bool value){
			askForComparison = value;
		}

		public bool getAskForComparison(){
			return askForComparison;
		}

		public void setNominatorDenominatorMisconception(bool value){
			nominatorDenominatorMisconception = true;
		}

		public bool getNominatorDenominatorMisconception(){
			return nominatorDenominatorMisconception;
		}

		public void setPartitionUsed(bool value){
			partitionUsed = value;
		}

		public bool getParticitionUsed(){
			return partitionUsed;
		}

		public void resetDoneButtonPressed(){
			doneButtonPressed = 0;
		}

		public void setDoneButtonPressed(){
			doneButtonPressed += 1;
		}

		public bool firstDoneButtonPressed(){
			if (doneButtonPressed < 1)return true;
			return false;
		}

		public bool isTaskCompleted(){
			return completed;
		}

		public void setTaskCompleted(bool value){
			completed = value;
		}

		public void setMisconceptionNominatorForgotten(){
			misconception = 1;
		}

		public bool isMisconceptionNominatorForgotten(){
			if (misconception == 1) return true;
			return false;
		}

		public void setCurrentFraction(String id){
			currentFraction = getFraction (id);
		}

		public Fraction getCurrentFraction(){
			return currentFraction;
		}

		public void setDisplayedMessageType(string value)
		{
			lastDisplayedMessageType = value;
		}
		
		public string getlastDisplayedMessageType()
		{
			return lastDisplayedMessageType;
		}

		public void setDisplaydMessageID(int value)
		{
			lastDisplayedMessageID = value;
		}
		
		public int getlastDisplayedMessageID()
		{
			return lastDisplayedMessageID;
		}

		public void setEventTime(long value)
		{
			time = value;
		}

		public long getEventTime()
		{
			return time;
		}

		public int getEquivalenceOpen()
		{
			return equivalenceOpen;
		}
		
		public void setEquivalenceOpen(int value)
		{
			equivalenceOpen = value;
		}


		public List<Fraction> getCurrentFractions()
		{
			return currentFractions;
		}

		public void addCurrentFractions(Fraction value)
		{
			currentFractions.Add(value);
		}

		public void removeFraction(String id)
		{
			for (int i = 0; i < currentFractions.Count; i++) 
			{
				Fraction current = currentFractions[i];
				if (current.getID() == id) 
				{
					currentFractions.RemoveAt(i);
					i = currentFractions.Count;
				}
			}
		}

		public void setNumeratorAtFraction(String id, int value)
		{
			Fraction current = getFraction(id);
			current.setNumerator (value);
		}

		public void setDenominatorAtFraction(String id, int value)
		{
			Fraction current = getFraction(id);
			current.setDenominator(value);
		}

		public void setPartitionAtFraction(String id, int value)
		{
			Fraction current = getFraction(id);
			current.setPartition(value);
		}

		private Fraction getFraction(String id)
		{
			for (int i = 0; i < currentFractions.Count; i++) {
				Fraction current = currentFractions [i];
				if (current.getID ().Equals(id)) return current;
			}
			return null;
		}

		public bool getCompared()
		{
			return compared;
		}
		
		public void setCompared(bool value)
		{
			compared = value;
		}

		public bool getComparedResult()
		{
			return comparedResult;
		}
		
		public void setComparedResult(bool value)
		{
			comparedResult = value;
		}



	}
}
