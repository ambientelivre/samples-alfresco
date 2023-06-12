/*
* counter.js
* Alfresco Sample Script For Incremental Counter by @ambientelivre
*
* This script start and set a value for incremental counter using a metadata
* 
* But first, you need to import the aspect for alfresco share available in the link below
* https://github.com/ambientelivre/samples-alfresco/blob/master/metadados/aspect/AlfrescoCounter.zip
*
* The project is open source in https://github.com/ambientelivre/samples-alfresco
* contrib!
* Created by Williane Delfino     at williane@ambientelivre.com.br 
*            Marcio Junior Vieira at marcio@ambientelivre.com.br
*
*/

var pasta = space;

var currentCounter = pasta.properties["alfcounter:incrementCounter"];
var newCounter = parseInt(currentCounter)+1;

document.properties["alfcounter:incrementCounter"] = newCounter;
document.save();

pasta.properties["alfcounter:incrementCounter"] = document.properties["alfcounter:incrementCounter"];
pasta.save();
