using System;

/// <summary>
/// Contains the avatar's basic information, included in Archetypes.csv.
/// </summary>
public class Archetype {
    public string name;
    public int age;
    public Gender gender;
    public float height; // in cm
    public float weight; // in kg
    public string subjectId;
        
    // C# cannot properly recognize ternary operators in a format string, so we have to use a helper property.
    public string GenderString => gender == Gender.Male ? "M" : "F";

    /// <summary>
    /// Converts the object to a JSON string. The string is formatted for debugging purposes; to reduce network package
    /// size, please consider converting to a string with no indents, which can be done in JObject.
    /// </summary>
    /// <returns>The JSON representation of the object.</returns>
    public string ToJson() => $@"
    {{
        ""client_subject_id"": ""{UnityEngine.Random.value}{((DateTimeOffset) DateTime.Now).ToUnixTimeSeconds()}"",
        ""dob"": ""{DateTime.Now.AddYears(-age):yyyy-MM-dd}"",
        ""height"": {{
            ""value"": {height},
            ""unit"": ""cm""
        }},
        ""sex"": ""{GenderString}"",
        ""biomarker_records"": [
            {{
                ""date"": ""{DateTime.Now:yyyy-MM-dd}"",
                ""biomarker_values"": [
                    {{
                        ""type"": ""body_weight"",
                        ""value"": {weight},
                        ""unit"": ""kg""
                    }}
                ]
            }}
        ]
    }}";
}