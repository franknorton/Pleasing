using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pleaseing
{
    public enum TweenState
    {
        Running,
        Paused,
        Stopped
    }

    /// <summary>
    /// The entry point for tweens. Call Tweening.Update() once per frame.
    /// </summary>
    public static class Tweening
    {
        private static List<TweenTimeline> tweens;

        static Tweening()
        {
            tweens = new List<TweenTimeline>();
        }

        /// <summary>
        /// Creates a new tween and timeline.
        /// </summary>
        /// <param name="obj">The object to tween.</param>
        /// <param name="easingFunction">The easing function to use. (e.g. Easing.Linear)</param>
        /// <param name="endTime">The time, in milliseconds when the tween will end.</param>
        /// <param name="startTime">The time in milliseconds when the tween will begin.</param>
        /// <returns>A TweenTimeline with a tween attached.</returns>
        public static Tween Tween(object obj, Func<float, float> easingFunction, float endTime, float startTime = 0)
        {
            var tween = new Tween(obj, startTime, endTime, easingFunction);
            var timeline = new TweenTimeline(endTime);
            timeline.AddTween(tween);
            return tween;
        }

        /// <summary>
        /// Creates a new timeline.
        /// </summary>
        /// <param name="duration">The length of the timeline in milliseconds.
        /// Leave blank to have tweens set it.</param>
        /// <returns></returns>
        public static TweenTimeline NewTimeline(float duration)
        {
            var timeline = new TweenTimeline(duration);
            tweens.Add(timeline);
            return timeline;
        }

        public static void Update(GameTime gameTime)
        {
            foreach (var tween in tweens)
            {
                tween.Update(gameTime);
            }
        }
    }

    /// <summary>
    /// The TweenTimeline holds tweens and runs them in sequence based on the elapsed time.
    /// </summary>
    public class TweenTimeline
    {
        public List<Tween> tweens;
        public float elapsedMilliseconds;
        public bool Loop;
        public float duration;
        public TweenState State;

        public TweenTimeline(float duration)
        {
            this.duration = duration;
            tweens = new List<Tween>();
            State = TweenState.Running;
        }

        public void Start()
        {
            State = TweenState.Running;
        }
        public void Stop()
        {
            elapsedMilliseconds = 0;
            State = TweenState.Stopped;
            ResetProperties();
        }
        public void Pause()
        {
            State = TweenState.Paused;
        }
        public void Restart()
        {
            elapsedMilliseconds = 0;
            State = TweenState.Running;
            ResetProperties();
        }

        /// <summary>
        /// Scans through the timeline backwards, resetting properties to their orginal state.
        /// </summary>
        protected void ResetProperties()
        {
            for(int i = tweens.Count - 1; i >= 0; i--)
            {
                tweens[i].ResetProperties();
            }
        }
        
        public Tween AddTween(object obj, Func<float, float> easingFunction, float startTime, float endTime)
        {
            var tween = new Tween(obj, startTime, endTime, easingFunction);
            tweens.Add(tween);

            if (endTime > duration)
                duration = endTime;

            tweens = tweens.OrderBy(x => x.startTime).ToList();
            return tween;
        }
        public Tween AddTween(Tween tween)
        {
            tweens.Add(tween);
            tweens = tweens.OrderBy(x => x.startTime).ToList();
            return tween;
        }

        public void Update(GameTime gameTime)
        {
            if (State == TweenState.Running)
            {
                elapsedMilliseconds += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (elapsedMilliseconds >= duration)
                {
                    if (Loop)
                    {
                        elapsedMilliseconds = elapsedMilliseconds - duration;
                        ResetProperties();
                    }
                    else
                    {
                        Stop();
                    }
                }

                if (State == TweenState.Running)
                {
                    foreach (var tween in tweens)
                    {
                        if(elapsedMilliseconds >= tween.startTime && elapsedMilliseconds <= tween.endTime)
                            tween.Update(elapsedMilliseconds);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tween contains an object and properties to manipulate over time.
    /// </summary>
    public class Tween
    {
        private object targetObject;
        private PropertyInfo[] properties;
        private FieldInfo[] fields;
        private List<TweenableProperty> tweeningProperties;
        public float startTime;
        public float endTime;
        private float elapsedMilliseconds;
        private Func<float, float> easingFunction;

        public float Progress
        {
            get
            {
                return easingFunction.Invoke(MathHelper.Clamp((elapsedMilliseconds - startTime) / (endTime - startTime), 0, 1));
            }
        }

        public Tween(object obj, float startTime, float endTime, Func<float, float> easingFunction)
        {
            targetObject = obj;
            this.easingFunction = easingFunction;
            properties = obj.GetType().GetProperties();
            fields = obj.GetType().GetFields();
            tweeningProperties = new List<TweenableProperty>();
            this.startTime = startTime;
            this.endTime = endTime;
        }

        /// <summary>
        /// Adds a new property based on the property's name.
        /// This way is easier to read but uses reflection.
        /// </summary>
        /// <typeparam name="T">The type of value to be tweened.</typeparam>
        /// <param name="propertyName">The name of the property to be tweened.</param>
        /// <param name="value">The final value to tween the property to.</param>
        /// <param name="lerpFunction">A function that will calculate the lerp for this value type.</param>
        /// <returns></returns>
        public Tween Add<T>(string propertyName, T value, LerpFunction<T> lerpFunction)
        {
            var property = properties.FirstOrDefault(x => x.Name == propertyName);
            if (property != null)
            {
                if (property.PropertyType == typeof(T))
                {
                    var tweenableProperty = new TweenProperty<T>(targetObject, property, value, lerpFunction);
                    tweeningProperties.Add(tweenableProperty);
                }
            }
            else
            {
                var field = fields.FirstOrDefault(x => x.Name == propertyName);
                if (field != null)
                {
                    if (field.FieldType == typeof(T))
                    {
                        var tweenableProperty = new TweenField<T>(targetObject, field, value, lerpFunction);
                        tweeningProperties.Add(tweenableProperty);
                    }
                }
            }

            return this;
        }
        public Tween Add(string propertyName, float value)
        {
            Add<float>(propertyName, value, LerpFunctions.Float);
            return this;
        }
        public Tween Add(string propertyName, Vector2 value)
        {
            Add(propertyName, value, LerpFunctions.Vector2);
            return this;
        }
        public Tween Add(string propertyName, Vector3 value)
        {
            Add(propertyName, value, LerpFunctions.Vector3);
            return this;
        }
        public Tween Add(string propertyName, Vector4 value)
        {
            Add(propertyName, value, LerpFunctions.Vector4);
            return this;
        }
        public Tween Add(string propertyName, Color value)
        {
            Add(propertyName, value, LerpFunctions.Color);
            return this;
        }
        public Tween Add(string propertyName, Quaternion value)
        {
            Add(propertyName, value, LerpFunctions.Quaternion);
            return this;
        }
        public Tween Add(string propertyName, Rectangle value)
        {
            Add(propertyName, value, LerpFunctions.Rectangle);
            return this;
        }

        /// <summary>
        /// Adds a new property that uses a setter to assign the tweened value.
        /// This has some useful cases such as when accessing a deeply nested property.
        /// </summary>
        /// <typeparam name="T">The type of value to tween.</typeparam>
        /// <param name="startValue">The initial value when the tween begins.</param>
        /// <param name="endValue">The value when the tween ends.</param>
        /// <param name="setter">A function that will be called with the tweened value.</param>
        /// <param name="lerpFunction">A function that will calculate the lerp for this type of value.</param>
        /// <returns></returns>
        public Tween Add<T>(T startValue, T endValue, Action<T> setter, LerpFunction<T> lerpFunction)
        {
            var tweenableProperty = new TweenSetter<T>(startValue, endValue, setter, lerpFunction);
            tweeningProperties.Add(tweenableProperty);
            return this;
        }
        public Tween Add(float startValue, float endValue, Action<float> setter)
        {
            return Add(startValue, endValue, setter, LerpFunctions.Float);
        }
        public Tween Add(Vector2 startValue, Vector2 endValue, Action<Vector2> setter)
        {
            return Add(startValue, endValue, setter, LerpFunctions.Vector2);
        }
        public Tween Add(Vector3 startValue, Vector3 endValue, Action<Vector3> setter)
        {
            return Add(startValue, endValue, setter, LerpFunctions.Vector3);
        }
        public Tween Add(Vector4 startValue, Vector4 endValue, Action<Vector4> setter)
        {
            return Add(startValue, endValue, setter, LerpFunctions.Vector4);
        }
        public Tween Add(Color startValue, Color endValue, Action<Color> setter)
        {
            return Add(startValue, endValue, setter, LerpFunctions.Color);
        }
        public Tween Add(Quaternion startValue, Quaternion endValue, Action<Quaternion> setter)
        {
            return Add(startValue, endValue, setter, LerpFunctions.Quaternion);
        }
        public Tween Add(Rectangle startValue, Rectangle endValue, Action<Rectangle> setter)
        {
            return Add(startValue, endValue, setter, LerpFunctions.Rectangle);
        }

        public void ResetProperties()
        {
            foreach (var property in tweeningProperties)
                property.Reset();
        }

        public void Update(float timelineElapsedMilliseconds)
        {
            elapsedMilliseconds = timelineElapsedMilliseconds;

            foreach (var property in tweeningProperties)
                property.Tween(Progress);

        }
    }

    internal interface TweenableProperty
    {
        void Tween(float progress);
        void Reset();
    }
    internal class TweenProperty<T> : TweenableProperty
    {
        public object target;
        public PropertyInfo property;
        public T startValue;
        public T value;
        LerpFunction<T> lerpFunction;

        public TweenProperty(object target, PropertyInfo property, T value, LerpFunction<T> lerpFunction)
        {
            this.target = target;
            this.property = property;
            this.value = value;
            this.lerpFunction = lerpFunction;
            startValue = (T)property.GetValue(target);
        }

        public void Reset()
        {
            property.SetValue(target, startValue);
        }

        public void Tween(float progress)
        {
            var lerpValue = lerpFunction(startValue, value, progress);
            property.SetValue(target, lerpValue);
        }
    }
    internal class TweenField<T> : TweenableProperty
    {
        public object target;
        public FieldInfo field;
        public T startValue;
        public T value;
        LerpFunction<T> lerpFunction;

        public TweenField(object target, FieldInfo field, T value, LerpFunction<T> lerpFunction)
        {
            this.target = target;
            this.field = field;
            this.value = value;
            this.lerpFunction = lerpFunction;
            startValue = (T)field.GetValue(target);
        }

        public void Reset()
        {
            field.SetValue(target, startValue);
        }

        public void Tween(float progress)
        {
            var lerpValue = lerpFunction(startValue, value, progress);
            field.SetValue(target, lerpValue);
        }
    }
    internal class TweenSetter<T> : TweenableProperty
    {
        public T startValue;
        public T endValue;
        public LerpFunction<T> lerpFunction;
        public Action<T> setter;

        public TweenSetter(T startValue, T endValue, Action<T> setter, LerpFunction<T> lerpFunction)
        {
            this.startValue = startValue;
            this.endValue = endValue;
            this.setter = setter;
            this.lerpFunction = lerpFunction;
        }

        public void Reset()
        {
            setter.Invoke(startValue);
        }

        public void Tween(float progress)
        {
            var currentValue = lerpFunction(startValue, endValue, progress);
            setter.Invoke(currentValue);
        }
    }
    
    public delegate T LerpFunction<T>(T start, T end, float progress);
    public static class LerpFunctions
    {
        public static LerpFunction<float> Float = (s, e, p) => s + (e - s) * p;
        public static LerpFunction<Vector2> Vector2 = (s, e, p) => { return Microsoft.Xna.Framework.Vector2.Lerp(s, e, p); };
        public static LerpFunction<Vector3> Vector3 = (s, e, p) => { return Microsoft.Xna.Framework.Vector3.Lerp(s, e, p); };
        public static LerpFunction<Vector4> Vector4 = (s, e, p) => { return Microsoft.Xna.Framework.Vector4.Lerp(s, e, p); };
        public static LerpFunction<Color> Color = (s, e, p) => { return Microsoft.Xna.Framework.Color.Lerp(s, e, p); };
        public static LerpFunction<Quaternion> Quaternion = (s, e, p) => { return Microsoft.Xna.Framework.Quaternion.Lerp(s, e, p); };
        public static LerpFunction<Rectangle> Rectangle = (s, e, p) =>
            {
                var pX = s.X + (e.X - s.X) * p;
                var pY = s.Y + (e.Y - s.Y) * p;
                var width = s.Width + (e.Width - s.Width) * p;
                var height = s.Height + (e.Height - s.Height) * p;
                return new Microsoft.Xna.Framework.Rectangle((int)pX, (int)pY, (int)width, (int)height);
            };
    }


}
