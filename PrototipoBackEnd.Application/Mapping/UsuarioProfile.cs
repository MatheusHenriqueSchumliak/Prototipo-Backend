using PrototipoBackEnd.Application.Dtos;
using PrototipoBackEnd.Domain.Entities;
using AutoMapper;

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
