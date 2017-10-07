using System;

namespace CamFootprintTester
{
    public class Quaternion
    {
        public double w { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public Quaternion()
        {

        }

        public Quaternion(double roll, double pitch, double yaw, bool isDegrees)
        {
            if (isDegrees)
            {
                roll = Radians(roll);
                pitch = Radians(pitch);
                yaw = Radians(yaw);
            }
            double cy = Math.Cos(yaw * 0.5);
            double sy = Math.Sin(yaw * 0.5);
            double cr = Math.Cos(roll * 0.5);
            double sr = Math.Sin(roll * 0.5);
            double cp = Math.Cos(pitch * 0.5);
            double sp = Math.Sin(pitch * 0.5);

            w = cy * cr * cp + sy * sr * sp;
            x = cy * sr * cp - sy * cr * sp;
            y = cy * cr * sp + sy * sr * cp;
            z = sy * cr * cp - cy * sr * sp;
        }

        public void ConvertToEuler(out double roll, out double pitch, out double yaw, bool returnDegrees)
        {
            // roll (x-axis rotation)
            double sinr = +2.0 * (w * x + y * z);
            double cosr = +1.0 - 2.0 * (x * x + y * y);
            roll = Math.Atan2(sinr, cosr);

            // pitch (y-axis rotation)
            double sinp = +2.0 * (w * y - z * x);
            if (Math.Abs(sinp) >= 1)
                pitch = CopySign(Math.PI / 2, sinp); // use 90 degrees if out of range
            else
                pitch = Math.Asin(sinp);

            // yaw (z-axis rotation)
            double siny = +2.0 * (w * z + x * y);
            double cosy = +1.0 - 2.0 * (y * y + z * z);
            yaw = Math.Atan2(siny, cosy);

            if (returnDegrees)
            {
                roll = Degrees(roll);
                pitch = Degrees(pitch);
                yaw = Degrees(yaw);
            }
        }

        public Quaternion Multiply(Quaternion q2)
        {
            return new Quaternion()
            {
                x = x * q2.w + y * q2.z - z * q2.y + w * q2.x,
                y = -x * q2.z + y * q2.w + z * q2.x + w * q2.y,
                z = x * q2.y - y * q2.x + z * q2.w + w * q2.z,
                w = -x * q2.x - y * q2.y - z * q2.z + w * q2.w
            };
        }

        public double CopySign(double x, double y)
        {
            return Math.Abs(x) * Math.Sign(y);
        }

        public double Radians(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        public double Degrees(double rad)
        {
            return rad * 180.0 / Math.PI;
        }
        
    }
}