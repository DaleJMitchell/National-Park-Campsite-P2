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


        public IList<int> GetCurrentAvailableSites(int parkId)
        {
            IList<int> list = new List<int>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    List<int> siteIdList = new List<int>();
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("" +
                        "SELECT DISTINCT site.site_id FROM site " +
                        "JOIN campground ON campground.campground_id = site.campground_id " +
                        "JOIN reservation ON reservation.site_id = site.site_id " +
                        "WHERE park_id = @park_id " +
                             "AND site.site_id NOT IN " +
                             "  (SELECT site_id from reservation WHERE (GETDATE() BETWEEN from_date AND to_date))"
                            , conn);
                    cmd.Parameters.AddWithValue("@park_id", parkId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int siteId = Convert.ToInt32(reader["site_id"]);
                        //CHANGE NAME
                        list.Add(siteId);
                    }
                    foreach (int value in siteIdList)
                    {
                        Console.WriteLine(value);
                    }

                    //foreach (int siteid in siteIdList)
                    //{

                    //    try
                    //    {
                    //        using (SqlConnection conne = new SqlConnection(connectionString))
                    //        {
                    //            conne.Open();
                    //            SqlCommand cmd2 = new SqlCommand("SELECT * FROM site WHERE site_id = @site_id;", conne);
                    //            cmd2.Parameters.AddWithValue("@site_id", siteid);
                    //            SqlDataReader reader2 = cmd2.ExecuteReader();
                    //            while (reader2.Read())
                    //            {
                    //                Site site = GetSiteFromReader(reader2);
                    //                list.Add(site);
                    //            }
                    //        }
                    //    }
                    //    catch
                    //    {
                    //        return list;
                    //    }
                    //
                    //
                    //}
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
