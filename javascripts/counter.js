/*
* counter.js
* Alfresco Sample Script For Incremental Counter by @ambientelivre
*
* This script start and set a value for incremental counter using a single metadata of an aspect for alfresco 
* (available in the link below to import)
* 
*
* The project is open source in https://github.com/ambientelivre/samples-alfresco
* contrib!
* Created by Williane Delfino     at williane@ambientelivre.com.br 
*            Marcio Junior Vieita at marcio@ambientelivre.com.br
*
*/

var pasta = space;

var currentCounter = pasta.properties["alfcounter:incrementCounter"];
var newCounter = parseInt(currentCounter)+1;

document.properties["alfcounter:incrementCounter"] = newCounter;
document.save();

pasta.properties["alfcounter:incrementCounter"] = document.properties["alfcounter:incrementCounter"];
pasta.save();
