using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Labs.Utility {

    public class Number {

        public static Random r = new Random((int)DateTime.Now.Ticks);
        
        public static int RandomInt(int min, int max) {
            return r.Next(min, max);
        }

        public static double RandomDouble(double min, double max) {
            return r.NextDouble() * (max - min) + min;

        }
    }
}
