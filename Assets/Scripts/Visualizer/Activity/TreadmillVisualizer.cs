using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadmillVisualizer : Visualizer {
    public override string VisualizerName { get { return "Jogging"; } }

    public Transform ArchetypeTransform { get { return HumanManager.Instance.SelectedHuman.transform; } }
    /// <summary>
    /// To be determined at runtime, so use property.
    /// </summary>
    /// <value>The archetype animator.</value>
    public Animator ArchetypeAnimator { get { return HumanManager.Instance.HumanAnimator; } }
    public override HealthStatus Status { get; set; }

    /// <summary>
    /// This cannot be determined at runtime because Awake() won't be called if
    /// the object is disabled and Pause() would shift the companion's position,
    /// even if the object is disabled.
    /// </summary>
    public Vector3 companionOriginalLocalPos;

    private IEnumerator archetypeMovement;
    private IEnumerator companionMovement;

    public override void Initialize() {
        ActivityManager.Instance.CompanionTransform.localPosition = companionOriginalLocalPos;
    }

    public override bool Visualize(int index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);

        if (archetypeMovement == null) {
            archetypeMovement = ArchetypeJog();
            StartCoroutine(archetypeMovement);
            companionMovement = CompanionJog();
            StartCoroutine(companionMovement);
        }

        if (newStatus != Status) {
            Status = newStatus;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Stops the animation, moves the people back to original position.
    /// </summary>
    public override void Pause() {
        if (archetypeMovement != null) {
            StopCoroutine(archetypeMovement);
            archetypeMovement = null;
            StopCoroutine(companionMovement);
            companionMovement = null;
            ActivityManager.Instance.CompanionAnimator.SetTrigger("Idle");
            ArchetypeAnimator.SetTrigger("Idle");
        }
    }

    /// <summary>
    /// Generates a new speed for both the archetype and the companion.
    /// Speed is calculated using the health scores AND the age.
    /// 
    /// </summary>
    /// <returns>The new speed.</returns>
    /// <param name="index">Index. Range from 0-4. The larger it is, the older the people are.</param>
    /// <param name="choice">Choice.</param>
    private HealthStatus GenerateNewSpeed(int index, HealthChoice choice) {
        int score = HealthDataContainer.Instance.choiceDataDictionary[choice].CalculateHealth(index,
          HumanManager.Instance.UseAlt);
        // Account for activity ability loss due to aging.
        float yearMultiplier = 1 - index * 0.05f;

        // Companion: always running
        ActivityManager.Instance.CompanionAnimator.SetFloat("AnimationSpeed", yearMultiplier);
        ActivityManager.Instance.CompanionAnimator.SetFloat("LerpAmount", yearMultiplier);

        // Archetype: switches among running, walking and wheelchairing.

        // Blend tree lerping:
        // The walking/jogging animation only plays at a score of 30-100 (not bad).
        // Therefore, we need to convert from a scale of 30-100 to 0-1.
        ArchetypeAnimator.SetFloat("LerpAmount", (score - 30) / 70.0f);

        // Walking and running requires different playback speeds.
        if (HealthUtil.CalculateStatus(score) == HealthStatus.Intermediate) {
            ArchetypeAnimator.SetFloat("AnimationSpeed", score * 0.02f);
        } else {
            ArchetypeAnimator.SetFloat("AnimationSpeed", score * 0.01f);
        }

        // TODO: wheelchair

        return HealthUtil.CalculateStatus(score);
    }

    private IEnumerator ArchetypeJog() {
        ArchetypeAnimator.SetTrigger("Jog");
        yield return null;
        //while (true) {
        //    yield return null;
        //}
    }

    private IEnumerator CompanionJog() {
        Animator currAnimator = ActivityManager.Instance.CompanionAnimator; 
        currAnimator.SetTrigger("Jog");
        while (true) {
            // Since companion may change (from younger to older and vise versa),
            // need to check if the companion has changed.
            if (currAnimator != ActivityManager.Instance.CompanionAnimator) {
                currAnimator = ActivityManager.Instance.CompanionAnimator;
                currAnimator.SetTrigger("Jog");
            }
            yield return null;
        }
    }
}
