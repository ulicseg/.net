using AutoMapper;
using ReservasApp.WebAPI.DTOs;
using ReservasApp.WebAPI.Models;

namespace ReservasApp.WebAPI.Services
{
    /// <summary>
    /// Perfiles de mapeo de AutoMapper
    /// ¿Por qué AutoMapper? Para automatizar la conversión entre entidades y DTOs
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ConfigurarMappingUsuario();
            ConfigurarMappingReserva();
            ConfigurarMappingQR();
        }

        private void ConfigurarMappingUsuario()
        {
            // Usuario a UserDto
            CreateMap<Usuario, UserDto>()
                .ForMember(dest => dest.NombreCompleto, 
                          opt => opt.MapFrom(src => src.NombreCompleto));

            // RegisterRequestDto a Usuario
            CreateMap<RegisterRequestDto, Usuario>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => DateTime.UtcNow));
        }

        private void ConfigurarMappingReserva()
        {
            // Reserva a ReservaListDto (para listados)
            CreateMap<Reserva, ReservaListDto>()
                .ForMember(dest => dest.TipoServicio, 
                          opt => opt.MapFrom(src => src.TipoServicioTexto))
                .ForMember(dest => dest.Estado, 
                          opt => opt.MapFrom(src => src.EstadoTexto))
                .ForMember(dest => dest.UsuarioNombre, 
                          opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.NombreCompleto : ""));

            // Reserva a ReservaDetailDto (para detalles)
            CreateMap<Reserva, ReservaDetailDto>()
                .ForMember(dest => dest.TipoServicioTexto, 
                          opt => opt.MapFrom(src => src.TipoServicioTexto))
                .ForMember(dest => dest.EstadoTexto, 
                          opt => opt.MapFrom(src => src.EstadoTexto));

            // CreateReservaDto a Reserva
            CreateMap<CreateReservaDto, Reserva>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => EstadoReserva.Activa));

            // UpdateReservaDto a Reserva
            CreateMap<UpdateReservaDto, Reserva>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());
        }

        private void ConfigurarMappingQR()
        {
            // QRLink a QRResponseDto
            CreateMap<QRLink, QRResponseDto>()
                .ForMember(dest => dest.Url, opt => opt.Ignore()) // Se asigna manualmente
                .ForMember(dest => dest.MinutosParaExpirar, 
                          opt => opt.MapFrom(src => (int)(src.FechaExpiracion - DateTime.UtcNow).TotalMinutes));

            // CreateQRDto a QRLink
            CreateMap<CreateQRDto, QRLink>()
                .ForMember(dest => dest.Hash, opt => opt.Ignore()) // Se genera automáticamente
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.FechaExpiracion, opt => opt.MapFrom(src => DateTime.UtcNow.AddMinutes(10)))
                .ForMember(dest => dest.Usado, opt => opt.MapFrom(src => false));
        }
    }
}
