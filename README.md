# Varneon's Unity Editor Utilities
Collection of useful Unity Editor utilities ranging from small macros to full toolkits

### [Installation Instructions](#installation)

---

# Features

## Asset Menu Actions

Context menu actions available in the Unity Editor's `Project` window

| Action | Description | Path |
| - | - | - |
| `Copy Full Path` | Copies the full path of an asset instead of the relative asset database path | `Assets/Copy Full Path` |
| `Copy GUID` | Copies the GUID of an asset | `Assets/Copy GUID` |

## Macros

Menu actions available in the Unity Editor's toolbar at `Varneon` > `Macros`

| Macro | Description | Path |
| - | - | - |
| `Delete Empty Scripts` | Deletes all empty scripts in a hierarchy | `Varneon/Macros/Delete Empty Scripts` |

## Context Menu Actions

Context menu actions available for different types of components

<details>
<summary>

## `Joint`</summary>

| Action | Description |
| - | - |
| `Set Connected Body To Parent` | Sets the nearest Rigidbody in any parent as connected body of the joint |
| `Set Connected Body To Child` | Sets the nearest Rigidbody in any child as connected body of the joint |

</details><details>
<summary>

## `LODGroup`</summary>

| Action | Description |
| - | - |
| `Remove Missing Renderers` | Removes missing renderer references from LODGroup. Prevents severe editor [errors](https://github.com/Varneon/UnityEditorUtilities/issues/10) |

</details>

---

# Installation

<details><summary>

### Import with [VRChat Creator Companion](https://vcc.docs.vrchat.com/vpm/packages#user-packages):</summary>

> 1. Download `com.varneon.editor-utilities.zip` from [here](https://github.com/Varneon/UnityEditorUtilities/releases/latest)
> 2. Unpack the .zip somewhere
> 3. In VRChat Creator Companion, navigate to `Settings` > `User Packages` > `Add`
> 4. Navigate to the unpacked folder, `com.varneon.vudon.repository-template` and click `Select Folder`
> 5. `Varneon's Editor Utilities` should now be visible under `Local User Packages` in the project view in VRChat Creator Companion
> 6. Click `Add`

</details><details><summary>

### Import with [Unity Package Manager (git)](https://docs.unity3d.com/2019.4/Documentation/Manual/upm-ui-giturl.html):</summary>

> 1. In the Unity toolbar, select `Window` > `Package Manager` > `[+]` > `Add package from git URL...` 
> 2. Copy and paste the following link into the URL input field: <pre lang="md">https://github.com/Varneon/UnityEditorUtilities.git?path=/Packages/com.varneon.editor-utilities</pre>

</details><details><summary>

### Import from [Unitypackage](https://docs.unity3d.com/2019.4/Documentation/Manual/AssetPackagesImport.html):</summary>

> 1. Download latest `com.varneon.editor-utilities.unitypackage` from [here](https://github.com/Varneon/UnityEditorUtilities/releases/latest)
> 2. Import the downloaded .unitypackage into your Unity project

</details>

<div align="center">

## Developed by Varneon with :hearts:

![Twitter Follow](https://img.shields.io/twitter/follow/Varneon?color=%231c9cea&label=%40Varneon&logo=Twitter&style=for-the-badge)
![YouTube Channel Subscribers](https://img.shields.io/youtube/channel/subscribers/UCKTxeXy7gyaxr-YA9qGWOYg?color=%23FF0000&label=Varneon&logo=YouTube&style=for-the-badge)
![GitHub followers](https://img.shields.io/github/followers/Varneon?color=%23303030&label=Varneon&logo=GitHub&style=for-the-badge)

</div>
