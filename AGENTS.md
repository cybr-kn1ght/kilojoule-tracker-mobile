# AGENTS.md — Project Conventions for AI Assistants

## Project Overview
.NET MAUI app (C#) targeting Android. Tracks kilojoule consumption via a terminal-style UI. 3 projects: MAUI app, Core library, Tests.

## Project Structure
```
src/
  KilojouleTracker/          — MAUI app (UI, platform code)
  KilojouleTracker.Core/     — Domain logic, data access, interfaces
tests/
  KilojouleTracker.Tests/    — xUnit tests
```

## Code Conventions

### Namespaces
- File-scoped namespaces everywhere.
- Root: `KilojouleTracker`. Features: `KilojouleTracker.Features.<FeatureName>`.
- Test namespace: `KilojouleTracker.Tests` (flat).

### Style
- 4-space indentation, Allman braces.
- `private readonly` fields prefixed with `_`.
- No `this.` qualifier.
- No explicit `private` on fields (implicit).
- Target-typed `new()` and collection expressions `[]`.
- Record types for data DTOs, classes for services.
- One type per file.

### Language Features
- Nullable reference types enabled.
- Implicit usings enabled.
- Switch expressions over switch statements.
- `is null` / `is not null` pattern matching.

### MVVM
- CommunityToolkit.Mvvm: `[ObservableProperty]`, `[RelayCommand]`.
- ViewModels inherit `ObservableObject`, are `partial`.
- Constructor DI, `BindingContext` set in code-behind.

### Tests (xUnit + NSubstitute)
- Class naming: `{ClassUnderTest}Tests`.
- SUT field: `_sut`.
- Method naming: `{Method}_{Scenario}_Returns{Expected}`.
- AAA pattern, `Assert.*` static methods, no fluent assertions.
- NSubstitute: `Substitute.For<T>()`, `Received()` / `DidNotReceive()`.

### Async
- Async methods return `Task`, suffixed with `Async`.

### DI
- Built-in Microsoft DI. `AddSingleton` for stateless services, `AddTransient` for ViewModels/Pages.

## Commands
- Build: `dotnet build`
- Test: `dotnet test`
- Lint: n/a
