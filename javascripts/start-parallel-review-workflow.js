/*
* start-parallel-review-workflow.js
* Sample Script for Start Parallel Reviews Workflow in Alfresco by @ambientelivre
*
* This script start and set Percent Aprovved Workflow in 100% custom for your need.
*
* The project is open source in https://github.com/ambientelivre/samples-alfresco
* contrib!
* Create by Williane Delfino     at williane@ambientelivre.com.br 
*           Marcio Junior Vieita at marcio@ambientelivre.com.br
*
* Parameters this model look workflowModel.xml :
* https://github.com/AlfrescoArchive/alfresco-repository/blob/master/src/main/resources/alfresco/workflow/workflowModel.xml
*
*/

function startWorkflow(assigneeList) {
    var workflow = actions.create("start-workflow");
    workflow.parameters.workflowName = "activiti$activitiParallelReview";
    workflow.parameters["bpm:workflowDescription"] = "Revisar e aprovar " + document.name;
    workflow.parameters["bpm:assignees"] = assigneeList;
    var futureDate = new Date();
    futureDate.setDate(futureDate.getDate() + 7);
    workflow.parameters["bpm:workflowDueDate"] = futureDate;
    workflow.parameters["wf:requiredApprovePercent"] = 100;
    return workflow.execute(document);
}

function main() {
  var listUsers = "admin,marcio";	
  var assigneeList = listUsers.split(",");
  var assigners = [];
  var assigner = []; 
  //logger.system.out(assigneeList.length);  	

  for (var i = 0; i < assigneeList.length; i++) {
     assigner[i] = people.getPerson(assigneeList[i]);

	 if (assigneeList.length == 1)
	   assigners = [assigner[0]];		
	 else if (assigneeList.length == 2)
	   assigners = [assigner[0],assigner[1]];	
  }	
  startWorkflow(assigners);	
}

main();
