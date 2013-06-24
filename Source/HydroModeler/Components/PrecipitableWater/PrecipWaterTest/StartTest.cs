using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrecipWaterTest
{
    class StartTest
    {
        static void Main()
        {
            PrecipWaterTest.Test test = new PrecipWaterTest.Test();
            test.Initialize();
            test.Process();
            test.finish();
        }
    }
}
