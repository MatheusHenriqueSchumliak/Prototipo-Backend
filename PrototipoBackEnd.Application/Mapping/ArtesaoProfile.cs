using PrototipoBackEnd.Application.Dtos;
using PrototipoBackEnd.Domain.Entities;
using AutoMapper;

namespace PrototipoBackEnd.Application.Mapping
{
	public class ArtesaoProfile : Profile
	{
		public ArtesaoProfile()
		{
			CreateMap<Artesao, ArtesaoDto>().ReverseMap();
				//.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

			CreateMap<ArtesaoDto, Artesao>()
				.ForAllMembers(opts =>
					opts.Condition((src, dest, srcMember, destMember) =>
						srcMember != null && !Equals(srcMember, destMember)));
		}
	}
}
