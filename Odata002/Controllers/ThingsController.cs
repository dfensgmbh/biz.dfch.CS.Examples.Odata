using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using biz.dfch.CS.Examples.Odata.Models;

namespace biz.dfch.CS.Examples.Odata.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using biz.dfch.CS.Examples.Odata.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Thing>("Things");
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ThingsController : ODataController
    {
        private readonly ServiceContext db = new ServiceContext();

        // GET: odata/Things
        [Queryable]
        public IQueryable<Thing> GetThings()
        {
            return db.Things.AsQueryable();
        }

        // GET: odata/Things(5)
        [Queryable]
        public SingleResult<Thing> GetThing([FromODataUri] int key)
        {
            return SingleResult.Create(db.Things.Where(thing => thing.ID == key));
        }

        // PUT: odata/Things(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Thing thing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != thing.ID)
            {
                return BadRequest();
            }

            db.Entry(thing).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThingExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(thing);
        }

        // POST: odata/Things
        public async Task<IHttpActionResult> Post(Thing thing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Things.Add(thing);
            await db.SaveChangesAsync();

            return Created(thing);
        }

        // PATCH: odata/Things(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Thing> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var thing = await db.Things.FindAsync(key);
            if (null == thing)
            {
                return NotFound();
            }

            patch.Patch(thing);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThingExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(thing);
        }

        // DELETE: odata/Things(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var thing = await db.Things.FindAsync(key);
            if (null == thing)
            {
                return NotFound();
            }

            db.Things.Remove(thing);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ThingExists(int key)
        {
            return db.Things.Count(e => e.ID == key) > 0;
        }
    }
}
