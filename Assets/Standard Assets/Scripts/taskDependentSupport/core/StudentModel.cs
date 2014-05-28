using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace taskDependentSupport.core
{
	public class StudentModel 
	{
		private static int equivalenceOpen = 0;
		private static List<Fraction> currentFractions = new List<Fraction>();
		private static bool compared = false;
		private static bool comparedResult = false;
		private static long time = 0;

		public static void setEventTime(long value)
		{
			time = value;
		}

		public static long getEventTime()
		{
			return time;
		}

		public static int getEquivalenceOpen()
		{
			return equivalenceOpen;
		}
		
		public static void setEquivalenceOpen(int value)
		{
			equivalenceOpen = value;
		}


		public static List<Fraction> getCurrentFractions()
		{
			return currentFractions;
		}

		public static void addCurrentFractions(Fraction value)
		{
			currentFractions.Add(value);
		}

		public static void removeFraction(String id)
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

		public static void setNumeratorAtFraction(String id, int value)
		{
			Fraction current = getCurrentFraction(id);
			current.setNominator (value);
		}

		public static void setDenominatorAtFraction(String id, int value)
		{
			Fraction current = getCurrentFraction(id);
			current.setDenominator(value);
		}

		public static void setPartitionAtFraction(String id, int value)
		{
			Fraction current = getCurrentFraction(id);
			current.setPartition(value);
		}

		private static Fraction getCurrentFraction(String id)
		{
			for (int i = 0; i < currentFractions.Count; i++) {
				Fraction current = currentFractions [i];
				if (current.getID () == id) return current;
			}
			return null;
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
