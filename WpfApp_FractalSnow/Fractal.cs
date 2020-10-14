using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WpfApp_FractalSnow
{
    public class Fractal
    {
        PointF Current; // Текущее положение
        float Length = 1; // Длина шага (любое число)
        float Angle = 0; // Текущий угол
        float BaseAngle; // Базовый угол (на который поворачиваемся)

        public GraphicsPath Path = null;
        public string PathGeometry = null;
        // Начальные параметры
        void Init()
        {
            Current = new PointF(0, 0);
            BaseAngle = (float)Math.PI / 2;
            Angle = 0;
        }

        // Создает фрактал по аксиоме
        public void Create(int L)
        {
            Path = new GraphicsPath();
            PathGeometry = "M 0,0";
            Init();

            // Аксиома: F-F-F-F
            F(L); M(); F(L); M(); F(L); M(); F(L);
        }

        // Правило F: F-B+FF-F-FF-FB-FF+B-FF+F+FF+FB+FFF
        void F(int L)
        {
            Move(true);
            if (L-- == 0) return;

            F(L); M(); B(L); P(); F(L); F(L); M(); F(L); M();
            F(L); F(L); M(); F(L); B(L); M(); F(L); F(L); P();
            B(L); M(); F(L); F(L); P(); F(L); P(); F(L); F(L);
            P(); F(L); B(L); P(); F(L); F(L); F(L);
        }

        // Правило B: BBBBBB
        void B(int L)
        {
            Move(false);
            if (L-- == 0) return;

            B(L); B(L); B(L); B(L); B(L); B(L);
        }

        // Правила + и - (плюс и минус) - это повороты
        void P() { Angle += BaseAngle; }
        void M() { Angle -= BaseAngle; }

        // Функция перемещения на один шаг вдоль текущего направления (Angle)
        // Если NeedToDraw = true то рисуем линию за собой, иначе - не рисуем
        void Move(bool NeedToDraw = true)
        {
            PointF Next = new PointF(
            Current.X + (float)Math.Cos(Angle) * Length,
            Current.Y + (float)Math.Sin(Angle) * Length);

            if (NeedToDraw)
            {
                Path.AddLine(Current, Next);
                PathGeometry += " M " + (int)Current.X*10 + "," + (int)Current.Y * 10 + " L " + (int)Next.X * 10 + "," + (int)Next.Y * 10;
                Path.CloseFigure(); 
            }
            Current = Next;

        }
    }
}
