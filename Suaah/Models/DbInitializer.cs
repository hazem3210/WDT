﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Suaah.Data;

namespace Suaah.Models
{
    public class DbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
             RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public async void Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception) { throw; }

            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Manager)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new IdentityUser
                {
                    UserName = "adminsuaah@gmail.com",
                    Email = "adminsuaah@gmail.com",
                    PhoneNumber = "01111111111",
                }, "Admin123/").GetAwaiter().GetResult();

                IdentityUser user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "adminsuaah@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Manager).GetAwaiter().GetResult();

                _userManager.CreateAsync(new IdentityUser
                {
                    UserName = "admin1@gmail.com",
                    Email = "admin1@gmail.com",
                    PhoneNumber = "01111111111",
                }, "Admin123/").GetAwaiter().GetResult();

                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin1@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

                _userManager.CreateAsync(new IdentityUser
                {
                    UserName = "admin2@gmail.com",
                    Email = "admin2@gmail.com",
                    PhoneNumber = "01111111111",
                }, "Admin123/").GetAwaiter().GetResult();

                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin2@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

                _userManager.CreateAsync(new IdentityUser
                {
                    UserName = "customer1@gmail.com",
                    Email = "customer1@gmail.com",
                    PhoneNumber = "01111111111",
                }, "Cust123/").GetAwaiter().GetResult();

                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "customer1@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();


                _userManager.CreateAsync(new IdentityUser
                {
                    UserName = "customer2@gmail.com",
                    Email = "customer2@gmail.com",
                    PhoneNumber = "01111111111",
                }, "Cust123/").GetAwaiter().GetResult();

                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "customer2@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

               
                var c1 = await _context.Users.FirstOrDefaultAsync(c => c.Email.Contains("customer1"));
                var c2 = await _context.Users.FirstOrDefaultAsync(c => c.Email.Contains("customer2"));

                var customers = new List<Customer>()
                {
                    new Customer()
                    {
                        Id = c1.Id,
                        Name = "customer1",
                        PassportNumber = "customer1Pass",
                        CitizenshipCountry = "Egypt",
                        ResidenceCountry = "Egypt",
                        BirthDate = new DateTime(1980, 12, 1)
                    },

                    new Customer()
                    {
                        Id = c2.Id,
                        Name = "customer2",
                        PassportNumber = "customer2Pass",
                        CitizenshipCountry = "Egypt",
                        ResidenceCountry = "Egypt",
                        BirthDate = new DateTime(1990, 12, 1)
                    }
                };

                await _context.Customers.AddRangeAsync(customers);
                await _context.SaveChangesAsync();

                var social = new List<SocialData>()
                {
                    new SocialData()
                    {
                        SocialName = "Phone",
                        Link ="011235469875"
                    },

                    new SocialData()
                    {
                        SocialName = "Facebook",
                        Link ="https://ar-ar.facebook.com/"
                    },
                    new SocialData()
                    {
                        SocialName = "Twitter",
                        Link ="https://twitter.com/home"
                    }
                };
                await _context.SocialData.AddRangeAsync(social);
                await _context.SaveChangesAsync();

                var hotels = new List<Hotel>()
                {
                    new Hotel()
                    {
                        Name="Rixos",
                        Address ="Sharm El Sheikh, Egypt",
                        Email = "RixosSharm@gmail.com",
                        PhoneNumber = "0123654789",
                        Description = "Property Location: Located in Sharm el Sheikh, Rixos Sharm El Sheikh - All Inclusive is by the sea, a 1-minute drive from Nabq Bay and 6 minutes from Rehana Beach. This 5-star resort is 4.2 mi (6.8 km) from Nabq Protected Area and 4.9 mi (7.8 km) from Shark's Bay.",
                        Stars = "5"
                    },
                     new Hotel()
                    {
                        Name="Conrad Cairo",
                        Address ="Cairo, Egypt",
                        Email = "ConradCairo@gmail.com",
                        PhoneNumber = "0113654789",
                        Description = "Property Location: With a stay at Conrad Cairo, you'll be centrally located in Cairo, within a 15-minute drive of Egyptian Museum and Tahrir Square. This 5-star hotel is 2.8 mi (4.5 km) from Khan el-Khalili and 4.8 mi (7.7 km) from Cairo Tower. ",
                        Stars = "4"
                    },
                     new Hotel()
                    {
                        Name="Mercure Paris Saint Lazare Monceau",
                        Address ="Paris, France",
                        Email = "Mercure@gmail.com",
                        PhoneNumber = "25631488",
                        Description = "Property Location: A stay at Mercure Paris Saint Lazare Monceau places you in the heart of Paris, within a 15-minute walk of Place de Clichy and Parc Monceau. This 4-star hotel is 0.7 mi (1.2 km) from Casino de Paris and 0.8 mi (1.3 km) from Moulin Rouge. ",
                        Stars = "5"
                    },
                      new Hotel()
                    {
                        Name="Traders ",
                        Address ="Kuala Lumpur",
                        Email = "Traders@gmail.com",
                        PhoneNumber = "589214753",
                        Description = "Property Location: With a stay at Traders Hotel Kuala Lumpur, you'll be centrally located in Kuala Lumpur, steps from Aquaria KLCC and minutes from KLCC Park. This 5-star hotel is close to Kuala Lumpur Convention Centre and Pavilion Kuala Lumpur. ",
                        Stars = "3"
                    },
                };

                await _context.Hotels.AddRangeAsync(hotels);
                await _context.SaveChangesAsync();

                var services = new List<Services>()
                {
                    new Services(){Name = "Air Conditioning"},
                    new Services(){Name = "Internet"},
                    new Services(){Name = "Babysitting"},
                    new Services(){Name = "Parking"},
                    new Services(){Name = "Sauna"},
                    new Services(){Name = "Spa"},
                    new Services(){Name = "Room Only"},
                };

                await _context.Services.AddRangeAsync(services);
                await _context.SaveChangesAsync();


                var hotelRooms = new List<HotelRoom>()
                {
                    new HotelRoom()
                    {
                        Price = 300.0,
                        CancelBeforeHours=0,
                        Description = "Deluxe, 1 King Bed, Non Smoking, City View",
                        HotelId=1
                    },

                    new HotelRoom()
                    {
                        Price = 600.0,
                        CancelBeforeHours=0,
                        Description = "Room, 2 Twin Beds, Non Smoking",
                        HotelId=1
                    },
                    
                    new HotelRoom()
                    {
                        Price = 250.0,
                        CancelBeforeHours=0,
                        Description = "Room, 2 Twin Beds, Non Smoking",
                        HotelId=1
                    }, 
                    new HotelRoom()
                    {
                        Price = 300.0,
                        CancelBeforeHours=0,
                        Description = "Room, 1 King Bed, Non Smoking ",
                        HotelId=2
                    },
                    new HotelRoom()
                    {
                        Price = 700.0,
                        CancelBeforeHours=0,
                        Description = "Executive Room, 2 Twin Beds, Non Smoking, City View",
                        HotelId=2
                    },
                    new HotelRoom()
                    {
                        Price = 800.0,
                        CancelBeforeHours=0,
                        Description = "Executive Room, 1 King Bed, Non Smoking, City View",
                        HotelId=2
                    }, 
                    new HotelRoom()
                    {
                        Price = 650.0,
                        CancelBeforeHours=0,
                        Description = "Junior Suite, 1 King Bed, Non Smoking, City View",
                        HotelId=3
                    }, 
                    new HotelRoom()
                    {
                        Price = 750.0,
                        CancelBeforeHours=0,
                        Description = "Suite, 1 Bedroom, Non Smoking, City View",
                        HotelId=3
                    },
                    new HotelRoom()
                    {
                        Price = 800.0,
                        CancelBeforeHours=0,
                        Description = "Suite, 2 Bedrooms, City View ",
                        HotelId=3
                    },
                    new HotelRoom()
                    {
                        Price = 500.0,
                        CancelBeforeHours=0,
                        Description = "Room Deluxe Canal View",
                        HotelId=4
                    },
                    new HotelRoom()
                    {
                        Price = 800.0,
                        CancelBeforeHours=0,
                        Description = "Double Deluxe Canal View",
                        HotelId=4
                    },
                    new HotelRoom()
                    {
                        Price = 1800.0,
                        CancelBeforeHours=0,
                        Description = "Twin Superior",
                        HotelId=4
                    }
                };
                await _context.HotelRooms.AddRangeAsync(hotelRooms);
                await _context.SaveChangesAsync();

                var roomServ = new List<HotelRoomServices>()
                {
                    new HotelRoomServices()
                    {
                        HotelRoomId = 1,
                        ServicesId=1
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 1,
                        ServicesId=3
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 2,
                        ServicesId=7
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 3,
                        ServicesId=7
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 4,
                        ServicesId=1
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 4,
                        ServicesId=5
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 5,
                        ServicesId=7
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 6,
                        ServicesId=6
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 6,
                        ServicesId=2
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 7,
                        ServicesId=7
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 8,
                        ServicesId=7
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 9,
                        ServicesId=7
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 10,
                        ServicesId=1
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 10,
                        ServicesId=4
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 10,
                        ServicesId=3
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 11,
                        ServicesId=7
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 12,
                        ServicesId=1
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 12,
                        ServicesId=2
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 12,
                        ServicesId=6
                    },
                    new HotelRoomServices()
                    {
                        HotelRoomId = 12,
                        ServicesId=4
                    }
                };

               await _context.HotelRoomServices.AddRangeAsync(roomServ);
                await _context.SaveChangesAsync();

                var flightClasses = new List<FlightClass>()
                {
                    new FlightClass()
                    {
                        Class="First"
                    },
                    new FlightClass()
                    {
                        Class="Business"
                    },
                    new FlightClass()
                    {
                        Class="Economy"
                    },
                };

                await _context.FlightClassss.AddRangeAsync(flightClasses);
                await _context.SaveChangesAsync();


                var countries = new List<Country>()
                {
                    new Country()
                    {
                        Name ="Egypt",
                        PhotoPath ="/img/Country/download.png"
                    },
                    new Country()
                    {
                        Name ="China",
                        PhotoPath ="/img/Country/download (2).png"
                    },
                    new Country()
                    {
                        Name ="Cuba",
                        PhotoPath ="/img/Country/download (1).png"
                    },
                    new Country()
                    {
                        Name ="Australia",
                        PhotoPath ="/img/Country/download (3).png"
                    },
                     new Country()
                    {
                        Name ="United States",
                        PhotoPath ="/img/Country/download (4).png"
                    },
                      new Country()
                    {
                        Name ="España",
                        PhotoPath ="/img/Country/download (6).png"
                    },
                      new Country()
                    {
                        Name ="France",
                        PhotoPath ="/img/Country/download (7).png"
                    }
                };

                await _context.Countries.AddRangeAsync(countries);
                await _context.SaveChangesAsync();


                var airlines = new List<Airline>()
                {
                    new Airline()
                    {
                        Code = "RBG",
                        Name ="Air Arabia Egypt",
                        Country = "Cairo, Egypt",
                        Description = "Air Arabia Egypt is an Alexandria-based Air Arabia Group company. The airline offers low-cost travel to a number of destinations across the region. The company focuses on booking online. Air Arabia Egypt also provides booking facilities through call centers, travel agents, appointed GSA's and sales shops."
                    },
                    new Airline()
                    {
                        Code = "MSC",
                        Name ="Air Cairo",
                        Country = "Europe",
                        Description = "Air Cairo is a low-fare airline based in Cairo, Egypt.  The airline is part owned by Egyptair. Air Cairo operates scheduled flights to the Middle East and Europe and also operates charter flights to Egypt from Europe on behalf of tour operators. Its base is Cairo International Airport, Sharm El Sheikh International Airport and Hurghada International Airport with the company head office in the Sheraton Heliopolis Zone. "
                    },
                    new Airline()
                    {
                        Code = "KHH",
                        Name ="Alexandria Airlines",
                        Country = "Cairo, Egypt",
                        Description = "The airline was established in 2006 and commenced its operations in March 2007.In April 2022, it was announced the airline would be resuming operations, from 23 April, from Alexandria to Amman and Kuwait."
                    },
                    new Airline()
                    {
                        Code = "MSR",
                        Name ="EgyptAir",
                        Country = "Cairo, Egypt",
                        Description = "Egyptair is the state-owned flag carrier of Egypt. The airline is headquartered at Cairo International Airport, its main hub, operating scheduled passenger and freight services to 70 destinations in the Middle East, Europe, Africa, Asia, and the Americas."
                    }
                };

                await _context.Airlines.AddRangeAsync(airlines);
                await _context.SaveChangesAsync();


                var airports = new List<Airport>()
                {
                    new Airport()
                    {
                        Name = "Cairo International Airport",
                        City = "Cairo",
                        CountryId = 1,
                        Description = "Cairo International Airport is the principal international airport of Cairo and the largest and busiest airport in Egypt and serves as the primary hub for EgyptAir and Nile Air as well as several other airlines. The airport is located in Heliopolis, to the northeast of Cairo around fifteen kilometres (eight nautical miles) from the business area of the city and has an area of approximately 37 km2 (14 sq mi). Since 2020, it is the busiest airport in Africa, in terms of total passengers."
                    },
                    new Airport()
                    {
                        Name = "Gran Canaria Airport",
                        City = "Gran Canaria",
                        CountryId = 6,
                        Description = "Gran Canaria Airport, sometimes also known as Gando Airport (Spanish: Aeropuerto de Gran Canaria), is a passenger and freight airport on the island of Gran Canaria. It is an important airport within the Spanish air-transport network (owned and managed by a public enterprise, AENA), as it holds the sixth position in terms of passengers, and fifth in terms of operations and cargo transported. It also ranks first of the Canary Islands in all three categories, although the island of Tenerife has higher passenger numbers overall if statistics from the two airports located on the island are combined."
                    },
                    new Airport()
                    {
                        Name = "Gran Canaria Airport",
                        City = "Gran Canaria",
                        CountryId = 6,
                        Description = "Gran Canaria Airport, sometimes also known as Gando Airport (Spanish: Aeropuerto de Gran Canaria), is a passenger and freight airport on the island of Gran Canaria. It is an important airport within the Spanish air-transport network (owned and managed by a public enterprise, AENA), as it holds the sixth position in terms of passengers, and fifth in terms of operations and cargo transported. It also ranks first of the Canary Islands in all three categories, although the island of Tenerife has higher passenger numbers overall if statistics from the two airports located on the island are combined."
                    },
                    new Airport()
                    {
                        Name = "Hurghada International Airport",
                        City = "Hurghada",
                        CountryId = 1,
                        Description = "Hurghada International Airport is the international airport of Hurghada in Egypt. It is located inland, 5 km (3.1 mi) southwest of El Dahar, the downtown of Hurghada. It is the second busiest airport in Egypt after Cairo International Airport and an important destination for leisure flights mainly from Europe."
                    }, 
                    new Airport()
                    {
                        Name = "Roland Garros Airport",
                        City = "Sainte-Marie",
                        CountryId = 7,
                        Description = "Hurghada International Airport is the international airport of Hurghada in Egypt. It is located inland, 5 km (3.1 mi) southwest of El Dahar, the downtown of Hurghada. It is the second busiest airport in Egypt after Cairo International Airport and an important destination for leisure flights mainly from Europe."
                    },
                };

                await _context.Airports.AddRangeAsync(airports);
                await _context.SaveChangesAsync();


                var flights = new List<Flight>()
                {
                    new Flight()
                    {
                        DepartingGate = "G10",
                        ArriveGate = "G2",
                        NumberOfStops = "0",
                        LeaveTime = new DateTime(2022,6,5,5,40,10),
                        AirlineCode = "MSR",
                        DepartingAirportId = 1,
                        ArrivingAirportId=2

                    },
                     new Flight()
                    {
                        DepartingGate = "G5",
                        ArriveGate = "G3",
                        NumberOfStops = "0",
                        LeaveTime = new DateTime(2022,6,5,5,40,10),
                        AirlineCode = "KHH",
                        DepartingAirportId = 2,
                        ArrivingAirportId=1

                    },

                     new Flight()
                    {
                        DepartingGate = "G14",
                        ArriveGate = "G12",
                        NumberOfStops = "5",
                        LeaveTime = new DateTime(2022,6,5,6,40,10),
                        AirlineCode = "KHH",
                        DepartingAirportId = 1,
                        ArrivingAirportId=2

                    },
                      new Flight()
                    {
                        DepartingGate = "G10",
                        ArriveGate = "G2",
                        NumberOfStops = "3",
                        LeaveTime = new DateTime(2022,6,5,6,40,10),
                        AirlineCode = "MSR",
                        DepartingAirportId = 3,
                        ArrivingAirportId=5

                    }
                };

                await _context.Flights.AddRangeAsync(flights);
                await _context.SaveChangesAsync();


                var flightsclasses = new List<FlightsClasses>()
                {
                    new FlightsClasses()
                    {
                        Price = 600,
                        FlightClassId = 1,
                        FlightId = 1
                    },
                    new FlightsClasses()
                    {
                        Price = 500,
                        FlightClassId = 2,
                        FlightId = 1
                    },
                    new FlightsClasses()
                    {
                        Price = 400,
                        FlightClassId = 3,
                        FlightId = 1
                    },
                    new FlightsClasses()
                    {
                        Price = 600,
                        FlightClassId = 1,
                        FlightId = 2
                    },

                    new FlightsClasses()
                    {
                        Price = 500,
                        FlightClassId = 2,
                        FlightId = 2
                    },

                    new FlightsClasses()
                    {
                        Price = 400,
                        FlightClassId = 3,
                        FlightId = 2
                    },
                    new FlightsClasses()
                    {
                        Price = 600,
                        FlightClassId = 1,
                        FlightId = 4
                    },
                    new FlightsClasses()
                    {
                        Price = 500,
                        FlightClassId = 2,
                        FlightId = 4
                    },
                    new FlightsClasses()
                    {
                        Price = 400,
                        FlightClassId = 3,
                        FlightId = 4
                    },
                    new FlightsClasses()
                    {
                        Price = 600,
                        FlightClassId = 1,
                        FlightId = 3
                    },
                    new FlightsClasses()
                    {
                        Price = 500,
                        FlightClassId = 2,
                        FlightId = 3
                    },
                    new FlightsClasses()
                    {
                        Price = 400,
                        FlightClassId = 3,
                        FlightId = 3
                    },
                };

                await _context.FlightsClasses.AddRangeAsync(flightsclasses);
                await _context.SaveChangesAsync();

            }

            return;
        }
    }
}