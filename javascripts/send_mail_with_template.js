// get members of group MyGroup and Send email with Template 

var group1 = people.getGroup("GROUP_MyGroup");
var members = people.getMembers(group1,false); 		

for (var member in members) 
  { 	   
    var username = members[member].properties.userName;
    var email_user = person.properties["cm:email"];
	   
    //print(username);  
    //print(email_user);  
	   
    var mail = actions.create("mail");
    mail.parameters.to =  email_user;
    mail.parameters.template =companyhome.childByNamePath("Data Dictionary/Email Templates/Workflow Notification/wf-email.html.ftl");
    mail.parameters.from =  "no-reply@ambientelivre.com.br";
    mail.parameters.subject = "The document approved";
    mail.parameters.text="Body Mail";	

    var templateArgs = new Array();
    templateArgs['workflowTitle'] = "Sample Task";
    templateArgs['workflowDescription'] = "Task assigner";
    //templateArgs['workflowId'] = task.id;
    templateArgs['workflowId'] = 1;
    templateArgs['workflowPooled'] = false;

    var templateModel = new Array();
    templateModel['args'] = templateArgs;
    mail.parameters.template_model = templateModel;
	   
    //mail.execute(bpm_package.children[0]);
    mail.execute(document);
  }
