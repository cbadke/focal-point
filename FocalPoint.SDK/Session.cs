using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocalPoint.SDK
{
    public class Session
    {
        public DateTime EndTime { get; set; }
        public int Duration { get; set; }

        public int PercentComplete
        {
            get
            {
                var currentTime = DateTime.UtcNow;
                if (currentTime > EndTime) return 100;

                var sessionLength = new TimeSpan(0, Duration, 0);
                var timeRemaining = (EndTime - currentTime).TotalMilliseconds;

                return (int)(100 * (sessionLength.TotalMilliseconds - timeRemaining) / sessionLength.TotalMilliseconds);
            }
        }
    }
}
