# Agar.net (Not up to date!)

A client for [agar.io](http://agar.io) written in C#

![Agar.net](http://puu.sh/iAoPl/31bfe3ecf5.png "Agar.net")


It's a work in progress, so don't expect everything to work !


## Features

- Connection to a server
- Selection of the game mode
- Spectating (press 's')
- Playing (press 'p' to spawn, left click to eject mass, right click to split)

## ToDo

- Clean the code !
- Finish the reverse engineering of the net protocol
- Ajust camera zoom / window size
- Add texts (names, score, leaderboard, ...)
- Interpolating  movements
- Smooth camera
- **Add scripting**


## Libs

Agar.net is using [WebSocketSharp](https://github.com/sta/websocket-sharp) and the [SFML](http://www.sfml-dev.org/) for the graphics.
