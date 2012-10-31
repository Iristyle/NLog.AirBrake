using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Targets;
using SharpBrake;

namespace NLog.AirBrake
{
  /// <summary>
  /// This class is an NLog target class that will log messages
  /// to AirBrake or any compatible solution, such as Errbit.
  /// </summary>
  [Target("AirBrake")]
  public class AirBrakeTarget : TargetWithLayout
  {
    /// <summary>
    /// Creates an instance of the AirBrakeTarget class.
    /// </summary>
    public AirBrakeTarget()
      :this(new AirbrakeClient())
    {
    }

    /// <summary>
    /// Used to override the client for unit testing purposes
    /// </summary>
    /// <param name="client"></param>
    public AirBrakeTarget(IAirbrakeClient client)
    {
      this.Client = client;
    }

    private IAirbrakeClient Client { get; set; }

    /// <summary>
    /// Writes logging event to the log target.
    /// </summary>
    /// <param name="logEvent">Logging event to be written out.</param>
    protected override void Write(LogEventInfo logEvent)
    {
      if (logEvent.Exception != null)
      {
        // TODO: can we send more information than just the exception?

        // This grabs the configuartion from the config file. We could also 
        // provide properties on the target to accept configuration information.
        this.Client.Send(logEvent.Exception);
      }
    }
  }
}
