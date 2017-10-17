using System;
using System.Net;

namespace CoreLib
{
    public class CookedWebClient : WebClient
    {
        readonly CookieContainer _cookies = new CookieContainer();

        public CookieContainer Cookies { get { return _cookies; } }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);

            if(request.GetType() == typeof(HttpWebRequest))
              ((HttpWebRequest)request).CookieContainer = _cookies;

            return request;
        }
    }
}

