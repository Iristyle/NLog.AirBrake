using SharpBrake;
using SharpBrake.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace NLog.AirBrake
{
  public class SharpbrakeClient : ISharpbrakeClient
  {
    public SharpbrakeClient()
    {
      this.Client = new AirbrakeClient();
      this.Builder = new AirbrakeNoticeBuilder();
    }
    
    public SharpbrakeClient(AirbrakeClient client, AirbrakeNoticeBuilder builder)
    {
      this.Client = client;
      this.Builder = builder;
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

    public AirbrakeNotice BuildNotice(AirbrakeError error)
    {
        return this.Builder.Notice(error);
    }
  }
}
