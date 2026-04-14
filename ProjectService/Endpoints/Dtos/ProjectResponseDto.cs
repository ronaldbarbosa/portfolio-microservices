namespace ProjectService.Endpoints.Dtos;

public record ProjectResponseDto(
    long Id,
    string Name,
    string Description,
    string Frontend,
    string Backend,
    string Tools,
    string Url,
    string Code,
    string Image,
    bool Finished);