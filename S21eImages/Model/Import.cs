using System.Collections.Generic;
using System.Xml.Serialization;

namespace S21eImages.Model
{
    [XmlRoot(ElementName = "gallery_information")]
	public class GalleryInformation
	{
		[XmlAttribute(AttributeName = "label")]
		public string Label { get; set; }
		[XmlAttribute(AttributeName = "position")]
		public string Position { get; set; }
		[XmlAttribute(AttributeName = "enabled")]
		public string Enabled { get; set; }
	}

	[XmlRoot(ElementName = "global")]
	public class Global
	{
		[XmlElement(ElementName = "role")]
		public List<string> Role { get; set; }
		[XmlElement(ElementName = "gallery_information")]
		public GalleryInformation GalleryInformation { get; set; }
	}

	[XmlRoot(ElementName = "image")]
	public class Image
	{
		[XmlElement(ElementName = "global")]
		public Global Global { get; set; }
		[XmlAttribute(AttributeName = "file_or_url")]
		public string FileOrUrl { get; set; }
		[XmlElement(ElementName = "gallery_information")]
		public GalleryInformation GalleryInformation { get; set; }
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
		public List<Simple> Simple { get; set; }
	}
}
