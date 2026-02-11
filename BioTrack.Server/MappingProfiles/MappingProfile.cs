using AutoMapper;
using BioTrack.Server.DTOs;
using BioTrack.Server.Models;

namespace BioTrack.Server.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ---------- Participants ----------
            // Entity <-> DTO (read model) – if ParticipantsDto exists for reads
            CreateMap<Participants, ParticipantsDto>().ReverseMap();

            // CreateParticipantDto -> Participants
            // - Ignore identity (ParticipantID) – DB generates it
            // - Ignore navs to avoid unintended attach/overposting
            // - Optional: pre-trim strings from DTO
            CreateMap<CreateParticipantDto, Participants>()
                .ForMember(dest => dest.ParticipantID, opt => opt.Ignore())
                .ForMember(dest => dest.TrialProtocol, opt => opt.Ignore())
                .ForMember(dest => dest.StudySite, opt => opt.Ignore())
                .ForMember(dest => dest.Consents, opt => opt.Ignore())
                .ForMember(dest => dest.Observations, opt => opt.Ignore())
                .ForMember(dest => dest.AdverseEvents, opt => opt.Ignore())
                .BeforeMap((src, _) =>
                {
                    // Defensive trimming – avoids “   ENROLLED  ” etc.
                    if (src.Name != null) src.Name = src.Name.Trim();
                    if (src.Gender != null) src.Gender = src.Gender.Trim();
                    if (src.Age != null) src.Age = src.Age.Trim();
                    if (src.ContactInfo != null) src.ContactInfo = src.ContactInfo.Trim();
                    if (src.Address != null) src.Address = src.Address.Trim();
                    if (src.EligibilityStatus != null) src.EligibilityStatus = src.EligibilityStatus.Trim().ToUpperInvariant();
                    if (src.Status != null) src.Status = src.Status.Trim().ToUpperInvariant();
                });

            // UpdateParticipantsDto -> Participants
            // - Ignore identity (you’ll set the tracked entity’s key via route/body)
            // - Ignore navs to keep updates simple and safe
            // - Optional: trim inputs
            CreateMap<UpdateParticipantsDto, Participants>()
                .ForMember(dest => dest.ParticipantID, opt => opt.Ignore())
                .ForMember(dest => dest.TrialProtocol, opt => opt.Ignore())
                .ForMember(dest => dest.StudySite, opt => opt.Ignore())
                .ForMember(dest => dest.Consents, opt => opt.Ignore())
                .ForMember(dest => dest.Observations, opt => opt.Ignore())
                .ForMember(dest => dest.AdverseEvents, opt => opt.Ignore())
                .BeforeMap((src, _) =>
                {
                    if (src.Name != null) src.Name = src.Name.Trim();
                    if (src.Gender != null) src.Gender = src.Gender.Trim();
                    if (src.Age != null) src.Age = src.Age.Trim();
                    if (src.ContactInfo != null) src.ContactInfo = src.ContactInfo.Trim();
                    if (src.Address != null) src.Address = src.Address.Trim();
                    if (src.EligibilityStatus != null) src.EligibilityStatus = src.EligibilityStatus.Trim().ToUpperInvariant();
                    if (src.Status != null) src.Status = src.Status.Trim().ToUpperInvariant();
                });

            // ---------- Observations ----------
            // Entity -> Read DTO (simple scalar mapping)
            CreateMap<Observations, ObservationsReadDto>();

            // Create DTO -> Entity
            // - Ignore identity (ObservationID) – DB generates it
            // - (Optional) If you later add navs to Observations, ignore them here too
            CreateMap<ObservationsCreateDto, Observations>()
                .ForMember(dest => dest.ObservationID, opt => opt.Ignore());
        }
    }
}