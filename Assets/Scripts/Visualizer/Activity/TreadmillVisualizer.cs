using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TreadmillVisualizer : Visualizer {
    public override string VisualizerKey => "General.ActJog";
    public override HealthStatus Status { get; set; }

    [SerializeField] private Renderer[] treadmills;
    [SerializeField] private Image[] labels;
    [SerializeField] private Color hightlightColor;
    [SerializeField] private Color originalColor;

    private bool initialized;
    private float[] speeds;
    private bool?[] isJogging; // not animating, jog/walk or wheelchair
    private WheelchairController[] wheelchairs;

    private IEnumerator textureMove;

    // Shader and animator properties
    private static readonly int alphaScale = Shader.PropertyToID("_AlphaScale");
    private static readonly int activityJog = Animator.StringToHash("ActivityJog");
    private static readonly int sitWheelchair = Animator.StringToHash("SitWheelchair");
    private static readonly int lerpAmount = Animator.StringToHash("LerpAmount");
    private static readonly int animationSpeed = Animator.StringToHash("AnimationSpeed");

    private void Start() {
        speeds = new float[treadmills.Length];
        isJogging = new bool?[treadmills.Length];
        wheelchairs = new WheelchairController[treadmills.Length];
        initialized = true;
    }

    public override bool Visualize(float index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);

        // Set transparency
        for (int i = 0; i < treadmills.Length; i++) {
            if ((HealthChoice) i == choice) {
                ActivityManager.Instance.Performers[i].Mat.SetFloat(alphaScale, 1);
                treadmills[i].material.SetFloat(alphaScale, 1);
                labels[i].color = hightlightColor;
            }
            else {
                ActivityManager.Instance.Performers[i].Mat.SetFloat(alphaScale, 0.5f);
                treadmills[i].material.SetFloat(alphaScale, 0.5f);
                labels[i].color = originalColor;
            }
        }

        if (textureMove == null) {
            textureMove = MoveStreetTexture();
            StartCoroutine(textureMove);
        }

        if (newStatus != Status) {
            Status = newStatus;
            return true;
        }

        return false;
    }

    public override void Stop() {
        for (int i = 0; i < treadmills.Length; i++) {
            ActivityManager.Instance.Performers[i].ArchetypeAnimator.SetBool(activityJog, false);
            ActivityManager.Instance.Performers[i].ArchetypeAnimator.SetBool(sitWheelchair, false);

            if (isJogging != null) {
                isJogging[i] = null;
            }

            ActivityManager.Instance.Performers[i].Mat.SetFloat(alphaScale, 1);
            treadmills[i].material.SetFloat(alphaScale, 1);
            labels[i].color = originalColor;
        }

        if (textureMove != null) {
            StopCoroutine(textureMove);
            textureMove = null;
        }
    }

    /// <summary>
    /// Dispose of the wheelchairs.
    /// </summary>
    public override void ResetVisualizer() {
        if (!initialized) {
            return;
        }

        Stop();
        for (int i = 0; i < wheelchairs.Length; i++) {
            if (wheelchairs[i] != null) {
                Destroy(wheelchairs[i].gameObject);
                wheelchairs[i] = null;
            }
        }

        initialized = false;
    }

    /// <summary>
    /// Generates a new speed for all characters.
    /// Speed is calculated using the health scores AND the age.
    /// </summary>
    /// <returns>The new speed.</returns>
    /// <param name="index">Index. The larger it is, the older the people are.</param>
    /// <param name="choice">Choice.</param>
    private HealthStatus GenerateNewSpeed(float index, HealthChoice choice) {
        HealthStatus status = HealthStatus.Bad; // default value

        for (int i = 0; i < treadmills.Length; i++) {
            HealthChoice currChoice = (HealthChoice) i;
            ArchetypeModel performer = ActivityManager.Instance.Performers[i];

            int score = HealthLoader.Instance
                .ChoiceDataDictionary[currChoice].CalculateHealth(index,
                    performer.ArchetypeData.gender);

            // Account for activity ability loss due to aging.
            float yearMultiplier = 1 - index * 0.05f;

            // Switch among running, walking and wheelchairing.
            // Blend tree lerping:
            // The walking/jogging animation only plays at a score of 30-100 (not bad).
            // Therefore, we need to convert from a scale of 30-100 to 0-1.
            Animator animator = performer.ArchetypeAnimator;
            animator.SetFloat(lerpAmount, (score - 30) / 70.0f);
            speeds[i] = score * 0.004f * yearMultiplier;
            // Walking and running requires different playback speeds.
            // Also controls the street animation.
            HealthStatus currStatus = HealthUtil.CalculateStatus(score);
            performer.Heart.Display(currStatus);

            if (currChoice == choice) {
                status = currStatus;
            }

            switch (currStatus) {
                case HealthStatus.Good:
                    if (isJogging[i] == null || isJogging[i] == false) {
                        isJogging[i] = true;
                        animator.SetBool(activityJog, true);
                        animator.SetBool(sitWheelchair, false);
                        if (wheelchairs[i] != null) {
                            wheelchairs[i].gameObject.SetActive(false);
                        }
                    }

                    animator.SetFloat(animationSpeed, score * 0.01f * yearMultiplier);
                    break;
                case HealthStatus.Moderate:
                    if (isJogging[i] == null || isJogging[i] == false) {
                        isJogging[i] = true;
                        animator.SetBool(activityJog, true);
                        animator.SetBool(sitWheelchair, false);
                        if (wheelchairs[i] != null) {
                            wheelchairs[i].gameObject.SetActive(false);
                        }
                    }

                    animator.SetFloat(animationSpeed, score * 0.02f * yearMultiplier);
                    break;
                case HealthStatus.Bad:
                    // switch to wheelchair.
                    if (isJogging[i] == null || isJogging[i] == true) {
                        isJogging[i] = false;
                        animator.SetBool(activityJog, false);
                        animator.SetBool(sitWheelchair, true);
                        if (wheelchairs[i] != null) {
                            wheelchairs[i].gameObject.SetActive(true);
                        }
                        else {
                            wheelchairs[i] =
                                Instantiate(ActivityManager.Instance.wheelchairPrefab)
                                    .GetComponent<WheelchairController>();
                            wheelchairs[i].transform.SetParent(
                                ActivityManager.Instance.performerPositions[i],
                                false);
                            wheelchairs[i].Initialize();
                            wheelchairs[i].gameObject.SetActive(true);
                        }
                    }

                    break;
            }
        }

        return status;
    }

    /// <summary>
    /// Refer to https://www.youtube.com/watch?v=auVq3TSz20o for more details.
    /// Notice that texture coordinates range from 0-1, so no need for Time.time.
    /// </summary>
    /// <returns>The street texture.</returns>
    private IEnumerator MoveStreetTexture() {
        float[] moves = new float[3];

        while (true) {
            for (int i = 0; i < moves.Length; i++) {
                moves[i] += speeds[i] * Time.deltaTime;
                if (moves[i] > 1) {
                    moves[i] = 0;
                }

                treadmills[i].material.mainTextureOffset = new Vector2(0, moves[i]);
            }

            yield return null;
        }
    }
}