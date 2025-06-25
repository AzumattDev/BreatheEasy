using System.Collections.Generic;
using HarmonyLib;
using static BreatheEasy.BreatheEasyPlugin;

namespace BreatheEasy.Dust;

[HarmonyPatch(typeof(Character), nameof(Character.OnDeath))]
static class CharacterOnDeathPatch
{
    public static void Prefix(Character __instance)
    {
        if (IsConflictingModLoaded(ConflictingModConstants.NoCreatureDust)) return;

        if (__instance.IsPlayer() || __instance.m_deathEffects == null)
        {
            return;
        }

        // Early return if neither relevant option is enabled
        if (RemoveAllVFX_Ncd.Value.IsOff() && RemoveAllRagdollVFX.Value.IsOff())
        {
            return;
        }

        List<EffectList.EffectData> filteredEffects = [];

        foreach (EffectList.EffectData effect in __instance.m_deathEffects.m_effectPrefabs)
        {
            if (effect.m_prefab == null)
            {
                filteredEffects.Add(effect);
                continue;
            }

            string effectName = effect.m_prefab.name;
            bool shouldSkip = false;

            // Check for ragdoll effects (case insensitive) - RemoveAllRagdollVFX has 100% control
            if (effectName.ToLower().Contains("ragdoll"))
            {
                if (RemoveAllRagdollVFX.Value.IsOn())
                {
                    shouldSkip = true;
                }
            }
            // Check for other vfx effects - only if RemoveAllVFX is on AND it's not a ragdoll
            else if (effectName.Contains("vfx_") && RemoveAllVFX_Ncd.Value.IsOn())
            {
                shouldSkip = true;
            }

            if (!shouldSkip)
            {
                filteredEffects.Add(effect);
            }
        }

        // Only modify if we actually removed something
        if (filteredEffects.Count != __instance.m_deathEffects.m_effectPrefabs.Length)
        {
            __instance.m_deathEffects.m_effectPrefabs = filteredEffects.ToArray();
        }
    }
}

[HarmonyPatch(typeof(Ragdoll), nameof(Ragdoll.DestroyNow))]
static class RagdollDestroyNowPatch
{
    static void Prefix(Ragdoll __instance)
    {
        if (IsConflictingModLoaded(ConflictingModConstants.NoCreatureDust)) return;

        if (__instance.m_removeEffect == null)
        {
            return;
        }

        if (RemoveAllRagdollVFX.Value.IsOff() && RemoveAllVFX_Ncd.Value.IsOff() && RemoveCreatureDust.Value.IsOff())
        {
            return;
        }

        // Use List<> for easier manipulation, then convert to array
        List<EffectList.EffectData> filteredEffects = [];

        foreach (EffectList.EffectData effect in __instance.m_removeEffect.m_effectPrefabs)
        {
            bool shouldSkip = false;

            if (effect.m_prefab is not null)
            {
                string effectName = effect.m_prefab.name;
                if (RemoveAllVFX_Ncd.Value.IsOn() && effectName.Contains("vfx_") || RemoveCreatureDust.Value.IsOn() && RemoveAllVFX_Ncd.Value.IsOff() && RemoveAllRagdollVFX.Value.IsOff() && effectName.Contains("vfx_corpse"))
                {
                    shouldSkip = true;
                }
            }

            if (!shouldSkip)
            {
                filteredEffects.Add(effect);
            }
        }

        // Only modify if we actually removed something
        if (filteredEffects.Count != __instance.m_removeEffect.m_effectPrefabs.Length)
        {
            __instance.m_removeEffect.m_effectPrefabs = filteredEffects.ToArray();
        }
    }
}