using System;
using System.Windows;
using System.Windows.Media;

namespace CamFootprintTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool enableSlider = false;
        public MainWindow()
        {
            InitializeComponent();
            acHeight.Value = 50.0;
            vfv.Value = 30;
            hfv.Value = 60;
            PointCollection pointCol = new PointCollection(4);
            pointCol.Add(new Point(0, 0));
            pointCol.Add(new Point(150, 150));
            pointCol.Add(new Point(300, 150));
            pointCol.Add(new Point(150, 0));
            footprint.Points = pointCol;
            enableSlider = true;
            genNewFootprint();
        }

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (enableSlider)
                genNewFootprint();
        }

        private void genNewFootprint()
        {

            var TR = new Quaternion( hfv.Value / 2,  vfv.Value / 2, 0, true);
            var TL = new Quaternion(-hfv.Value / 2,  vfv.Value / 2, 0, true);
            var BR = new Quaternion( hfv.Value / 2, -vfv.Value / 2, 0, true);
            var BL = new Quaternion(-hfv.Value / 2, -vfv.Value / 2, 0, true);
            
            var gimRot = new Quaternion(gimRoll.Value, gimPitch.Value, gimYaw.Value, true);
            var acRot = new Quaternion(acRoll.Value, acPitch.Value, acYaw.Value, true);

            double r, p, y;

            Quaternion TR1 = acRot.Multiply(gimRot.Multiply(TR));
            Quaternion TL1 = acRot.Multiply(gimRot.Multiply(TL));
            Quaternion BR1 = acRot.Multiply(gimRot.Multiply(BR));
            Quaternion BL1 = acRot.Multiply(gimRot.Multiply(BL));

            var pc = new PointCollection(4);
            pc.Add(getPointOnGround(TR1));
            pc.Add(getPointOnGround(TL1));
            pc.Add(getPointOnGround(BL1));
            pc.Add(getPointOnGround(BR1));
            footprint.Points = bindToViewerDimensions(pc);
        }

        private Point getPointOnGround(Quaternion q)
        {
            double r;
            double p;
            double y;
            q.ConvertToEuler(out r, out p, out y, true);

            r = limit(r, -anglelimit.Value, anglelimit.Value);
            p = limit(p, -anglelimit.Value, anglelimit.Value);

            double dx = acHeight.Value * Math.Tan(q.Radians(r));
            double dy = acHeight.Value * Math.Tan(q.Radians(p));

            double utmx =  dx * Math.Cos(q.Radians(y)) - dy * Math.Sin(q.Radians(y)) + 150; // 150 is the centre of viewer. (-) sign is required on dy due to viewer
            double utmy = -dx * Math.Sin(q.Radians(y)) - dy * Math.Cos(q.Radians(y)) + 150; // 150 is the centre of viewer. (-) sign is required on dx due to viewer

            return new Point(utmx , utmy );
        }

        private PointCollection bindToViewerDimensions(PointCollection pc)
        {
            var outpc = new PointCollection(4);
            foreach(Point p in pc)
            {
                outpc.Add(new Point(limit(p.X, 0, 300), limit(p.Y, 0, 300)));
            }
            return outpc;
        }

        public double limit(double value, double min, double max)
        {
            if (value < min) { return min; }
            if (value > max) { return max; }
            return value;
        }
    }
}