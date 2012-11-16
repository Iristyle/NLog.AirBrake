using SharpBrake;
using SharpBrake.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace NLog.AirBrake
{
  public class SharpbrakeClient : ISharpbrakeClient
  {
    public SharpbrakeClient(AirbrakeClient client, AirbrakeNoticeBuilder builder)
    {
      this.Client = new AirbrakeClient();
      this.Builder = new AirbrakeNoticeBuilder();
    }

    private AirbrakeClient Client { get; set; }
    private AirbrakeNoticeBuilder Builder { get; set; }

    public void Send(AirbrakeNotice notice)
    {
      this.Client.Send(notice);
    }

    public AirbrakeNotice BuildNotice(Exception ex)
    {
      return this.Builder.Notice(ex);
    }
  }
}
