// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

var source = new TraceSource("Foobar", SourceLevels.All);
source.Listeners.Clear();
source.Listeners.Add(new DefaultTraceListener() { LogFileName = "trace.log" });

var eventTypes = (TraceEventType[])Enum.GetValues(typeof(TraceEventType));
var eventId = 1;
Array.ForEach(eventTypes, it => source.TraceEvent(it, eventId++, $"This is a {it} message."));
Console.WriteLine("Hello, World!");


//逻辑And运算符 & 两个进制位都位1，结果位1，否则为0
//逻辑异或运算符^ 两个进制位相同，则为0，否则为1
//逻辑或运算符 |  两个进制位都为0，则为0，否则为1

public class SourceSwitch : Switch {
    public SourceSwitch(string displayName, string? description) : base(displayName, description) {
    }

    protected override void OnValueChanged() {
        base.SwitchSetting = (int)Enum.Parse(typeof(SourceLevels),base.Value,true);
    }

    public bool ShouldTrace(TraceEventType eventType) {
       return (SwitchSetting & (int)eventType) > 0;
    }
}