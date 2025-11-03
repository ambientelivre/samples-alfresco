# Plugin com Custom Constraint e Metadados Customizados.

### Descrição

O CustomConstraint consiste em uma classe Java que retorna uma lista de valores no formato String, utilizada para preencher uma propriedade customizada criada em um modelo de conteúdo do Alfresco. O objetivo é permitir o preenchimento automático dessa propriedade, seja de forma estática ou por meio de consulta ao banco de dados.

### Quando eu uso o plugin

Utilize o plugin caso tenha alguma dessas dores:

- Propriedades preenchidas com erros ortográficos
- Falta de padronização dos metadados conforme as regras da organização
- Tempo excessivo gasto no preenchimento manual das propriedades dos documentos

### Composição

- **custom-constraint-platform:** plugin para o Alfresco.
- **custom-constraint-share:** plugin para a interface gráfica do Alfresco Share.

### Como Instalar

1. Clone o repositório:

   ```bash
      git clone https://github.com/ambientelivre/samples-alfresco.git
   ```

2. Copie os arquivos SNAPSHOT.jar para o diretório correspondente do seu Alfresco:

   ```bash
      cp samples-alfresco/plugins/CustomConstraint/custom-constraint-share-1.0-SNAPSHOT.jar  <YOU-INSTALL-ALFRESCO>/share/modules/jars
   ```
   
   ```bash
      cp samples-alfresco/plugins/CustomConstraint/custom-constraint-platform-1.0-SNAPSHOT.jar  <YOU-INSTALL-ALFRESCO>/alfresco/modules/jars
   ```

3. Reinicie o Alfresco para aplicar as mudanças. (neste exemplo estamos usando **docker-compose**)

   ```bash
      cd <YOU-INSTALL-ALFRESCO>
      docker-compose down
      docker-compose up -d --build
   ```

4. Acesse seu alfresco e aplique o modelo customizado na pasta desejada via regra.(nesse exemplo o modelo denominado é Usuarios)

### Como Desenvolver localmente 

1. Utilize o Archtype Maeven `org.alfresco.maven.archetype:alfresco-allinone-archetype` e selecione a versão correspondente a versão do alfresco. Ele traz o ambiente de desenvolvimento pronto do alfresco e a interface gráfica para o desenvolvimento de plugins.

2. Inicialize o Archtype
   
 ```bash
      sudo ./run.sh build_start
   ```
3. Coloque o endereço no browser

   - [Share](http://localhost:8180/share/): `http://localhost:8180/share/`
   - [Alfresco](http://localhost:8080/alfresco/) : `http://localhost:8080/alfresco/`

### Passo a Passo 

1. Acesse seu alfresco e crie novo modelo na seção `Ferramentas > Gerente de Modelos`

2. Crie o Layout em todos os Tipos do Documentos criados no Designer de Layout

3. Ative o Modelo, Exporte o Modelo e Exclua o modelo
   
4. Crie o arquivo `CustomConstraint.java` no diretório `PROJECT_NAME-platform > src > main > java > <nome-do-groupId> > platformsample` e adicione o código 

 ```bash

// Classe que estende ListOfValuesConstraint para buscar valores permitidos de um banco de dados
public class CustomConstraint extends ListOfValuesConstraint {
    // Variáveis para conexão JDBC
    private String jdbcUrl;
    private String username;
    private String password;

    // Sobrescreve método para não permitir set manual dos valores permitidos
    @Override
    public void setAllowedValues(List<String> allowedValues) {
        // Não faz nada, pois os valores são buscados do banco
    }

    // Sobrescreve método para não permitir alteração manual da sensibilidade de caixa
    @Override
    public void setCaseSensitive(boolean caseSensitive) {
        // Não faz nada, pois a sensibilidade é definida no initialize
    }

    // Método chamado para inicializar o constraint
    public void initialize() {
        // Define que não é sensível a caixa
        super.setCaseSensitive(false);
        // Busca os dados do banco de dados
        this.fetchDataFromDb();
    }

    // Método responsável por buscar os valores permitidos do banco de dados
    protected void fetchDataFromDb() {
        List<String> allowedValueList = new ArrayList<String>();
        Connection conn = null;
        Statement statement = null;
        ResultSet resultset = null;

        try {
            // Carrega o driver JDBC do SQL Server
            Class.forName("org.postgresql.Driver");

            // Define os parâmetros de conexão
            jdbcUrl = "jdbc:postgresql://postgres:5432/alfresco";
			   username = "alfresco";
			   password = "alfresco";

            // Realiza a conexão com o banco de dados
            conn = DriverManager.getConnection(jdbcUrl, username, password);

            System.out.println("Foi conectado o Banco de Dados Postgres com Sucesso!");

            // Cria o statement e executa a query
            statement = conn.createStatement();
            resultset = statement.executeQuery("SELECT authority FROM alf_authority");

            //System.out.println("RESULTADO DA QUERY DO BANCO DE DADOS : ");

            // Adiciona os resultados da query na lista de valores permitidos
            while (resultset.next()) {
                allowedValueList.add(resultset.getString("authority"));
                //System.out.println(resultset.getString("authority"));
            }

        } catch (ClassNotFoundException e) {
            // Caso o driver JDBC não seja encontrado
            System.err.println("PostgreSQL JDBC Driver não foi encontrado." + e);
        } catch (SQLException e) {
            // Caso ocorra erro na conexão ou execução da query
            System.err.println("Não conseguiu fazer a conexão ou executar a query: " + e);
        } finally {
            // Fecha o ResultSet
            if (resultset != null) {
                try {
                    resultset.close();
                } catch (SQLException e) {
                    System.err.println("Erro ao fechar o ResultSet: " + e);
                }
            }
            // Fecha o Statement
            if (statement != null) {
                try {
                    statement.close();
                } catch (SQLException e) {
                    System.err.println("Erro ao fechar o Statement: " + e);
                }
            }
            // Fecha a conexão
            if (conn != null) {
                try {
                    conn.close();
                } catch (SQLException e) {
                    System.err.println("Erro ao fechar a conexão: " + e);
                }
            }
        }

        // Caso não tenha retornado nenhum valor do banco, adiciona valores padrão
        if (allowedValueList.isEmpty()) {
            allowedValueList.add("Não retornou nenhum valor da query");
        }

        // Define os valores permitidos no constraint
        super.setAllowedValues(allowedValueList);
    }
}
   ```
   
5. Copie o arquivo `<nome-do-modelo>.xml` e cole no diretório `PROJECT_NAME-plataform > src > main > resources > alfresco > module.PROJECT_NAME-plataform > model`
   
6. No arquivo do modelo adicione a linha de código dentro da tag na propriedade desejada
   
   ```bash
     <constraints>
         <constraint name="<prefix>:<nome-propriedade>" type="<nome-do-groupId>.platformsample.<nome-classe-java>"/>
      </constraints>
   ```

7. Acesse o arquivo `PROJECT_NAME-plataform > src > main > resources > alfresco > module.PROJECT_NAME-plataform > context > bootstrap-context.xml` e adicione as linha de código dentro da tag `<property name="models"<list>{codigo}</list></property>`

```bash
      <value>alfresco/module/${project.artifactId}/model/<nome-do-modelo>.xml</value>
   ```

8. Acesse o arquivo `CMM_<nome-do-modelo>_module.xml`, copie todo o conteúdo do arquivo que esteja dentro da tag `<configurations>` e cole no arquivo `PROJECT_NAME-share > src > main > resources > share-config-custom.xml`

9. Adicione dentro da tag `<types>` todos os tipos de documentos criados, caso tenha criado aspectos são na tag `<aspects>`

- Estrutura para adicionar aspectos: 
```bash
      <subtype label="<nome-tipo-documento>" name="<prefixo-do-modelo>:<nome-do-tipo"/>
   ```
- Estrutura para adicionar tipos dos documentos:
  
```bash
      <aspect label="<nome-do-aspecto>" name="<prefixo-do-modelo>:<nome-do-aspecto"/>
   ```

10. Reinicie o alfresco 
    
 ```bash
      sudo ./run.sh build_start
   ```