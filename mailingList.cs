using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using System.Configuration;
//
namespace ekoal.lib.mailgun
{
    public class mailingList
    {
        //APP CONFIG
        //API KEY
        //MAILLIST NAME
        //MAILLIST DESC
        //DOMAIN
        //
        private static RestClient getRestClient()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                    new HttpBasicAuthenticator("api", ConfigurationManager.AppSettings.Get("mg.akey"));
            return client;
        }
        //
        public static IRestResponse create()
        {
            String list = ConfigurationManager.AppSettings.Get("mg.list");
            String listDesc = ConfigurationManager.AppSettings.Get("mg.listdesc");

            RestClient client = getRestClient();
            RestRequest request = new RestRequest();
            request.Resource = "lists";
            request.AddParameter("address", list);
            request.AddParameter("description", listDesc);
            request.Method = Method.POST;
            //request.Resource = "domains";
            //request.AddParameter("skip", 0);
            //request.AddParameter("limit", 3);
            return client.Execute(request);
        }
        public static IRestResponse addListMember(String email)
        {
            String list = ConfigurationManager.AppSettings.Get("mg.list");
            return addListMember(email,list);
        }
        public static IRestResponse addListMember(String email, String list)
        {
            RestClient client = getRestClient();
            RestRequest request = new RestRequest();
            request.Resource = "lists/{list}/members";
            request.AddParameter("list", list, ParameterType.UrlSegment);
            request.AddParameter("address", email);
            request.AddParameter("subscribed", true);
            request.Method = Method.POST;
            return client.Execute(request);
        }
        public static IRestResponse removeListMember(String email)
        {
            String list = ConfigurationManager.AppSettings.Get("mg.list");
            return removeListMember(email,list);
        }
        public static IRestResponse removeListMember(String email, String list)
        {
            RestClient client = getRestClient();
            RestRequest request = new RestRequest();
            request.Resource = "lists/{list}/members/{member}";
            request.AddParameter("list", list, ParameterType.UrlSegment);
            request.AddParameter("member", email, ParameterType.UrlSegment);
            request.Method = Method.DELETE;
            IRestResponse irr = client.Execute(request);
            return irr;
        }
        public static IRestResponse sendMessage(String body, String subj, String attachPath, String to)
        {
            String list = ConfigurationManager.AppSettings.Get("mg.list");
            String from = ConfigurationManager.AppSettings.Get("mg.from");
            String domain = ConfigurationManager.AppSettings.Get("mg.domain");
            RestClient client = getRestClient();
            RestRequest request = new RestRequest();

            request.AddParameter("domain",
                                 domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", from);
            request.AddParameter("to", to);
            request.AddParameter("subject", subj);
            request.AddParameter("html", body);
            FileInfo fi = null;
            var name = "";
            if (attachPath.Trim() != "")
            {
                fi = new FileInfo(attachPath);
                name = (subj.Trim() != "" ? subj + fi.Extension : fi.Name + fi.Extension);
                name = name.Replace("é", "e");
                name = name.Replace("è", "e");
                name = Regex.Replace(name.Trim(), "[^A-Za-z0-9_. ]+", "");
                name = name.Replace(" ", "-");
                File.Copy(attachPath, fi.DirectoryName + "\\" + name);
                request.AddFile("attachment", fi.DirectoryName + "\\" + name);
            }
            request.Method = Method.POST;
            IRestResponse irr = client.Execute(request);

            if (attachPath.Trim() != "")
            {
                File.Delete(fi.DirectoryName + "\\" + name);
            }
            
            return irr;
        }
    }
}
