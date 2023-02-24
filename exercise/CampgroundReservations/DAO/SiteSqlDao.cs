using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
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
