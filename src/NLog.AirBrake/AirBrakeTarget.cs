using System;
using System.Collections.Generic;
using System.Text;
using NLog.Targets;
using SharpBrake;
using SharpBrake.Serialization;

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
      :this(new SharpbrakeClient(new AirbrakeClient(), new AirbrakeNoticeBuilder()))
    {
    }

    /// <summary>
    /// Used to override the client for unit testing purposes
    /// </summary>
    /// <param name="client"></param>
    public AirBrakeTarget(ISharpbrakeClient client)
    {
      this.SharpbrakeClient = client;
    }

    private ISharpbrakeClient SharpbrakeClient { get; set; }

    /// <summary>
    /// Writes logging event to the log target.
    /// </summary>
    /// <param name="logEvent">Logging event to be written out.</param>
    protected override void Write(LogEventInfo logEvent)
    {
      if (logEvent.Exception != null)
      {
        AirbrakeNotice notice = this.SharpbrakeClient.BuildNotice(logEvent.Exception);

        // Override the notice message so we have the full exception
        // message, including the messages of the inner exceptions.
        // Also, include the log message, if it is set.
        string exceptionMessage = BuildExceptionMessage(logEvent.Exception);
        notice.Error.Message = !string.IsNullOrEmpty(logEvent.Message) ? logEvent.Message + " " + exceptionMessage : exceptionMessage;

        this.SharpbrakeClient.Send(notice);
      }
    }

    private string BuildExceptionMessage(Exception ex)
    {
      if (ex == null)
      {
        return string.Empty;
      }

      List<string> messages = new List<string>();
      while (ex != null)
      {
        messages.Add(ex.GetType().Name + ": " + ex.Message);
        ex = ex.InnerException;
      }

      return string.Join(" --> ", messages.ToArray());
    }

  }
}
