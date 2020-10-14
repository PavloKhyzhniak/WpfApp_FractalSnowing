using System;
using System.Windows.Media.Animation;

namespace WpfApp_FractalSnow
{
    public class SinusEase : EasingFunctionBase
    {
        public double Amplitude { get; set; } = 1;
        public double Period { get; set; } = 1;

        protected override double EaseInCore(double normalizedTime)
        {
            switch(EasingMode)
            {
                case EasingMode.EaseIn:
                    if (normalizedTime < 0.5)
                        return (1.5 - normalizedTime) * Amplitude * Math.Sin(normalizedTime * 2 * Math.PI * Period);
                    else
                        return Amplitude * Math.Sin(normalizedTime * 2 * Math.PI * Period);
                case EasingMode.EaseInOut:
                    return 1.5*Amplitude * Math.Sin(normalizedTime * 2 * Math.PI * Period);
                case EasingMode.EaseOut:
                    if (normalizedTime > 0.5)
                        return (0.5 + normalizedTime) * Amplitude * Math.Sin(normalizedTime * 2 * Math.PI * Period);
                    else
                        return Amplitude * Math.Sin(normalizedTime * 2 * Math.PI * Period);
            }
            return Amplitude * Math.Sin(normalizedTime * 2 * Math.PI * Period);
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new SinusEase();
        }
    }
}
