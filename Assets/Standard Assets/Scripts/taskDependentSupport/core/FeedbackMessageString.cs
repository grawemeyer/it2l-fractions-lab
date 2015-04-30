using System;
namespace taskDependentSupport.core
{
	public class FeedbackMessageString
	{
		public static int startNumerator = 0;
		public static int endNumerator = 0;
		public static int startDenominator = 0;
		public static int endDenominator = 0;
		public static String representation = "area";

		public static String S1M_socratic = "How are you going to tackle this task?";
		public static String S1M_didacticConceptual = "Read the task again, and explain how you are going to tackle it.";

		public static String S2M_socratic = "What do you need to do in this task?";
		public static String S2M_guidance = "You can click one of the buttons on the representations toolbox to create a fraction.";
		public static String S2M_didacticConceptual ="Read the task again, and explain how you are going to tackle it.";

		public static String S3M_socratic = "Good. What do you need to do now, to change the fraction?";
		public static String S3M_guidance = "You can use the arrow buttons to change the fraction.";
		public static String S3M_didacticConceptual = "Now click the up arrow next to the empty fraction, to make the denominator.";
		public static String S3M_didacticProcedural = "Click the up arrow next to the empty fraction, to make the denominator (the bottom part of the fraction) "+endDenominator+".";

		public static String M1M_socratic = "Is the denominator in your fraction correct?";
		public static String M1M_guidance = "You can click the up arrow next to your fraction to change the denominator.";
		public static String M1M_didacticConceptual = "Check that the denominator in your fraction is correct.";
		public static String M1M_didacticProcedural = "Check that the denominator (the bottom part of your fraction) is "+endDenominator+".";

		public static String M2M_socratic = "Have you changed the numerator or the denominator?";
		public static String M2M_guidance = "Remember that the denominator is the bottom part of the fraction.";
		public static String M2M_didacticConceptual = "Check that you have changed the denominator, not the numerator.";
		public static String M2M_didacticProcedural = "Check that the denominator in your fraction, not the numerator, is "+endDenominator+".";

		public static String CM2M_socratic  = "Have you changed the numerator or the denominator?";
		public static String CM2M_guidance = "Remember that the denominator is the bottom part of the fraction.";
		public static String CM2M_didacticConceptual = "Check that you have changed the denominator, not the numerator.";
		public static String CM2M_didacticProcedural = "Check that the denominator in your fraction, not the numerator, is "+endDenominator+".";

		public static String M3M_socratic = "Is this the fraction you were planning to make?";
		public static String M3M_guidance = "Please read the task again carefully.";
		public static String M3M_didacticConceptual = "Re-read the task then echeck your fraction.";

		public static String M4M_didacticConceptual = "Excellent. Please explain what the numerator and denominator are.";

		public static String M5M_socratic = "Have you changed the denominator or the numerator?";
		public static String M5M_guidance = "The denominator is the bottom part of the fraction.";
		public static String M5M_didacticConceptual = "You changed the numerator. You need to change the denominator.";
		public static String M5M_didacticProcedural = "You changed the numerator. You need to change the denominator to "+endDenominator+".";

		public static String CM5M_socratic = "Have you changed the denominator or the numerator?";
		public static String CM5M_guidance = "The denominator is the bottom part of the fraction.";
		public static String CM5M_didacticConceptual = "You changed the numerator. You need to change the denominator.";
		public static String CM5M_didacticProcedural = "You changed the numerator. You need to change the denominator to "+endDenominator+".";

		public static String M6M_socratic = "Excellent. Now, how are you going to change the numerator?";
		public static String M6M_guidance = "If you click near the top of your fraction, and click the arrow, you can change the numerator (the top part of the fraction).";
		public static String M6M_didacticConceptual = "You changed the denominator. Now, change the numerator.";
		public static String M6M_DidacticProcedural = "Now, change the numerator. Remember, you need to make the fraction equivalent to "+startNumerator+"/"+startDenominator+".";

		public static String CM6M_socratic = "Excellent. Now, how are you going to compare the fraction?";
		public static String CM6M_guidance = "Now that you have made the first fraction, you need to compare it with the second fraction.";
		public static String CM6M_didacticConceptual = "You now need to compare the fraction with a second fraction.";
		public static String CM6M_didacticProcedural = "Now, create a second fraction using the same representation.";

		public static String CM6MSecond_Socratic = "Excellent. Now, how are you going to compare the fraction?";
		public static String CM6MSecond_guidance = "Now that you have made the first fraction, you need to compare it with the second fraction.";
		public static String CM6MSecond_didacticConceptual = "You now need to compare the fraction with a second fraction.";
		public static String CM6MSecond_didacticProcedural = "Now, create a second fraction using the same representation.";

		public static String M7M_socratic = "Excellent. How are you going to make the equivalent fraction?";
		public static String M7M_guidance = "You could now copy the fraction and use `find equivalent' to make an equivalent fraction. Right-click the fraction.";
		public static String M7M_didacticConceptual = "Excellent. Now copy this fraction, right click the copy, and select `find equivalent'.";
		public static String M7M_didacticProcedural = "Excellent. Now copy this fraction, right click the copy, and select `find equivalent' to make a fraction with a denominator of "+endDenominator+".";

		public static String CM7M_socratic = "Why have you decided to use a different representation? What does the task ask you to do?";
		public static String CM7M_guidance = "Please use the same representation for the second fraction.";
		public static String CM7M_didacticProcedural = "Please create the second fraction using the same representation that you used for the first fraction.";

		public static String M8M_socratic = "Now that you have made "+endNumerator+"/"+endDenominator+" how are you going to compare it with "+startNumerator+"/"+startDenominator+"?";
		public static String M8M_guidance = "Look at the task and think about what other fraction you need to make to compare it with "+endNumerator+"/"+endDenominator+".";
		public static String M8M_didacticConceptual = "You need one of your fractions to be "+startNumerator+"/"+startDenominator+", to compare it with "+endNumerator+"/"+endDenominator+".";
		public static String M8M_didacticProcedural = "Keep the fraction "+endNumerator+"/"+endDenominator+". Now change the other fraction to "+startNumerator+"/"+startDenominator+". Then compare the two fractions using the comparison box.";

		public static String M9M_socratic = "How are you going to make a fraction with the denominator "+endDenominator+", that is equivalent to "+startNumerator+"/"+startDenominator+".";
		public static String M9M_guidance = "Look at the task and think about what other fraction you need to make to compare it with "+startNumerator+"/"+startDenominator+".";
		public static String M9M_didacticConceptual = "As well as "+startNumerator+"/"+startDenominator+", you need a second fraction that is equivalent to "+startNumerator+"/"+startDenominator+" and that has "+endDenominator+" as its denominator.";
		public static String M9M_didacticProcedural = "Keep the fraction "+startNumerator+"/"+startDenominator+". Now change the other fraction to "+endNumerator+"/"+endDenominator+". Then compare the two fractions using the comparison box.";

		public static String M10M_socratic = "How could you compare your fraction with "+startNumerator+"/"+startDenominator+"?";
		public static String M10M_guidance = "Think about the denominators in the two fractions. What is the relationship between them?";
		public static String M10M_didacticConceptual = "Compare your fraction with "+startNumerator+"/"+startDenominator+"  by creating another fraction, this time "+startNumerator+"/"+startDenominator+", and using the comparison box.";

		public static String CM8M_socratic = "Excellent. How using the comparison tool to compare the two fractions?";
		public static String CM8M_guidance = "You could now compare the fractions. Open the comparison tool at the top of the screen.";
		public static String CM8M_didacticConceptual = "Excellent. Now use the comparison tool.";
		public static String CM8M_didacticProcedural = "Excellent. Not open the comparison tool, at the top of the screen, and drag in the two fractions.";

		public static String M11M_socratic = "What are you going to compare your fraction with?";
		public static String M11M_guidance = "In equivalent fractions, the two numerators must be the same multiple of each other as the two denominators.";
		public static String M11M_didacticConceptual = "First create another fraction, this time "+startNumerator+"/"+startDenominator+". Then compare your two fractions.";

		public static String M12M_didacticConceptual = "Excellent. Please explain why you made the denominator "+endDenominator+".";

		public static String M13M_socratic = "How can you check, using a Fractions Lab tool, that your solution is correct?";
		public static String M13M_guidance = "You could use the comparison box to compare your fractions.";
		public static String M13M_didacticConceptual = "Compare the two fractions using the comparison box.";

		public static String CM11M_didacticConceptual = "Excellent. Please explain why you made the denominator 3.";

		public static String CM12M_socratic = "Excellent, what are you going to do now?";

		public static String E1M_didacticConceptual = "The way that you worked that out was excellent. Well done.";

		public static String E2M_didacticConceptual = "Please explain what you did to the numerator and the denominator of "+startNumerator+"/"+startDenominator+" to make an equivalent fraction with "+endDenominator+" as the denominator.";

		public static String CE2M_didacticConceptual = "Please explain what you found by comparing 1/3 and 1/5.";

		public static String R1M_didacticConceptual = "What has happened to the numerator and denominator? Have they been affected the same or differently?";

		public static String R2M_didacticConceptual = "Why did you make the fraction "+endNumerator+"/"+endDenominator+"? What did you do to the numerator and denominator of "+startNumerator+"/"+startDenominator+"?";

		public static String O1M_didacticConceptual = "You don't appear to have completed the task.";

		public static String O2M_didacticConceptual = "If you need more help to finish the task, you could ask your teacher.";

		public static String MF6M_socratic = "Excellent. Now, how are you going to use a different representation?";
		public static String MF6M_guidance = "Excellent, Now, click the representation toolbar and create another representation of exactly the same fraction.";

		public static String MF10M_socratic = "Excellent. How are your representations similar, and how are they different?";

		public static String MF11M_socratic = "Excellent! Have you completed the task?";
		public static String MF11M_guidance = "Excellent! Keep going. Now make your fraction using a different representation.";

		public static String MF12M_socratic = "Excellent! Do your fractions have the same values?";
		public static String MF12M_guidance = "Excellent! Now change your fractions to the same values.";

		public static String MFE1M_didacticConceptual = "Excellent, you have made your fraction using all the representations. Well done.";

		public static String MFE2M_didacticConceptual ="How are all your representations similar, and how are they different?";

		public static String F2M1M_socratic = "Good. What do you need to do now, to complete the fraction?";
		public static String F2M1M_guidance = "You can use the arrow buttons at the top of the fraction to complete your fraction.";
		public static String F2M1M_didacticConceptual = "Now click the up arrow next to the top of the fraction, to make the numerator.";
		public static String F2M1M_didacticProcedural = "Click the up arrow next to the top of the fraction, to make the numerator (the top part of the fraction).";

		public static String F2M4M_socratic = "Excellent. What fraction have you made?";

		public static String F2M6M_socratic = "Excellent. Now, how are you going to partition the fraction?";
		public static String F2M6M_guidance = "Right click the fraction, select `find equivalent', and partition the fraction into two.";
		public static String F2M6M_didacticConceptual = "Click the fraction with the right-hand mouse button, then select `find equivalent' and partition the fraction into two.";
		public static String F2M6M_didacticProcedural = "Click the fraction with the right-hand mouse button. Then click `find equivalent'. Then click to partition the fraction into two.";

		public static String F2M7M_socratic = "Excellent. Now, how are you going to partition the fraction into 3?";
		public static String F2M7M_guidance = "Right click the fraction, select `find equivalent', and partition the fraction into 3.";
		public static String F2M7M_didacticConceptual = "Click the fraction with the right-hand mouse button, then select `find equivalent', and partition the fraction into 3.";
		public static String F2M7M_didacticProcedural = "Click the fraction with the right-hand mouse button. Then click `find equivalent'. Then click to partition the fraction into 3.";

		public static String F2M7bM_socratic = "Excellent. Now, how are you going to partition the fraction into 4?";
		public static String F2M7bM_guidance = "Right click the fraction, select `find equivalent', and partition the fraction into 4.";
		public static String F2M7bM_didacticConceptual = "Click the fraction with the right-hand mouse button, then select `find equivalent', and partition the fraction into 4.";
		public static String F2M7bM_didacticProcedural = "Click the fraction with the right-hand mouse button. Then click `find equivalent'. Then click to partition the fraction into 4.";

		public static String F2M7cM_socratic = "Excellent. Now, how are you going to partition the fraction into 5?";
		public static String F2M7cM_guidance = "Right click the fraction, select `find equivalent', and partition the fraction into 5.";
		public static String F2M7cM_didacticConceptual = "Click the fraction with the right-hand mouse button, then select `find equivalent', and partition the fraction into 5.";
		public static String F2M7cM_didacticProcedural = "Click the fraction with the right-hand mouse button. Then click `find equivalent'. Then click to partition the fraction into 5.";

		public static String F2M10M_socratic = "Excellent. Is your new fraction equivalent to your original fraction? What has happened to the denominator and what has happened to the numerator?";

		public static String F2M11M_socratic = "Excellent! Have you completed the task?";
		public static String F2M11M_guidance = "Keep going. Now partition your fraction further.";

		public static String MF2E1M_didacticConceptual = "Excellent, you have partitioned your fraction 2, 3, 4 and 5 times. Well done.";

		public static String MF2E2M_didacticConceptual = "When you used find equivalent, what happened to the denominators and numerators?";


		public static String T24M1M_socratic = "Is the denominator in your fraction correct?";
		public static String T24M1M_guidance = "You can click the up arrow next to your fraction to change it.";
		public static String T24M1M_didacticConceptual = "Check that the denominator in your fraction is correct.";
		public static String T24M1M_didacticProcedural = "Check that the denominator (the bottom part of your fraction) is "+startDenominator+".";

		public static String T24M2M_socratic = "Have you changed the numerator or denominator?";
		public static String T24M2M_guidance = "Remember that the denominator is the bottom part of the fraction.";
		public static String T24M2M_didacticConceptual = "Check that you have changed the denominator, not the numerator.";
		public static String T24M2M_didacticProcedural = "Check that the denominator in your fraction, not the numerator, is "+startDenominator+".";

		public static String T24M3M_socratic = "Is this the fraction you were planning to make?";
		public static String T24M3M_guidance = "Please read the task again carefully.";
		public static String T24M3M_didacticConceptual = "Re-read the task then check your fraction.";

		public static String T24M4M_didacticConceptual = "Excellent. Please explain what the 'numerator' and `denominator' are";

		public static String T24M5M_socratic = "Have you changed the denominator or the numerator?";
		public static String T24M5M_guidance = "The denominator is the bottom part of the fraction.";
		public static String T24M5M_didacticConceptual = "You changed the numerator. You need to change the denominator.";
		public static String T24M5M_didacticProcedural = "You changed the numerator. You need to change the denominator to "+startDenominator+".";

		public static String T24M6M_socratic = "Excellent. Now, how are you going to change the numerator?";
		public static String T24M6M_guidance = "If you click near the top of your fraction, and click the up arrow, you can change the numerator (the top part of the fraction).";
		public static String T24M6M_didacticConceptual = "You changed the denominator. Now, change the numerator.";
		public static String T24M6M_didacticProcedural = "Now, change the numerator. Remember, you need to make a fraction equivalent to "+startNumerator+"/"+startDenominator+".";

		public static String T24M7M_socratic = "Excellent. How are you going to make an equivalent fraction?";
		public static String T24M7M_guidance = "You could now copy the fraction and use `find equivalent' to make an equivalent fraction.";
		public static String T24M7M_didacticConceptual = "Excellent. Now copy this fraction and use `find equivalent' to change it.";
		public static String T24M7M_didacticProcedural = "Excellent. Now right click the fraction and copy it. Then right click the copy and select `find equivalent' to make an equivalent fraction.";

		public static String T24M8M_socratic = "How could you compare your fraction with "+startNumerator+"/"+startDenominator+".";
		public static String T24M8M_guidance = "Think about the denominators in your fraction and in "+startNumerator+"/"+startDenominator+". What is the relationship between them? What do you need to do to "+startNumerator+" to work out the correct numerator for your fraction?";
		public static String T24M8M_didacticConceptual = "Compare your fraction with "+startNumerator+"/"+startDenominator+" by using the comparison box.";

		public static String T24M9M_socratic = "What are you going to compare your fraction with?";
		public static String T24M9M_guidance = "In equivalent fractions, the two numerators must be the same multiple of each other as the two denominators.";
		public static String T24M9M_didacticConceptual = "First create another fraction, this time "+startNumerator+"/"+startDenominator+". Then compare your two fractions.";

		public static String T24M10M_didacticConceptual = "Excellent. Please explain why you made the denominator "+startDenominator+" or a multiple of "+startDenominator+".";

		public static String T24M11M_socratic = "How can you check, using a Fractions Lab tool,  that your solution is correct?";
		public static String T24M11M_guidance = "You could use the comparison box to compare your fractions.";
		public static String T24M11M_didacticConceptual = "Compare the two fractions using the comparison box.";

		public static String T24M12M_socratic = "Now that you have made a fraction that is equivalent to "+startNumerator+"/"+startDenominator+", how are you going to compare it with "+startNumerator+"/"+startDenominator+"?";
		public static String T24M12M_guidance = "Look at the task and think about what other fraction you need to make, to compare with your fraction.";
		public static String T24M12M_didacticConceptual = "To compare your fraction to "+startNumerator+"/"+startDenominator+", you will need to make a second fraction, this time "+startNumerator+"/"+startDenominator+".";
		public static String T24M12M_didacticProcedural = "Keep the fraction that you have made. Now create a second fraction. This time "+startNumerator+"/"+startDenominator+". Then compare the two fractions using the comparison box.";

		public static String T24M13M_socratic = "Now that you have made a fraction that is equivalent to "+startNumerator+"/"+startDenominator+", how are you going to compare it with "+startNumerator+"/"+startDenominator+"?";
		public static String T24M13M_guidance = "Look at the task and think about what other fraction you need to make, to compare with your fraction.";
		public static String T24M13M_didacticConceptual = "To compare your fraction to "+startNumerator+"/"+startDenominator+", you will need to make a second fraction, this time "+startNumerator+"/"+startDenominator+".";
		public static String T24M13M_didacticProcedural = "Keep the fraction that is equivalent to "+startNumerator+"/"+startDenominator+". Now change the other fraction to "+startNumerator+"/"+startDenominator+". Then compare the two fractions using the comparison box.";

		public static String T24E1M_didacticConceptual = "The way that you worked that out was excellent. Well done.";

		public static String T24E2M_didacticConceptual = "Please explain what you did to the numerator and denominator of "+startNumerator+"/"+startDenominator+" to make an equivalent fraction.";



		public static String T26M1M_guidance = "You can click the up arrow next to your fraction to change it.";
		public static String T26M1M_socratic = "Is the denominator in your fraction correct?";
		public static String T26M1M_didacticConceptual = "Check that the denominator in your fraction is correct.";
		public static String T26M1M_didacticProcedural = "Check that the denominator (the bottom part of your fraction) is "+startDenominator+" or "+endDenominator+".";

		public static String T26M2M_guidance = "Remember that the denominator is the bottom part of the fraction.";
		public static String T26M2M_socratic = "Have you changed the numerator or denominator?";
		public static String T26M2M_didacticConceptual = "Check that you have changed the denominator, not the numerator.";
		public static String T26M2M_didacticProcedural = "Check that the denominator in your fraction, not the numerator, is "+startDenominator+" or "+endDenominator+".";

		public static String T26M3M_guidance = "Please read the task again carefully.";
		public static String T26M3M_socratic = "Is this the fraction you were planning to make?";
		public static String T26M3M_didacticConceptual = "Re-read the task then check your fraction.";

		public static String T26M4M_didacticConceptual = "Excellent. Please explain what the 'numerator' and `denominator' are.";

		public static String T26M5M_guidance = "The denominator is the bottom part of the fraction.";
		public static String T26M5M_socratic = "Have you changed the denominator or the numerator?";
		public static String T26M5M_didacticConceptual = "You changed the numerator. You need to change the denominator.";
		public static String T26M5M_didacticProcedural = "You changed the numerator. You need to change the denominator to "+startDenominator+" or "+endDenominator+".";

		public static String T26M6M_guidance = "If you click near the top of your fraction, and click the up arrow, you can change the numerator (the top part of the fraction).";
		public static String T26M6M_socratic = "Excellent. Now, how are you going to change the numerator?";
		public static String T26M6M_didacticConceptual = "You changed the denominator.  Now, change the numerator.";
		public static String T26M6M_didacticProcedural = "Now, change the numerator. Remember, you need to make the fraction "+startNumerator+"/"+startDenominator+" or "+endNumerator+"/"+endDenominator+".";

		public static String T26M7M_guidance = "There are two fractions in this task.  You can make another fraction by using the representation tool.";
		public static String T26M7M_socratic = "Excellent. How are you going to make the second fraction?";
		public static String T26M7M_didacticConceptual = "Please make the other fraction.";
		public static String T26M7M_didacticProcedural = "You have made "+startNumerator+"/"+startDenominator+" or "+endNumerator+"/"+endDenominator+".  Please make "+startNumerator+"/"+startDenominator+" or "+endNumerator+"/"+endDenominator+".";

		public static String T26M8M_guidance = "It is easier to compare fractions when they are represented the same way.";
		public static String T26M8M_socratic = "Can you think of a clearer way to represent the fractions?";
		public static String T26M8M_didacticConceptual = "Please change one of your fractions so they use the same representation.";

		public static String T26M10M_didacticConceptual = "Great. Please explain why you made these fractions.";

		public static String T26M11M_guidance = "You could use the comparison box to compare your fractions.";
		public static String T26M11M_socratic = "How can you check, using a Fractions Lab tool,  that your solution is correct?";
		public static String T26M11M_didacticConceptual = "Compare the two fractions using the comparison box.";

		public static String T26E1M_didacticConceptual = "The way that you worked that out was excellent. Well done.";

		public static String T26E2M_didacticConceptual = "Please explain why you agree or disagree.";



		public static String T3aP1M1M_socratic = "Is the denominator in your fraction correct?";
		public static String T3aP1M1M_guidance = "You can click the up arrow next to your fraction to change it.";
		public static String T3aP1M1M_didacticConceptual = "Check that the denominator in your fraction is correct.";
		public static String T3aP1M1M_didacticProcedural = "Check that the denominator (the bottom part of your fraction) is "+startDenominator+".";

		public static String T3aP1M2M_socratic = "Have you changed the numerator or denominator?";
		public static String T3aP1M2M_guidance = "Remember that the denominator is the bottom part of the fraction.";
		public static String T3aP1M2M_didacticConceptual = "Check that you have changed the denominator, not the numerator.";
		public static String T3aP1M2M_didacticProcedural = "Check that the denominator in your fraction, not the numerator, is "+startDenominator+".";

		public static String T3aP1M3M_socratic = "Is this the fraction you were planning to make?";
		public static String T3aP1M3M_guidance = "Please read the task again carefully.";
		public static String T3aP1M3M_didacticConceptual = "Re-read the task then check your fraction.";

		public static String T3aP1M4M_didacticConceptual = "Excellent. Please explain what the 'numerator' and `denominator' are.";

		public static String T3aP1M5M_socratic = "Have you changed the denominator or the numerator?";
		public static String T3aP1M5M_guidance = "The denominator is the bottom part of the fraction.";
		public static String T3aP1M5M_didacticConceptual = "You changed the numerator. You need to change the denominator.";
		public static String T3aP1M5M_didacticProcedural = "You changed the numerator. You need to change the denominator to "+startDenominator+".";

		public static String T3aP1M6M_socratic = "Excellent. Now, how are you going to change the numerator?";
		public static String T3aP1M6M_guidance = "If you click near the top of your fraction, and click the up arrow, you can change the numerator (the top part of the fraction).";
		public static String T3aP1M6M_didacticConceptual = "You changed the denominator.  Now, change the numerator.";
		public static String T3aP1M6M_didacticProcedural = "Now, change the numerator. Remember, you need to make two fractions that can be added together to make "+startNumerator+"/"+startDenominator+".";

		public static String T3aP1M7M_socratic = "Excellent. You've made one fraction. What do you need to do now, to complete the task?";
		public static String T3aP1M7M_guidance = "You now need to make a second fraction that, when added together with your first fraction, makes the fraction shown in the task.";

		public static String T3aP1M8M_socratic = "Think about how you add two fractions together. What needs to be the same, and what do you need to add together?";
		public static String T3aP1M8M_guidance = "To add two fractions together, the denominators need to be the same. Then you add together the numerators.";

		public static String T3aP1M10M_didacticConceptual = "Excellent. Please explain why you made the denominator "+startDenominator+".";

		public static String T3aP1M11M_socratic = "How can you check, using a Fractions Lab tool,  that your solution is correct?";
		public static String T3aP1M11M_guidance = "You could use the addition box to add your fractions.";
		public static String T3aP1M11M_didacticConceptual = "Add your two fractions using the addition box.";
	
		public static String T3aP1E1M_didacticConceptual = "The way that you worked that out was excellent. Well done.";

		public static String T3aP1E2M_didacticConceptual = "Please explain how you answered the task. Why did you make the fractions that you made?.";


		public FeedbackMessageString (){}

		
	}
}

