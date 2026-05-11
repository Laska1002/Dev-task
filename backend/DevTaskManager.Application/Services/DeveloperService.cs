using AutoMapper;
using DevTaskManager.Application.DTOs.Developer;
using DevTaskManager.Application.Interfaces;
using DevTaskManager.Application.Validators;
using DevTaskManager.Domain.Entities;
using DevTaskManager.Infrastructure.Repositories;

namespace DevTaskManager.Application.Services;

public class DeveloperService : IDeveloperService
{
    private readonly DeveloperRepository _developerRepository;
    private readonly IMapper _mapper;

    public DeveloperService(DeveloperRepository developerRepository, IMapper mapper)
    {
        _developerRepository = developerRepository;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<DeveloperResponseDto> Items, int Total)> GetPagedAsync(
        int page, int size, string? status, string? seniority, uint? technologyId = null, uint? projectTypeId = null)
    {
        var (items, total) = await _developerRepository.GetPagedAsync(page, size, status, seniority, technologyId, projectTypeId);
        var dtos = _mapper.Map<IEnumerable<DeveloperResponseDto>>(items);
        return (dtos, total);
    }

    public async Task<DeveloperResponseDto?> GetByIdAsync(uint id)
    {
        var dev = await _developerRepository.GetByIdWithUserAsync(id);
        return dev == null ? null : _mapper.Map<DeveloperResponseDto>(dev);
    }

    public async Task<DeveloperResponseDto> CreateAsync(CreateDeveloperDto dto)
    {
        if (!CedulaValidator.ValidarCedula(dto.Cedula))
            throw new Exception("La cédula ingresada no es válida según el algoritmo de verificación ecuatoriano.");

        if (await _developerRepository.ExistsByCedulaAsync(dto.Cedula))
            throw new Exception("La cédula ya está registrada.");

        var dev = _mapper.Map<Developer>(dto);
        dev.DeveloperTechnologies = dto.TechnologyIds.Select(id => new DeveloperTechnology { TechnologyId = id }).ToList();
        dev.CreatedAt = DateTime.UtcNow;
        dev.UpdatedAt = DateTime.UtcNow;
        
        await _developerRepository.AddAsync(dev);
        
        var created = await _developerRepository.GetByIdWithUserAsync(dev.Id);
        return _mapper.Map<DeveloperResponseDto>(created);
    }

    public async Task<DeveloperResponseDto> UpdateAsync(uint id, UpdateDeveloperDto dto)
    {
        var dev = await _developerRepository.GetByIdWithUserAsync(id);
        if (dev == null)
            throw new Exception("Developer no encontrado.");

        // dto.Cedula is ignored by JSON binder anyway, but we ensure we don't map it here manually either
        _mapper.Map(dto, dev);
        dev.DeveloperTechnologies.Clear();
        foreach (var techId in dto.TechnologyIds)
        {
            dev.DeveloperTechnologies.Add(new DeveloperTechnology { TechnologyId = techId });
        }
        dev.UpdatedAt = DateTime.UtcNow;

        await _developerRepository.UpdateAsync(dev);

        var updated = await _developerRepository.GetByIdWithUserAsync(dev.Id);
        return _mapper.Map<DeveloperResponseDto>(updated);
    }

    public async Task SoftDeleteAsync(uint id)
    {
        var dev = await _developerRepository.GetByIdAsync(id);
        if (dev != null)
        {
            dev.IsActive = false;
            dev.UpdatedAt = DateTime.UtcNow;
            await _developerRepository.UpdateAsync(dev);
        }
    }
}
