using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JoggingVisualizer : Visualizer {
    [SerializeField] private GameObject wheelchairPrefab;

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

    public ArchetypePerformer Performer { get; set; }
    public ActivityController Controller => Performer.Activity;
    // TODO: to be replaced by random props
    public PropAnimation Props { get; set; }

    // Animator properties
    private static readonly int ActivityJog = Animator.StringToHash("ActivityJog");
    private static readonly int SitWheelchair = Animator.StringToHash("SitWheelchair");
    private static readonly int LerpAmount = Animator.StringToHash("LerpAmount");
    private static readonly int AnimationSpeed = Animator.StringToHash("AnimationSpeed");

    public override bool Visualize(float index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);

        if (propsCoroutine == null) {
            propsCoroutine = Props.Animate();
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
        int score = Performer.ArchetypeHealth.CalculateHealth(index, Performer.ArchetypeData.gender,
            HealthType.bmi, HealthType.sbp);

        // Account for activity ability loss due to aging.
        float yearMultiplier = 1 - index * 0.02f;

        // Switch among running, walking and wheelchairing.
        // Blend tree lerping:
        // The walking/jogging animation only plays at a score of 30-100 (not bad).
        // Therefore, we need to convert from a scale of 30-100 to 0-1.
        Animator animator = Performer.ArchetypeAnimator;
        animator.SetFloat(LerpAmount, (score - 30) / 70.0f);
        Props.Speed = score * 0.006f * yearMultiplier;
        // Walking and running requires different playback speeds.
        // Also controls the street animation.
        HealthStatus newStatus = HealthUtil.CalculateStatus(score);
        Controller.heart.Display(newStatus);

        switch (newStatus) {
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
                        wheelchair = Instantiate(wheelchairPrefab, transform, false);
                        wheelchairController = wheelchair.GetComponent<WheelchairController>();
                    }
                }

                break;
        }

        return newStatus;
    }
}