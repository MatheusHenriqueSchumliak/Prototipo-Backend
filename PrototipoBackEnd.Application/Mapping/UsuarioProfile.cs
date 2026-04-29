using PrototipoBackEnd.Domain.Entities;
using AutoMapper;
using PrototipoBackEnd.Application.Dtos.Usuario;

namespace PrototipoBackEnd.Application.Mapping
{
	public class UsuarioProfile : Profile
	{
		public UsuarioProfile()
		{
			CreateMap<Usuario, UsuarioDto>();

			CreateMap<UsuarioDto, Usuario>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) =>
			srcMember != null && !Equals(srcMember, destMember)));

		}
	}
}
