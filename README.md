# Window Automation Plugin

Automation plugin for SAL Windows host providing keyboard / mouse macro recording, playback and window inspection utilities for classic Win32 / WinForms desktop environments.

## Features
* Action Project ("Clicker")
  * Record keyboard strokes and mouse clicks into a sequence of typed actions (ActionKey, ActionMouse, ActionText, ActionMethod).
  * Persist projects as JSON (multi-targeted serializer using Newtonsoft.Json for modern TFMs and compatible fallback for .NET 3.5).
  * Validate and replay actions with configurable per–action delays (Timeout) and looping when final ActionMethod returns true.
* Dynamic Method Integration
  * Optional runtime compilation plugin (identified by GUID `425f8b0c-f049-44ee-8375-4cc874d6bf94`) can supply custom methods (ActionMethod) invoked inside sequences.
  * Wrapper `CompilerPlugin` shields host from reflection details and exposes: list, existence check, delete and invoke operations for dynamic methods.
* Window Introspection
  * `WindowInfo` utility to query native HWND properties: class name, caption, rectangle, visibility, icon, module path, parent/child traversal, screen ↔ client coordinate transforms and lightweight screenshot / border highlight drawing.
* Global Input Hooks
  * Unified hook base (`GlobalWindowsHookBase`) supporting keyboard and mouse (or both) low‑level hooks with proper IDisposable pattern and safe unhook.
  * Debounce / anti‑chatter implementations (`GlobalWindowsHookAntiDebounce`, `GlobalWindowsHookAntiDebounceWithTrace`) to suppress spurious rapid key / mouse repeats while optionally aggregating and tracing suppression statistics.
* UI Components
  * WinForms based panel (`PanelWindowClicker`) with list management, property grid editing, inline shortcut configuration, recording / playback toolstrip controls and project file import/export.
* Multi‑Targeting
  * Builds for: `.NET Framework 3.5` (legacy environments) and `net8.0-windows` (modern). Conditional references ensure UIAutomation assemblies only load on net35; WPF desktop framework reference for net8 where required by host ecosystem.
* Packaging / Output
  * Central `Directory.Build.props` routes build output into a shared artifacts directory with target framework appended. For `net8.0-windows` transitive dependencies are copied locally via `CopyLocalLockFileAssemblies=true`.

## Configuration (PluginSettings)
Property | Description
---------|------------
Start | Shortcut to start / stop action playback.
Record | Shortcut to start / stop recording.
AntiDebounceHookType | Flags (None / Keyboard / Mouse) selecting which global anti‑debounce hooks to enable.

Projects are persisted under an internal blob key (`ClickerJson`) and can also be exported/imported as `.clc` JSON files through the UI.

## Action Types
Action | Purpose
-------|--------
ActionKey | Single key press / release representation.
ActionMouse | Mouse button click with screen location.
ActionText | Sequence of characters (aggregated keystrokes).
ActionMethod | Dynamically compiled method (optional runtime compiler plugin required).

## Debounce Logic
* Constant time O(1) arrays store last accepted timestamp per VK or mouse button.
* Suppress if new event occurs within 50 ms (customizable via code constant `_thresholdMs`).
* Tracing subclass batches suppression statistics once per second to minimize hook overhead.

## Building
Prerequisites: .NET SDK 8.x (for net8 target) and classic developer pack for .NET Framework 3.5 if building legacy target.

Standard build:
```
msbuild /p:Configuration=Release
```

Artifacts appear under `../bin/Plugin.WindowAutomation/<TargetFramework>/` relative to solution root.

## Runtime Compiler (Optional)
If the external compiler plugin is present, the Clicker UI can launch an editor window where user code defines methods referenced by ActionMethod steps. Absent plugin, ActionMethod entries are substituted with a safe placeholder.

## Safety / Security
* Polymorphic JSON serialization uses an explicit allow‑list binder to prevent arbitrary type instantiation.
* Global hook implementations ensure unhook in finalizer and Dispose to avoid leaking hooks.

## Extensibility
Key extension points:
* Derive from `GlobalWindowsHookAntiDebounce` and override `OnSuppressedKey` / `OnSuppressedMouse` for custom metrics or telemetry (example: tracing subclass).
* Add new ActionXXX types and register them in `ActionsProject.ActionTypes` allow‑list for serialization.
* Integrate other dynamic code providers by implementing a thin wrapper similar to `CompilerPlugin`.

## License
MIT — see repository for details.

## Summary
This plugin supplies a compact yet extensible automation layer for Windows desktop scenarios, combining robust window metadata APIs, macro recording / playback, dynamic method injection and low‑level input filtering while supporting both legacy and current .NET platforms.