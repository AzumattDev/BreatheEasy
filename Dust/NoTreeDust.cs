using System.Linq;
using HarmonyLib;
using UnityEngine;
using static BreatheEasy.BreatheEasyPlugin;

namespace BreatheEasy.Dust;

[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
static class ZNetSceneAwakePatch_NoTreeDust
{
    [HarmonyPriority(Priority.Last)]
    static void Postfix(ZNetScene __instance)
    {
        if (IsConflictingModLoaded(ConflictingModConstants.NoTreeDust)) return;
        BreatheEasyLogger.LogDebug("ZNetScene Awake Postfix, turning off tree dust");
        foreach (GameObject prefab in __instance.m_prefabs)
        {
            if (prefab.TryGetComponent(out TreeBase treeBase))
            {
                UpdateEffectList(treeBase.m_destroyedEffect.m_effectPrefabs, DestroyedEffectsEnabled.Value.IsOn());
                UpdateEffectList(treeBase.m_hitEffect.m_effectPrefabs, HitEffectsEnabled.Value.IsOn());
                UpdateEffectList(treeBase.m_respawnEffect.m_effectPrefabs, RespawnEffectsEnabled.Value.IsOn());
            }

            if (prefab.TryGetComponent(out TreeLog treeLog))
            {
                UpdateEffectList(treeLog.m_destroyedEffect.m_effectPrefabs, DestroyedEffectsEnabled.Value.IsOn());
                UpdateEffectList(treeLog.m_hitEffect.m_effectPrefabs, HitEffectsEnabled.Value.IsOn());
            }

            if (!prefab.TryGetComponent(out Destructible destructible) || destructible.m_destructibleType != DestructibleType.Tree) continue;
            UpdateEffectList(destructible.m_destroyedEffect.m_effectPrefabs, DestroyedEffectsEnabled.Value.IsOn());
            UpdateEffectList(destructible.m_hitEffect.m_effectPrefabs, HitEffectsEnabled.Value.IsOn());
        }
    }

    internal static void UpdateAllDestroyedEffects()
    {
        if(IsConflictingModLoaded(ConflictingModConstants.NoTreeDust)) return;
        bool isEnabled = DestroyedEffectsEnabled.Value.IsOn();

        foreach (TreeBase? treeBase in Resources.FindObjectsOfTypeAll<TreeBase>())
        {
            UpdateEffectList(treeBase.m_destroyedEffect.m_effectPrefabs, isEnabled);
        }


        foreach (TreeLog? treeLog in Resources.FindObjectsOfTypeAll<TreeLog>())
        {
            UpdateEffectList(treeLog.m_destroyedEffect.m_effectPrefabs, isEnabled);
        }


        foreach (Destructible? destructible in Resources.FindObjectsOfTypeAll<Destructible>().Where(x => x.m_destructibleType == DestructibleType.Tree))
        {
            UpdateEffectList(destructible.m_destroyedEffect.m_effectPrefabs, isEnabled);
        }
    }

    internal static void UpdateAllHitEffects()
    {
        if(IsConflictingModLoaded(ConflictingModConstants.NoTreeDust)) return;
        bool isEnabled = HitEffectsEnabled.Value.IsOn();

        foreach (TreeBase? treeBase in Resources.FindObjectsOfTypeAll<TreeBase>())
        {
            UpdateEffectList(treeBase.m_hitEffect.m_effectPrefabs, isEnabled);
        }

        foreach (TreeLog? treeLog in Resources.FindObjectsOfTypeAll<TreeLog>())
        {
            UpdateEffectList(treeLog.m_hitEffect.m_effectPrefabs, isEnabled);
        }

        foreach (Destructible? destructible in Resources.FindObjectsOfTypeAll<Destructible>().Where(x => x.m_destructibleType == DestructibleType.Tree))
        {
            UpdateEffectList(destructible.m_hitEffect.m_effectPrefabs, isEnabled);
        }
    }


    internal static void UpdateAllRespawnEffects()
    {
        if(IsConflictingModLoaded(ConflictingModConstants.NoTreeDust)) return;
        bool isEnabled = RespawnEffectsEnabled.Value.IsOn();

        foreach (TreeBase? treeBase in Resources.FindObjectsOfTypeAll<TreeBase>())
        {
            UpdateEffectList(treeBase.m_respawnEffect.m_effectPrefabs, isEnabled);
        }
    }

    internal static void UpdateEffectList(EffectList.EffectData[] effects, bool isEnabled)
    {
        if(IsConflictingModLoaded(ConflictingModConstants.NoTreeDust)) return;
        if (effects == null)
            return;

        foreach (var effect in effects)
        {
            if (effect != null && effect.m_prefab != null && effect.m_prefab.name.Contains(VfxKeyword))
            {
                effect.m_enabled = isEnabled;
            }
        }
    }
}