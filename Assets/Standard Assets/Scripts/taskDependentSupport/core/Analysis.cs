using UnityEngine;
using System.Collections;

namespace taskDependentSupport.core
{

	public class Analysis {



		public void analyseEvent(StudentModel studentModel, string type, string name, string id, string value, int fractionsValue, string position, long time)
		{
			studentModel.setEventTime(time);
			studentModel.setCurrentFraction(id);

			if (type.Equals ("ClickButton")){
				if (name.Equals ("Equivalence")){
					if (studentModel.getEquivalenceOpen() == 0) studentModel.setEquivalenceOpen(1);
					else studentModel.setEquivalenceOpen(0);
				}
			}
			if (type.Equals ("FractionGenerated")){
				Fraction thisFraction = new Fraction();
				thisFraction.setName(name);
				thisFraction.setID(id);
				studentModel.addCurrentFractions(thisFraction);
				studentModel.setCompared(false);
				studentModel.setComparedResult(false);

			}
			if (type.Equals("FractionChange")){
				if (name.Equals("Numerator")) studentModel.setNumeratorAtFraction(id, fractionsValue);
				if (name.Equals("Denominator")) studentModel.setDenominatorAtFraction(id, fractionsValue);
				if (name.Equals("Partitions")) studentModel.setPartitionAtFraction(id, fractionsValue);
			}

			if (type.Equals ("FractionTrashed")){
				studentModel.removeFraction(id);
				studentModel.setCompared(false);
				studentModel.setComparedResult(false);
			}
			if (type.Equals ("OperationResult")){
				studentModel.setCompared(true);
				if (name.Equals ("True")){
					studentModel.setComparedResult(true);
				}
				else {
					studentModel.setComparedResult(false);
				}
			}

		
		}

	
	}
}
