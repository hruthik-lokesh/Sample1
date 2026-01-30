using UnityEngine;

/// <summary>
/// Defines all experiment mode conditions for the XR training system.
/// Each mode maps to a specific behavior type (Proactive/Reactive/Baseline) and environment (VR/AR).
/// Numbers are included in enum names for clear identification in Unity Inspector.
/// </summary>
public enum ExperimentModeCondition
{
    /// <summary>
    /// Mode 1: Proactive VR - Shows coin markers and target points immediately in VR
    /// Behavior: Proactive (1)
    /// </summary>
    [InspectorName("1. Proactive_VR")]
    _1_Proactive_VR = 1,

    /// <summary>
    /// Mode 2: Reactive VR - Reveals coins when user approaches target points in VR
    /// Behavior: Reactive (2)
    /// </summary>
    [InspectorName("2. Reactive_VR")]
    _2_Reactive_VR = 2,

    /// <summary>
    /// Mode 3: Baseline VR - Normal mode with no visual cues in VR
    /// Behavior: Baseline/Normal (0)
    /// </summary>
    [InspectorName("3. Baseline_VR")]
    _3_Baseline_VR = 3,

    /// <summary>
    /// Mode 4: Proactive AR - Shows coin markers and target points immediately in AR
    /// Behavior: Proactive (1)
    /// </summary>
    [InspectorName("4. Proactive_AR")]
    _4_Proactive_AR = 4,

    /// <summary>
    /// Mode 5: Reactive AR - Reveals coins when user approaches target points in AR
    /// Behavior: Reactive (2)
    /// </summary>
    [InspectorName("5. Reactive_AR")]
    _5_Reactive_AR = 5,

    /// <summary>
    /// Mode 6: Baseline AR - Normal mode with no visual cues in AR
    /// Behavior: Baseline/Normal (0)
    /// </summary>
    [InspectorName("6. Baseline_AR")]
    _6_Baseline_AR = 6
}

/// <summary>
/// Helper class for ExperimentModeCondition operations
/// </summary>
public static class ExperimentModeConditionHelper
{
    /// <summary>
    /// Gets the behavior type (0=Baseline, 1=Proactive, 2=Reactive) from the experiment mode condition.
    /// This maps to the internal behavior logic used by CoinBombScript1.
    /// </summary>
    public static int GetBehaviorMode(ExperimentModeCondition mode)
    {
        switch (mode)
        {
            case ExperimentModeCondition._1_Proactive_VR:
            case ExperimentModeCondition._4_Proactive_AR:
                return 1; // Proactive behavior

            case ExperimentModeCondition._2_Reactive_VR:
            case ExperimentModeCondition._5_Reactive_AR:
                return 2; // Reactive behavior

            case ExperimentModeCondition._3_Baseline_VR:
            case ExperimentModeCondition._6_Baseline_AR:
            default:
                return 0; // Baseline/Normal behavior
        }
    }

    /// <summary>
    /// Returns true if the mode is an AR mode
    /// </summary>
    public static bool IsARMode(ExperimentModeCondition mode)
    {
        return mode == ExperimentModeCondition._4_Proactive_AR ||
               mode == ExperimentModeCondition._5_Reactive_AR ||
               mode == ExperimentModeCondition._6_Baseline_AR;
    }

    /// <summary>
    /// Returns true if the mode is a VR mode
    /// </summary>
    public static bool IsVRMode(ExperimentModeCondition mode)
    {
        return mode == ExperimentModeCondition._1_Proactive_VR ||
               mode == ExperimentModeCondition._2_Reactive_VR ||
               mode == ExperimentModeCondition._3_Baseline_VR;
    }

    /// <summary>
    /// Gets a descriptive string for the mode
    /// </summary>
    public static string GetModeDescription(ExperimentModeCondition mode)
    {
        switch (mode)
        {
            case ExperimentModeCondition._1_Proactive_VR:
                return "1. Proactive_VR - Visual cues shown immediately";
            case ExperimentModeCondition._2_Reactive_VR:
                return "2. Reactive_VR - Cues revealed on approach";
            case ExperimentModeCondition._3_Baseline_VR:
                return "3. Baseline_VR - No visual cues";
            case ExperimentModeCondition._4_Proactive_AR:
                return "4. Proactive_AR - Visual cues shown immediately";
            case ExperimentModeCondition._5_Reactive_AR:
                return "5. Reactive_AR - Cues revealed on approach";
            case ExperimentModeCondition._6_Baseline_AR:
                return "6. Baseline_AR - No visual cues";
            default:
                return "Unknown Mode";
        }
    }

    /// <summary>
    /// Gets the environment string (VR or AR)
    /// </summary>
    public static string GetEnvironment(ExperimentModeCondition mode)
    {
        return IsARMode(mode) ? "AR" : "VR";
    }

    /// <summary>
    /// Gets the behavior name
    /// </summary>
    public static string GetBehaviorName(ExperimentModeCondition mode)
    {
        int behavior = GetBehaviorMode(mode);
        switch (behavior)
        {
            case 1: return "Proactive";
            case 2: return "Reactive";
            default: return "Baseline";
        }
    }

    /// <summary>
    /// Gets a clean display name without the underscore prefix
    /// </summary>
    public static string GetDisplayName(ExperimentModeCondition mode)
    {
        switch (mode)
        {
            case ExperimentModeCondition._1_Proactive_VR: return "1. Proactive_VR";
            case ExperimentModeCondition._2_Reactive_VR: return "2. Reactive_VR";
            case ExperimentModeCondition._3_Baseline_VR: return "3. Baseline_VR";
            case ExperimentModeCondition._4_Proactive_AR: return "4. Proactive_AR";
            case ExperimentModeCondition._5_Reactive_AR: return "5. Reactive_AR";
            case ExperimentModeCondition._6_Baseline_AR: return "6. Baseline_AR";
            default: return "Unknown";
        }
    }
}
