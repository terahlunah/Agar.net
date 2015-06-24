# Agar.net

A client for [agar.io](http://agar.io) written in C#

![Agar.net](http://puu.sh/iAoPl/31bfe3ecf5.png "Agar.net")


It's a work in progress, so don't expect everything to work !


## Features

- Connection to a server
- Selection of the game mode
- Spectating mode (press 's')

## ToDo

- Clean the code !
- Finish the reverse engineering of the net protocol
- Add texts (names, score, leaderbord, ...)
- Add input to allow playing (instead of just watching atm)
- Interpolation (to smooth movements, camera included)
- **Add scripting**


## Libs

Agar.net is using [WebSocketSharp](https://github.com/sta/websocket-sharp) and the [SFML](http://www.sfml-dev.org/) for the graphics.
