using AutoMapper;
using InfarlabSis.Application.DTOs.Actores;
using InfarlabSis.Application.DTOs.Compras;
using InfarlabSis.Application.DTOs.Devoluciones;
using InfarlabSis.Application.DTOs.Despachos;
using InfarlabSis.Application.DTOs.Inventario;
using InfarlabSis.Application.DTOs.Persona;
using InfarlabSis.Application.DTOs.Productos;
using InfarlabSis.Application.DTOs.Usuarios;
using InfarlabSis.Application.DTOs.Ventas;
using InfarlabSis.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace InfarlabSis.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Persona
            CreateMap<Persona, PersonaDto>().ReverseMap();

            // Actores
            CreateMap<Cliente, ClienteDto>().ReverseMap();

            CreateMap<Proveedor, ProveedorDto>().ReverseMap();


            // Productos y Almacenes
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Almacen, AlmacenDto>().ReverseMap();
            CreateMap<UnidadMedida, UnidadMedidaDto>().ReverseMap();
            CreateMap<Lote, LoteDto>()
                .ForMember(dest => dest.NombreProducto, opt => opt.MapFrom(src => src.Producto != null ? src.Producto.Nombre : null))
                .ForMember(dest => dest.NombreProveedor, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.Nombre : null));
            CreateMap<AreaCalidad, AreaCalidadDto>()
                .ForMember(dest => dest.NumeroLote, opt => opt.MapFrom(src => src.Lote != null ? src.Lote.NumeroLote : null))
                .ForMember(dest => dest.NombreAlmacenDestino, opt => opt.MapFrom(src => src.AlmacenDestino != null ? src.AlmacenDestino.Nombre : null));
            CreateMap<Producto, ProductoDto>()
                .ForMember(dest => dest.NombreCategoria, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : string.Empty))
                .ForMember(dest => dest.NombreProveedor, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.Nombre : null))
                .ForMember(dest => dest.NombreGenerico, opt => opt.MapFrom(src => src.NombreGenerico))
                .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Codigo))
                .ForMember(dest => dest.StockMinimo, opt => opt.MapFrom(src => src.StockMinimo))
                .ForMember(dest => dest.IdUnidad, opt => opt.MapFrom(src => src.IdUnidad))
                .ForMember(dest => dest.NombreUnidad, opt => opt.MapFrom(src => src.UnidadMedida != null ? src.UnidadMedida.Nombre : null))
                .ForMember(dest => dest.RegistroSanitario, opt => opt.MapFrom(src => src.RegistroSanitario))
                .ReverseMap();

            // Usuarios
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(dest => dest.NombrePersona, opt => opt.MapFrom(src => src.Persona != null ? $"{src.Persona.Nombre} {src.Persona.ApPat}" : string.Empty))
                .ForMember(dest => dest.NombreRol, opt => opt.MapFrom(src => src.Rol != null ? src.Rol.Nombre : string.Empty));

            CreateMap<CrearUsuarioDto, Usuario>();

            CreateMap<UsuarioCreaDto, Usuario>()
            .ForMember(dest => dest.IdUsuario, opt => opt.Ignore())
            .ForMember(dest => dest.Persona, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore());

            // Ventas
            CreateMap<Venta, VentaDto>();
            CreateMap<VentaDto, Venta>()
                .ForMember(dest => dest.IdVenta, opt => opt.Ignore())
                .ForMember(dest => dest.IdUsuario, opt => opt.Ignore())
                .ForMember(dest => dest.IdCliente, opt => opt.Ignore());
            CreateMap<VentaCreaDto, Venta>();

            CreateMap<DetalleVenta, DetalleVentaDto>();
            CreateMap<DetalleVentaDto, DetalleVenta>()
                .ForMember(dest => dest.IdDetalleVenta, opt => opt.Ignore())
                .ForMember(dest => dest.IdVenta, opt => opt.Ignore())
                .ForMember(dest => dest.Subtotal, opt => opt.Ignore());
            CreateMap<DetalleVentaCreaDto, DetalleVenta>();

            // Devoluciones
            CreateMap<Devolucion, DevolucionDto>();
            CreateMap<Devolucion, DevolucionConDetallesDto>();
            CreateMap<DevolucionDto, Devolucion>()
                .ForMember(dest => dest.IdDevolucion, opt => opt.Ignore())
                .ForMember(dest => dest.IdVenta, opt => opt.Ignore());
            CreateMap<DevolucionCreaDto, Devolucion>();

            CreateMap<DetalleDevolucion, DetalleDevolucionDto>();
            CreateMap<DetalleDevolucionDto, DetalleDevolucion>()
                .ForMember(dest => dest.IdDetalleDevolucion, opt => opt.Ignore())
                .ForMember(dest => dest.IdDevolucion, opt => opt.Ignore());
            CreateMap<DetalleDevolucionCreaDto, DetalleDevolucion>();


            // CompraProveedor
            CreateMap<CompraProveedor, CompraProveedorDto>();
            CreateMap<CompraProveedorDto, CompraProveedor>()
                .ForMember(dest => dest.IdCompra, opt => opt.Ignore())
                .ForMember(dest => dest.IdProveedor, opt => opt.Ignore())
                .ForMember(dest => dest.IdCategoria, opt => opt.Ignore());
            CreateMap<CompraProveedorCreaDto, CompraProveedor>();

            // Inventario y despachos
            CreateMap<StockAlmacen, StockAlmacenDto>()
                .ForMember(dest => dest.NombreAlmacen, opt => opt.MapFrom(src => src.Almacen.Nombre))
                .ForMember(dest => dest.NombreProducto, opt => opt.MapFrom(src => src.Producto.Nombre))
                .ForMember(dest => dest.NumeroLote, opt => opt.MapFrom(src => src.Lote.NumeroLote));

            CreateMap<Kardex, KardexDto>()
                .ForMember(dest => dest.NombreProducto, opt => opt.MapFrom(src => src.Producto.Nombre))
                .ForMember(dest => dest.NombreAlmacen, opt => opt.MapFrom(src => src.Almacen.Nombre))
                .ForMember(dest => dest.NumeroLote, opt => opt.MapFrom(src => src.Lote.NumeroLote));

            CreateMap<EntregaDespacho, EntregaDespachoDto>();
            CreateMap<DetalleEntrega, DetalleEntregaDto>()
                .ForMember(dest => dest.NombreProducto, opt => opt.MapFrom(src => src.Producto.Nombre))
                .ForMember(dest => dest.NumeroLote, opt => opt.MapFrom(src => src.Lote.NumeroLote));
        }
    }
}
