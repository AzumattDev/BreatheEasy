using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

namespace BreatheEasy.Camera_Effects;

public abstract class LensDirt
{
    static float Original = 0.0f;

    private static void _RemoveLensDirt()
    {
        GameCamera instance = GameCamera.instance;
        if (instance == null)
            return;
        PostProcessingBehaviour component = instance.gameObject.GetComponent<PostProcessingBehaviour>();
        if (component == null)
            return;
        // Cache original value to restore it later
        Original = component.profile.bloom.m_Settings.lensDirt.intensity;
        component.profile.bloom.m_Settings.lensDirt.intensity = BreatheEasyPlugin.RemoveLensDirt.Value.IsOn() ? 0.0f : Original;
    }

    internal static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => _RemoveLensDirt();
}