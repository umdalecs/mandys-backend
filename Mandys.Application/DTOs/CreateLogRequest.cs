namespace Mandys.DTOs;

public record CreateLogRequest(
    string Verb,
    string Description
);