using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class StringExt
{
    private const string emailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

    private const string namePattern = "^[A-Za-z0-9]*$";

    public static bool IsValidMailAddress(this string email)
            => email != null
            && Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);

    public static bool IsValidName(this string name)
        => name != null
        && Regex.IsMatch(name, namePattern, RegexOptions.IgnoreCase);

    public static readonly char[] CHARS = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
    public static string RandString(uint wordCount, uint wordslength)
    {
        string glitchedString = "";

        for (int i = 0; i < wordCount; i++)
        {
            for (int j = 0; j < wordslength; j++)
            {

                glitchedString += CHARS[Random.Range(0, CHARS.Length - 1)];
            }
            glitchedString += " "; //Add spaces
        }
        return glitchedString;
    }
    public static string DebugLogLines(LogType logType, params string[] lines)
        => lines.DebugLogLines(logType);
    public static string DebugLogLines(params string[] lines)
        => lines.DebugLogLines();
    public static string DebugLogLines(this IEnumerable<string> lines, LogType logType = LogType.Log, Object context = null, string tag = null)
    {
        string msg = string.Join("\n", lines.NotNullOrWhiteSpace());
        Debug.unityLogger.Log(logType, tag, msg, context);
        return msg;
    }
    public static IEnumerable<string> NotNullOrWhiteSpace(this IEnumerable<string> strings)
        => strings.Where((x) => !string.IsNullOrWhiteSpace(x));
    public static string ToDisplayName(this string str)
        => SplitCamelCase(FirstLetterToUpper(str));
    public static string SplitCamelCase(this string str)
        => Regex.Replace(str, "(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])|(?<=[0-9])(?=[A-Z][a-z])|(?<=[a-zA-Z])(?=[0-9])", " ", RegexOptions.Compiled).Trim();
    public static string FirstLetterToUpper(this string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
        {
            if (str[0] == '_')
                str = str.Substring(1);

            return char.ToUpper(str[0]) + str.Substring(1);
        }

        return str.ToUpper();
    }
    public static string RemoveAllWhitespaces(this string str)
        => string.Concat(str.Where(c => !char.IsWhiteSpace(c)));

    public static string RemoveSpecialCharacters(this string str)
        => Regex.Replace(str, "[^a-zA-Z0-9% ._]", string.Empty);

    public static string SpaceToUnderScore(this string str)
        => str.Replace(' ', '_');

    public static string ToLines(this IEnumerable<string> lines)
        => string.Join("\n", lines);

    public static IEnumerable<string> ToLines(this string lines)
        => lines.Split("\n").Trim();
    public static IEnumerable<string> Trim(this IEnumerable<string> strs)
    {
        foreach (var str in strs)
            yield return str.Trim();
    }
}
