using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace S21eimagesexport.Model
{

	[XmlRoot(ElementName = "global")]
	public class Global
	{
		[XmlElement(ElementName = "role")]
		public List<string> Role { get; set; }
	}

	[XmlRoot(ElementName = "image")]
	public class Image
	{
		[XmlElement(ElementName = "global")]
		public Global Global { get; set; }
		[XmlAttribute(AttributeName = "file_or_url")]
		public string FileOrUrl { get; set; }
	}

	[XmlRoot(ElementName = "images")]
	public class Images
	{
		[XmlElement(ElementName = "image")]
		public List<Image> Image { get; set; }
	}

	[XmlRoot(ElementName = "simple")]
	public class Simple
	{
		[XmlElement(ElementName = "images")]
		public Images Images { get; set; }
		[XmlAttribute(AttributeName = "sku")]
		public string Sku { get; set; }
	}

	[XmlRoot(ElementName = "import")]
	public class Import
	{
		[XmlElement(ElementName = "simple")]
		public Simple Simple { get; set; }
	}

}

