using System;
using System.Collections.Generic;
using System.Drawing;
using DSharpPlus.Entities;
using Svg;
using DiscordPlaysKTANE.Game.Bombs;
using System.Linq;
using System.Text.RegularExpressions;
using DiscordPlaysKTANE.Discord;
using System.Threading.Tasks;

namespace DiscordPlaysKTANE.Game.Modules.Vanilla {
    public class SimonSaysModule : BaseModule {
        public override string ModuleName => "Simon Says";

        public override string HelpMessage => "Press a color with `!{0} press <color>`. Press multiple colors with `!{0} press <color1> <color2>`.";

        private int stages;
        private int currentStage = 1;
        private int[] flashingColors;
        private int[] correctColors;
        private bool hasVowel;
        private int currentColor = 0;

        public override void Init() {
            stages = RandomUtil.Int(3, 6);
            flashingColors = new int[stages];
            correctColors = new int[stages];
            for (int i = 0; i < stages; i++) {
                flashingColors[i] = RandomUtil.Int(4);
            }
            hasVowel = BombInfo.GetSerialNumberLetters().Any(x => "AEIOU".Contains(x));

            CalculateCorrectColors();

            PrepImages(this);
        }

        public override string ProcessDiscordMessage(string command, DiscordMessage msg) {
            string[] pieces = command.Trim().ToLowerInvariant().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (pieces[0] == "press") {
                if (pieces.Length >= 2) {
                    List<int> presses = new List<int>();
                    if (Regex.IsMatch(pieces[1], @"^[ rgby]+$")) {
                        foreach (char piece in pieces[1]) {
                            if (piece == ' ') {
                                continue;
                            } else if ("rgby".Contains(piece)) {
                                presses.Add("rbgy".IndexOf(piece));
                            } else {
                                return ResponseStrings.InvalidArguments;
                            }
                        }
                    } else {
                        foreach (string piece in pieces[1].Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries)) {
                            var index = piece.FindUniqueIn(ColorNames);
                            if (index >= 0 && index < 4) {
                                presses.Add(index);
                            } else {
                                return ResponseStrings.InvalidArguments;
                            }
                        }
                    }
                    if (currentColor == 0) {
                        CalculateCorrectColors();
                    }
                    for (int i = 0; i < presses.Count; i++) {
                        if (presses[i] == correctColors[currentColor]) {
                            currentColor++;
                            if (currentColor >= currentStage) {
                                currentStage++;
                                currentColor = 0;
                                if (currentStage > stages) {
                                    HandlePass();
                                    return ResponseStrings.Solve;
                                }
                            }
                        } else {
                            currentColor = 0;
                            HandleStrike();
                            return ResponseStrings.Strike;
                        }
                    }
                    return null;
                } else {
                    return ResponseStrings.MissingArguments;
                }
            } else {
                return ResponseStrings.InvalidCommand;
            }
        }

        private static readonly string[] ColorNames = {
            "red", "blue", "green", "yellow"
        };
        private static readonly int[,,] ColorTables = {
            {
                {1, 0, 3, 2},
                {3, 2, 1, 0},
                {2, 0, 3, 1}
            },
            {
                {1, 3, 2, 0},
                {0, 1, 3, 2},
                {3, 2, 1, 0}
            }
        };

        private void CalculateCorrectColors() {
            for (int i = 0; i < stages; i++) {
                correctColors[i] = ColorTables[hasVowel ? 0 : 1, BombInfo.GetStrikes() >= 2 ? 2 : BombInfo.GetStrikes(), flashingColors[i]];
            }
        }

        private static readonly SvgColourServer[] LIGHT_COLORS = {
            new SvgColourServer(Color.Red),
            new SvgColourServer(Color.FromArgb(255, 0, 0, 255)),
            new SvgColourServer(Color.FromArgb(255, 0, 255, 0)),
            new SvgColourServer(Color.Yellow)
        };

        private static Bitmap[] ImageCache;

        public override IEnumerable<object> GetImage() {
            yield return Tuple.Create(Template, 5);
            for (int i = 0; i < currentStage; i++) {
                yield return ImageCache[flashingColors[i]];
            }
            yield break;
        }

        public static bool prepped = false;
        public static void PrepImages(SimonSaysModule module) {
            if (!prepped) {
                ImageCache = new Bitmap[4];
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < 4; i++) {
                    var j = i;
                    tasks.Add(Task.Run(delegate {
                        var temp = (SvgDocument)module.Template.DeepCopy<SvgDocument>();
                        temp.GetElementById<SvgRectangle>(ColorNames[j]).Fill = LIGHT_COLORS[j];
                        ImageCache[j] = temp.Draw();
                    }));
                }
                Task.WaitAll(tasks.ToArray());
                prepped = true;
            }
        }
    }
}
