using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static WpfApp_FractalSnow.EnumerationExtension;

namespace WpfApp_FractalSnow
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rand = new Random();
        DispatcherTimer timer = new DispatcherTimer();

        EnumerationMember CurrentStatus1 = new EnumerationMember();
        EnumerationMember CurrentStatus2 = new EnumerationMember();
        EnumerationMember CurrentStatus3 = new EnumerationMember();
        EnumerationMember CurrentStatus4 = new EnumerationMember();
        EnumerationMember CurrentStatus5 = new EnumerationMember();

        public MainWindow()
        {          
            InitializeComponent();

            DataContext = this;

            timer.Interval = new TimeSpan(0,0,0,0,1000);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        List<Ellipse> removeList = new List<Ellipse>();
        List<Storyboard> storyboardList = new List<Storyboard>();

        bool flag_control = true;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!flag_control)
                return;

            //mainCanvas.Children.Clear();
            offsetAngleList.Clear();
            int max_deep = 5;//максимальная глубина рекурсии(количество слоев)
            //зададим размеры холста для формирования фигуры, иначе будет обрезана
            double size = 1000;
            //сгенерируем фигуру
            GenerateFractalSnowFirst(rand.Next(50,150), TypeFractalPrimitive.Line, new Point(size/2, size/2), rand.Next(360), rand.Next(3,max_deep));
            //используя полученную сокращенную запись фигуры, создадим ее
            Path new_path  = CreateFigureFromPath(currentPath);
            //окрасим фигуру
            new_path.Stroke = new LinearGradientBrush(Color.FromArgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255)),
                Color.FromArgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255)), 0.0);
            new_path.RenderTransformOrigin = new Point(0.5, 0.5);

            var storyboard = new Storyboard();


            //**************************************************
            //**************************************************
            //**************************************************
//Заменим стиль на code-behind для общего контролля всеми эффектами
            //            new_path.Style = (Style)TryFindResource("PathShadow");//наложим эффекты и любые другие стили

            new_path.Effect = new DropShadowEffect()
            {
                Color = Colors.Red,
                Direction = 100,
                RenderingBias = RenderingBias.Quality,
                ShadowDepth = 0,
                BlurRadius = 10,
                Opacity = 1
            };

            DoubleAnimation shadowDepth = new DoubleAnimation();
            shadowDepth.From = 0;
            shadowDepth.To = 7;
            shadowDepth.Duration = TimeSpan.FromSeconds(1.5);
            shadowDepth.AutoReverse = true;
            shadowDepth.RepeatBehavior = RepeatBehavior.Forever;

            storyboard.Children.Add(shadowDepth);
            Storyboard.SetTarget(shadowDepth, new_path);
            Storyboard.SetTargetProperty(shadowDepth, new PropertyPath("Effect.ShadowDepth"));

            DoubleAnimation direction = new DoubleAnimation();
            direction.From = 100;
            direction.To = 460;
            direction.Duration = TimeSpan.FromSeconds(4);
            direction.RepeatBehavior = RepeatBehavior.Forever;

            storyboard.Children.Add(direction);
            Storyboard.SetTarget(direction, new_path);
            Storyboard.SetTargetProperty(direction, new PropertyPath("Effect.Direction"));

            ColorAnimation color = new ColorAnimation();
            color.To = Colors.Blue;
            color.Duration = TimeSpan.FromSeconds(8);
            color.AutoReverse = true;
            color.RepeatBehavior = RepeatBehavior.Forever;

            storyboard.Children.Add(color);
            Storyboard.SetTarget(color, new_path);
            Storyboard.SetTargetProperty(color, new PropertyPath("Effect.Color"));
            //**************************************************
            //**************************************************
            //**************************************************

            new_path.Width = size;
            new_path.Height = size;
//            Path new_canvas = new_path;
            //разместим фигуру на холсте, его и будем анимировать
            Canvas new_canvas = new Canvas() { RenderTransformOrigin = new Point(0.5, 0.5) ,Width= size, Height= size };
            new_canvas.Children.Add(new_path);

            //отцентрируем нашу фигуру, совместим центр фигуры с верхним левым углом Canvas
            //ведь его мы будем анимировать - двигать по X и Y
            Canvas.SetTop(new_path, -size / 2);
            Canvas.SetLeft(new_path, -size / 2);

            //установим время всех анимаций
            double timeAnimation = 2 + rand.NextDouble() * 10;


            //Движение сверху-вниз(падение) с эффектом отскока
            BounceEase ease = new BounceEase();
            ease.EasingMode = EasingMode.EaseOut;
            ease.Bounces = rand.Next(2, 10);

            DoubleAnimation downAnimation = new DoubleAnimation();
            downAnimation.From = 0;
            downAnimation.To = mainCanvas.ActualHeight;
            downAnimation.EasingFunction = ease;
            downAnimation.Duration = TimeSpan.FromSeconds(timeAnimation);

//            new_canvas.BeginAnimation(Canvas.TopProperty, downAnimation);
            storyboard.Children.Add(downAnimation);
            Storyboard.SetTarget(downAnimation, new_canvas);
            Storyboard.SetTargetProperty(downAnimation, new PropertyPath("Top"));

            //Движение слева-направо, колебания, смещение, флуктуации
            SinusEase easySinus = new SinusEase();
            easySinus.EasingMode = EasingMode.EaseIn;
            easySinus.Amplitude = 0.1;
            easySinus.Period = rand.Next(1, 5);

            DoubleAnimation leftAnimation = new DoubleAnimation();
            leftAnimation.From = rand.Next((int)mainCanvas.ActualWidth);
            leftAnimation.To = rand.Next((int)mainCanvas.ActualWidth);
            leftAnimation.EasingFunction = easySinus;
            leftAnimation.Duration = TimeSpan.FromSeconds(timeAnimation);

//            new_canvas.BeginAnimation(Canvas.LeftProperty, leftAnimation);
            storyboard.Children.Add(leftAnimation);
            Storyboard.SetTarget(leftAnimation, new_canvas);
            Storyboard.SetTargetProperty(leftAnimation, new PropertyPath("Left"));

            //наложим общие эффекты преобразования
            var tg = new TransformGroup();
            //масштабирование
            var scale = new ScaleTransform() { ScaleX = 1, ScaleY = 1, CenterX = new_path.Width/2,CenterY = new_path.Height/2  };
            tg.Children.Add(scale);
            //вращение
            var rotate = new RotateTransform() { Angle = 0 };
            tg.Children.Add(rotate);
            new_canvas.RenderTransform = tg;


            //МАСШТАБИРОВАНИЕ НЕ ПОЛУЧИЛОСЬ
        //    DoubleAnimation scaleXAnimation = new DoubleAnimation();
        //    scaleXAnimation.From =0.1 + rand.NextDouble();
        //    scaleXAnimation.To = 0.5*rand.NextDouble();
        //    scaleXAnimation.AutoReverse = true;
        //    //scaleXYAnimation.RepeatBehavior = RepeatBehavior.Forever;
        //    scaleXAnimation.Duration = TimeSpan.FromSeconds(timeAnimation);
        //
        //    DoubleAnimation scaleYAnimation = new DoubleAnimation();
        //    scaleYAnimation.From = 0.1 + rand.NextDouble();
        //    scaleYAnimation.To = 0.5 * rand.NextDouble();
        //    scaleYAnimation.AutoReverse = true;
        //    scaleYAnimation.By = rand.Next(1, 5);
        //    //scaleXYAnimation.RepeatBehavior = RepeatBehavior.Forever;
        //    scaleYAnimation.Duration = TimeSpan.FromSeconds(timeAnimation);

            //вращение
            DoubleAnimation rotateAnimation = new DoubleAnimation();
            rotateAnimation.From = 0;
            rotateAnimation.To = 360*rand.Next(1,3);
            //rotateAnimation.RepeatBehavior = RepeatBehavior.Forever;
            rotateAnimation.Duration = TimeSpan.FromSeconds(timeAnimation);

            //обесцвечивание - исчезание, перед удалением
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.From = 1;
            opacityAnimation.To = 0;
            opacityAnimation.Duration = TimeSpan.FromSeconds(timeAnimation+15);//через 15 секунд удалим обьект
            opacityAnimation.BeginTime = TimeSpan.FromSeconds(timeAnimation);//начнем когда анимации завершены

            mainCanvas.Children.Add(new_canvas);

            //Canvas.SetTop(new_canvas, -size / 2);

//            var storyboard = new Storyboard();

        //    storyboard.Children.Add(scaleXAnimation);
        //    storyboard.Children.Add(scaleYAnimation);
            storyboard.Children.Add(rotateAnimation);
            storyboard.Children.Add(opacityAnimation);

            //Storyboard.SetTarget(scaleXAnimation, new_path);
            //Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            //Storyboard.SetTarget(scaleXAnimation, new_path);
            //Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            Storyboard.SetTarget(rotateAnimation, new_path);
            Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("RenderTransform.Children[1].Angle"));
            Storyboard.SetTarget(opacityAnimation, new_path);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));

            //storyboard.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            storyboard.Completed +=
              (sndr, evtArgs) => {
                  mainCanvas.Children.Remove(new_canvas);//по завершению анимирования данного обьекта, удалить
                  storyboardList.Remove(storyboard);
              };

            //отображение пути
            if (flag_show_way)
            {
                //            storyboard.CurrentTimeInvalidated += Storyboard_CurrentTimeInvalidated;
                storyboard.CurrentTimeInvalidated += (sndr, evtArgs) =>
                {
                    if (flag_show_way)//рисуем только если установлен флаг
                    {
                        double x = Canvas.GetLeft(new_canvas);// + new_canvas.RenderSize.Width / 2;
                        double y = Canvas.GetTop(new_canvas);// + new_canvas.RenderSize.Height / 2;

                        //Console.WriteLine(@"x = {0}, y = {1}", x, y);

                        Ellipse point = new Ellipse();
                        point.StrokeThickness = 0;

                        point.Fill = new SolidColorBrush(Colors.Red);
                        point.Width = 3;
                        point.Height = 3;
                        Canvas.SetLeft(point, x);
                        Canvas.SetTop(point, y);
                        mainCanvas.Children.Add(point);
                        removeList.Add(point);//учтем все нарисованные точки, для оперативного удаления
                    }
                };
            }
            else
            {
                //удаление пути с холста
                if (removeList.Count > 0)
                {
                    //очистим поле от точек пути
                    foreach (var item in removeList)
                        mainCanvas.Children.Remove(item);
                    removeList.Clear();
                }
            }

            storyboard.Begin();
            storyboardList.Add(storyboard);
        }

        //   private void Storyboard_CurrentTimeInvalidated(object sender, EventArgs e)
        //   {
        //       double x = Canvas.GetLeft();
        //       double y = Canvas.GetTop(rect1);
        //
        //       //Console.WriteLine(@"x = {0}, y = {1}", x, y);
        //
        //       Ellipse point = new Ellipse();
        //       point.StrokeThickness = 0;
        //
        //       point.Fill = pointSolidBrush;
        //       point.Width = 3;
        //       point.Height = 3;
        //       Canvas.SetLeft(point, x);
        //       Canvas.SetTop(point, y);
        //       canvas1.Children.Add(point);
        //   }
       

       
        
        public T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(rand.Next(v.Length));
        }

        //для соблюдения симметричности
        List<double> offsetAngleList = new List<double>();//чтоб количество поворотов(лучей) на каждом слое было одинаковым
        List<TypeFractalPrimitive> typeList = new List<TypeFractalPrimitive>();//теперь на каждом слое свой тип примитива формируеться

        string currentPath = null;
        private void GenerateFractalSnowFirst(double size, TypeFractalPrimitive type, Point startPoint, double startAngleGrad, int deep)
        {
            currentPath = "M " + startPoint.X + "," + startPoint.Y;

            //определим параметры будущей фигуры(фрактала)
            offsetAngleList.Clear();
            typeList.Clear();
            while (offsetAngleList.Count < deep)
            {
                offsetAngleList.Add(rand.Next(30, 90));
                typeList.Add(RandomEnumValue<TypeFractalPrimitive>());
            }

            if(flag_set_TypePrimitive)
            {
                typeList.Clear();
                typeList.Add((TypeFractalPrimitive)CurrentStatus5.Value);
                typeList.Add((TypeFractalPrimitive)CurrentStatus4.Value);
                typeList.Add((TypeFractalPrimitive)CurrentStatus3.Value);
                typeList.Add((TypeFractalPrimitive)CurrentStatus2.Value);
                typeList.Add((TypeFractalPrimitive)CurrentStatus1.Value);
            }

            if (size < 5 || deep <= 0)
                return;
        
            double offsetAngle = offsetAngleList[deep - 1];
        
            int count = (int)(360.0 / offsetAngle);
            offsetAngle = 360.0 / count;
            offsetAngle *= (2 * Math.PI / 360);//переводим в радианы
        
            double angleRad = startAngleGrad;
            angleRad *= (2*Math.PI/360);//переводим в радианы
        
            while (count-- > 0)
            {
                switch (typeList[deep-1])
                {
                    case TypeFractalPrimitive.Circle:
                        GenerateCircleFractal(size * 2 / 3, type, startPoint, angleRad, deep);
                        break;
                    case TypeFractalPrimitive.Rectangle:
                        GenerateRectangleFractal(size * 2 / 3, type, startPoint, angleRad, deep);
                        break;
                    case TypeFractalPrimitive.Line:
                        GenerateLineFractal(size * 2 / 3, type, startPoint, angleRad, deep);
                        break;
                    case TypeFractalPrimitive.Arc:
                        GenerateArcFractal(size * 2 / 3, type, startPoint, angleRad, deep);
                        break;
        
                }
                angleRad += offsetAngle;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

        private int _cutAngleGrad = 180;
        public int cutAngleGrad { get { return _cutAngleGrad; } set { _cutAngleGrad = value; OnPropertyChanged(); } }
        private void GenerateFractalSnow(double size, TypeFractalPrimitive type, Point startPoint, double startAngleRad, int deep)
        {
            if (size < 5 || deep <= 0)
                return;

            double offsetAngle = offsetAngleList[deep - 1];
            int count = (int)((360.0-cutAngleGrad) / offsetAngle);
            offsetAngle = (360.0 - cutAngleGrad) / count;
            offsetAngle *= (2 * Math.PI / 360);

            double angleRad = startAngleRad + (cutAngleGrad/2 - 180) * (2 * Math.PI / 360);

            while (count-- > 0)
            {
                switch (type)
                {
                    case TypeFractalPrimitive.Circle:
                        GenerateCircleFractal(size * 2 / 3, type, startPoint, angleRad, deep);
                        break;
                    case TypeFractalPrimitive.Rectangle:
                        GenerateRectangleFractal(size * 2 / 3, type, startPoint, angleRad, deep);
                        break;
                    case TypeFractalPrimitive.Line:
                        GenerateLineFractal(size * 2 / 3, type, startPoint, angleRad, deep);
                        break;
                    case TypeFractalPrimitive.Arc:
                        GenerateArcFractal(size * 2 / 3, type, startPoint, angleRad, deep);
                        break;

                }
                angleRad += offsetAngle;
            }

        }

        private void GenerateLineFractal(double size,TypeFractalPrimitive type, Point startPoint, double startAngleRad, int deep)
        {
           Point endPoint = new Point(startPoint.X + size * Math.Cos(startAngleRad), startPoint.Y + size * Math.Sin(startAngleRad));

        //    Line newLine = new Line()
        //    {
        //        X1 = startPoint.X,
        //        Y1 = startPoint.Y,
        //        X2 = endPoint.X,
        //        Y2 = endPoint.Y,
        //        StrokeThickness = 2,
        //    //    Stroke=Brushes.Red
        //        Stroke = new LinearGradientBrush(Color.FromArgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255)),
        //        Color.FromArgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255)),0.0)
        //    };

            currentPath += " M " + (int)startPoint.X  + "," + (int)startPoint.Y  + " L " + (int)endPoint.X  + "," + (int)endPoint.Y ;

            //Console.WriteLine("New Line(angle):" +startAngle);
            //Console.WriteLine("x1:" + newLine.X1);
            //Console.WriteLine("y1:" + newLine.Y1);
            //Console.WriteLine("x2:" + newLine.X2);
            //Console.WriteLine("y2:" + newLine.Y2);


        //    mainCanvas.Children.Add(newLine);
            
            TypeFractalPrimitive nextType = typeList[deep-1];
            GenerateFractalSnow(size, nextType, endPoint, startAngleRad,--deep);
        }

        private void GenerateArcFractal(double size, TypeFractalPrimitive type, Point startPoint, double startAngleRad, int deep)
        {
            Point endPoint = new Point(startPoint.X + size * Math.Cos(startAngleRad), startPoint.Y + size * Math.Sin(startAngleRad));

            //Data = "M150,100 A100,100 0 0 0 50,50"
            currentPath += " M " + (int)startPoint.X + "," + (int)startPoint.Y + " A " + (int)size + "," + (int)size + " 0 0 0 " + (int)endPoint.X + "," + (int)endPoint.Y;


            //Path arc_path = DrawArc(startPoint, size, startAngle,startAngle+size);

            TypeFractalPrimitive nextType = typeList[deep - 1];
            GenerateFractalSnow(size, nextType, endPoint, startAngleRad, --deep);
        }

        private void GenerateCircleFractal(double size, TypeFractalPrimitive type, Point startPoint, double startAngleRad, int deep)
        {
            Point endPoint = new Point(startPoint.X + size * Math.Cos(startAngleRad), startPoint.Y + size * Math.Sin(startAngleRad));

            //Data = "M150,100 A100,100 0 0 0 50,50"
            string Data = " M " + (int)endPoint.X + "," + (int)endPoint.Y 
                + " A " + (int)size + "," + (int)size + " 0 1 0 " + (int)startPoint.X + "," + (int)startPoint.Y;
            currentPath += Data;


            //mainCanvas.Children.Add(new Path() { Data = Geometry.Parse(Data),Stroke=Brushes.Red,StrokeThickness=2 });

            //Path arc_path = DrawArc(startPoint, size, startAngle,startAngle+size);

            TypeFractalPrimitive nextType = typeList[deep - 1];
            GenerateFractalSnow(size, nextType, endPoint, startAngleRad, --deep);
        }

        public Path DrawArc(Point startPoint, double radius, double start_angle, double end_angle)
        {
            Path arc_path = new Path();
            arc_path.Stroke = Brushes.Black;
            arc_path.StrokeThickness = 2;
            Canvas.SetLeft(arc_path, 0);
            Canvas.SetTop(arc_path, 0);

            start_angle = ((start_angle % (Math.PI * 2)) + Math.PI * 2) % (Math.PI * 2);
            end_angle = ((end_angle % (Math.PI * 2)) + Math.PI * 2) % (Math.PI * 2);
            if (end_angle < start_angle)
            {
                double temp_angle = end_angle;
                end_angle = start_angle;
                start_angle = temp_angle;
            }
            double angle_diff = end_angle - start_angle;
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            ArcSegment arcSegment = new ArcSegment();
            arcSegment.IsLargeArc = angle_diff >= Math.PI;
            //Set start of arc
            pathFigure.StartPoint = new Point(startPoint.X + radius * Math.Cos(start_angle), startPoint.Y + radius * Math.Sin(start_angle));
            //set end point of arc.
            arcSegment.Point = new Point(startPoint.X + radius * Math.Cos(end_angle), startPoint.Y + radius * Math.Sin(end_angle));
            arcSegment.Size = new Size(radius, radius);
            arcSegment.SweepDirection = SweepDirection.Clockwise;

            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);
            arc_path.Data = pathGeometry;
            
            return arc_path;
        }

        private void GenerateRectangleFractal(double size, TypeFractalPrimitive type, Point startPoint, double startAngleRad, int deep)
        {
            Point endPoint = new Point(startPoint.X + size * Math.Cos(startAngleRad), startPoint.Y + size * Math.Sin(startAngleRad));

            //Data = "M150,100 A100,100 0 0 0 50,50"
            currentPath += " M " + (int)startPoint.X + "," + (int)startPoint.Y
                + " L " + (int)endPoint.X + "," + (int)startPoint.Y
                + " L " + (int)endPoint.X + "," + (int)endPoint.Y
                + " L " + (int)startPoint.X + "," + (int)endPoint.Y
                + " L " + (int)startPoint.X + "," + (int)startPoint.Y;

            //Path arc_path = DrawArc(startPoint, size, startAngle, startAngle + size);

            TypeFractalPrimitive nextType = typeList[deep - 1];
            GenerateFractalSnow(size, nextType, endPoint, startAngleRad, --deep);
        }

        public Rectangle CreateRectangle()
        {
            // Create a Rectangle  
            Rectangle blueRectangle = new Rectangle();
            blueRectangle.Height = 100;
            blueRectangle.Width = 200;
            // Create a blue and a black Brush  
            SolidColorBrush blueBrush = new SolidColorBrush();
            blueBrush.Color = Colors.Blue;
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            // Set Rectangle's width and color  
            blueRectangle.StrokeThickness = 2;
            blueRectangle.Stroke = blackBrush;
            // Fill rectangle with blue color  
            blueRectangle.Fill = blueBrush;

            return blueRectangle;
        }

        Fractal fractal;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
     
    //        // Создаем фрактал глубины 2 (её можно менять, 1-5)
    //        fractal = new Fractal();
    //        fractal.Create(2);
    //        
    //        PathGeometry pg = new PathGeometry();
    //        pg = PathGeometry.CreateFromGeometry(Geometry.Parse(fractal.PathGeometry));
    //        Pen pen = new Pen(Brushes.AliceBlue, 2);
    //        Path p = new Path()
    //        {
    //            StrokeThickness=2,
    //            Stroke=Brushes.Blue,
    //            RenderTransformOrigin=new Point(0.5,0.5),
    //        };
    //
    //        TransformGroup tg = new TransformGroup();
    //
    //        // Получаем размеры фрактала
    //        // И выбираем масштаб так чтобы он вписался в окно
    //        var rect = fractal.Path.GetBounds();
    //        var scale = Math.Min(
    //            (float)(mainCanvas.ActualWidth - 50) / rect.Width,
    //            (float)(mainCanvas.ActualHeight - 50) / rect.Height);
    //
    //        scale = 0.2F;
    //        // Перемещаем (0, 0) в центр окна и устанавливаем масштаб
    //        tg.Children.Add(new TranslateTransform(mainCanvas.ActualWidth / 2, mainCanvas.ActualHeight / 2));
    //        tg.Children.Add(new ScaleTransform(scale, scale));
    //        tg.Children.Add(new TranslateTransform(-rect.X - rect.Width / 2, -rect.Y - rect.Height / 2));
    //
    //        p.RenderTransform =tg;
    //        p.Data = Geometry.Parse(fractal.PathGeometry);
    //        
    //        mainCanvas.Children.Add(p);
        }

        private Path CreateFigureFromPath(string pathString)
        {
            Path path = new Path()
            {
                StrokeThickness=2,
                Stroke=Brushes.Blue,
                RenderTransformOrigin=new Point(0.5,0.5),
            };
            
            TransformGroup tg = new TransformGroup();

            //    // Получаем размеры фрактала
            //    // И выбираем масштаб так чтобы он вписался в окно
            //    var rect = fractal.Path.GetBounds();
            //    var scale = Math.Min(
            //        (float)(mainCanvas.ActualWidth - 50) / rect.Width,
            //        (float)(mainCanvas.ActualHeight - 50) / rect.Height);
            //    
            //    scale = 0.2F;
            //    // Перемещаем (0, 0) в центр окна и устанавливаем масштаб
            //    tg.Children.Add(new TranslateTransform(mainCanvas.ActualWidth / 2, mainCanvas.ActualHeight / 2));
            tg.Children.Add(new ScaleTransform(0.3, 0.3));
            tg.Children.Add(new RotateTransform());
            //    tg.Children.Add(new TranslateTransform(-rect.X - rect.Width / 2, -rect.Y - rect.Height / 2));

            path.RenderTransform =tg;
            path.Data = Geometry.Parse(pathString);
            
            return path;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem != null)
                {
                    string str = ((ComboBoxItem)comboBox.SelectedItem).Content.ToString();
                    timer.Interval = new TimeSpan(0, 0, 0,0, int.Parse(str));
                }
            }
        }

        public bool flag_show_way = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            flag_show_way = !flag_show_way;

            if (flag_show_way)
                button.Background = Brushes.Blue;
            else
                button.Background = Brushes.Green;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mainCanvas.Children.Clear();
        }
              
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            flag_control = false;
            foreach (var item in storyboardList)
                item.Pause();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            flag_control = true;
            foreach (var item in storyboardList)
                item.Resume();
        }

        bool flag_set_TypePrimitive = false;
        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ComboBox comboBox)
            {
                if (comboBox.SelectedIndex == -1 && comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;

                CurrentStatus1 = (EnumerationMember)comboBox.SelectedItem;
            }
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedIndex == -1 && comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;

                CurrentStatus2 = (EnumerationMember)comboBox.SelectedItem;
            }
        }

        private void ComboBox_SelectionChanged_3(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedIndex == -1 && comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;

                CurrentStatus3 = (EnumerationMember)comboBox.SelectedItem;
            }
        }

        private void ComboBox_SelectionChanged_4(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedIndex == -1 && comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;

                CurrentStatus4 = (EnumerationMember)comboBox.SelectedItem;
            }
        }

        private void ComboBox_SelectionChanged_5(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedIndex == -1 && comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;

                CurrentStatus5 = (EnumerationMember)comboBox.SelectedItem;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
               flag_set_TypePrimitive = (bool)checkBox.IsChecked;
        }
    }
}
