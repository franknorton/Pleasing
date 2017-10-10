![Pleasing](/Pleasing.gif?raw=true)

**Pleasing** is an easy-to-use Monogame *easing* and *tweening* library. It aims to be as simple as possible while also being flexible enough for any scenario. Unlike other tweening libraries, pleasing includes a tweening timeline which makes it simple to plan multiple tweens on multiple objects.

**The Pleasing logo was created using Pleasing.*

## Installation

**Nuget** 
- Coming soon.

**Zip**

1. Download the repository as a .zip file.
2. Extract it somewhere.
3. Copy Tweening.cs and Easing.cs into your project.
4. Add the namespace to any files using it and you're ready to go!

## Usage

1. Add the namespace: `using Pleasing;`
2. Get a new timeline: `var timeline = Tweening.NewTimeline(0);`
3. Add a property to the timeline: `var positionProperty = timeline.AddProperty(Player, "position");`
4. Add Keyframes: `positionProperty.AddFrame(1000, new Vector2(500, 250), Easing.Cubic.InOut);`
5. Call Update every frame: `Tweening.Update(gameTime);`
6. Watch and enjoy!

Consult the Wiki for more in-depth information and tutorials.

## Credits

Authors:

* Frank Norton

## License

Pleasing is under the MIT license (2017). A copy of the license is found in the root of the repository.
