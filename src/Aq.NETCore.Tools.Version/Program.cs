using System;
using Newtonsoft.Json;

namespace Aq.NETCore.Tools.Version {
    public class Program {
        public static void Main(string[] args) {
            var options = Options.Parse(args);

            if (options == null) {
                Help();
                return;
            }

            switch (options.Display) {
                case null:
                case "all":
                case "json":
                case "long":
                case "short":
                case "as-is":
                case "major":
                case "minor":
                case "patch":
                case "release":
                case "build":
                    break;
                default:
                    Help();
                    return;
            }

            SemVer version;
            var projectFile = ProjectFile.Read(options.Filename);
            switch (options.Increment) {
                case null:
                    break;
                case "major":
                    version = projectFile.GetVersion();
                    version.IncrementMajor(options.Value == 0 ? 1 : options.Value);
                    projectFile.ReplaceVersion(version.Long);
                    break;
                case "minor":
                    version = projectFile.GetVersion();
                    version.IncrementMinor(options.Value == 0 ? 1 : options.Value);
                    projectFile.ReplaceVersion(version.Long);
                    break;
                case "patch":
                    version = projectFile.GetVersion();
                    version.IncrementPatch(options.Value == 0 ? 1 : options.Value);
                    projectFile.ReplaceVersion(version.Long);
                    break;
                case "release":
                    version = projectFile.GetVersion();
                    version.IncrementRelease(options.Value == 0 ? 1 : options.Value);
                    projectFile.ReplaceVersion(version.Long);
                    break;
                case "build":
                    version = projectFile.GetVersion();
                    version.IncrementBuild(options.Value == 0 ? 1 : options.Value);
                    projectFile.ReplaceVersion(version.Long);
                    break;
                default:
                    Help();
                    return;
            }

            switch (options.Display) {
                case "all":
                    version = projectFile.GetVersion();
                    Console.WriteLine(version.Major);
                    Console.WriteLine(version.Minor);
                    Console.WriteLine(version.Build);
                    Console.WriteLine(version.Release);
                    Console.WriteLine(version.Build);
                    break;
                case "json":
                    version = projectFile.GetVersion();
                    Console.WriteLine(JsonConvert.SerializeObject(new {
                        major = version.Major,
                        minor = version.Minor,
                        patch = version.Patch,
                        release = version.Release,
                        build = version.Build
                    }));
                    break;
                case "long":
                    Console.WriteLine(projectFile.GetVersion().Long);
                    break;
                case "short":
                    Console.WriteLine(projectFile.GetVersion().Short);
                    break;
                case "as-is":
                    Console.WriteLine(projectFile.Version);
                    break;
                case "major":
                    Console.WriteLine(projectFile.GetVersion().Major);
                    break;
                case "minor":
                    Console.WriteLine(projectFile.GetVersion().Minor);
                    break;
                case "patch":
                    Console.WriteLine(projectFile.GetVersion().Patch);
                    break;
                case "release":
                    Console.WriteLine(projectFile.GetVersion().Release);
                    break;
                case "build":
                    Console.WriteLine(projectFile.GetVersion().Build);
                    break;
            }
        }

        private static void Help() {
            Console.WriteLine("Usage: dotnet-version [FILENAME] [OPTION]...");
            Console.WriteLine("  or:  dotnet version [FILENAME] [OPTION]...");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -d, --display=DISPLAY      displays version read from (written to, if incrementing) FILENAME");
            Console.WriteLine("                               all: all version numbers on separate lines");
            Console.WriteLine("                               json: JSON with version numbers");
            Console.WriteLine("                               as-is: version as string");
            Console.WriteLine("                               major: MAJOR number");
            Console.WriteLine("                               minor: MINOR number");
            Console.WriteLine("                               patch: PATCH number");
            Console.WriteLine("                               release: RELEASE number");
            Console.WriteLine("                               build: BUILD number");
            Console.WriteLine("  -i, --increment=INCREMENT increments version and writes it back to FILENAME");
            Console.WriteLine("                               major: increment MAJOR number, zeroing MINOR and PATCH");
            Console.WriteLine("                               minor: increment MINOR number, zeroing PATCH");
            Console.WriteLine("                               patch: increment PATCH number");
            Console.WriteLine("                               release: increment RELEASE number");
            Console.WriteLine("                               build: increment BUILD number");
            Console.WriteLine("  -v, --value=VALUE         when incrementing, value to add to chosen number (default: 1)");
            Environment.Exit(1);
        }
    }
}
