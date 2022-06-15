using Microsoft.AspNetCore.Identity;
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

        public void Initialize()
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

                IdentityUser user = _context.Users.FirstOrDefault(u => u.Email == "adminsuaah@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Manager).GetAwaiter().GetResult();

                _userManager.CreateAsync(new IdentityUser
                {
                    UserName = "admin1@gmail.com",
                    Email = "admin1@gmail.com",
                    PhoneNumber = "01111111111",
                }, "Admin123/").GetAwaiter().GetResult();

                user = _context.Users.FirstOrDefault(u => u.Email == "admin1@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

                _userManager.CreateAsync(new IdentityUser
                {
                    UserName = "admin2@gmail.com",
                    Email = "admin2@gmail.com",
                    PhoneNumber = "01111111111",
                }, "Admin123/").GetAwaiter().GetResult();

                user = _context.Users.FirstOrDefault(u => u.Email == "admin2@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

                _userManager.CreateAsync(new IdentityUser
                {
                    UserName = "customer1@gmail.com",
                    Email = "customer1@gmail.com",
                    PhoneNumber = "01111111111",
                }, "Cust123/").GetAwaiter().GetResult();

                user = _context.Users.FirstOrDefault(u => u.Email == "customer1@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Customer).GetAwaiter().GetResult();


                _userManager.CreateAsync(new IdentityUser
                {
                    UserName = "customer2@gmail.com",
                    Email = "customer2@gmail.com",
                    PhoneNumber = "01111111111",
                }, "Cust123/").GetAwaiter().GetResult();

                user = _context.Users.FirstOrDefault(u => u.Email == "customer2@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Customer).GetAwaiter().GetResult();

               
                var c1 = _context.Users.FirstOrDefault(c => c.Email.Contains("customer1"));
                var c2 = _context.Users.FirstOrDefault(c => c.Email.Contains("customer2"));

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

                _context.Customers.AddRange(customers);
                _context.SaveChanges();

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
                _context.SocialData.AddRange(social);
                _context.SaveChanges();

                var countries = new List<Country>()
                {
                    new Country()
                    {
                        Name ="Egypt",
                        PhotoPath ="/img/Country/download.jpeg",
                        FlagPath="/img/Country/Egypt.webp"
                    },
                    new Country()
                    {
                        Name ="China",
                        PhotoPath ="/img/Country/download (2).jpg",
                        FlagPath="/img/Country/China.png"
                    },
                    new Country()
                    {
                        Name ="Cuba",
                        PhotoPath ="/img/Country/download (1).jpg",
                        FlagPath="/img/Country/Cuba.svg"
                    },
                    new Country()
                    {
                        Name ="Australia",
                        PhotoPath ="/img/Country/download (3).jpg",
                        FlagPath="/img/Country/Australia.png"
                    },
                     new Country()
                    {
                        Name ="United States",
                        PhotoPath ="/img/Country/download (4).jpg",
                        FlagPath="/img/Country/America.png"
                    },
                      new Country()
                    {
                        Name ="España",
                        PhotoPath ="/img/Country/download (6).jpg",
                        FlagPath="/img/Country/Spain.webp"
                    },
                      new Country()
                    {
                        Name ="France",
                        PhotoPath ="/img/Country/download (7).jfif",
                        FlagPath="/img/Country/France.webp"
                    },
                    new Country()
                    {
                        Name ="Malaysia",
                        PhotoPath ="/img/Country/Malaysia.svg.png",
                        FlagPath="/img/Country/Malaysia.svg.png"
                    }
                };

                _context.Countries.AddRange(countries);
                _context.SaveChanges();


                var hotels = new List<Hotel>()
                {
                    new Hotel()
                    {
                        Name="Rixos",
                        CountryId = 1,
                        Address ="Sharm El Sheikh",
                        Email = "RixosSharm@gmail.com",
                        PhoneNumber = "0123654789",
                        Description = "Property Location: Located in Sharm el Sheikh, Rixos Sharm El Sheikh - All Inclusive is by the sea, a 1-minute drive from Nabq Bay and 6 minutes from Rehana Beach. This 5-star resort is 4.2 mi (6.8 km) from Nabq Protected Area and 4.9 mi (7.8 km) from Shark's Bay.",
                        Stars = "5",
                        ImageUrl=@"\img\hotel\Rixos.webp"
                    },
                     new Hotel()
                    {
                        Name="Conrad Cairo",
                         CountryId = 1,
                        Address ="Cairo",
                        Email = "ConradCairo@gmail.com",
                        PhoneNumber = "0113654789",
                        Description = "Property Location: With a stay at Conrad Cairo, you'll be centrally located in Cairo, within a 15-minute drive of Egyptian Museum and Tahrir Square. This 5-star hotel is 2.8 mi (4.5 km) from Khan el-Khalili and 4.8 mi (7.7 km) from Cairo Tower. ",
                        Stars = "4",
                        ImageUrl=@"\img\hotel\Conrad Cairo.jpg"
                    },
                     new Hotel()
                    {
                        Name="Mercure Paris Saint Lazare Monceau",
                         CountryId = 7,
                        Address ="Paris",
                        Email = "Mercure@gmail.com",
                        PhoneNumber = "25631488",
                        Description = "Property Location: A stay at Mercure Paris Saint Lazare Monceau places you in the heart of Paris, within a 15-minute walk of Place de Clichy and Parc Monceau. This 4-star hotel is 0.7 mi (1.2 km) from Casino de Paris and 0.8 mi (1.3 km) from Moulin Rouge. ",
                        Stars = "5",
                        ImageUrl=@"\img\hotel\Mercure Paris Saint Lazare Monceau.jpg"
                    },
                      new Hotel()
                    {
                        Name="Traders ",
                        CountryId= 8,
                        Address ="Kuala Lumpur",
                        Email = "Traders@gmail.com",
                        PhoneNumber = "589214753",
                        Description = "Property Location: With a stay at Traders Hotel Kuala Lumpur, you'll be centrally located in Kuala Lumpur, steps from Aquaria KLCC and minutes from KLCC Park. This 5-star hotel is close to Kuala Lumpur Convention Centre and Pavilion Kuala Lumpur. ",
                        Stars = "3",
                        ImageUrl=@"\img\hotel\Traders.jpg"
                    },
                };

                _context.Hotels.AddRange(hotels);
                _context.SaveChanges();

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

                _context.Services.AddRange(services);
                _context.SaveChanges();


                var hotelRooms = new List<HotelRoom>()
                {
                    new HotelRoom()
                    {
                        Price = 300.0,
                        CancelBeforeHours=0,
                        Description = "Deluxe, 1 King Bed, Non Smoking, City View",
                        HotelId=1,
                        ImageUrl=@"\img\Rooms\1.jpg"
                    },

                    new HotelRoom()
                    {
                        Price = 600.0,
                        CancelBeforeHours=0,
                        Description = "Room, 2 Twin Beds, Non Smoking",
                        HotelId=1,
                        ImageUrl=@"\img\Rooms\2.jpg"
                    },
                    
                    new HotelRoom()
                    {
                        Price = 350.0,
                        CancelBeforeHours=0,
                        Description = "Room, 2 Twin Beds",
                        HotelId=1,
                        ImageUrl=@"\img\Rooms\3.jpg"
                    }, 
                    new HotelRoom()
                    {
                        Price = 300.0,
                        CancelBeforeHours=0,
                        Description = "Room, 1 King Bed, Non Smoking ",
                        HotelId=2,
                        ImageUrl=@"\img\Rooms\4.jpg"
                    },
                    new HotelRoom()
                    {
                        Price = 700.0,
                        CancelBeforeHours=0,
                        Description = "Executive Room, 2 Twin Beds, Non Smoking, City View",
                        HotelId=2,
                        ImageUrl=@"\img\Rooms\5.jpg"
                    },
                    new HotelRoom()
                    {
                        Price = 800.0,
                        CancelBeforeHours=0,
                        Description = "Executive Room, 1 King Bed, Non Smoking, City View",
                        HotelId=2,
                        ImageUrl=@"\img\Rooms\6.jpg"
                    }, 
                    new HotelRoom()
                    {
                        Price = 650.0,
                        CancelBeforeHours=0,
                        Description = "Junior Suite, 1 King Bed, Non Smoking, City View",
                        HotelId=3,
                        ImageUrl=@"\img\Rooms\7.jpg"
                    }, 
                    new HotelRoom()
                    {
                        Price = 750.0,
                        CancelBeforeHours=0,
                        Description = "Suite, 1 Bedroom, Non Smoking, City View",
                        HotelId=3,
                        ImageUrl=@"\img\Rooms\8.jpg"
                    },
                    new HotelRoom()
                    {
                        Price = 800.0,
                        CancelBeforeHours=0,
                        Description = "Suite, 2 Bedrooms, City View ",
                        HotelId=3,
                        ImageUrl=@"\img\Rooms\9.jpg"
                    },
                    new HotelRoom()
                    {
                        Price = 500.0,
                        CancelBeforeHours=0,
                        Description = "Room Deluxe Canal View",
                        HotelId=4,
                        ImageUrl=@"\img\Rooms\10.jpg"
                    },
                    new HotelRoom()
                    {
                        Price = 800.0,
                        CancelBeforeHours=0,
                        Description = "Double Deluxe Canal View",
                        HotelId=4,
                        ImageUrl=@"\img\Rooms\11.jpg"
                    },
                    new HotelRoom()
                    {
                        Price = 1800.0,
                        CancelBeforeHours=0,
                        Description = "Twin Superior",
                        HotelId=4,
                        ImageUrl=@"\img\Rooms\12.jpg"
                    }
                };
                _context.HotelRooms.AddRange(hotelRooms);
                _context.SaveChanges();

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

                _context.HotelRoomServices.AddRange(roomServ);
                _context.SaveChanges();

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

                _context.FlightClassss.AddRange(flightClasses);
                _context.SaveChanges();

               
                var airlines = new List<Airline>()
                {
                    new Airline()
                    {
                        Code = "RBG",
                        Name ="Air Arabia Egypt",
                        CountryId = 1,
                        Description = "Air Arabia Egypt is an Alexandria-based Air Arabia Group company. The airline offers low-cost travel to a number of destinations across the region. The company focuses on booking online. Air Arabia Egypt also provides booking facilities through call centers, travel agents, appointed GSA's and sales shops.",
                        ImageUrl=@"\img\airlines\Air Arabia Egypt.png"
                    },
                    new Airline()
                    {
                        Code = "MSC",
                        Name ="Air Cairo",
                        CountryId = 1,
                        Description = "Air Cairo is a low-fare airline based in Cairo, Egypt.  The airline is part owned by Egyptair. Air Cairo operates scheduled flights to the Middle East and Europe and also operates charter flights to Egypt from Europe on behalf of tour operators. Its base is Cairo International Airport, Sharm El Sheikh International Airport and Hurghada International Airport with the company head office in the Sheraton Heliopolis Zone. ",
                        ImageUrl=@"\img\airlines\Air Cairo.jpg"
                    },
                    new Airline()
                    {
                        Code = "KHH",
                        Name ="Alexandria Airlines",
                        CountryId = 1,
                        Description = "The airline was established in 2006 and commenced its operations in March 2007.In April 2022, it was announced the airline would be resuming operations, from 23 April, from Alexandria to Amman and Kuwait.",
                        ImageUrl=@"\img\airlines\Alexandria Airlines.jpg"
                    },
                    new Airline()
                    {
                        Code = "MSR",
                        Name ="EgyptAir",
                         CountryId = 1,
                        Description = "Egyptair is the state-owned flag carrier of Egypt. The airline is headquartered at Cairo International Airport, its main hub, operating scheduled passenger and freight services to 70 destinations in the Middle East, Europe, Africa, Asia, and the Americas.",
                        ImageUrl=@"\img\airlines\EgyptAir.png"
                    }
                };

                _context.Airlines.AddRange(airlines);
                _context.SaveChanges();

                var airports = new List<Airport>()
                {
                    new Airport()
                    {
                        Name = "Cairo International Airport",
                        City = "Cairo",
                        CountryId = 1,
                        Description = "Cairo International Airport is the principal international airport of Cairo and the largest and busiest airport in Egypt and serves as the primary hub for EgyptAir and Nile Air as well as several other airlines. The airport is located in Heliopolis, to the northeast of Cairo around fifteen kilometres (eight nautical miles) from the business area of the city and has an area of approximately 37 km2 (14 sq mi). Since 2020, it is the busiest airport in Africa, in terms of total passengers.",
                        ImageUrl=@"\img\airport\Cairo International Airport.jpeg"
                    },
                    new Airport()
                    {
                        Name = "Gran Canaria Airport",
                        City = "Gran Canaria",
                        CountryId = 6,
                        Description = "Gran Canaria Airport, sometimes also known as Gando Airport (Spanish: Aeropuerto de Gran Canaria), is a passenger and freight airport on the island of Gran Canaria. It is an important airport within the Spanish air-transport network (owned and managed by a public enterprise, AENA), as it holds the sixth position in terms of passengers, and fifth in terms of operations and cargo transported. It also ranks first of the Canary Islands in all three categories, although the island of Tenerife has higher passenger numbers overall if statistics from the two airports located on the island are combined.",
                        ImageUrl=@"\img\airport\Gran Canaria Airport.jpg"
                    },
                    new Airport()
                    {
                        Name = "Hurghada International Airport",
                        City = "Hurghada",
                        CountryId = 1,
                        Description = "Hurghada International Airport is the international airport of Hurghada in Egypt. It is located inland, 5 km (3.1 mi) southwest of El Dahar, the downtown of Hurghada. It is the second busiest airport in Egypt after Cairo International Airport and an important destination for leisure flights mainly from Europe.",
                        ImageUrl=@"\img\airport\Hurghada International Airport.jpg"
                    }, 
                    new Airport()
                    {
                        Name = "Roland Garros Airport",
                        City = "Sainte-Marie",
                        CountryId = 7,
                        Description = "Hurghada International Airport is the international airport of Hurghada in Egypt. It is located inland, 5 km (3.1 mi) southwest of El Dahar, the downtown of Hurghada. It is the second busiest airport in Egypt after Cairo International Airport and an important destination for leisure flights mainly from Europe.",
                        ImageUrl=@"\img\airport\Roland Garros Airport.jpg"
                    },
                };

                _context.Airports.AddRange(airports);
                _context.SaveChanges();

                var flights = new List<Flight>()
                {
                    new Flight()
                    {
                        DepartingGate = "G100",
                        ArriveGate = "G205",
                        NumberOfStops = "0",
                        LeaveTime = new DateTime(2022,7,5,5,0,0),
                        AirlineCode = "MSR",
                        DepartingAirportId = 1,
                        ArrivingAirportId=2

                    },
                     new Flight()
                    {
                        DepartingGate = "G517",
                        ArriveGate = "G143",
                        NumberOfStops = "0",
                        LeaveTime = new DateTime(2022,6,30,10,30,0),
                        AirlineCode = "KHH",
                        DepartingAirportId = 2,
                        ArrivingAirportId=1

                    },

                     new Flight()
                    {
                        DepartingGate = "G921",
                        ArriveGate = "G12",
                        NumberOfStops = "5",
                        LeaveTime = new DateTime(2022,8,1,13,0,0),
                        AirlineCode = "KHH",
                        DepartingAirportId = 1,
                        ArrivingAirportId=2

                    },
                      new Flight()
                    {
                        DepartingGate = "G10",
                        ArriveGate = "G2",
                        NumberOfStops = "3",
                        LeaveTime = new DateTime(2022,6,16,19,30,0),
                        AirlineCode = "MSR",
                        DepartingAirportId = 3,
                        ArrivingAirportId=4

                    }
                };

                _context.Flights.AddRange(flights);
                _context.SaveChanges();

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

                _context.FlightsClasses.AddRange(flightsclasses);
                _context.SaveChanges();
            }

            return;
        }
    }
}
