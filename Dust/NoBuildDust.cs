using System.Linq;
using HarmonyLib;
using UnityEngine;
using static BreatheEasy.BreatheEasyPlugin;

namespace BreatheEasy.Dust;

[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
static class ZNetSceneAwakePatch
{
    [HarmonyPriority(Priority.Last)]
    static void Postfix(ZNetScene __instance)
    {
        if(IsConflictingModLoaded(ConflictingModConstants.NoBuildDust)) return;
        if(RemoveAllVFX_Nbd.Value.IsOff()) return;
        BreatheEasyLogger.LogWarning("ZNetScene Awake Postfix, turning off build dust");
        foreach (GameObject instanceMPrefab in __instance.m_prefabs)
        {
            instanceMPrefab.TryGetComponent(out Piece? piece);
            instanceMPrefab.TryGetComponent(out WearNTear? wearNTear);
            if (piece != null)
            {
                try
                {
                    if (piece.m_placeEffect.m_effectPrefabs.Length > 0)
                    {
                        piece.m_placeEffect.m_effectPrefabs = piece.m_placeEffect.m_effectPrefabs.Where(effect => !effect.m_prefab.name.Contains("vfx")).ToArray();
                    }
                }
                catch
                {
                    BreatheEasyLogger.LogWarning($"Couldn't replace the placement effects for: {Localization.instance.Localize(piece.m_name)} [{piece.name}]");
                }
            }

            if (wearNTear != null)
            {
                try
                {
                    if (wearNTear.m_destroyedEffect.m_effectPrefabs.Length > 0)
                    {
                        wearNTear.m_destroyedEffect.m_effectPrefabs = wearNTear.m_destroyedEffect.m_effectPrefabs.Where(effect => !effect.m_prefab.name.Contains("vfx")).ToArray();
                    }
                }
                catch
                {
                    BreatheEasyLogger.LogWarning($"Couldn't replace the destruction effects for: {Utils.GetPrefabName(wearNTear.transform.root.gameObject)}");
                }
            }
        }
    }
}