using System.Linq;
using AutoMapper;
using Leaderboard.Areas.Identity.Models;
using Leaderboard.Areas.Identity.ViewModels;
using Leaderboard.Areas.Leaderboards.Models;
using Leaderboard.Areas.Leaderboards.ViewModels;

namespace Leaderboard.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ScoreModel, ScoreViewModel>().ReverseMap();
            CreateMap<Category, CategoryViewModel>().ReverseMap();
            CreateMap<UnitOfMeasureModel, UnitOfMeasureViewModel>().ReverseMap();
            CreateMap<WeightClass, WeightClassViewModel>().ReverseMap();

            // AutoMapper does some spooky stuff by replacing these variables at runtime
            // alongside calls to Map. false is just the default
            string JoinUrl = null;
            string ViewUrl = null;
            CreateMap<LeaderboardModel, LeaderboardViewModel>()
                .ForMember(m => m.JoinUrl, m => m.MapFrom(_ => JoinUrl))
                .ForMember(m => m.ViewUrl, m => m.MapFrom(_ => ViewUrl))
                .ReverseMap();

            bool IsMember = false;
            bool IsRecommended = false;
            CreateMap<LeaderboardModel, UserLeaderboardViewModel>()
                .IncludeBase<LeaderboardModel, LeaderboardViewModel>()
                .ForMember(m => m.IsMember, m => m.MapFrom(_ => IsMember))
                .ForMember(m => m.IsRecommended, m => m.MapFrom(_ => IsRecommended))
                .ReverseMap();

            bool IsAdmin = false;
            CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(m => m.Interests, m => m.MapFrom(p => p.UserCategories.Select(uc => uc.Category)))
                .ForMember(m => m.Leaderboards, m => m.MapFrom(p => p.UserLeaderboards.Select(ul => ul.Leaderboard)))
                .ForMember(m => m.IsAdmin, m => m.MapFrom(_ => IsAdmin))
                .ReverseMap();

            CreateMap<Division, DivisionViewModel>()
                .ForMember(m => m.Categories, m => m.MapFrom(p => p.DivisionCategories.Select(dc => dc.Category)))
                .ReverseMap();

        }
    }
}
