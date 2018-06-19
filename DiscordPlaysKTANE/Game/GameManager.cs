using System;
using System.Threading;
using DiscordPlaysKTANE.Game.Bombs;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using DiscordPlaysKTANE.Discord.Commands;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using DiscordPlaysKTANE.Game.Modules;
using DiscordPlaysKTANE.Discord;

namespace DiscordPlaysKTANE.Game {
    public class GameManager {
        public Random Random;

        public static GameManager Instance => _game ?? (_game = new GameManager());
        private static GameManager _game;

        public GameManager(int seed = 1) {
            Random = new Random(seed);
        }

        public static void StartThread() {
            Debug.Log("[Game] Game thread started.");
            DelayedActionsAsync().Wait();
        }

        public static List<DiscordMessage> _actionMessages = new List<DiscordMessage>();
        public static List<IEnumerator<string>> _delayedActions = new List<IEnumerator<string>>();
        public static List<string> _actionLasts = new List<string>();
        public static List<BaseModule> _actionModules = new List<BaseModule>();
        private static Queue<int> _toRemove = new Queue<int>();

        private static async Task DelayedActionsAsync() {
            while (true) {
                if (_toRemove.Count > 0) {
                    int offset = 0;
                    while (_toRemove.Count > 0) {
                        var index = _toRemove.Dequeue();
                        _delayedActions.RemoveAt(index - offset);
                        _actionMessages.RemoveAt(index - offset);
                        _actionLasts.RemoveAt(index - offset);
                        _actionModules.RemoveAt(index - offset);
                        offset++;
                    }
                }
                for (int i = 0; i < _delayedActions.Count; i++) {
                    if (_delayedActions[i].MoveNext()) {
                        _actionLasts[i] = _delayedActions[i].Current;
                        continue;
                    } else {
                        _toRemove.Enqueue(i);
                        await ModuleCommandHandler.HandleResponse(_actionMessages[i], _actionLasts[i], _actionModules[i]);
                    }
                }
                await Task.Delay(1000);
            }
        }

        public static void AddDelayedAction(IEnumerable<string> action, DiscordMessage msg, BaseModule module) {
            _actionMessages.Add(msg);
            _actionLasts.Add(null);
            _actionModules.Add(module);
            _delayedActions.Add(action.GetEnumerator());
        }

        public Bomb CurrentBomb;
        public bool BombInProgress;

        public bool NewBomb(int modules) {
            if (BombInProgress) {
                return false;
            } else {
                CurrentBomb = BombGenerator.Generate(modules);
                Debug.LogFormat("[Game] Starting new bomb with {0} modules...", modules);
                BombInProgress = true;
                return true;
            }
        }

        public void OnSolve() {
            CurrentBomb = null;
            Debug.LogFormat("[Game] Bomb defused!");
            BombInProgress = false;
            Bot.Instance.Broadcast(ResponsesTemplates.BombDefused);
        }

        public bool Detonate() {
            if (!BombInProgress) {
                return false;
            } else {
                CurrentBomb = null;
                Debug.LogFormat("[Game] Detonating the bomb...");
                BombInProgress = false;
                Bot.Instance.Broadcast(ResponsesTemplates.BombExploded);
                return true;
            }
        }

        public string GetEdgeworkString() {
            if (!BombInProgress) {
                return "Bomb not in progress";
            } else {
                BombInfo bombInfo = CurrentBomb.BombInfo;
                return "{0}b {1}h // {2} {3} // {4} // {5}".FormatThis(
                    bombInfo.GetBatteryCount(),
                    bombInfo.GetBatteryHolderCount(),
                    String.Join(" ", bombInfo.GetOnIndicators().Select(x => "*" + x)),
                    String.Join(" ", bombInfo.GetOffIndicators()),
                    String.Join(" ", bombInfo.GetPortPlates().Select(x => "[" + String.Join(", ", x) + "]")),
                    bombInfo.GetSerialNumber()
                );
            }
        }
    }
}