using Microsoft.AspNetCore.Identity;
using DenunciaDo.Domain.Entities;
using DenunciaDo.Core.Domain.Entities;

namespace DenunciaDo.Infrastructure.Services
{
    public static class PasswordHashingService
    {
        public static string HashPassword(string password)
        {
            var hasher = new PasswordHasher<User>();
            var user = new User(); // Solo para el contexto del hasher
            return hasher.HashPassword(user, password);
        }

        // Método para generar los hashes que van en el seed data
        public static void GenerateHashesForSeedData()
        {
            var adminHash = HashPassword("Admin123!");
            var userHash = HashPassword("User123!");

            Console.WriteLine($"Admin Hash: {adminHash}");
            Console.WriteLine($"User Hash: {userHash}");
        }
    }
}
