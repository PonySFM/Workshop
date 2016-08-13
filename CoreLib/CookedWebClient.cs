using System;
using System.Net;

namespace CoreLib
{
    public class CookedWebClient : WebClient
    {
        CookieContainer cookies = new CookieContainer();

        public CookieContainer Cookies { get { return cookies; } }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            if(request.GetType() == typeof(HttpWebRequest))
              ((HttpWebRequest)request).CookieContainer = cookies;

            return request;
        }
    }
}

