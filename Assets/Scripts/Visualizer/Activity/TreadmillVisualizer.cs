using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadmillVisualizer : Visualizer {
    public override string VisualizerKey { get { return "General.ActJog"; } }

    public Transform ArchetypeTransform { get { return HumanManager.Instance.SelectedHuman.transform; } }
    /// <summary>
    /// To be determined at runtime, so use property.
    /// </summary>
    /// <value>The archetype animator.</value>
    public Animator ArchetypeAnimator { get { return HumanManager.Instance.HumanAnimator; } }
    public override HealthStatus Status { get; set; }

    public Renderer archetypeTreadmill, companionTreadmill;
    private float archetypeTreadmillSpeed, companionTreadmillSpeed;
    private IEnumerator textureMove;

    private bool? isJogging; // not animating, jog/walk or wheelchair

    public override bool Visualize(float index, HealthChoice choice) {
        HealthStatus newStatus = GenerateNewSpeed(index, choice);

        // Set stars
        ActivityManager.Instance.charHeart.Display(newStatus);

        // Set textures and models
        // TODO: when blend shapes are all complete, change archetype blend shape HERE
        ActivityManager.Instance.CurrentCompanion.SetTexture(index * 5);

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

    /// <summary>
    /// Stops the animation, moves the people back to original position.
    /// </summary>
    public override void Pause() {
        if (ActivityManager.Instance.CurrentAnimator.gameObject.activeInHierarchy &&
            !ActivityManager.Instance.CurrentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            ActivityManager.Instance.CurrentAnimator.SetTrigger("Idle");
        }
        if (!ArchetypeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            ArchetypeAnimator.SetTrigger("Idle");
        }

        isJogging = null;

        if (textureMove != null) { 
            StopCoroutine(textureMove);
            textureMove = null;
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
    private HealthStatus GenerateNewSpeed(float index, HealthChoice choice) {
        int score = HealthDataContainer.Instance.choiceDataDictionary[choice].CalculateHealth(index,
          HumanManager.Instance.selectedArchetype.gender);
        // Account for activity ability loss due to aging.
        float yearMultiplier = 1 - index * 0.05f;

        // Companion: always running
        ActivityManager.Instance.CurrentAnimator.SetFloat("AnimationSpeed", yearMultiplier);
        ActivityManager.Instance.CurrentAnimator.SetFloat("LerpAmount", yearMultiplier);
        ActivityManager.Instance.CurrentAnimator.SetTrigger("Jog");
        companionTreadmillSpeed = yearMultiplier * 0.4f;

        // Archetype: switches among running, walking and wheelchairing.

        // Blend tree lerping:
        // The walking/jogging animation only plays at a score of 30-100 (not bad).
        // Therefore, we need to convert from a scale of 30-100 to 0-1.
        ArchetypeAnimator.SetFloat("LerpAmount", (score - 30) / 70.0f);
        archetypeTreadmillSpeed = score * 0.004f * yearMultiplier;
        // Walking and running requires different playback speeds.
        // Also controls the street animation.
        HealthStatus status = HealthUtil.CalculateStatus(score);
        switch (status) {
            case HealthStatus.Good:
                if (isJogging == null || isJogging == false) {
                    isJogging = true;
                    ArchetypeAnimator.SetTrigger("Jog");
                    ActivityManager.Instance.wheelchair.ToggleOff();
                }
                ArchetypeAnimator.SetFloat("AnimationSpeed", score * 0.01f * yearMultiplier);
                break;
            case HealthStatus.Intermediate:
                if (isJogging == null || isJogging == false) {
                    isJogging = true;
                    ArchetypeAnimator.SetTrigger("Jog");
                    ActivityManager.Instance.wheelchair.ToggleOff();
                }
                ArchetypeAnimator.SetFloat("AnimationSpeed", score * 0.02f * yearMultiplier);
                break;
            case HealthStatus.Bad:
                // switch to wheelchair.
                if (isJogging == null || isJogging == true) {
                    isJogging = false;
                    ArchetypeAnimator.SetTrigger("SitWheelchair");
                    ActivityManager.Instance.wheelchair.ToggleOn();
                }

                break;
        }
        return status;
    }

    /// <summary>
    /// Refer to https://www.youtube.com/watch?v=auVq3TSz20o for more details.
    /// Notice that texture coordinates range from 0-1, so no need for Time.time.
    /// </summary>
    /// <returns>The street texture.</returns>
    private IEnumerator MoveStreetTexture() {
        float archetypeMove = 0, companionMove = 0;
        while (true) {
            archetypeMove += archetypeTreadmillSpeed * Time.deltaTime;
            companionMove += companionTreadmillSpeed * Time.deltaTime;
            if (archetypeMove > 1) {
                archetypeMove = 0;
            }
            if (companionMove > 1) {
                companionMove = 0;
            }
            archetypeTreadmill.material.mainTextureOffset = new Vector2(0, archetypeMove);
            companionTreadmill.material.mainTextureOffset = new Vector2(0, companionMove);
            yield return null;
        }
    }
}
