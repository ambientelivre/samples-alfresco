#### Exemplos Alfresco

<p align="right">
  <b>
    <a href="/docs/README.en.md">English</a> |
    <a href="/README.md">Português</a>
  </b>
</p>

Exemplos com Alfresco Community Edition por Ambiente Livre Labs.

Diretórios de Exemplos

## cmis

Exemplos com CMIS

### cmis/csharp

Exemplos com CMIS e C#

Exemplo de conexão, criação de pasta, upload de documentos, salvando metadados no Alfresco CMIS usando C#

## Javascript

Exemplos com Javascript.

- send_mail_with_template.js - Envia e-mails usando a API Javascript do Alfresco.
- start-parallel-review-workflow.js - Inicia workflow de revisões paralelas usando a API Javascript do Alfresco.

## metadados

Exemplos com Metadados

## metadados/customsearch

Exemplos de busca personalizada com campo de metadado customizado, busca ao vivo e busca avançada.

## Temas

<br>
<p align="center">
   <img src="/docs/img/loginPage.png" alt="login_page" width="650">
</p>

Contém plugins para a personalização do Afresco Share.

### Personalizações Incluídas

- Página de Login: plano de fundo, logo, copyright, cores.
- Cabeçalho: logo, cor.
- Rodapé: logo, copyright
- Alfresco: elementos HTML e CSS do Alfresco foram modificados para personalização visual.

### Como Instalar

1. **Clone o repositório:**

   ```bash
      git clone https://github.com/ambientelivre/samples-alfresco.git
   ```

2. **Copie os arquivos .amp para o diretório correspondente do seu Alfresco:**

   ```bash
      cp samples-alfresco/themes/ambiente-livre3.0.amp  <YOU-INSTALL-ALFRESCO>/share/modules/amps
   ```

3. **Reinicie o Alfresco para aplicar as mudanças. (neste exemplo estamos usando docker-compose)**

   ```bash
      cd <YOU-INSTALL-ALFRESCO>
      docker-compose down
      docker-compose up -d --build
   ```

4. **Acesse seu alfresco e selecione o tema na seção ferramentas.**

## Ambiente Livre

Trainning in Alfresco? Look this <https://www.ambientelivre.com.br/treinamento/alfresco/fundamental.html>
