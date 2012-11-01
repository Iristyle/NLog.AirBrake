using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using FakeItEasy;

namespace NLog.AirBrake.Tests
{
  public class AirBrakeTargetTest
  {
    private static Logger logger = LogManager.GetCurrentClassLogger(); 

    [Fact]
    public void LogEvent_WithExecption_CallsClient()
    {
      var client = A.Fake<IAirbrakeClient>();
      AirBrakeTarget target = new AirBrakeTarget(client);
      NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target);
      Exception ex = new ApplicationException("something bad happened");

      logger.InfoException("kaboom", ex);
      A.CallTo(() => client.Send(ex)).MustHaveHappened();
    }

    [Fact]
    public void LogEvent_NoExecption_DoesNotCallClient()
    {
      var client = A.Fake<IAirbrakeClient>();
      AirBrakeTarget target = new AirBrakeTarget(client);
      NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target);
      Exception ex = null;

      logger.Info("no exception with this one.");
      A.CallTo(() => client.Send(ex)).MustNotHaveHappened();
    }
  }
}
