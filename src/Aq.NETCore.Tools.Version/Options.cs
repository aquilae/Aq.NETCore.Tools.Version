using System;

namespace Aq.NETCore.Tools.Version {
    public class Options {
        public static Options Parse(string[] args) {
            var options = new Options();

            var state = ParserState.Root;
            foreach (var arg in args) {
                int value;
                switch (state) {
                    case ParserState.Root:
                        if (arg == "-h") {
                            return null;
                        }
                        if (arg == "--help") {
                            return null;
                        }

                        if (arg == "-i") {
                            if (options.Increment != null) {
                                return null;
                            }
                            state = ParserState.Increment;
                        }
                        else if (arg.StartsWith("--increment=")) {
                            if (options.Increment != null) {
                                return null;
                            }
                            options.Increment = arg.Substring("--increment=".Length);
                        }
                        else if (arg == "-v") {
                            if (options.Value != 0) {
                                return null;
                            }
                            state = ParserState.Value;
                        }
                        else if (arg.StartsWith("--value=")) {
                            if (options.Value != 0) {
                                return null;
                            }
                            if (!int.TryParse(arg.Substring("--value=".Length), out value)) {
                                return null;
                            }
                            options.Value = value;
                        }
                        else if (arg == "-d") {
                            if (options.Display != null) {
                                return null;
                            }
                            state = ParserState.Display;
                        }
                        else if (arg.StartsWith("--display=")) {
                            if (options.Display != null) {
                                return null;
                            }
                            options.Display = arg.Substring("--display=".Length);
                        }
                        else {
                            if (options.Filename != null) {
                                return null;
                            }
                            options.Filename = arg;
                        }
                        break;
                    case ParserState.Increment:
                        options.Increment = arg;
                        state = ParserState.Root;
                        break;
                    case ParserState.Value:
                        if (!int.TryParse(arg, out value)) {
                            return null;
                        }
                        if (options.Value != 0) {
                            return null;
                        }
                        options.Value = value;
                        state = ParserState.Root;
                        break;
                    case ParserState.Display:
                        options.Display = arg;
                        state = ParserState.Root;
                        break;
                }
            }

            if (state != ParserState.Root) {
                return null;
            }

            switch (options.Increment) {
                case null:
                    if (options.Value != 0) {
                        return null;
                    }
                    break;
                case "major":
                case "minor":
                case "patch":
                case "release":
                case "build":
                    break;
                default:
                    return null;
            }

            switch (options.Display) {
                case null:
                    options.Display = "as-is";
                    break;
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
                    return null;
            }

            return options;
        }

        public string Filename { get; private set; }
        public string Increment { get; private set; }
        public string Display { get; private set; }
        public int Value { get; private set; }

        private enum ParserState {
            Root,
            Increment,
            Display,
            Value
        }
    }
}
