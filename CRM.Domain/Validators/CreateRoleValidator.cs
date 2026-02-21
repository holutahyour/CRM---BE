using CRM.Domain.DTOs.Core;
using FluentValidation;

namespace CRM.Domain.Validators;

public class CreateRoleValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50)
            .Matches("^[A-Z_]+$").WithMessage("Code must be uppercase with underscores only");
        RuleFor(x => x.PermissionIds).NotEmpty();
    }
}
