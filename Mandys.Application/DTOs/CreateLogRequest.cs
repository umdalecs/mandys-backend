namespace Mandys.Application.DTOs;

public record CreateLogRequest(
    string Verb,
    string Description
);