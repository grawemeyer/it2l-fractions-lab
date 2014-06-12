using System;

namespace taskDependentSupport.core
{
		public class Fraction
		{

			private static string id;
			private string name;
			private int nominator;
			private int denominator;
			private int partition;

			public Fraction ()
			{
			}


			public void setID(string value)
			{
				id = value;
			}

			public void setName(string value)
			{
				name = value;	
			}

			public void setNominator(int value)
			{
				nominator = value;
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

			public int getNominator()
			{
				return nominator;
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

