public abstract class OrganVisualizer : Visualizer {
    protected int score;
    protected HealthStatus status;

    protected ColorLibrary Library => performer.prius.colorLibrary;
    
    public abstract string ExplanationText { get; }
    public abstract bool UpdateStatus(float index, HealthChoice choice);
}