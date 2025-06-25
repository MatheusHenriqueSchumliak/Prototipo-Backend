using PrototipoBackEnd.Application.Dtos;
using PrototipoBackEnd.Domain.Entities;
using AutoMapper;

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
