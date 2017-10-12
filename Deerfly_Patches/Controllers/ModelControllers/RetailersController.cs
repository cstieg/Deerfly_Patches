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
using Deerfly_Patches.Models;
using Deerfly_Patches.Modules.Google;
using System.Net.Mime;

namespace Deerfly_Patches.Controllers
{
    /// <summary>
    /// The controller providing model scaffolding for Retailers
    /// </summary>
    [Authorize(Roles = "Administrator")]
    public class RetailersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Retailers
        public ActionResult Index()
        {
            var retailers = db.Retailers.Include(r => r.LatLng).OrderBy(r => r.Address.Zip);
            return View(retailers.ToList());
        }

        // GET: Retailers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retailer retailer = db.Retailers.Find(id);
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
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(retailer);
        }

        // GET: Retailers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retailer retailer = db.Retailers.Find(id);
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
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(retailer);
        }

        // GET: Retailers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Retailer retailer = db.Retailers.Find(id);
            if (retailer == null)
            {
                return HttpNotFound();
            }
            return View(retailer);
        }

        // POST: Retailers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Retailer retailer = db.Retailers.Find(id);
            db.Retailers.Remove(retailer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Returns JSON list of retailers
        /// </summary>
        /// <returns>JSON list of retailers</returns>
        public JsonResult ListJson()
        {
            //TODO: filter by location
            var retailers = db.Retailers.Include(r => r.LatLng);
            var returnval = retailers.ToList();
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
            // Delete current data if requested
            string deleteCurrent = Request.Params.Get("deleteCurrent");
            if (Request.Params.Get("deleteCurrent") == "on")
            {
                db.Retailers.RemoveRange(db.Retailers.ToList());
            }

            // List of errors
            List<string[]> ErrorList = new List<string[]>();
            HttpPostedFileBase file;
            StreamReader textReader;
            CsvParser csvParser;
            string[] headerRow;
            string[] dataRow;

            // Load and parse CSV file
            try
            {
                file = _ModelControllersHelper.GetFile(ModelState, Request, "RetailersCsv");
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
                            Zip = dataRow[4].Trim(),
                            Phone = dataRow[5].Trim()
                        },
                        Website = dataRow[6].Trim()
                    };

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

                    retailer.LatLng = await new GoogleMapsClient().GeocodeAddress(retailer.Address);

                    db.Retailers.Add(retailer);
                    db.SaveChanges();
                }
                catch
                {
                    ErrorList.Add(dataRow);
                }
                break;
                // read next row
                dataRow = csvParser.Read();
            }

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
               
                return new FileStreamResult(writeStream, "application/text")
                {
                    FileDownloadName = "errors.txt"
                };
            }
            
            return RedirectToAction("Index");
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
