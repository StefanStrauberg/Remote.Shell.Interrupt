namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Helper;

internal static class Helper
{
    public static string DictionaryToJson(Dictionary<string, List<string>> dict)
    {
        var entries = dict.Select(d =>
            string.Format("\"{0}\": [{1}]", d.Key, string.Join(",", d.Value)));
        return "{" + string.Join(",", entries) + "}";
    }
}
