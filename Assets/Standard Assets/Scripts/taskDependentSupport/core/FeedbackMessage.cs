using System;
namespace taskDependentSupport.core

{
		public class FeedbackMessage
		{
			private String guidance = "";
			private String socratic = "";
			private String didacticConceptual = "";
			private String didacticProcedural = "";
			private int highlighting = 0;
		
			public void setHighlighting (int value){
				highlighting = value;
			}
		
			public int getHighlighting(){
				return highlighting;
			}

			public void setGuidance (String message){
				guidance = message;
			}

			public void setSocratic (String message){
				socratic = message;
			}

			public void setDidacticConceptual(String message){
				didacticConceptual = message;
			}

			public void setDidacticProcedural(String message){
				didacticProcedural = message;
			}

			public String getGuidance(){
				return guidance;
			}

			public String getSocratic(){
				return socratic;
			}

			public String getDidacticConceptual(){
				return 	didacticConceptual;	
			}

			public String getDidacticProcedural(){
				return didacticProcedural;
			}

		}
}

