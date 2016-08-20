using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Aq.NETCore.Tools.Version {
    public class SemVer {
        public string Long {
            get {
                if (this.Major.Length == 0) {
                    return "0.0";
                }
                var result = new StringBuilder(this.Major);
                result.Append('.');
                if (this.Minor.Length == 0) {
                    result.Append('0');
                }
                else {
                    result.Append(this.Minor);
                    if (this.Patch.Length > 0) {
                        result.Append('.');
                        result.Append(this.Patch);
                    }
                }
                if (this.Release.Length > 0) {
                    result.Append('-');
                    result.Append(this.Release);
                    if (this.Build.Length > 0) {
                        result.Append('+');
                        result.Append(this.Build);
                    }
                }
                return result.ToString();
            }
        }

        public string Short {
            get {

                if (this.Major.Length == 0) {
                    return "0.0";
                }
                var result = new StringBuilder(this.Major);
                result.Append('.');
                if (this.Minor.Length == 0) {
                    result.Append('0');
                }
                else {
                    result.Append(this.Minor);
                    if (this.Patch.Length > 0) {
                        result.Append('.');
                        result.Append(this.Patch);
                    }
                }
                return result.ToString();
            }
        }

        public string Source { get; }
        public string Major { get; set; }
        public string Minor { get; set; }
        public string Patch { get; set; }
        public string Release { get; set; }
        public string Build { get; set; }

        public SemVer(string version) {
            this.Source = version;

            var match = SemVerRx.Match(this.Source);

            if (!match?.Success ?? false) {
                throw new Exception($"Could not parse version string: {this.Source}");
            }

            // ReSharper disable once PossibleNullReferenceException
            this.Major = match.Groups["major"].Value;
            this.Minor = match.Groups["minor"].Value;
            this.Patch = match.Groups["patch"].Value;
            this.Release = match.Groups["release"].Value;
            this.Build = match.Groups["build"].Value;
        }

        public void IncrementMajor(int value) {
            int current;
            if (int.TryParse(this.Major, out current)) {
                this.Major = (current + value).ToString();
            }
            else {
                throw new Exception($"Could not increment MAJOR of {this.Source}");
            }
        }

        public void IncrementMinor(int value) {
            int current;
            if (int.TryParse(this.Minor, out current)) {
                this.Minor = (current + value).ToString();
            }
            else {
                throw new Exception($"Could not increment MINOR of {this.Source}");
            }
        }

        public void IncrementPatch(int value) {
            int current;
            if (int.TryParse(this.Patch, out current)) {
                this.Patch = (current + value).ToString();
            }
            else {
                throw new Exception($"Could not increment PATCH of {this.Source}");
            }
        }

        public void IncrementRelease(int value) {
            int current;
            if (int.TryParse(this.Release, out current)) {
                this.Release = (current + value).ToString();
            }
            else {
                throw new Exception($"Could not increment RELEASE of {this.Source}");
            }
        }

        public void IncrementBuild(int value) {
            int current;
            if (int.TryParse(this.Build, out current)) {
                this.Build = (current + value).ToString();
            }
            else {
                throw new Exception($"Could not increment BUILD of {this.Source}");
            }
        }

        private static readonly Regex SemVerRx = new Regex(@"
                ^
                (?<major>(?:\w+?)|\*)
                \.
                (?<minor>(?:\w+?)|\*)
                (?:
                    \.
                    (?<patch>(?:\w+?)|\*)
                )?
                (?:
                    \-
                    (?<release>(?:\w[^\+]*?)|\*)
                )?
                (?:
                    \+
                    (?<build>(?:\w.+?)|\*)
                )?
                $
            ",
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
    }
}
