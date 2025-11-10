# Soul Collector - Game Programming Basics Project

## About This Project

This "Coin Collector" game was a project created for the "Game Programming Basics" module (4FSCOPE001) at SAE Institute, as part of a BSc in Games Programming. The goal of the project was to create a simple game demonstrating a foundational understanding of Object-Oriented Programming (OOP), classes, loops, and vector maths.

The second assignment in this module required us to implement a sorting algorithm, which I decided to incorporate into my existing "Coin Collector" project. I gave the tiles health and added a cannon that fires on the weakest tiles, using a sorting algorithm to decide which tiles to fire on.

No external assets were used and built-in functions were intentionally replicated for educational purposes.

The game was developed in Unity 6 and coded in C#. This readme also serves as a basic game design document.

## Gameplay

The player controls a small ghost character within a series of gridded levels The objective is to collect all the floating "souls" in each level to progress to the next.

Each grid tile has a health value that drops by one when the player hovers over them, though the health value is clamped at 1, so the player can never destroy a tile just by hovering over it. Every 3 moves the player makes, a cannon will select the 3 weakest tiles and fire on them, reducing their health. If the tile's health is brought to zero by the cannon, the tile will be destroyed and the player can't walk there anymore.

If the player is on a tile that is fired on by the cannon, the player dies and the game is over.

* **Controls:** Use WASD, Arrow Keys, or joypad analogue stick to move the character one square at a time.

## Assignment Constraints

This project was created under a specific set of constraints designed to focus on core programming skills:

* **No External Assets:** All visuals are created using the engine's built-in primitive shapes (cubes, spheres, capsules).
* **Simple Materials:** Only basic, solid in-engine material colours.
* **Manual Maths Functions:** Core mathematical functions, such as the LERP for player movement, were implemented manually to demonstrate understanding.
* **Sorting Algorithm:** We are required to implement a sorting algorithm (that we must code by hand) in our project. In this case, I implemented bubble sort and insertion sort methods.
* **No AI Assistance:** All code and logic were written entirely by the developer without the use of generative AI tools.