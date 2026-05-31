namespace Generic.Application.Utils.Services;

public sealed class SentinelAuthClientOptions
{
    public long ApplicationClientId { get; init; }
    public string ClientId { get; init; } = string.Empty;
    public long AdminRoleId { get; init; }
    public long SellerRoleId { get; init; }
    public long ClientRoleId { get; init; }

    public long GetRoleId(int legacyRole)
    {
        return legacyRole switch
        {
            0 => AdminRoleId,
            1 => SellerRoleId,
            2 => ClientRoleId,
            _ => 0
        };
    }
}
