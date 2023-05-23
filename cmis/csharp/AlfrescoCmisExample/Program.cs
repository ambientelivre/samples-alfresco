using System.Text.RegularExpressions;
using DotCMIS;
using DotCMIS.Client;
using DotCMIS.Client.Impl;
using DotCMIS.Data;
using DotCMIS.Data.Impl;
using DotCMIS.Enums;

class Program
{
    static void Main(string[] args)
    {
        // Conexão CMIS
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters[SessionParameter.BindingType] = BindingType.AtomPub;
        parameters[SessionParameter.AtomPubUrl] = "http://localhost/alfresco/api/-default-/public/cmis/versions/1.0/atom";

        // Autenticação
        parameters[SessionParameter.User] = "admin";
        parameters[SessionParameter.Password] = "admin";

        // Cria sessão CMIS
        ISessionFactory sessionFactory = SessionFactory.NewInstance();
        ISession session = sessionFactory.GetRepositories(parameters)[0].CreateSession();

        Console.WriteLine("Conectado");
        Console.WriteLine();

        // Cria diretório no Alfresco
        string parentFolderId = "ba186cb9-fa8a-4575-9dd3-1dc369d34649"; // ID da pasta pai
        IFolder newFolder = CriaDiretorio(session, parentFolderId);

        // Faz upload pro Alfresco a partir de uma pasta da máquina local
        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string directoryPath = Path.Combine(homeDirectory, "arquivos-para-alfresco");
        string[] files = Directory.GetFiles(directoryPath);
        //Console.WriteLine("Encontrei a pasta");

        //Passa por cada conteúdo dentro da pasta local
        foreach (string filePath in files)
        {
            string fileName = Path.GetFileName(filePath);
            string mimeType = GetMimeType(fileName);

            string upload = UploadDocumento(session, newFolder.Id, filePath, mimeType, fileName);

            string documentId = upload;
            Console.WriteLine(documentId);

            // Define os metadados customizados para o documento carregado
            string especie = "Processo de Pagamento";
            string descricao = "Este processo é advindo da unidade x e referente a pagamento";

            DefinirMetadadosAtos(session, documentId, especie, descricao);
        }

        Console.WriteLine("Upload feito.");
        Console.WriteLine();


        //Lista diretórios e conteúdos de cada diretório
        ListarDiretorios(session, parentFolderId);

        // // Mostra Metadados de documento
        // string documentId2 = "717c7747-dbf1-4cb6-b9fc-7a1bc85b4f1d"; // ID do documento no Alfresco
        // MostrarMetadadosDocumentoEspecífico(session, documentId2);

        // Mostra Metadados de todos os documentos de uma pasta
        MostrarMetadadosPasta(session, parentFolderId);

        // Faz o download de conteúdos do Alfresco pra pasta local "Downloads"
        string localDownloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        string alfrescoFolderPath = newFolder.Id;

        DownloadDocumentos(session, localDownloadPath, alfrescoFolderPath);

        Console.WriteLine("Download concluído.");
    }
    static IFolder CriaDiretorio(ISession session, string parentFolderId)
    {
        // Obtém nome único para a nova pasta dentro do diretório pai
        string name = DateTime.Now.ToString("yyyy-MM-dd");
        string newFolderName = ObterNomePasta(session, parentFolderId, name);

        // Obtém o objeto IFolder do diretório pai
        IFolder? parentFolder = session.GetObject(parentFolderId) as IFolder;
        IDictionary<string, object> properties = new Dictionary<string, object>();
        properties[PropertyIds.ObjectTypeId] = "cmis:folder";
        properties[PropertyIds.Name] = newFolderName;

        // Cria a nova pasta dentro do diretório pai
        IFolder newFolder = parentFolder.CreateFolder(properties);

        Console.WriteLine("Novo diretório criado:");
        Console.WriteLine("Nome: " + newFolder.Name);
        Console.WriteLine("ID: " + newFolder.Id);
        Console.WriteLine();

        return newFolder;
    }
    static string ObterNomePasta(ISession session, string parentFolderId, string name)
    {
        IFolder parentFolder = session.GetObject(parentFolderId) as IFolder;

        // Obtém a lista de subdiretórios existentes no diretório pai
        IList<ICmisObject> children = parentFolder.GetChildren().ToList();

        // Filtra os subdiretórios que possuem o formato de nome esperado (yyyy-MM-dd)
        var filteredFolders = children.Where(child => child.BaseTypeId == BaseTypeId.CmisFolder && Regex.IsMatch(child.Name, $@"^{name}_\d+$"));

        // Ordena os subdiretórios pelo número iterativo
        var sortedFolders = filteredFolders.OrderBy(folder =>
        {
            string folderName = folder.Name;
            int iteration = int.Parse(Regex.Match(folderName, @"\d+$").Value);
            return iteration;
        });

        // Verifica o último número iterativo utilizado e incrementa mais um
        int lastIteration = sortedFolders.Any() ? int.Parse(Regex.Match(sortedFolders.Last().Name, @"\d+$").Value) : 1;
        int newIteration = lastIteration + 1;
        string newFolderName = $"{name}_{newIteration}";

        return newFolderName;
    }
    static string UploadDocumento(ISession session, string parentFolderId, string filePath, string mimeType, string fileName)
    {
        IFolder parentFolder = session.GetObject(parentFolderId) as IFolder;

        IDictionary<string, object> properties = new Dictionary<string, object>
        {
            [PropertyIds.ObjectTypeId] = "cmis:document",
            [PropertyIds.Name] = fileName
        };

        // Abre um fluxo de arquivo para ler o conteúdo do arquivo a ser carregado
        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            // Cria um objeto IContentStream para o conteúdo do arquivo
            IContentStream contentStream = new ContentStream
            {
                //Define nome, tamanho, mimetype e fluxo de cada aqruivo
                FileName = fileName,
                Length = stream.Length,
                MimeType = mimeType,
                Stream = stream
            };

            // "Cria" arquivo dentro do repositório
            IDocument document = parentFolder.CreateDocument(properties, contentStream, VersioningState.Major);

            // Obtém o ID sem a informação de versão do documento
            string documentIdWithVersion = document.Id;
            string documentId = documentIdWithVersion.Split(';')[0];

            return documentId;
        }
    }
    static string GetMimeType(string fileName)
    {
        // Obtém a extensão do arquivo 
        string extension = Path.GetExtension(fileName).ToLower();

        // Verifica a extensão do arquivo e retorna o mimetype correspondente
        switch (extension)
        {
            case ".txt":
                return "text/plain";
            case ".pdf":
                return "application/pdf";
            case ".doc":
            case ".docx":
                return "application/msword";
            case ".xls":
            case ".xlsx":
                return "application/vnd.ms-excel";
            case ".ppt":
            case ".pptx":
                return "application/vnd.ms-powerpoint";
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".png":
                return "image/png";
            default:
                return "application/octet-stream";
        }
    }
    static void ListarDiretorios(ISession session, string folderId)
    {
        // Obtém o objeto de pasta pai
        IFolder parentFolder = session.GetObject(folderId) as IFolder;

        // Itera sobre os objetos filhos da pasta pai
        foreach (ICmisObject child in parentFolder.GetChildren())
        {
            // Verifica se o objeto filho é uma pasta
            if (child.BaseTypeId == BaseTypeId.CmisFolder)
            {
                IFolder subFolder = (IFolder)child;
                Console.WriteLine("Nome do diretório: " + subFolder.Name);
                Console.WriteLine("ID do diretório: " + subFolder.Id);
                Console.WriteLine();

                // Lista o conteúdo do subdiretório
                ListarConteudoDiretorio(session, subFolder.Id);

                // Faz uma chamada recursiva para listar subdiretórios aninhados
                ListarDiretorios(session, subFolder.Id);
            }
        }
    }
    static void ListarConteudoDiretorio(ISession session, string folderId)
    {
        // Obtem o objeto de pasta com base no ID fornecido
        IFolder folder = session.GetObject(folderId) as IFolder;

        // Itera sobre os objetos filhos da pasta
        foreach (ICmisObject child in folder.GetChildren())
        {
            Console.WriteLine("     Nome do objeto: " + child.Name);
            Console.WriteLine("     ID do objeto: " + child.Id);
            Console.WriteLine("     Tipo do objeto: " + child.BaseTypeId);
            Console.WriteLine();
        }
    }
    static void MostrarMetadadosPasta(ISession session, string parentFolderId)
    {
        IFolder folder = session.GetObject(parentFolderId) as IFolder;

        // Chama o método para listar os metadados dos arquivos na pasta
        ListarMetadadosArquivos(folder);
    }
    static void ListarMetadadosArquivos(IFolder folder)
    {
        // Percorre todos os objetos (arquivos e subpastas) dentro da pasta fornecida
        foreach (ICmisObject child in folder.GetChildren())
        {
            // Verifica se é do tipo pasta
            if (child.BaseTypeId == BaseTypeId.CmisFolder)
            {
                // Chamada recursiva para as subpastas
                ListarMetadadosArquivos((IFolder)child);
            }
            // Verifica se é documento
            else if (child.BaseTypeId == BaseTypeId.CmisDocument)
            {
                // Chamada recursiva do método que mostra metadados do documento
                MostrarMetadadosDocumento(child);
            }
        }
    }
    static void MostrarMetadadosDocumento(ICmisObject document)
    {
        Console.WriteLine("Nome do documento: " + document.Name);

        // Percorre todas as propriedades do documento
        foreach (var property in document.Properties)
        {
            Console.WriteLine(property.QueryName + ": " + property.Value);
        }

        Console.WriteLine();
    }

    // static void MostrarMetadadosDocumentoEspecífico(ISession session, string documentId2)
    // {
    //     // Obtém o objeto de documento pelo ID
    //     ICmisObject document = session.GetObject(documentId2);
    //
    //    // Percorre todas as propriedades do documento e exibe seus metadados
    //     foreach (var property in document.Properties)
    //     {
    //         Console.WriteLine(property.QueryName + ": " + property.Value);
    //     }
    // }
    static void DefinirMetadadosAtos(ISession session, string documentId, string especie, string descricao)
    {
        IDocument document = session.GetObject(documentId) as IDocument;

        // Cria um dicionário de propriedades para definir os metadados
        IDictionary<string, object> properties = new Dictionary<string, object>();

        properties["atos:docEspecie"] = especie;
        properties["atos:docDescricao"] = descricao;

        // Atualiza as propriedades do documento
        document.UpdateProperties(properties);

        Console.WriteLine("Metadados customizados do documento atualizados com sucesso!");
    }
    static void DownloadDocumentos(ISession session, string localDownloadPath, string alfrescoFolderPath)
    {
        IFolder folder = session.GetObject(alfrescoFolderPath) as IFolder;

        // Itera sobre cada documento do repositório
        foreach (ICmisObject child in folder.GetChildren())
        {
            //Verifica se é documento e não pasta
            if (child.BaseTypeId == BaseTypeId.CmisDocument)
            {
                // Converte pra documento pro momento do download
                IDocument document = (IDocument)child;
                string documentName = document.Name;
                string documentId = document.Id;

                // Cria o caminho completo para o arquivo local de download
                string localFilePath = Path.Combine(localDownloadPath, documentName);

                // Verifica se o arquivo já existe localmente
                if (File.Exists(localFilePath))
                {
                    // Se já existir, renomeia o arquivo com uma lógica iterativa
                    int iteration = 1;
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(documentName);
                    string fileExtension = Path.GetExtension(documentName);
                    string renamedFileName = $"{fileNameWithoutExtension}_{iteration}{fileExtension}";

                    while (File.Exists(Path.Combine(localDownloadPath, renamedFileName)))
                    {
                        iteration++;
                        renamedFileName = $"{fileNameWithoutExtension}_{iteration}{fileExtension}";
                    }

                    // Cria o caminho completo para o arquivo local de download, combinando o diretório de download local com o nome do documento
                    localFilePath = Path.Combine(localDownloadPath, renamedFileName);
                }

                // Faz o download do documento para o caminho local
                using (FileStream stream = new FileStream(localFilePath, FileMode.Create))
                {
                    // Obtém o fluxo de conteúdo do documento no Alfresco e copia pro fluxo do arquivo local
                    IContentStream contentStream = document.GetContentStream();
                    contentStream.Stream.CopyTo(stream);
                }

                Console.WriteLine("Download realizado:");
                Console.WriteLine("Nome do documento: " + documentName);
                Console.WriteLine("Caminho local: " + localFilePath);
                Console.WriteLine();
            }
        }
    }
}