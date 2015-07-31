# biz.dfch.CS.Examples.Odata
[![License](https://img.shields.io/badge/license-Apache%20License%202.0-blue.svg)](https://github.com/dfensgmbh/biz.dfch.CS.Examples.Odata/blob/master/LICENSE)

Unit Testing OData Controllers with Service References and Katana TestServer

## Introduction

This solution shows how to test OData Controllers by utilising the [Katana TestServer](https://katanaproject.codeplex.com/). This means the unit tests (or maybe integration tests) use the integrated Katana web server to load the `WebApiConfig` and perform acutal HTTP requests and responses. This example is further extended by using an OData Service Reference to abstract the actual HTTP request/response logic. Let's show you an example:

``` csharp
[TestInitialize]
public void TestInitialize()
{
	server = TestServer.Create(app =>
	{
		HttpConfiguration config = new HttpConfiguration();
		biz.dfch.CS.Examples.Odata.WebApiConfig.Register(config);
		app.UseWebApi(config);
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
```

The above code is used to actually instantiate an HTTP server context. After that, all you have to do is to do something with the controller, such as retrieving a `Thing`:

``` csharp
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
```

The `biz.dfch.CS.Examples.Odata.Client` is a service reference to the actual OData Controllers' service route (encapsulated in `Utilities.Container`). There you get all the benefits of an ORM mapper that abstracts away the deserialisation of the OData models. Of course you can then go on and `Mock` the underlying `DBContext` in case you do not really want to write to the database.

## Solution/Project description

* Odata002
	the OData controllers under test

* Odata002.Client
	the service reference, that encapsulates the OData controllers

* Odata002.Tests
	_regular_ unit tests; here you can also use the service reference client if needed

* Odata002.Controller.Tests
	This is the project that uses 

## Issues

* There is currently an issue that I cannot automatically detect the port of the server instance (see #1); but instead have to hard code it. A workaround might be to parse the service refrence file. But I have not implemented that.

* Using HTTP mehtods such as `PATCH` and `BATCH` is only implicitly possible by setting the respective parameters on the container. The same holds true for media formatting (`JSON` vs `XML`).

* Currently only tested and working with OData v3 (not OData v4).

* No authentication (has not been tested yet).

## Resources

The examples build upon the deescription of these articles:

* [Unit Testing Web API OData Services – Part 1](http://www.rainman-63.com/?p=833)
* [Unit Testing Web API OData Services – Part 2](http://www.rainman-63.com/?p=931)
* [Unit Testing Web API OData Services – Part 3](http://www.rainman-63.com/?p=1001)
* [OWIN Self-Hosted Test Server for Integration Testing of OData and Web API](http://www.cameronjtinker.com/post/2014/10/10/OWIN-Self-Hosted-Test-Server-for-Integration-Testing-of-OData-and-Web-API.aspx)
