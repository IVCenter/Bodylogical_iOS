using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class Localization {
    [XmlRoot("Localization")]
    public class LocalizationGroup {
        [XmlArray("General")]
        public List<LocalizationItem> general;
        [XmlArray("Buttons")]
        public List<LocalizationItem> buttons;
        [XmlArray("Legends")]
        public List<LocalizationItem> legends;
        [XmlArray("Archetypes")]
        public List<LocalizationItem> archetypes;
        [XmlArray("Instructions")]
        public List<LocalizationItem> instructions;
    }

    [XmlRoot("Item")]
    public class LocalizationItem {
        [XmlAttribute]
        public string id;
        [XmlText]
        public string text;
    }

    public Language language;
    public Dictionary<string, string> general, buttons, legends, archetypes, instructions;

    public Localization(Language lang, string path) {
        language = lang;
        XmlSerializer serializer = new XmlSerializer(typeof(LocalizationGroup));
        FileStream stream = new FileStream(path, FileMode.Open);
        var container = serializer.Deserialize(stream) as LocalizationGroup;
        stream.Close();


    }
}