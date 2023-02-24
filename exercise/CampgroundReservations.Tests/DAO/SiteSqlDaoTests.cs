using CampgroundReservations.DAO;
using CampgroundReservations.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CampgroundReservations.Tests.DAO
{
    [TestClass]
    public class SiteSqlDaoTests : BaseDaoTests
    {
        [TestMethod]
        public void GetSitesThatAllowRVs_Should_ReturnSites()
        {
            // Arrange
            SiteSqlDao dao = new SiteSqlDao(ConnectionString);

            // Act
            IList<Site> sites = dao.GetSitesThatAllowRVs(ParkId);

            // Assert
            Assert.AreEqual(2, sites.Count);
        }


        [TestMethod]
        //I think we misunderstood the point of the function. Our function gets sites available tonight, which would be 1-5 and 10 and 11.
        public void GetCurrentAvailableSites_ReturnsCorrectNumberOfSites()
        {
            //ARRANGE
            SiteSqlDao dao = new SiteSqlDao(ConnectionString);

            //ACT
            IList<int> list = dao.GetCurrentAvailableSites(ParkId);
            int resultCount = (list).Count;

            //ASSERT
            Assert.AreEqual(7, resultCount);
        }

    }
}
