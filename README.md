# Heirloom
This game is an attempt at participating in the [Godot Wild Jam 24](https://itch.io/jam/godot-wild-jam-24-our-2-year) game jam, a jam where you have make a game with the theme "Family" with Godot in 9 days.

The game uses the [Godot](https://godotengine.org/) game engine.

## Running the game

The repo uses [git lfs](https://git-lfs.github.com/) to store images and sounds, so start by installing it on your computer. Then you can clone the repository with
```
git lfs clone git@github.com:TeamCourgette/my-little-roguelike.git
```

You need to install the [Godot 3.2.2, Mono Version](https://godotengine.org/download/). It requires MSBuild, so make sure to follow all the steps on the page.
You should be able to call the following command with no error:
```
msbuild -version
```

Then, open the project through the Godot editor, you should be able to run it by pressing 'F5', or clicking on the triangle on the top right.
