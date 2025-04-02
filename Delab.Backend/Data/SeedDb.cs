using Delab.AccessData.Data;
using Delab.Helpers;
using Delab.Shared.Entities;
using Delab.Shared.Enum;

namespace Delab.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;
    private readonly IUserHelper _userHelper;

    public SeedDb(DataContext context, IUserHelper userHelper)
    {
        _context = context;
        _userHelper = userHelper;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCountries();
        await CheckSoftPlan();
        await CheckRolesAsync();
        await CheckUserAsync("Nexxtplanet", "SPI", "soporte@nexxtplanet.net", "+1 786 503", UserType.Admin);
    }

    private async Task CheckSoftPlan()
    {
        if (!_context.SoftPlans.Any())
        {
            //Alimentando Planes
            _context.SoftPlans.Add(new SoftPlan
            {
                Name = "Plan 1 Mes",
                Price = 50,
                Meses = 1,
                ClinicsCount = 2,
                Active = true
            });
            _context.SoftPlans.Add(new SoftPlan
            {
                Name = "Plan 6 Mes",
                Price = 300,
                Meses = 6,
                ClinicsCount = 10,
                Active = true
            });
            _context.SoftPlans.Add(new SoftPlan
            {
                Name = "Plan 12 Mes",
                Price = 600,
                Meses = 12,
                ClinicsCount = 100,
                Active = true
            });
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckRolesAsync()
    {
        await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
        await _userHelper.CheckRoleAsync(UserType.Usuario.ToString());
        await _userHelper.CheckRoleAsync(UserType.Auxiliar.ToString());
        await _userHelper.CheckRoleAsync(UserType.Cajero.ToString());
        await _userHelper.CheckRoleAsync(UserType.Tecnico.ToString());
        await _userHelper.CheckRoleAsync(UserType.Clinica.ToString());
    }

    private async Task<User> CheckUserAsync(string firstName, string lastName, string email,
            string phone, UserType userType)
    {
        User user = await _userHelper.GetUserAsync(email);
        if (user == null)
        {
            user = new()
            {
                FirstName = firstName,
                LastName = lastName,
                FullName = $"{firstName} {lastName}",
                Email = email,
                UserName = email,
                PhoneNumber = phone,
                JobPosition = "Administrador",
                UserFrom = "SeedDb",
                UserRoleDetails = new List<UserRoleDetails> { new UserRoleDetails { UserType = userType } },
                Active = true,
            };

            await _userHelper.AddUserAsync(user, "123456");
            await _userHelper.AddUserToRoleAsync(user, userType.ToString());

            //Para Confirmar automaticamente el Usuario y activar la cuenta
            string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            await _userHelper.ConfirmEmailAsync(user, token);
            await _userHelper.AddUserClaims(userType, email);
        }
        return user;
    }

    private async Task CheckCountries()
    {
        if (!_context.Countries.Any())
        {
            _context.Countries.Add(new Country
            {
                Name = "Colombia",
                CodPhone = "+57",
                States = new List<State>()
                {
                    new State
                    {
                        Name = "Antioquia",
                        Cities = new List<City>()
                        {
                            new City{ Name = "Madellin"},
                            new City { Name = "Medellin"},
                            new City { Name = "Envigado"},
                            new City { Name = "Bello"},
                            new City { Name = "Rionegro"}
                        }
                    },
                    new State
                    {
                        Name = "Cundinamarca",
                        Cities = new List<City>()
                        {
                            new City{ Name = "Soacha"},
                            new City { Name = "Facatativa"},
                            new City { Name = "Fusagasuga"},
                            new City { Name = "Chia"},
                            new City { Name = "Zipaquira"}
                        }
                    }
                }
            });

            _context.Countries.Add(new Country
            {
                Name = "Maxico",
                CodPhone = "+57",
                States = new List<State>()
                {
                    new State
                    {
                        Name = "Chiapas",
                        Cities = new List<City>()
                        {
                            new City{ Name = "Tuctla"},
                            new City { Name = "Tapachula"},
                            new City { Name = "San Cristobal"},
                            new City { Name = "Comitan"},
                            new City { Name = "Cintalapa"}
                        }
                    },
                    new State
                    {
                        Name = "Colima",
                        Cities = new List<City>()
                        {
                            new City{ Name = "Manzanillo"},
                            new City { Name = "Queseria"},
                            new City { Name = "El Colomo"},
                            new City { Name = "Comala"},
                            new City { Name = "Armeria"}
                        }
                    }
                }
            });

            await _context.SaveChangesAsync();
        }
    }
}