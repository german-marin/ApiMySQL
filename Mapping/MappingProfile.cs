using AutoMapper;
using ApiMySQL.Model;
using ApiMySQL.DTOs;

namespace ApiMySQL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de DTO a Entidad
            CreateMap<MuscleGroupDto, MuscleGroup>()
                .ForMember(dest => dest.LastUpdate, opt => opt.Ignore()); // Ignorar el campo LastUpdate al mapear de DTO a Entidad
            // Mapeo de Entidad a DTO
            CreateMap<MuscleGroup, MuscleGroupDto>(); // LastUpdate no se incluye porque no existe en el DTO

            // Mapeo de Category a CategoryDto
            CreateMap<Category, CategoryDto>();
            // Mapeo de CategoryDto a Category
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.LastUpdate, opt => opt.Ignore()); // Ignoramos LastUpdate

            // Mapeo de CustomerDto a Customer
            CreateMap<CustomerDto, Customer>()
                .ForMember(dest => dest.LastUpdated, opt => opt.Ignore());
            // Mapeo de Customer a CustomerDto
            CreateMap<Customer, CustomerDto>();

            // Mapeo de ExerciseDto a Exercise
            CreateMap<ExerciseDto, Exercise>()
                .ForMember(dest => dest.LastUpdated, opt => opt.Ignore());
            CreateMap<Exercise, ExerciseDto>();

            // Mapeo de TrainingDto a Training
            CreateMap<TrainingDto, Training>()
                .ForMember(dest => dest.LastUpdate, opt => opt.Ignore());
            CreateMap<Training, TrainingDto>();

            // Mapeo de TrainingLineDto a Training
            CreateMap<TrainingLineDto, TrainingLine>()
                .ForMember(dest => dest.LastUpdated, opt => opt.Ignore());
            CreateMap<TrainingLine, TrainingLineDto>();

            // Mapeo de UserDto a User
            CreateMap<User, UserDto>()
               .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
               .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
        }
    }
}
