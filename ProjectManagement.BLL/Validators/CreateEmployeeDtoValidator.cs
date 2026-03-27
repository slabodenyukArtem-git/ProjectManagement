using FluentValidation;
using ProjectManagement.BLL.DTOs;

namespace ProjectManagement.BLL.Validators;

public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя обязательно")
            .MaximumLength(100).WithMessage("Имя не может быть длиннее 100 символов");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Фамилия обязательна")
            .MaximumLength(100).WithMessage("Фамилия не может быть длиннее 100 символов");
            
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен")
            .EmailAddress().WithMessage("Некорректный формат email")
            .MaximumLength(200).WithMessage("Email не может быть длиннее 200 символов");
    }
}
