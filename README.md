# Open Commissioning UI

Unity package **`com.spiratec.oc.ui`** — runtime UI for Open Commissioning.

## Adding the package

Add Git dependencies in the Unity project’s **`Packages/manifest.json`**.

```json
{
  "dependencies": {
    "com.spiratec.oc.ui": "https://github.com/OpenCommissioning/OC_Unity_UI.git",
    "com.cqf.outline": "https://github.com/CristianQiu/Unity-URP-Outline.git"
  }
}
```

## Dependencies

**Engine**

- Unity **6000.3** or newer ([`package.json`](package.json)).
- **URP**: `Unity.RenderPipelines.Core.Runtime` and `Unity.RenderPipelines.Universal.Runtime` ([`Runtime/OC.UI.asmdef`](Runtime/OC.UI.asmdef)).

**Unity / registry packages** (asmdef references)

- TextMeshPro  
- Cinemachine **3.0–3.1.5** (defines `CINEMACHINE_3_0_OR_NEWER` when in range)  
- Input System **≥ 1.17.0** (defines `INPUTSYSTEM_1_17_0_OR_NEWER`)  

**Sibling / host**

- Assembly **`OC`** — not part of this package; the consuming project must supply it (e.g. `OC.Interactions` used by UI code).

**Outline rendering (`com.cqf.outline`)**

- URP **renderer feature** + volume override **Custom → Outline**; driven by **rendering layers** named **`Outline_1` … `Outline_4`** (contract from integrated stack).
- Enable **post-processing** on the **camera** and on the **URP Renderer** asset.
- Optional: `OutlineRendererFeature.ForceCullPass = true` when nothing uses the outline layers, to skip passes.
- Provenance: [Unity-URP-Outline](https://github.com/CristianQiu/Unity-URP-Outline) (MIT). Details: [Documentation/Outline.md](Documentation/Outline.md).

**Limitations (outline)**

- No distinct outline width per object (shared border size).  
- No alpha clip.  
- Outlines do not follow vertex-animated meshes.

## Setup

1. Ensure **`com.spiratec.oc.ui`** and **`com.cqf.outline`** resolve (see **Adding the package**).
2. Ensure **URP** is the active pipeline; merge or replicate renderer settings so the active renderer includes **`OutlineRendererFeature`**. Reference assets in this package: `Runtime/Settings/OC URP Renderer.asset`, `Runtime/Settings/OC Volume.asset`.
3. Add a **Volume** with the **Outline** override where you need global/local tuning.
4. **Edit → Project Settings → Tags and Layers → Rendering Layers**: add **`Outline_1`–`Outline_4`**. Align names with the **`Outline`** component on interactables (defaults: hover `Outline_1`, selection `Outline_2`).
5. On the **Camera**, enable **Post Processing**.
