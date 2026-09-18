# Changelog

## 2026-09-18 (3)

- Implemented v1's first feature: transparent, topmost, borderless `MainWindow` (200x200) with mouse-drag move via `DragMove()`. Placeholder `Ellipse` used to visually verify transparency/topmost behavior.
- Verified: `dotnet build` clean, app runs without crash, user confirmed drag/transparent/topmost behavior by running it directly.
- Updated `patchlog/current/module-overview.md` to describe the new window behavior.

## 2026-09-18 (2)

- Decided v1 scope: sprite-sheet visuals, transparent/topmost draggable window, walk animation, tray icon + context menu, MSI/Installer distribution. See `patchlog/decisions/2026-09-18-v1-scope.md`.
- Updated `patchlog/current/module-overview.md` to reflect resolved v1 scope decisions.

## 2026-09-18

- Added repository-level `AGENTS.md` for patchlog and code-change rules, adapted from `module-patchlog-structure.md` for a single-module repository (dropped multi-module tracking and parent-project references).
- Added patchlog structure with `current/`, `decisions/`, and `changelog.md`.
- Added initial `patchlog/current/module-overview.md` based on current WPF scaffold code.
