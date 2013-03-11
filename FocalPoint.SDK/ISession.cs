using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocalPoint.SDK
{
    public interface ISession
    {
        DateTime EndTime { get; }
        int Duration { get; }

        double PercentComplete { get; }
    }
}
