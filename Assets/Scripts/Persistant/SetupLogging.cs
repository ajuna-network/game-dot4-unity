using NLog;
using NLog.Config;
using NLog.Targets;
using UnityEngine;
using Logger = NLog.Logger;

[Target("Unity")]
public class UnityLogger : TargetWithLayout
{
   public UnityLogger(string name) : base()
   {
      Name = name;
   }
   protected override void Write(LogEventInfo logEvent)
   {
      if (logEvent.Level >= LogLevel.Error)
      {
         Debug.LogError(RenderLogEvent(Layout, logEvent));
      }
      else if (logEvent.Level >= LogLevel.Warn)
      {
         Debug.LogWarning(RenderLogEvent(Layout, logEvent));
      }
      else
      {
         Debug.Log(RenderLogEvent(Layout, logEvent));
      }
   }
}

public class SetupLogging : MonoBehaviour
{
   private readonly Logger _logger = LogManager.GetCurrentClassLogger();
   // Start is called before the first frame update
   void Awake()
   {
      var config = new LoggingConfiguration();
      var target = new UnityLogger("Unity");
      config.AddTarget(target);
      config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, target));
      LogManager.Configuration = config;
      _logger.Trace("logmanager config updated.");
      _logger.Warn("example warning");
      _logger.Error("example error");
   }

   // Update is called once per frame
   void Update()
   {

   }
}
