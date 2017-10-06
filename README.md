# Pleasing

Pleasing is a easy-to-use easing and tweening library. It aims to be as simple as possible while also being flexible enough to fit all your needs. Unlike other tweening libraries, pleasing includes a tweening timeline which makes it simple to plan multiple tweens on multiple objects.

## Easing

Easing is at the core of tweening and motion graphics / animation in general. Pleasing includes a single file easing class that handles basic easing curves as well as custom cubic Bezier curves.

### Basics

Use any of the built in easing functions for quickly adding some smoothness to your graphics.

```csharp
var value = 100;
var percentComplete = 0.5; //A float from 0-1
var easedPercent = Easing.Ease(EasingFunction.CubicInOut, value); 
value = value * easedPercent;

//Alternate method not using enum.
var easedPercent = Easing.Cubic.InOut(value);
```

Pleasing contains quick access to all of [Robert Penner's easing functions:](http://robertpenner.com/easing/)

* Linear

*The following come with In / Out / InOut versions.*

* Quadratic
* Cubic
* Quartic
* Quintic
* Sinusoidal
* Exponential
* Circular
* Elastic
* Back
* Bounce

### Advanced

Take control of easing and get that custom motion you've been looking for using Cubic Bezier curves.

```csharp
var value = 100;
var percentComplete = 0.5;
var easedPercent = Easing.Bezier(percentComplete, 0.3f, 0.75f, 0, 1);
value = value * easedPercent;
```



## Tweening

Pleasing incorporates it's custom easing with an easy-to-use tweening library. Tweening let's you transition from value to value in a smooth fashion.

### Basics

The `Tweening` static class is the entry point and container for tweening. `Tweening.Update(gameTime)` must be called once per frame.

**TweenTimeline**: A virtual timeline for tweens, allowing them to be run in sequence at specific moments, as well as looping them as a group.

**Tween**: The tween itself which contains the object to be tweened, when the tweening occurs, and all the properties and values to be tweened.

The simplest usage is to create a new TweenTimeline and Tween simultaneously:

```csharp
//An object with multiple properties.
//e.g. position, rotation, color, scale.
var banner = new Banner();

//Create a new tween timeline and tween for the banner object with 2 second duration.
var tween = Tweening.Tween(banner, Easing.Cubic.InOut, 2000);

//Now assign properties to be tweened.
tween.Add(nameof(banner.position), new Vector2(500, 500));
tween.Add(nameof(banner.color), Color.Maroon);

//All done! The tween starts automatically the next frame and will run once.
```



Adding properties to a tween can be done two ways.

*Using reflection and property names:*

```csharp
tween.Add(nameof(banner.position), new Vector2(500, 500));
```

*Or using a setter:*

```csharp
tween.Add(banner.position, new Vector2(500, 500) (x) => banner.position = x);
```

The setter has the advantage of being able to go deeper into nested objects:

```csharp
tween.Add(banner.position.x, 500f, (x) => banner.position.x = x);
```





