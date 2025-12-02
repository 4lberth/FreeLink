using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using MediatR;

namespace FreeLink.Application.UseCase.Contracts.Commands.SignContract;

public class SignContractHandler : IRequestHandler<SignContractCommand, SignContractResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly IPdfService _pdfService;

    public SignContractHandler(IUnitOfWork unitOfWork, INotificationService notificationService, IPdfService pdfService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _pdfService = pdfService;
    }

    public async Task<SignContractResponse> Handle(SignContractCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contract = await _unitOfWork.Repository<Contract>().GetById(request.ContractId);
            if (contract == null)
            {
                return new SignContractResponse { Success = false, Message = "Contrato no encontrado" };
            }

            // Verify user is authorized to sign
            bool isClient = contract.ClientId == request.UserId;
            bool isFreelancer = contract.FreelancerId == request.UserId;

            if (!isClient && !isFreelancer)
            {
                return new SignContractResponse { Success = false, Message = "No tienes permiso para firmar este contrato" };
            }

            // Check if already signed by this user
            var existingSignature = await _unitOfWork.Repository<Contractsignature>()
                .GetFirstOrDefaultAsync(s => s.ContractId == request.ContractId && s.UserId == request.UserId);
            
            if (existingSignature != null)
            {
                return new SignContractResponse { Success = false, Message = "Ya has firmado este contrato" };
            }

            // Create signature
            var signature = new Contractsignature
            {
                ContractId = request.ContractId,
                UserId = request.UserId,
                SignedAt = DateTime.UtcNow,
                IpAddress = request.IpAddress
            };

            await _unitOfWork.Repository<Contractsignature>().Add(signature);

            // Update contract
            if (isClient)
                contract.ClientSignedAt = DateTime.UtcNow;
            else
                contract.FreelancerSignedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Contract>().Update(contract);
            await _unitOfWork.Complete();

            // Check if both signed
            bool bothSigned = contract.ClientSignedAt.HasValue && contract.FreelancerSignedAt.HasValue;
            string? pdfUrl = null;

            if (bothSigned)
            {
                contract.ContractStatus = "Firmado";

                // ✅ Generate PDF with signatures and save URL
                try
                {
                    pdfUrl = await _pdfService.GenerateContractPdfAsync(request.ContractId);
                    contract.ContractPdfUrl = pdfUrl;  // Save URL to database
                }
                catch (Exception ex)
                {
                    // Log error but don't fail the signing process
                    Console.WriteLine($"Error generando PDF firmado: {ex.Message}");
                    pdfUrl = null;
                }

                _unitOfWork.Repository<Contract>().Update(contract);
                await _unitOfWork.Complete();

                // Notify both parties
                var notificationList = new List<int> { contract.ClientId, contract.FreelancerId };
                await _notificationService.CreateMultipleNotificationsAsync(
                    notificationList,
                    "Contract",
                    "Contrato completamente firmado",
                    "El contrato ha sido firmado por ambas partes. Puedes descargarlo ahora.",
                    "Contract",
                    request.ContractId
                );

                await _notificationService.LogProjectActivityAsync(
                    contract.ProjectId,
                    request.UserId,
                    "ContractFullySigned",
                    "Contrato firmado por ambas partes. Listo para descargar."
                );
            }
            else
            {
                // Notify other party
                int otherPartyId = isClient ? contract.FreelancerId : contract.ClientId;
                string role = isClient ? "cliente" : "freelancer";

                await _notificationService.CreateNotificationAsync(
                    userId: otherPartyId,
                    type: "Contract",
                    title: "Contrato firmado",
                    message: $"El {role} ha firmado el contrato. Ahora es tu turno de firmarlo.",
                    resourceType: "Contract",
                    resourceId: request.ContractId
                );

                await _notificationService.LogProjectActivityAsync(
                    contract.ProjectId,
                    request.UserId,
                    "ContractSigned",
                    $"Contrato firmado por {(isClient ? "el cliente" : "el freelancer")}"
                );
            }

            return new SignContractResponse
            {
                Success = true,
                Message = bothSigned ? "Contrato firmado. Ambas partes han firmado." : "Firma registrada exitosamente",
                BothSigned = bothSigned,
                PdfUrl = pdfUrl
            };
        }
        catch (Exception ex)
        {
            return new SignContractResponse { Success = false, Message = $"Error: {ex.Message}" };
        }
    }
}