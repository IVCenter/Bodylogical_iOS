using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

public class Localization {
    [XmlRoot("Localization")]
    public class LocalizationGroup {
        [XmlArray("General")]
        [XmlArrayItem("Item", Type = typeof(LocalizationItem))]
        public List<LocalizationItem> general;
        [XmlArray("Buttons")]
        [XmlArrayItem("Item", Type = typeof(LocalizationItem))]
        public List<LocalizationItem> buttons;
        [XmlArray("Legends")]
        [XmlArrayItem("Item", Type = typeof(LocalizationItem))]
        public List<LocalizationItem> legends;
        [XmlArray("Archetypes")]
        [XmlArrayItem("Item", Type = typeof(LocalizationItem))]
        public List<LocalizationItem> archetypes;
        [XmlArray("Instructions")]
        [XmlArrayItem("Item", Type = typeof(LocalizationItem))]
        public List<LocalizationItem> instructions;
        [XmlArray("Tutorials")]
        [XmlArrayItem("Item", Type = typeof(LocalizationItem))]
        public List<LocalizationItem> tutorials;
    }

    [XmlRoot("Item")]
    public class LocalizationItem {
        [XmlAttribute]
        public string id;
        [XmlText]
        public string text;
    }

    public Language CurrLanguage { get; }
    public Dictionary<string, string> General { get; }
    public Dictionary<string, string> Buttons { get; }
    public Dictionary<string, string> Legends { get; }
    public Dictionary<string, string> Archetypes { get; }
    public Dictionary<string, string> Instructions { get; }
    public Dictionary<string, string> Tutorials { get; }

    public Localization(Language lang, string xml) {
        CurrLanguage = lang;

        XmlSerializer serializer = new XmlSerializer(typeof(LocalizationGroup));
        StringReader stream = new StringReader(xml);
        LocalizationGroup group = serializer.Deserialize(stream) as LocalizationGroup;
        stream.Close();

        General = group.general.ToDictionary(x => x.id, x => x.text);
        Buttons = group.buttons.ToDictionary(x => x.id, x => x.text);
        Legends = group.legends.ToDictionary(x => x.id, x => x.text);
        Archetypes = group.archetypes.ToDictionary(x => x.id, x => x.text);
        Instructions = group.instructions.ToDictionary(x => x.id, x => x.text);
        Tutorials = group.tutorials.ToDictionary(x => x.id, x => x.text);
    }
}