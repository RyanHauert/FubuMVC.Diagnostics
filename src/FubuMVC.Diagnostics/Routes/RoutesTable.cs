using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.Diagnostics.Routes
{
    public class RoutesTable : TableTag
    {
        public RoutesTable(IEnumerable<RouteReport> reports)
        {
            AddClass("table");

            AddHeaderRow(row => {
                row.Header("Details");
                row.Header("Route");
                row.Header("Contraints");
                row.Header("Action(s)");
            });

            reports.Each(report => {
                AddRow(report);
            });
        }

        public void AddRow(RouteReport report)
        {
            AddBodyRow(row => {
                row.Cell().Add("a").Text("Details").Attr("href", report.DetailsUrl);
                row.Cell(report.Route);
                row.Cell(report.Constraints);
                row.Cell(report.Action.Join(", "));

                row.Data("summary-url", report.SummaryUrl);
            });
        }
    }
}