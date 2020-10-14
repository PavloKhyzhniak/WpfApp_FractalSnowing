using System;
using System.Windows.Media.Animation;

namespace WpfApp_FractalSnow
{
    class PowerOfSixEase : EasingFunctionBase
    {
        protected override double EaseInCore(double normalizedTime)
        {
            return Math.Pow(normalizedTime, 6);
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new SinusEase();
        }
    }
}
