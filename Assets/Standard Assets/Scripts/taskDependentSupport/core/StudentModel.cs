using UnityEngine;
using System.Collections;

namespace taskDependentSupport.core
{
	public class StudentModel 
	{
		private static int equivalenceOpen = 0;
		private static int currentFractions = 0;
		private static bool compared = false;
		private static bool comparedResult = false;

		public static int getEquivalenceOpen()
		{
			return equivalenceOpen;
		}
		
		public static void setEquivalenceOpen(int value)
		{
			equivalenceOpen = value;
		}


		public static int getCurrentFractions()
		{
			return currentFractions;
		}

		public static void setCurrentFractions(int value)
		{
			currentFractions = value;
		}

		public static bool getCompared()
		{
			return compared;
		}
		
		public static void setCompared(bool value)
		{
			compared = value;
		}

		public static bool getComparedResult()
		{
			return comparedResult;
		}
		
		public static void setComparedResult(bool value)
		{
			comparedResult = value;
		}



	}
}
