using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLog.AirBrake
{
  /// <summary>
  /// Simple interface is to
  /// </summary>
  public interface IAirbrakeClient
  {
    void Send(Exception ex);
  }
}
