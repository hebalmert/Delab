using Delab.AccessData.Data;
using Delab.Shared.Entities;

namespace Delab.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;

    public SeedDb(DataContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCountries();
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