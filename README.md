# Eshop API
Case study for a .NET developer position at Alza.cz consisting of a simple REST API over a products database.

**Main features**:

- SQLite database (`Microsoft.EntityFrameworkCore.SQLite`)
- Versioning (`Asp.Versioning.Http`, `Asp.Versioning.Mvc.ApiExplorer`)
- Offset pagination
- In-memory asynchronous queue for update processing
- Swagger documentation (`Swashbuckle.AspNetCore`)
- Unit tests (`xUnit`)

## Running the project

First you need to meet some **prerequisites**:

- Installed *.NET 8 SDK* ([Download .NET (Linux, macOS, and Windows) | .NET](https://dotnet.microsoft.com/en-us/download))
- Installed *Visual Studio 2022* with *ASP.NET and web development* workload ([Visual Studio 2022 IDE - AI for coding debugging and testing](https://visualstudio.microsoft.com/vs/))

Now you are ready to **run the project**:

- Clone or download (and extract) this repository.
- Open the `EshopAPI.sln` file (in `EshopApi/`) in Visual Studio 2022.
- Press F5 (or Ctrl+F5) to run it.

A new browser window should be opened, displaying Swagger documentation of the API. You can then use either the documentation or type URLs directly into the browser to interact with the application.

To **run unit tests** in Visual Studio, select *Test* in the menu, then *Test Explorer* and finally click on the green arrow in the top-left corner of the Test Explorer panel.

## Remarks

I am well aware there is a lot of room for improvements and my selected methods may have shortcomings. However I feel that some decisions may need a little explanation.

**Why Minimal API?**

I have decided to use Minimal API (instead of Controllers), because I've just started to learn and this seems like a better starting point. However this decision makes sense for many other reasons. The application is very simple, there are only a few endpoints, and on top of that it is possible to introduce some more code structure even with Minimal API. Also, in the job description, microservices are mentioned. Minimal API would be quite suitable for them too.

**How to handle stock update?**

There could be several ways to design the API for stock update. I have used a `/quantity` subresource to better indicate that it is just a small part of the whole product. And because of that I have chosen the PUT method, because we are essentially replacing the whole part we are referencing. On the other hand, I have also decided to support an arbitrary update of the whole product via the PATCH method.

**Why URL versioning?**

This versioning is very explicit, it is immediately visible. Also, the changes introduced in the second version are quite significant.

**Why not GUID?**

Instead of GUID, I have decided to use 32-bit integer for unique product identifiers. Mostly because it is easier to test, it is just a small demo without the potential to overflow with products, and it does not indicate anything secret which should not be easily locatable at random. However I am aware of its shortcomings.

**Why in-memory queue?**

I know there are many disadvantages, especially not storing it persistently. However it was explicitly mentioned in the task description as a valid possibility, so I have decided to use it, because it is simpler. If I had more time, I would look into RabbitMQ or Kafka.

**Why 0-indexed offset pagination?**

Offset-based pagination is not very performant and not very suitable for applications with large amount of data. I know there are better ways to do it. However I have rather decided on something simpler, which I can implement in the given timeframe. And I started counting from 0 instead of 1, just because it makes more sense (programmatically) and calculations are easier that way. I would however start from 1 if it was something for end-users and not developers to interact with.

### Assumptions

There were some loose ends in the task description. Therefore I have taken it upon myself to come up with a resolution. Based on that, I have made the following decisions:

- Duplicates among products are not detected. If we try to add a product, which is already there, it will be created again with a new ID.
- "Available products" may indicate either all products available in the shop catalogue, or all products available in stock. Therefore I have decided to support both possibilities and add an optional parameter to get only products currently in stock.
- Product stock update could be absolute (providing a new value directly), or relative (providing only a relative change from the current value). I have decided to support the relative one, because it makes sense (in real life, we don't want to calculate everything) and in this way race conditions cannot occur (e.g. two clients saw the same value at the same time and decided to update it based on that).
- There are potentially two options how to handle negative values (price, quantity). Either ignore them and clamp them to zero, or just refuse to execute the request straightaway. I have decided to return `BadRequest` in this case, because then the client is notified and the model is still in a consistent state.
