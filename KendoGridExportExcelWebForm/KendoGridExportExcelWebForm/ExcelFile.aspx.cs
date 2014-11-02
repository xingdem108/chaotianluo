using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using KendoGridExportExcel.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KendoGridExportExcelWebForm
{
    public partial class ExcelFile : System.Web.UI.Page
    {
        // http://stackoverflow.com/questions/975455/is-there-an-equivalent-to-javascript-parseint-in-c
        private static readonly Regex LeadingInteger = new Regex(@"^(-?\d+)");

        protected void Page_Load(object sender, EventArgs e)
        {
            string title = Request.QueryString["title"];

            // Is there a spreadsheet stored in session?
            if (Session[title] != null)
            {
                // Get the spreadsheet from seession.
                byte[] file = Session[title] as byte[];
                string filename = string.Format("{0}.xlsx", title);

                // Remove the spreadsheet from session.
                Session.Remove(title);

                // Return the spreadsheet.
                Response.Clear();
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", filename));
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.BinaryWrite(file);
                Response.End();
            }
            else
            {
                throw new Exception(string.Format("{0} not found", title));
            }
        }

        /// <summary>
        /// Create the Excel spreadsheet.
        /// </summary>
        /// <param name="model">Definition of the columns for the spreadsheet.</param>
        /// <param name="data">Grid data.</param>
        /// <param name="title">Title of the spreadsheet.</param>
        /// <returns></returns>
        [System.Web.Services.WebMethod]
        public static object ExportToExcel(string model, string data, string title)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                /* Create the worksheet. */

                SpreadsheetDocument spreadsheet = Excel.CreateWorkbook(stream);
                Excel.AddBasicStyles(spreadsheet);
                Excel.AddAdditionalStyles(spreadsheet);
                Excel.AddWorksheet(spreadsheet, title);
                Worksheet worksheet = spreadsheet.WorkbookPart.WorksheetParts.First().Worksheet;


                /* Get the information needed for the worksheet */

                var modelObject = JsonConvert.DeserializeObject<dynamic>(model);
                var dataObject = JsonConvert.DeserializeObject<dynamic>(data);


                /* Add the column titles to the worksheet. */

                // For each column...
                for (int mdx = 0; mdx < modelObject.Count; mdx++)
                {
                    // If the column has a title, use it.  Otherwise, use the field name.
                    Excel.SetColumnHeadingValue(spreadsheet, worksheet, Convert.ToUInt32(mdx + 1),
                        (modelObject[mdx].title == null || modelObject[mdx].title == "&nbsp;")
                            ? modelObject[mdx].field.ToString()
                            : modelObject[mdx].title.ToString(),
                        false, false);

                    // Is there are column width defined?
                    Excel.SetColumnWidth(worksheet, mdx + 1, modelObject[mdx].width != null
                        ? int.Parse(LeadingInteger.Match(modelObject[mdx].width.ToString()).Value) / 4
                        : 25);
                }


                /* Add the data to the worksheet. */

                // For each row of data...
                for (int idx = 0; idx < dataObject.Count; idx++)
                {
                    // For each column...
                    for (int mdx = 0; mdx < modelObject.Count; mdx++)
                    {
                        // Set the field value in the spreadsheet for the current row and column.
                        Excel.SetCellValue(spreadsheet, worksheet, Convert.ToUInt32(mdx + 1), Convert.ToUInt32(idx + 2),
                            dataObject[idx][modelObject[mdx].field.ToString()].ToString(),
                            false, false);
                    }
                }


                /* Save the worksheet and store it in Session using the spreadsheet title. */

                worksheet.Save();
                spreadsheet.Close();
                byte[] file = stream.ToArray();
                HttpContext.Current.Session[title] = file;
            }

            return new { success = true };
        }
    }
}