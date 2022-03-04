using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Moq;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SoundBlockBlockTests {
        [TestMethod]
        public void IsSpeakerOn() {
            string script = @"print ""Enabled: "" + ""test speaker"" is on";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.Enabled).Returns(true);

                test.RunOnce();

                Assert.AreEqual("Enabled: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TurnOnTheSpeaker() {
            string script = @"turn on the ""test speaker""";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);


                test.RunOnce();

                mockSpeaker.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void IsSpeakerPowered() {
            string script = @"print ""Enabled: "" + ""test speaker"" is powered";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.Enabled).Returns(true);

                test.RunOnce();

                Assert.AreEqual("Enabled: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PowerOnTheSpeaker() {
            string script = @"power on the ""test speaker""";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);


                test.RunOnce();

                mockSpeaker.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void GetSpeakerVolume() {
            string script = @"print ""Volume: "" + ""test speaker"" volume";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.Volume).Returns(0.8f);

                test.RunOnce();

                Assert.AreEqual("Volume: 0.8", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetSpeakerVolume() {
            string script = @"set the ""test speaker"" volume to 0.7";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);


                test.RunOnce();

                mockSpeaker.VerifySet(b => b.Volume = 0.7f);
            }
        }

        [TestMethod]
        public void GetSpeakerRange() {
            string script = @"print ""Range: "" + ""test speaker"" range";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.Range).Returns(100);

                test.RunOnce();

                Assert.AreEqual("Range: 100", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetSpeakerRange() {
            string script = @"set the ""test speaker"" range to 200";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                test.RunOnce();

                mockSpeaker.VerifySet(b => b.Range = 200);
            }
        }

        [TestMethod]
        public void GetSpeakerRadius() {
            string script = @"print ""Radius: "" + ""test speaker"" radius";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.Range).Returns(100);

                test.RunOnce();

                Assert.AreEqual("Radius: 100", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetSpeakerRadius() {
            string script = @"set the ""test speaker"" radius to 200";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                test.RunOnce();

                mockSpeaker.VerifySet(b => b.Range = 200);
            }
        }

        [TestMethod]
        public void GetSpeakerPeriod() {
            string script = @"print ""Period: "" + ""test speaker"" period";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.LoopPeriod).Returns(60);

                test.RunOnce();

                Assert.AreEqual("Period: 60", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetSpeakerPeriod() {
            string script = @"set the ""test speaker"" period to 120";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);


                test.RunOnce();

                mockSpeaker.VerifySet(b => b.LoopPeriod = 120);
            }
        }

        [TestMethod]
        public void IsSpeakerPlaying() {
            string script = @"print ""Playing: "" + ""test speaker"" is playing";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.DetailedInfo).Returns("Loop timer: 4 sec");

                test.RunOnce();

                Assert.AreEqual("Playing: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsSpeakerPlayingWhenNotPlaying() {
            string script = @"print ""Playing: "" + ""test speaker"" is playing";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.DetailedInfo).Returns("");

                test.RunOnce();

                Assert.AreEqual("Playing: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PlayTheSpeaker() {
            string script = @"play the ""test speaker""";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.CustomData).Returns("");

                test.RunOnce();

                mockSpeaker.Verify(b => b.Play());
            }
        }

        [TestMethod]
        public void TellSpeakerToStopPlaying() {
            string script = @"tell the ""test speaker"" to stop playing";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.CustomData).Returns("");

                test.RunOnce();

                mockSpeaker.Verify(b => b.Stop());
            }
        }

        [TestMethod]
        public void SilenceSpeaker() {
            string script = @"silence the ""test speaker""";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.CustomData).Returns("");

                test.RunOnce();

                mockSpeaker.Verify(b => b.Stop());
            }
        }

        [TestMethod]
        public void IsSpeakerPlayingSound() {
            string script = @"print ""Playing: "" + ""test speaker"" is playing ""My Song""";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.SelectedSound).Returns("My Song");

                test.RunOnce();

                Assert.AreEqual("Playing: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsSpeakerPlayingSoundWhenNotPlayingIt() {
            string script = @"print ""Playing: "" + ""test speaker"" is playing ""My Song""";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.SelectedSound).Returns("Other Song");

                test.RunOnce();

                Assert.AreEqual("Playing: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetSpeakerSound() {
            string script = @"print ""Current Sound: "" + ""test speaker"" sound";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.SelectedSound).Returns("My Song");

                test.RunOnce();

                Assert.AreEqual("Current Sound: My Song", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TellSpeakerToPlaySong() {
            string script = @"tell ""test speaker"" to play ""My Sound""";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                mockSpeaker.Setup(b => b.CustomData).Returns("");

                test.RunOnce();

                mockSpeaker.VerifySet(b => b.SelectedSound = "My Sound");
            }
        }

        [TestMethod]
        public void SetSpeakerSound() {
            string script = @"set the ""test speaker"" sound to ""My Sound""";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);

                test.RunOnce();

                mockSpeaker.VerifySet(b => b.SelectedSound = "My Sound");
            }
        }

        [TestMethod]
        public void GetAvailableSpeakerSounds() {
            string script = @"print ""Available Sounds: "" +  ""test speaker"" sounds";
            using (var test = new ScriptTest(script)) {
                var mockSpeaker = new Mock<IMySoundBlock>();
                test.MockBlocksOfType("test speaker", mockSpeaker);
                List<string> availableSounds = new List<string>();
                List<string> expectedList = new List<string>();
                mockSpeaker.Setup(b => b.GetSounds(expectedList)).Callback<List<string>>(t => {
                    t.Add("Sound1");
                    t.Add("Sound2");
                });

                test.RunOnce();

                Assert.AreEqual("Available Sounds: [Sound1,Sound2]", test.Logger[0]);
            }
        }
    }
}
