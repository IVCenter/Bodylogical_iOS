using System;

[Serializable]
public class Lifestyle {
    public Archetype archetype;
    public HealthChoice choice;
    public float sleepHours;
    public float carbIntake; // grams
    public float fatIntake; // grams
    public float proteinIntake; // grams
    public float exercise;
    public HealthStatus adherence;
    
    /// <summary>
    /// Converts the object to a JSON string. The string is formatted for debugging purposes; to reduce network package
    /// size, please consider converting to a string with no indents, which can be done in JObject.
    /// </summary>
    /// <returns>The JSON representation of the object.</returns>
    public string ToJson() => $@"{{
        ""forecast_timeline"": {{
            ""duration"": 24,
            ""time_unit"": ""month"",
            ""interval"": 3
        }},
        ""interventions"": [{{
            ""start_date"": ""{DateTime.Today.AddDays(1):yyyy-MM-dd}"",
            ""duration"": 24,
            ""time_unit"": ""month"",
            ""diet"": [
                {{
                    ""name"": ""carb_intake"",
                    ""value"": {carbIntake},
                    ""unit"": ""grams"",
                    ""type"": ""relative""
                }},
                {{
                    ""name"": ""fat_intake"",
                    ""value"": {fatIntake},
                    ""unit"": ""grams"",
                    ""type"": ""relative""
                }},
                {{
                    ""name"": ""protein_intake"",
                    ""value"": {proteinIntake},
                    ""unit"": ""grams"",
                    ""type"": ""relative""
                }}
            ],
            ""sleep"": {{
                ""value"": {sleepHours},
                ""unit"": ""hours"",
                ""type"": ""relative""
            }},
            ""exercise"": {{
                ""value"": {exercise * archetype.weight / 20},
                ""unit"": ""kcal"",
                ""type"": ""relative""
            }}
        }}]
    }}";
}