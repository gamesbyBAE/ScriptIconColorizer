# Script Icon Customiser

**Customise script assets icon and/or their tint color** in the Unity Editor.

- Package: `com.basement-experiments.script-icon-customiser`

- Version: `0.1.2`
- Author: Mayank _'DerperDoing'_ Bagchi (www.mayankbagchi.dev)
- Unity Compatibility: Unity 2021.3 LTS or newer (recommended Unity 6000.3)
- License: MIT

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![Unity](https://img.shields.io/badge/unity-%23000000.svg?style=for-the-badge&logo=unity&logoColor=white)
![GitHub repo size](https://img.shields.io/github/repo-size/gamesbyBAE/script-icon-customiser?style=for-the-badge)

## 🔍 Overview

`Script Icon Customiser` provides a small Unity Editor utility window that allows you to:-

1. Change the icon displayed for script (C#) asset files in the Unity Project window.
2. Revert back to default Unity script icon.

You can customise & apply a new icon to one or multiple selected scripts by either:-

1. choosing a custom icon texture from you project assets and/or apply a tint color to further customise it,
2. or simply changing the color of the default Unity script icon.

## ⭐ Key Features

- The icon is reflected in the Unity Inspector and everywhere in Unity Editor.
- Move your custom Texture2D icon asset within the project safely without script icons reverting to default.
- Quick restore to Unity default icon.
- Clean UI built with UIElements (VisualElement + USS).

## 📦 Installation

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

4. Save the file and return to Unity.<br>
   Unity will automatically resolve and install the package.

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

## 🖱 Usage

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

## 🗂️ UI & Code Reference

Main files:

- [ScriptIconChangerWindow.cs](Editor/Scripts/ScriptIconChangerWindow.cs) - EditorWindow and menu hooks.
- [WindowPresenter.cs](Editor/Scripts/Presenter/WindowPresenter.cs) - Creates [views](Editor/Scripts/Views), builds layout, coordinates events & views.
- [CoreService.cs](Editor/Scripts/Core/CoreService.cs) - High-level orchestrator for core features.
  - [IconGenerator.cs](Editor/Scripts/Core/Services/IconGenerator.cs) - Fetches the default Unity icon, generates only the tinted icons (copies and tints) else passes the stock user Texture2D.
  - [IconSaver.cs](Editor/Scripts/Core/Services/IconSaver.cs) - Saves generated icons under `Assets/BasementExperiments/CustomScriptIcons` folder.
  - [IconApplier.cs](Editor/Scripts/Core/Services/IconApplier.cs) - Applies icon using `MonoImporter`.
- [ScriptIconCustomiser.uss](Editor/Stylesheets/ScriptIconCustomiser.uss) - Unity Style Sheet (USS) for customising the window's appearance.

## ⚠️ Notes & Tips

- The utility modifies the icon set via `MonoImporter.SetIcon` followed by `SaveAndReimport()`, which may trigger a reimport for the asset(s). This is normal.
- Icon textures that are tinted and not in their stock form are saved to `Assets/BasementExperiments/CustomScriptIcons`. It's necessary to save in the project because Unity needs the GUID to map the icon to the project asset.<br>
  For the case, you installed the package from your local disk, it's possible to directly save the icons in the package directory itself without bloating your actual Unity project.

## 🛠 Development & Contribution

- Clone the repo and install it as local package in Unity (2021.3 LTS or newer).<br>
  Check [Option #3 of Installation](#option-3-install-via-local-package-clone-repository) for more info.
- Explore the code under `Editor/Scripts` for logic and `Editor/Stylesheets` to modify styles.
- Run the window from the menu `Assets ➜ Script Icon ➜ Change Icon`.
- Contributions welcome via PRs - fixes, performance improvements, or feature additions.

## 📜 License

MIT, see [LICENSE](LICENSE) in this folder.

## 🫱🏼‍🫲🏽 Contact

Mayank _'DerperDoing'_ Bagchi - www.mayankbagchi.dev

![YouTube Channel Views](https://img.shields.io/youtube/channel/views/UCnFsX4GcmDx1hDbpTC9NYow)
