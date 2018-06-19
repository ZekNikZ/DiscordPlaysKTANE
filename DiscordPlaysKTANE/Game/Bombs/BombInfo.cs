using System;
using System.Collections.Generic;
using DiscordPlaysKTANE.Game.Widgets;
using DiscordPlaysKTANE.Game.Modules;
using Newtonsoft.Json;

namespace DiscordPlaysKTANE.Game.Bombs {
    public class BombInfo {
        public Widget[] widgets;

        public const int numStrikes = 3;

        public bool solved;
        public int strikes = 0;
        public string serial;

        public void Init(int numWidgets = 5) {
            StartTime = DateTime.Now;

            widgets = new Widget[numWidgets];
            for (int a = 0; a < numWidgets; a++) {
                int r = RandomUtil.Int(0, 3);
                if (r == 0) widgets[a] = new PortWidget();
                else if (r == 1) widgets[a] = new IndicatorWidget();
                else widgets[a] = new BatteryWidget();
            }

            char[] possibleCharArray =
            {
            'A','B','C','D','E',
            'F','G','H','I','J',
            'K','L','M','N','E',
            'P','Q','R','S','T',
            'U','V','W','X','Z',
            '0','1','2','3','4',
            '5','6','7','8','9'
            };
            string str1 = string.Empty;
            for (int index = 0; index < 2; ++index) str1 = str1 + possibleCharArray[RandomUtil.Int(0, possibleCharArray.Length)];
            string str2 = str1 + (object)RandomUtil.Int(0, 10);
            for (int index = 3; index < 5; ++index) str2 = str2 + possibleCharArray[RandomUtil.Int(0, possibleCharArray.Length - 10)];
            serial = str2 + RandomUtil.Int(0, 10);

            Debug.Log("Serial: " + serial);
        }

        public DateTime StartTime;
        public float _totalTime = 600f;

        public float Time {
            get {
                return (_timeLeft = _totalTime - (float)(DateTime.Now - StartTime).TotalSeconds);
            }
            set {
                _timeLeft = value;
            }
        }
        public float _timeLeft = 600f;

        public float GetTime() {
            return Time;
        }

        public float GetStartTime() {
            return _totalTime;
        }

        public string GetFormattedTime() {
            string time = "";
            if (Time < 60) {
                if (_timeLeft < 10) time += "0";
                time += (int)_timeLeft;
                time += ".";
                int s = (int)(_timeLeft * 100);
                if (s < 10) time += "0";
                time += s;
            } else {
                if (_timeLeft < 600) time += "0";
                time += (int)_timeLeft / 60;
                time += ":";
                int s = (int)_timeLeft % 60;
                if (s < 10) time += "0";
                time += s;
            }
            return time;
        }

        public string GetFormattedStartTime() {
            string time = "";
            if (Time < 60) {
                if (_totalTime < 10) time += "0";
                time += (int)_totalTime;
                time += ".";
                int s = (int)(_totalTime * 100);
                if (s < 10) time += "0";
                time += s;
            } else {
                if (_totalTime < 600) time += "0";
                time += (int)_totalTime / 60;
                time += ":";
                int s = (int)_totalTime % 60;
                if (s < 10) time += "0";
                time += s;
            }
            return time;
        }

        public int GetStrikes() {
            return strikes;
        }

        public List<KeyValuePair<BaseModule, bool>> modules = new List<KeyValuePair<BaseModule, bool>>();
        //TODO: public List<KMNeedyModule> needyModules = new List<KMNeedyModule>();

        public List<string> GetModuleNames() {
            List<string> moduleList = new List<string>();
            foreach (KeyValuePair<BaseModule, bool> m in modules) {
                moduleList.Add(m.Key.ModuleName);
            }
            //TODO:
            /*foreach (KMNeedyModule m in needyModules) {
                moduleList.Add(m.ModuleDisplayName);
            }*/
            return moduleList;
        }

        public List<string> GetSolvableModuleNames() {
            List<string> moduleList = new List<string>();
            foreach (KeyValuePair<BaseModule, bool> m in modules) {
                moduleList.Add(m.Key.ModuleName);
            }
            return moduleList;
        }

        public List<string> GetSolvedModuleNames() {
            List<string> moduleList = new List<string>();
            foreach (KeyValuePair<BaseModule, bool> m in modules) {
                if (m.Key.Solved) moduleList.Add(m.Key.ModuleName);
            }
            return moduleList;
        }

        public const string QUERYKEY_GET_SERIAL_NUMBER = "serial";
        public const string QUERYKEY_GET_PORTS = "ports";
        public const string QUERYKEY_GET_INDICATOR = "indicator";
        public const string QUERYKEY_GET_BATTERIES = "batteries";

        public List<string> GetWidgetQueryResponses(string queryKey, string queryInfo) {
            List<string> responses = new List<string>();
            if (queryKey == BombInfo.QUERYKEY_GET_SERIAL_NUMBER) {
                responses.Add(JsonConvert.SerializeObject((object)new Dictionary<string, string>()
                {
                {
                    "serial", serial
                }
            }));
            }
            foreach (Widget w in widgets) {
                string r = w.GetResult(queryKey, queryInfo);
                if (r != null) responses.Add(r);
            }
            if (queryKey == "Unity")
                responses.Add(JsonConvert.SerializeObject(new Dictionary<string, bool>() { { "Unity", true } }));
            return responses;
        }

        public bool IsBombPresent() {
            return true;
        }

        public void HandleStrike() {
            strikes++;
            Debug.Log(strikes + "/" + numStrikes);
            if (strikes == numStrikes) {
                if (Detonate != null) Detonate();
                Debug.Log("KABOOM!");
                GameManager.Instance.Detonate();
            }
        }

        public delegate void OnDetonate();
        public OnDetonate Detonate;

        public void HandleStrike(string reason) {
            Debug.Log("Strike: " + reason);
            HandleStrike();
        }

        public delegate void OnSolved();
        public OnSolved HandleSolved;

        public void Solved() {
            solved = true;
            if (HandleSolved != null) HandleSolved();
            Debug.Log("Bomb defused!");
        }
    }
}
