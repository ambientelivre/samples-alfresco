# Plugin with Custom Constraint and Custom Metadata

### Description

CustomConstraint consists of a Java class that returns a list of values in String format, used to fill a custom property created in an Alfresco content model. The goal is to allow automatic filling of this property, either statically or by querying the database.

### When to use the plugin

Use the plugin if you have any of these issues:

- Properties filled with spelling errors
- Lack of standardization of metadata according to organizational rules
- Excessive time spent manually filling document properties

### Composition

- **custom-constraint-platform:** plugin for Alfresco.
- **custom-constraint-share:** plugin for the Alfresco Share graphical interface.

### How to Install

1. Clone the repository:

   ```bash
      git clone https://github.com/ambientelivre/samples-alfresco.git
   ```

2. Copy the SNAPSHOT.jar files to the corresponding directory in your Alfresco installation:

   ```bash
      cp samples-alfresco/plugins/CustomConstraint/custom-constraint-share-1.0-SNAPSHOT.jar  <YOU-INSTALL-ALFRESCO>/share/modules/jars
   ```
   
   ```bash
      cp samples-alfresco/plugins/CustomConstraint/custom-constraint-platform-1.0-SNAPSHOT.jar  <YOU-INSTALL-ALFRESCO>/alfresco/modules/jars
   ```

3. Restart Alfresco to apply the changes. (in this example we are using **docker-compose**)

   ```bash
      cd <YOU-INSTALL-ALFRESCO>
      docker-compose down
      docker-compose up -d --build
   ```

4. Access your Alfresco and apply the custom model to the desired folder via rule (in this example, the model is named Usuarios).

### How to develop locally

1. Use the Maven Archetype `org.alfresco.maven.archetype:alfresco-allinone-archetype` and select the version corresponding to your Alfresco version. It provides a ready-to-use Alfresco development environment and graphical interface for plugin development.

2. Initialize the Archetype
   
   ```bash
      sudo ./run.sh build_start
   ```
3. Open the address in your browser

   - [Share](http://localhost:8180/share/): `http://localhost:8180/share/`
   - [Alfresco](http://localhost:8080/alfresco/) : `http://localhost:8080/alfresco/`

### Step by Step

1. Access your Alfresco and create a new model in `Tools > Model Manager`
2. Create the layout for all document types created in the Layout Designer
3. Activate the model, export the model, and delete the model
4. Create the file `CustomConstraint.java` in the directory `PROJECT_NAME-platform > src > main > java > <groupId> > platformsample` and add the code

   ```java
   // Class that extends ListOfValuesConstraint to fetch allowed values from a database
   public class CustomConstraint extends ListOfValuesConstraint {
       // JDBC connection variables
       private String jdbcUrl;
       private String username;
       private String password;

       // Override method to prevent manual setting of allowed values
       @Override
       public void setAllowedValues(List<String> allowedValues) {
           // Does nothing, values are fetched from the database
       }

       // Override method to prevent manual change of case sensitivity
       @Override
       public void setCaseSensitive(boolean caseSensitive) {
           // Does nothing, sensitivity is set in initialize
       }

       // Method called to initialize the constraint
       public void initialize() {
           // Set case sensitivity to false
           super.setCaseSensitive(false);
           // Fetch data from the database
           this.fetchDataFromDb();
       }

       // Method responsible for fetching allowed values from the database
       protected void fetchDataFromDb() {
           List<String> allowedValueList = new ArrayList<String>();
           Connection conn = null;
           Statement statement = null;
           ResultSet resultset = null;

           try {
               // Load the PostgreSQL JDBC driver
               Class.forName("org.postgresql.Driver");

               // Set connection parameters
               jdbcUrl = "jdbc:postgresql://postgres:5432/alfresco";
               username = "alfresco";
               password = "alfresco";

               // Connect to the database
               conn = DriverManager.getConnection(jdbcUrl, username, password);

               System.out.println("Successfully connected to PostgreSQL database!");

               // Create statement and execute query
               statement = conn.createStatement();
               resultset = statement.executeQuery("SELECT authority FROM alf_authority");

               // Add query results to allowed values list
               while (resultset.next()) {
                   allowedValueList.add(resultset.getString("authority"));
               }

           } catch (ClassNotFoundException e) {
               System.err.println("PostgreSQL JDBC Driver not found." + e);
           } catch (SQLException e) {
               System.err.println("Could not connect or execute query: " + e);
           } finally {
               if (resultset != null) {
                   try {
                       resultset.close();
                   } catch (SQLException e) {
                       System.err.println("Error closing ResultSet: " + e);
                   }
               }
               if (statement != null) {
                   try {
                       statement.close();
                   } catch (SQLException e) {
                       System.err.println("Error closing Statement: " + e);
                   }
               }
               if (conn != null) {
                   try {
                       conn.close();
                   } catch (SQLException e) {
                       System.err.println("Error closing connection: " + e);
                   }
               }
           }

           // If no values returned, add default value
           if (allowedValueList.isEmpty()) {
               allowedValueList.add("No values returned from query");
           }

           // Set allowed values in constraint
           super.setAllowedValues(allowedValueList);
       }
   }
   ```

5. Copy the `<model-name>.xml` file and paste it into `PROJECT_NAME-platform > src > main > resources > alfresco > module.PROJECT_NAME-platform > model`
6. In the model file, add the code line inside the property tag

   ```xml
   <constraints>
       <constraint name="<prefix>:<property-name>" type="<groupId>.platformsample.<java-class-name>"/>
   </constraints>
   ```

7. Access the file `PROJECT_NAME-platform > src > main > resources > alfresco > module.PROJECT_NAME-platform > context > bootstrap-context.xml` and add the code line inside the `<property name="models"><list>{code}</list></property>` tag

   ```xml
   <value>alfresco/module/${project.artifactId}/model/<model-name>.xml</value>
   ```

8. Access the file `CMM_<model-name>_module.xml`, copy all content inside the `<configurations>` tag and paste it into `PROJECT_NAME-share > src > main > resources > share-config-custom.xml`
9. Add all document types created inside the `<types>` tag, and if you created aspects, add them inside the `<aspects>` tag

- Structure to add aspects:
   ```xml
   <subtype label="<document-type-name>" name="<model-prefix>:<type-name"/>
   ```
- Structure to add document types:
   ```xml
   <aspect label="<aspect-name>" name="<model-prefix>:<aspect-name"/>
   ```

10. Restart Alfresco
   
   ```bash
      sudo ./run.sh build_start
   ```
