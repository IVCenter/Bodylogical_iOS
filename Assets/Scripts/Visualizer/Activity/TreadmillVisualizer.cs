using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TreadmillVisualizer : Visualizer {
    private HealthStatus status;
    
    [SerializeField] private Image label;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color originalColor;
    [SerializeField] private HealthChoice visualizerChoice;

    private enum JoggingStatus {
        NotAnimating,
        Jogging, // includes walking
        Wheelchair
    }
    
    private JoggingStatus joggingStatus;
    private Renderer treadmill;
    private float speed;
    private GameObject wheelchair;
    private IEnumerator textureMove;

    public ArchetypeModel Performer { get; set; }
    public Transform PerformerTransform { get; set; }
    
    // Shader and animator properties
    private static readonly int AlphaScale = Shader.PropertyToID("_AlphaScale");
    private static readonly int ActivityJog = Animator.StringToHash("ActivityJog");
    private static readonly int SitWheelchair = Animator.StringToHash("SitWheelchair");
    private static readonly int LerpAmount = Animator.StringToHash("LerpAmount");
    private static readonly int AnimationSpeed = Animator.StringToHash("AnimationSpeed");

    private void Start() {
        treadmill = GetComponent<Renderer>();
    }
    
    public override bool Visualize(float index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);

        // Set transparency
        if (visualizerChoice == choice) {
            Performer.Mat.SetFloat(AlphaScale, 1);
            treadmill.material.SetFloat(AlphaScale, 1);
            label.color = highlightColor;
            if (wheelchair != null) {
                wheelchair.GetComponent<WheelchairController>().Alpha = 1;
            }

            Performer.Heart.Opaque(false);
        } else {
            Performer.Mat.SetFloat(AlphaScale, 0.5f);
            treadmill.material.SetFloat(AlphaScale, 0.5f);
            label.color = originalColor;
            if (wheelchair != null) {
                wheelchair.GetComponent<WheelchairController>().Alpha = 0.5f;
            }

            Performer.Heart.Opaque(true);
        }

        if (textureMove == null) {
            textureMove = MoveStreetTexture();
            StartCoroutine(textureMove);
        }

        if (newStatus != status) {
            status = newStatus;
            return true;
        }

        return false;
    }

    public override void Stop() {
        Performer.ArchetypeAnimator.SetBool(ActivityJog, false);
        Performer.ArchetypeAnimator.SetBool(SitWheelchair, false);

        joggingStatus = JoggingStatus.NotAnimating;

        Performer.Mat.SetFloat(AlphaScale, 1);
        treadmill.material.SetFloat(AlphaScale, 1);
        label.color = originalColor;

        if (textureMove != null) {
            StopCoroutine(textureMove);
            textureMove = null;
        }
    }

    /// <summary>
    /// Dispose of the wheelchairs.
    /// </summary>
    public override void ResetVisualizer() {
        Stop();

        if (wheelchair != null) {
            Destroy(wheelchair);
            wheelchair = null;
        }
    }

    /// <summary>
    /// Generates a new speed for all characters.
    /// Speed is calculated using the health scores AND the age.
    /// </summary>
    /// <returns>The new speed.</returns>
    /// <param name="index">Index. The larger it is, the older the people are.</param>
    /// <param name="choice">Choice.</param>
    private HealthStatus GenerateNewSpeed(float index, HealthChoice choice) {
        HealthStatus newStatus = HealthStatus.Bad; // default value
        
        int score = ArchetypeManager.Instance.Selected.ArchetypeData.healthDict[visualizerChoice].CalculateHealth(index,
            Performer.ArchetypeData.gender, HealthType.bmi, HealthType.sbp);

        // Account for activity ability loss due to aging.
        float yearMultiplier = 1 - index * 0.05f;

        // Switch among running, walking and wheelchairing.
        // Blend tree lerping:
        // The walking/jogging animation only plays at a score of 30-100 (not bad).
        // Therefore, we need to convert from a scale of 30-100 to 0-1.
        Animator animator = Performer.ArchetypeAnimator;
        animator.SetFloat(LerpAmount, (score - 30) / 70.0f);
        speed = score * 0.004f * yearMultiplier;
        // Walking and running requires different playback speeds.
        // Also controls the street animation.
        HealthStatus currStatus = HealthUtil.CalculateStatus(score);
        Performer.Heart.Display(currStatus);

        if (visualizerChoice == choice) {
            newStatus = currStatus;
        }

        switch (currStatus) {
            case HealthStatus.Good:
                if (joggingStatus != JoggingStatus.Jogging) {
                    joggingStatus = JoggingStatus.Jogging;
                    animator.SetBool(ActivityJog, true);
                    animator.SetBool(SitWheelchair, false);
                    if (wheelchair != null) {
                        wheelchair.SetActive(false);
                    }
                }

                animator.SetFloat(AnimationSpeed, score * 0.01f * yearMultiplier);
                break;
            case HealthStatus.Moderate:
                if (joggingStatus != JoggingStatus.Jogging) {
                    joggingStatus = JoggingStatus.Jogging;
                    animator.SetBool(ActivityJog, true);
                    animator.SetBool(SitWheelchair, false);
                    if (wheelchair != null) {
                        wheelchair.SetActive(false);
                    }
                }

                animator.SetFloat(AnimationSpeed, score * 0.02f * yearMultiplier);
                break;
            case HealthStatus.Bad:
                // switch to wheelchair.
                if (joggingStatus != JoggingStatus.Wheelchair) {
                    joggingStatus = JoggingStatus.Wheelchair;
                    animator.SetBool(ActivityJog, false);
                    animator.SetBool(SitWheelchair, true);
                    if (wheelchair != null) {
                        wheelchair.SetActive(true);
                    } else {
                        wheelchair = Instantiate(ActivityManager.Instance.wheelchairPrefab);
                        wheelchair.transform.SetParent(PerformerTransform, false);
                    }
                }

                break;
        }

        return newStatus;
    }

    /// <summary>
    /// Refer to https://www.youtube.com/watch?v=auVq3TSz20o for more details.
    /// Notice that texture coordinates range from 0-1, so no need for Time.time.
    /// </summary>
    private IEnumerator MoveStreetTexture() {
        float moves = 0;

        while (true) {
            moves += speed * Time.deltaTime;
            if (moves > 1) {
                moves = 0;
            }

            treadmill.material.mainTextureOffset = new Vector2(0, moves);
            yield return null;
        }
    }
}