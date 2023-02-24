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
        public void GetCurrentAvailableSites_ReturnsCorrectNumberOfSites()
        {
            //ARRANGE
            SiteSqlDao dao = new SiteSqlDao(ConnectionString);

            //ACT
            int resultCount = (dao.GetCurrentAvailableSites(ParkId)).Count;

            //ASSERT
            Assert.AreEqual(2, resultCount);
        }

    }
}
