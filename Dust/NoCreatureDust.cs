using HarmonyLib;
using static BreatheEasy.BreatheEasyPlugin;

namespace BreatheEasy.Dust;

[HarmonyPatch(typeof(Character), nameof(Character.OnDeath))]
static class CharacterOnDeathPatch
{
    public static void Prefix(Character __instance)
    {
        if(IsConflictingModLoaded(ConflictingModConstants.NoCreatureDust)) return;
        if (__instance.IsPlayer() || __instance.m_deathEffects == null || RemoveAllVFX_Ncd.Value.IsOff())
        {
            return;
        }

        // Remove the vfx from the deathEffects and only the vfx
        EffectList newEffects = new EffectList();
        foreach (EffectList.EffectData? effect in __instance.m_deathEffects.m_effectPrefabs)
        {
            if (effect.m_prefab == null)
            {
                continue;
            }

            if (effect.m_prefab.name.Contains("vfx_"))
            {
                continue;
            }

            newEffects.m_effectPrefabs.AddItem(effect);
        }

        __instance.m_deathEffects = newEffects;
    }
}

[HarmonyPatch(typeof(Ragdoll), nameof(Ragdoll.DestroyNow))]
static class RagdollDestroyNowPatch
{
    static void Prefix(Ragdoll __instance)
    {
        if(IsConflictingModLoaded(ConflictingModConstants.NoCreatureDust)) return;
        if (__instance.m_removeEffect == null)
        {
            return;
        }

        // Remove the vfx from the deathEffects and only the vfx
        EffectList newEffects = new EffectList();
        foreach (EffectList.EffectData? effect in __instance.m_removeEffect.m_effectPrefabs)
        {
            if (effect.m_prefab == null)
            {
                continue;
            }

            if (effect.m_prefab.name.Contains("vfx_corpse"))
            {
                continue;
            }

            if (effect.m_prefab.name.Contains("vfx_") && RemoveAllRagdollVFX.Value.IsOn())
            {
                continue;
            }

            newEffects.m_effectPrefabs.AddItem(effect);
        }

        __instance.m_removeEffect = newEffects;
    }
}