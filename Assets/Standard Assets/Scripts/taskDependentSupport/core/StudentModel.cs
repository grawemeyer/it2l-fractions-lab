﻿using UnityEngine;
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
		private int lastDisplayedMessageID = 0;
		private int lastDisplayedMessageType = 0; 
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
		private FeedbackElem currentFeedback = new FeedbackElem ();
		private int currentFeedbackLevel = 0;
		private bool previousViewed = false;
		private bool languageEnglish = true;
		private bool languageGerman = false;
		private bool languageSpanish = false;
		private bool popUpClosed = true;
		private String messageRule = "";
		private String followedRule = "";
		private String feedbackCounterRule = "";
		private FeedbackElem viewedFeedback = new FeedbackElem ();
		private String viewedMessage = "";
		private bool addedFractions = false;
		private Fraction currentAddedFraction;


		public void setCurrentAddedFraction(String value){
			//vrect_1_cut
			int valueLength = value.Length;
			String id = value;
			if (valueLength > 4){
				id = value.Substring (0, (valueLength-4));
			}
			Debug.Log ("addedFraction value: "+value);
			Debug.Log ("addedFraction id: "+id);
			currentAddedFraction = getFraction(id);
		}

		public Fraction getcurrentAddedFraction(){
			return currentAddedFraction;
		}

		public void setAddedFractions(bool value){
			addedFractions = value;
		}

		public bool getAddedFractions(){
			return addedFractions;
		}

		public void setViewedMessage (String value){
			viewedMessage = value;
		}

		public String getViewedMessage(){
			return viewedMessage;
		}

		public void setFeedbackElemViewed(FeedbackElem current){
			viewedFeedback = current;
		}

		public FeedbackElem getFeedbackElemViewed (){
			return viewedFeedback;
		}

		public void setFeedbackCounterRule(String value){
			feedbackCounterRule = value;
		}
		
		public String getFeedbackCounterRule(){
			return feedbackCounterRule;
		}

		public void setFeedbackFollowedRule(String rule){
			followedRule = rule;
		}
		
		public String getFeedbackFollowedRule(){
			return followedRule;
		}

		public void setMessageRule(String rule){
			messageRule = rule;
		}

		public String getMessageRule(){
			return messageRule;
		}

		public void setPopUpClosed(bool value){
			popUpClosed = value;
		}

		public bool getPopUpClosed(){
			return popUpClosed;
		}

		public void setLanguageEnglish(bool value){
			languageEnglish = value;
		}

		public bool getLanguageEnglish(){
			return languageEnglish;
		}

		public void setLanguageGerman(bool value){
			languageGerman = value;
		}
		
		public bool getLanguageGerman(){
			return languageGerman;
		}

		public void setLanguageSpanish(bool value){
			languageSpanish = value;
		}
		
		public bool getLanguageSpanish(){
			return languageSpanish;
		}

		public StudentModel(String taskID){
			feedbackData = new FeedbackData (taskID, this); 
		}

		public void setPreviousViewed(bool value){
			previousViewed = value;
		}

		public bool getPreviousViewed(){
			return previousViewed;
		}

		public int getCurrentFeedbackLevel(){
			return currentFeedbackLevel;
		}

		public void setCurrentFeedbackLevel(int value){
			currentFeedbackLevel = value;
		}

		public void setCurrentFeedback(FeedbackElem value){
			currentFeedback = value;
		}

		public FeedbackElem getCurrentFeedback(){
			return currentFeedback;
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

		private bool fractionIncludedInSet(String id){
			for (int i = 0; i < currentFractions.Count; i++) 
			{
				Fraction current = currentFractions[i];
				if (current.getID() == id) 
				{
					return true;
				}
			}
			return false;
		}

		public void setCurrentFraction(String id){
			if (fractionIncludedInSet (id)) {
				currentFraction = getFraction (id);
			}
		}



		public Fraction getCurrentFraction(){
			return currentFraction;
		}

		public void setDisplayedMessageType(int value)
		{
			lastDisplayedMessageType = value;
		}
		
		public int getlastDisplayedMessageType()
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
			Debug.Log ("<< remove >> id: "+id);
			for (int i = 0; i < currentFractions.Count; i++) 
			{
				Fraction current = currentFractions[i];
				if (current.getID() == id) 
				{
					currentFractions.RemoveAt(i);
					i = currentFractions.Count;
				}
			}
			int newCount = currentFractions.Count;
			Debug.Log (" << newCount >> "+newCount);
			if (newCount > 0) {
				currentFraction = currentFractions [newCount - 1];
				Debug.Log ("<< currentFraction >> id: "+currentFraction.getID ());
			} 
			else {
				currentFraction = null;
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
