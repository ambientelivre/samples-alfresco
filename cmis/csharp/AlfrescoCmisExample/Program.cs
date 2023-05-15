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

        // Cria um diretório no Alfresco
        string parentFolderId = "ba186cb9-fa8a-4575-9dd3-1dc369d34649"; // ID da pasta pai
        IFolder newFolder = CriaDiretorio(session, parentFolderId);

        // Faz upload de documentos para o Alfresco a partir de uma pasta da máquina local
        string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string directoryPath = Path.Combine(homeDirectory, "arquivos-para-alfresco");
        string[] files = Directory.GetFiles(directoryPath);
        //Console.WriteLine("Encontrei a pasta");

        //Passa por cada arquivo dentro da pasta local
        foreach (string filePath in files)
        {
            string fileName = Path.GetFileName(filePath);
            string mimeType = GetMimeType(fileName);

            UploadDocument(session, newFolder.Id, filePath, mimeType, fileName);
        }

        Console.WriteLine("Upload feito.");
    }
    static IFolder CriaDiretorio(ISession session, string parentFolderId)
    {

        string name = DateTime.Now.ToString("yyyy-MM-dd");
        string newFolderName = name + "_" + GenerateRandomNumber();

        int GenerateRandomNumber()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }

        IFolder? parentFolder = session.GetObject(parentFolderId) as IFolder;
        IDictionary<string, object> properties = new Dictionary<string, object>();
        properties[PropertyIds.ObjectTypeId] = "cmis:folder";
        properties[PropertyIds.Name] = newFolderName;

        IFolder newFolder = parentFolder.CreateFolder(properties);

        Console.WriteLine("Novo diretório criado:");
        Console.WriteLine("Nome: " + newFolder.Name);
        Console.WriteLine("ID: " + newFolder.Id);

        return newFolder;
    }
    static void UploadDocument(ISession session, string parentFolderId, string filePath, string mimeType, string fileName)
    {
        IFolder? parentFolder = session.GetObject(parentFolderId) as IFolder;
        IDictionary<string, object> properties = new Dictionary<string, object>();
        properties[PropertyIds.ObjectTypeId] = "cmis:document";
        properties[PropertyIds.Name] = fileName;

        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            IContentStream contentStream = new ContentStream
            {
                FileName = fileName,
                Length = stream.Length,
                MimeType = mimeType,
                Stream = stream
            };

            IDocument document = parentFolder.CreateDocument(properties, contentStream, VersioningState.Major);

            // Adicionar o aspecto personalizado
            string aspectName = "my:customAspect";
            document.AddAspect(aspectName);
        }
    }
    static string GetMimeType(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();

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

}