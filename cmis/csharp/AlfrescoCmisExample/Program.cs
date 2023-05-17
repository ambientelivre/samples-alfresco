using System.IO;
using System;
using System.Collections.Generic;
using System.IO;
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

        // Cria uma sessão CMIS
        ISessionFactory sessionFactory = SessionFactory.NewInstance();
        ISession session = sessionFactory.GetRepositories(parameters)[0].CreateSession();

        Console.WriteLine("Conectado");
        Console.WriteLine();

        // // Cria um diretório no Alfresco
        // string parentFolderId = "ba186cb9-fa8a-4575-9dd3-1dc369d34649"; // ID da pasta pai
        // IFolder newFolder = CriaDiretorio(session, parentFolderId);

        // // Faz upload de documentos para o Alfresco a partir de uma pasta da máquina local
        // string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        // string directoryPath = Path.Combine(homeDirectory, "arquivos-para-alfresco");
        // string[] files = Directory.GetFiles(directoryPath);
        // //Console.WriteLine("Encontrei a pasta");

        // //Passa por cada conteúdo dentro da pasta local
        // foreach (string filePath in files)
        // {
        //     string fileName = Path.GetFileName(filePath);
        //     string mimeType = GetMimeType(fileName);

        //     UploadDocumento(session, newFolder.Id, filePath, mimeType, fileName);
        // }

        // Console.WriteLine("Upload feito.");
        // Console.WriteLine();

        // //Lista diretórios e conteúdos de cada diretório
        // ListarDiretorios(session, parentFolderId);

        // // Faz o download de conteúdos do Alfresco para a pasta local de downloads
        // string localDownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); // Pasta local de downloads
        // string alfrescoFolderPath = newFolder.Id;

        // DownloadDocumentos(session, localDownloadPath, alfrescoFolderPath);

        //Console.WriteLine("Download concluído.");

    }

    // static IFolder CriaDiretorio(ISession session, string parentFolderId)
    // {
    //     string name = DateTime.Now.ToString("yyyy-MM-dd");
    //     string newFolderName = ObterNomePasta(session, parentFolderId, name);

    //     IFolder? parentFolder = session.GetObject(parentFolderId) as IFolder;
    //     IDictionary<string, object> properties = new Dictionary<string, object>();
    //     properties[PropertyIds.ObjectTypeId] = "cmis:folder";
    //     properties[PropertyIds.Name] = newFolderName;

    //     IFolder newFolder = parentFolder.CreateFolder(properties);

    //     Console.WriteLine("Novo diretório criado:");
    //     Console.WriteLine("Nome: " + newFolder.Name);
    //     Console.WriteLine("ID: " + newFolder.Id);
    //     Console.WriteLine();

    //     return newFolder;
    // }
    // static string ObterNomePasta(ISession session, string parentFolderId, string name)
    // {
    //     IFolder parentFolder = session.GetObject(parentFolderId) as IFolder;

    //     // Obtém a lista de subdiretórios existentes no diretório pai
    //     IList<ICmisObject> children = parentFolder.GetChildren().ToList();

    //     // Filtra os subdiretórios que possuem o formato de nome esperado (yyyy-MM-dd)
    //     var filteredFolders = children.Where(child => child.BaseTypeId == BaseTypeId.CmisFolder &&
    //                                                   Regex.IsMatch(child.Name, $@"^{name}_\d+$"));

    //     // Ordena os subdiretórios pelo número iterativo
    //     var sortedFolders = filteredFolders.OrderBy(folder =>
    //     {
    //         string folderName = folder.Name;
    //         int iteration = int.Parse(Regex.Match(folderName, @"\d+$").Value);
    //         return iteration;
    //     });

    //     // Verifica o último número iterativo utilizado
    //     int lastIteration = sortedFolders.Any() ? int.Parse(Regex.Match(sortedFolders.Last().Name, @"\d+$").Value) : 1;

    //     // Incrementa o último número iterativo em 1
    //     int newIteration = lastIteration + 1;

    //     // Constrói o novo nome da pasta com a iteração
    //     string newFolderName = $"{name}_{newIteration}";

    //     return newFolderName;
    // }
    // static void UploadDocumento(ISession session, string parentFolderId, string filePath, string mimeType, string fileName)
    // {
    //     IFolder? parentFolder = session.GetObject(parentFolderId) as IFolder;
    //     IDictionary<string, object> properties = new Dictionary<string, object>();
    //     properties[PropertyIds.ObjectTypeId] = "cmis:document";
    //     properties[PropertyIds.Name] = fileName;

    //     using (FileStream stream = new FileStream(filePath, FileMode.Open))
    //     {
    //         IContentStream contentStream = new ContentStream
    //         {
    //             FileName = fileName,
    //             Length = stream.Length,
    //             MimeType = mimeType,
    //             Stream = stream
    //         };

    //         IDocument document = parentFolder.CreateDocument(properties, contentStream, VersioningState.Major);
    //     }
    // }
    // static string GetMimeType(string fileName)
    // {
    //     string extension = Path.GetExtension(fileName).ToLower();

    //     switch (extension)
    //     {
    //         case ".txt":
    //             return "text/plain";
    //         case ".pdf":
    //             return "application/pdf";
    //         case ".doc":
    //         case ".docx":
    //             return "application/msword";
    //         case ".xls":
    //         case ".xlsx":
    //             return "application/vnd.ms-excel";
    //         case ".ppt":
    //         case ".pptx":
    //             return "application/vnd.ms-powerpoint";
    //         case ".jpg":
    //         case ".jpeg":
    //             return "image/jpeg";
    //         case ".png":
    //             return "image/png";
    //         default:
    //             return "application/octet-stream";
    //     }
    // }
    // static void ListarDiretorios(ISession session, string folderId)
    // {
    //     IFolder parentFolder = session.GetObject(folderId) as IFolder;

    //     foreach (ICmisObject child in parentFolder.GetChildren())
    //     {
    //         if (child.BaseTypeId == BaseTypeId.CmisFolder)
    //         {
    //             IFolder subFolder = (IFolder)child;
    //             Console.WriteLine("Nome do diretório: " + subFolder.Name);
    //             Console.WriteLine("ID do diretório: " + subFolder.Id);
    //             Console.WriteLine();

    //             // Listar o conteúdo do subdiretório
    //             ListarConteudoDiretorio(session, subFolder.Id);

    //             // Chamada recursiva para listar subdiretórios aninhados
    //             ListarDiretorios(session, subFolder.Id);
    //         }
    //     }
    // }
    // static void ListarConteudoDiretorio(ISession session, string folderId)
    // {
    //     IFolder folder = session.GetObject(folderId) as IFolder;

    //     foreach (ICmisObject child in folder.GetChildren())
    //     {
    //         Console.WriteLine("     Nome do objeto: " + child.Name);
    //         Console.WriteLine("     ID do objeto: " + child.Id);
    //         Console.WriteLine("     Tipo do objeto: " + child.BaseTypeId);
    //         Console.WriteLine();
    //     }
    // }
    // static void DownloadDocumentos(ISession session, string localDownloadPath, string alfrescoFolderPath)
    // {
    //     IFolder folder = session.GetObject(alfrescoFolderPath) as IFolder;

    //     foreach (ICmisObject child in folder.GetChildren())
    //     {
    //         if (child.BaseTypeId == BaseTypeId.CmisDocument)
    //         {
    //             IDocument document = (IDocument)child;
    //             string documentName = document.Name;
    //             string documentId = document.Id;

    //             // Cria o caminho completo para o arquivo local de download
    //             string localFilePath = Path.Combine(localDownloadPath, documentName);

    //             if (File.Exists(localFilePath))
    //             {
    //                 // Se o arquivo já existe, renomeia o arquivo com uma lógica iterativa
    //                 int iteration = 1;
    //                 string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(documentName);
    //                 string fileExtension = Path.GetExtension(documentName);
    //                 string renamedFileName = $"{fileNameWithoutExtension}_{iteration}{fileExtension}";

    //                 while (File.Exists(Path.Combine(localDownloadPath, renamedFileName)))
    //                 {
    //                     iteration++;
    //                     renamedFileName = $"{fileNameWithoutExtension}_{iteration}{fileExtension}";
    //                 }

    //                 localFilePath = Path.Combine(localDownloadPath, renamedFileName);
    //             }

    //             // Faz o download do documento para o caminho local
    //             using (FileStream stream = new FileStream(localFilePath, FileMode.Create))
    //             {
    //                 IContentStream contentStream = document.GetContentStream();
    //                 contentStream.Stream.CopyTo(stream);
    //             }

    //             Console.WriteLine("Download realizado:");
    //             Console.WriteLine("Nome do documento: " + documentName);
    //             Console.WriteLine("Caminho local: " + localFilePath);
    //             Console.WriteLine();
    //         }
    //     }
    // }
}