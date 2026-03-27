using FluentValidation;
using ProjectManagement.BLL.DTOs;

namespace ProjectManagement.BLL.Validators;

public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название проекта обязательно")
            .MaximumLength(200).WithMessage("Название не может быть длиннее 200 символов");
            
        RuleFor(x => x.CustomerCompany)
            .NotEmpty().WithMessage("Компания-заказчик обязательна")
            .MaximumLength(200).WithMessage("Название компании не может быть длиннее 200 символов");
            
        RuleFor(x => x.ExecutorCompany)
            .NotEmpty().WithMessage("Компания-исполнитель обязательна")
            .MaximumLength(200).WithMessage("Название компании не может быть длиннее 200 символов");
            
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Дата начала обязательна");
            
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Дата окончания обязательна")
            .GreaterThan(x => x.StartDate).WithMessage("Дата окончания должна быть позже даты начала");
            
        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 10).WithMessage("Приоритет должен быть от 1 до 10");
            
        RuleFor(x => x.ProjectManagerId)
            .GreaterThan(0).WithMessage("Руководитель проекта обязателен");
    }
}
