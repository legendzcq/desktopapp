using System;
using System.Net;
using System.Xml.Linq;

namespace CdelService.Remote
{
    public abstract class RemoteBase
    {
        private const string Xmlprefix = @"<?xml version=""1.0"" encoding=""utf-8"" ?>";

        protected string FixXmlHead(string xml)
        {
            return Xmlprefix + xml;
        }
    }
}
