# Unity Tetris Clone

This is a simple practice project to try and create a tetris clone in Unity

Can be played in a WebGL build here:
https://evasilyev.com/games/tetris

## Screenshots
![In Game Screenshot](https://media.githubusercontent.com/media/Zeejfps/Unity-Tetromino-Game/main/Screenshots/Screenshot%202023-12-14%20101002.png)

## Resources
This turns out to be a great resource for internal workings of Tetris.
https://www.colinfahey.com/tetris/tetris.html

## Architecture
The entry point to the application can be found in [TetrisDiContainer.cs](https://github.com/Zeejfps/Unity-Tetromino-Game/blob/main/Assets/_Tetris/Scripts/Di/TetrisDiContainer.cs).
This file is where all the 
dependencies are registered.

The actual bulk of the work is done inside the [GameController.cs](https://github.com/Zeejfps/Unity-Tetromino-Game/blob/main/Assets/_Tetris/Scripts/GameController.cs)
### Controllers
In this project a Controller is a Monobehaviour. 
The main purpose of a controller is to receive events such as: input, game, application, or any other kind of events, 
and response to those events.

## Credits
* Dev - Evgeny "Zee" Vasilyev
* Music - *Korobeiniki*, Downloaded from [khinsider.com](https://downloads.khinsider.com/game-soundtracks/album/tetris-gb)
* Font - *Crang*, Downloaded from [dafonts.com](https://www.dafont.com/crang.font)