using System;
using DiscordPlaysKTANE.Discord;
using DiscordPlaysKTANE.Game.Bombs;
using Svg;
using System.Drawing;
using System.Collections.Generic;
using System.Collections;
using DSharpPlus.Entities;

namespace DiscordPlaysKTANE.Game.Modules.Vanilla {
    public class TheButtonModule : BaseModule {
        public override string ModuleName => "The Button";

        public override string HelpMessage => "Press the button with `!{0} press` or `!{0} tap`. Hold the button with `!{0} hold`, release it with `!{0} release <num>`.";

        private static readonly string[] Labels = { "ABORT", "DETONATE", "HOLD", "PRESS" };
        private static readonly SvgColourServer[] Colors = {
            new SvgColourServer(Color.Red), // RED
            new SvgColourServer(Color.Yellow), // YELLOW
            new SvgColourServer(Color.Blue), // BLUE
            new SvgColourServer(Color.White)  // WHITE
        };
        private static readonly string[] ColorNames = {
            "RED", "YELLOW", "BLUE", "WHITE"
        };
        private static readonly string[] ReleaseNumbers = {
            "1", "5", "4", "1"
        };

        private int label;
        private int color;

        private bool solRequiresHold;
        private int solReleaseColor = -1;

        public override void Init() {
            label = RandomUtil.Int(4);
            color = RandomUtil.Int(4);
            Log($"Button: {ColorNames[color]} {Labels[label]}");

            solRequiresHold =
                color == 2 && label == 1 ? true :
                BombInfo.GetBatteryCount() > 1 && label == 1 ? false :
                color == 3 && BombInfo.IsIndicatorOn(Indicator.CAR) ? true :
                BombInfo.GetBatteryCount() > 2 && BombInfo.IsIndicatorOn(Indicator.FRK) ? false :
                color == 1 ? true :
                color == 0 && label == 2 ? false :
                true;

            Log($"Solution: " + (solRequiresHold ? "HOLD" : "PRESS"));
        }

        public override string ProcessDiscordMessage(string command, DiscordMessage msg) {
            string[] pieces = command.Trim().ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (pieces.Length >= 1) {
                switch (pieces[0]) {
                    case "press":
                    case "tap":
                        if (!solRequiresHold) {
                            Log("Button tapped corectly! Module defused.");
                            HandlePass();
                            return ResponseStrings.Solve;
                        } else {
                            Log("Button tapped incorectly! Strike!");
                            HandleStrike();
                            return ResponseStrings.Strike;
                        }
                    case "hold":
                        if (solReleaseColor == -1) {
                            solReleaseColor = RandomUtil.Int(4);
                            Log($"Button is being held. LED strip is {ColorNames[solReleaseColor]}. Release with timer contains a '{ReleaseNumbers[solReleaseColor]}'.");
                            return null;
                        } else {
                            Log("Error: attempted to hold alread-held button.");
                            return ResponseStrings.UnsubmittablePenalty;
                        }
                    case "release":
                        if (pieces.Length >= 2) {
                            Log($"Waiting to release button until timer contains a '{pieces[1]}'...");
                            GameManager.AddDelayedAction(WaitUntilTimeContains(pieces[1]), msg, this);
                            return ResponseStrings.Waiting;
                        } else {
                            if (solRequiresHold && BombInfo.GetFormattedTime().Contains(ReleaseNumbers[solReleaseColor])) {
                                Log("Button held and released corectly! Module defused.");
                                HandlePass();
                                solReleaseColor = -1;
                                return ResponseStrings.Solve;
                            } else {
                                if (!solRequiresHold) {
                                    Log("Button held when supposed to be tapped! Strike!");
                                } else {
                                    Log($"Button released at incorrect time ({BombInfo.GetFormattedTime()})! Strike!");
                                }
                                HandleStrike();
                                solReleaseColor = -1;
                                return ResponseStrings.Strike;
                            }
                        }
                }
            }
            return ResponseStrings.InvalidCommand;
        }

        private IEnumerable<string> WaitUntilTimeContains(string number) {
            while (true) {
                if (BombInfo.GetFormattedTime().Contains(number)) {
                    if (solRequiresHold && ReleaseNumbers[solReleaseColor] == number) {
                        Log("Button held and released corectly! Module defused.");
                        HandlePass();
                        yield return ResponseStrings.Solve;
                    } else {
                        if (!solRequiresHold) {
                            Log("Button held when supposed to be tapped! Strike!");
                        } else {
                            Log($"Button released at incorrect time ({BombInfo.GetFormattedTime()})! Strike!");
                        }
                        HandleStrike();
                        yield return ResponseStrings.Strike;
                    }
                    solReleaseColor = -1;
                    yield break;
                } else {
                    yield return null;
                }
            }
        }

        public override IEnumerable<object> GetImage() {
            SvgDocument workingCopy = (SvgDocument)Template.Clone();
            workingCopy.GetElementById<SvgCircle>("buttonbg").Fill = Colors[color];
            workingCopy.GetElementById<SvgText>("buttontext").Fill = color % 2 != 0 ? ImageUtil.COLOR_BLACK : ImageUtil.COLOR_WHITE;
            workingCopy.GetElementById<SvgText>("buttontext").Text = Labels[label];
            workingCopy.GetElementById<SvgRectangle>("strip").Fill = solReleaseColor != -1 ? Colors[solReleaseColor] : ImageUtil.COLOR_GRAY;
            workingCopy.GetElementById<SvgCircle>("statuslight").Fill = Solved ? ImageUtil.COLOR_SL_GREEN : ImageUtil.COLOR_SL_BLACK;
            yield return workingCopy;
            yield break;
        }
    }
}