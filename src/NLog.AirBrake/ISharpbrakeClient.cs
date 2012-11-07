using SharpBrake.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLog.AirBrake
{
  /// <summary>
  /// An interface to abstract the Sharpbrake interaction to make this project
  /// easier to test.
  /// </summary>
  public interface ISharpbrakeClient
  {
    void Send(AirbrakeNotice notice);
    AirbrakeNotice BuildNotice(Exception ex);
  }
}
