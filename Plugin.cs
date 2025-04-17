using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BreatheEasy.Camera_Effects;
using BreatheEasy.Dust;
using HarmonyLib;
using JetBrains.Annotations;
using ServerSync;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BreatheEasy
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class BreatheEasyPlugin : BaseUnityPlugin
    {
        internal const string ModName = "BreatheEasy";
        internal const string ModVersion = "1.0.1";
        internal const string Author = "RandomSteve";
        private const string ModGUID = $"{Author}.{ModName}";
        private static string ConfigFileName = $"{ModGUID}.cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        private readonly Harmony _harmony = new(ModGUID);
        public static readonly ManualLogSource BreatheEasyLogger = BepInEx.Logging.Logger.CreateLogSource(ModName);
        private FileSystemWatcher _watcher = null!;
        private readonly object _reloadLock = new();
        private DateTime _lastConfigReloadTime;
        private const long RELOAD_DELAY = 10000000; // One second
        internal const string VfxKeyword = "vfx";

        private static readonly ConfigSync ConfigSync = new(ModGUID) { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion, ModRequired = true };

        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        public static class ConflictingModConstants
        {
            private const string Azu = "Azumatt";
            internal const string NoBuildDust = $"{Azu}.NoBuildDust";
            internal const string NoCreatureDust = $"{Azu}.NoCreatureDust";
            internal const string NoCultivatorDust = $"{Azu}.NoCultivatorDust";
            internal const string NoHoeDust = $"{Azu}.NoHoeDust";
            internal const string NoTreeDust = $"{Azu}.NoTreeDust";
            internal const string NoWeaponDust = $"{Azu}.NoWeaponDust";
            internal const string TrueInstantLootDrop = $"{Azu}.TrueInstantLootDrop";
        }

        public void Awake()
        {
            bool saveOnSet = Config.SaveOnConfigSet;
            Config.SaveOnConfigSet = false;

            // Lens Dirt
            RemoveLensDirt = config("1 - Lens Dirt", "Remove Lens Dirt", Toggle.On, "If on, this will remove the lens dirt effect from the camera. If off, it will not.", false);
            RemoveLensDirt.SettingChanged += (_, _) => LensDirt.OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

            // Ashlands Heat Wave
            RemoveAshlandsHeatWave = config("1 - Ashlands Heat Wave", "Remove Ashlands Heat Wave", Toggle.On, "If on, this will remove the heat wave effect from the camera. If off, it will not.", false);
            RemoveAshlandsHeatWave.SettingChanged += (_, _) => AshlandsHeatWave.OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);


            DisableSmoke = config("1 - Smoke", "Disable Smoke", Toggle.On, "If on, the smoke from cooking stations, fireplaces and smelters will be disabled.", false);
            FireplacesStayLit = config("1 - Fireplace", "Stay Lit", Toggle.On, "If on, fireplaces will stay lit indefinitely. This is anything that uses the Fireplace component.");

            // No Build Dust
            RemoveAllVFX_Nbd = config("1 - No Build Dust", "Remove All Effects", Toggle.Off, "Removes all visual effects from when a building is destroyed, not just the vanilla dust.", false);

            // No Creature Dust
            RemoveAllVFX_Ncd = config("1 - No Creature Dust", "Remove All Effects", Toggle.Off, "Removes all visual effects from when a creature dies, not just the vanilla dust.", false);
            RemoveAllRagdollVFX = config("1 -  No Creature Dust", "Remove All Ragdoll Effects", Toggle.Off, "Removes all ragdoll effects from creatures, not just the vanilla dust. Just in case mods add more you want to remove.", false);
            RemoveCreatureDust = config("1 -  No Creature Dust", "Remove Creature Dust", Toggle.Off, "Removes vanilla dust/poof. Overridden by Remove All Ragdoll Effects & Remove All Effects. Meaning, if you have those on, this will do nothing.", false);

            // No Cultivator Dust
            RemoveAllVFX_Ncultd = config("1 - No Cultivator Dust", "Remove All Effects", Toggle.On, "Removes all visual effects from when a cultivator is used, not just the vanilla dust.", false);

            // No Hoe Dust
            RemoveAllVFX_Nhd = config("1 - No Hoe Dust", "Remove All Effects", Toggle.On, "Removes all visual effects from when a hoe is used, not just the vanilla dust.", false);

            // No Tree Dust
            DestroyedEffectsEnabled = config("1 - No Tree Dust", "Destroy Effects", Toggle.Off, "Enable/Disable destroy effects on the trees", false);
            HitEffectsEnabled = config("1 - No Tree Dust", "Hit Effects", Toggle.Off, "Enable/Disable hit effects on the trees", false);
            RespawnEffectsEnabled = config("1 - No Tree Dust", "Respawn Effects", Toggle.Off, "Enable/Disable respawn effects on the trees", false);

            DestroyedEffectsEnabled.SettingChanged += (_, _) => ZNetSceneAwakePatch_NoTreeDust.UpdateAllDestroyedEffects();
            HitEffectsEnabled.SettingChanged += (_, _) => ZNetSceneAwakePatch_NoTreeDust.UpdateAllHitEffects();
            RespawnEffectsEnabled.SettingChanged += (_, _) => ZNetSceneAwakePatch_NoTreeDust.UpdateAllRespawnEffects();

            // No Weapon Dust
            RemoveTriggerEffects = config("1 - No Weapon Dust", "Remove Trigger Effects", Toggle.On, "If on, this will remove the dust effect from weapons (On Trigger). If off, it will not.", false);
            RemoveHitTerrainEffects = config("1 - No Weapon Dust", "Remove Hit Terrain Effects", Toggle.On, "If on, this will remove the dust effect from weapons (On Hit Terrain). If off, it will not.", false);
            RemoveHitEffects = config("1 - No Weapon Dust", "Remove Hit Effects", Toggle.On, "If on, this will remove the dust effect from weapons (On Hit). If off, it will not.", false);
            RemoveStartEffects = config("1 - No Weapon Dust", "Remove Start Effects", Toggle.On, "If on, this will remove the dust effect from weapons (On Start). If off, it will not.", false);

            SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(LensDirt.OnSceneLoaded);
            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();

            Config.Save();
            if (saveOnSet)
            {
                Config.SaveOnConfigSet = saveOnSet;
            }
        }

        public static bool IsConflictingModLoaded(string guid)
        {
            return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(guid);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(LensDirt.OnSceneLoaded);
            SaveWithRespectToConfigSet();
            _watcher?.Dispose();
        }

        private void SetupWatcher()
        {
            _watcher = new FileSystemWatcher(Paths.ConfigPath, ConfigFileName);
            _watcher.Changed += ReadConfigValues;
            _watcher.Created += ReadConfigValues;
            _watcher.Renamed += ReadConfigValues;
            _watcher.IncludeSubdirectories = true;
            _watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            _watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            DateTime now = DateTime.Now;
            long time = now.Ticks - _lastConfigReloadTime.Ticks;
            if (time < RELOAD_DELAY)
            {
                return;
            }

            lock (_reloadLock)
            {
                if (!File.Exists(ConfigFileFullPath))
                {
                    BreatheEasyLogger.LogWarning("Config file does not exist. Skipping reload.");
                    return;
                }

                try
                {
                    BreatheEasyLogger.LogDebug("Reloading configuration...");
                    SaveWithRespectToConfigSet(true);
                    BreatheEasyLogger.LogInfo("Configuration reload complete.");
                }
                catch (Exception ex)
                {
                    BreatheEasyLogger.LogError($"Error reloading configuration: {ex.Message}");
                }
            }

            _lastConfigReloadTime = now;
        }

        private void SaveWithRespectToConfigSet(bool reload = false)
        {
            bool originalSaveOnSet = Config.SaveOnConfigSet;
            Config.SaveOnConfigSet = false;
            if (reload)
                Config.Reload();
            Config.Save();
            if (originalSaveOnSet)
            {
                Config.SaveOnConfigSet = originalSaveOnSet;
            }
        }


        #region ConfigOptions

        public static ConfigEntry<Toggle> RemoveLensDirt = null!;

        public static ConfigEntry<Toggle> RemoveAshlandsHeatWave = null!;

        public static ConfigEntry<Toggle> DisableSmoke = null!;
        public static ConfigEntry<Toggle> FireplacesStayLit = null!;

        public static ConfigEntry<Toggle> RemoveAllVFX_Nbd = null!;


        public static ConfigEntry<Toggle> RemoveAllVFX_Ncd = null!;
        public static ConfigEntry<Toggle> RemoveAllRagdollVFX = null!;
        public static ConfigEntry<Toggle> RemoveCreatureDust = null!;

        public static ConfigEntry<Toggle> RemoveAllVFX_Ncultd = null!;

        public static ConfigEntry<Toggle> RemoveAllVFX_Nhd = null!;

        internal static ConfigEntry<Toggle> RemoveTriggerEffects = null!;
        internal static ConfigEntry<Toggle> RemoveHitTerrainEffects = null!;
        internal static ConfigEntry<Toggle> RemoveHitEffects = null!;
        internal static ConfigEntry<Toggle> RemoveStartEffects = null!;

        public static ConfigEntry<Toggle> DestroyedEffectsEnabled = null!;
        public static ConfigEntry<Toggle> HitEffectsEnabled = null!;
        public static ConfigEntry<Toggle> RespawnEffectsEnabled = null!;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription = new(description.Description + (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"), description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description, bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        #endregion
    }

    public static class ToggleExtentions
    {
        public static bool IsOn(this BreatheEasyPlugin.Toggle value)
        {
            return value == BreatheEasyPlugin.Toggle.On;
        }

        public static bool IsOff(this BreatheEasyPlugin.Toggle value)
        {
            return value == BreatheEasyPlugin.Toggle.Off;
        }
    }
}