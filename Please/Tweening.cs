using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Please
{
    public static class Tweening
    {
        private static List<Tween> tweens;

        static Tweening()
        {
            tweens = new List<Tween>();
        }

        public static Tween Tween(int duration, EasingFunction easingFunction = EasingFunction.Linear)
        {
            return new Tween(duration, easingFunction);
        }

        public static void Update(GameTime gameTime)
        {
            foreach (var tween in tweens)
            {
                tween.Update(gameTime);
            }
        }
    }

    public enum TweenState
    {
        Running,
        Paused,
        Stopped
    }

    public class Tween
    {
        private List<ITween> tweenedProperties;
        private float elapsedTime;
        private int duration;

        public EasingFunction EasingFunction { get; set; }

        public float Progress
        {
            get
            {
                return Easing.Ease(EasingFunction, MathHelper.Clamp(elapsedTime / duration, 0, 1));
            }
        }

        public Tween(int duration, EasingFunction easingFunction = EasingFunction.Linear)
        {
            this.duration = duration;
            EasingFunction = easingFunction;
        }

        public Tween Add(ITween property)
        {
            tweenedProperties.Add(property);
            return this;
        }
        public Tween Add(float current, float end, Action<float> setter) {
            Add(new FloatTweenProperty(current, end, setter));
            return this;
        }
        public Tween Add(Vector2 current, Vector2 end, Action<Vector2> setter)
        {
            Add(new Vector2TweenProperty(current, end, setter));
            return this;
        }
        public Tween Add(Vector3 current, Vector3 end, Action<Vector3> setter)
        {
            Add(new Vector3TweenProperty(current, end, setter));
            return this;
        }
        public Tween Add(Vector4 current, Vector4 end, Action<Vector4> setter)
        {
            Add(new Vector4TweenProperty(current, end, setter));
            return this;
        }
        public Tween Add(Color current, Color end, Action<Color> setter)
        {
            Add(new ColorTweenProperty(current, end, setter));
            return this;
        }
        public Tween Add(Quaternion current, Quaternion end, Action<Quaternion> setter)
        {
            Add(new QuaternionTweenProperty(current, end, setter));
            return this;
        }
        public Tween Add<T>(T start, T end, LerpFunction<T> lerpFunction, Action<T> setter)
        {
            var tweenProperty = new TweenProperty<T>(start, end, lerpFunction, setter);
            tweenedProperties.Add(tweenProperty);
            return this;
        }

        public void Update(GameTime gameTime)
        {
            UpdateTime(gameTime);
            UpdateProperties();
        }

        protected void UpdateTime(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        protected void UpdateProperties()
        {
            foreach (var property in tweenedProperties)
                property.Tween(Progress);
        }

    }

    public interface ITween
    {
        void Tween(float progress);
    }

    public delegate T LerpFunction<T>(T start, T end, float progress);

    public class TweenProperty<T> : ITween
    {
        protected T beginning;
        protected T current;
        protected T end;
        protected LerpFunction<T> lerpFunction;

        public TweenProperty(T current, T end, LerpFunction<T> lerpFunction, Action<T> setter)
        {
            this.lerpFunction = lerpFunction;
            this.beginning = current;
            this.current = current;
            this.end = end;
        }

        public void Tween(float progress)
        {
            var currentValue = lerpFunction(beginning, end, progress);
            current = currentValue;
        }
    }

    public class FloatTweenProperty : TweenProperty<float>
    {
        private static readonly LerpFunction<float> function = (s, e, p) => s + (e - s) * p;
        public FloatTweenProperty(float current, float end, Action<float> setter) : base(current, end, function, setter) { }
    }

    public class Vector2TweenProperty : TweenProperty<Vector2>
    {
        private static readonly LerpFunction<Vector2> function = (s, e, p) => { return Vector2.Lerp(s, e, p); };
        public Vector2TweenProperty(Vector2 current, Vector2 end, Action<Vector2> setter) : base(current, end, function, setter) { }
    }

    public class Vector3TweenProperty : TweenProperty<Vector3>
    {
        private static readonly LerpFunction<Vector3> function = (s, e, p) => { return Vector3.Lerp(s, e, p); };
        public Vector3TweenProperty(Vector3 current, Vector3 end, Action<Vector3> setter) : base(current, end, function, setter) { }
    }

    public class Vector4TweenProperty : TweenProperty<Vector4>
    {
        private static readonly LerpFunction<Vector4> function = (s, e, p) => { return Vector4.Lerp(s, e, p); };
        public Vector4TweenProperty(Vector4 current, Vector4 end, Action<Vector4> setter) : base(current, end, function, setter) { }
    }

    public class ColorTweenProperty : TweenProperty<Color>
    {
        private static readonly LerpFunction<Color> function = (s, e, p) => { return Color.Lerp(s, e, p); };
        public ColorTweenProperty(Color current, Color end, Action<Color> setter) : base(current, end, function, setter) { }
    }

    public class QuaternionTweenProperty : TweenProperty<Quaternion>
    {
        private static readonly LerpFunction<Quaternion> function = (s, e, p) => { return Quaternion.Lerp(s, e, p); };
        public QuaternionTweenProperty(Quaternion current, Quaternion end, Action<Quaternion> setter) : base(current, end, function, setter) { }
    }


}
