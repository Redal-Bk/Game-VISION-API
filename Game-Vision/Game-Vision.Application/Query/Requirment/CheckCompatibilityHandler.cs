using Game_Vision.Application.DTO.CompatibilityResultDto;
using Game_Vision.Application.Interface;
using Game_Vision.Application.Query.Requirment;
using Game_Vision.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Game_Vision.Application.Query.Requirement
{
    public class CheckCompatibilityHandler : IRequestHandler<CheckCompatibilityQuery, CompatibilityResultDto>
    {
        private readonly GameVisionDbContext _context;
        private readonly IWebSearchService _webSearchService;

        public CheckCompatibilityHandler(GameVisionDbContext context, IWebSearchService webSearchService)
        {
            _context = context;
            _webSearchService = webSearchService;
        }

        public async Task<CompatibilityResultDto> Handle(CheckCompatibilityQuery request, CancellationToken ct)
        {
            var gameReq = await _context.SystemRequirements
                .FirstOrDefaultAsync(sr => sr.GameId == request.GameId, ct);

            var userSpecs = await _context.UserPcspecs
                .FirstOrDefaultAsync(us => us.UserId == request.UserId, ct);

            if (gameReq == null)
                throw new InvalidOperationException("مشخصات سیستم مورد نیاز بازی یافت نشد.");

            if (userSpecs == null)
                throw new InvalidOperationException("مشخصات سیستم شما ثبت نشده است.");

            var result = new CompatibilityResultDto
            {
                CanRun = true,
                Level = CompatibilityLevel.Perfect,
                OverallScore = 100,
                Bottleneck = "",
                Message = "بازی به راحتی روی سیستم شما اجرا می‌شود."
            };

            var issues = new List<string>();

            // مقایسه RAM
            if (userSpecs.Ram < gameReq.MinRam)
            {
                issues.Add("RAM کافی نیست");
                result.Level = CompatibilityLevel.CannotRun;
            }
            else if (userSpecs.Ram < gameReq.RecRam)
            {
                result.Level = CompatibilityLevel.Minimum;
            }

            // مقایسه Storage
            if (userSpecs.StorageAvailable < gameReq.MinStorage)
            {
                issues.Add("فضای ذخیره‌سازی کافی نیست");
                result.Level = CompatibilityLevel.CannotRun;
            }

            // مقایسه OS
            var userOsVersion = GetOSVersion(userSpecs.Os);
            var minOsVersion = GetOSVersion(gameReq.MinOs);
            if (userOsVersion < minOsVersion)
            {
                issues.Add("سیستم عامل قدیمی است");
                result.Level = CompatibilityLevel.CannotRun;
            }

            // مقایسه DirectX
            if (!string.IsNullOrEmpty(gameReq.RecDirectX) && !string.IsNullOrEmpty(userSpecs.DirectX))
            {
                var userDx = ParseDirectXVersion(userSpecs.DirectX);
                var reqDx = ParseDirectXVersion(gameReq.RecDirectX);
                if (userDx < reqDx)
                {
                    issues.Add("نسخه DirectX قدیمی است");
                    result.Level = CompatibilityLevel.Low;
                }
            }

            // مقایسه CPU با بنچمارک
            var userCpuScore = await _webSearchService.GetBenchmarkScore(userSpecs.Cpu, "CPU") ?? 0;
            var minCpuScore = await _webSearchService.GetBenchmarkScore(gameReq.MinCpu, "CPU") ?? 0;
            var recCpuScore = await _webSearchService.GetBenchmarkScore(gameReq.RecCpu, "CPU") ?? minCpuScore;

            result.Cpu.UserCpu = userSpecs.Cpu;
            result.Cpu.MinCpu = gameReq.MinCpu;
            result.Cpu.RecCpu = gameReq.RecCpu;
            result.Cpu.UserScore = userCpuScore;
            result.Cpu.MinScore = minCpuScore;
            result.Cpu.RecScore = recCpuScore;
            result.Cpu.MeetsMinimum = userCpuScore >= minCpuScore * 0.9;
            result.Cpu.MeetsRecommended = userCpuScore >= recCpuScore * 0.9;

            if (!result.Cpu.MeetsMinimum)
            {
                issues.Add("پردازنده ضعیف است");
                result.Level = result.Level == CompatibilityLevel.Perfect ? CompatibilityLevel.Low : result.Level;
            }
            else if (!result.Cpu.MeetsRecommended)
            {
                result.Level = CompatibilityLevel.Good;
            }

            // مقایسه GPU (همین منطق)
            var userGpuScore = await _webSearchService.GetBenchmarkScore(userSpecs.Gpu, "GPU") ?? 0;
            var minGpuScore = await _webSearchService.GetBenchmarkScore(gameReq.MinGpu, "GPU") ?? 0;
            var recGpuScore = await _webSearchService.GetBenchmarkScore(gameReq.RecGpu, "GPU") ?? minGpuScore;

            result.Gpu.UserGpu = userSpecs.Gpu;
            result.Gpu.MinGpu = gameReq.MinGpu;
            result.Gpu.RecGpu = gameReq.RecGpu;
            result.Gpu.UserScore = userGpuScore;
            result.Gpu.MinScore = minGpuScore;
            result.Gpu.RecScore = recGpuScore;
            result.Gpu.MeetsMinimum = userGpuScore >= minGpuScore * 0.9;
            result.Gpu.MeetsRecommended = userGpuScore >= recGpuScore * 0.9;

            if (!result.Gpu.MeetsMinimum)
            {
                issues.Add("کارت گرافیک ضعیف است");
                result.Level = CompatibilityLevel.CannotRun;
            }
            else if (!result.Gpu.MeetsRecommended)
            {
                result.Level = CompatibilityLevel.Good;
            }

            // تعیین بوتل‌نک
            if (result.Cpu.MeetsRecommended && !result.Gpu.MeetsRecommended)
                result.Bottleneck = "کارت گرافیک بوتل‌نک اصلی است.";
            else if (result.Gpu.MeetsRecommended && !result.Cpu.MeetsRecommended)
                result.Bottleneck = "پردازنده بوتل‌نک اصلی است.";

            // اگر مشکلی بود
            if (issues.Count > 0)
            {
                result.CanRun = result.Level != CompatibilityLevel.CannotRun;
                result.Bottleneck = string.Join(" ", issues);
                result.Message = result.Level switch
                {
                    CompatibilityLevel.CannotRun => "بازی روی سیستم شما اجرا نمی‌شود.",
                    CompatibilityLevel.Low => "بازی با تنظیمات خیلی پایین اجرا می‌شود.",
                    CompatibilityLevel.Minimum => "بازی با تنظیمات حداقل اجرا می‌شود.",
                    CompatibilityLevel.Good => "بازی با تنظیمات متوسط تا بالا اجرا می‌شود.",
                    _ => result.Message
                };
            }

            // محاسبه امتیاز کلی
            result.OverallScore = CalculateOverallScore(result);

            return result;
        }

        // تبدیل OS به عدد برای مقایسه
        private int GetOSVersion(string os)
        {
            if (string.IsNullOrEmpty(os)) return 0;

            os = os.ToLower();
            if (os.Contains("windows 11")) return 11;
            if (os.Contains("windows 10")) return 10;
            if (os.Contains("windows 8")) return 8;
            if (os.Contains("windows 7")) return 7;
            return 0;
        }

        // تبدیل DirectX به عدد
        private int ParseDirectXVersion(string dx)
        {
            if (string.IsNullOrEmpty(dx)) return 0;
            var match = System.Text.RegularExpressions.Regex.Match(dx, @"\d+");
            return match.Success ? int.Parse(match.Value) : 0;
        }

        // محاسبه امتیاز کلی (0-100)
        private int CalculateOverallScore(CompatibilityResultDto result)
        {
            int score = 100;

            if (!result.Cpu.MeetsRecommended) score -= 20;
            if (!result.Cpu.MeetsMinimum) score -= 40;

            if (!result.Gpu.MeetsRecommended) score -= 25;
            if (!result.Gpu.MeetsMinimum) score -= 45;

            if (result.Ram.UserRam < result.Ram.MinRam) score -= 30;
            if (result.Storage.UserStorage < result.Storage.RequiredStorage) score -= 20;

            if (!result.Os.Compatible) score -= 15;
            if (!result.DirectX.Compatible) score -= 10;

            return Math.Max(0, score);
        }
    }
}