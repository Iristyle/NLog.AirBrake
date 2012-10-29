using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Targets;

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
    {
    }

    /// <summary>
    /// Gets or sets the host value. This property is set in the nlog config file.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Writes logging event to the log target.
    /// </summary>
    /// <param name="logEvent">Logging event to be written out.</param>
    protected override void Write(LogEventInfo logEvent)
    {
      string logMessage = this.Layout.Render(logEvent);

      // TODO: log message to AirBrake

      base.Write(logEvent);
    }
  }
}
