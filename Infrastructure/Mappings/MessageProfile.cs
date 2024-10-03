using AutoMapper;
using Infrastructure.Models;
using BLL.Entities;

namespace Infrastructure.Mappings;

public class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<MessageEntity, MessageModel>().ReverseMap();
    }
}