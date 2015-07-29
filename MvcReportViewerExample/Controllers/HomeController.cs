﻿using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Data;
using MvcReportViewer.Example.Models;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;

namespace MvcReportViewer.Example.Controllers
{
    public class HomeController : Controller
    {
        private const string RemoteReportName = "/TestReports/TestReport";
        private const string LocalReportName = "App_Data/Reports/Products.rdlc";
        private const string LocalNoDataReportName = "App_Data/Reports/NoDataReport.rdlc";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Fluent()
        {
            return View();
        }

        public ActionResult Post()
        {
            return View();
        }

        public ActionResult Multiple()
        {
            return View();
        }

        public ActionResult VisibilityCheck()
        {
            return View();
        }

        public ActionResult NoDataLocalReport()
        {
            return View();
        }

        public ActionResult DownloadPdfLocalNoData()
        {
            return this.Report(
                ReportFormat.Pdf,
                LocalNoDataReportName,
                new { TodayDate = DateTime.Now },
                ProcessingMode.Local);
        }

        public ActionResult DownloadExcel()
        {
            return DownloadReport(ReportFormat.Excel, true, "Report.xls");
        }

        public ActionResult DownloadWord()
        {
            return DownloadReportMultipleValues(ReportFormat.Word, "Report.doc");
        }

        public ActionResult DownloadPdf()
        {
            return DownloadReport(ReportFormat.Pdf, false);
        }

        private ActionResult DownloadReport(ReportFormat format, bool isLocalReport, string filename = null)
        {
            if (isLocalReport)
            {
                return this.Report(
                    format,
                    LocalReportName,
                    new { Parameter1 = "Test", Parameter2 = 123 },
                    ProcessingMode.Local,
                    new Dictionary<string, DataTable>
                    {
                        { "Products", GetProducts() },
                        { "Cities", GetCities() }
                    },
                    filename);
            }

            return this.Report(
                format,
                RemoteReportName,
                new { Parameter1 = "Hello World!", Parameter2 = DateTime.Now, Parameter3 = 12345 },
                filename: filename);
        }

        private ActionResult DownloadReportMultipleValues(ReportFormat format, string filename = null)
        {
            return this.Report(
                format,
                RemoteReportName,
                new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("Parameter1", "Value 1"),
                    new KeyValuePair<string, object>("Parameter1", "Value 2"),
                    new KeyValuePair<string, object>("Parameter2", DateTime.Now),
                    new KeyValuePair<string, object>("Parameter2", DateTime.Now.AddYears(10)),
                    new KeyValuePair<string, object>("Parameter3", 12345)
                },
                filename: filename);
        }

        public ActionResult LocalReports()
        {
            var model = new SqlLocalReportsModel
            {
                Products = "select * from dbo.Products",
                Cities = "select * from dbo.Cities",
                FilteredCities = "select * from dbo.Cities where Id < 3"
            };

            return View(model);
        }

        #region Helper Methods for SessionLocalDataSourceProvider examples

        private static DataTable GetProducts()
        {
            return GetDataTable("select * from dbo.Products");
        }

        private static DataTable GetCities()
        {
            return GetDataTable("select * from dbo.Cities");
        }

        private static DataTable GetDataTable(string sql)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Products"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }

        #endregion
    }
}
