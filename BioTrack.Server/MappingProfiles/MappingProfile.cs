using AutoMapper;
using BioTrack.Server.Controllers;
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

            CreateMap<AdverseEvents, AdverseEventsReadDto>();

            CreateMap<AdverseEventsCreateDto, AdverseEvents>()
                            .ForMember(dest => dest.EventID, opt => opt.Ignore())
                            .ForMember(dest => dest.Participant, opt => opt.Ignore()) // avoid attaching navs
                            .ForMember(dest => dest.ReportedDate, opt => opt.MapFrom(src => src.ReportedDate ?? DateTime.UtcNow))
                            .BeforeMap((src, _) =>
                            {
                                if (src.ActionTaken != null) src.ActionTaken = src.ActionTaken.Trim();
                                if (src.Outcome != null) src.Outcome = src.Outcome.Trim();
                            });


            CreateMap<StudySites, StudySiteReadDto>()
                            .ForMember(dest => dest.PrincipalInvestigator,
                                       opt => opt.MapFrom(src => src.PrincipalInvestigator));


            CreateMap<ResearcherCredentials, ResearcherMiniDto>();

            // Create DTO -> Entity
            // ProtocolID and PrincipalInvestigatorId are set in the controller (business rule)
            CreateMap<StudySiteCreateDto, StudySites>()
                .ForMember(dest => dest.ProtocolID, opt => opt.Ignore())
                .ForMember(dest => dest.PrincipalInvestigatorId, opt => opt.Ignore());
        
        // Entity -> Read DTO
        CreateMap<ProtocolDeviation, ReadProtocolDeviation>();

// Create DTO -> Entity
CreateMap<CreateProtocolDeviationDto, ProtocolDeviation>()
    .ForMember(dest => dest.DeviationId, opt => opt.Ignore())
    .ForMember(dest => dest.TrialProtocol, opt => opt.Ignore())
    .ForMember(dest => dest.Participant, opt => opt.Ignore())
    .ForMember(dest => dest.Observation, opt => opt.Ignore())
    .ForMember(dest => dest.ReportedDate,
               opt => opt.MapFrom(src => src.ReportedDate ?? DateTime.UtcNow))
    .BeforeMap((src, _) =>
    {
            if (src.Description != null) src.Description = src.Description.Trim();
        });

// Update DTO -> Entity
CreateMap<UpdateProtocolDeviation, ProtocolDeviation>()
    .ForMember(dest => dest.DeviationId, opt => opt.Ignore())
    .ForMember(dest => dest.TrialProtocol, opt => opt.Ignore())
    .ForMember(dest => dest.Participant, opt => opt.Ignore())
    .ForMember(dest => dest.Observation, opt => opt.Ignore())
    .BeforeMap((src, _) =>
    {
            if (src.Description != null) src.Description = src.Description.Trim();
        });



            CreateMap<ComplianceReports, ReadComplianceReportDto>();

            // Create DTO -> Entity
            // - Ignore identity & navs
            // - We'll set DeviationCount and GeneratedDate in controller (business logic)
            CreateMap<CreateComplianceReport, ComplianceReports>()
                .ForMember(dest => dest.ReportID, opt => opt.Ignore())
                .ForMember(dest => dest.TrialProtocol, opt => opt.Ignore())
                .ForMember(dest => dest.DeviationCount, opt => opt.Ignore())
                .ForMember(dest => dest.GeneratedDate, opt => opt.Ignore());

            CreateMap<ConsentForm, ReadConsent>();

            CreateMap<CreateConsent, ConsentForm>()
                .ForMember(dest => dest.ConsentID, opt => opt.Ignore())
                .ForMember(dest => dest.Participant, opt => opt.Ignore());


            // Use this to return researcher info safely (no PasswordHash)
            CreateMap<ResearcherCredentials, ResearcherMiniDto>();

            // ---------- CreateResearcherDto -> ResearcherCredentials ----------
            // Create a researcher WITHOUT password here (PasswordHash ignored).
            // You will set PasswordHash later via a dedicated SetPassword endpoint.
            CreateMap<CreateResearcherDto, ResearcherCredentials>()
                .ForMember(dest => dest.ResearcherId, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // set later
                                                                           // Avoid attaching navs from DTO during creation:
                .ForMember(dest => dest.PrincipalInvestigatorSites, opt => opt.Ignore())
                .ForMember(dest => dest.CollaboratingSites, opt => opt.Ignore())
                .ForMember(dest => dest.LeadProtocols, opt => opt.Ignore());

            CreateMap<TrialProtocols, ReadProtocolDto>()
                            .ForMember(d => d.LeadResearcher, opt => opt.MapFrom(src =>
                                src.LeadResearcher == null
                                    ? null
                                    : new ResearcherMiniDto
                                    {
                                        ResearcherId = src.LeadResearcherId!.Value,
                                        FullName = src.LeadResearcher.FullName,   // adjust to your actual property names
                                        Email = src.LeadResearcher.Email
                                    }))
                            .ForMember(d => d.StudySiteCount, opt => opt.MapFrom(s => s.StudySites.Count))
                            .ForMember(d => d.ParticipantCount, opt => opt.MapFrom(s => s.Participants.Count))
                            .ForMember(d => d.ComplianceReportCount, opt => opt.MapFrom(s => s.ComplianceReports.Count))
                            .ForMember(d => d.TrialReportCount, opt => opt.MapFrom(s => s.TrialReports.Count))
                            .ForMember(d => d.ProtocolDeviationCount, opt => opt.MapFrom(s => s.ProtocolDeviations.Count))
                            // Map related IDs (adjust key names if different in your models)
                            .ForMember(d => d.StudySiteIds, opt => opt.MapFrom(s => s.StudySites.Select(x => x.SiteID)))
                            .ForMember(d => d.ParticipantIds, opt => opt.MapFrom(s => s.Participants.Select(x => x.ParticipantID)))
                            .ForMember(d => d.ComplianceReportIds, opt => opt.MapFrom(s => s.ComplianceReports.Select(x => x.ReportID)))
                            .ForMember(d => d.TrialReportIds, opt => opt.MapFrom(s => s.TrialReports.Select(x => x.ReportID)))
                            .ForMember(d => d.ProtocolDeviationIds, opt => opt.MapFrom(s => s.ProtocolDeviations.Select(x => x.DeviationId)));

            // Create DTO -> Model
            CreateMap<CreateProtocolDto, TrialProtocols>()
                .ForMember(d => d.ProtocolID, opt => opt.Ignore())
                .ForMember(d => d.LeadResearcherId, opt => opt.MapFrom(s => s.LeadResearcherId))
                // Collections handled in the service layer when attaching existing entities
                .ForMember(d => d.StudySites, opt => opt.Ignore())
                .ForMember(d => d.Participants, opt => opt.Ignore())
                .ForMember(d => d.ComplianceReports, opt => opt.Ignore())
                .ForMember(d => d.TrialReports, opt => opt.Ignore())
                .ForMember(d => d.ProtocolDeviations, opt => opt.Ignore());

            // Update DTO -> Model (overwrite scalar fields; relations handled in service)
            CreateMap<UpdateProtcolDto, TrialProtocols>()
                .ForMember(d => d.StudySites, opt => opt.Ignore())
                .ForMember(d => d.Participants, opt => opt.Ignore())
                .ForMember(d => d.ComplianceReports, opt => opt.Ignore())
                .ForMember(d => d.TrialReports, opt => opt.Ignore())
                .ForMember(d => d.ProtocolDeviations, opt => opt.Ignore());
        



    }

}

}




