﻿using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.Xml;

namespace MusicCollection.WebServices.Amazon.WCF
{
	public class AmazonHeader : MessageHeader {
		private string	name;
		private string	value;

		public AmazonHeader(string name, string value) {
			this.name	= name;
			this.value	= value;
		}

		public override string Name			{ get { return name; } }
        //public override string Namespace { get { return ConfigurationManager.AppSettings["amazonSecurityNamespace"]; } }
        public override string Namespace { get { return "http://security.amazonaws.com/doc/2007-01-01/"; } }

		protected override void OnWriteHeaderContents(XmlDictionaryWriter xmlDictionaryWriter, MessageVersion messageVersion) {
			xmlDictionaryWriter.WriteString(value);
		}
	}
}
