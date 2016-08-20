using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Aq.NETCore.Tools.Version {
    public class ProjectFile {
        public static ProjectFile Read(string path) {
            var result = new ProjectFile(path ?? "project.json");
            result.Read();
            return result;
        }

        public string Version { get; set; }

        public SemVer GetVersion() {
            return new SemVer(this.Version);
        }

        public void ReplaceVersion(string version) {
            var matches = LineBreaksRx.Matches(this.Text);
            var lines = matches.Cast<Match>().Select(x => x.Value).ToList();
            var line = lines[this.VersionLine];

            lines[this.VersionLine] =
                line.Substring(0, this.VersionLeft) +
                version + line.Substring(this.VersionRight);

            File.WriteAllText(this.Path, string.Join("", lines));
            this.Read();
        }

        private static readonly Regex LineBreaksRx = new Regex(
            @".+?(?:(?>\u000D\u000A)|[\u000A\u000B\u000C\u000D\u0085\u2028\u2029])",
            RegexOptions.Compiled | RegexOptions.Multiline);

        private string Path { get; }

        private string Text { get; set; }
        private int VersionLine { get; set; }
        private int VersionLeft { get; set; }
        private int VersionRight { get; set; }

        private ProjectFile(string path) {
            this.Path = path;
        }

        private void Read() {
            this.Text = File.ReadAllText(this.Path);

            using (var stringReader = new StringReader(this.Text)) {
                var jsonReader = new JsonTextReader(stringReader);

                if (!jsonReader.Read()) {
                    throw new Exception("Unexpected end of file");
                }

                if (jsonReader.TokenType != JsonToken.StartObject) {
                    throw new Exception(
                        "Invalid file format. Expected " +
                        $"JsonToken.StartObject, got: {jsonReader.TokenType}");
                }

                while (jsonReader.Read()) {
                    if (jsonReader.TokenType == JsonToken.PropertyName) {
                        if (jsonReader.ValueType == typeof (string)) {
                            if ("version" == (string) jsonReader.Value) {
                                if (jsonReader.Read()) {
                                    if (jsonReader.TokenType == JsonToken.String) {
                                        if (jsonReader.ValueType == typeof (string)) {
                                            this.Version = (string) jsonReader.Value;

                                            // Json.NET line number is 1-based
                                            this.VersionLine = jsonReader.LineNumber - 1;

                                            // -1 for quotes
                                            this.VersionRight = jsonReader.LinePosition - 1;
                                            this.VersionLeft = this.VersionRight - this.Version.Length;
                                            return;
                                        }
                                        else {
                                            throw new Exception(
                                                "Invalid project.json format. Expected " +
                                                $"string, got: {jsonReader.ValueType}");
                                        }
                                    }
                                    else {
                                        throw new Exception(
                                            "Invalid project.json format. Expected " +
                                            $"JsonToken.String, got: {jsonReader.TokenType}");
                                    }
                                }
                                else {
                                    throw new Exception("Unexpected end of project.json");
                                }
                            }
                            else {
                                jsonReader.Skip();
                            }
                        }
                        else {
                            throw new Exception(
                                "Invalid file format. Expected " +
                                $"string, got: {jsonReader.ValueType}");
                        }
                    }
                    else {
                        throw new Exception(
                            "Invalid file. Expected " +
                            $"JsonToken.PropertyName, got: {jsonReader.TokenType}");
                    }
                }
            }

            throw new Exception("Unexpected end of file");
        }
    }
}
