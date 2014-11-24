using System;
using UnityEngine;
using System.Collections;
namespace taskDependentSupport.core

{
		
	public class FeedbackData
	{
				
		public FeedbackElem S1, S2, S3, M1, M2, M3, M4, M5, M6, M7, M8, M9, M10, M11, E1, E2, R1, R2, O1, O2;
		public FeedbackElem CM2, CM5, CM6, CM6Second, CM7, CM8, CM11, CM12, CE2;

		public FeedbackData (String taskID){
			Debug.Log (":::: FeedbackData taskID: "+taskID);

			int startNumerator = 0;
			int endNumerator = 0;
			int startDenominator = 0;
			int endDenominator = 0;

			if (taskID.Equals ("Comp1")) {
				endDenominator = 3;
			}
			
			if (taskID.Equals("EQUIValence1")){
				startNumerator = 3;
				endNumerator = 9;
				startDenominator = 4;
				endDenominator = 12;
			}
			else if (taskID.Equals("EQUIValence2")){
				startNumerator = 1;
				endNumerator = 2;
				startDenominator = 2;
				endDenominator = 4;
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
			M1M.setGuidance ("You can click the up arrow next to your fraction to change it.");
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
			M11M.setSocratic ("How can you check, using Fractions Lab tool, that your solution is correct?");
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
			O1M.setDidacticConceptual ("It seems like you haven't completed the task.");
			O1.setFeedbackMessage (O1M);
			Fraction nextStepO1 = new Fraction ();
			nextStepO1.setAnyValue (true);
			O1.setNextStep (nextStepO1);
			O1.setFeedbackType (FeedbackType.other);

			O2 = new FeedbackElem ();
			O2.setID ("O1");
			FeedbackMessage O2M = new FeedbackMessage ();
			O2M.setDidacticConceptual ("If you need more help to finish the task, ask your teacher.");
			O2.setFeedbackMessage (O1M);
			Fraction nextStepO2 = new Fraction ();
			nextStepO2.setAnyValue (true);
			O2.setNextStep (nextStepO2);
			O2.setFeedbackType (FeedbackType.other);

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
			else return new FeedbackElem ();
		}
	}


}

