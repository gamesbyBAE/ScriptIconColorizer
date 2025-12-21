# Script Icon Customiser ![C#](https://img.shields.io/badge/C%23-grey?style=flat) ![Unity](https://img.shields.io/badge/Unity-2021.3%2B-blue?style=flat&logo=unity) ![MIT](https://img.shields.io/badge/license-MIT-blue?style=flat) ![GitHub repo size](https://img.shields.io/github/repo-size/gamesbyBAE/script-icon-customiser?style=flat&color=green)

Customise script assets icon and/or their tint color in the Unity Editor.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Installation](#installation)
  - [Option 1: Unity Package Manager (Recommended)](#option-1-install-via-unity-package-manager-git-url---recommended)
  - [Option 2: manifest.json](#option-2-install-via-manifestjson)
  - [Option 3: Local Package](#option-3-install-via-local-package-clone-repository)
- [Usage](#usage)
  - [Change Icon](#steps-to-apply-custom-icon)
  - [Restore Icon](#steps-to-restore-default-icon)
- [Code References](#code-references)
- [Notes](#notes)
- [Contributions](#contributions)
- [License](#license)
- [Contact](#contact)

## Overview

`Script Icon Customiser` provides a small Unity Editor utility window that allows you to:-

1. Change the icon displayed for script (C#) asset files in the Unity Project window.
2. Revert back to default Unity script icon.

You can customise & apply a new icon to one or multiple selected scripts by either:-

1. choosing a custom icon texture from you project assets and/or apply a tint color to further customise it,
2. or simply changing the color (tint) of the default Unity script icon.

## Key Features

- Clean UI built with UIElements (VisualElement + USS).
- The icon is reflected in the Unity Inspector and everywhere in Unity Editor.
- Move your custom Texture2D icon asset within the project safely without script icons reverting to default.
- Quick restore to Unity default icon.
- Exists only in the Editor, nothing's packaged into the build.

## Installation

> [!NOTE]
> Recommended Unity Editor version `6000.3+`, albeit supports `2021.3+`.\
> Some UI alignments might be iffy in Editor versions below 6000.3 but does the job it's meant to.

#### Option 1: Install via Unity Package Manager (Git URL) - Recommended

This makes it easier to update the package from the Package Manager directly.

1. Open your project in Unity.

2. Go to `Window ➜ Package Manager` or `Window ➜ Package Management ➜ Package Manager`

3. Click the `+` button in the top-left corner.

4. Select `Add package from git URL…` or `Install package from git URL…`

5. Paste this repository URL:

```
https://github.com/gamesbyBAE/script-icon-customiser.git
```

6. Click `Add` or `Install`

7. Unity will download and install the package automatically.

---

#### Option 2: Install via manifest.json

1. Open your Unity project.

2. Navigate to:

```
Path/Of/Your/UnityProject/Packages/manifest.json
```

3. Add the package under dependencies:

```json
{
  "dependencies": {
    "com.basement-experiments.script-icon-customiser": "https://github.com/gamesbyBAE/script-icon-customiser.git"
  }
}
```

4. Save the file and return to Unity.\
   Unity will automatically resolve and install the package.

---

#### Option 3: Install via Local Package (Clone Repository)

1. Clone the repository to your local machine:

```bash
git clone https://github.com/gamesbyBAE/script-icon-customiser.git
```

2. Open your project in Unity

3. Go to `Window ➜ Package Manager` or `Window ➜ Package Management ➜ Package Manager`

4. Click the `+` button in the top-left corner.

5. Select `Add package from disk…` or `Install package from disk...`

6. Browse and navigate to the cloned repository.

7. Select and open the `package.json` file.

8. The package will now be added as a local package.

_Note:_ Local packages are linked to the folder on disk. Any changes made to the package files will immediately reflect in Unity. Hence, good for modding the package and testing.

## Usage

#### Steps to Apply Custom Icon:

1. Select one or more script assets in the Project window.
2. Open the Editor Window:

   - Menu: `Assets ➜ Script Icon ➜ Change Icon`
   - Shortcut: `Cmd/Ctrl + Alt + Shift + C` (%&#c)
   - Right Click Menu: `Right Click ➜ Script Icon ➜ Change Icon`

3. Use the `Image Picker` to choose a custom texture (or choose `None` to use Unity’s default MonoScript icon).
4. Use the `Color Picker` to choose a tint color (optional).
5. `Preview` shows the, you-guessed-it, the preview (result). Adjust color or texture as needed.
6. You can drag & drop more scripts to be modified in the `Target Scripts` area.
7. Click `Apply` to set the icon for selected MonoScript assets.

#### Steps to Restore Default Icon:

1. Select one or more script assets in the Project window.
2. There are two ways to restore:-

   1. Quick Restore:

      - Menu: `Assets ➜ Script Icon ➜ Restore Icon`
      - Shortcut: `Cmd/Ctrl + Alt + Shift + R` (%&#r)
      - Right Click Menu: `Right Click ➜ Script Icon ➜ Restore Icon`

   2. From Editor Window:
      1. Follow any of the below option to open the window:
         - Menu: `Assets ➜ Script Icon ➜ Change Icon`
         - Shortcut: `Cmd/Ctrl + Alt + Shift + C` (%&#c)
         - Right Click Menu: `Right Click ➜ Script Icon ➜ Change Icon`
      2. You can drag & drop more scripts to be modified in the `Target Scripts` area.
      3. Click `Restore` to set the default Unity MonoScript asset icon.

## Code References

- [ScriptIconCustomiser.uss](Editor/Stylesheets/ScriptIconCustomiser.uss) - Unity Style Sheet (USS) for customising the window's appearance.

- [ScriptIconChangerWindow.cs](Editor/Scripts/ScriptIconChangerWindow.cs) - EditorWindow and menu hooks.
- [WindowPresenter.cs](Editor/Scripts/Presenter/WindowPresenter.cs) - Creates [views](Editor/Scripts/Views), builds layout, coordinates events & views.
- [CoreService.cs](Editor/Scripts/Core/CoreService.cs) - High-level orchestrator for core features.
  - [IconGenerator.cs](Editor/Scripts/Core/Services/IconGenerator.cs) - Fetches the default Unity icon, generates only the tinted icons (copies and tints) else passes the stock user Texture2D.
  - [IconSaver.cs](Editor/Scripts/Core/Services/IconSaver.cs) - Saves generated icons under `Assets/BasementExperiments/CustomScriptIcons` folder.
  - [IconApplier.cs](Editor/Scripts/Core/Services/IconApplier.cs) - Applies icon using `MonoImporter`.

## Notes

- The utility modifies the icon set via `MonoImporter.SetIcon` followed by `SaveAndReimport()`, which may trigger a reimport for the asset(s). This is normal.

- Icon textures that are tinted and _not_ in their stock form are saved to the directory, `Assets/BasementExperiments/CustomScriptIcons`.
  - Necessary to save in the project because Unity uses the GUID to map the icon to the project asset.
  - For the case, package is installed from local disk, it's possible to directly save the icons in the package directory itself without bloating your actual Unity project directory.

## Contributions

Contributions welcome via PRs - fixes, performance improvements, or feature additions.

1. Open a new Unity project.
2. Create a new directory in the project's Assets directory, `Assets ➜ <your-directory-name>`
3. Clone the repository into this newly created directory.
4. Explore the code under `Editor/Scripts` for logic and `Editor/Stylesheets` to modify styles.\
   All the scripts _must_ live inside the `Editor` directory.
5. Run the window from the menu, `Assets ➜ Script Icon ➜ Change Icon`.

## License

MIT, see [LICENSE](LICENSE) in this folder.

## Contact

Mayank _'DerperDoing'_ Bagchi\
www.mayankbagchi.dev

![Medium](https://img.shields.io/badge/Medium%20-%20black?style=flat&logo=medium&link=https%3A%2F%2Fderperdoing.medium.com%2F)
![YouTube Channel Views](https://img.shields.io/youtube/channel/views/UCnFsX4GcmDx1hDbpTC9NYow?style=flat&logo=youtube&logoColor=ff0132&label=Views&labelColor=f2f2f2&color=ff0132)
