using System.Text.RegularExpressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Content.Goobstation.Common.CCVar;
using Content.Goobstation.Common.StationReport;
using Robust.Shared.Configuration;

namespace Content.Goobstation.Server.StationReportDiscordIntergrationSystem;

public sealed partial class StationReportDiscordIntergrationSystem : EntitySystem
{
    //thank you Timfa for writing this code
    private static readonly HttpClient Client = new();  // Reserve edit: Fix Station Report Discord integration
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private ILogManager _logManager = default!;  // Reserve edit: Fix Station Report Discord integration
    private ISawmill _sawmill = default!;  // Reserve edit: Fix Station Report Discord integration

    private string? _webhookUrl;

    public override void Initialize()
    {
        base.Initialize();

        _sawmill = _logManager.GetSawmill("StationReportDiscordIntergrationSystem");  // Reserve edit: Fix Station Report Discord integration

        //subscribes to the endroundevent and Stationreportevent
        SubscribeLocalEvent<StationReportEvent>(OnStationReportReceived);

        // Keep track of CCVar value, update if changed
        _cfg.OnValueChanged(GoobCVars.StationReportDiscordWebHook, url => _webhookUrl = url, true);
    }

    private static string? _report;  // Reserve edit: Fix Station Report Discord integration

    private static readonly TagReplacement[] Replacements =  // Reserve edit: Fix Station Report Discord integration
    {
        new(@"\[/?bold\]", @"**"),
        new(@"\[/?italic\]", @"_"),
        new(@"\[/?mono\]", @"`"),
        new(@">", @""),
        new(@"\[h1\]", @"# "),
        new(@"\[h2\]", @"## "),
        new(@"\[h3\]", @"### "),
        new(@"\[h4\]", @"-# "),
        new(@"\[/h[0-9]\]", @""),
        new(@"\[head=1\]", @"# "),
        new(@"\[head=2\]", @"## "),
        new(@"\[head=3\]", @"### "),
        new(@"\[head=4\]", @"-# "),
        new(@"\[/head\]", @""),
        new(@"\[/?color(=[#0-9a-zA-Z]+)?\]", @"")
    };

    private void OnStationReportReceived(StationReportEvent ev)
    {
        _report = ev.StationReportText;  // Reserve edit: Fix Station Report Discord integration

        if (string.IsNullOrWhiteSpace(_report))  // Reserve edit: Fix Station Report Discord integration
            return;

        foreach (var replacement in Replacements)  // Reserve edit: Fix Station Report Discord integration
        {
            var regex = new Regex(replacement.Tag);  // Reserve edit: Fix Station Report Discord integration
            _report = regex.Replace(_report, replacement.Replacement);  // Reserve edit: Fix Station Report Discord integration
        }

        // Run async without blocking
        _ = SendMessageAsync(_report);  // Reserve edit: Fix Station Report Discord integration
    }

    private async Task SendMessageAsync(string message)
    {
        if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(_webhookUrl))
            return;

        var payload = new { content = message };
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await Client.PostAsync(_webhookUrl, content);  // Reserve edit: Fix Station Report Discord integration
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)  // Reserve edit: Fix Station Report Discord integration
        {
            _sawmill.Error($"Error sending station report to discord: {ex}");  // Reserve edit: Fix Station Report Discord integration
        }
    }

    public struct TagReplacement
    {
        public string Tag, Replacement;
        public TagReplacement(string tag, string replacement)
        {
            Tag = tag;
            Replacement = replacement;
        }
    }
}
