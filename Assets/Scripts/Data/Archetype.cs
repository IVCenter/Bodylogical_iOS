/// <summary>
/// Contains the avatar's basic information, included in Archetypes.csv.
/// </summary>
public class Archetype {
    public int age;
    public Gender gender;
    public float height; // in cm
    public float weight; // in kg
    public string subjectId;
        
    // C# cannot properly recognize ternary operators in a format string, so we have to use a helper property.
    public string GenderString => gender == Gender.Male ? "M" : "F";
}