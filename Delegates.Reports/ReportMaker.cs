using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Delegates.Reports {
    public struct Statistics<T> {
        public string ValueType { get; set; }
        public T Entry { get; set; }
    }

    public static class Report<TStats, TReportMaker, TStatsMaker>
        where TReportMaker : ReportMaker<TStats>
        where TStatsMaker : IStatsMaker<TStats>, new() {
        public static string MakeReport(IEnumerable<Measurement> data) {
            var statsMaker = new TStatsMaker();
            var reportMaker = (TReportMaker) Activator.CreateInstance(typeof(TReportMaker), statsMaker.Caption);

            var statsList = new List<Statistics<TStats>> {
                statsMaker.MakeStats("Temperature", data.Select(m => m.Temperature)),
                statsMaker.MakeStats("Humidity", data.Select(m => m.Humidity))
            };

            return reportMaker.MakeReport(statsList);
        }
    }

    public abstract class ReportMaker<TStats> {
        protected readonly string Caption;

        protected ReportMaker(string caption) {
            Caption = caption;
        }

        public abstract string MakeReport(IEnumerable<Statistics<TStats>> stats);
    }

    public class HtmlReportMaker<TStats> : ReportMaker<TStats> {
        public HtmlReportMaker(string caption) : base(caption) { }

        public override string MakeReport(IEnumerable<Statistics<TStats>> stats) {
            var result = new StringBuilder();
            result.Append($"<h1>{Caption}</h1>");
            result.Append("<ul>");
            foreach (var stat in stats) {
                result.Append($"<li><b>{stat.ValueType}</b>: {stat.Entry}");
            }

            result.Append("</ul>");
            return result.ToString();
        }
    }

    public class MarkdownReportMaker<TStats> : ReportMaker<TStats> {
        public MarkdownReportMaker(string caption) : base(caption) { }

        public override string MakeReport(IEnumerable<Statistics<TStats>> stats) {
            var result = new StringBuilder();
            result.Append($"## {Caption}\n\n");
            foreach (var stat in stats) {
                result.Append($" * **{stat.ValueType}**: {stat.Entry}\n\n");
            }

            return result.ToString();
        }
    }

    public interface IStatsMaker<TStats> {
        string Caption { get; }
        Statistics<TStats> MakeStats(string valueType, IEnumerable<double> data);
    }

    public class MeanAndStdMaker : IStatsMaker<MeanAndStd> {
        public string Caption => "Mean and Std";

        public Statistics<MeanAndStd> MakeStats(string valueType, IEnumerable<double> data) {
            var mean = data.Average();
            var entry = new MeanAndStd {
                Mean = mean,
                Std = Math.Sqrt(data.Select(z => Math.Pow(z - mean, 2)).Sum() / (data.Count() - 1))
            };
            return new Statistics<MeanAndStd> {
                ValueType = valueType,
                Entry = entry
            };
        }
    }

    public class MedianMaker : IStatsMaker<double> {
        public string Caption => "Median";

        public Statistics<double> MakeStats(string valueType, IEnumerable<double> data) {
            var list = data.OrderBy(z => z).ToList();
            double entry;
            if (list.Count % 2 == 0)
                entry = (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
            else
                entry = list[list.Count / 2];
            return new Statistics<double> {
                ValueType = valueType,
                Entry = entry
            };
        }
    }

    public static class ReportMakerHelper {
        public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data) {
            return Report<MeanAndStd, HtmlReportMaker<MeanAndStd>, MeanAndStdMaker>.MakeReport(data);
        }

        public static string MedianMarkdownReport(IEnumerable<Measurement> data) {
            return Report<double, MarkdownReportMaker<double>, MedianMaker>.MakeReport(data);
        }

        public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> data) {
            return Report<MeanAndStd, MarkdownReportMaker<MeanAndStd>, MeanAndStdMaker>.MakeReport(data);
        }

        public static string MedianHtmlReport(IEnumerable<Measurement> data) {
            return Report<double, HtmlReportMaker<double>, MedianMaker>.MakeReport(data);
        }
    }
}