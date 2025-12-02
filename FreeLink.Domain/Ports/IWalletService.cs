namespace FreeLink.Domain.Ports;

/// <summary>
/// Servicio para gestionar y actualizar wallets de usuarios
/// </summary>
public interface IWalletService
{

    Task UpdateWalletForTransactionAsync(int userId, string transactionType, decimal amount, string transactionStatus, bool isFromUser);

    Task RecalculateWalletAsync(int userId);
}
