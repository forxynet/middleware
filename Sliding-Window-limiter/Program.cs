using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using WebRateLimitAuth.Models;

var builder = WebApplication.CreateBuilder(args);
var myOptions = new MyRateLimitOptions();
builder.Configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);
var slidingPolicy = "sliding";

builder.Services.AddRateLimiter(_ => _ 
.AddSlidingWindowLimiter(policyName: slidingPolicy, options => {
    options.PermitLimit = myOptions.PermitLimit;
    options.Window = TimeSpan.FromSeconds(myOptions.Window);
    options.SegmentsPerWindow = myOptions.SegmentsPerWindow;
    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    options.QueueLimit = myOptions.QueueLimit;
}));

var app = builder.Build();

app.UseRateLimiter();

static string GetTicks() => (DateTime.Now.Ticks & 0x11111).ToString("00000");

app.MapGet("/", () => Results.Ok($"Sliding Window Limiter {GetTicks()}")).RequireRateLimiting(slidingPolicy);

app.Run();
