using PrototipoBackEnd.Domain.Entities;
using AutoMapper;
using PrototipoBackEnd.Application.Dtos.Artesanato;

namespace PrototipoBackEnd.Application.Mapping
{
	public class ArtesanatoProfile : Profile
	{
		public ArtesanatoProfile()
		{
			CreateMap<Artesanato, ArtesanatoDto>();

			CreateMap<ArtesanatoDto, Artesanato>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) =>
			srcMember != null && !Equals(srcMember, destMember)));
		}
	}
}
