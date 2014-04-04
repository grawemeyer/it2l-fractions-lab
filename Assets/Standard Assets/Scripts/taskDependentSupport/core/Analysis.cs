using UnityEngine;
using System.Collections;

namespace taskDependentSupport.core
{

	public class Analysis {


		public void analyseEvent(string type, string name, string id)
		{
			if (type.Equals ("ClickButton")){
				if (name.Equals ("Equivalence")){
					if (StudentModel.getEquivalenceOpen() == 0) StudentModel.setEquivalenceOpen(1);
					else StudentModel.setEquivalenceOpen(0);
				}
			}
			if (type.Equals ("FractionGenerated")){
				StudentModel.setCurrentFractions(StudentModel.getCurrentFractions() +1);
				StudentModel.setCompared(false);
				StudentModel.setComparedResult(false);
			}
			if (type.Equals ("FractionTrashed")){
				StudentModel.setCurrentFractions(StudentModel.getCurrentFractions() -1);
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
