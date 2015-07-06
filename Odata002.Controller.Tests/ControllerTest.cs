using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using biz.dfch.CS.Examples.Odata;

using biz.dfch.CS.Examples.Odata.Client;
using biz.dfch.CS.Examples.Odata.Client.Utilities;
using System.Net;
using System.Collections.Generic;

namespace biz.dfch.CS.Examples.Odata.Tests
{
    [TestClass]
    public class ControllerTest
    {
        protected TestServer server;
        protected HttpConfiguration httpConfiguration;
        protected IAppBuilder appBuilder;

        protected Uri uri = new Uri("http://localhost:51288/Utilities.svc/");

        [TestInitialize]
        public void TestInitialize()
        {
            server = TestServer.Create(app =>
            {
                httpConfiguration = new HttpConfiguration();
                biz.dfch.CS.Examples.Odata.WebApiConfig.Register(httpConfiguration);
                appBuilder = app.UseWebApi(httpConfiguration);
            });
        }
        
        [TestCleanup]
        public void TestCleanup()
        {
            if (null != server)
            {
                server.Dispose();
            }
        }

        [TestMethod]
        public void CreateThingReturnsHttp201()
        {
            // Arrange
            var container = new biz.dfch.CS.Examples.Odata.Client.Utilities.Container(uri);
            var name = "theThing";
            var entity = new Thing();
            entity.Name = name;
            entity.Description = new Decimal(1.0);

            // Act
            container.AddToThings(entity);
            container.UpdateObject(entity);
            var serviceResponse = container.SaveChanges();

            // Assert
            Assert.AreEqual(1, serviceResponse.Count());
            var r = serviceResponse.First();
            Assert.IsNotNull(r);
            Assert.AreEqual(201, r.StatusCode);
            Assert.IsNull(r.Error);

            Assert.IsNotNull(container.Things.Select(i => i.ID.Equals(entity.ID)));
        }

        [TestMethod]
        public void GetThingByNameReturnsThing()
        {
            // Arrange
            var container = new biz.dfch.CS.Examples.Odata.Client.Utilities.Container(uri);
            var name = "theThing";

            // Act
            var entity = container.Things.Where(e => e.Name.Equals(name)).First();

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(name, entity.Name);
        }

        [TestMethod]
        public void GetAllThingsReturnsList()
        {
            // Arrange
            var container = new biz.dfch.CS.Examples.Odata.Client.Utilities.Container(uri);

            // Act
            var entitySet = container.Things.ToList();

            // Assert
            Assert.IsNotNull(entitySet);
            // see https://devaspirin.wordpress.com/2013/04/29/the-assert-isinstanceoftype-headache/
            // for usage on Assert.IsInstanceOfType()
            Assert.IsInstanceOfType(entitySet.GetType(), typeof(List<Thing>).GetType());
            foreach(var entity in entitySet)
            {
                Assert.IsNotNull(entity);
            }
        }

        [TestMethod]
        public void UpdateThingReturnsHttp204()
        {
            // Arrange
            var container = new biz.dfch.CS.Examples.Odata.Client.Utilities.Container(uri);
            var name = "theThing";
            var nameNew = "theOtherThing";
            var entity = container.Things.Where(e => e.Name.Equals(name)).First();

            // Act
            entity.Name = nameNew;
            container.UpdateObject(entity);
            var serviceResponse = container.SaveChanges();

            // Assert
            Assert.AreEqual(1, serviceResponse.Count());
            var r = serviceResponse.First();
            Assert.IsNotNull(r);
            Assert.AreEqual(204, r.StatusCode);
            Assert.IsNull(r.Error);

            var entityNew = container.Things.Where(i => i.Name.Equals(nameNew)).Single();
            Assert.IsNotNull(entityNew);
            Assert.AreEqual(nameNew, entityNew.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Data.Services.Client.DataServiceQueryException))]
        public void DeleteThingReturnsHttp204()
        {
            // Arrange
            var container = new biz.dfch.CS.Examples.Odata.Client.Utilities.Container(uri);
            var name = "theOtherThing";
            var entity = container.Things.Where(e => e.Name.Equals(name)).First();

            // Act
            container.DeleteObject(entity);
            var serviceResponse = container.SaveChanges();

            // Assert
            Assert.AreEqual(1, serviceResponse.Count());
            var r = serviceResponse.First();
            Assert.IsNotNull(r);
            Assert.AreEqual(204, r.StatusCode);
            Assert.IsNull(r.Error);

            container.Things.Where(i => i.ID.Equals(entity.ID)).Single();
        }
    }
}
