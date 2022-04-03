using Microsoft.EntityFrameworkCore;
using Suaah.Models;

namespace Suaah.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Airline> Airlines { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<SocialData> SocialData { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightBooking> FlightBookings { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelBooking> HotelBookings { get; set; }
        public DbSet<HotelRoom> HotelRooms { get; set; }
        public DbSet<FlightClass> FlightClassss { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Airline>().ToTable("Airline");
            modelBuilder.Entity<Airport>().ToTable("Airport");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Flight>().ToTable("Flight");
            modelBuilder.Entity<FlightBooking>().ToTable("FlightBooking");
            modelBuilder.Entity<Hotel>().ToTable("Hotel");
            modelBuilder.Entity<HotelRoom>().ToTable("HotelRoom");
            modelBuilder.Entity<HotelBooking>().ToTable("HotelBooking");
            modelBuilder.Entity<FlightClass>().ToTable("FlightClass");

            modelBuilder.Entity<Flight>()
                    .HasOne(f => f.DepartingAirport)
                    .WithMany(t => t.DepartingFlights)
                    .HasForeignKey(m => m.DepartingAirportId)
                    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                   .HasOne(f => f.ArrivingAirport)
                   .WithMany(t => t.ArrivingFlights)
                   .HasForeignKey(m => m.ArrivingAirportId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
