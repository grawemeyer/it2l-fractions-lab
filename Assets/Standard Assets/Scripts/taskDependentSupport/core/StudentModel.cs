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
		private long time = 0;
		private int lastDisplayedMessageID = 0;
		private string lastDisplayedMessageType = ""; 
		private int misconception = 0;
		private bool completed = false;
		private int doneButtonPressed = 0;
		private bool partitionUsed = false;
		private bool nominatorDenominatorMisconception = false;
		private bool askForComparison = false;



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
			if (doneButtonPressed <= 1)return true;
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
			current.setNominator (value);
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
				if (current.getID () == id) return current;
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
