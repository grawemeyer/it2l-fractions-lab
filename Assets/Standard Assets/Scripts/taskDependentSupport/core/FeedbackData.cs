using System;
using UnityEngine;
using System.Collections;
namespace taskDependentSupport.core

{
		
	public class FeedbackData
	{
				
		public FeedbackElem S1, S2, S3, M1, M2, M3, M4, M5, M6, M7, M8, M9, M10, M11, E1, E2, R1, R2, O1, O2;
		public FeedbackElem CM2, CM5, CM6, CM6Second, CM7, CM8, CM11, CM12, CE2;
		public FeedbackElem FM6, FM10, FM11, FE1, FE2;
		public FeedbackElem F2M1, F2M4, F2M6, F2M7, F2M7b, F2M7c,F2M10, F2M11, F2E1, F2E2;
		public FeedbackElem T24M1, T24M2, T24M3, T24M4, T24M5, T24M6, T24M7, T24M8, T24M9, T24M10, T24M11, T24E1, T24E2;
		public FeedbackElem T26M1, T26M2, T26M3, T26M4, T26M5, T26M6, T26M7, T26M8, T26M10, T26M11, T26E1, T26E2;
		public FeedbackElem T3aP1M1, T3aP1M2, T3aP1M3, T3aP1M4, T3aP1M5, T3aP1M6, T3aP1M7, T3aP1M8, T3aP1M9, T3aP1M10, T3aP1M11, T3aP1E1, T3aP1E2; 

		public FeedbackData (String taskID){
			Debug.Log (":::: FeedbackData taskID: "+taskID);

			int startNumerator = 0;
			int endNumerator = 0;
			int startDenominator = 0;
			int endDenominator = 0;
			String representation = "area";

			if (taskID.Equals ("task1.1setA")) {
				endDenominator = 3;
			}
			
			if (taskID.Equals("task2.7.setA")){
				startNumerator = 1;
				endNumerator = 3;
				startDenominator = 6;
				endDenominator = 18;
			}
			else if (taskID.Equals("task2.7.setB")){
				startNumerator = 3;
				endNumerator = 9;
				startDenominator = 4;
				endDenominator = 12;
			}
			else if (taskID.Equals("task2.7.setC")){
				startNumerator = 7;
				endNumerator = 28;
				startDenominator = 3;
				endDenominator = 12;
			}
			else if (taskID.Equals("task2.6.setA")){
				startNumerator = 3;
				startDenominator = 4;
				endNumerator = 1;
				endDenominator = 12;
			}
			else if (taskID.Equals("task2.6.setB")){
				startNumerator = 2;
				startDenominator = 5;
				endNumerator = 1;
				endDenominator = 10;
			}
			else if (taskID.Equals("task2.6.setC")){
				startNumerator = 7;
				startDenominator = 3;
				endNumerator = 1;
				endDenominator = 21;
			}
			else if (taskID.Equals("task2.4.setA.area")){
				startNumerator = 1;
				startDenominator = 2;
			}
			else if (taskID.Equals("task2.4.setB.area")){
				startNumerator = 3;
				startDenominator = 4;
			}
			else if (taskID.Equals("task2.4.setC.area")){
				startNumerator = 7;
				startDenominator = 3;
			}
			else if (taskID.Equals("task2.4.setA.numb")){
				startNumerator = 1;
				startDenominator = 2;
				representation = "number line";
			}
			else if (taskID.Equals("task2.4.setB.numb")){
				startNumerator = 3;
				startDenominator = 4;
				representation = "number line";
			}
			else if (taskID.Equals("task2.4.setC.numb")){
				startNumerator = 7;
				startDenominator = 3;
				representation = "number line";
			}
			else if (taskID.Equals("task2.4.setA.sets")){
				startNumerator = 1;
				startDenominator = 2;
				representation = "sets";
			}
			else if (taskID.Equals("task2.4.setB.sets")){
				startNumerator = 3;
				startDenominator = 4;
				representation = "sets";
			}
			else if (taskID.Equals("task2.4.setC.sets")){
				startNumerator = 7;
				startDenominator = 3;
				representation = "sets";
			}
			else if (taskID.Equals("task2.4.setA.liqu")){
				startNumerator = 1;
				startDenominator = 2;
				representation = "liquid measures";
			}
			else if (taskID.Equals("task2.4.setB.liqu")){
				startNumerator = 3;
				startDenominator = 4;
				representation = "liquid measures";
			}
			else if (taskID.Equals("task2.4.setC.liqu")){
				startNumerator = 7;
				startDenominator = 3;
				representation = "liquid measures";
			}
			else if (taskID.Equals("task3aPlus.1.setA.area")){
				startNumerator = 3;
				startDenominator = 5;
				representation = "area";
			}
			else if (taskID.Equals("task3aPlus.1.setB.area")){
				startNumerator = 4;
				startDenominator = 7;
				representation = "area";
			}
			else if (taskID.Equals("task3aPlus.1.setC.area")){
				startNumerator = 12;
				startDenominator = 9;
				representation = "number line";
			}
			else if (taskID.Equals("task3aPlus.1.setA.numb")){
				startNumerator = 3;
				startDenominator = 5;
				representation = "number line";
			}
			else if (taskID.Equals("task3aPlus.1.setB.numb")){
				startNumerator = 4;
				startDenominator = 7;
				representation = "number line";
			}
			else if (taskID.Equals("task3aPlus.1.setC.numb")){
				startNumerator = 12;
				startDenominator = 9;
				representation = "number line";
			}
			else if (taskID.Equals("task3aPlus.1.setA.sets")){
				startNumerator = 3;
				startDenominator = 5;
				representation = "sets";
			}
			else if (taskID.Equals("task3aPlus.1.setB.sets")){
				startNumerator = 4;
				startDenominator = 7;
				representation = "sets";
			}
			else if (taskID.Equals("task3aPlus.1.setC.sets")){
				startNumerator = 12;
				startDenominator = 9;
				representation = "sets";
			}
			else if (taskID.Equals("task3aPlus.1.setA.liqu")){
				startNumerator = 3;
				startDenominator = 5;
				representation = "liquid measures";
			}
			else if (taskID.Equals("task3aPlus.1.setB.liqu")){
				startNumerator = 4;
				startDenominator = 7;
				representation = "liquid measures";
			}
			else if (taskID.Equals("task3aPlus.1.setC.liqu")){
				startNumerator = 12;
				startDenominator = 9;
				representation = "liquid measures";
			}

			S1 = new FeedbackElem ();
			S1.setID("S1");
			FeedbackMessage S1M = new FeedbackMessage ();
			S1M.setSocratic ("How are you going to tackle this task?");
			S1M.setDidacticConceptual ("Read the task again, and explain how you are going to tackle it.");
			S1.setFeedbackMessage (S1M);
			S1.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepS1 = new Fraction ();
			nextStepS1.setAnyValue (true);
			S1.setNextStep (nextStepS1);

			S2 = new FeedbackElem ();
			S2.setID ("S2");
			FeedbackMessage S2M = new FeedbackMessage ();
			S2M.setSocratic ("What do you need to do in this task?");
			S2M.setGuidance ("You can click one of the buttons on the representations toolbox to create a fraction.");
			S2M.setDidacticConceptual ("Read the task again, and explain how you are going to tackle it.");
			S2M.setHighlighting(Highlighting.RepresentationToolBox);
			S2.setFeedbackMessage (S2M);
			S2.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepS2 = new Fraction ();
			nextStepS2.setAnyValue (true);
			S2.setNextStep (nextStepS2);

			S3 = new FeedbackElem ();
			S3.setID ("S3");
			FeedbackMessage S3M = new FeedbackMessage ();
			S3M.setSocratic ("Good. What do you need to do now, to change the fraction?");
			S3M.setGuidance ("You can use the arrow buttons to change the fraction.");
			S3M.setDidacticConceptual ("Now click the up arrow next to the empty fraction, to make the denominator.");
			S3M.setDidacticProcedural ("Click the up arrow next to the empty fraction, to make the denominator (the bottom part of the fraction) "+endDenominator+".");
			S3M.setHighlighting(Highlighting.ArrowButtons);
			S3.setFeedbackMessage (S3M);
			S3.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepS3 = new Fraction ();
			nextStepS3.setAnyValue (true);
			S3.setNextStep (nextStepS3);

			M1 = new FeedbackElem ();
			M1.setID ("M1");
			FeedbackMessage M1M = new FeedbackMessage ();
			M1M.setSocratic ("Is the denominator in your fraction correct?");
			M1M.setGuidance ("You can click the up arrow next to your fraction to change the denominator.");
			M1M.setDidacticConceptual ("Check that the denominator in your fraction is correct.");
			M1M.setDidacticProcedural ("Check that the denominator (the bottom part of your fraction) is "+endDenominator+".");
			M1M.setHighlighting(Highlighting.ArrowButtons);
			M1.setFeedbackMessage (M1M);
			M1.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepM1 = new Fraction ();
			nextStepM1.setDenominator(endDenominator);
			M1.setNextStep (nextStepM1);

			M2 = new FeedbackElem ();
			M2.setID ("M2");
			FeedbackMessage M2M = new FeedbackMessage ();
			M2M.setSocratic ("Have you changed the numerator or the denominator?");
			M2M.setGuidance ("Remember that the denominator is the bottom part of the fraction.");
			M2M.setDidacticConceptual ("Check that you have changed the denominator, not the numerator.");
			M2M.setDidacticProcedural ("Check that the denominator in your fraction, not the numerator, is "+endDenominator+".");
			M2.setFeedbackMessage (M2M);
			M2.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepM2 = new Fraction ();
			nextStepM2.setDenominator(endDenominator);
			M2.setNextStep (nextStepM2);

			CM2 = new FeedbackElem ();
			CM2.setID ("CM2");
			FeedbackMessage CM2M = new FeedbackMessage ();
			CM2M.setSocratic ("Have you changed the numerator or the denominator?");
			CM2M.setGuidance ("Remember that the denominator is the bottom part of the fraction.");
			CM2M.setDidacticConceptual ("Check that you have changed the denominator, not the numerator.");
			CM2M.setDidacticProcedural ("Check that the denominator in your fraction, not the numerator, is "+5+".");
			CM2.setFeedbackMessage (CM2M);
			CM2.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepCM2 = new Fraction ();
			nextStepCM2.setDenominator(5);
			CM2.setNextStep (nextStepCM2);

			M3 = new FeedbackElem ();
			M3.setID ("M3");
			FeedbackMessage M3M = new FeedbackMessage ();
			M3M.setSocratic ("Is this the fraction you were planning to make?");
			M3M.setGuidance ("Please read the task again carefully.");
			M3M.setDidacticConceptual ("Re-read the task then echeck your fraction.");
			M3.setFeedbackMessage (M3M);
			M3.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepM3 = new Fraction ();
			nextStepM3.setNumerator(endNumerator);
			nextStepM3.setDenominator(endDenominator);
			M3.setNextStep (nextStepM2);

			M4 = new FeedbackElem ();
			M4.setID ("M4");
			FeedbackMessage M4M = new FeedbackMessage ();
			M4M.setDidacticConceptual ("Excellent. Please explain what the numerator and denominator are.");
			M4.setFeedbackMessage (M4M);
			M4.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepM4 = new Fraction ();
			nextStepM4.setSpeech (true);	
			M4.setNextStep (nextStepM2);

			M5 = new FeedbackElem ();
			M5.setID ("M5");
			FeedbackMessage M5M = new FeedbackMessage ();
			M5M.setSocratic ("Have you changed the denominator or the numerator?");
			M5M.setGuidance ("The denominator is the bottom part of the fraction.");
			M5M.setDidacticConceptual ("You changed the numerator. You need to change the denominator.");
			M5M.setDidacticProcedural ("You changed the numerator. You need to change the denominator to "+endDenominator+".");
			M5.setFeedbackMessage (M5M);
			M5.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepM5 = new Fraction ();
			nextStepM5.setDenominator(endDenominator);
			M5.setNextStep (nextStepM2);

			CM5 = new FeedbackElem ();
			CM5.setID ("CM5");
			FeedbackMessage CM5M = new FeedbackMessage ();
			CM5M.setSocratic ("Have you changed the denominator or the numerator?");
			CM5M.setGuidance ("The denominator is the bottom part of the fraction.");
			CM5M.setDidacticConceptual ("You changed the numerator. You need to change the denominator.");
			CM5M.setDidacticProcedural ("You changed the numerator. You need to change the denominator to "+5+".");
			CM5.setFeedbackMessage (M5M);
			CM5.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepCM5 = new Fraction ();
			nextStepCM5.setDenominator(5);
			CM5.setNextStep (nextStepCM2);

			M6 = new FeedbackElem ();
			M6.setID ("M6");
			FeedbackMessage M6M = new FeedbackMessage ();
			M6M.setSocratic ("Excellent. Now, how are you going to change the numerator?");
			M6M.setGuidance ("If you click near the top of your fraction, and click the arrow, you can change the numerator (the top part of the fraction).");
			M6M.setDidacticConceptual ("You changed the denominator. Now, change the numerator.");
			M6M.setDidacticProcedural ("Now, change the numerator. Remember, you need to make the fraction equivalent to "+startNumerator+"/"+startDenominator+".");
			M6M.setHighlighting (Highlighting.ArrowButtons);
			M6.setFeedbackMessage (M6M);
			M6.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepM6 = new Fraction ();
			nextStepM6.setNumerator(endNumerator);
			M6.setNextStep (nextStepM6);


			CM6 = new FeedbackElem ();
			CM6.setID ("CM6");
			FeedbackMessage CM6M = new FeedbackMessage ();
			CM6M.setSocratic ("Excellent. Now, how are you going to compare the fraction?");
			CM6M.setGuidance ("Now that you have made the first fraction, you need to compare it with the second fraction.");
			CM6M.setDidacticConceptual ("You now need to compare the fraction with a second fraction.");
			CM6M.setDidacticProcedural ("Now, create a second fraction using the same representation.");
			CM6M.setHighlighting (Highlighting.ArrowButtons);
			CM6.setFeedbackMessage (CM6M);
			CM6.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepCM6 = new Fraction ();
			nextStepCM6.setNumerator(1);
			nextStepCM6.setDenominator(3);
			nextStepCM6.sameRepresentation (true);
			CM6.setNextStep (nextStepCM6);

			CM6Second = new FeedbackElem ();
			CM6Second.setID ("CM6Second");
			FeedbackMessage CM6MSecond = new FeedbackMessage ();
			CM6MSecond.setSocratic ("Excellent. Now, how are you going to compare the fraction?");
			CM6MSecond.setGuidance ("Now that you have made the first fraction, you need to compare it with the second fraction.");
			CM6MSecond.setDidacticConceptual ("You now need to compare the fraction with a second fraction.");
			CM6MSecond.setDidacticProcedural ("Now, create a second fraction using the same representation.");
			CM6MSecond.setHighlighting (Highlighting.ArrowButtons);
			CM6Second.setFeedbackMessage (CM6MSecond);
			CM6Second.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepCM6Second = new Fraction ();
			nextStepCM6Second.setNumerator(1);
			nextStepCM6Second.setDenominator(5);
			nextStepCM6Second.sameRepresentation (true);
			CM6Second.setNextStep (nextStepCM6Second);

			M7 = new FeedbackElem ();
			M7.setID ("M7");
			FeedbackMessage M7M = new FeedbackMessage ();
			M7M.setSocratic ("Excellent. How about copying this and using the partition tool to make the equivalent fraction?");
			M7M.setGuidance ("You could now copy the fraction and use the partition tool to make an equivalent fraction. To open the partition tool, right click the fraction.");
			M7M.setDidacticConceptual ("Excellent. Now copy this fraction and use the partition tool to change it.");
			M7M.setDidacticProcedural ("Excellent. Now copy the fraction use the partition tool to change it to a fraction with a denominator of "+endDenominator+".");
			M7.setFeedbackMessage (M7M);
			M7.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepM7 = new Fraction ();
			nextStepM7.setDenominator(endDenominator);
			M7.setNextStep (nextStepM7);

			CM7 = new FeedbackElem ();
			CM7.setID ("CM7");
			FeedbackMessage CM7M = new FeedbackMessage ();
			CM7M.setSocratic ("Why have you decided to use a different representation? What does the task ask you to do?");
			CM7M.setGuidance ("Please use the same representation for the second fraction.");
			CM7M.setDidacticProcedural ("Please create the second fraction using the same representation that you used for the first fraction.");
			CM7.setFeedbackMessage (CM7M);
			CM7.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepCM7 = new Fraction ();
			nextStepCM7.sameRepresentation (true);
			CM7.setNextStep (nextStepCM7);

			M8 = new FeedbackElem ();
			M8.setID ("M8");
			FeedbackMessage M8M = new FeedbackMessage ();
			M8M.setSocratic ("How could you compare your fraction with "+startNumerator+"/"+startDenominator+"?");
			M8M.setGuidance ("Think about the denominators in the two fractions. What is the relationship between them? What do you need to do to "+startNumerator+" to work out the correct numerator for your fraction?");
			M8M.setDidacticConceptual ("Compare your fraction with "+startNumerator+"/"+startDenominator+" by using the compairson box.");
			M8M.setHighlighting (Highlighting.ComparisonBox);
			M8.setFeedbackMessage (M8M);
			M8.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepM8 = new Fraction ();
			nextStepM8.setComparison (true);
			M8.setNextStep (nextStepM8);

			CM8 = new FeedbackElem ();
			CM8.setID ("CM8");
			FeedbackMessage CM8M = new FeedbackMessage ();
			CM8M.setSocratic ("Excellent. How using the comparison tool to compare the two fractions?");
			CM8M.setGuidance ("You could now compare the fractions. Open the comparison tool at the top of the screen.");
			CM8M.setDidacticConceptual ("Excellent. Now use the comparison tool.");
			CM8M.setDidacticProcedural ("Excellent. Not open the comparison tool, at the top of the screen, and drag in the two fractions.");
			CM8M.setHighlighting (Highlighting.ComparisonBox);
			CM8.setFeedbackMessage (CM8M);
			CM8.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepCM8 = new Fraction ();
			nextStepCM8.setComparison (true);
			CM8.setNextStep (nextStepCM8);

			M9 = new FeedbackElem ();
			M9.setID ("M9");
			FeedbackMessage M9M = new FeedbackMessage ();
			M9M.setSocratic ("What are you going to compare your fraction with?");
			M9M.setGuidance ("In equivalent fractions, the two numerators must be the same multiple of each other as the two denominators.");
			M9M.setDidacticConceptual ("First create another fraction, this time "+startNumerator+"/"+startDenominator+". Then compare your two fractions.");
			M9M.setHighlighting (Highlighting.RepresentationToolBox);
			M9.setFeedbackMessage (M9M);
			M9.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepM9 = new Fraction ();
			nextStepM9.setNumerator (startNumerator);
			nextStepM9.setDenominator (startDenominator);
			nextStepM9.setComparison (true);
			M9.setNextStep (nextStepM9);

			M10 = new FeedbackElem ();
			M10.setID ("M10");
			FeedbackMessage M10M = new FeedbackMessage ();
			M10M.setDidacticConceptual ("Excellent. Please explain why you made the denominator "+endDenominator+".");
			M10.setFeedbackMessage (M10M);
			M10.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepM10 = new Fraction ();
			nextStepM10.setSpeech (true);
			M10.setNextStep (nextStepM10);

			M11 = new FeedbackElem ();
			M11.setID ("M11");
			FeedbackMessage M11M = new FeedbackMessage ();
			M11M.setSocratic ("How can you check, using a Fractions Lab tool, that your solution is correct?");
			M11M.setGuidance ("You could use the comparison box to compare your fractions.");
			M11M.setDidacticConceptual ("Compare the two fractions using the comparison box.");
			M11M.setHighlighting (Highlighting.ComparisonBox);
			M11.setFeedbackMessage (M11M);
			M11.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepM11 = new Fraction ();
			nextStepM11.setComparison (true);
			M11.setNextStep (nextStepM11);

			CM11 = new FeedbackElem ();
			CM11.setID ("CM11");
			FeedbackMessage CM11M = new FeedbackMessage ();
			CM11M.setDidacticConceptual ("Excellent. Please explain why you made the denominator 3.");
			CM11.setFeedbackMessage (CM11M);
			CM11.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepCM11 = new Fraction ();
			nextStepCM11.setSpeech (true);
			CM11.setNextStep (nextStepCM11);

			CM12 = new FeedbackElem ();
			CM12.setID ("CM12");
			FeedbackMessage CM12M = new FeedbackMessage ();
			CM12M.setSocratic ("Excellent, what are you going to do now?");
			CM12.setFeedbackMessage (CM12M);
			CM12.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepCM12 = new Fraction ();
			nextStepCM12.setSpeech (true);
			CM12.setNextStep (nextStepCM12);

			E1 = new FeedbackElem ();
			E1.setID ("E1");
			FeedbackMessage E1M = new FeedbackMessage ();
			E1M.setDidacticConceptual ("The way that you worked that out was excellent. Well done.");
			E1.setFeedbackMessage (E1M);
			E1.setFeedbackType (FeedbackType.affirmation);

			E2 = new FeedbackElem ();
			E2.setID ("E2");
			FeedbackMessage E2M = new FeedbackMessage ();
			E2M.setDidacticConceptual ("Please explain what you did to the numerator and the denominator of "+startNumerator+"/"+startDenominator+" to make an equivalent fraction with "+endDenominator+" as the denominator.");
			E2.setFeedbackMessage (E2M);
			Fraction nextStepE2 = new Fraction ();
			nextStepE2.setSpeech (true);
			E2.setNextStep (nextStepE2);
			E2.setFeedbackType (FeedbackType.reflection);

			CE2 = new FeedbackElem ();
			CE2.setID ("CE2");
			FeedbackMessage CE2M = new FeedbackMessage ();
			CE2M.setDidacticConceptual ("Please explain what you found by comparing 1/3 and 1/5.");
			CE2.setFeedbackMessage (CE2M);
			Fraction nextStepCE2 = new Fraction ();
			nextStepCE2.setSpeech (true);
			CE2.setNextStep (nextStepCE2);
			CE2.setFeedbackType (FeedbackType.reflection);

			R1 = new FeedbackElem ();
			R1.setID ("R1");
			FeedbackMessage R1M = new FeedbackMessage ();
			R1M.setDidacticConceptual ("What has happened to the numerator and denominator? Have they been affected the same or differently?");
			R1.setFeedbackMessage (R1M);
			Fraction nextStepR1 = new Fraction ();
			nextStepR1.setSpeech (true);
			R1.setNextStep (nextStepR1);
			R1.setFeedbackType (FeedbackType.reflection);

			R2 = new FeedbackElem ();
			R2.setID ("R2");
			FeedbackMessage R2M = new FeedbackMessage ();
			R2M.setDidacticConceptual ("Why did you make the fraction "+endNumerator+"/"+endDenominator+"? What did you do to the numerator and denominator of "+startNumerator+"/"+startDenominator+"?");
			R2.setFeedbackMessage (R2M);
			Fraction nextStepR2 = new Fraction ();
			nextStepR2.setSpeech (true);
			R2.setNextStep (nextStepR2);
			R2.setFeedbackType (FeedbackType.reflection);

			O1 = new FeedbackElem ();
			O1.setID ("O1");
			FeedbackMessage O1M = new FeedbackMessage ();
			O1M.setDidacticConceptual ("You don't appear to have completed the task.");
			O1.setFeedbackMessage (O1M);
			Fraction nextStepO1 = new Fraction ();
			nextStepO1.setAnyValue (true);
			O1.setNextStep (nextStepO1);
			O1.setFeedbackType (FeedbackType.taskNotFinished);

			O2 = new FeedbackElem ();
			O2.setID ("O2");
			FeedbackMessage O2M = new FeedbackMessage ();
			O2M.setDidacticConceptual ("If you need more help to finish the task, you could ask your teacher.");
			O2.setFeedbackMessage (O2M);
			Fraction nextStepO2 = new Fraction ();
			nextStepO2.setAnyValue (true);
			O2.setNextStep (nextStepO2);
			O2.setFeedbackType (FeedbackType.taskNotFinished);

			FM6 = new FeedbackElem ();
			FM6.setID ("FM6");
			FeedbackMessage MF6M = new FeedbackMessage ();
			MF6M.setSocratic ("Excellent. Now, how are you going to use a different representation?");
			MF6M.setGuidance ("Excellent, Now, click the representation toolbar and create another representation of exactly the same fraction.");
			FM6.setFeedbackMessage (MF6M);
			FM6.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepFM6 = new Fraction ();
			nextStepFM6.differntRepresentation (true);
			nextStepFM6.allSameValue(true);
			FM6.setNextStep (nextStepFM6);

			FM10 = new FeedbackElem ();
			FM10.setID ("FM10");
			FeedbackMessage MF10M = new FeedbackMessage ();
			MF10M.setSocratic ("Excellent. How are your representations similar, and how are they different?");
			FM10.setFeedbackMessage (MF10M);
			FM10.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepFM10 = new Fraction ();
			nextStepFM10.setSpeech (true);
			FM10.setNextStep (nextStepFM10);

			FM11 = new FeedbackElem ();
			FM11.setID ("FM11");
			FeedbackMessage MF11M = new FeedbackMessage ();
			MF11M.setSocratic ("Excellent! Have you completed the task?");
			MF11M.setGuidance ("Excellent! Keep going. Now make your fraction using a different representation.");
			FM11.setFeedbackMessage (MF11M);
			FM11.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepFM11 = new Fraction ();
			nextStepFM11.differntRepresentation (true);
			nextStepFM11.allSameValue(true);
			FM11.setNextStep (nextStepFM11);

			FE1 = new FeedbackElem ();
			FE1.setID("FE1");
			FeedbackMessage MFE1M = new FeedbackMessage ();
			MFE1M.setDidacticConceptual ("Excellent, you have made your fraction using all the representations. Well done.");
			FE1.setFeedbackMessage (MFE1M);
			FE1.setFeedbackType (FeedbackType.affirmation);

			FE2 = new FeedbackElem ();
			FE2.setID("FE2");
			FeedbackMessage MFE2M = new FeedbackMessage ();
			MFE2M.setDidacticConceptual ("How are all your representations similar, and how are they different?");
			FE2.setFeedbackMessage (MFE2M);
			FE2.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepFE2 = new Fraction ();
			nextStepFE2.setSpeech (true);
			FE2.setNextStep (nextStepFE2);

			F2M1 = new FeedbackElem ();
			F2M1.setID ("F2M1");
			FeedbackMessage F2M1M = new FeedbackMessage ();
			F2M1M.setSocratic ("Good. What do you need to do now, to complete the fraction?");
			F2M1M.setGuidance ("You can use the arrow buttons at the top of the fraction to complete your fraction.");
			F2M1M.setDidacticConceptual ("Now click the up arrow next to the top of the fraction, to make the numerator.");
			F2M1M.setDidacticProcedural ("Click the up arrow next to the top of the fraction, to make the numerator (the top part of the fraction).");
			F2M1.setFeedbackMessage (F2M1M);
			F2M1.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepF2M1 = new Fraction ();
			nextStepF2M1.setNumeratorAnyValue (true);
			F2M1.setNextStep (nextStepF2M1);

			F2M4 = new FeedbackElem ();
			F2M4.setID ("F2M4");
			FeedbackMessage F2M4M = new FeedbackMessage ();
			F2M4M.setSocratic ("“Excellent. What fraction have you made?");
			F2M4.setFeedbackMessage (F2M4M);
			F2M4.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepF2M4 = new Fraction ();
			nextStepF2M4.setSpeech(true);
			F2M4.setNextStep (nextStepF2M4);

			F2M6 = new FeedbackElem ();
			F2M6.setID ("F2M6");
			FeedbackMessage F2M6M = new FeedbackMessage ();
			F2M6M.setSocratic ("Excellent. Now, how are you going to partition the fraction?");
			F2M6M.setGuidance ("Right click the fraction, select ‘find equivalent’, and partition the fraction into two.");
			F2M6M.setDidacticConceptual ("Click the fraction with the right-hand mouse button, then select ‘find equivalent’ and partition the fraction into two.");
			F2M6M.setDidacticProcedural ("Click the fraction with the right-hand mouse button. Then click ‘find equivalent’. Then click to partition the fraction into two.");
			F2M6.setFeedbackMessage (F2M6M);
			F2M6.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepF2M6 = new Fraction ();
			nextStepF2M6.setPartition(2);
			F2M6.setNextStep (nextStepF2M6);

			F2M7 = new FeedbackElem ();
			F2M7.setID ("F2M7");
			FeedbackMessage F2M7M = new FeedbackMessage ();
			F2M7M.setSocratic ("Excellent. Now, how are you going to partition the fraction into 3?");
			F2M7M.setGuidance ("Right click the fraction, select ‘find equivalent’, and partition the fraction into 3.");
			F2M7M.setDidacticConceptual ("Click the fraction with the right-hand mouse button, then select ‘find equivalent’, and partition the fraction into 3.");
			F2M7M.setDidacticProcedural ("Click the fraction with the right-hand mouse button. Then click ‘find equivalent’. Then click to partition the fraction into 3.");
			F2M7.setFeedbackMessage (F2M7M);
			F2M7.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepF2M7 = new Fraction ();
			nextStepF2M7.setPartition(3);
			F2M7.setNextStep (nextStepF2M7);

			F2M7b = new FeedbackElem ();
			F2M7b.setID ("F2M7b");
			FeedbackMessage F2M7bM = new FeedbackMessage ();
			F2M7bM.setSocratic ("Excellent. Now, how are you going to partition the fraction into 4?");
			F2M7bM.setGuidance ("Right click the fraction, select ‘find equivalent’, and partition the fraction into 4.");
			F2M7bM.setDidacticConceptual ("Click the fraction with the right-hand mouse button, then select ‘find equivalent’, and partition the fraction into 4.");
			F2M7bM.setDidacticProcedural ("Click the fraction with the right-hand mouse button. Then click ‘find equivalent’. Then click to partition the fraction into 4.");
			F2M7b.setFeedbackMessage (F2M7bM);
			F2M7b.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepF2M7b = new Fraction ();
			nextStepF2M7b.setPartition(4);
			F2M7b.setNextStep (nextStepF2M7b);

			F2M7c = new FeedbackElem ();
			F2M7c.setID ("F2M7c");
			FeedbackMessage F2M7cM = new FeedbackMessage ();
			F2M7cM.setSocratic ("Excellent. Now, how are you going to partition the fraction into 5?");
			F2M7cM.setGuidance ("Right click the fraction, select ‘find equivalent’, and partition the fraction into 5.");
			F2M7cM.setDidacticConceptual ("Click the fraction with the right-hand mouse button, then select ‘find equivalent’, and partition the fraction into 5.");
			F2M7cM.setDidacticProcedural ("Click the fraction with the right-hand mouse button and click the new arrow to partition the fraction into 5.");
			F2M7c.setFeedbackMessage (F2M7cM);
			F2M7c.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepF2M7c = new Fraction ();
			nextStepF2M7c.setPartition(5);
			F2M7c.setNextStep (nextStepF2M7c);

			F2M10 = new FeedbackElem ();
			F2M10.setID ("F2M10");
			FeedbackMessage F2M10M = new FeedbackMessage ();
			F2M10M.setSocratic ("Excellent. Is your new fraction equivalent to your original fraction? What has happened to the denominator and what has happened to the numerator?");
			F2M10.setFeedbackMessage (F2M10M);
			F2M10.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepF2M10 = new Fraction ();
			nextStepF2M10.setSpeech(true);
			F2M10.setNextStep (nextStepF2M10);

			F2M11 = new FeedbackElem ();
			F2M11.setID ("F2M11");
			FeedbackMessage F2M11M = new FeedbackMessage ();
			F2M11M.setSocratic ("Excellent! Have you completed the task?");
			F2M11M.setGuidance ("Keep going. Now partition your fraction further.");
			F2M11.setFeedbackMessage (F2M11M);
			F2M11.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepF2M11 = new Fraction ();
			nextStepF2M11.setPartition(5);
			F2M11.setNextStep (nextStepF2M11);

			F2E1 = new FeedbackElem ();
			F2E1.setID("F2E1");
			FeedbackMessage MF2E1M = new FeedbackMessage ();
			MF2E1M.setDidacticConceptual ("Excellent, you have partitioned your fraction 2, 3, 4 and 5 times. Well done.");
			F2E1.setFeedbackMessage (MF2E1M);
			F2E1.setFeedbackType (FeedbackType.affirmation);
			
			F2E2 = new FeedbackElem ();
			F2E2.setID("F2E2");
			FeedbackMessage MF2E2M = new FeedbackMessage ();
			MF2E2M.setDidacticConceptual ("When you used find equivalent, what happened to the denominators and numerators?");
			F2E2.setFeedbackMessage (MF2E2M);
			F2E2.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepF2E2 = new Fraction ();
			nextStepF2E2.setSpeech (true);
			F2E2.setNextStep (nextStepF2E2);


			T24M1 = new FeedbackElem ();
			T24M1.setID ("T24M1");
			FeedbackMessage T24M1M = new FeedbackMessage ();
			T24M1M.setSocratic ("Is the denominator in your fraction correct?");
			T24M1M.setGuidance ("You can click the up arrow next to your fraction to change it.");
			T24M1M.setDidacticConceptual ("Check that the denominator in your fraction is correct.");
			T24M1M.setDidacticProcedural ("Check that the denominator (the bottom part of your fraction) is "+startDenominator+".");
			T24M1M.setHighlighting (Highlighting.ArrowButtons);
			T24M1.setFeedbackMessage (T24M1M);
			T24M1.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT24M1 = new Fraction ();
			nextStepT24M1.setDenominator(startDenominator);
			T24M1.setNextStep (nextStepT24M1);

			T24M2 = new FeedbackElem ();
			T24M2.setID ("T24M2");
			FeedbackMessage T24M2M = new FeedbackMessage ();
			T24M2M.setSocratic ("Have you changed the numerator or denominator?");
			T24M2M.setGuidance ("Remember that the denominator is the bottom part of the fraction.");
			T24M2M.setDidacticConceptual ("Check that you have changed the denominator, not the numerator.");
			T24M2M.setDidacticProcedural ("Check that the denominator in your fraction, not the numerator, is "+startDenominator+".");
			T24M2.setFeedbackMessage (T24M2M);
			T24M2.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT24M2 = new Fraction ();
			nextStepT24M2.setDenominator(startDenominator);
			T24M2.setNextStep (nextStepT24M2);

			T24M3 = new FeedbackElem ();
			T24M3.setID ("T24M3");
			FeedbackMessage T24M3M = new FeedbackMessage ();
			T24M3M.setSocratic ("Is this the fraction you were planning to make?");
			T24M3M.setGuidance ("Please read the task again carefully.");
			T24M3M.setDidacticConceptual ("Re-read the task then check your fraction.");
			T24M3.setFeedbackMessage (T24M3M);
			T24M3.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT24M3 = new Fraction ();
			nextStepT24M3.setNumerator(startNumerator);
			nextStepT24M3.setDenominator(startDenominator);
			T24M3.setNextStep (nextStepT24M3);

			T24M4 = new FeedbackElem ();
			T24M4.setID ("T24M4");
			FeedbackMessage T24M4M = new FeedbackMessage ();
			T24M4M.setDidacticConceptual ("Excellent. Please explain what the 'numerator' and `denominator' are");
			T24M4.setFeedbackMessage (T24M4M);
			T24M4.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepT24M4 = new Fraction ();
			nextStepT24M4.setSpeech (true);
			T24M4.setNextStep (nextStepT24M4);

			T24M5 = new FeedbackElem ();
			T24M5.setID ("T24M5");
			FeedbackMessage T24M5M = new FeedbackMessage ();
			T24M5M.setSocratic ("Have you changed the denominator or the numerator?");
			T24M5M.setGuidance ("The denominator is the bottom part of the fraction.");
			T24M5M.setDidacticConceptual ("You changed the numerator. You need to change the denominator.");
			T24M5M.setDidacticProcedural ("You changed the numerator. You need to change the denominator to "+startDenominator+".");
			T24M5.setFeedbackMessage (T24M5M);
			T24M5.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT24M5 = new Fraction ();
			nextStepT24M5.setDenominator(startDenominator);
			T24M5.setNextStep (nextStepT24M5);

			T24M6 = new FeedbackElem ();
			T24M6.setID ("T24M6");
			FeedbackMessage T24M6M = new FeedbackMessage ();
			T24M6M.setSocratic ("Excellent. Now, how are you going to change the numerator?");
			T24M6M.setGuidance ("If you click near the top of your fraction, and click the up arrow, you can change the numerator (the top part of the fraction).");
			T24M6M.setDidacticConceptual ("You changed the denominator. Now, change the numerator.");
			T24M6M.setDidacticProcedural ("Now, change the numerator. Remember, you need to make a fraction equivalent to "+startNumerator+"/"+startDenominator+".");
			T24M6M.setHighlighting (Highlighting.ArrowButtons);
			T24M6.setFeedbackMessage (T24M6M);
			T24M6.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepT24M6 = new Fraction ();
			nextStepT24M6.setNumerator(startNumerator);
			nextStepT24M6.setDenominator(startDenominator);
			nextStepT24M6.setEquivalentFraction (true);
			T24M6.setNextStep (nextStepT24M6);

			T24M7 = new FeedbackElem ();
			T24M7.setID ("T24M7");
			FeedbackMessage T24M7M = new FeedbackMessage ();
			T24M7M.setSocratic ("Excellent. How about copying this and using the partition tool to make the equivalent fraction?");
			T24M7M.setGuidance ("You could now copy the fraction and use the partition tool to make an equivalent fraction. To open the partition tool, right-click the fraction.");
			T24M7M.setDidacticConceptual ("Excellent. Now copy this fraction and use the partition tool to change it.");
			T24M7M.setDidacticProcedural ("Excellent. Now copy the fraction use the partition tool to change it to an equivalent fraction.");
			T24M7.setFeedbackMessage (T24M7M);
			T24M7.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepT24M7 = new Fraction ();
			nextStepT24M7.setPartitionBool (true);
			T24M7.setNextStep (nextStepT24M7);

			T24M8 = new FeedbackElem ();
			T24M8.setID ("T24M8");
			FeedbackMessage T24M8M = new FeedbackMessage ();
			T24M8M.setSocratic ("How could you compare your fraction with "+startNumerator+"/"+startDenominator+".");
			T24M8M.setGuidance ("Think about the denominators in your fraction and in "+startNumerator+"/"+startDenominator+". What is the relationship between them? What do you need to do to "+startNumerator+" to work out the correct numerator for your fraction?");
			T24M8M.setDidacticConceptual ("Compare your fraction with "+startNumerator+"/"+startDenominator+" by using the comparison box.");
			T24M8M.setHighlighting (Highlighting.ComparisonBox);
			T24M8.setFeedbackMessage (T24M8M);
			T24M8.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT24M8 = new Fraction ();
			nextStepT24M8.setComparison (true);
			T24M8.setNextStep (nextStepT24M8);

			T24M9 = new FeedbackElem ();
			T24M9.setID ("T24M9");
			FeedbackMessage T24M9M = new FeedbackMessage ();
			T24M9M.setSocratic ("What are you going to compare your fraction with?");
			T24M9M.setGuidance ("In equivalent fractions, the two numerators must be the same multiple of each other as the two denominators.");
			T24M9M.setDidacticConceptual ("First create another fraction, this time "+startNumerator+"/"+startDenominator+". Then compare your two fractions.");
			T24M9M.setHighlighting (Highlighting.RepresentationToolBox);
			T24M9.setFeedbackMessage (T24M9M);
			T24M9.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepT24M9 = new Fraction ();
			nextStepT24M9.setComparison (true);
			T24M9.setNextStep (nextStepT24M9);

			T24M10 = new FeedbackElem ();
			T24M10.setID ("T24M10");
			FeedbackMessage T24M10M = new FeedbackMessage ();
			T24M10M.setDidacticConceptual ("Excellent. Please explain why you made the denominator "+startDenominator+" or a multiple of "+startDenominator+".");
			T24M10.setFeedbackMessage (T24M10M);
			T24M10.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepT24M10 = new Fraction ();
			nextStepT24M10.setSpeech (true);
			T24M10.setNextStep (nextStepT24M10);

			T24M11 = new FeedbackElem ();
			T24M11.setID ("T24M11");
			FeedbackMessage T24M11M = new FeedbackMessage ();
			T24M11M.setSocratic ("How can you check, using a Fractions Lab tool,  that your solution is correct?");
			T24M11M.setGuidance ("You could use the comparison box to compare your fractions.");
			T24M11M.setDidacticConceptual ("Compare the two fractions using the comparison box.");
			T24M11M.setHighlighting (Highlighting.ComparisonBox);
			T24M11.setFeedbackMessage (T24M11M);
			T24M11.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepT24M11 = new Fraction ();
			nextStepT24M11.setComparison (true);
			T24M11.setNextStep (nextStepT24M11);

			T24E1 = new FeedbackElem ();
			T24E1.setID ("T24E1");
			FeedbackMessage T24E1M = new FeedbackMessage ();
			T24E1M.setDidacticConceptual ("The way that you worked that out was excellent. Well done.");
			T24E1.setFeedbackMessage (T24E1M);
			T24E1.setFeedbackType (FeedbackType.affirmation);

			T24E2 = new FeedbackElem ();
			T24E2.setID ("T24E2");
			FeedbackMessage T24E2M = new FeedbackMessage ();
			T24E2M.setDidacticConceptual ("Please explain what you did to the numerator and denominator of "+startNumerator+"/"+startDenominator+" to make an equivalent fraction.");
			T24E2.setFeedbackMessage (T24E2M);
			T24E2.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepT24E2 = new Fraction ();
			nextStepT24E2.setSpeech (true);
			T24E2.setNextStep (nextStepT24E2);


			T26M1 = new FeedbackElem ();
			T26M1.setID ("T26M1");
			FeedbackMessage T26M1M = new FeedbackMessage ();
			T26M1M.setGuidance ("You can click the up arrow next to your fraction to change it.");
			T26M1M.setSocratic ("Is the denominator in your fraction correct?");
			T26M1M.setDidacticConceptual ("Check that the denominator in your fraction is correct.");
			T26M1M.setDidacticProcedural ("Check that the denominator (the bottom part of your fraction) is "+startDenominator+" or "+endDenominator+".");
			T26M1M.setHighlighting (Highlighting.ArrowButtons);
			T26M1.setFeedbackMessage (T26M1M);
			T26M1.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT26M1 = new Fraction ();
			int[] values = new int[2] {startDenominator, endDenominator};
			nextStepT26M1.setDenominators (values);
			T26M1.setNextStep (nextStepT26M1);

			T26M2 = new FeedbackElem ();
			T26M2.setID ("T26M2");
			FeedbackMessage T26M2M = new FeedbackMessage ();
			T26M2M.setGuidance ("Remember that the denominator is the bottom part of the fraction.");
			T26M2M.setSocratic ("Have you changed the numerator or denominator?");
			T26M2M.setDidacticConceptual ("Check that you have changed the denominator, not the numerator.");
			T26M2M.setDidacticProcedural ("Check that the denominator in your fraction, not the numerator, is "+startDenominator+" or "+endDenominator+".");
			T26M2M.setHighlighting (Highlighting.ArrowButtons);
			T26M2.setFeedbackMessage (T26M2M);
			T26M2.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT26M2 = new Fraction ();
			int[] values2 = new int[2] {startDenominator, endDenominator};
			nextStepT26M2.setDenominators (values2);
			T26M2.setNextStep (nextStepT26M2);

			T26M3 = new FeedbackElem ();
			T26M3.setID ("T26M3");
			FeedbackMessage T26M3M = new FeedbackMessage ();
			T26M3M.setGuidance ("Please read the task again carefully.");
			T26M3M.setSocratic ("Is this the fraction you were planning to make?");
			T26M3M.setDidacticConceptual ("Re-read the task then check your fraction.");
			T26M3.setFeedbackMessage (T26M3M);
			T26M3.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT26M3 = new Fraction ();
			int[] values3 = new int[2] {startDenominator, endDenominator};
			int[] values4 = new int[2] {startNumerator, endNumerator};
			nextStepT26M3.setNumerators (values4);
			nextStepT26M3.setDenominators (values3);
			T26M3.setNextStep (nextStepT26M3);

			T26M4 = new FeedbackElem ();
			T26M4.setID ("T26M4");
			FeedbackMessage T26M4M = new FeedbackMessage ();
			T26M4M.setDidacticConceptual ("Excellent. Please explain what the 'numerator' and `denominator' are.");
			T26M4.setFeedbackMessage (T26M4M);
			T26M4.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepT26M4 = new Fraction ();
			nextStepT26M4.setSpeech (true);
			T26M4.setNextStep (nextStepT26M4);

			T26M5 = new FeedbackElem ();
			T26M5.setID ("T26M5");
			FeedbackMessage T26M5M = new FeedbackMessage ();
			T26M5M.setGuidance ("The denominator is the bottom part of the fraction.");
			T26M5M.setSocratic ("Have you changed the denominator or the numerator?");
			T26M5M.setDidacticConceptual ("You changed the numerator. You need to change the denominator.");
			T26M5M.setDidacticProcedural ("You changed the numerator. You need to change the denominator to "+startDenominator+" or "+endDenominator+".");
			T26M5.setFeedbackMessage (T26M5M);
			T26M5.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT26M5 = new Fraction ();
			int[] valuesM5 = new int[2] {startDenominator, endDenominator};
			nextStepT26M5.setDenominators (valuesM5);
			T26M5.setNextStep (nextStepT26M5);

			T26M6 = new FeedbackElem ();
			T26M6.setID ("T26M6");
			FeedbackMessage T26M6M = new FeedbackMessage ();
			T26M6M.setGuidance ("If you click near the top of your fraction, and click the up arrow, you can change the numerator (the top part of the fraction).");
			T26M6M.setSocratic ("Excellent. Now, how are you going to change the numerator?");
			T26M6M.setDidacticConceptual ("You changed the denominator.  Now, change the numerator.");
			T26M6M.setDidacticProcedural ("Now, change the numerator. Remember, you need to make the fraction "+startNumerator+"/"+startDenominator+" or "+endNumerator+"/"+endDenominator+".");
			T26M6M.setHighlighting (Highlighting.ArrowButtons);
			T26M6.setFeedbackMessage (T26M6M);
			T26M6.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepT26M6 = new Fraction ();
			int[] valuesM6num = new int[2] {startNumerator, endNumerator};
			int[] valuesM6den = new int[2] {startDenominator, endDenominator};
			nextStepT26M6.setNumerators (valuesM6num);
			nextStepT26M6.setDenominators (valuesM6den);
			T26M6.setNextStep (nextStepT26M6);

			T26M7 = new FeedbackElem ();
			T26M7.setID ("T26M7");
			FeedbackMessage T26M7M = new FeedbackMessage ();
			T26M7M.setGuidance ("There are two fractions in this task.  You can make another fraction by using the representation tool.");
			T26M7M.setSocratic ("Excellent. How are you going to make the second fraction?");
			T26M7M.setDidacticConceptual ("Please make the other fraction.");
			T26M7M.setDidacticProcedural ("You have made "+startNumerator+"/"+startDenominator+" or "+endNumerator+"/"+endDenominator+".  Please make "+startNumerator+"/"+startDenominator+" or "+endNumerator+"/"+endDenominator+".");
			T26M7M.setHighlighting (Highlighting.ArrowButtons);
			T26M7.setFeedbackMessage (T26M7M);
			T26M7.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepT26M7 = new Fraction ();
			int[] valuesM7num = new int[2] {startNumerator, endNumerator};
			int[] valuesM7den = new int[2] {startDenominator, endDenominator};
			nextStepT26M7.setNumerators (valuesM6num);
			nextStepT26M7.setDenominators (valuesM6den);
			T26M7.setNextStep (nextStepT26M7);

			T26M8 = new FeedbackElem ();
			T26M8.setID ("T26M8");
			FeedbackMessage T26M8M = new FeedbackMessage ();
			T26M8M.setGuidance ("It is easier to compare fractions when they are represented the same way.");
			T26M8M.setSocratic ("Can you think of a clearer way to represent the fractions?");
			T26M8M.setDidacticConceptual ("Please change one of your fractions so they use the same representation.");
			T26M8M.setHighlighting (Highlighting.ComparisonBox);
			T26M8.setFeedbackMessage (T26M8M);
			T26M8.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT26M8 = new Fraction ();
			nextStepT26M8.sameRepresentation (true);
			T26M8.setNextStep (nextStepT26M8);

			T26M10 = new FeedbackElem ();
			T26M10.setID ("T26M10");
			FeedbackMessage T26M10M = new FeedbackMessage ();
			T26M10M.setDidacticConceptual ("Great. Please explain why you made these fractions.");
			T26M10.setFeedbackMessage (T26M10M);
			T26M10.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepT26M10 = new Fraction ();
			nextStepT26M10.setSpeech (true);
			T26M10.setNextStep (nextStepT26M10);

			T26M11 = new FeedbackElem ();
			T26M11.setID ("T26M11");
			FeedbackMessage T26M11M = new FeedbackMessage ();
			T26M11M.setGuidance ("You could use the comparison box to compare your fractions.");
			T26M11M.setSocratic ("How can you check, using a Fractions Lab tool,  that your solution is correct?");
			T26M11M.setDidacticConceptual ("Compare the two fractions using the comparison box.");
			T26M11M.setHighlighting (Highlighting.ComparisonBox);
			T26M11.setFeedbackMessage (T26M11M);
			T26M11.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepT26M11 = new Fraction ();
			nextStepT26M11.setComparison (true);
			T26M11.setNextStep (nextStepT26M11);

			T26E1 = new FeedbackElem ();
			T26E1.setID ("T26E1");
			FeedbackMessage T26E1M = new FeedbackMessage ();
			T26E1M.setDidacticConceptual ("The way that you worked that out was excellent. Well done.");
			T26E1.setFeedbackMessage (T26E1M);
			T26E1.setFeedbackType (FeedbackType.affirmation);

			T26E2 = new FeedbackElem ();
			T26E2.setID ("T26E2");
			FeedbackMessage T26E2M = new FeedbackMessage ();
			T26E2M.setDidacticConceptual ("Please explain why you agree or disagree.");
			T26E2.setFeedbackMessage (T26E2M);
			T26E2.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepT26E2 = new Fraction ();
			nextStepT26E2.setSpeech (true);
			T26E2.setNextStep (nextStepT26E2);

			T3aP1M1 = new FeedbackElem ();
			T3aP1M1.setID ("T3aP1M1");
			FeedbackMessage T3aP1M1M = new FeedbackMessage ();
			T3aP1M1M.setSocratic ("Is the denominator in your fraction correct?");
			T3aP1M1M.setGuidance ("You can click the up arrow next to your fraction to change it.");
			T3aP1M1M.setDidacticConceptual ("Check that the denominator in your fraction is correct.");
			T3aP1M1M.setDidacticProcedural ("Check that the denominator (the bottom part of your fraction) is "+startDenominator+".");
			T3aP1M1M.setHighlighting (Highlighting.ArrowButtons);
			T3aP1M1.setFeedbackMessage (T3aP1M1M);
			T3aP1M1.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT3aP1M1 = new Fraction ();
			nextStepT3aP1M1.setDenominator (startDenominator);
			T3aP1M1.setNextStep (nextStepT3aP1M1);

			T3aP1M2 = new FeedbackElem ();
			T3aP1M2.setID ("T3aP1M2");
			FeedbackMessage T3aP1M2M = new FeedbackMessage ();
			T3aP1M2M.setSocratic ("Have you changed the numerator or denominator?");
			T3aP1M2M.setGuidance ("Remember that the denominator is the bottom part of the fraction.");
			T3aP1M2M.setDidacticConceptual ("Check that you have changed the denominator, not the numerator.");
			T3aP1M2M.setDidacticProcedural ("Check that the denominator in your fraction, not the numerator, is "+startDenominator+".");
			T3aP1M2.setFeedbackMessage (T3aP1M2M);
			T3aP1M2.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT3aP1M2 = new Fraction ();
			nextStepT3aP1M2.setDenominator (startDenominator);
			T3aP1M2.setNextStep (nextStepT3aP1M2);

			T3aP1M3 = new FeedbackElem ();
			T3aP1M3.setID ("T3aP1M3");
			FeedbackMessage T3aP1M3M = new FeedbackMessage ();
			T3aP1M3M.setSocratic ("Is this the fraction you were planning to make?");
			T3aP1M3M.setGuidance ("Please read the task again carefully.");
			T3aP1M3M.setDidacticConceptual ("Re-read the task then check your fraction.");
			T3aP1M3.setFeedbackMessage (T3aP1M3M);
			T3aP1M3.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT3aP1M3 = new Fraction ();
			nextStepT3aP1M3.setFractionForAdditionTask(startDenominator, startNumerator);
			T3aP1M3.setNextStep (nextStepT3aP1M3);

			T3aP1M4 = new FeedbackElem ();
			T3aP1M4.setID ("T3aP1M4");
			FeedbackMessage T3aP1M4M = new FeedbackMessage ();
			T3aP1M4M.setDidacticConceptual ("Excellent. Please explain what the 'numerator' and `denominator' are.");
			T3aP1M4.setFeedbackMessage (T3aP1M4M);
			T3aP1M4.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepT3aP1M4 = new Fraction ();
			nextStepT3aP1M4.setSpeech (true);
			T3aP1M4.setNextStep (nextStepT3aP1M4);

			T3aP1M5 = new FeedbackElem ();
			T3aP1M5.setID ("T3aP1M5");
			FeedbackMessage T3aP1M5M = new FeedbackMessage ();
			T3aP1M5M.setSocratic ("Have you changed the denominator or the numerator?");
			T3aP1M5M.setGuidance ("The denominator is the bottom part of the fraction.");
			T3aP1M5M.setDidacticConceptual ("You changed the numerator. You need to change the denominator.");
			T3aP1M5M.setDidacticProcedural ("You changed the numerator. You need to change the denominator to "+startDenominator+".");
			T3aP1M5.setFeedbackMessage (T3aP1M5M);
			T3aP1M5.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT3aP1M5 = new Fraction ();
			nextStepT3aP1M5.setDenominator (startDenominator);
			T3aP1M5.setNextStep (nextStepT3aP1M5);

			T3aP1M6 = new FeedbackElem ();
			T3aP1M6.setID ("T3aP1M6");
			FeedbackMessage T3aP1M6M = new FeedbackMessage ();
			T3aP1M6M.setSocratic ("Excellent. Now, how are you going to change the numerator?");
			T3aP1M6M.setGuidance ("If you click near the top of your fraction, and click the up arrow, you can change the numerator (the top part of the fraction).");
			T3aP1M6M.setDidacticConceptual ("You changed the denominator.  Now, change the numerator.");
			T3aP1M6M.setDidacticProcedural ("Now, change the numerator. Remember, you need to make two fractions that can be added together to make "+startNumerator+"/"+startDenominator+".");
			T3aP1M6M.setHighlighting (Highlighting.ArrowButtons);
			T3aP1M6.setFeedbackMessage (T3aP1M6M);
			T3aP1M6.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepT3aP1M6 = new Fraction ();
			nextStepT3aP1M6.setFractionForAdditionTask(startDenominator, startNumerator);
			T3aP1M6.setNextStep (nextStepT3aP1M6);

			T3aP1M7 = new FeedbackElem ();
			T3aP1M7.setID ("T3aP1M7");
			FeedbackMessage T3aP1M7M = new FeedbackMessage ();
			T3aP1M7M.setSocratic ("Excellent. You've made one fraction. What do you need to do now, to complete the task?");
			T3aP1M7M.setGuidance ("You now need to make a second fraction that, when added together with your first fraction, makes the fraction shown in the task.");
			T3aP1M7.setFeedbackMessage (T3aP1M7M);
			T3aP1M7.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT3aP1M7 = new Fraction ();
			nextStepT3aP1M7.setFractionForAdditionTaskEnd(startDenominator, startNumerator);
			T3aP1M7.setNextStep (nextStepT3aP1M7);

			T3aP1M8 = new FeedbackElem ();
			T3aP1M8.setID ("T3aP1M8");
			FeedbackMessage T3aP1M8M = new FeedbackMessage ();
			T3aP1M8M.setSocratic ("Think about how you add two fractions together. What needs to be the same, and what do you need to add together?");
			T3aP1M8M.setGuidance ("To add two fractions together, the denominators need to be the same. Then you add together the numerators.");
			T3aP1M8.setFeedbackMessage (T3aP1M8M);
			T3aP1M8.setFeedbackType (FeedbackType.problemSolving);
			Fraction nextStepT3aP1M8 = new Fraction ();
			nextStepT3aP1M8.setFractionForAdditionTaskEnd(startDenominator, startNumerator);
			T3aP1M8.setNextStep (nextStepT3aP1M8);

			T3aP1M9 = new FeedbackElem ();
			T3aP1M9.setID ("T3aP1M9");
			FeedbackMessage T3aP1M9M = new FeedbackMessage ();
			T3aP1M9M.setHighlighting (Highlighting.RepresentationToolBox);
			T3aP1M9.setFeedbackMessage (T3aP1M9M);
			T3aP1M9.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepT3aP1M9 = new Fraction ();
			nextStepT3aP1M9.setFractionForAdditionTaskEnd(startDenominator, startNumerator);
			T3aP1M9.setNextStep (nextStepT3aP1M9);

			T3aP1M10 = new FeedbackElem ();
			T3aP1M10.setID ("T3aP1M10");
			FeedbackMessage T3aP1M10M = new FeedbackMessage ();
			T3aP1M10M.setDidacticConceptual ("Excellent. Please explain why you made the denominator "+startDenominator+".");
			T3aP1M10.setFeedbackMessage (T3aP1M10M);
			T3aP1M10.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepT3aP1M10 = new Fraction ();
			nextStepT3aP1M10.setSpeech(true);
			T3aP1M10.setNextStep (nextStepT3aP1M10);

			T3aP1M11 = new FeedbackElem ();
			T3aP1M11.setID ("T3aP1M11");
			FeedbackMessage T3aP1M11M = new FeedbackMessage ();
			T3aP1M11M.setSocratic ("How can you check, using a Fractions Lab tool,  that your solution is correct?");
			T3aP1M11M.setGuidance ("You could use the addition box to add your fractions.");
			T3aP1M11M.setDidacticConceptual ("Add your two fractions using the addition box.");
			T3aP1M11.setFeedbackMessage (T3aP1M11M);
			T3aP1M11.setFeedbackType (FeedbackType.nextStep);
			Fraction nextStepT3aP1M11 = new Fraction ();
			nextStepT3aP1M11.setAdditionBox (true);
			T3aP1M11.setNextStep (nextStepT3aP1M11);

			T3aP1E1 = new FeedbackElem ();
			T3aP1E1.setID ("T3aP1E1");
			FeedbackMessage T3aP1E1M = new FeedbackMessage ();
			T3aP1E1M.setDidacticConceptual ("The way that you worked that out was excellent. Well done.");
			T3aP1E1.setFeedbackMessage (T3aP1E1M);
			T3aP1E1.setFeedbackType (FeedbackType.affirmation);

			T3aP1E2 = new FeedbackElem ();
			T3aP1E2.setID ("T3aP1E2");
			FeedbackMessage T3aP1E2M = new FeedbackMessage ();
			T3aP1E2M.setDidacticConceptual ("Please explain how you answered the task. Why did you make the fractions that you made?.");
			T3aP1E2.setFeedbackMessage (T3aP1E1M);
			T3aP1E2.setFeedbackType (FeedbackType.reflection);
			Fraction nextStepT3aPE2 = new Fraction ();
			nextStepT3aPE2.setSpeech(true);
			T3aP1E2.setNextStep (nextStepT3aPE2);


		}



		public FeedbackElem getFeedbackElem(string id){
			if (S1.getID ().Equals (id)) return S1;
			else if (S2.getID ().Equals (id)) return S2;
			else if (S3.getID ().Equals (id)) return S3;
			else if (M1.getID ().Equals (id)) return M1;
			else if (M2.getID ().Equals (id)) return M2;
			else if (M3.getID ().Equals (id)) return M3;
			else if (M4.getID ().Equals (id)) return M4;
			else if (M5.getID ().Equals (id)) return M5;
			else if (M6.getID ().Equals (id)) return M6;
			else if (M7.getID ().Equals (id)) return M7;
			else if (M8.getID ().Equals (id)) return M8;
			else if (M9.getID ().Equals (id)) return M9;
			else if (M10.getID ().Equals (id)) return M10;
			else if (M11.getID ().Equals (id)) return M11;
			else if (E1.getID ().Equals (id)) return E1;
			else if (E2.getID ().Equals (id)) return E2;
			else if (R1.getID ().Equals (id)) return R1;
			else if (R2.getID ().Equals (id)) return R2;
			else if (O1.getID ().Equals (id)) return O1;
			else if (O2.getID ().Equals (id)) return O2;
			else if (CM2.getID ().Equals (id)) return CM2;
			else if (CM5.getID ().Equals (id)) return CM5;
			else if (CM6.getID ().Equals (id)) return CM6;
			else if (CM6Second.getID ().Equals (id)) return CM6Second;
			else if (CM7.getID ().Equals (id)) return CM7;
			else if (CM8.getID ().Equals (id)) return CM8;
			else if (CM11.getID ().Equals (id)) return CM11;
			else if (CM12.getID ().Equals (id)) return CM12;
			else if (CE2.getID ().Equals (id)) return CE2;
			else if (FM6.getID ().Equals (id)) return FM6;
			else if (FM10.getID ().Equals (id)) return FM10;
			else if (FM11.getID ().Equals (id)) return FM11;
			else if (FE1.getID ().Equals (id)) return FE1;
			else if (FE2.getID ().Equals (id)) return FE2;
			else if (F2M1.getID ().Equals (id)) return F2M1;
			else if (F2M4.getID ().Equals (id)) return F2M4;
			else if (F2M6.getID ().Equals (id)) return F2M6;
			else if (F2M7.getID ().Equals (id)) return F2M7;
			else if (F2M7b.getID ().Equals (id)) return F2M7b;
			else if (F2M7c.getID ().Equals (id)) return F2M7c;
			else if (F2M10.getID ().Equals (id)) return F2M10;
			else if (F2M11.getID ().Equals (id)) return F2M11;
			else if (F2E1.getID ().Equals (id)) return F2E1;
			else if (F2E2.getID ().Equals (id)) return F2E2;
			else if (T24M1.getID ().Equals (id)) return T24M1;
			else if (T24M2.getID ().Equals (id)) return T24M2;
			else if (T24M3.getID ().Equals (id)) return T24M3;
			else if (T24M4.getID ().Equals (id)) return T24M4;
			else if (T24M5.getID ().Equals (id)) return T24M5;
			else if (T24M6.getID ().Equals (id)) return T24M6;
			else if (T24M7.getID ().Equals (id)) return T24M7;
			else if (T24M8.getID ().Equals (id)) return T24M8;
			else if (T24M9.getID ().Equals (id)) return T24M9;
			else if (T24M10.getID ().Equals (id)) return T24M10;
			else if (T24M11.getID ().Equals (id)) return T24M11;
			else if (T24E1.getID ().Equals (id)) return T24E1;
			else if (T24E2.getID ().Equals (id)) return T24E2;

			else if (T26M1.getID ().Equals (id)) return T26M1;
			else if (T26M2.getID ().Equals (id)) return T26M2;
			else if (T26M3.getID ().Equals (id)) return T26M3;
			else if (T26M4.getID ().Equals (id)) return T26M4;
			else if (T26M5.getID ().Equals (id)) return T26M5;
			else if (T26M6.getID ().Equals (id)) return T26M6;
			else if (T26M7.getID ().Equals (id)) return T26M7;
			else if (T26M8.getID ().Equals (id)) return T26M8;
			else if (T26M10.getID ().Equals (id)) return T26M10;
			else if (T26M11.getID ().Equals (id)) return T26M11;
			else if (T26E1.getID ().Equals (id)) return T26E1;
			else if (T26E2.getID ().Equals (id)) return T26E2;

			else if (T3aP1M1.getID ().Equals (id)) return T3aP1M1;
			else if (T3aP1M2.getID ().Equals (id)) return T3aP1M2;
			else if (T3aP1M3.getID ().Equals (id)) return T3aP1M3;
			else if (T3aP1M4.getID ().Equals (id)) return T3aP1M4;
			else if (T3aP1M5.getID ().Equals (id)) return T3aP1M5;
			else if (T3aP1M6.getID ().Equals (id)) return T3aP1M6;
			else if (T3aP1M7.getID ().Equals (id)) return T3aP1M7;
			else if (T3aP1M8.getID ().Equals (id)) return T3aP1M8;
			else if (T3aP1M9.getID ().Equals (id)) return T3aP1M9;
			else if (T3aP1M10.getID ().Equals (id)) return T3aP1M10;
			else if (T3aP1M11.getID ().Equals (id)) return T3aP1M11;
			else if (T3aP1E1.getID ().Equals (id)) return T3aP1E1;
			else if (T3aP1E2.getID ().Equals (id)) return T3aP1E2;

			else return new FeedbackElem ();
		}
	}


}

