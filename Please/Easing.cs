using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Please
{
    public enum EasingFunction
    {
        Linear,
        QuadraticIn,
        QuadraticOut,
        QuadraticInOut,
        CubicIn,
        CubicOut,
        CubicInOut,
        QuarticIn,
        QuarticOut,
        QuarticInOut,
        QuinticIn,
        QuinticOut,
        QuinticInOut,
        SinusoidalIn,
        SinusoidalOut,
        SinusoidalInOut,
        ExponentialIn,
        ExponentialOut,
        ExponentialInOut,
        CircularIn,
        CircularOut,
        CircularInOut,
        ElasticIn,
        ElasticOut,
        ElasticInOut,
        BackIn,
        BackOut,
        BackInOut,
        BounceIn,
        BounceOut,
        BounceInOut,

    }

    public static class Easing
    {
        public static float Ease(EasingFunction easingFunction, float k)
        {
            switch (easingFunction)
            {
                case EasingFunction.Linear:
                    return Linear(k);
                case EasingFunction.QuadraticIn:
                    return Quadratic.In(k);
                case EasingFunction.QuadraticOut:
                    return Quadratic.Out(k);
                case EasingFunction.QuadraticInOut:
                    return Quadratic.InOut(k);
                case EasingFunction.CubicIn:
                    return Cubic.In(k);
                case EasingFunction.CubicOut:
                    return Cubic.Out(k);
                case EasingFunction.CubicInOut:
                    return Cubic.InOut(k);
                case EasingFunction.QuarticIn:
                    return Quartic.In(k);
                case EasingFunction.QuarticOut:
                    return Quartic.Out(k);
                case EasingFunction.QuarticInOut:
                    return Quartic.InOut(k);
                case EasingFunction.QuinticIn:
                    return Quintic.In(k);
                case EasingFunction.QuinticOut:
                    return Quintic.Out(k);
                case EasingFunction.QuinticInOut:
                    return Quintic.InOut(k);
                case EasingFunction.SinusoidalIn:
                    return Sinusoidal.In(k);
                case EasingFunction.SinusoidalOut:
                    return Sinusoidal.Out(k);
                case EasingFunction.SinusoidalInOut:
                    return Sinusoidal.InOut(k);
                case EasingFunction.ExponentialIn:
                    return Exponential.In(k);
                case EasingFunction.ExponentialOut:
                    return Exponential.Out(k);
                case EasingFunction.ExponentialInOut:
                    return Exponential.InOut(k);
                case EasingFunction.CircularIn:
                    return Circular.In(k);
                case EasingFunction.CircularOut:
                    return Circular.Out(k);
                case EasingFunction.CircularInOut:
                    return Circular.InOut(k);
                case EasingFunction.ElasticIn:
                    return Elastic.In(k);
                case EasingFunction.ElasticOut:
                    return Elastic.Out(k);
                case EasingFunction.ElasticInOut:
                    return Elastic.InOut(k);
                case EasingFunction.BackIn:
                    return Back.In(k);
                case EasingFunction.BackOut:
                    return Back.Out(k);
                case EasingFunction.BackInOut:
                    return Back.InOut(k);
                case EasingFunction.BounceIn:
                    return Bounce.In(k);
                case EasingFunction.BounceOut:
                    return Bounce.Out(k);
                case EasingFunction.BounceInOut:
                    return Bounce.InOut(k);
                default:
                    return Linear(k);
            }
            
        }

        public static float Linear(float k)
        {
            return k;
        }

        public class Quadratic
        {
            public static float In(float k)
            {
                return k * k;
            }

            public static float Out(float k)
            {
                return k * (2f - k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k;
                return -0.5f * ((k -= 1f) * (k - 2f) - 1f);
            }
        };

        public class Cubic
        {
            public static float In(float k)
            {
                return k * k * k;
            }

            public static float Out(float k)
            {
                return 1f + ((k -= 1f) * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k * k;
                return 0.5f * ((k -= 2f) * k * k + 2f);
            }
        };

        public class Quartic
        {
            public static float In(float k)
            {
                return k * k * k * k;
            }

            public static float Out(float k)
            {
                return 1f - ((k -= 1f) * k * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k * k * k;
                return -0.5f * ((k -= 2f) * k * k * k - 2f);
            }
        };

        public class Quintic
        {
            public static float In(float k)
            {
                return k * k * k * k * k;
            }

            public static float Out(float k)
            {
                return 1f + ((k -= 1f) * k * k * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k * k * k * k;
                return 0.5f * ((k -= 2f) * k * k * k * k + 2f);
            }
        };

        public class Sinusoidal
        {
            public static float In(float k)
            {
                return 1f - (float)Math.Cos(k * Math.PI / 2f);
            }

            public static float Out(float k)
            {
                return (float)Math.Sin(k * Math.PI / 2f);
            }

            public static float InOut(float k)
            {
                return 0.5f * (1f - (float)Math.Cos(Math.PI * k));
            }
        };

        public class Exponential
        {
            public static float In(float k)
            {
                return k == 0f ? 0f : (float)Math.Pow(1024f, k - 1f);
            }

            public static float Out(float k)
            {
                return k == 1f ? 1f : 1f - (float)Math.Pow(2f, -10f * k);
            }

            public static float InOut(float k)
            {
                if (k == 0f) return 0f;
                if (k == 1f) return 1f;
                if ((k *= 2f) < 1f) return 0.5f * (float)Math.Pow(1024f, k - 1f);
                return 0.5f * (float)(-Math.Pow(2f, -10f * (k - 1f)) + 2f);
            }
        };

        public class Circular
        {
            public static float In(float k)
            {
                return 1f - (float)Math.Sqrt(1f - k * k);
            }

            public static float Out(float k)
            {
                return (float)Math.Sqrt(1f - ((k -= 1f) * k));
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return -0.5f * (float)(Math.Sqrt(1f - k * k) - 1);
                return 0.5f * (float)(Math.Sqrt(1f - (k -= 2f) * k) + 1f);
            }
        };

        public class Elastic
        {
            public static float In(float k)
            {
                if (k == 0) return 0;
                if (k == 1) return 1;
                return (float)-Math.Pow(2f, 10f * (k -= 1f)) * (float)Math.Sin((k - 0.1f) * (2f * Math.PI) / 0.4f);
            }

            public static float Out(float k)
            {
                if (k == 0) return 0;
                if (k == 1) return 1;
                return (float)Math.Pow(2f, -10f * k) * (float)Math.Sin((k - 0.1f) * (2f * Math.PI) / 0.4f) + 1f;
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return -0.5f * (float)Math.Pow(2f, 10f * (k -= 1f)) * (float)Math.Sin((k - 0.1f) * (2f * Math.PI) / 0.4f);
                return (float)Math.Pow(2f, -10f * (k -= 1f)) * (float)Math.Sin((k - 0.1f) * (2f * Math.PI) / 0.4f) * 0.5f + 1f;
            }
        };

        public class Back
        {
            static float s = 1.70158f;
            static float s2 = 2.5949095f;

            public static float In(float k)
            {
                return k * k * ((s + 1f) * k - s);
            }

            public static float Out(float k)
            {
                return (k -= 1f) * k * ((s + 1f) * k + s) + 1f;
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * (k * k * ((s2 + 1f) * k - s2));
                return 0.5f * ((k -= 2f) * k * ((s2 + 1f) * k + s2) + 2f);
            }
        };

        public class Bounce
        {
            public static float In(float k)
            {
                return 1f - Out(1f - k);
            }

            public static float Out(float k)
            {
                if (k < (1f / 2.75f))
                {
                    return 7.5625f * k * k;
                }
                else if (k < (2f / 2.75f))
                {
                    return 7.5625f * (k -= (1.5f / 2.75f)) * k + 0.75f;
                }
                else if (k < (2.5f / 2.75f))
                {
                    return 7.5625f * (k -= (2.25f / 2.75f)) * k + 0.9375f;
                }
                else
                {
                    return 7.5625f * (k -= (2.625f / 2.75f)) * k + 0.984375f;
                }
            }

            public static float InOut(float k)
            {
                if (k < 0.5f) return In(k * 2f) * 0.5f;
                return Out(k * 2f - 1f) * 0.5f + 0.5f;
            }
        };
    }
}
