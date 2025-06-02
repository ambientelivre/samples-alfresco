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

## metadados

Samples with Metadata

## metadados/customsearch

Samples for custom search with custom metadata fields, live search, and advanced search.

## Themes

<br>
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

Training in Alfresco? Check this out: <https://www.ambientelivre.com.br/treinamento/alfresco/fundamental.html>
