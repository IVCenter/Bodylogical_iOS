public enum AppState {
    /// <summary>
    /// User is choosing a language
    /// </summary>
    ChooseLanguage,
    /// <summary>
    /// User is finding a suitable Plane Surface
    /// </summary>
    FindPlane,
    /// <summary>
    /// User is placing the stage
    /// </summary>
    PlaceStage,
    /// <summary>
    /// Expanding the information panels for the selected archetype
    /// </summary>
    ShowDetails,
    /// <summary>
    /// Waiting for further user actions
    /// </summary>
    Idle,
    /// <summary>
    /// Visualizations
    /// </summary>
    Visualizations,
}