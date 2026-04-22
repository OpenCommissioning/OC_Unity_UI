# Outline rendering (URP)

## Source

Integrated stack: **`com.cqf.outline`** (URP renderer feature + volume component).  
Upstream: [CristianQiu / Unity-URP-Outline](https://github.com/CristianQiu/Unity-URP-Outline) (MIT).

## What it does

Single outline pass wired to the **URP volume** system. Outlined objects are selected by **Rendering Layer Mask** (four named layers). No extra per-object outline components are required beyond assigning renderers to those layers.

**Requirements:** Unity **6000.3** or newer, **URP**, **post-processing** enabled on the active **Camera** and on the **URP Renderer** asset.

## Project setup

1. **Rendering layers** — In **Edit → Project Settings → Tags and Layers**, define four **Rendering Layers** named exactly: `Outline_1`, `Outline_2`, `Outline_3`, `Outline_4`.
2. **Renderer** — Add **`OutlineRendererFeature`** to the **URP Renderer** asset used by the pipeline.
3. **Volume** — Add a **Volume** (global or local) and add override **Custom → Outline** (outline volume component).
4. **Objects** — For each renderer that should show an outline, set **Rendering Layer Mask** so it includes one of `Outline_1`–`Outline_4` (Inspector or code). Each layer corresponds to its own block of parameters on the volume override, except **border width** (see below).

## Volume vs layers

- **Per-layer styling:** `Outline_1` … `Outline_4` each have independent settings on the volume (colors, intensity, etc., per upstream design).
- **Shared border width:** **Border Size** is one value for all four layers (performance / implementation constraint).

## Performance (`ForceCullPass`)

`OutlineRendererFeature` exposes a static **`ForceCullPass`**. Set it to **`true`** to skip the outline passes entirely when you know **no** renderers are using `Outline_1`–`Outline_4`.  
There is no built-in render-graph hook to flip this automatically; the application should track outlined renderers and toggle the flag.

## Limitations

- **No per-object outline width** — only the shared **Border Size**.
- **No alpha clip** on outlined materials for this path.
- **No correct outline on heavy vertex animation** — vertex-shader–displaced geometry is not supported.

## This package

`OC.UI` uses the same layer naming contract. The **`Outline`** behaviour (`OC.UI.Interactions`) maps interaction state to **`Outline_1`** (hover) and **`Outline_2`** (selection) by default; keep project rendering layer names in sync with serialized fields on that component.
