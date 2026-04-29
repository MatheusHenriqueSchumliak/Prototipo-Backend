using PrototipoBackEnd.Domain.Entities;
using AutoMapper;
using PrototipoBackEnd.Application.Dtos.Artesao;

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
