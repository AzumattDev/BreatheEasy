# Description

## Removes smoke, dust, camera effects (heat wave in ashlands), and keeps things lit so you can breathe easy. Azumatt's NoDust series is included in this mod.

#### Azumatt's NoDust series override's this mod. The patches that handle that will not run if Azumatt's NoDust series (or singular mods) is/are installed.

`Version checks with itself. If installed on the server, it will kick clients who do not have it installed. This is mostly for the Stay Lit option, since that's the only one synced with the server. Keep it client side only if you simply want to use the other options.`

`This mod uses ServerSync, if installed on the server and all clients, it will sync configs that have the [Synced with Server] tag to the client`

`This mod uses a file watcher. If the configuration file is not changed with BepInEx Configuration manager, but changed in the file directly on the server, upon file save, it will sync the changes to all clients.`

## Configuration Options

### 🎥 Camera Effects

| Setting                       | Default | Description                                              |
|-------------------------------|---------|----------------------------------------------------------|
| **Remove Lens Dirt**          | On      | Removes the lens dirt effect from the camera             |
| **Remove Ashlands Heat Wave** | On      | Removes the heat wave effect from the camera in Ashlands |

### 🔥 Smoke & Fire

| Setting           | Default | Description                                                   |
|-------------------|---------|---------------------------------------------------------------|
| **Disable Smoke** | On      | Disables smoke from cooking stations, fireplaces and smelters |

### 🏠 Fireplace Controls

| Setting                     | Default | Description                                                                        |
|-----------------------------|---------|------------------------------------------------------------------------------------|
| **Stay Lit**                | On      | Fireplaces will stay lit indefinitely (anything that uses the Fireplace component) |
| **Fireplaces Use Fuel**     | Off     | When enabled, fireplaces will still consume fuel even when Stay Lit is enabled     |
| **Show Fuel in Hover Text** | Off     | Shows fuel levels in hover text even when Stay Lit is enabled                      |

### ⚒️ Smelters & Cooking Stations

| Setting                       | Default | Description                                                                          |
|-------------------------------|---------|--------------------------------------------------------------------------------------|
| **Smelters Use Fuel**         | Off     | When enabled, smelters will still consume fuel (coal) even when Stay Lit is enabled  |
| **Cooking Stations Use Fuel** | Off     | When enabled, cooking stations will still consume fuel even when Stay Lit is enabled |

### 🏗️ Building Effects

| Setting                      | Default | Description                                                                    |
|------------------------------|---------|--------------------------------------------------------------------------------|
| **Remove All Build Effects** | Off     | Removes all visual effects when buildings are destroyed, not just vanilla dust |

### 💀 Creature Death Effects

| Setting                        | Default | Description                                                                                                                                                                                                                                                                             |
|--------------------------------|---------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Remove All Effects**         | Off     | Removes ALL visual effects when creatures die (blood, dust, particles, etc.) but keeps the ragdoll body. Use this if you want clean deaths but still want loot to drop normally from ragdolls                                                                                           |
| **Remove All Ragdoll Effects** | Off     | Removes the ragdoll body entirely when creatures die. This makes loot drop instantly since there's no ragdoll effects to wait for. Also removes any dust/particles that would normally appear when the ragdoll despawns                                                                 |
| **Remove Creature Dust**       | Off     | Removes only the vanilla dust/poof effect when ragdolls despawn. This is the most minimal option - keeps ragdolls and all other effects, just removes the small dust cloud. **Note:** This setting is ignored if either 'Remove All Effects' or 'Remove All Ragdoll Effects' is enabled |

### 🌱 Tool Effects

| Setting                           | Default | Description                                                                 |
|-----------------------------------|---------|-----------------------------------------------------------------------------|
| **Remove All Cultivator Effects** | On      | Removes all visual effects when using the cultivator, not just vanilla dust |
| **Remove All Hoe Effects**        | On      | Removes all visual effects when using the hoe, not just vanilla dust        |

### 🌳 Tree Effects

| Setting             | Default | Description                             |
|---------------------|---------|-----------------------------------------|
| **Destroy Effects** | Off     | Enable/Disable destroy effects on trees |
| **Hit Effects**     | Off     | Enable/Disable hit effects on trees     |
| **Respawn Effects** | Off     | Enable/Disable respawn effects on trees |

### ⚔️ Weapon Effects

| Setting                        | Default | Description                                        |
|--------------------------------|---------|----------------------------------------------------|
| **Remove Trigger Effects**     | On      | Removes dust effects from weapons (On Trigger)     |
| **Remove Hit Terrain Effects** | On      | Removes dust effects from weapons (On Hit Terrain) |
| **Remove Hit Effects**         | On      | Removes dust effects from weapons (On Hit)         |
| **Remove Start Effects**       | On      | Removes dust effects from weapons (On Start)       |

---