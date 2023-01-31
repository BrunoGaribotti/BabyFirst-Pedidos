using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Negocio
{
    /// <summary>
    /// Interacción con el FTP
    /// </summary>
    class Ftp
    {
        //(B) dependiendo de si es el server de Grupo360 o BabyFirst
        private string host = "ftp://www.mundolove.com/";
        private string user = "###";
        private string pass = "###";
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 2048;

        public Ftp(string hostIP, string userName, string password) { host = hostIP; user = userName; pass = password; }

        /// <summary>
        /// Descargar archivos desde el FTP.
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <param name="localFile"></param>
        public void download(string remoteFile, string localFile)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                byte[] byteBuffer = new byte[bufferSize];
                int bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                try
                {
                    while (bytesRead > 0)
                    {
                        localFileStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = ftpStream.Read(byteBuffer, 0, bufferSize);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                localFileStream.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }

        /// <summary>
        /// Subir archivos al FTP
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <param name="localFile"></param>
        public void upload(string remoteFile, string localFile)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpStream = ftpRequest.GetRequestStream();
                FileStream localFileStream = new FileStream(localFile, FileMode.Open);
                byte[] byteBuffer = new byte[bufferSize];
                int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                try
                {
                    while (bytesSent != 0)
                    {
                        ftpStream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
                    }
                    localFileStream.Close();
                    ftpStream.Close();
                    ftpRequest = null;
                    File.Delete(localFile);       
                }
                catch (Exception ex) { 
                    Console.WriteLine(ex.ToString()); 
                }

                
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.ToString()); }
            return;
        }

        /// <summary>
        /// Eliminar archivos del FTP.
        /// </summary>
        /// <param name="deleteFile"></param>
        public void delete(string deleteFile)
        {
            try
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + deleteFile);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }

        /// <summary>
        /// Renombrar archivo del FTP.
        /// </summary>
        /// <param name="currentFileNameAndPath"></param>
        /// <param name="newFileName"></param>
        public void rename(string currentFileNameAndPath, string newFileName)
        {
            try
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + currentFileNameAndPath);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                ftpRequest.RenameTo = newFileName;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }

        /// <summary>
        /// Crear nuevo directorio en el FTP.
        /// </summary>
        /// <param name="newDirectory"></param>
        public void createDirectory(string newDirectory)
        {
            try
            {
                ftpRequest = (FtpWebRequest)WebRequest.Create(host + "/" + newDirectory);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            return;
        }

        /// <summary>
        /// Obtener la fecha del archivo del FTP.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string getFileCreatedDateTime(string fileName)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + fileName);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                StreamReader ftpReader = new StreamReader(ftpStream);
                string fileInfo = null;
                try { fileInfo = ftpReader.ReadToEnd(); }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;

                return fileInfo;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            /* Return an Empty string Array if an Exception Occurs */
            return "";
        }

        /// <summary>
        /// Obtener el peso de un archivo del FTP.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string getFileSize(string fileName)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + fileName);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                StreamReader ftpReader = new StreamReader(ftpStream);
                string fileInfo = null;
                try { while (ftpReader.Peek() != -1) { fileInfo = ftpReader.ReadToEnd(); } }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                return fileInfo;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            return "";
        }

        /// <summary>
        /// Muestra una lista (string) con los nombres de los archivos de un directorio.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public string[] directoryListSimple(string directory)
        {
            try
            {
                string uri = host;
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(new Uri(uri));

                //ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directory);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                StreamReader ftpReader = new StreamReader(ftpStream);
                string directoryRaw = null;
                try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                try { string[] directoryList = directoryRaw.Split("|".ToCharArray()); return directoryList; }
                catch (Exception ex) { 
                    Console.WriteLine(ex.ToString()); 
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            return new string[] { "" };
        }

        /// <summary>
        /// Lee el contenido de un directorio con detalles.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public string[] directoryListDetailed(string directory)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + directory);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpStream = ftpResponse.GetResponseStream();
                StreamReader ftpReader = new StreamReader(ftpStream);
                string directoryRaw = null;
                try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                ftpReader.Close();
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
                
                try { string[] directoryList = directoryRaw.Split("|".ToCharArray()); return directoryList; }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            return new string[] { "" };
        }
    }
}
