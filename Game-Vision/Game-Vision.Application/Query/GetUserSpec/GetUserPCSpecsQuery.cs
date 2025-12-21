using Game_Vision.Application.DTO.UserPcReq;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Vision.Application.Query.GetUserSpec
{
    public class GetUserPCSpecsQuery : IRequest<UserPCSpecsDto>
    {
        public int UserId { get; set; }
    }
}
