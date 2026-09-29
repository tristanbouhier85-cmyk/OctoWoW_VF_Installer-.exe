using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OctoWoW_VF_Installer;

public sealed class InstallerService
{
    static readonly string[] ClientCandidates = [@"C:\OctoWoW", @"D:\OctoWoW", @"E:\OctoWoW"];
    static readonly string[] FrenchCandidates = [@"C:\WOWfrFR\WOWfrFR", @"D:\WOWfrFR\WOWfrFR", @"E:\WOWfrFR\WOWfrFR"];
    readonly string stateName = "OctoWoW_VF_installation.json";
    public void Install(Action<string> report)
    {
        var root = FindClient(ClientCandidates, IsOcto) ?? SelectFolder("Sélectionnez le dossier OctoWoW"); if (root is null) throw new InvalidOperationException("Dossier OctoWoW introuvable.");
        var french = FindClient(FrenchCandidates, IsFrench) ?? SelectFolder("Sélectionnez le client français contenant Data\\speech.MPQ");
        if (french is null || !IsFrench(french)) throw new InvalidOperationException("Client français invalide : WoW.exe et Data\\speech.MPQ sont requis.");
        if (Process.GetProcessesByName("WoW").Length > 0 || Process.GetProcessesByName("OctoLauncher").Length > 0 || Process.GetProcessesByName("OctoWoWLauncher").Length > 0) throw new InvalidOperationException("Fermez WoW et le launcher avant l’installation.");
        report = WithFileLog(root, report); var exe = Path.Combine(root, "WoW.exe"); ValidateExe(exe); var data = Path.Combine(root, "Data"); Directory.CreateDirectory(data); report($"Client OctoWoW : {root}"); report($"Client français : {french}");
        var state = new InstallState { Root = root, FrenchRoot = french, InstalledAt = DateTimeOffset.Now, ConfigExisted = File.Exists(Path.Combine(root, "WTF", "Config.wtf")) };
        BackupFile(exe, state, report); BackupFile(Path.Combine(root, "WTF", "Config.wtf"), state, report);
        var wdb = Path.Combine(root, "WDB"); if (Directory.Exists(wdb)) { var moved = Path.Combine(root, "WDB_sauvegarde_" + DateTime.Now.ToString("yyyyMMdd_HHmmss")); Directory.Move(wdb, moved); state.WdbBackup = moved; report("WDB sauvegardé : " + moved); }
        CopyEmbedded("OctoWoW_VF_Installer.assets.patch-6.mpq", Path.Combine(data, "patch-6.mpq"), state, "Patch6", report);
        CopyEmbedded("OctoWoW_VF_Installer.assets.speech.MPQ", Path.Combine(data, "patch-7.mpq"), state, "Speech", report);
        PatchExe(exe, state, report); PatchConfig(Path.Combine(root, "WTF", "Config.wtf"), state, report);
        state.Installed = true; SaveState(root, state); report("Installation terminée"); report("Progress:100");
    }
    public void Uninstall(Action<string> report)
    {
        var root = FindClient(ClientCandidates, IsOcto) ?? SelectFolder("Sélectionnez le dossier OctoWoW"); if (root is null) throw new InvalidOperationException("Dossier introuvable.");
        report = WithFileLog(root, report); var statePath = Path.Combine(root, stateName); if (!File.Exists(statePath)) throw new InvalidOperationException("Aucun état d’installation trouvé.");
        var state = JsonSerializer.Deserialize<InstallState>(File.ReadAllText(statePath)) ?? throw new InvalidOperationException("État invalide.");
        foreach (var item in new[] { (state.Patch6Path, state.Patch6Hash), (state.SpeechPath, state.SpeechHash) }) if (item.Item1 is not null && File.Exists(item.Item1) && Hash(item.Item1) == item.Item2) { File.Delete(item.Item1); report("Supprimé : " + item.Item1); }
        Restore(state.ExeBackup, Path.Combine(root, "WoW.exe"), report); Restore(state.ConfigBackup, Path.Combine(root, "WTF", "Config.wtf"), report); if (!state.ConfigExisted) { var config = Path.Combine(root, "WTF", "Config.wtf"); if (File.Exists(config)) File.Delete(config); } if (state.WdbBackup is not null && Directory.Exists(state.WdbBackup)) { if (Directory.Exists(Path.Combine(root, "WDB"))) Directory.Delete(Path.Combine(root, "WDB"), true); Directory.Move(state.WdbBackup, Path.Combine(root, "WDB")); report("WDB restauré"); }
        File.Delete(statePath); report("Désinstallation terminée");
    }
    public void Verify(Action<string> report) { var root = FindClient(ClientCandidates, IsOcto) ?? SelectFolder("Sélectionnez le dossier OctoWoW"); if (root is null) throw new InvalidOperationException("Dossier introuvable."); report = WithFileLog(root, report); var s = Path.Combine(root, stateName); if (!File.Exists(s)) { report("Aucune installation VF enregistrée."); return; } var state = JsonSerializer.Deserialize<InstallState>(File.ReadAllText(s))!; report("Client : " + root); report("patch-6.mpq : " + (ExistsHash(state.Patch6Path, state.Patch6Hash) ? "OK" : "absent ou modifié")); report("patch-7.mpq : " + (ExistsHash(state.SpeechPath, state.SpeechHash) ? "OK" : "absent ou modifié")); report("État : " + (state.Installed ? "Installation active" : "inactif")); }
    public void Launch(Action<string> report)
    { var root = FindClient(ClientCandidates, IsOcto) ?? SelectFolder("Sélectionnez le dossier OctoWoW"); if (root is null) return; foreach (var name in new[] { "OctoLauncher.exe", "OctoWoWLauncher.exe", "Launcher.exe", "WoW.exe" }) { var p = Path.Combine(root, name); if (File.Exists(p)) { Process.Start(new ProcessStartInfo(p) { WorkingDirectory = root, UseShellExecute = true }); report("Lancement : " + name); return; } } throw new FileNotFoundException("Aucun launcher ou WoW.exe trouvé."); }
    static bool IsOcto(string p) => File.Exists(Path.Combine(p, "WoW.exe")) && Directory.Exists(Path.Combine(p, "Data")) && Directory.Exists(Path.Combine(p, "WTF"));
    static bool IsFrench(string p) => File.Exists(Path.Combine(p, "WoW.exe")) && File.Exists(Path.Combine(p, "Data", "speech.MPQ"));
    static string? FindClient(IEnumerable<string> candidates, Func<string, bool> valid) => candidates.FirstOrDefault(valid);
    static string? SelectFolder(string description) { using var d = new FolderBrowserDialog { Description = description, UseDescriptionForTitle = true }; return d.ShowDialog() == DialogResult.OK ? d.SelectedPath : null; }
    void CopyEmbedded(string resource, string dest, InstallState s, string kind, Action<string> report) { using var source = typeof(InstallerService).Assembly.GetManifestResourceStream(resource) ?? throw new FileNotFoundException("Ressource intégrée absente : " + resource); using var expectedSha = SHA256.Create(); var expected = Convert.ToHexString(expectedSha.ComputeHash(source)); if (File.Exists(dest)) { var existing = Hash(dest); if (!existing.Equals(expected, StringComparison.OrdinalIgnoreCase)) throw new IOException($"Le fichier existe déjà et ne correspond pas à la ressource VF : {dest}. Aucun fichier n’a été écrasé."); SetHash(s, kind, dest, existing); report("Déjà installé, vérifié SHA-256 : " + dest); return; } source.Position = 0; using var output = File.Create(dest); source.CopyTo(output); output.Flush(true); SetHash(s, kind, dest, expected); report($"Copié : {dest} (SHA-256 {expected})"); }
    static void SetHash(InstallState s, string kind, string path, string hash) { if (kind == "Patch6") { s.Patch6Path = path; s.Patch6Hash = hash; } else { s.SpeechPath = path; s.SpeechHash = hash; } }
    static void BackupFile(string path, InstallState s, Action<string> report) { if (!File.Exists(path)) return; var backup = path + ".vf-sauvegarde-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"); File.Copy(path, backup); if (path.EndsWith("WoW.exe")) s.ExeBackup = backup; else s.ConfigBackup = backup; report("Sauvegardé : " + backup); }
    static void PatchExe(string path, InstallState s, Action<string> report) { var b = File.ReadAllBytes(path); byte[] a = [0xA1,0xA4,0xA2,0xC2,0x00,0xC6,0x45,0xB1], r = [0xB8,0x52,0x46,0x72,0x66]; byte[] a2 = [0x33,0xF6,0x8B,0xFF,0x8B,0x04,0xB5,0xA4], r2 = [0xBE,0x02,0x00,0x00,0x00,0xEB,0x1F]; if (Match(b, 0x1B2115, r) && Match(b, 0x253C, r2)) { report("WoW.exe est déjà configuré en français"); return; } if (!Match(b, 0x1B2115, a) || !Match(b, 0x253C, a2)) throw new InvalidOperationException("WoW.exe ne correspond pas à la version attendue 1.12.1. Aucun octet n’a été modifié."); Array.Copy(r, 0, b, 0x1B2115, r.Length); Array.Copy(r2, 0, b, 0x253C, r2.Length); File.WriteAllBytes(path, b); s.ExePatched = true; report("WoW.exe modifié avec les signatures attendues."); }
    static void ValidateExe(string path) { var b = File.ReadAllBytes(path); byte[] a = [0xA1,0xA4,0xA2,0xC2,0x00,0xC6,0x45,0xB1], r = [0xB8,0x52,0x46,0x72,0x66], a2 = [0x33,0xF6,0x8B,0xFF,0x8B,0x04,0xB5,0xA4], r2 = [0xBE,0x02,0x00,0x00,0x00,0xEB,0x1F]; if (!(Match(b, 0x1B2115, a) && Match(b, 0x253C, a2)) && !(Match(b, 0x1B2115, r) && Match(b, 0x253C, r2))) throw new InvalidOperationException("WoW.exe ne correspond pas à la version attendue 1.12.1. Aucun fichier n’a été modifié."); }
    static void PatchConfig(string path, InstallState s, Action<string> report) { Directory.CreateDirectory(Path.GetDirectoryName(path)!); var text = File.Exists(path) ? File.ReadAllText(path) : ""; if (!text.Contains("SET locale \"frFR\"", StringComparison.OrdinalIgnoreCase)) text += Environment.NewLine + "SET locale \"frFR\"" + Environment.NewLine; File.WriteAllText(path, text, Encoding.ASCII); report("Config.wtf configuré en frFR"); }
    static bool Match(byte[] b, int offset, byte[] expected) => offset >= 0 && offset + expected.Length <= b.Length && b.AsSpan(offset, expected.Length).SequenceEqual(expected);
    static string Hash(string p) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(p)));
    static bool ExistsHash(string? p, string? h) => p is not null && h is not null && File.Exists(p) && Hash(p).Equals(h, StringComparison.OrdinalIgnoreCase);
    static void Restore(string? backup, string target, Action<string> report) { if (backup is not null && File.Exists(backup)) { File.Copy(backup, target, true); File.Delete(backup); report("Restauré : " + target); } }
    static Action<string> WithFileLog(string root, Action<string> report) { var log = Path.Combine(root, "OctoWoW_VF_installation.log"); return message => { File.AppendAllText(log, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}"); report(message); }; }
    static void SaveState(string root, InstallState state) => File.WriteAllText(Path.Combine(root, "OctoWoW_VF_installation.json"), JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true }));
    sealed class InstallState { public string? Root { get; set; } public string? FrenchRoot { get; set; } public DateTimeOffset InstalledAt { get; set; } public bool Installed { get; set; } public bool ExePatched { get; set; } public bool ConfigExisted { get; set; } public string? ExeBackup { get; set; } public string? ConfigBackup { get; set; } public string? WdbBackup { get; set; } public string? Patch6Path { get; set; } public string? Patch6Hash { get; set; } public string? SpeechPath { get; set; } public string? SpeechHash { get; set; } }
}
