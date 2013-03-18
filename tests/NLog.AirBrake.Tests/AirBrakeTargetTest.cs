using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FakeItEasy;
using SharpBrake.Serialization;
using SharpBrake;

namespace NLog.AirBrake.Tests
{
  public class AirBrakeTargetTest
  {
    private static Logger logger = LogManager.GetCurrentClassLogger(); 

    [Fact]
    public void LogEvent_WithExecption_CallsClient()
    {
      var client = A.Fake<ISharpbrakeClient>();
      Exception ex = new ApplicationException("something bad happened");

      // using activator to avoid the obsolete tag on the constructor.
      AirbrakeError error = Activator.CreateInstance<AirbrakeError>(); 
      AirbrakeNotice notice = new AirbrakeNotice() { Error = error };
      A.CallTo(() => client.BuildNotice(ex)).Returns(notice);

      AirBrakeTarget target = new AirBrakeTarget(client);
      NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target);

      logger.InfoException("kaboom", ex);
      A.CallTo(() => client.Send(notice)).MustHaveHappened();
    }

    [Fact]
    public void LogEvent_NoExecption_DoesNotCallClient()
    {
      var client = A.Fake<ISharpbrakeClient>();
      AirBrakeTarget target = new AirBrakeTarget(client);
      NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target);

      logger.Info("no exception with this one.");
      A.CallTo(() => client.Send(A<AirbrakeNotice>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public void AirBrakeTarget_InnerException_CreateBackTrace()
    {
        AirBrakeTarget target = new AirBrakeTarget();
        NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target);
        try
        {
            new InnerExceptionTestHelper().ThrowException();
        }
        catch(Exception ex)
        {
            List<AirbrakeTraceLine> traceLines = new List<AirbrakeTraceLine>();
            if (ex.InnerException != null)
            {
                traceLines = target.GetBackTraceLines(ex, 1);
            }
            Assert.True(traceLines.Count == 12);
        }
        
    }
  }
}
