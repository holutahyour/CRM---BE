namespace CRM.Domain.DTOs.Core;

public record MenuDTO(Guid Id, string Name, string Label, string? Icon, string? Route, int Position, List<MenuDTO> Children);