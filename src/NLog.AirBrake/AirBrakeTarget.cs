using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
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
    private int maxException = 20;

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

      string maxExceptionConfig = ConfigurationManager.AppSettings["Airbrake.MaxExceptions"];
      if(!string.IsNullOrEmpty(maxExceptionConfig))
          maxException = int.Parse(maxExceptionConfig);
    }

    private ISharpbrakeClient SharpbrakeClient { get; set; }

    /// <summary>
    /// Writes logging event to the log target.
    /// </summary>
    /// <param name="logEvent">Logging event to be written out.</param>
    protected override void Write(LogEventInfo logEvent)
    {
        var notice = (logEvent.Exception != null) ? SharpbrakeClient.BuildNotice(logEvent.Exception) : SharpbrakeClient.BuildNotice(logEvent.ToAirBrakeError());

        if (logEvent.Exception != null && logEvent.Exception.InnerException != null)
            notice.Error.Backtrace = GetBackTraceLines(logEvent.Exception.InnerException, 1).ToArray();
        // Override the notice message so we have the full exception
        // message, including the messages of the inner exceptions.
        // Also, include the log message, if it is set.
        string exceptionMessage = BuildExceptionMessage(logEvent.Exception);
        notice.Error.Message = !string.IsNullOrEmpty(logEvent.FormattedMessage) ? logEvent.FormattedMessage + " " + exceptionMessage : exceptionMessage;

        this.SharpbrakeClient.Send(notice);
    }

    public List<AirbrakeTraceLine> GetBackTraceLines(Exception ex, int exceptionCount)
    {
        List<AirbrakeTraceLine> lines = new List<AirbrakeTraceLine>();
        var stackTrace = new StackTrace(ex);
        StackFrame[] frames = stackTrace.GetFrames();

        foreach (StackFrame frame in frames)
        {
            MethodBase method = frame.GetMethod();

            int lineNumber = frame.GetFileLineNumber();
            lines.Add(new AirbrakeTraceLine("---- INNER EXCEPTION ----", 0));

            if (lineNumber == 0)
            {
                lineNumber = frame.GetILOffset();
            }

            if (lineNumber == -1)
            {
                lineNumber = frame.GetNativeOffset();
            }

            // AirBrake doesn't allow for negative line numbers which can happen with lambdas
            if (lineNumber < 0)
            {
                lineNumber = 0;
            }

            string file = frame.GetFileName();

            if (String.IsNullOrEmpty(file))
            {
                file = method.ReflectedType != null
                  ? method.ReflectedType.FullName
                  : "(unknown)";
            }

            AirbrakeTraceLine line = new AirbrakeTraceLine(file, lineNumber)
            {
                Method = method.Name
            };

            lines.Add(line);
        }
        exceptionCount++;
        if (ex.InnerException != null && exceptionCount <= maxException)
            lines.AddRange(GetBackTraceLines(ex.InnerException, exceptionCount));
        return lines;
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
