using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Vision.Application.DTO.UserPcReq
{
    public class UserPCSpecsDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string OS { get; set; } = string.Empty;

        public string CPU { get; set; } = string.Empty;

        public int RAM { get; set; } // به گیگابایت

        public string GPU { get; set; } = string.Empty;

        public string DirectX { get; set; } = string.Empty;

        public int Storage { get; set; } // به گیگابایت

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
