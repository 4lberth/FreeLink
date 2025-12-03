using FreeLink.Domain.Ports;
using FreeLink.Domain.Entities;
using MediatR;

namespace FreeLink.Application.UseCase.Payments.Commands.GeneratePaymentReceipt;

public class GeneratePaymentReceiptHandler : IRequestHandler<GeneratePaymentReceiptCommand, GeneratePaymentReceiptResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPdfService _pdfService;
    private readonly ISupabaseStorageService _storageService;

    public GeneratePaymentReceiptHandler(IUnitOfWork unitOfWork, IPdfService pdfService, ISupabaseStorageService storageService)
    {
        _unitOfWork = unitOfWork;
        _pdfService = pdfService;
        _storageService = storageService;
    }

    public async Task<GeneratePaymentReceiptResponse> Handle(GeneratePaymentReceiptCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Obtener transacción
            var transaction = await _unitOfWork.Repository<Transaction>().GetById(request.TransactionId);
            if (transaction == null || transaction.TransactionType != "Liberación")
                return new GeneratePaymentReceiptResponse { Success = false, Message = "Transacción inválida" };

            // 2. Verificar que la transacción tenga escrow asociado
            if (!transaction.EscrowId.HasValue)
                return new GeneratePaymentReceiptResponse { Success = false, Message = "Transacción sin escrow asociado" };

            var escrow = await _unitOfWork.Repository<Escrowaccount>().GetById(transaction.EscrowId.Value);
            if (escrow == null)
                return new GeneratePaymentReceiptResponse { Success = false, Message = "Escrow no encontrado" };

            // 3. Verificar permisos (solo cliente o freelancer pueden ver el recibo)
            if (request.RequestingUserId != transaction.FromUserId && request.RequestingUserId != transaction.ToUserId)
                return new GeneratePaymentReceiptResponse { Success = false, Message = "No autorizado" };

            // 4. Si ya existe el recibo, devolverlo
            if (!string.IsNullOrWhiteSpace(transaction.ReceiptUrl))
            {
                return new GeneratePaymentReceiptResponse
                {
                    Success = true,
                    Message = "Recibo ya existe",
                    ReceiptUrl = transaction.ReceiptUrl
                };
            }

            // 5. Generar el HTML del recibo
            var html = GenerateReceiptHtml(transaction, escrow);

            // 6. Generar PDF
            var pdfBytes = _pdfService.GeneratePdfFromHtml(html);

            // 7. Subir PDF a Supabase en el bucket payment-receipts
            var fileName = $"receipt_{transaction.TransactionId}.pdf";
            var folder = $"project_{escrow.ProjectId}";

            var receiptUrl = await _storageService.UploadPdfAsync(pdfBytes, fileName, "payment-receipts", folder);

            // 8. Actualizar la transacción con la URL del recibo
            transaction.ReceiptUrl = receiptUrl;
            await _unitOfWork.Repository<Transaction>().Update(transaction);
            await _unitOfWork.Complete();

            return new GeneratePaymentReceiptResponse
            {
                Success = true,
                Message = "Recibo generado exitosamente",
                ReceiptUrl = receiptUrl,
                PdfBytes = pdfBytes
            };
        }
        catch (Exception ex)
        {
            return new GeneratePaymentReceiptResponse
            {
                Success = false,
                Message = $"Error al generar recibo: {ex.Message}"
            };
        }
    }

    private string GenerateReceiptHtml(Transaction transaction, Escrowaccount escrow)
    {
        return $@"
<html>
<head>
    <style>
        body {{ font-family: Arial; margin: 20px; }}
        h1 {{ color: #2196F3; }}
        table {{ width: 100%; border-collapse: collapse; }}
        td {{ padding: 8px; border-bottom: 1px solid #ddd; }}
        .label {{ font-weight: bold; }}
    </style>
</head>
<body>
    <h1>Comprobante de Pago - FreeLink</h1>
    <p><strong>DOCUMENTO NO FISCAL - SOLO PARA CONTROL INTERNO</strong></p>
    
    <table>
        <tr>
            <td class='label'>ID Transacción:</td>
            <td>{transaction.TransactionId}</td>
        </tr>
        <tr>
            <td class='label'>Monto:</td>
            <td>${transaction.Amount}</td>
        </tr>
        <tr>
            <td class='label'>Tipo:</td>
            <td>{transaction.TransactionType}</td>
        </tr>
        <tr>
            <td class='label'>Estado:</td>
            <td>{transaction.TransactionStatus}</td>
        </tr>
        <tr>
            <td class='label'>Fecha:</td>
            <td>{transaction.CreatedAt}</td>
        </tr>
        <tr>
            <td class='label'>ID Escrow:</td>
            <td>{escrow.EscrowId}</td>
        </tr>
        <tr>
            <td class='label'>Proyecto:</td>
            <td>{escrow.ProjectId}</td>
        </tr>
    </table>
    
    <p>Este documento es un comprobante interno de la transacción realizada en la plataforma FreeLink.</p>
</body>
</html>";
    }
}
