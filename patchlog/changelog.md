# Changelog

## 2026-09-22

- Added `TrayIconService` (`System.Windows.Forms.NotifyIcon`) with a right-click "종료" menu item wired to `System.Windows.Application.Current.Shutdown()`. Enabled `UseWindowsForms` in the csproj and fully qualified `Application` in `App.xaml.cs`/`MainWindow.xaml.cs` to resolve the WPF/WinForms namespace clash.
- Deferred the "설정" tray menu item until a real settings screen exists, per plan agreed with the user.
- `MainWindow.Closed` now stops the wander timer and disposes the tray icon.
- Verified: `dotnet build` clean, app runs without crash, user confirmed the tray icon appears and right-click 종료 exits the app.
- Updated `patchlog/current/module-overview.md` for tray icon behavior and the WPF/WinForms dependency note.

## 2026-09-18 (4)

- Added `PetWanderMovement`, a pure logic class computing left/right auto-wander position with edge bounce.
- Wired a `DispatcherTimer` (16ms) into `MainWindow` to drive auto-wander, pausing while the user drags the window.
- Verified: `dotnet build` clean, app runs without crash, user confirmed auto-wander/pause-on-drag behavior by running it directly.
- Updated `patchlog/current/module-overview.md` to describe wander movement.

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
