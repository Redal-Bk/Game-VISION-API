using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Vision.Application.DTO.CompatibilityResultDto
{
    public class CompatibilityResultDto
    {
        // آیا بازی روی سیستم کاربر اجرا می‌شه؟
        public bool CanRun { get; set; }

        // نتیجه کلی (برای نمایش رنگ)
        public CompatibilityLevel Level { get; set; } // Perfect, Good, Minimum, Low, CannotRun

        // امتیاز کلی سازگاری (از 0 تا 100)
        public int OverallScore { get; set; }

        // توضیح بوتل‌نک یا مشکل اصلی (اگر وجود داشته باشه)
        public string Bottleneck { get; set; } = string.Empty;

        // جزئیات مقایسه هر بخش
        public CpuCompatibility Cpu { get; set; } = new();
        public GpuCompatibility Gpu { get; set; } = new();
        public RamCompatibility Ram { get; set; } = new();
        public StorageCompatibility Storage { get; set; } = new();
        public OsCompatibility Os { get; set; } = new();
        public DirectXCompatibility DirectX { get; set; } = new();

        // پیام نهایی برای کاربر
        public string Message { get; set; } = string.Empty;
    }

    // سطح سازگاری کلی
    public enum CompatibilityLevel
    {
        CannotRun,   // قرمز - اجرا نمی‌شه
        Low,         // نارنجی - با تنظیمات خیلی پایین
        Minimum,     // زرد - حداقل اجرا می‌شه
        Good,        // سبز روشن - خوب اجرا می‌شه
        Perfect      // سبز تیره - عالی و بدون مشکل
    }

    // جزئیات هر بخش
    public class CpuCompatibility
    {
        public string UserCpu { get; set; } = string.Empty;
        public string MinCpu { get; set; } = string.Empty;
        public string RecCpu { get; set; } = string.Empty;
        public int UserScore { get; set; }
        public int MinScore { get; set; }
        public int RecScore { get; set; }
        public bool MeetsMinimum { get; set; }
        public bool MeetsRecommended { get; set; }
    }

    public class GpuCompatibility
    {
        public string UserGpu { get; set; } = string.Empty;
        public string MinGpu { get; set; } = string.Empty;
        public string RecGpu { get; set; } = string.Empty;
        public int UserScore { get; set; }
        public int MinScore { get; set; }
        public int RecScore { get; set; }
        public bool MeetsMinimum { get; set; }
        public bool MeetsRecommended { get; set; }
    }

    public class RamCompatibility
    {
        public int UserRam { get; set; }
        public int MinRam { get; set; }
        public int RecRam { get; set; }
        public bool MeetsMinimum { get; set; }
        public bool MeetsRecommended { get; set; }
    }

    public class StorageCompatibility
    {
        public int UserStorage { get; set; }
        public int RequiredStorage { get; set; }
        public bool HasEnough { get; set; }
    }

    public class OsCompatibility
    {
        public string UserOs { get; set; } = string.Empty;
        public string MinOs { get; set; } = string.Empty;
        public bool Compatible { get; set; }
    }

    public class DirectXCompatibility
    {
        public string UserDirectX { get; set; } = string.Empty;
        public string RequiredDirectX { get; set; } = string.Empty;
        public bool Compatible { get; set; }
    }
}
