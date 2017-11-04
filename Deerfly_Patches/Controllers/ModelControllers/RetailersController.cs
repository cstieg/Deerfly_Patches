using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using Cstieg.Telephony;
using Deerfly_Patches.Models;
using Deerfly_Patches.Modules.Google;
using Deerfly_Patches.ActionFilters;
using Cstieg.WebFiles.Controllers;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// The controller providing model scaffolding for Retailers
    /// </summary>
    [Authorize(Roles = "Administrator")]
    [ClearCache]
    public class RetailersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Retailers
        public async Task<ActionResult> Index()
        {
            var retailers = db.Retailers.Include(r => r.LatLng).OrderBy(r => r.Address.PostalCode);
            return View(await retailers.ToListAsync());
        }

        // GET: Retailers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retailer retailer = await db.Retailers.FindAsync(id);
            if (retailer == null)
            {
                return HttpNotFound();
            }
            return View(retailer);
        }

        // GET: Retailers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Retailers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RetailerId,Name,Address,Website")] Retailer retailer)
        {
            if (ModelState.IsValid)
            {
                // Geocode address of retailer
                retailer.LatLng = await new GoogleMapsClient().GeocodeAddress(retailer.Address);

                db.Retailers.Add(retailer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(retailer);
        }

        // GET: Retailers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retailer retailer = await db.Retailers.FindAsync(id);
            if (retailer == null)
            {
                return HttpNotFound();
            }

            return View(retailer);
        }

        // POST: Retailers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RetailerId,Name,LatLngId")] Retailer retailer)
        {
            if (ModelState.IsValid)
            {
                // Geocode address of retailer
                retailer.LatLng = await new GoogleMapsClient().GeocodeAddress(retailer.Address);

                db.Entry(retailer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(retailer);
        }

        // GET: Retailers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retailer retailer = await db.Retailers.FindAsync(id);
            if (retailer == null)
            {
                return HttpNotFound();
            }
            return View(retailer);
        }

        // POST: Retailers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Retailer retailer = await db.Retailers.FindAsync(id);
            db.Retailers.Remove(retailer);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Returns JSON list of retailers
        /// </summary>
        /// <returns>JSON list of retailers</returns>
        public async Task<JsonResult> ListJson()
        {
            //TODO: filter by location
            var retailers = db.Retailers.Include(r => r.LatLng);
            var returnval = await retailers.ToListAsync();
            return Json(returnval, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Renders form to upload retailers csv file
        /// </summary>
        [HttpGet]
        public ActionResult UploadCsv()
        {
            return View();
        }

        /// <summary>
        /// Uploads csv list of retailers
        /// </summary>
        /// <returns>Redirect to Index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostUploadCsv()
        {
            // allow longer time for processing
            int previousTimeout = HttpContext.Server.ScriptTimeout;
            HttpContext.Server.ScriptTimeout = 3600;

            // Delete current data if requested
            bool deleteCurrent = Request.Params.Get("deleteCurrent") == "on";
            if (deleteCurrent)
            {
                db.Retailers.RemoveRange(await db.Retailers.ToListAsync());
            }

            // List of errors
            List<string> ErrorList = new List<string>();
            HttpPostedFileBase file = _ModelControllersHelper.GetFile(ModelState, Request, "RetailersCsv");

            await UploadRetailersList(file, deleteCurrent, ErrorList);
            await GeocodeRetailers(ErrorList);

            if (ErrorList.Count > 0)
            {
                var writeStream = new MemoryStream();
                var textWriter = new StreamWriter(writeStream);

                for (int i = 0; i < ErrorList.Count; i++)
                {
                    string row = string.Join(", ", ErrorList[i]);
                    textWriter.WriteLine(row);
                }
                textWriter.Flush();
                writeStream.Position = 0;

                HttpContext.Server.ScriptTimeout = previousTimeout;
                return new FileStreamResult(writeStream, "application/text")
                {
                    FileDownloadName = "errors.txt"
                };
            }

            HttpContext.Server.ScriptTimeout = previousTimeout;
            return RedirectToAction("Index");
        }

        private async Task UploadRetailersList(HttpPostedFileBase file, bool deleteCurrent, List<string> ErrorList)
        {
            StreamReader textReader;
            CsvParser csvParser;
            string[] headerRow;
            string[] dataRow;

            // Load and parse CSV file
            try
            {
                textReader = new StreamReader(file.InputStream);
                csvParser = new CsvParser(textReader);
                headerRow = csvParser.Read();
            }
            catch
            {
                throw new Exception("Failure reading file!");
            }

            // Iterate through rows, adding retailers to table
            dataRow = csvParser.Read();
            while (dataRow != null)
            {
                await AddRetailer(dataRow, deleteCurrent, ErrorList);

                // read next row
                dataRow = csvParser.Read();
            }
        }

        private async Task AddRetailer(string[] dataRow, bool deleteCurrent, List<string> ErrorList)
        {
            try
            {
                Retailer retailer = new Retailer()
                {
                    Name = dataRow[0].Trim(),
                    Address = new Address()
                    {
                        Address1 = dataRow[1].Trim(),
                        City = dataRow[2].Trim(),
                        State = dataRow[3].Trim(),
                        PostalCode = dataRow[4].Trim(),
                        Phone = Phone.FormatDigits(dataRow[5].Trim())
                    },
                    Website = dataRow[6].Trim()
                };

                // skip if retailer is already in database
                if (!deleteCurrent)
                {
                    Retailer existingRetailer = await db.Retailers.Where(r => r.Address.Phone == retailer.Address.Phone).FirstOrDefaultAsync();
                    if (existingRetailer != null)
                    {
                        // if address is the same, skip
                        if (retailer.Address.AddressIsSame(existingRetailer.Address))
                        {
                            return;
                        }
                        // if address is different, replace
                        else
                        {
                            db.Retailers.Remove(existingRetailer);
                        }
                    }
                }

                // fix for empty string failing url validation
                if (retailer.Website.Equals(""))
                {
                    retailer.Website = null;
                }

                // add http:// if missing
                if (retailer.Website != null && retailer.Website.Length > 4 && retailer.Website.Substring(0, 4) != "http")
                {
                    retailer.Website = "http://" + retailer.Website;
                }

                db.Retailers.Add(retailer);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                ErrorList.Add(string.Join(",", dataRow));
            }

        }

        private async Task GeocodeRetailers(List<string> ErrorList)
        {
            var retailers = await db.Retailers.ToListAsync();
            foreach (var retailer in retailers)
            {
                if (retailer.LatLngId == null)
                {
                    try
                    {
                        retailer.LatLng = await new GoogleMapsClient().GeocodeAddress(retailer.Address);
                    }
                    catch (Exception e)
                    {
                        ErrorList.Add("Error: " + e.Message + "  Retailer: " + retailer.ToString());
                    }
                    db.Entry(retailer).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
