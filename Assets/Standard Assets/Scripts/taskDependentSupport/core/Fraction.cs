using System;

namespace taskDependentSupport.core
{
		public class Fraction
		{

			private string id;
			private string name;
			private int numerator = 0;
			private int denominator = 0;
			private int partition = 0;
			private bool anyValue = false;
			private bool speech = false;
			private bool comparison = false;
			private bool sameRep = false; 
			private bool differentRep=false;
			private bool sameValue=false;
			private bool numeratorAnyValue = false;
			private bool equivalentFraction = false;
			private bool partitionBool = false;
			private int[] denominators;
			private int[] numerators;
			private int numeratorAdd = 0;
			private int denominatorAdd = 0;
			private int numeratorAddEnd = 0;
			private int denominatorAddEnd = 0;
			private bool additionBox = false;
			private int[] partitionValues;
			private int repsOnScreen = 0;
			
			public Fraction ()
			{
			}

			public void setRepsOnScreenBelow(int value){
				repsOnScreen = value;
			}

			public int getRepsOnScreenBelow(){
				return repsOnScreen;
			}
			
			public void setPartitionValues(int[] values){
				partitionValues = values;
			}

			public int[] getPartitionValues(){
			return partitionValues;
			}

			public void setAdditionBox(bool value){
				additionBox = value;
			}

			public bool getAdditionBox(){
				return additionBox;
			}

			public void setFractionForAdditionTaskEnd(int numerator, int denominator){
				numeratorAddEnd = numerator;
				denominatorAddEnd = denominator;
			}

			public int getNumeratorForAdditionTaskEnd(){
				return numeratorAddEnd;
			}
		
			public int getDenominatorForAdditionTaskEnd(){
				return denominatorAddEnd;
			}

			public void setFractionForAdditionTask(int numerator, int denominator){
				numeratorAdd = numerator;
				denominatorAdd = denominator;
			}

			public int getNumeratorForAdditionTask(){
				return numeratorAdd;
			}

			public int getDenominatorForAdditionTask(){
				return denominatorAdd;
			}

			public void setNumerators(int[] values){
				numerators = values;
			}
		
			public int[] getNumerators(){
				return numerators;
			}

			public void setDenominators(int[] values){
				denominators = values;
			}

			public int[] getDenominators(){
				return denominators;
			}

			public void setPartitionBool(bool value){
				partitionBool = value;
			}

			public bool getPartitionBool(){
				return partitionBool;
			}

			public void setEquivalentFraction(bool value){
				equivalentFraction = value;
			}

			public bool getEquivalentFraction(){
				return equivalentFraction;		
			}

			public void setNumeratorAnyValue(bool value){
				numeratorAnyValue = value;
			}

			public bool getNumeratorAnyValue(){
				return numeratorAnyValue;		
			}
			
			public void allSameValue(bool value){
				sameValue = value;
			}

			public bool getAllSameValue(){
				return sameValue;
			}

			public void differntRepresentation(bool value){
				differentRep = value;
			}

			public bool getDifferentRepresentation(){
				return differentRep;
			}

			public void sameRepresentation(bool value){
				sameRep = value;
			}

			public bool getSameRepresentation(){
				return sameRep;
			}

			public void setComparison(bool value)
			{
				comparison = value;
			}

			public bool getComparison()
			{
				return comparison;	
			}


			public void setSpeech(bool value)
			{
				speech = value;
			}

			public bool getSpeech()
			{
				return speech;	
			}


			public void setAnyValue(bool value)
			{
				anyValue = value;
			}
			
			public bool getAnyValye()
			{
				return anyValue;
			}

			public void setID(string value)
			{
				id = value;
			}

			public void setName(string value)
			{
				name = value;	
			}

			public void setNumerator(int value)
			{
				numerator = value;
			}

			public void setDenominator(int value)
			{
				denominator = value;
			}

			public void setPartition(int value)
			{
				partition = value;
			}

			
			public string getID()
			{
				return id;	
			}

			public string getName()
			{
				return name;
			}

			public int getNumerator()
			{
				return numerator;
			}

			public int getDenominator()
			{
				return denominator;
			}

			public int getPartition()
			{
				return partition;
			}


		}
}

