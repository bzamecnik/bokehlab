using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;


namespace Meshomatic {
	public class ColladaLoader {
		string SchemaFile = "collada_schema_1_4";
		public ColladaLoader () {
		}
		public ColladaLoader(string schemafile) {
			SchemaFile = schemafile;
		}
		
		public MeshData LoadFile(string file) {
			using(FileStream s = File.Open(file, FileMode.Open)) {
				return LoadStream(s);
			}
		}
		
		private static void ValidationEventHandler(object sender, ValidationEventArgs args) {
			Console.WriteLine("sender {0}, args {1}", sender, args);
		}
		
		public MeshData LoadStream(Stream stream) {
			XmlReaderSettings settings = new XmlReaderSettings();
			Console.WriteLine("Now loading schemas, this will be slow...");
			// Apparently .NET doesn't have the schema schema, so we have to have our own copy!  :D
			settings.Schemas.Add("http://www.w3.org/XML/1998/namespace", "xml.xsd");
			settings.Schemas.Add("http://www.collada.org/2005/11/COLLADASchema", "collada_schema_1_4");
			Console.WriteLine("Done!  Wasn't that horrible?");
			//Console.WriteLine("Gods, now we have to actually validate the Collada file...");
			settings.ValidationType = ValidationType.Schema;
			XmlReader reader = XmlReader.Create(stream, settings);
			XmlDocument doc = new XmlDocument();
			doc.Load(reader);
			ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);
			doc.Validate(eventHandler);
			
			//XmlNode geom = doc.SelectNodes("/COLLADA/library_geometries/mesh");

			return null;
			/*
			XmlReader textReader = new XmlTextReader(stream);
			 textReader.Read();

            // If the node has value

            while (textReader.Read())
            {
                // Move to fist element
                textReader.MoveToElement();
                Console.WriteLine("XmlTextReader Properties Test");
                Console.WriteLine("===================");
                // Read this element's properties and display them on console
                Console.WriteLine("Name:" + textReader.Name);
                Console.WriteLine("Base URI:" + textReader.BaseURI);
                Console.WriteLine("Local Name:" + textReader.LocalName);
                Console.WriteLine("Attribute Count:" + textReader.AttributeCount.ToString());
                Console.WriteLine("Depth:" + textReader.Depth.ToString());
                //Console.WriteLine("Line Number:" + textReader.LineNumber.ToString());
                Console.WriteLine("Node Type:" + textReader.NodeType.ToString());
                Console.WriteLine("Attribute Count:" + textReader.Value.ToString());
            }
			
			XmlDocument d = new XmlDocument();
			XmlSchema s = new XmlSchema();
			
			return null;
			*/
		}
	}
}
