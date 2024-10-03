using AutoMapper;
using SpecTech.Domain.Entities;
using SpecTech.Domain.Models;

namespace Infrastructure.Mappings;

public class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<TelegramMessageEntity, MessageModel>().ReverseMap();
    }
}