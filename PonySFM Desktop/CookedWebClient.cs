﻿using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Desktop
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

