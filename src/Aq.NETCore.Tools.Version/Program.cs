using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aq.NETCore.Tools.Version {
    public class Program {
        public static void Main(string[] args) {
            if (args.Length > 0) {
                switch (args[0]) {
                    case "--help":
                        Help(args.Skip(1));
                        return;
                    case "increment":
                        Increment(args.Skip(1));
                        return;
                }
            }
            Help(null);
        }

        static void Help(IEnumerable<string> args) {
            Console.WriteLine("Usage: dotnet version <command>");
            Console.WriteLine("Available commands:");
            Console.WriteLine("  increment - increments project version");

            if (args == null || args.Any()) {
                Environment.Exit(1);
            }
        }

        static void Increment(IEnumerable<string> args) {
            var enumerator = args.GetEnumerator();

            string which;
            if (enumerator.MoveNext()) {
                switch (enumerator.Current) {
                    case "major":
                        which = "major";
                        break;
                    case "minor":
                        which = "minor";
                        break;
                    case "patch":
                        which = "patch";
                        break;
                    default:
                        IncrementHelp();
                        return;
                }
            }
            else {
                which = "patch";
            }

            if (enumerator.MoveNext()) {
                IncrementHelp();
                return;
            }

            Action<string> replaceVersion;
            var text = File.ReadAllText("project.json");
            var version = FindVersion(text, out replaceVersion);
            var oldVersion = version.ToString(3);

            switch (which) {
                case "major":
                    version = new System.Version(version.Major + 1, 0, 0);
                    break;
                case "minor":
                    version = new System.Version(version.Major, version.Minor + 1, 0);
                    break;
                case "patch":
                    version = new System.Version(version.Major, version.Minor, version.Build + 1);
                    break;
            }

            replaceVersion(version.ToString(3));
            Console.WriteLine($"{oldVersion} -> {version.ToString(3)}");
        }

        static void IncrementHelp() {
            Console.WriteLine("Usage: dotnet version increment <which>");
            Console.WriteLine("  which - what number to increment: major, minor or patch (default: patch)");
            Environment.Exit(1);
        }

        static System.Version FindVersion(string text, out Action<string> replaceVersion) {
            var reader = new JsonTextReader(new StringReader(text));

            if (!reader.Read()) {
                throw new Exception("Unexpected end of project.json");
            }

            if (reader.TokenType != JsonToken.StartObject) {
                throw new Exception(
                    "Invalid project.json format. Expected " +
                    $"JsonToken.StartObject, got: {reader.TokenType}");
            }

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.PropertyName) {
                    if (reader.ValueType == typeof (string)) {
                        if ("version" == (string) reader.Value) {
                            if (reader.Read()) {
                                if (reader.TokenType == JsonToken.String) {
                                    if (reader.ValueType == typeof (string)) {
                                        var linePosition = reader.LinePosition;
                                        var versionString = (string) reader.Value;
                                        var offset = linePosition - versionString.Length - 2;

                                        replaceVersion = value => {
                                            var m = new Regex(@".+?\r?\n", RegexOptions.Multiline).Matches(text);
                                            var l = m.Cast<Match>().Select(x => x.Value).ToList();
                                            var ln = reader.LineNumber - 1; // Json.NET line number is 1-based
                                            l[ln] = l[ln].Substring(0, offset) + "\"" +
                                                value + "\"" + l[ln].Substring(linePosition);
                                            File.WriteAllText("project.json", string.Join("", l));
                                        };

                                        return new System.Version(versionString);
                                    }
                                    else {
                                        throw new Exception(
                                            "Invalid project.json format. Expected " +
                                            $"string, got: {reader.ValueType}");
                                    }
                                }
                                else {
                                    throw new Exception(
                                        "Invalid project.json format. Expected " +
                                        $"JsonToken.String, got: {reader.TokenType}");
                                }
                            }
                            else {
                                throw new Exception("Unexpected end of project.json");
                            }
                        }
                        else {
                            reader.Skip();
                        }
                    }
                    else {
                        throw new Exception(
                            "Invalid project.json format. Expected " +
                            $"string, got: {reader.ValueType}");
                    }
                }
                else {
                    throw new Exception(
                        "Invalid project.json format. Expected " +
                        $"JsonToken.PropertyName, got: {reader.TokenType}");
                }
            }

            throw new Exception("Unexpected end of project.json");
        }
    }
}
