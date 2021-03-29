public class LinearIndicatorSlideBarManager : SlideBarManager {
    /// <summary>
    /// Minimum and maximum values this slide bar can hold.
    /// </summary>
    public float min, max;

    public IndicatorSlideBarBackground background;

    /// <summary>
    /// Warning and upper thresholds for the slide bar. 
    /// </summary>
    public float warning, upper;

    public void SetBackground() {
        background.warningBound = Percentage(warning);
        background.upperBound = Percentage(upper);
        background.SetWarningBound();
        background.SetDangerBound();
    }

    protected override int GetPercentage(int index, float number) {
        return Percentage(number);
    }

    private int Percentage(float number) {
        return (int) ((number - min) / (max - min) * 100);
    }
}