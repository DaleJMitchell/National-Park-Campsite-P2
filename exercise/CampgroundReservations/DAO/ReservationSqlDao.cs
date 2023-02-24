using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CampgroundReservations.Models;

namespace CampgroundReservations.DAO
{
    public class ReservationSqlDao : IReservationDao
    {
        private readonly string connectionString;

        public ReservationSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public int CreateReservation(int siteId, string name, DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("" +
                        "INSERT INTO reservation (site_id, name, from_date, to_date) " +
                        "OUTPUT INSERTED.reservation_id " +
                        "VALUES (@site_id, @name, @from_date, @to_date);", conn);
                    cmd.Parameters.AddWithValue("@site_id", siteId);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@from_date", fromDate);
                    cmd.Parameters.AddWithValue("@to_date", toDate);

                    int reservationNumber = Convert.ToInt32(cmd.ExecuteScalar());
                    return reservationNumber;

                }
            }
            catch { return 0; }
        }

        public IList<Reservation> UpcomingReservation(int parkId)
        {
            IList<Reservation> list = new List<Reservation>();
            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("" +
                        "SELECT * " +
                        "FROM reservation " +
                        "JOIN site on site.site_id = reservation.site_id " +
                        "JOIN campground ON campground.campground_id = site.campground_id " +
                        "WHERE from_date BETWEEN GETDATE() AND (GETDATE()+30) " +
                            "AND to_date > GETDATE() " +
                            "AND park_id = @park_id; ", conn);
                    cmd.Parameters.AddWithValue("@park_id", parkId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        Reservation reservation = GetReservationFromReader(reader);
                        list.Add(reservation);
                    }
                }
            }
            catch
            {
                return list;
            }
            return list;
        }

        private Reservation GetReservationFromReader(SqlDataReader reader)
        {
            Reservation reservation = new Reservation();
            reservation.ReservationId = Convert.ToInt32(reader["reservation_id"]);
            reservation.SiteId = Convert.ToInt32(reader["site_id"]);
            reservation.Name = Convert.ToString(reader["name"]);
            reservation.FromDate = Convert.ToDateTime(reader["from_date"]);
            reservation.ToDate = Convert.ToDateTime(reader["to_date"]);
            reservation.CreateDate = Convert.ToDateTime(reader["create_date"]);

            return reservation;
        }
    }
}
