using UnityEngine;
using System.Collections;

namespace taskDependentSupport.core
{

	public class Analysis {

		 
		public void analyseEvent(string type, string name, string id, string value, int fractionsValue, string position, long time)
		{
			StudentModel.setEventTime(time);

			if (type.Equals ("ClickButton")){
				if (name.Equals ("Equivalence")){
					if (StudentModel.getEquivalenceOpen() == 0) StudentModel.setEquivalenceOpen(1);
					else StudentModel.setEquivalenceOpen(0);
				}
			}
			if (type.Equals ("FractionGenerated")){
				Fraction thisFraction = new Fraction();
				thisFraction.setName(name);
				thisFraction.setID(id);
				StudentModel.addCurrentFractions(thisFraction);
				StudentModel.setCompared(false);
				StudentModel.setComparedResult(false);
			}
			if (type.Equals("FractionChange")){
				if (name.Equals("Numerator")) StudentModel.setNumeratorAtFraction(id, fractionsValue);
				if (name.Equals("Denominator")) StudentModel.setDenominatorAtFraction(id, fractionsValue);
				if (name.Equals("Partition")) StudentModel.setPartitionAtFraction(id, fractionsValue);
			}

			if (type.Equals ("FractionTrashed")){
				StudentModel.removeFraction(id);
				StudentModel.setCompared(false);
				StudentModel.setComparedResult(false);
			}
			if (type.Equals ("OperationResult")){
				StudentModel.setCompared(true);
				if (name.Equals ("True")){
					StudentModel.setComparedResult(true);
				}
				else {
					StudentModel.setComparedResult(false);
				}
			}
		
		}


	}
}
