using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;

namespace FreeLink.Infrastructure.Services;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;

    public WalletService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task UpdateWalletForTransactionAsync(int userId, string transactionType,
        decimal amount, string transactionStatus, bool isFromUser)
    {
        // Solo actualizar para transacciones completadas
        if (transactionStatus != "Completada")
        {
            return;
        }

        // Obtener o crear wallet del usuario
        var wallets = await _unitOfWork.Repository<Userwallet>()
            .GetAsync(w => w.UserId == userId);
        var wallet = wallets.FirstOrDefault();

        if (wallet == null)
        {
            // Si no existe wallet, crearlo
            wallet = new Userwallet
            {
                UserId = userId,
                Balance = 0,
                PendingBalance = 0,
                TotalEarnings = 0,
                TotalSpent = 0,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Userwallet>().Add(wallet);
        }

        // Actualizar según tipo de transacción
        switch (transactionType)
        {
            case "Depósito":
                // Usuario deposita dinero en escrow (FromUserId)
                if (isFromUser)
                {
                    wallet.Balance -= amount;  // Dinero bloqueado en escrow
                    wallet.TotalSpent += amount;
                }
                break;

            case "Liberación":
                // Usuario recibe pago (ToUserId)
                if (!isFromUser)
                {
                    wallet.Balance += amount;  // Dinero recibido
                    wallet.TotalEarnings += amount;
                }
                break;

            case "Retiro":
                // Usuario retira dinero (FromUserId)
                if (isFromUser)
                {
                    wallet.Balance -= amount;  // Dinero retirado
                    // TotalSpent no cambia para retiros
                }
                break;

            case "Comisión":
                // Comisión de plataforma - no afecta wallets de usuarios
                break;
        }

        wallet.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Complete();
    }

    public async Task RecalculateWalletAsync(int userId)
    {
        // Obtener todas las transacciones del usuario
        var allTransactions = await _unitOfWork.Repository<Transaction>()
            .GetAsync(t => t.FromUserId == userId || t.ToUserId == userId);

        // Obtener o crear wallet
        var wallets = await _unitOfWork.Repository<Userwallet>()
            .GetAsync(w => w.UserId == userId);
        var wallet = wallets.FirstOrDefault();

        if (wallet == null)
        {
            wallet = new Userwallet
            {
                UserId = userId,
                Balance = 0,
                PendingBalance = 0,
                TotalEarnings = 0,
                TotalSpent = 0,
                UpdatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Userwallet>().Add(wallet);
        }

        // Resetear valores
        decimal balance = 0;
        decimal totalEarnings = 0;
        decimal totalSpent = 0;
        decimal pendingBalance = 0;

        // Recalcular desde todas las transacciones
        foreach (var transaction in allTransactions)
        {
            if (transaction.TransactionStatus == "Completada")
            {
                // Liberación: dinero recibido
                if (transaction.ToUserId == userId && transaction.TransactionType == "Liberación")
                {
                    balance += transaction.Amount;
                    totalEarnings += transaction.Amount;
                }

                // Depósito: dinero enviado a escrow
                if (transaction.FromUserId == userId && transaction.TransactionType == "Depósito")
                {
                    balance -= transaction.Amount;
                    totalSpent += transaction.Amount;
                }

                // Retiro completado: dinero retirado
                if (transaction.FromUserId == userId && transaction.TransactionType == "Retiro")
                {
                    balance -= transaction.Amount;
                }
            }

            // Retiros pendientes
            if (transaction.TransactionStatus == "Pendiente" &&
                transaction.FromUserId == userId &&
                transaction.TransactionType == "Retiro")
            {
                pendingBalance += transaction.Amount;
            }
        }

        // Actualizar wallet
        wallet.Balance = balance;
        wallet.TotalEarnings = totalEarnings;
        wallet.TotalSpent = totalSpent;
        wallet.PendingBalance = pendingBalance;
        wallet.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Complete(); 
    }
}