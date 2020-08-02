using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.Failures {

    public enum FailureType {
        UnexpectedShutdown = 0,
        ShortNonResponding = 1,
        HardwareFailures = 2,
        ConnectionProblems = 3
    }

    public readonly struct Failure {
        public readonly DateTime FailureDate;
        public readonly FailureType FailureType;

        public Failure(DateTime d, FailureType t) {
            FailureDate = d;
            FailureType = t;
        }
    }

    public readonly struct Device {
        public readonly int Id;
        public readonly string Name;

        public Device(int id, string name) {
            Id = id;
            Name = name;
        }
    }

    public class Common {
        public static bool IsFailureSerious(FailureType failureType) {
            return failureType == FailureType.UnexpectedShutdown ||
                   failureType == FailureType.HardwareFailures;
        }


        public static bool Earlier(DateTime l, DateTime r) {
            return l < r;
        }
    }

    public class ReportMaker {
        /// <summary>
        /// </summary>
        /// <param name="day"></param>
        /// <param name="failureTypes">
        /// 0 for unexpected shutdown, 
        /// 1 for short non-responding, 
        /// 2 for hardware failures, 
        /// 3 for connection problems
        /// </param>
        /// <param name="deviceId"></param>
        /// <param name="times"></param>
        /// <param name="devices"></param>
        /// <returns></returns>
        public static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes,
            int[] deviceId,
            object[][] times,
            List<Dictionary<string, object>> devices
        ) {
            var border = new DateTime(year, month, day);
            var failures = new List<Failure>(failureTypes.Length);
            var devicesList = new List<Device>(failureTypes.Length);

            for (int i = 0; i < failureTypes.Length; ++i) {
                failures.Add(
                    new Failure(
                        new DateTime((int) times[i][2], (int) times[i][1], (int) times[i][0]), 
                        (FailureType) failureTypes[i]
                    )
                );
                devicesList.Add(
                    new Device(
                        deviceId[i],
                        devices[i]["Name"] as string
                    )
                );
            }

            return FindDevicesFailedBeforeDate(border, failures, devicesList);
        }

        public static List<string> FindDevicesFailedBeforeDate(
            DateTime border,
            List<Failure> failures,
            List<Device> devices
        ) {
            List<string> result = new List<string>(devices.Count);
            int index = 0;
            foreach (var failure in failures) {
                if (
                    Common.IsFailureSerious(failure.FailureType) &&
                    Common.Earlier(failure.FailureDate, border)
                )
                    result.Add(devices[index].Name);
                ++index;
            }

            return result;
        }
    }
}