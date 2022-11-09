# Varneon's Unity Editor Utilities
Collection of useful Unity Editor utilities ranging from small macros to full toolkits

### [Import Instructions](#import-instructions-1)

---

# Features

## Asset Menu Actions
| Action | Description | Path |
| - | - | - |
| `Copy Full Path` | Copies the full path of an asset instead of the relative asset database path | `Assets/Copy Full Path` |
| `Copy GUID` | Copies the GUID of an asset | `Assets/Copy GUID` |

## Macros
| Macro | Description | Path |
| - | - | - |
| `Delete Empty Scripts` | Deletes all empty scripts in a hierarchy | `Varneon/Macros/Delete Empty Scripts` |

## Context Menu Actions

Find the available context menu actions for each component type below:

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

# Import Instructions

<details><summary>

## Import with [Unity Package Manager (git)](https://docs.unity3d.com/2019.4/Documentation/Manual/upm-ui-giturl.html):</summary>

> 1. Navigate to your toolbar: `Window` > `Package Manager` > `[+]` > `Add package from git URL...` and paste in: `https://github.com/Varneon/UnityEditorUtilities.git`

</details><details><summary>

## Import from [Unitypackage](https://docs.unity3d.com/2019.4/Documentation/Manual/AssetPackagesImport.html):</summary>

> 1. Download latest Unity Editor Utilities from [here](https://github.com/Varneon/UnityEditorUtilities/releases/latest)
> 2. Import the downloaded .unitypackage into your Unity project

</details><details><summary>

## Import with [VRChat Creator Companion](https://vcc.docs.vrchat.com/vpm/packages#user-packages):</summary>

> 1. Download the the repository's .zip [here](https://github.com/Varneon/UnityEditorUtilities/archive/refs/heads/main.zip)
> 2. Unpack the .zip somewhere
> 3. In VRChat Creator Companion, navigate to `Settings` > `User Packages` > `Add`
> 4. Navigate to the unpacked folder, `com.varneon.editor-utilities` and click `Select Folder`
> 5. `Varneon's Editor Utilities` should now be visible under `Local User Packages` in the project view in VRChat Creator Companion

</details>
