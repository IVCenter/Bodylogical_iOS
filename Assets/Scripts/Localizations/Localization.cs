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

    public Language language;
    public Dictionary<string, string> general, buttons, legends, archetypes, instructions, tutorials;

    public Localization(Language lang, string xml) {
        language = lang;

        XmlSerializer serializer = new XmlSerializer(typeof(LocalizationGroup));
        StringReader stream = new StringReader(xml);
        LocalizationGroup group = serializer.Deserialize(stream) as LocalizationGroup;
        stream.Close();

        general = group.general.ToDictionary(x => x.id, x => x.text);
        buttons = group.buttons.ToDictionary(x => x.id, x => x.text);
        legends = group.legends.ToDictionary(x => x.id, x => x.text);
        archetypes = group.archetypes.ToDictionary(x => x.id, x => x.text);
        instructions = group.instructions.ToDictionary(x => x.id, x => x.text);
        tutorials = group.tutorials.ToDictionary(x => x.id, x => x.text);
    }
}