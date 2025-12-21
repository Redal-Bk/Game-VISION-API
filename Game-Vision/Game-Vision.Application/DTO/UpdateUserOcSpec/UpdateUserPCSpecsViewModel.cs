using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Game_Vision.Application.DTO.UpdateUserOcSpec
{
    public class UpdateUserPCSpecsViewModel
    {
        [Display(Name = "سیستم عامل")]
        public string OS { get; set; } = string.Empty;

        [Display(Name = "پردازنده (CPU)")]
        public string CPU { get; set; } = string.Empty;

        [Display(Name = "رم (RAM به گیگابایت)")]
        public int RAM { get; set; }

        [Display(Name = "کارت گرافیک (GPU)")]
        public string GPU { get; set; } = string.Empty;

        [Display(Name = "نسخه DirectX")]
        public string DirectX { get; set; } = string.Empty;

        [Display(Name = "حافظه (به گیگابایت)")]
        public int Storage { get; set; }
    }
}
