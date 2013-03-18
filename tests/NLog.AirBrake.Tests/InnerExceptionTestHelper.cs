using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLog.AirBrake.Tests
{
    public class InnerExceptionTestHelper
    {
        public void ThrowException1()
        {
            throw new Exception("Ex1");
        }

        public void ThrowException()
        {
            try
            {
                ThrowException1();
            }
            catch (Exception e)
            {
                try
                {
                    throw new Exception("Ex2", e);
                }
                catch (Exception e1)
                {
                    ThrowException2(e1);
                }
            }
        }

        public void ThrowException2(Exception e)
        {
            throw new Exception("Ex2", e);
        }


    }
}
