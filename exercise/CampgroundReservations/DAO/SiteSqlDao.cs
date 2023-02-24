using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using CampgroundReservations.Models;

namespace CampgroundReservations.DAO
{
    public class SiteSqlDao : ISiteDao
    {
        private readonly string connectionString;

        public SiteSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public IList<Site> GetSitesThatAllowRVs(int parkId)
        {
            IList<Site> list = new List<Site>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM site JOIN campground ON campground.campground_id = site.campground_id WHERE park_id = @park_id AND site.max_rv_length > 0", conn);
                    cmd.Parameters.AddWithValue("@park_id", parkId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Site site = GetSiteFromReader(reader);
                        list.Add(site);
                    }
                }
            }
            catch
            {
                return list;
            }
            return list;
        } 


        public IList<Site> GetCurrentAvailableSites(int parkId)
        {
            IList<Site> list = new List<Site>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("" +
                        "SELECT * FROM site " +
                        "JOIN campground ON campground.campground_id = site.campground_id " +
                        "JOIN reservation ON reservation.site_id = site.site_id " +
                        "WHERE park_id = @park_id AND " +
                            "from_date > GETDATE() OR " +
                            "to_date < GETDATE() ;", conn);




//                    SELECT site.site_id, site.site_number, site.max_occupancy, site.accessible, site.utilities FROM site
//JOIN campground ON campground.campground_id = site.campground_id
//JOIN reservation ON reservation.site_id = site.site_id
//WHERE park_id = 1 AND
//from_date > GETDATE() OR
//to_date < GETDATE()
//GROUP BY site.site_id 

                    cmd.Parameters.AddWithValue("@park_id", parkId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Site site = GetSiteFromReader(reader);
                        list.Add(site);
                    }
                }

            }
            catch
            {
                return list;
            }
            return list;
        }

        private Site GetSiteFromReader(SqlDataReader reader)
        {
            Site site = new Site();
            site.SiteId = Convert.ToInt32(reader["site_id"]);
            site.CampgroundId = Convert.ToInt32(reader["campground_id"]);
            site.SiteNumber = Convert.ToInt32(reader["site_number"]);
            site.MaxOccupancy = Convert.ToInt32(reader["max_occupancy"]);
            site.Accessible = Convert.ToBoolean(reader["accessible"]);
            site.MaxRVLength = Convert.ToInt32(reader["max_rv_length"]);
            site.Utilities = Convert.ToBoolean(reader["utilities"]);

            return site;
        }
    }
}
