using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FluentValidation.Results;
using Job.Application.Commands.Motoboy;
using Job.Application.Commands.Motoboy.Validations;
using Job.Application.Dtos.Motoboy;
using Job.Application.Repositories;
using Job.Domain.Entities.User;

namespace Job.Application.Services;

public sealed class MotoboyService(ILogger<MotoboyService> logger, IMotoboyRepository motoboyRepository) :
    IRequestHandler<AuthenticationMotoboyCommand, Result<MotoboyDto>>,
    IRequestHandler<CreateMotoboyCommand, Result>,
    IRequestHandler<UploadCnhMotoboyCommand, Result>
{
    private const int WorkFactor = 12;

    public async Task<Result<MotoboyDto>> Handle(AuthenticationMotoboyCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Buscando motoboy {cnpj}", request.Cnpj);
        var validate = await new AuthenticationMotoboyValidation().ValidateAsync(request, cancellationToken);

        if (!validate.IsValid)
            return Result.Fail(validate.Errors.Select(x => x.ErrorMessage));

        var cnpj = CnpjValidation.FormatCnpj(request.Cnpj);
        var password = BCrypt.Net.BCrypt.HashPassword(request.Password, WorkFactor);
        var motoboy = await motoboyRepository.GetAsync(cnpj, password, cancellationToken);

        if (motoboy is null)
        {
            logger.LogError("Motoboy não encontrado");
            return Result.Ok();
        }

        logger.LogInformation("Motoboy encontrado com sucesso");
        var query = new MotoboyDto(motoboy.Id, motoboy.Cnpj);
        return Result.Ok(query);
    }

    public async Task<Result> Handle(CreateMotoboyCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando o processo de criação de um motoboy");
        var validate = await new CreateMotoboyValidation().ValidateAsync(request, cancellationToken);

        logger.LogInformation("Criando objeto motoboy");
        var password = BCrypt.Net.BCrypt.HashPassword(request.Password, WorkFactor);
        var motoboyEntity = new MotoboyEntity(password, request.Name, request.Cnpj,
            DateOnly.FromDateTime(request.DateBirth), request.Cnh, request.TypeCnh);

        await CheckDocumentAsync(validate, motoboyEntity, cancellationToken);

        if (!validate.IsValid)
        {
            logger.LogInformation("Erros de validação encontrados {errors}", validate.Errors);
            return Result.Fail(validate.Errors.Select(x => x.ErrorMessage));
        }

        await motoboyRepository.CreateAsync(motoboyEntity, cancellationToken);

        logger.LogInformation("Motoboy criado com sucesso");
        return Result.Ok();
    }

    public async Task<Result> Handle(UploadCnhMotoboyCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando upload de imagem");

        var validationFailures = new List<ValidationFailure>();
        var permittedExtensions = new[] {".png", ".bmp"};
        var extension = Path.GetExtension(request.FileDetails.FileName).ToLowerInvariant();
        var permittedMimeTypes = new[] {"image/png", "image/bmp"};
        if (!permittedMimeTypes.Contains(request.FileDetails.ContentType) && !permittedExtensions.Contains(extension))
        {
            logger.LogInformation("Tipo de arquivo inválido");
            validationFailures.Add(new ValidationFailure("File", "Tipo de arquivo inválido"));
            return Result.Fail(validationFailures.Select(x => x.ErrorMessage));
        }

        logger.LogInformation("Buscando motoboy {cnpj}", request.Cnpj);
        var motoboy = await motoboyRepository.GetByCnpjAsync(request.Cnpj, cancellationToken);

        if (motoboy is null)
        {
            logger.LogError("Motoboy não encontrado");
            validationFailures.Add(new ValidationFailure("Cnpj", "Motoboy não encontrado"));
            return Result.Fail(validationFailures.Select(x => x.ErrorMessage));
        }

        var stream = request.FileDetails.OpenReadStream();
        var path = await UploadImage(request.FileDetails.FileName, stream, cancellationToken);

        if (path is null)
        {
            logger.LogError("Erro ao realizar upload de imagem");
            validationFailures.Add(new ValidationFailure("File", "Erro ao realizar upload de imagem"));
            return Result.Fail(validationFailures.Select(x => x.ErrorMessage));
        }

        motoboy.UpdateCnhImage(path);
        await motoboyRepository.UpdateAsync(motoboy, cancellationToken);

        logger.LogInformation("Upload de imagem realizado com sucesso");
        return Result.Ok();
    }

    private async Task CheckDocumentAsync(ValidationResult validate, MotoboyEntity motoboyEntity,
        CancellationToken cancellationToken)
    {
        if (validate.IsValid && await motoboyRepository.CheckCnpjExistsAsync(motoboyEntity.Cnpj, cancellationToken))
        {
            logger.LogInformation("CNPJ já cadastrado {cnpj}", motoboyEntity.Cnpj);
            validate.Errors.Add(new ValidationFailure("Cnpj", "CNPJ já cadastrado"));
        }

        if (validate.IsValid && await motoboyRepository.CheckCnhExistsAsync(motoboyEntity.Cnh, cancellationToken))
        {
            logger.LogInformation("CNH já cadastrada {cnh}", motoboyEntity.Cnh);
            validate.Errors.Add(new ValidationFailure("Cnh", "CNH já cadastrada"));
        }
    }

    private static async Task<string?> UploadImage(string fileName, Stream stream, CancellationToken cancellationToken)
    {
        const string cloud = "cloud";
        const string apiKey = "";
        const string apiSecret = "";
        var account = new Account(cloud, apiKey, apiSecret);

        var cloudinary = new Cloudinary(account);
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, stream)
        };
        var result = await cloudinary.UploadAsync(uploadParams, cancellationToken);
        return result?.SecureUrl.AbsoluteUri;
    }
}