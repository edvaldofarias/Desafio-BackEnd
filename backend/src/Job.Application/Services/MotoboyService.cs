using FluentValidation.Results;
using Job.Application.Commands.Motoboy;
using Job.Application.Commands.Motoboy.Validations;
using Job.Application.Dtos.Motoboy;
using Job.Application.Repositories;
using Job.Domain.Entities.User;
using Job.Domain.Enums;

namespace Job.Application.Services;

public sealed class MotoboyService(
    ILogger<MotoboyService> logger,
    IMotoboyRepository motoboyRepository,
    IFileStorageService fileStorageService) :
    IRequestHandler<AuthenticationMotoboyCommand, Result<MotoboyDto>>,
    IRequestHandler<CreateMotoboyCommand, Result>,
    IRequestHandler<UploadCnhMotoboyCommand, Result>
{
    private const int WorkFactor = 12;

    public async Task<Result<MotoboyDto>> Handle(AuthenticationMotoboyCommand request,
        CancellationToken cancellationToken)
    {
        var validate = await new AuthenticationMotoboyValidation().ValidateAsync(request, cancellationToken);

        if (!validate.IsValid)
            return Result.Fail(validate.Errors.Select(x => x.ErrorMessage));

        var cnpj = CnpjValidation.FormatCnpj(request.Cnpj);
        var motoboy = await motoboyRepository.GetByCnpjAsync(cnpj, cancellationToken);

        if (motoboy is not null && BCrypt.Net.BCrypt.Verify(request.Password, motoboy.Password))
        {
            var query = new MotoboyDto(motoboy.Identifier, motoboy.Cnpj);
            return Result.Ok(query);
        }

        return Result.Ok();
    }

    public async Task<Result> Handle(CreateMotoboyCommand request, CancellationToken cancellationToken)
    {
        var validate = await new CreateMotoboyValidation().ValidateAsync(request, cancellationToken);
        if (!validate.IsValid)
            return Result.Fail(validate.Errors.Select(x => x.ErrorMessage));

        if (!TryParseCnh(request.TypeCnh, out var typeCnh))
            return Result.Fail("Tipo de cnh inválido");

        var password = BCrypt.Net.BCrypt.HashPassword(request.Password, WorkFactor);
        var motoboyEntity = new MotoboyEntity(
            string.IsNullOrWhiteSpace(request.Identifier) ? Guid.NewGuid().ToString("N") : request.Identifier,
            password,
            request.Name,
            request.Cnpj,
            DateOnly.FromDateTime(request.DateBirth),
            request.Cnh,
            typeCnh);

        var failures = new List<string>();
        if (await motoboyRepository.CheckIdentifierExistsAsync(motoboyEntity.Identifier, cancellationToken))
            failures.Add("Identificador já cadastrado");
        if (await motoboyRepository.CheckCnpjExistsAsync(motoboyEntity.Cnpj, cancellationToken))
            failures.Add("CNPJ já cadastrado");
        if (await motoboyRepository.CheckCnhExistsAsync(motoboyEntity.Cnh, cancellationToken))
            failures.Add("CNH já cadastrada");

        if (failures.Count > 0)
            return Result.Fail(failures);

        if (!string.IsNullOrWhiteSpace(request.CnhImage))
        {
            var savedPath = await SaveBase64ImageAsync(request.CnhImage, cancellationToken);
            if (savedPath.IsFailed)
                return Result.Fail(savedPath.Errors);
            motoboyEntity.UpdateCnhImage(savedPath.Value);
        }

        await motoboyRepository.CreateAsync(motoboyEntity, cancellationToken);
        logger.LogInformation("Motoboy {Identifier} criado", motoboyEntity.Identifier);
        return Result.Ok();
    }

    public async Task<Result> Handle(UploadCnhMotoboyCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CnhImage))
            return Result.Fail("Imagem da CNH é obrigatória");

        var motoboy = await motoboyRepository.GetByIdentifierAsync(request.Identifier, cancellationToken);
        if (motoboy is null)
            return Result.Fail("Entregador não encontrado");

        var saved = await SaveBase64ImageAsync(request.CnhImage, cancellationToken);
        if (saved.IsFailed)
            return Result.Fail(saved.Errors);

        motoboy.UpdateCnhImage(saved.Value);
        await motoboyRepository.UpdateAsync(motoboy, cancellationToken);
        return Result.Ok();
    }

    private async Task<Result<string>> SaveBase64ImageAsync(string base64, CancellationToken cancellationToken)
    {
        try
        {
            var (extension, bytes) = DecodeBase64Image(base64);
            if (extension is null)
                return Result.Fail("Tipo de arquivo inválido (apenas png ou bmp)");

            using var stream = new MemoryStream(bytes);
            var path = await fileStorageService.SaveAsync($"cnh{extension}", stream, cancellationToken);
            return Result.Ok(path);
        }
        catch (FormatException)
        {
            return Result.Fail("Imagem inválida");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao salvar imagem da CNH");
            return Result.Fail("Erro ao salvar imagem da CNH");
        }
    }

    private static (string? extension, byte[] bytes) DecodeBase64Image(string input)
    {
        var payload = input;
        string? extension = null;

        const string dataPrefix = "data:";
        if (payload.StartsWith(dataPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var commaIndex = payload.IndexOf(',');
            if (commaIndex < 0) throw new FormatException();
            var meta = payload[..commaIndex];
            payload = payload[(commaIndex + 1)..];

            if (meta.Contains("image/png", StringComparison.OrdinalIgnoreCase)) extension = ".png";
            else if (meta.Contains("image/bmp", StringComparison.OrdinalIgnoreCase)) extension = ".bmp";
            else return (null, Array.Empty<byte>());
        }

        var bytes = Convert.FromBase64String(payload);

        if (extension is null)
        {
            if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                extension = ".png";
            else if (bytes.Length >= 2 && bytes[0] == 0x42 && bytes[1] == 0x4D)
                extension = ".bmp";
            else
                return (null, bytes);
        }

        return (extension, bytes);
    }

    private static bool TryParseCnh(string value, out ECnhType typeCnh)
    {
        switch (value?.Trim().ToUpperInvariant())
        {
            case "A": typeCnh = ECnhType.A; return true;
            case "B": typeCnh = ECnhType.B; return true;
            case "AB":
            case "A+B":
                typeCnh = ECnhType.AB; return true;
            default: typeCnh = default; return false;
        }
    }
}