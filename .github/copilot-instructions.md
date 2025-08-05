# Utility Supplements (Continued) - GitHub Copilot Instructions

## Mod Overview and Purpose

**Utility Supplements (Continued)** is an updated version of Pelador's mod, expanding the range of utility items available in RimWorld. These utilities offer practical enhancements in-game without being classified as medical or social supplements. The mod includes new methods and items for handling gases, foams, and toxins. The primary goal is to provide more strategic tools and options for managing environments and threats.

## Key Features and Systems

- **Delivery Methods:** The mod introduces several unique delivery systems for various toxins and foams, including:
  - **Sprayer:** Manual application of foams with controlled dispersal.
  - **Mortar Shells:** Long-range delivery via mortars, enabling strategic placement.
  - **IEDs:** Use of toxins in trap setups, specifically for insects and other threats.
  - **Foam Poppers:** Automated deployment for area denial or area effect, triggered by specific conditions.
  - **Grenades:** Toxin weaponization for direct combat use.

- **Mod Options:**
  - Customize research costs and graphical performance settings related to plant gases and their radii.
  - Adjust toxicity settings to change how active chemicals perform.

- **Compatibility and Expansion Support:**
  - Integrates well with other mods such as Apothecary, Remote Tech, RimPlas, and Seeds Please.
  - Provides multiplayer support in beta phase.
  - Custom Harmony patch for applying mod-specific filth to fertile terrain.

## Coding Patterns and Conventions

- Use of **inheritance and polymorphism** in creating varied damage workers (`DamageWorker_USXXX`).
- Classes typically inherit from game-specific superclasses (`ThingComp`, `Gas`, `Filth`).
- Separation of application logic with private and public methods to enforce encapsulation and code reusability.

## XML Integration

XML files play a crucial role in defining objects and their attributes. Mod developers should familiarize themselves with the XML files to adjust item definitions, research requirements, and other configurations. Utilize localization files for supporting multiple languages.

## Harmony Patching

Harmony patches are utilized to ensure compatibility and expand functionality:
- **CanReserve_Patch.cs:** Overriding game's default reservation checks.
- **TerrainAcceptsFilthPatch.cs:** Ensures proper application of the mod's filth mechanics.
- Patches should follow the standard Harmony pattern, targeting specific methods to avoid conflicts.

## Suggestions for Copilot

1. **Automate Boilerplate Code:** Use Copilot to generate repetitive code structures such as class templates for `DamageWorker` classes.
2. **Suggest Refactorings:** Identify opportunities for refactoring methods with similar logic, especially in damage handling and toxin effect application.
3. **XML Schema Suggestions:** Propose XML schema structures for creating new items or expanding existing definitions.
4. **Localization Assistance:** Aid in maintaining and expanding localization files, including suggestions for translation placeholders.
5. **Performance Optimization:** Advise on efficient coding patterns and identify potential bottlenecks in the toxin and foam handling logic.

---

Ensure you test features thoroughly, as this mod involves complex interactions with the game's systems. Report any aberrations resembling defined issues, particularly those stemming from the absence of source code updates.
