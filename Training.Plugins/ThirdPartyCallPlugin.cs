﻿using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PluginsForD365
{
    //<Data> <key>8e0700a1477ca8ff8a388f28b792e7bf</key> <url>http://apilayer.net/api/live</url> </Data>
    public class ThirdPartyCallPlugin : IPlugin
    {
        private readonly string _configSettings;
        private readonly string _key;
        private readonly string _url;

        public ThirdPartyCallPlugin(string configurationSettings) { 
        
            if(!string.IsNullOrWhiteSpace(configurationSettings))
            {
                try
                {
                    _configSettings = configurationSettings;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(_configSettings);
                    _key = doc.SelectSingleNode("Data/key").InnerText;
                    _url = doc.SelectSingleNode("Data/url").InnerText;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("configs not found" + ex.InnerException.ToString());                
                }
            }
        }
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            var ent = (Entity)context.InputParameters["Target"];

            HttpClient client = new HttpClient();
            var query = $"access_key={_key}&currencies=EUR,GBP,CAD,PLN&source=USD&format=1";

            var request = (HttpWebRequest)WebRequest.Create(_url + "?" + query);
            request.Method = "GET";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            var content = string.Empty;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }
            var parsedResponseJSON = JObject.Parse(content);
            var CurrenciesJSON = parsedResponseJSON["quotes"];

            var parsedCurrenciesJSON = JObject.Parse(CurrenciesJSON.ToString());
            var USDTOEUR = parsedCurrenciesJSON["USDEUR"];

            //add a note
            Entity Note = new Entity("annotation");
            Note["objectid"] = new EntityReference("contact", ent.Id);
            Note["objecttypecode"] = 2;
            Note["subject"] = "Latest Currency Exchange Data";
            Note["notetext"] = "USDEUR : " + USDTOEUR + " " + _url + "?" + query + " " + content;
            service.Create(Note);
        }
    }
}
