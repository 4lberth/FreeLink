using FreeLink.Application.Contracts;
using FreeLink.Domain.Entities;
using FreeLink.Domain.Ports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FreeLink.Infrastructure.Services;

public class PdfService : IPdfService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISupabaseStorageService _supabaseStorage;

    public PdfService(IUnitOfWork unitOfWork, ISupabaseStorageService supabaseStorage)
    {
        _unitOfWork = unitOfWork;
        _supabaseStorage = supabaseStorage;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<string> GenerateContractPdfAsync(int contractId)
    {
        // 1. Load contract with all data
        var contract = await _unitOfWork.Repository<Contract>().GetById(contractId);
        if (contract == null) throw new Exception("Contract not found");

        var proposal = await _unitOfWork.Repository<Proposal>().GetById(contract.ProposalId);
        var project = await _unitOfWork.Repository<Project>().GetById(contract.ProjectId);
        var client = await _unitOfWork.Repository<User>().GetById(contract.ClientId);
        var freelancer = await _unitOfWork.Repository<User>().GetById(contract.FreelancerId);

        var clientProfile = await _unitOfWork.Repository<Userprofile>()
            .GetFirstOrDefaultAsync(p => p.UserId == contract.ClientId);
        var freelancerProfile = await _unitOfWork.Repository<Userprofile>()
            .GetFirstOrDefaultAsync(p => p.UserId == contract.FreelancerId);

        var costBreakdown = await _unitOfWork.Repository<Proposalcostbreakdown>()
            .GetAsync(c => c.ProposalId == proposal.ProposalId);
        var timeline = await _unitOfWork.Repository<Proposaltimeline>()
            .GetAsync(t => t.ProposalId == proposal.ProposalId);
        var deliverables = await _unitOfWork.Repository<Proposaldeliverable>()
            .GetAsync(d => d.ProposalId == proposal.ProposalId);

        var signatures = await _unitOfWork.Repository<Contractsignature>()
            .GetAsync(s => s.ContractId == contractId);

        // Pre-load signature user data
        var signatureData = new List<(User user, Userprofile? profile, DateTime signedAt, string? ip)>();
        foreach (var sig in signatures)
        {
            var signUser = await _unitOfWork.Repository<User>().GetById(sig.UserId);
            var signProfile = await _unitOfWork.Repository<Userprofile>()
                .GetFirstOrDefaultAsync(p => p.UserId == sig.UserId);
            signatureData.Add((signUser!, signProfile, sig.SignedAt, sig.IpAddress));
        }

        // 2. Generate PDF in memory
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Text($"CONTRATO DE SERVICIOS FREELANCE - #{contract.ContractId}")
                    .SemiBold().FontSize(16).FontColor(Colors.Blue.Darken2);

                page.Content().Column(column =>
                {
                    column.Spacing(15);

                    // Project Info
                    column.Item().Text(text =>
                    {
                        text.Span("PROYECTO: ").Bold();
                        text.Span(project.Title);
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("FECHA DE GENERACIÓN: ").Bold();
                        text.Span(contract.GeneratedAt.ToString("dd/MM/yyyy"));
                    });

                    // Parties
                    column.Item().PaddingTop(10).Text("PARTES DEL CONTRATO").Bold().FontSize(14);
                    
                    column.Item().Text(text =>
                    {
                        text.Span("CLIENTE: ").Bold();
                        text.Span(clientProfile != null 
                            ? $"{clientProfile.FirstName} {clientProfile.LastName}" 
                            : client.Email);
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("FREELANCER: ").Bold();
                        text.Span(freelancerProfile != null 
                            ? $"{freelancerProfile.FirstName} {freelancerProfile.LastName}" 
                            : freelancer.Email);
                    });

                    // Cost Breakdown
                    column.Item().PaddingTop(10).Text("DESGLOSE DE COSTOS").Bold().FontSize(14);
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Descripción").Bold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Monto").Bold();
                        });

                        foreach (var item in costBreakdown.OrderBy(c => c.ItemOrder))
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text(item.ItemDescription);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text($"${item.Amount:N2}");
                        }

                        table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("TOTAL").Bold();
                        table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text($"${contract.TotalAmount:N2}").Bold();
                    });

                    // Timeline
                    if (timeline.Any())
                    {
                        column.Item().PaddingTop(10).Text("CRONOGRAMA").Bold().FontSize(14);
                        foreach (var milestone in timeline.OrderBy(t => t.ItemOrder))
                        {
                            column.Item().Text(text =>
                            {
                                text.Span($"• {milestone.MilestoneName}").Bold();
                                text.Span($" ({milestone.EstimatedDuration} días)");
                                if (!string.IsNullOrEmpty(milestone.Description))
                                    text.Span($" - {milestone.Description}");
                            });
                        }
                    }

                    // Deliverables
                    if (deliverables.Any())
                    {
                        column.Item().PaddingTop(10).Text("ENTREGABLES").Bold().FontSize(14);
                        foreach (var item in deliverables.OrderBy(d => d.ItemOrder))
                        {
                            column.Item().Text($"• {item.DeliverableName}");
                        }
                    }

                    // Signatures
                    column.Item().PaddingTop(20).Text("FIRMAS DIGITALES").Bold().FontSize(14);
                    foreach (var sigData in signatureData.OrderBy(s => s.signedAt))
                    {
                        var name = sigData.profile != null 
                            ? $"{sigData.profile.FirstName} {sigData.profile.LastName}" 
                            : sigData.user?.Email ?? "Unknown";

                        column.Item().Text(text =>
                        {
                            text.Span(sigData.user?.UserType == "Cliente" ? "CLIENTE: " : "FREELANCER: ").Bold();
                            text.Span($"{name}\n");
                            text.Span($"Firmado el: {sigData.signedAt:dd/MM/yyyy HH:mm}\n");
                            text.Span($"IP: {sigData.ip}").FontSize(9);
                        });
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
            });
        });

        // 3. Upload PDF to Supabase Storage
        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        var pdfBytes = stream.ToArray();

        var fileName = $"contract_{contract.ContractId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
        var publicUrl = await _supabaseStorage.UploadPdfAsync(pdfBytes, fileName);

        return publicUrl;
    }
}