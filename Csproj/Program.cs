using System;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Csproj
{
	class MainClass
	{
		static readonly XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";

		public static void Main (string[] args)
		{
//			string path = "/Users/rzaitov/Documents/Apps/A_Xamarin/monotouch-samples/AirLocate/AirLocate/AirLocate.csproj";
			string path = args [0];
			Console.WriteLine ("path={0}", path);

			XDocument doc = XDocument.Load (path);
			IEnumerable<XElement> propGroups = GetPropertyGroups (doc);
			Fix (propGroups);
			Save (doc, path);
		}

		static IEnumerable<XElement> GetPropertyGroups(XDocument document)
		{
			XElement project = document.Root;
			return project.Elements().Where(e => e.Name.LocalName == "PropertyGroup");
		}

		static void Save(XDocument document, string path)
		{
			XmlWriterSettings ws = new XmlWriterSettings ();
			ws.Indent = true;
			ws.IndentChars = "  ";
			ws.NewLineChars = Environment.NewLine;
			ws.Encoding = new UTF8Encoding (false);

			using (XmlWriter writer = XmlWriter.Create (path, ws))
				document.Save (writer);
		}

		static void Fix(IEnumerable<XElement> propertyGroups)
		{
			foreach (XElement propertyGroup in propertyGroups)
				Fix (propertyGroup);
		}

		static void Fix(XElement propertyGroup)
		{
			var attributes = propertyGroup.Attributes ();
			var condition = attributes.FirstOrDefault (a => a.Name == "Condition");
			if (condition != null)
				FixArch (propertyGroup, condition);
		}

		static void FixArch(XElement propertyGroup, XAttribute condition)
		{
			string val = condition.Value;

			if (val.Contains ("Debug|iPhoneSimulator")
			    || val.Contains ("Release|iPhoneSimulator"))
				SetArch (propertyGroup, "i386");
			else if (val.Contains ("Debug|iPhone"))
				SetArch (propertyGroup, "ARMv7");
			else if (val.Contains ("Release|iPhone")
			         || val.Contains ("Ad-Hoc|iPhone")
			         || val.Contains ("AppStore|iPhone"))
				SetArch (propertyGroup, "ARMv7, ARM64");
			else
				Console.WriteLine ("unexpected condition = {0}", val);
		}

		static void SetArch(XElement propertyGroup, string arch)
		{
			var archElem = propertyGroup.Elements ().FirstOrDefault (e => e.Name.LocalName == "MtouchArch");

			if (archElem != null)
				archElem.Value = arch;
			else
				propertyGroup.Add (new XElement (ns + "MtouchArch", arch));
		}
	}
}
