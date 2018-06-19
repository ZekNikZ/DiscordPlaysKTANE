using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using DiscordPlaysKTANE.Game.Bombs;

namespace DiscordPlaysKTANE.Game.Widgets {
    public abstract class Widget : Object {
        public abstract string GetResult(string key, string data);
    }

    public class PortWidget : Widget {
        List<string> ports;

        public PortWidget() {
            ports = new List<string>();
            string portList = "";

            if (RandomUtil.Double() > 0.5) {
                if (RandomUtil.Double() > 0.5) {
                    ports.Add("Parallel");
                    portList += "Parallel";
                }
                if (RandomUtil.Double() > 0.5) {
                    ports.Add("Serial");
                    if (portList.Length > 0) portList += ", ";
                    portList += "Serial";
                }
            } else {
                if (RandomUtil.Double() > 0.5) {
                    ports.Add("DVI");
                    portList += "DVI";
                }
                if (RandomUtil.Double() > 0.5) {
                    ports.Add("PS2");
                    if (portList.Length > 0) portList += ", ";
                    portList += "PS2";
                }
                if (RandomUtil.Double() > 0.5) {
                    ports.Add("RJ45");
                    if (portList.Length > 0) portList += ", ";
                    portList += "RJ45";
                }
                if (RandomUtil.Double() > 0.5) {
                    ports.Add("StereoRCA");
                    if (portList.Length > 0) portList += ", ";
                    portList += "StereoRCA";
                }
            }

            if (portList.Length == 0) portList = "Empty plate";
            Debug.Log("Added port widget: " + portList);
        }

        public override string GetResult(string key, string data) {
            if (key == BombInfo.QUERYKEY_GET_PORTS) {
                return JsonConvert.SerializeObject((object)new Dictionary<string, List<string>>()
                {
                    {
                        "presentPorts", ports
                    }
                });
            }
            return null;
        }
    }

    public class IndicatorWidget : Widget {
        static List<string> possibleValues = new List<string>(){
            "SND","CLR","CAR",
            "IND","FRQ","SIG",
            "NSA","MSA","TRN",
            "BOB","FRK"
        };

        private string val;
        private bool on;

        public IndicatorWidget() {
            int pos = RandomUtil.Int(0, possibleValues.Count);
            val = possibleValues[pos];
            possibleValues.RemoveAt(pos);
            on = RandomUtil.Double() > 0.4f;

            Debug.Log("Added indicator widget: " + val + " is " + (on ? "ON" : "OFF"));
        }

        public override string GetResult(string key, string data) {
            if (key == BombInfo.QUERYKEY_GET_INDICATOR) {
                return JsonConvert.SerializeObject((object)new Dictionary<string, string>()
                {
                    {
                        "label", val
                    },
                    {
                        "on", on?bool.TrueString:bool.FalseString
                    }
                });
            } else return null;
        }
    }

    public class BatteryWidget : Widget {
        private int batt;

        public BatteryWidget() {
            batt = RandomUtil.Int(1, 3);

            Debug.Log("Added battery widget: " + batt);
        }

        public override string GetResult(string key, string data) {
            if (key == BombInfo.QUERYKEY_GET_BATTERIES) {
                return JsonConvert.SerializeObject((object)new Dictionary<string, int>()
                {
                    {
                        "numbatteries", batt
                    }
                });
            } else return null;
        }
    }
}
