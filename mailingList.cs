using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

//
namespace ekoal.lib.mailgun
{
    public class mailingList
    {
        public IRestResponse create()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                    new HttpBasicAuthenticator("api",
                                               "key-a7d9a1ee33b14b7ee20b96cef1da25f5");
            RestRequest request = new RestRequest();
            request.Resource = "lists";
            request.AddParameter("address", "devs@ekoal.re");
            request.AddParameter("description", "Mailgun developers list");
            request.Method = Method.POST;
            //request.Resource = "domains";
            //request.AddParameter("skip", 0);
            //request.AddParameter("limit", 3);
            return client.Execute(request);
        }
        public void get()
        { 
        }
    }
}
