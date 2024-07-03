using System;
using System.Drawing;
using System.Numerics;
using Terraria;

namespace GuardianClass;

public class CoolMath
{
    

    public static float EaseInBack(double x)
    {
        const double c1 = 1.70158;
        const double c3 = c1 + 1;

        return (float)(c3 * Math.Pow(x, 3) - c1 * Math.Pow(x, 2));
    }

    public static float EaseInBack6(double x)
    {
        const double c1 = 1.70158;
        const double c3 = c1 + 1;

        return (float)(c3 * Math.Pow(x, 6) - c1 * Math.Pow(x, 2));
    }
    public static Microsoft.Xna.Framework.Vector2 PointInRect(Microsoft.Xna.Framework.Rectangle rect)
    {
        return new Microsoft.Xna.Framework.Vector2(Main.rand.Next(0, rect.Width), Main.rand.Next(0, rect.Height));
    }

    public static Func<float, float> InOutQuadBlend = t => {
        if (t <= 0.5f) {
            return 2.0f * t * t;
        }

        t -= 0.5f;
        return 2.0f * t * (1.0f - t) + 0.5f;
    };

    public static Func<float, float> EaseOutBounce = x => {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (x < 1 / d1) {
            return n1 * x * x;
        }

        if (x < 2 / d1) {
            x -= 1.5f / d1;
            return n1 * x * x + 0.75f;
        }

        if (x < 2.5f / d1) {
            x -= 2.25f / d1;
            return n1 * x * x + 0.9375f;
        }

        x -= 2.625f / d1;
        return n1 * x * x + 0.984375f;
    };

    public static Func<float, float> EaseOutCubic = x => (float)(1 - Math.Pow(1 - x, 3));

    public static Func<float, float> EaseInCirc = x => 1 - (float)Math.Sqrt(1 - Math.Pow(x, 2));

    public static Func<float, float> EaseInCubic = x => x * x * x;

    public static Func<float, float> EaseInOutBack = x => {
        const double c1 = 1.70158;
        const double c2 = c1 * 1.525;

        return x < 0.5
            ? (float)(Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2) / 2)
            : (float)((Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2);
    };

    public static Func<float, float> EaseOutBack = x => {
        const double c1 = 1.70158;
        const double c3 = c1 + 1;
        return (float)(1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2));
    };
}
