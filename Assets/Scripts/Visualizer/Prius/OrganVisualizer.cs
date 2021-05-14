public abstract class OrganVisualizer : Visualizer {
    protected int score;
    protected HealthStatus status;

    protected ColorLibrary Library => performer.prius.colorLibrary;
}