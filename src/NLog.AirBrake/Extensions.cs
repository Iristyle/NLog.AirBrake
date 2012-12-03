using System;
using System.Collections.Generic;
using System.Text;
using SharpBrake.Serialization;
using System.Diagnostics;

namespace System.Runtime.CompilerServices
    //This bit of code lets our project compile under .NET 2.0
{
   class ExtensionAttribute : Attribute
   {

   }
}

namespace NLog.AirBrake
{

    public static class ExtensionHolder
    {
        public static AirbrakeError ToAirBrakeError(this LogEventInfo logEvent)
        {
            var error = Activator.CreateInstance<AirbrakeError>();
            if (logEvent.HasStackTrace)
            {
                StackFrame[] frames = logEvent.StackTrace.GetFrames();

                if (frames == null || frames.Length == 0)
                {
                    // Airbrake requires that at least one line is present in the XML.
                    error.Backtrace = new AirbrakeTraceLine[1] { new AirbrakeTraceLine("none", 0) };
                }

                List<AirbrakeTraceLine> lines = new List<AirbrakeTraceLine>();
                foreach (StackFrame frame in frames)
                {
                    var method = frame.GetMethod();

                    int lineNumber = frame.GetFileLineNumber();

                    if (lineNumber == 0)
                    {
                        //this.log.Debug(f => f("No line number found in {0}, using IL offset instead.", method));
                        lineNumber = frame.GetILOffset();
                    }

                    string file = frame.GetFileName();

                    if (String.IsNullOrEmpty(file))
                    {
                        // ReSharper disable ConditionIsAlwaysTrueOrFalse
                        file = method.ReflectedType != null
                                   ? method.ReflectedType.FullName
                                   : "(unknown)";
                        // ReSharper restore ConditionIsAlwaysTrueOrFalse
                    }

                    AirbrakeTraceLine line = new AirbrakeTraceLine(file, lineNumber)
                    {
                        Method = method.Name
                    };

                    lines.Add(line);
                }

                error.Backtrace = lines.ToArray();
            }
            error.Class = logEvent.LoggerName;
            return error;

        }
    }
}


