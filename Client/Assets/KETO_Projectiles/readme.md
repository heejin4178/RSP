# Elemental Projectiles

This asset pack is a fusion of visual effects and seamlessly integrated scripts, designed to infuse your game with a spellbinding array of magical projectiles.

## Key Features:

Five distinct elemental attributes

Total 15 prefabs, each combining cast effect, projectile, and impact elements.

## Dependencies

- Unity 2021.3.9 or higher

- Universal Render Pipeline (URP)


**Note:** This project has a dependency on URP and will only work with URP installed and set as the current render pipeline.

#Demo Scene:

Locate the "Scenes/SampleScene.unity" file in your project hierarchy.
Double-click on "SampleScene.unity" to open the Demo scene in Unity's Scene view.

Click the "Play" button at the top of the Unity editor to start the game in play mode.

Testing Skills:

Mouse Click: When you click the mouse button, the script detects the click and triggers the currently selected skill. The position where you clicked the mouse determines the skill execution position. In other words, if you click on a point in the game world, the skill will be cast at that location.

Skill Switching:
The script allows you to switch between different skills using the Q and E keys.

Q Key: Pressing the Q key will switch to the previous skill in the list of available skills.
E Key: Pressing the E key will switch to the next skill in the list of available skills.

So, in summary, when you click the mouse, a projectile is fired at the clicked location, and you can cycle through your skills using the Q and E keys to choose the skill you want to use.


# SkillManager Script Discription

The `SkillManager` script is a component used in Unity engine to manage and execute various skills within a game. This script detects mouse and keyboard inputs to trigger specific skills, and it provides functionalities for skill switching and rotation.

## Introduction

The `SkillManager` script serves the following purposes:

1. **Skill Management**: It manages and allows the switching of various skills.
2. **Mouse Input Handling**: It triggers skills using mouse clicks, determining the skill execution position based on the clicked location.
3. **Skill Switching**: It allows switching to the next and previous skills using the Q and E keys.
4. **Skill Rotation**: It rotates the character to the clicked location when a skill is used.

## Variables

- `m_totalNumberOfSkills` (SerializeField, private): Stores the total number of available skills in the game.
- `m_currentSkillNumber` (private): Stores the index of the currently selected skill.
- `LAYER_DAMAGEABLE` (private, const string): Stores the name of the layer for damageable objects.
- `m_ray` (private): Stores the ray that shoots from the mouse pointer towards the screen.
- `m_maxDistance` (private): Stores the maximum distance for raycasting.
- `m_hitPosition` (private): Stores the position where the raycast collided.
- `m_rotation` (private): Quaternion that stores the rotation value of the character.
- `m_currentSkill` (private): Stores the currently selected skill object.
- `m_skill_01` ~ `m_skill_05` (private): Store objects of each skill type.
- `m_textGui` (private): Stores the GUI element that manages displaying text on the screen.

## Methods

### `GetCurrentSkillByNumber(int _num)`

Returns the skill object corresponding to the currently selected skill index.

### `Awake()`

Called when the game object becomes active, performing initialization. Initializes each skill object and sets the current skill.

### `Start()`

Called once after the game object becomes active, performs initial skill setup, and GUI initialization.

### `Update()`

Called every frame, detects mouse and key inputs to perform skill triggering and switching actions.

### `RotateToMouse(Vector3 _destination)`

Method to rotate the character to the given position `_destination`. Applies rotation values on the x and z axes to perform character rotation.

## Summary

The `SkillManager` script plays a vital role in Unity engine, managing the various aspects of skills, including management, triggering, switching, and rotation. It handles mouse and key inputs, manages each skill as an object, and implements the game's skill system.



# Projectiles Script Discription

The `Projectiles` script is responsible for managing projectile behavior in Unity. This script controls the movement of the projectile, collision handling, and impact effects.

## Overview

- **Component Dependencies**: This script requires a `Rigidbody` component to be attached to the GameObject.
- **HitPrefab**: A reference to the impact prefab that is instantiated upon collision.
- **Speed**: The initial speed of the projectile.

## Methods

### `Start()`

- Initializes the script by getting the `Rigidbody` component attached to the GameObject.

### `FixedUpdate()`

- If `speed` is not zero and the `Rigidbody` component exists, the projectile's position is updated based on its forward direction and speed.

### `OnCollisionEnter(Collision _collision)`

- Handles collision events when the projectile collides with other objects.
- If the collided object's tag is "Projectiles", no further action is taken.
- The main camera's `ShakeCamera` component is instructed to shake.
- The projectile's speed is set to zero, and its `Rigidbody` component is set to kinematic.
- The collision contact point and normal are used to calculate impact position and rotation.
- If a `HitPrefab` is assigned, it's instantiated at the impact position with the calculated rotation.
- If the `HitPrefab` has a `ParticleSystem`, the prefab is destroyed after the particle system's duration.
- The projectile GameObject is destroyed.

## Summary

The `Projectiles` script controls the behavior of projectiles in the game. It handles movement, collision detection, impact effects, and cleanup. The script interacts with a `Rigidbody` and can trigger impact effects using a provided prefab.




# Skill Scripts Documentation

In this section, we'll provide explanations for the five skill scripts: `Skill_01` through `Skill_05`. These scripts implement various skills in your game and are all structured in a similar way.

## Overview

The skill scripts (`Skill_01` through `Skill_05`) are responsible for defining and executing different skills in the game. These scripts handle skill cooldowns, effects casting, and projectile generation.

## Script Structure

Each skill script follows a similar structure:

- **`m_name`**: The name of the skill.
- **`m_coolTime`**: The cooldown time of the skill.
- **`EffectData` struct**: This struct defines effect-related data, including the transform, prefab, and wait time for effects.
- **`CastEffect` and `ProjectileEffect`**: Instances of the `EffectData` struct, specifying casting and projectile effects.
- **`m_coolTimeCounter`**: Tracks the cooldown timer.
- **`m_rotation`**: Stores the calculated rotation for effects.
- **`Initialize()`**: Initializes the cooldown timer.
- **`StartSkill(Vector3 _position)`**: Starts the skill if the cooldown is met.
- **`Cancel()`**: Cancels the skill (empty implementation in this case).
- **`GetName()`**: Returns the name of the skill.
- **`Update()`**: Updates the cooldown timer.
- **`SkillCoroutine(Vector3 _position)`**: Coroutine that handles skill execution, including casting and generating projectiles.

## Usage

To use these skill scripts:

1. Attach the appropriate script to a GameObject.
2. Set the `m_name`, `m_coolTime`, `CastEffect`, and `ProjectileEffect` fields in the Inspector.

These scripts are designed to provide a foundation for creating and managing various skills in your game. Customize the `EffectData` fields and the logic within the `SkillCoroutine` method to achieve different skill effects.

Remember that these explanations apply to all the skill scripts from `Skill_01` to `Skill_05`. Each script handles a different skill, but they share a common structure and usage pattern.

Feel free to refer to the specific script names (e.g., `Skill_01`, `Skill_02`, etc.) for more details on each skill's implementation.