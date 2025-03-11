using HarmonyLib;
using UnityEngine.SceneManagement;

namespace BreatheEasy.Camera_Effects;

[HarmonyPatch(typeof(HeatDistortImageEffect), nameof(HeatDistortImageEffect.OnRenderImage))]
static class HeatDistortImageEffectOnRenderImagePatch
{
    static void Postfix(HeatDistortImageEffect __instance)
    {
        __instance.enabled = !BreatheEasyPlugin.RemoveAshlandsHeatWave.Value.IsOn();
    }
}

public abstract class AshlandsHeatWave
{
    private static void _RemoveHeatWave()
    {
        GameCamera instance = GameCamera.instance;
        if (instance == null)
            return;
        HeatDistortImageEffect component = instance.gameObject.GetComponent<HeatDistortImageEffect>();
        if (component == null)
            return;
        BreatheEasyPlugin.BreatheEasyLogger.LogWarning("Removing Ashlands Heat Wave");
        component.enabled = !BreatheEasyPlugin.RemoveAshlandsHeatWave.Value.IsOn();
    }

    internal static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => _RemoveHeatWave();
}