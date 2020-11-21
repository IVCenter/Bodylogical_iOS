using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JoggingVisualizer : Visualizer {
    [SerializeField] private Image label;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color originalColor;
    [SerializeField] private HealthChoice visualizerChoice;
    [SerializeField] private PropAnimation propAnimation;
    
    private enum JoggingStatus {
        NotAnimating,
        Jogging, // includes walking
        Wheelchair
    }
    private JoggingStatus isJogging;
    private HealthStatus status;
    private GameObject wheelchair;
    private IEnumerator propsCoroutine;
    private WheelchairController wheelchairController;
    
    public ArchetypeModel Performer { get; set; }
    public Transform PerformerTransform { get; set; }
    
    // Animator properties
    private static readonly int AlphaScale = Shader.PropertyToID("_AlphaScale");
    private static readonly int ActivityJog = Animator.StringToHash("ActivityJog");
    private static readonly int SitWheelchair = Animator.StringToHash("SitWheelchair");
    private static readonly int LerpAmount = Animator.StringToHash("LerpAmount");
    private static readonly int AnimationSpeed = Animator.StringToHash("AnimationSpeed");

    public override bool Visualize(float index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);

        if (choice == visualizerChoice) {
            Performer.Mat.SetFloat(AlphaScale, 1);
            label.color = highlightColor;
            Performer.Heart.Opaque(false);
            if (wheelchairController != null) {
                wheelchairController.Alpha = 1;
            }
        } else {
            Performer.Mat.SetFloat(AlphaScale, 0.5f);
            label.color = originalColor;
            Performer.Heart.Opaque(true);
            if (wheelchairController != null) {
                wheelchairController.Alpha = 0.5f;
            }
        }

        if (propsCoroutine == null) {
            propsCoroutine = propAnimation.Animate();
            StartCoroutine(propsCoroutine);
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

        isJogging = JoggingStatus.NotAnimating;

        Performer.Mat.SetFloat(AlphaScale, 1);
        label.color = originalColor;
        if (wheelchairController != null) {
            wheelchairController.Alpha = 1;
        }
        
        if (propsCoroutine != null) {
            StopCoroutine(propsCoroutine);
            propsCoroutine = null;
        }
    }

    public override void ResetVisualizer() {
        Stop();
        if (wheelchair != null) {
            Destroy(wheelchair);
            wheelchair = null;
            wheelchairController = null;
        }
    }

    private HealthStatus GenerateNewSpeed(float index, HealthChoice choice) {
        HealthStatus newStatus = HealthStatus.Bad; // default value

        int score = ArchetypeManager.Instance.Selected.ArchetypeData.healthDict[visualizerChoice].CalculateHealth(index,
            Performer.ArchetypeData.gender, HealthType.bmi, HealthType.sbp);

        // Account for activity ability loss due to aging.
        float yearMultiplier = 1 - index * 0.02f;

        // Switch among running, walking and wheelchairing.
        // Blend tree lerping:
        // The walking/jogging animation only plays at a score of 30-100 (not bad).
        // Therefore, we need to convert from a scale of 30-100 to 0-1.
        Animator animator = Performer.ArchetypeAnimator;
        animator.SetFloat(LerpAmount, (score - 30) / 70.0f);
        propAnimation.Speed = score * 0.006f * yearMultiplier;
        // Walking and running requires different playback speeds.
        // Also controls the street animation.
        HealthStatus currStatus = HealthUtil.CalculateStatus(score);
        Performer.Heart.Display(currStatus);

        if (visualizerChoice == choice) {
            newStatus = currStatus;
        }

        switch (currStatus) {
            case HealthStatus.Good:
                if (isJogging != JoggingStatus.Jogging) {
                    isJogging = JoggingStatus.Jogging;
                    animator.SetBool(ActivityJog, true);
                    animator.SetBool(SitWheelchair, false);
                    if (wheelchair != null) {
                        wheelchair.SetActive(false);
                    }
                }

                animator.SetFloat(AnimationSpeed, score * 0.01f * yearMultiplier);
                break;
            case HealthStatus.Moderate:
                if (isJogging != JoggingStatus.Jogging) {
                    isJogging = JoggingStatus.Jogging;
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
                if (isJogging != JoggingStatus.Wheelchair) {
                    isJogging = JoggingStatus.Wheelchair;
                    animator.SetBool(ActivityJog, false);
                    animator.SetBool(SitWheelchair, true);
                    if (wheelchair != null) {
                        wheelchair.SetActive(true);
                    } else {
                        wheelchair = Instantiate(ActivityManager.Instance.wheelchairPrefab, PerformerTransform,
                            false);
                        wheelchairController = wheelchair.GetComponent<WheelchairController>();
                    }
                }

                break;
        }

        return newStatus;
    }
}