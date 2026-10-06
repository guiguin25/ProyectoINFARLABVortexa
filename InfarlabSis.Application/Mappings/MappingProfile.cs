using InfarlabSis.Application.DTOs.Actores;
using InfarlabSis.Application.DTOs.Persona;
using InfarlabSis.Application.DTOs.Productos;
using InfarlabSis.Application.DTOs.Usuarios;
using InfarlabSis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;
namespace InfarlabSis.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Persona
            CreateMap<Persona, PersonaDto>().ReverseMap();

            // Actores
            CreateMap<Cliente, ClienteDto>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Persona != null ? $"{src.Persona.Nombre} {src.Persona.ApPat}" : string.Empty))
                .ForMember(dest => dest.CI, opt => opt.MapFrom(src => src.Persona != null ? src.Persona.CI : string.Empty))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Persona != null ? src.Persona.Email : string.Empty))
                .ReverseMap();

            CreateMap<Proveedor, ProveedorDto>()
                .ForMember(dest => dest.NombrePersonaContacto, opt => opt.MapFrom(src => src.Persona != null ? $"{src.Persona.Nombre} {src.Persona.ApPat}" : null))
                .ReverseMap();

            // Productos y Almacenes
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Almacen, AlmacenDto>().ReverseMap();
            CreateMap<Producto, ProductoDto>()
                .ForMember(dest => dest.NombreCategoria, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : string.Empty))
                .ForMember(dest => dest.NombreProveedor, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.Nombre : null))
                .ReverseMap();

            // Usuarios
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(dest => dest.NombrePersona, opt => opt.MapFrom(src => src.Persona != null ? $"{src.Persona.Nombre} {src.Persona.ApPat}" : string.Empty))
                .ForMember(dest => dest.NombreRol, opt => opt.MapFrom(src => src.Rol != null ? src.Rol.Nombre : string.Empty));

            CreateMap<CrearUsuarioDto, Usuario>();
        }
    }
}
