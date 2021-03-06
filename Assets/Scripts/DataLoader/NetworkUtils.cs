using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

/// <summary>
/// Utility class to communicate with the Bodylogical server.
/// </summary>
public static class NetworkUtils {
    #region Data Formatting

    private static readonly Dictionary<string, HealthType> ValueMap = new Dictionary<string, HealthType> {
        {"body_weight", HealthType.weight},
        {"body_mass_index", HealthType.bmi},
        {"fasting_serum_glucose", HealthType.glucose},
        {"serum_hba1c", HealthType.aic},
        {"systolic_blood_pressure", HealthType.sbp},
        {"diastolic_blood_pressure", HealthType.dbp}
    };

    private static Health HealthFromJson(JObject jObject) => new Health {
        date = (DateTime) jObject["date"],
        values = (from obj in (JArray) jObject["biomarker_values"]
                select new {key = (string) obj["type"], value = (float) obj["value"]})
            .ToDictionary(x => ValueMap[x.key], x => x.value)
    };

    private static void PopulateHealthFromJson(JArray array, LongTermHealth health) {
        health.healths = (from obj in array select HealthFromJson((JObject) obj)).ToList();
    }

    /// <summary>
    /// Converts the object to a JSON string. The string is formatted for debugging purposes; to reduce network package
    /// size, please consider converting to a string with no indents, which can be done in JObject.
    /// </summary>
    /// <returns>The JSON representation of the archetype.</returns>
    // The following id is the same as in the Javascript code, but for some reason the server doesn't like the format.
    //""client_subject_id"": ""{UnityEngine.Random.value}{((DateTimeOffset) DateTime.Now).ToUnixTimeSeconds()}"",
    private static string ArchetypeToJson(Archetype archetype) => $@"
    {{
        ""client_subject_id"": ""demo-test001"",
        ""dob"": ""{DateTime.Now.AddYears(-archetype.age):yyyy-MM-dd}"",
        ""height"": {{
            ""value"": {archetype.height},
            ""unit"": ""cm""
        }},
        ""sex"": ""{archetype.GenderString}"",
        ""biomarker_records"": [
            {{
                ""date"": ""{DateTime.Now:yyyy-MM-dd}"",
                ""biomarker_values"": [
                    {{
                        ""type"": ""body_weight"",
                        ""value"": {archetype.weight},
                        ""unit"": ""kg""
                    }}
                ]
            }}
        ]
    }}";

    /// <summary>
    /// Converts the lifestyle object to a JSON string. The string is formatted for debugging purposes;
    /// to reduce network package size, please consider converting to a string with no indents, which can be done in JObject.
    /// The archetype is required because of how "exercise" is calculated.
    /// </summary>
    /// <returns>The JSON representation of the object.</returns>
    private static string LifestyleToJson(Archetype archetype, Lifestyle lifestyle, int numMonths = 60,
        int interval = 3) => $@"{{
        ""forecast_timeline"": {{
            ""duration"": {numMonths},
            ""time_unit"": ""month"",
            ""interval"": {interval}
        }},
        ""interventions"": [{{
            ""start_date"": ""{DateTime.Today.AddDays(1):yyyy-MM-dd}"",
            ""duration"": {numMonths},
            ""time_unit"": ""month"",
            ""diet"": [
                {{
                    ""name"": ""carb_intake"",
                    ""value"": {lifestyle.carbIntake},
                    ""unit"": ""grams"",
                    ""type"": ""relative""
                }},
                {{
                    ""name"": ""fat_intake"",
                    ""value"": {lifestyle.fatIntake},
                    ""unit"": ""grams"",
                    ""type"": ""relative""
                }},
                {{
                    ""name"": ""protein_intake"",
                    ""value"": {lifestyle.proteinIntake},
                    ""unit"": ""grams"",
                    ""type"": ""relative""
                }}
            ],
            ""sleep"": {{
                ""value"": {lifestyle.sleepHours},
                ""unit"": ""hours"",
                ""type"": ""relative""
            }},
            ""exercise"": {{
                ""value"": {lifestyle.exercise * archetype.weight / 20},
                ""unit"": ""kcal"",
                ""type"": ""relative""
            }}
        }}]
    }}";

    #endregion

    #region Network Requests

    private const string UserMatchUrl = "https://orchestrator-dot-pg-us-e-app-165685.appspot.com/subjects";

    private static string ForecastURL(string subjectId) =>
        $"https://forecast-service-dot-pg-us-e-app-165685.appspot.com/subjects/{subjectId}/forecast";

    /// <summary>
    /// Notice that UnityWebRequest.POST() will escape characters, meaning that the request body will be corrupted.
    /// Therefore, we need to convert the json into a byte array (which won't be escaped) and construct the request
    /// from scratch.
    /// </summary>
    private static byte[] ToBytes(string json) => Encoding.UTF8.GetBytes(
        JObject.Parse(json).ToString(Formatting.None));

    /// <summary>
    /// Matches the user against an existing archetype, and retrieves the baseline health.
    /// </summary>
    /// <param name="archetype">The user's basic information.</param>
    /// <param name="health">A reference to the baseline health. It will be populated after the coroutine ends.</param>
    /// <param name="error">An error object that will carry error messages (if any).</param>
    public static IEnumerator UserMatch(Archetype archetype, LongTermHealth health, NetworkError error) {
        using (UnityWebRequest www = new UnityWebRequest(UserMatchUrl)) {
            www.method = "POST";
            www.uploadHandler = new UploadHandlerRaw(ToBytes(ArchetypeToJson(archetype)));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("org-id", "11");
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) {
                error.status = (NetworkStatus) www.responseCode;
                error.message = www.error;
                yield break;
            }

            JObject jObject = JObject.Parse(www.downloadHandler.text);
            if (!(bool) jObject["success"]) {
                error.status = NetworkStatus.Unknown;
                error.message = (string) jObject["message"];
                yield break;
            }

            error.status = NetworkStatus.Success;
            string subjectId = (string) jObject["subject_id"];
            archetype.subjectId = subjectId;
            Lifestyle baseline = new Lifestyle();

            yield return Forecast(archetype, baseline, health, error);
        }
    }

    /// <summary>
    /// Connects to the server and predicts the archetype's future health.
    /// </summary>
    /// <param name="archetype">The archetype we want to predict. We need the subject id and the weight.</param>
    /// <param name="lifestyle">The lifestyle of the archetype.</param>
    /// <param name="health">The health object to be populated by the server.</param>
    /// <param name="error">An error object that will carry error messages (if any).</param>
    public static IEnumerator Forecast(Archetype archetype, Lifestyle lifestyle, LongTermHealth health,
        NetworkError error) {
        using (UnityWebRequest www = new UnityWebRequest(ForecastURL(archetype.subjectId))) {
            www.method = "POST";
            www.uploadHandler = new UploadHandlerRaw(ToBytes(LifestyleToJson(archetype, lifestyle)));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("org-id", "11");
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) {
                error.status = (NetworkStatus) www.responseCode;
                error.message = www.error;
                yield break;
            }

            JObject jObject = JObject.Parse(www.downloadHandler.text);
            if (!(bool) jObject["success"]) {
                error.status = NetworkStatus.Unknown;
                error.message = (string) jObject["message"];
                yield break;
            }

            error.status = NetworkStatus.Success;
            PopulateHealthFromJson((JArray) jObject["biomarker_forecasts"], health);
        }
    }

    #endregion
}