using System.Security.Cryptography;

namespace Auth.Application.Utils.Services;

public static class PasswordHash
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public static string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return string.Empty;
        }

        var salt = RandomNumberGenerator.GetBytes(SaltSize);  // Salt aleatorio para a mesma senha gerar hashes diferentes, evitar ataques de rainbow tables
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize); // Gerar a hash usando PBKDF2 com HMAC-SHA256, o número de iterações e o tamanho da chave definidos

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}"; // Armazenar o número de iterações, o salt e a hash em um formato legível.
                                                                                             // Salt salvo somente para verificar a senha posteriormente.
                                                                                             // O número de iterações é salvo para permitir ajustes futuros sem invalidar hashes antigos.
    }

    public static bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        var parts = passwordHash.Split('.', 3); // Dividir a string hash em partes: iterações, salt e hash.
                                                // O número máximo de divisões é 3 para garantir que o salt e a hash possam conter pontos.
        if (parts.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parts[0], out var iterations) || iterations <= 0)
        {
            return false;
        }

        byte[] salt;
        byte[] key;
        try
        {
            salt = Convert.FromBase64String(parts[1]);
            key = Convert.FromBase64String(parts[2]);
        }
        catch
        {
            return false;
        }

        var keyToCheck = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, key.Length); // Gerar a hash da senha fornecida usando o mesmo salt e número de iterações para comparação.
        return CryptographicOperations.FixedTimeEquals(keyToCheck, key); // Comparar as hashes usando uma comparação de tempo fixo para evitar ataques de timing.
    }
}
