using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pleaeing
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
        private static List<TweenTimeline> timelines;

        //For one-off tweens
        private static List<TweenTimeline> singleTimelines;
        private static List<TweenTimeline> removeTimelines;

        static Tweening()
        {
            timelines = new List<TweenTimeline>();
            singleTimelines = new List<TweenTimeline>();
            removeTimelines = new List<TweenTimeline>();
        }

        /// <summary>
        /// Creates a new tween and timeline.
        /// </summary>
        /// <param name="obj">The object to tween.</param>
        /// <param name="easingFunction">The easing function to use. (e.g. Easing.Linear)</param>
        /// <param name="endTime">The time, in milliseconds when the tween will end.</param>
        /// <param name="startTime">The time in milliseconds when the tween will begin.</param>
        /// <returns>A TweenTimeline with a tween attached.</returns>
        public static void Tween<T>(object target, string propertyName, T value, float duration, EasingFunction easingFunction, LerpFunction<T> lerpFunction, float delay = 0)
        {
            var timeline = new TweenTimeline();
            var property = timeline.AddProperty<T>(target, propertyName, lerpFunction);
            property.AddFrame(duration, value);
            singleTimelines.Add(timeline);
        }

        public static TweenTimeline NewTimeline()
        {
            var timeline = new TweenTimeline(0);
            timeline.AdaptiveDuration = true;
            timelines.Add(timeline);
            return timeline;
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
            timelines.Add(timeline);
            return timeline;
        }

        public static void Clear()
        {
            timelines.Clear();
            singleTimelines.Clear();
            removeTimelines.Clear();
        }

        public static void Update(GameTime gameTime)
        {
            foreach (var timeline in timelines)
            {
                timeline.Update(gameTime);
            }

            foreach (var timeline in singleTimelines)
            {
                timeline.Update(gameTime);
                if (timeline.State == TweenState.Stopped)
                    removeTimelines.Add(timeline);
            }
            foreach(var timeline in removeTimelines)
            {
                singleTimelines.Remove(timeline);
            }
            removeTimelines.Clear();
        }
    }

    /// <summary>
    /// The TweenTimeline holds tweens and runs them in sequence based on the elapsed time.
    /// </summary>
    public class TweenTimeline
    {
        public List<ITweenableProperty> tweeningProperties;
        public float elapsedMilliseconds;
        public bool Loop;
        public float duration;
        public TweenState State;
        public bool AdaptiveDuration;

        public TweenTimeline() : this(0)
        {
            AdaptiveDuration = true;
        }
        public TweenTimeline(float duration)
        {
            this.duration = duration;
            tweeningProperties = new List<ITweenableProperty>();
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
            for (int i = tweeningProperties.Count - 1; i >= 0; i--)
            {
                tweeningProperties[i].Reset();
            }
        }

        public TweenableProperty<T> AddProperty<T>(object target, string propertyName, LerpFunction<T> lerpFunction)
        {
            var properties = target.GetType().GetProperties();
            var fields = target.GetType().GetFields();

            var property = properties.FirstOrDefault(x => x.Name == propertyName);
            if (property != null)
            {
                if (property.PropertyType == typeof(T))
                {
                    var t = new TweenProperty<T>(target, property, lerpFunction);
                    tweeningProperties.Add(t);
                    return t;
                }
            }
            else
            {
                var field = fields.FirstOrDefault(x => x.Name == propertyName);
                if (field != null)
                {
                    if (field.FieldType == typeof(T))
                    {
                        var t = new TweenField<T>(target, field, lerpFunction);
                        tweeningProperties.Add(t);
                        return t;
                    }
                }
            }
            return null;
        }
        public TweenableProperty<float> AddFloat(object target, string propertyName) 
        {
            return AddProperty(target, propertyName, LerpFunctions.Float);
        }
        public TweenableProperty<Vector2> AddVector2(object target, string propertyName)
        {
            return AddProperty(target, propertyName, LerpFunctions.Vector2);
        }
        public TweenableProperty<Vector3> AddVector3(object target, string propertyName)
        {
            return AddProperty(target, propertyName, LerpFunctions.Vector3);
        }
        public TweenableProperty<Vector4> AddVector4(object target, string propertyName)
        {
            return AddProperty(target, propertyName, LerpFunctions.Vector4);
        }
        public TweenableProperty<Color> AddColor(object target, string propertyName)
        {
            return AddProperty(target, propertyName, LerpFunctions.Color);
        }
        public TweenableProperty<Quaternion> AddQuaternion(object target, string propertyName)
        {
            return AddProperty(target, propertyName, LerpFunctions.Quaternion);
        }
        public TweenableProperty<Rectangle> AddRectangle(object target, string propertyName)
        {
            return AddProperty(target, propertyName, LerpFunctions.Rectangle);
        }

        public TweenableProperty<T> AddProperty<T>(T initialValue, Action<T> setter, LerpFunction<T> lerpFunction)
        {
            var t = new TweenSetter<T>(initialValue, setter, lerpFunction);
            return t;
        }
        public TweenableProperty<float> AddFloat(float initialValue, Action<float> setter)
        {
            return AddProperty(initialValue, setter, LerpFunctions.Float);
        }
        public TweenableProperty<Vector2> AddVector2(Vector2 initialValue, Action<Vector2> setter)
        {
            return AddProperty(initialValue, setter, LerpFunctions.Vector2);
        }
        public TweenableProperty<Vector3> AddVector3(Vector3 initialValue, Action<Vector3> setter)
        {
            return AddProperty(initialValue, setter, LerpFunctions.Vector3);
        }
        public TweenableProperty<Vector4> AddVector4(Vector4 initialValue, Action<Vector4> setter)
        {
            return AddProperty(initialValue, setter, LerpFunctions.Vector4);
        }
        public TweenableProperty<Color> AddColor(Color initialValue, Action<Color> setter)
        {
            return AddProperty(initialValue, setter, LerpFunctions.Color);
        }
        public TweenableProperty<Rectangle> AddRectangle(Rectangle initialValue, Action<Rectangle> setter)
        {
            return AddProperty(initialValue, setter, LerpFunctions.Rectangle);
        }
        public TweenableProperty<Quaternion> AddQuaternion(Quaternion initialValue, Action<Quaternion> setter)
        {
            return AddProperty(initialValue, setter, LerpFunctions.Quaternion);
        }

        public void Update(GameTime gameTime)
        {
            if(AdaptiveDuration)
            {
                elapsedMilliseconds += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if(State == TweenState.Running)
                {
                    var propertiesFinished = 0;
                    foreach (var property in tweeningProperties)
                    {
                        if(!property.Update(elapsedMilliseconds)) //No Frames Left
                        {
                            propertiesFinished++;
                        }
                    }
                    if (propertiesFinished == tweeningProperties.Count)
                    {
                        elapsedMilliseconds = 0;
                        if (!Loop)
                        {
                            State = TweenState.Stopped;
                        }
                    }
                }
            }
            else if (State == TweenState.Running)
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
                    foreach (var property in tweeningProperties)
                    {
                        property.Update(elapsedMilliseconds);                        
                    }                    
                }
            }
        }
    }

    public interface ITweenableProperty
    {
        bool Update(float timelineElapsed);
        void Reset();
    }
    public abstract class TweenableProperty<T> : ITweenableProperty
    {
        protected T initialValue;
        protected List<TweenKeyFrame<T>> keyFrames;
        protected LerpFunction<T> lerpFunction;
        protected bool Done;

        public TweenableProperty(LerpFunction<T> lerpFunction)
        {
            keyFrames = new List<TweenKeyFrame<T>>();
            this.lerpFunction = lerpFunction;
        }

        public TweenableProperty<T> AddFrame(float frame, T value)
        {
            return AddFrame(frame, value, Easing.Linear);
        }
        public TweenableProperty<T> AddFrame(float frame, T value, EasingFunction easingFunction)
        {
            var newFrame = new TweenKeyFrame<T>(frame, value, easingFunction);
            keyFrames.Add(newFrame);
            keyFrames = keyFrames.OrderBy(x => x.frame).ToList();
            return this;
        }

        public bool Update(float timelineElapsed)
        {
            TweenKeyFrame<T> lastFrame = null;
            TweenKeyFrame<T> nextFrame = null;
            foreach(var frame in keyFrames)
            {
                if(frame.frame <= timelineElapsed)
                {
                    lastFrame = frame;
                }
                else
                {
                    nextFrame = frame;
                    break;
                }
            }

            if(nextFrame == null)
            {
                if(!Done)
                    SetValue(lastFrame.value);

                Done = true;
                return false;
            }
            else
            {
                if (lastFrame == null)
                {
                    var progress = timelineElapsed / nextFrame.frame;
                    var easedProgress = nextFrame.easingFunction(progress);
                    var newValue = lerpFunction(initialValue, nextFrame.value, easedProgress);
                    SetValue(newValue);
                }
                else
                {
                    var progress = (timelineElapsed - lastFrame.frame) / (nextFrame.frame - lastFrame.frame);
                    var easedProgress = nextFrame.easingFunction(progress);
                    var newValue = lerpFunction(lastFrame.value, nextFrame.value, easedProgress);
                    SetValue(newValue);
                }
            }

            return true;
            
        }

        public abstract void SetValue(T value);

        public void Reset()
        {
            
        }
    }
    internal class TweenProperty<T> : TweenableProperty<T>
    {
        public object target;
        public PropertyInfo property;

        public TweenProperty(object target, PropertyInfo property, LerpFunction<T> lerpFunction)
            :base(lerpFunction)
        {
            this.target = target;
            this.property = property;
            initialValue = (T)property.GetValue(target);
        }
        
        public override void SetValue(T value)
        {
            property.SetValue(target, value);
        }
    }
    internal class TweenField<T> : TweenableProperty<T>
    {
        public object target;
        public FieldInfo field;

        public TweenField(object target, FieldInfo field, LerpFunction<T> lerpFunction)
            :base(lerpFunction)
        {
            this.target = target;
            this.field = field;
            initialValue = (T)field.GetValue(target);
        }
        
        public override void SetValue(T value)
        {
            field.SetValue(target, value);
        }
    }
    internal class TweenSetter<T> : TweenableProperty<T>
    {
        public Action<T> setter;

        public TweenSetter(T initialValue, Action<T> setter, LerpFunction<T> lerpFunction)
            : base(lerpFunction)
        {
            this.setter = setter;
        }
        
        public override void SetValue(T value)
        {
            setter.Invoke(value);
        }
    }

    public class TweenKeyFrame<T>
    {
        public float frame;
        public T value;
        public EasingFunction easingFunction;

        public TweenKeyFrame(float frame, T value, EasingFunction easingFunction)
        {
            this.frame = frame;
            this.value = value;
            this.easingFunction = easingFunction;
        }
    }

    public delegate float EasingFunction(float val);
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
