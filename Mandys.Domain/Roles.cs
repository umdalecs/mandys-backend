namespace Mandys.Domain;

public static class Roles
{
    public const string Administrator = "administrador";
    public const string OpChief = "gerenteOperaciones";
    public const string CentralWarehouseChief = "encargadoAlmCentral";
    public const string WarehouseChief = "encargadoAlmacen";
    public const string Cashier = "cajero";
    public const string Customer = "cliente";
    public const string KitchenChief = "jefeCocina";
    public const string BranchChief = "gerenteSucursal";

    public static readonly IReadOnlyList<string> All = [
        Administrator, OpChief, CentralWarehouseChief, WarehouseChief, Cashier, Customer, KitchenChief, BranchChief];

    public static bool IsValid(string? role) =>
        !string.IsNullOrWhiteSpace(role) && All.Contains(role, StringComparer.OrdinalIgnoreCase);

    public static string Normalize(string role)
    {
        if (string.Equals(role, Administrator, StringComparison.OrdinalIgnoreCase)) return OpChief;
        if (string.Equals(role, OpChief, StringComparison.OrdinalIgnoreCase)) return OpChief;
        if (string.Equals(role, CentralWarehouseChief, StringComparison.OrdinalIgnoreCase)) return OpChief;
        if (string.Equals(role, WarehouseChief, StringComparison.OrdinalIgnoreCase)) return OpChief;
        if (string.Equals(role, Cashier, StringComparison.OrdinalIgnoreCase)) return OpChief;
        if (string.Equals(role, Customer, StringComparison.OrdinalIgnoreCase)) return OpChief;
        if (string.Equals(role, KitchenChief, StringComparison.OrdinalIgnoreCase)) return OpChief;
        if (string.Equals(role, BranchChief, StringComparison.OrdinalIgnoreCase)) return OpChief;
        return role;
    }
}
