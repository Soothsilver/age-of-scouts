"Auxiliary" is an XNA library of classes and functions. It consists of two parts: The main "Auxiliary" class library that works only with XNA 4, and the "Cother" class library which consists of general C# classes not requiring XNA.

To fully utilize the Auxiliary library, notably its GamePhase and Primitives classes, you must first take these steps:

1. Add the "Auxiliary" and "AuxiliaryContent" projects to your solution.
2. At program startup, call the Root.Init() static method with appropriate parameters.
3. In your main Draw() method, add these lines:

spriteBatch.Begin();
Root.DrawPhase(gameTime);
Root.DrawOverlay(gameTime);
spriteBatch.End();

where "spriteBatch" is the sprite batch passed to the Root.Init() method.

4. In your main Update() method, add this line:

Root.Update(gameTime);

5. You should now be able to use all classes and methods of the library.
==========================
HOW THE GAMEPHASE SYSTEM WORKS

If you want to use the library's GamePhase system, this is what you need to know:
Each screen in your game (for example, MainMenu, LevelSelection, InGame, LoadGameDialog or ConfirmationDialog) may be represented as a derived class of GamePhase. You may then use Root.PushPhase to add an instance of the screen to the PhaseStack and Root.PopFromPhase() to remove.
In Root.DrawPhase(), all phases in the stack will be drawn using their Draw() method.
In Root.Update() only the topmost game phase on the stack will be update using its Update() method.