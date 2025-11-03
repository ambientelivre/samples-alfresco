### Alfresco Samples

<p align="right">
  <b>
    <a href="/docs/README.en.md">English</a> |
    <a href="/README.md">Português</a>
  </b>
</p>

Samples with Alfresco Community Edition by Ambiente Livre Labs.

Sample Directories

## cmis

Samples with CMIS

### cmis/csharp

Samples with CMIS and C#

Sample for connecting, creating folders, uploading documents, and saving metadata in Alfresco CMIS using C#.

## Javascript

Samples with Javascript.

- send_mail_with_template.js - Sends emails using the Alfresco Javascript API.
- start-parallel-review-workflow.js - Starts parallel review workflows using the Alfresco Javascript API.

## Libraries 

**alfrescoXmlMetadataImporter** — Automatically fills metadata based on XML file. (Currently PDF only)
  
### How to use

1. Copy the desired file(s) and place them in the `Repository > Data Dictionary > Scripts` folder in Alfresco.
   
2. Create a rule in the desired folder and select, in the "Run Action" field, the "run script" option, then choose the template file.
   
3. Edit the template and XML file as needed.

> **Note:** The XML file must already be in the folder where the rule will be applied.

## metadata

Samples with Metadata

## metadata/customsearch

Samples for custom search with custom metadata fields, live search, and advanced search.

## Custom Constraint

- **CustomConstraint:** custom content models automatically filled via database request.

## Themes

<p align="center">
   <img src="/docs/img/loginPage.png" alt="login_page" width="650">
</p>

Contains plugins for customizing Alfresco Share.

### Included Customizations

- Login Page: background, logo, copyright, colors.
- Header: logo, color.
- Footer: logo, copyright.
- Alfresco: HTML and CSS elements have been modified for visual customization.

### How to Install

1. Clone the repository:

   ```bash
      git clone https://github.com/ambientelivre/samples-alfresco.git
   ```

2. Copy the .amp files to the corresponding directory in your Alfresco installation:

   ```bash
      cp samples-alfresco/themes/ambiente-livre3.0.amp  <YOU-INSTALL-ALFRESCO>/share/modules/amps
   ```

3. Restart Alfresco to apply the changes. (in this example we are using **docker-compose**)

   ```bash
      cd <YOU-INSTALL-ALFRESCO>
      docker-compose down
      docker-compose up -d --build
   ```

4. Access your Alfresco and select the Ambiente Livre theme in the tools section.

## Ambiente Livre

Alfresco training? See more at <https://www.ambientelivre.com.br/treinamento/alfresco/fundamental.html>
