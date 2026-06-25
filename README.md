# 🎣 Fishing a Fish (VR Fishing Simulator)

A virtual reality fishing simulator built in **Unity** using the **Unity XR Interaction Toolkit**. Players stand on a dock overlooking the ocean, cast their fishing rod into the water, wait for a fish to bite, and complete an interactive fishing minigame to successfully catch fish.

## Overview

**Fishing a Fish** is a VR fishing game inspired by the fishing mechanics found in *Stardew Valley*. The experience combines immersive VR interactions with a timing-based fishing system that challenges players to reel in fish after receiving a bite.

Once a fish bites the hook, players must quickly reel in the line to begin the fishing minigame. During the minigame, the objective is to keep a movable green box aligned with the fish until the progress meter is completely filled, successfully landing the catch.

## Features

* 🎣 Immersive VR fishing experience
* 🪝 Interactive fishing rod with casting and reeling mechanics
* 🐟 Timing-based fish bite detection
* 🎮 Stardew Valley-inspired fishing minigame
* 🌅 Dynamic day/night environment
* 🌊 Ocean environment with optimized VR interactions
* 🥽 Built using the Unity XR Interaction Toolkit

## Technical Challenges

### Input Action Conflicts

**Problem:**
Using the right thumbstick to reel in the fishing line also triggered the XR teleport locomotion system, causing the player to unintentionally teleport into the ocean.

**Solution:**
The teleport interactor was detached from the right-hand `ActionBasedControllerManager`, reserving the right thumbstick exclusively for the custom reeling mechanic implemented in `HookReel.cs`.

---

### UI Clipping Through the Ocean

**Problem:**
When the fishing minigame appeared, it spawned directly in front of the player's camera. If the player was looking downward at the fishing bobber, the UI could intersect with the ocean surface.

**Solution:**
The camera's forward vector was flattened on the Y-axis before calculating the UI position, ensuring the minigame always appeared horizontally at eye level without clipping into the environment.

---

### Unity Collaboration & Merge Conflicts

**Problem:**
Collaborating on the same Unity scene resulted in difficult Git merge conflicts due to Unity scene serialization.

**Solution:**
Each team member developed features in separate sandbox scenes. Once completed, the individual work was merged into the final scene, greatly reducing merge conflicts and improving development efficiency.

## Future Improvements

Future enhancements include:

* Additional immersive environments
* Fish rarity system with scaling difficulty
* Score and progression system
* Spatial audio for water splashes and environmental effects
* More fish species and gameplay variety

## Technologies Used

* Unity
* C#
* Unity XR Interaction Toolkit
* Git & GitHub
* Unity Device Simulator

## Team Contributions

### Andrew Cao

* Designed and implemented the Stardew Valley-inspired fishing minigame
* Developed the dynamic UI system
* Implemented the two-second reaction window
* Engineered camera positioning logic to prevent UI clipping

### Amanda Christine Riordan

* Implemented the dynamic day/night cycle
* Designed the environmental lighting
* Optimized scene lighting for visual quality and performance
* Added environmental polish and atmosphere

### Arshiya Salehi

* Designed the initial game environment
* Developed the core gameplay mechanics
* Implemented the fishing rod and hook physics
* Engineered the casting, lowering, and reeling mechanics that trigger the fishing minigame

## What We Learned

This project provided hands-on experience with VR game development using Unity and introduced challenges unique to immersive applications, including controller input management, comfortable UI placement, VR interaction design, and collaborative Unity workflows.

Beyond implementing gameplay mechanics, the project reinforced best practices for developing VR experiences while maintaining usability, immersion, and performance.
