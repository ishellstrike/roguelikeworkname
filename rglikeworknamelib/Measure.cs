using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
#if MEASURE
namespace rglikeworknamelib
{
    public static class Measure {
        public static Measurer sectorCreation = new Measurer();
        public static Measurer sectorSaving = new Measurer();
        public static Measurer store = new Measurer();
        public static Measurer unstore = new Measurer();
    }

    public class Measurer {
        private Stopwatch time = new Stopwatch();
        public long Count { get; private set; }

        public void Begin() {
            time.Start();
        }

        public void End() {
            if (time.IsRunning) {
                Count++;
                time.Stop();
            }
        }

        public TimeSpan GetAverageTime() {
            if (Count == 0) return TimeSpan.Zero;
            return new TimeSpan(time.ElapsedTicks/Count);
        }
    }
}
#endif
