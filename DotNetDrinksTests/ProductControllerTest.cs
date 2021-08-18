using DotNetDrinks.Controllers;
using DotNetDrinks.Data;
using DotNetDrinks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetDrinksTests
{
    [TestClass]
    class ProductControllerTest
    {
        // declare a db connection, mock list of products, and instance of ProductsController at the class level
        // arrange these objects globally for use in all the unit tests
        private ApplicationDbContext _context;
        List<Product> products = new List<Product>();
        ProductsController controller;

        [TestInitialize]  // runs automatically before every unit test in the arrange phase
        public void TestInitialize()
        {
            // set the options for & create an in-memory db instead of using SQL Server (which would be integration testing instead)
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // create some mock data & add it to the in-memory db
            var category = new Category { Id = 700, Name = "Category Seven Hundred" };
            _context.Categories.Add(category);
            _context.SaveChanges();

            products.Add(new Product { Id = 37, Name = "Prod Thirty-Seven", Price = 37, CategoryId = 700 });
            products.Add(new Product { Id = 64, Name = "Prod Sixty-Four", Price = 64, CategoryId = 700 });
            products.Add(new Product { Id = 56, Name = "Prod Fifty-Six", Price = 56, CategoryId = 700 });

            foreach (var p in products)
            {
                _context.Products.Add(p);
            }
            _context.SaveChanges();

            // now instantiate controller and pass the populated in-memory db to it
            controller = new ProductsController(_context);
        }
        #region Delete
        [TestMethod]
        public void DeleteIsNoIdReturnsError()
        {
            //act
            var result = (ViewResult)controller.Delete(null).Result;
            //assert
            Assert.AreEqual("404", result.ViewName);
        }
        [TestMethod]
        public void DeleteInvalidIdReturnsError()
        {
            //act- Passing id which is not present in the in-memory db
            var result = (ViewResult)controller.Delete(33).Result;
            //assert
            Assert.AreEqual("404", result.ViewName);
        }
        [TestMethod]
        public void DeleteValidIdLoadsDeleteView()
        {
            //act- Passing id which is not present in the in-memory db
            var result = (ViewResult)controller.Delete(37).Result;
            //assert
            Assert.AreEqual("Delete", result.ViewName);
        }
        [TestMethod]
        public void DeleteValidIdReturnsMatchingDeletedOffice()
        {
            //act- Passing id which is not present in the in-memory db
            var result = (ViewResult)controller.Delete(37).Result;
            var model = (Product)result.Model;
            //assert
            Assert.AreEqual(products[0], model);
        }

        #endregion Delete

        [TestMethod]
        public void PostDeleteValidProductRemoved()
        {
            var result = (ActionResult)controller.DeleteConfirmed(37).Result;

        }
    }
}
