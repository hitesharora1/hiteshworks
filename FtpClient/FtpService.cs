using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using System.Threading.Tasks;

namespace FtpClient
{
    public class FtpService
    {
        public   class FtpCredentials
        {
            public  string Url { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }

        public string UploadFile(string folder, string filename, byte[] fileContents, FtpCredentials credentaials)
        {
            if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(filename) && !string.IsNullOrEmpty(credentaials.password) && !string.IsNullOrEmpty(credentaials.Url) && !string.IsNullOrEmpty(credentaials.username))
            {
                

                try
                {
                    List<string> existingfiles = ViewFile(folder, credentaials).ToList();
                    List<string> listsamefile = existingfiles.Where(a => a.Contains(filename)).ToList();
                    string responssse;
                    if (listsamefile.Count == 0)
                    {
                        string ftp = credentaials.Url + folder + "/" + filename;
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp);
                        //Enter FTP Server credentials.
                        request.Method = WebRequestMethods.Ftp.UploadFile;
                        request.Credentials = new NetworkCredential(credentaials.username, credentaials.password);
                        request.UsePassive = true;
                        request.UseBinary = true;
                        request.EnableSsl = false;
                        request.ContentLength = fileContents.Length;
                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(fileContents, 0, fileContents.Length);
                        requestStream.Close();
                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                        Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
                        response.Close();
                        responssse = response.StatusDescription;
                    }
                    else
                    {
                        responssse = "File Already Exists";
                    }
                    return responssse;

                }
                catch (Exception exx)
                {
                    return exx.ToString();
                }
            }
            else
            {
                return "Please enter all the  credentials";
            }
        }


        public FtpWebResponse DowloadFile(string folder, string filename, FtpCredentials credentaials)
        {

            if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(filename) && !string.IsNullOrEmpty(credentaials.password) && !string.IsNullOrEmpty(credentaials.Url) && !string.IsNullOrEmpty(credentaials.username))
            {
                try
                {
                    //FTP Server URL.
                    string ftp = credentaials.Url + folder + "/" + filename;
                    //Create FTP Request.
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;

                    //Enter FTP Server credentials.
                    request.Credentials = new NetworkCredential(credentaials.username, credentaials.password);
                    request.UsePassive = true;
                    request.UseBinary = true;
                    request.EnableSsl = false;

                    //Fetch the Response and read it into a MemoryStream object.
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    return response;
                    //using (MemoryStream stream = new MemoryStream())
                    //{
                    //    //Download the File.
                    //    response.GetResponseStream().CopyTo(stream);
                    //    Response.AddHeader("content-disposition", "attachment;filename=" + filename);
                    //    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    //    Response.BinaryWrite(stream.ToArray());
                    //    Response.End();
                    //}
                }
                catch (WebException ex)
                {
                    throw new Exception((ex.Response as FtpWebResponse).StatusDescription);

                }
            }
            else
            {
                return null;
            }
        }



        public string[] ViewFile(string folder, FtpCredentials credentaials)
        {

            string ftp = credentaials.Url + folder;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            //Enter FTP Server credentials.
            request.Credentials = new NetworkCredential(credentaials.username, credentaials.password);
            request.UsePassive = true;
            request.UseBinary = true;
            request.EnableSsl = false;
            List<string> list = new List<string>();
            //Fetch the Response and read it into a MemoryStream object.

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream, true))
                    {
                        while (!reader.EndOfStream)
                        {
                            list.Add(reader.ReadLine());
                        }
                    }
                }
            }

            return list.ToArray();
        }

        public string DeleteFile(string folder, string filename, FtpCredentials credentaials)
        {
            if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(filename) && !string.IsNullOrEmpty(credentaials.password) && !string.IsNullOrEmpty(credentaials.Url) && !string.IsNullOrEmpty(credentaials.username))
            {

                try
                {
                    string ftp = credentaials.Url + folder + "/" + filename;

                    List<string> existingfiles = ViewFile(folder, credentaials).ToList();
                    List<string> listsamefile = existingfiles.Where(a => a.Contains(filename)).ToList();
                    string responssse;
                    if (listsamefile.Count > 0)
                    {
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp);
                        request.Method = WebRequestMethods.Ftp.DeleteFile;
                        request.Credentials = new NetworkCredential(credentaials.username, credentaials.password);
                        request.UsePassive = true;
                        request.UseBinary = true;
                        request.EnableSsl = false;
                        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                        {
                            responssse = response.StatusDescription;
                        }
                    }
                    else
                    {
                        responssse = "No file found with this name";
                    }
                    return responssse;
                }
                catch (Exception eexcx)
                {
                    return eexcx.ToString();
                }
            }
            else
            {
                return "Please enter all the  credentials";
            }
        }
    }
}
