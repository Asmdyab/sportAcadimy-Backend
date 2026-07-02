using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;
using SportAcademy.Infrastructure.Persistence.DBContext;

namespace SportAcademy.Web
{
    public class AspUsersSeeder
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AspUsersSeeder> _logger;

        private const string AdminEmail = "admin@sportacademy.com";
        private const string AdminPassword = "Admin@123";

        public AspUsersSeeder(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AspUsersSeeder> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task AddRulesAsync()
        {
            try
            {
                _logger.LogInformation("Starting role seeding process...");
                var roles = new[] { "Admin", "Manager", "Trainee", "Coach" };
                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        var result = await _roleManager.CreateAsync(new IdentityRole(role));
                        if (result.Succeeded)
                        {
                            _logger.LogInformation($"Successfully created role: {role}");
                        }
                        else
                        {
                            _logger.LogError($"Failed to create role {role}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Role already exists: {role}", role);
                    }
                }
                _logger.LogInformation("Role seeding completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during role seeding");
                throw;
            }
        }

        public async Task SeedAdminAsync()
        {
            try
            {
                _logger.LogInformation("Starting admin seeding...");

                if (await _userManager.FindByEmailAsync(AdminEmail) != null)
                {
                    _logger.LogInformation("Admin user already exists.");
                    return;
                }

                var adminUser = new AppUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    IsBanned = false
                };

                var result = await _userManager.CreateAsync(adminUser, AdminPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation("Successfully created admin user: {Email}", AdminEmail);
                }
                else
                {
                    _logger.LogError("Failed to create admin user. Errors: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during admin seeding");
                throw;
            }
        }

        public async Task AssignRolesAsync(ApplicationDbContext context)
        {
            try
            {
                _logger.LogInformation("Starting role assignment for all users...");

                var coachUserIds = await context.Employees
                    .Where(e => e.AppUserId != null && e.Position == Position.Coach)
                    .Select(e => e.AppUserId!)
                    .ToListAsync();

                var managerUserIds = await context.Employees
                    .Where(e => e.AppUserId != null && e.Position != Position.Coach)
                    .Select(e => e.AppUserId!)
                    .ToListAsync();

                var traineeUserIds = await context.Trainees
                    .Where(t => t.AppUserId != null)
                    .Select(t => t.AppUserId!)
                    .ToListAsync();

                var coachSet = coachUserIds.ToHashSet();
                var managerSet = managerUserIds.ToHashSet();
                var traineeSet = traineeUserIds.ToHashSet();

                var users = _userManager.Users.ToList();

                if (users.Count == 0)
                {
                    _logger.LogInformation("No users found to assign roles.");
                    return;
                }

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Count > 0)
                    {
                        _logger.LogDebug("User {Email} already has roles: {Roles}", user.Email, string.Join(", ", roles));
                        continue;
                    }

                    if (coachSet.Contains(user.Id))
                    {
                        await _userManager.AddToRoleAsync(user, "Coach");
                        _logger.LogInformation("Assigned Coach role to {Email}", user.Email);
                    }
                    else if (managerSet.Contains(user.Id))
                    {
                        await _userManager.AddToRoleAsync(user, "Manager");
                        _logger.LogInformation("Assigned Manager role to {Email}", user.Email);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Trainee");
                        _logger.LogInformation("Assigned Trainee role to {Email}", user.Email);
                    }
                }

                _logger.LogInformation("Role assignment completed for all users.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during role assignment");
                throw;
            }
        }

        public async Task SeedUsersAsync()
        {
            try
            {
                _logger.LogInformation("Starting user seeding process...");

                // Check if users already exist to avoid duplicates
                var existingUsersCount = _userManager.Users.Count();
                if (existingUsersCount >= 100)
                {
                    _logger.LogInformation($"Users already seeded. Current count: {existingUsersCount}");
                    return;
                }

                var random = new Random();
                var usersToCreate = 100 - existingUsersCount;

                for (int i = 1; i <= usersToCreate; i++)
                {
                    var user = new AppUser
                    {
                        UserName = $"user{(i + 50):D3}@example.com",
                        Email = $"user{(i + 50):D3}@example.com",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        TwoFactorEnabled = false,
                        LockoutEnabled = true,
                        AccessFailedCount = 0,
                        IsBanned = random.Next(0, 10) == 0 // 10% chance of being banned
                    };

                    // Create user with default password
                    var result = await _userManager.CreateAsync(user, "TempPassword123!");

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Successfully created user: {user.UserName}");
                    }
                    else
                    {
                        _logger.LogError($"Failed to create user {user.UserName}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }

                _logger.LogInformation($"User seeding completed. Total users in database: {_userManager.Users.Count()}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user seeding");
                throw;
            }
        }

        // Alternative method with more realistic data
        public async Task SeedUsersWithRealisticDataAsync()
        {
            try
            {
                _logger.LogInformation("Starting realistic user seeding process...");

                var existingUsersCount = _userManager.Users.Count();
                if (existingUsersCount >= 100)
                {
                    _logger.LogInformation($"Users already seeded. Current count: {existingUsersCount}");
                    return;
                }

                var firstNames = new List<string>
            {
                "John", "Jane", "Michael", "Sarah", "David", "Lisa", "Robert", "Emily",
                "James", "Ashley", "William", "Jessica", "Richard", "Amanda", "Thomas",
                "Jennifer", "Charles", "Michelle", "Christopher", "Melissa", "Daniel",
                "Kimberly", "Matthew", "Donna", "Anthony", "Carol", "Mark", "Ruth",
                "Donald", "Sharon", "Steven", "Laura", "Paul", "Sandra", "Andrew",
                "Cynthia", "Kenneth", "Kathleen", "Joshua", "Amy", "Kevin", "Angela",
                "Brian", "Helen", "George", "Deborah", "Timothy", "Rachel", "Ronald", "Carolyn"
            };

                var lastNames = new List<string>
            {
                "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller",
                "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez",
                "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
                "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark",
                "Ramirez", "Lewis", "Robinson", "Walker", "Young", "Allen", "King",
                "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores", "Green",
                "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts"
            };

                var random = new Random();
                var usersToCreate = 100 - existingUsersCount;

                for (int i = 0; i < usersToCreate; i++)
                {
                    var firstName = firstNames[random.Next(firstNames.Count)];
                    var lastName = lastNames[random.Next(lastNames.Count)];
                    var username = $"{firstName.ToLower()}.{lastName.ToLower()}{random.Next(100, 999)}";
                    var email = $"{username}@example.com";

                    var user = new AppUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = random.Next(0, 10) > 1, // 90% confirmed
                        PhoneNumber = GeneratePhoneNumber(random),
                        PhoneNumberConfirmed = random.Next(0, 10) > 3, // 70% confirmed
                        TwoFactorEnabled = random.Next(0, 10) > 7, // 30% enabled
                        LockoutEnabled = true,
                        AccessFailedCount = random.Next(0, 3),
                        IsBanned = random.Next(0, 20) == 0 // 5% chance of being banned
                    };

                    var result = await _userManager.CreateAsync(user, "DefaultPassword123!");

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Successfully created user: {user.UserName}");
                    }
                    else
                    {
                        _logger.LogError($"Failed to create user {user.UserName}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }

                _logger.LogInformation($"Realistic user seeding completed. Total users: {_userManager.Users.Count()}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during realistic user seeding");
                throw;
            }
        }

        private string GeneratePhoneNumber(Random random)
        {
            // Generate a Kuwait phone number format: XXXX XXXX
            // Kuwait mobile numbers start with 5, 6, 9 for mobiles
            // Landline numbers start with 2 for Kuwait City area

            var prefixes = new[] { 5, 6, 9 }; // Mobile prefixes in Kuwait
            var selectedPrefix = prefixes[random.Next(prefixes.Length)];

            // Generate 7 more digits after the prefix
            var remainingDigits = random.Next(1000000, 9999999);

            return $"{selectedPrefix}{remainingDigits:D7}";
        }
    }
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUserSeeder(this IServiceCollection services)
        {
            services.AddScoped<AspUsersSeeder>();
            return services;
        }
    }
    public static class DatabaseInitializer
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<AspUsersSeeder>();

            await seeder.AddRulesAsync();
            await seeder.SeedAdminAsync();
            // Use either method based on your preference
            await seeder.SeedUsersAsync(); // Simple seeding
                                           // OR
                                           // await seeder.SeedUsersWithRealisticDataAsync(); // More realistic data
        }

        public static async Task AssignRoles(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            using var scope = serviceProvider.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<AspUsersSeeder>();

            await seeder.AssignRolesAsync(context);
        }
    }
}
