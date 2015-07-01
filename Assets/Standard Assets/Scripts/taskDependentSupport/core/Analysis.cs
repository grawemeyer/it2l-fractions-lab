using UnityEngine;
using System.Collections;

namespace taskDependentSupport.core
{

	public class Analysis {



		public void analyseEvent(StudentModel studentModel, string type, string name, string id, string value, int fractionsValue, string position)
		{
			if (!id.Equals ("")) {
				studentModel.setCurrentFraction(id);
			}


			Debug.Log ("::: ANAYLSE: "+type+" name: "+name+" id: "+id+" value: "+value+" fractionsValue: "+fractionsValue);

			if (type.Equals ("ActionEvent")){
				if (name.Equals ("ProperSum")){
					studentModel.setAddedFractions(true);
					studentModel.setCurrentAddedFraction(value);
				}
			}

			if (type.Equals ("ClickButton")){
				if (name.Equals ("Equivalence")){
					if (studentModel.getEquivalenceOpen() == 0) studentModel.setEquivalenceOpen(1);
					else {
						studentModel.setEquivalenceOpen(0);
						studentModel.setCompared(false);
						studentModel.setComparedResult(false);
						studentModel.setComparedFractions(false);
					}
				}
			}

			if (type.Equals ("FractionGenerated") || type.Equals ("EquivalenceGenerated")){
				Fraction thisFraction = new Fraction();
				thisFraction.setName(name);
				thisFraction.setID(id);
				if (studentModel.getAddedFractions()){
					Debug.Log ("<<<<< added Fraction!!!!!");
					Fraction addedFraction = studentModel.getcurrentAddedFraction();
					Debug.Log ("<<<< added Fraction "+addedFraction);
					int currentDenoninator = 0;
					if (addedFraction != null){
						currentDenoninator = addedFraction.getDenominator();
					}
					Debug.Log ("<<<< acurrentDenoninator "+currentDenoninator);
					thisFraction.setDenominator (currentDenoninator);
					Debug.Log ("<<<< after setDenominator at id"+id+" denominator: "+currentDenoninator);
					studentModel.setAddedFractions(false);
				}
				studentModel.addCurrentFractions(thisFraction);
				studentModel.setCompared(false);
				studentModel.setComparedResult(false);
				studentModel.setComparedFractions(false);
				studentModel.setCurrentFraction(id);
				taskDependentSupport.TDSWrapper.sendRepresentationTypeToSNA(name);
				taskDependentSupport.TDSWrapper.SaveEvent ("TDS.fractionGenerated", id);
				Debug.Log ("!!!!! generated name: "+name+" id: "+id);

			}
			if (type.Equals("FractionChange")){
				if (name.Equals("Numerator")){
					studentModel.setNumeratorAtFraction(id, fractionsValue);
					taskDependentSupport.TDSWrapper.SaveEvent ("TDS.fractionChanged", id+" numerator "+fractionsValue);
				}
				if (name.Equals("Denominator")){
					studentModel.setDenominatorAtFraction(id, fractionsValue);
					taskDependentSupport.TDSWrapper.SaveEvent ("TDS.fractionChanged", id+" denominator "+fractionsValue);
				}

				if (name.Equals("Partitions")) {
					Debug.Log ("::::: partition: "+id+" value: "+fractionsValue);
					studentModel.setPartitionAtFraction(id, fractionsValue);
					taskDependentSupport.TDSWrapper.SaveEvent ("TDS.fractionChanged", id+" partition "+fractionsValue);
				}
				studentModel.setComparedResult(false);
				studentModel.setComparedResult(false);
				studentModel.setComparedFractions(false);
			}

			if (type.Equals ("FractionTrashed")){
				Debug.Log (" <<<< FractionTrashed >>>> id"+id);
				studentModel.removeFraction(id);
				Debug.Log (" <<<< after removed fraction >>>> id"+id);
				taskDependentSupport.TDSWrapper.SaveEvent ("TDS.fractionTrashed", id);
				studentModel.setCompared(false);
				studentModel.setComparedResult(false);
				studentModel.setComparedFractions(false);

			}

			if (type.Equals ("OperationResult")){
				if (name.Equals("Sum")) {
					Debug.Log (":::::: setAdditionBox ::::::");
					studentModel.setAdditionBox(true);
					taskDependentSupport.TDSWrapper.SaveEvent ("TDS.additionOperationBox", "open");
				}
				else if (name.Equals("Substraction")){
					studentModel.setSubstractionBox(true);
					taskDependentSupport.TDSWrapper.SaveEvent ("TDS.substractionOperationBox", "open");
				}
				else {
					studentModel.setCompared(true);
					Debug.Log ("::: student model set compared true");
					taskDependentSupport.TDSWrapper.SaveEvent ("TDS.compareOperationBox", "open");
					if (id.Equals ("=")){
						Debug.Log ("set compared result true");
						taskDependentSupport.TDSWrapper.SaveEvent ("TDS.compareOperationResult", "=");
						studentModel.setComparedResult(true);
					}
					else if (id.Equals (">") || id.Equals ("<")){
						Debug.Log ("set compared fraction true");
						taskDependentSupport.TDSWrapper.SaveEvent ("TDS.compareOperationResult", "> or <");
						studentModel.setComparedFractions(true);
					}
					else {
						studentModel.setComparedResult(false);
						studentModel.setComparedFractions(false);
					}
				}
			}

			if (type.Equals ("ReleaseFraction")){
				Fraction current = studentModel.getCurrentFraction ();
				if (current != null){
					string fractionId = current.getID();
					taskDependentSupport.TDSWrapper.SaveEvent ("TDS.fractionInUse", fractionId);
				}
				else {
					taskDependentSupport.TDSWrapper.SaveEvent ("TDS.fractionInUse", "null");
				}
			}
		
		}

	
	}
}
