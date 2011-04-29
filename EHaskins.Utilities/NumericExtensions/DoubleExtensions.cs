using System;

namespace EHaskins.Utilities.NumericExtensions
{
    public static class DoubleExtensions
    {
        public static double Deadband(this double val, double center, double range, double deadband)
        {
            if (deadband > range)
                throw new ArgumentOutOfRangeException("Deadband must be greater than range.");
            val = val.Limit(center - range, center + range);

            double factor = range/(range - deadband);
            if (val > center + deadband)
                val = (val - deadband) * factor;
            else if (val < center - deadband)
                val = (val + deadband) * factor;
            else
                val = 0;
            return val;
        }

        public static double Limit(this double val, double min, double max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("min must be less than max.");
            if (val < min) val = min;
            if (val > max) val = max;
            return val;
        }

        public static double Scale(this double val, double inMin, double inMax, double outMin, double outMax)
        {
            if (inMin > inMax)
                throw new ArgumentOutOfRangeException("inMin must be less than inMax.");
            if (outMin > outMax)
                throw new ArgumentOutOfRangeException("outMin must be less than outMax.");

            double factor = (outMax - outMin)/(inMax - inMin);
            val -= inMin;
            val *= factor;
            val += outMin;
            return val;
        }
    }
}
