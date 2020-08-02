using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace Memory.Timers {
    public static class Timer {
        public class TimerHandler : IDisposable {
            private readonly Stopwatch _stopwatch = new Stopwatch();
            private bool _disposed;
            private readonly string _name;
            private readonly int _depth;

            public TimerHandler(string name, int depth) {
                _stopwatch.Start();
                _name = name;
                _depth = depth;
                ++_currentDepth;

                if (depth == _depthToEntry.Count)
                    _depthToEntry.Add(new DepthEntry {Count = 1, Time = 0});
                else
                    ++_depthToEntry[depth].Count;
            }

            private void WriteReport() {
                var myStr = "";
                for (int i = 0; i < _depth; ++i) {
                    myStr += "    ";
                }

                myStr += _name;
                myStr += new string(' ', 20 - myStr.Length);
                myStr += ": " + _stopwatch.ElapsedMilliseconds + '\n';

                if (_currentDepth == _depthToEntry.Count)
                    Report += myStr;
                else
                    Report = myStr + Report;

                _depthToEntry[_depth].Time += _stopwatch.ElapsedMilliseconds;

                if (_currentDepth < _depthToEntry.Count) {
                    myStr = "";
                    for (int i = 0; i < _currentDepth; ++i) {
                        myStr += "    ";
                    }

                    myStr += "Rest";
                    myStr += new string(' ', 20 - myStr.Length);
                    myStr += ": " + (_stopwatch.ElapsedMilliseconds - _depthToEntry[_currentDepth].Time) + '\n';
                    Report += myStr;
                }

                --_depthToEntry[_depth].Count;

                if (_depth == 0 && _depthToEntry[_depth].Count == 0)
                    _needToResetStaticData = true;
            }

            public void Dispose() {
                if (!_disposed) {
                    _stopwatch.Stop();
                    WriteReport();
                    --_currentDepth;
                    _disposed = true;
                }
            }

            ~TimerHandler() {
                Dispose();
            }
        }

        private static int _currentDepth;

        private class DepthEntry {
            public int Count;
            public double Time;
        }

        private static List<DepthEntry> _depthToEntry = new List<DepthEntry>();

        private static bool _needToResetStaticData = true;

        public static TimerHandler Start(string name = "*") {
            if (_needToResetStaticData) {
                Report = "";
                _depthToEntry = new List<DepthEntry>();
                _needToResetStaticData = false;
            }

            return new TimerHandler(name, _currentDepth);
        }

        public static string Report { get; private set; }
    }
}