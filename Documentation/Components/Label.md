# Label

## Purpose

Show text labels in the game view from any MonoBehaviour **Input** script: simple serialized fields, public properties, and OC `IProperty` values, plus an optional Caption.

## Setup

1. Select the object to label and add **Open Commissioning > UI > Label**. A child `Text` object with a `TextMeshPro` component is created automatically.
2. Drag a **script** into **Input**, or click **Find Parent**. Transforms are rejected — scripts (`MonoBehaviour`) only.
3. Tick **Displayed Properties** (simple serialized fields + public properties), or use **Select All** / **Deselect All**. Each ticked row can have an optional **unit** (e.g. `mm`, `%`). Displayed properties and the Caption are mutually exclusive.
4. Choose a **Visibility Mode**, **Face Camera**, and **Reverse Direction** if needed.
5. At runtime, turn on the **Label Tool** in the AppUI toolbar. With `LabelManager` present, all labels start hidden until that tool is on.

**Script:** [`Runtime/Common/Label.cs`](../../Runtime/Common/Label.cs), editor [`Editor/Scripts/LabelEditor.cs`](../../Editor/Scripts/LabelEditor.cs), manager [`Runtime/System/Scripts/LabelManager.cs`](../../Runtime/System/Scripts/LabelManager.cs)

## Input members

The Displayed Properties list includes:

| Kind | Included |
|------|----------|
| Public readable properties | Simple types and `IProperty` (e.g. `Phase`, `IsRunning`, `Target`) |
| Serialized fields | Public or `[SerializeField]` fields of those same types (e.g. `_phase`) |

Skipped: `UnityEngine.Object` references, arrays, and other complex types (keeps conveyor/sensor wiring out of the list).

`IProperty` / `Property<T>` values are unwrapped to `.Value` automatically.

## Visibility

| Mode | Behaviour |
|------|-----------|
| **Always Visible** | Shows the label (when the Label Tool master switch is on). Font size (default `0.2`), font style, and colour are configurable. |
| **Show On Hover** | Shows the label while the object is hovered (and the Label Tool is on). Uses the same `Interaction` hover as the white outline when present; otherwise a physics raycast. |

**Label Tool / LabelManager** is a global master switch for every Label in the scene. It does not disable the Label's GameObject.

**Face Camera** (on by default) rotates the text to face the main camera.

**Reverse Direction** (off by default) turns the text 180° around its vertical axis.

**Text Offset** (default `(0, 0.2, 0)`) lifts the text above the object.

The label text is excluded from the Interaction outline pass.

## Caption

Enter a **Caption** to mix static text with values (only when no displayed properties are selected):

```text
{Name}
Phase: {Phase}
Running: {IsRunning}
```

| Token | Result |
|-------|--------|
| `{Name}` | Name of the label's GameObject |
| `{MemberName}` | Value of a public property or serialized field on Input |
| `{Token:format}` | Same, with a .NET format string (e.g. `{Value:N2}`) |

Unknown tokens are left unchanged. With an empty Caption, every checked property is listed as `Name: value`, one per line.
