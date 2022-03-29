# Overview
Unity2Foundry is a toolset that allows you to render maps from within [Unity Editor](https://unity.com/), while also generating walls data. That means you can use tools like [ProBuilder](https://unity.com/features/probuilder) and or get some 3D game assets from the [Unity AssetStore](https://assetstore.unity.com/). There is also an [FoundryVTT module](https://github.com/CoolcatFVTT/Unity2Foundry-Module) the adds support for projected walls which are needed when using isometric or 3D-projected maps. This works by adding dynamic walls at runtime, just in the right position to produce the correct vision cone.

Some screenshots of FoundryVTT scenes made for the [StarTrek Adventures](https://www.modiphius.net/collections/star-trek-adventures) episode 'The Ångström operation' from the second mission compendium. Note that the compatible [Shared Vision module by CDeenen](https://github.com/CDeenen/SharedVision) was also used for those screenshots.
![Image](https://raw.githubusercontent.com/wiki/CoolcatFVTT/Unity2Foundry-Project/images/sta-angstrom-ops.webp)
![Image](https://raw.githubusercontent.com/wiki/CoolcatFVTT/Unity2Foundry-Project/images/sta-angstrom-quarters.webp)

# Features
- Using **Unity 2022.1.0b13 BETA**.
- **Exporter script**: Put it on a Camera in your scene and it will render the image in your desired resolution and save it in WebP format. This will also export walls in the JSON format needed for FoundryVTT or patch them into an existing JSON scene.
- **Wall script**: Used to setup meta-data needed for walls, doors, windows, etc. If you use a [prefab-based](https://docs.unity3d.com/Manual/Prefabs.html) construction-kit that has this already setup, you get pixel-perfectly projected wall-data for your maps almost for free.
- **Door script**: Allows you to quickly open/close doors within Unity. Note that this currently doesn't connect to above wall-data.
- **Some example assets**:
  - Prefab construction-kit for coridors, rooms, doors and windows as well as some maintenance tunnels. These include the meta-data needed for wall-data export. That means you can just use Unity's grid system (and Ctrl+D to duplicate) to quickly setup any layout.
  - Some StarTrek-style consoles, display textures can be easily switched.
  - All assets were build using ProBuilder within Unity, so they can be easily modified as needed.
  - Example scene with basic HDRP effects setup (shadows, ambient occlusion, anti-alias, etc.)

# Credits
Some code/assets by others are included for convenience: 
- [Unity.WebP](https://github.com/netpyoung/unity.webp) by Eunpyoung Kim (MIT License)
- [SimpleJSON](https://github.com/Bunny83/SimpleJSON) by Markus Göbel (MIT License)
- Textures
  - [Seamless Concrete 48](https://www.sharetextures.com/textures/concrete/seamless_concrete_48/) ([CC0](https://creativecommons.org/publicdomain/zero/1.0/) License)
  - [Metal Walkway 010](https://ambientcg.com/a/MetalWalkway010) ([CC0](https://creativecommons.org/publicdomain/zero/1.0/) License)
- Various Unity standard assets
