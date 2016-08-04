using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using CLAP;
using CLAP.Validation;

namespace UpdateVersionInfo
{
    internal class UpdateVersionInfoImpl
    {
        private const string AssemblyVersionExpression = @"^\s*\[assembly:\s*(?<attribute>(?:System\.)?(?:Reflection\.)?AssemblyVersion(?:Attribute)?\s*\(\s*""(?<version>[^""]+)""\s*\)\s*)\s*\]\s*$";
        private const string AssemblyFileVersionExpression = @"^\s*\[assembly:\s*(?<attribute>(?:System\.)?(?:Reflection\.)?AssemblyFileVersion(?:Attribute)?\s*\(\s*""(?<version>[^""]+)""\s*\)\s*)\s*\]\s*$";

        private static readonly Regex AssemblyVersionRegEx = new Regex(AssemblyVersionExpression, RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex AssemblyFileVersionRegEx = new Regex(AssemblyFileVersionExpression, RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Shows help/usage information
        /// </summary>
        /// <param name="help"></param>
        [Empty, Help]
        public static void Help(string help)
        {
            // this is an empty handler that prints
            // the automatic help string to the console.

            Console.WriteLine(help);
        }
        [Verb(IsDefault = true)]
        public static void Version(
            [DefaultValue(1), MoreOrEqualTo(0), Description("A numeric major version number greater than zero."), Aliases("v")]int major,
            [DefaultValue(0), MoreOrEqualTo(0), Description("A numeric minor number equal or greater than zero."), Aliases("m")]int minor,
            [DefaultValue(0), MoreOrEqualTo(0), Description("A numeric build number equal or greater than zero."), Aliases("b")]int build,
            [DefaultValue(0), MoreOrEqualTo(0), Description("A numeric revision number equal or greater than zero."), Aliases("r")]int revision,
            [Description("The path to a C# file to update with version information."), Aliases("p,path")]string versionCsPath,
            [Description("The path to an android manifest file to update with version information."), Aliases("a,androidManifest")]string androidManifestPath,
            [Description("The path to an iOS plist file to update with version information."), Aliases("t,touchPlist")]string touchPListPath)
        {
            try
            {
                var version = new Version(
                    major,
                    minor,
                    build,
                    revision);

                if (!string.IsNullOrEmpty(versionCsPath) && IsValidCSharpVersionFile(versionCsPath))
                {
                    UpdateCsVersionInfo(versionCsPath, version);
                }
                if (!string.IsNullOrEmpty(androidManifestPath) && IsValidAndroidManifest(androidManifestPath))
                {
                    UpdateAndroidVersionInfo(androidManifestPath, version);
                }
                if (!string.IsNullOrEmpty(touchPListPath) && IsValidTouchPList(touchPListPath))
                {
                    UpdateTouchVersionInfo(touchPListPath, version);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An unexpected error was encountered:" + e.Message);
            }
        }
        private static void UpdateCsVersionInfo(string path, Version version)
        {
            string contents;
            using (var reader = new StreamReader(path))
            {
                contents = reader.ReadToEnd();
            }
            contents = AssemblyVersionRegEx.Replace(contents,
                $"[assembly: System.Reflection.AssemblyVersion(\"{version}\")]");
            if (AssemblyFileVersionRegEx.IsMatch(contents))
            {
                contents = AssemblyFileVersionRegEx.Replace(contents,
                    $"[assembly: System.Reflection.AssemblyFileVersion(\"{version}\")]");
            }
            using (var writer = new StreamWriter(path, false))
            {
                writer.Write(contents);
            }
        }

        private static void UpdateAndroidVersionInfo(string path, Version version)
        {
            const string androidNs = "http://schemas.android.com/apk/res/android";
            var versionCodeAttributeName = XName.Get("versionCode", androidNs);
            var versionNameAttributeName = XName.Get("versionName", androidNs);
            var doc = XDocument.Load(path);
            // ReSharper disable once PossibleNullReferenceException
            doc.Root.SetAttributeValue(versionCodeAttributeName, version.Build);
            doc.Root.SetAttributeValue(versionNameAttributeName, $"{version.Major}.{version.Minor}");
            doc.Save(path);
        }

        private static void UpdateTouchVersionInfo(string path, Version version)
        {
            var doc = XDocument.Load(path);

            var shortVersionElement = doc.XPathSelectElement("plist/dict/key[string()='CFBundleShortVersionString']").NextNode as XElement;
            // ReSharper disable once PossibleNullReferenceException
            shortVersionElement.Value = $"{version.Major}.{version.Minor}";

            var bundleVersionElement = doc.XPathSelectElement("plist/dict/key[string()='CFBundleVersion']").NextNode as XElement;
            // ReSharper disable once PossibleNullReferenceException
            bundleVersionElement.Value = $"{version.Build}.{version.Revision}";

            doc.Save(path);
        }

        private static bool IsValidCSharpVersionFile(string path)
        {
            if (!File.Exists(path)) return false;
            if ((new FileInfo(path).Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) return false;

            try
            {
                string contents;
                using (var reader = new StreamReader(path))
                {
                    contents = reader.ReadToEnd();
                }

                if (AssemblyVersionRegEx.IsMatch(contents))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError(e.Message);
            }

            return false;
        }

        private static bool IsValidAndroidManifest(string path)
        {
            if (!File.Exists(path)) return false;
            if ((new FileInfo(path).Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) return false;

            try
            {
                // <manifest ...
                var doc = XDocument.Load(path);
                var rootElement = doc.Root;
                if (rootElement != null && rootElement.Name == "manifest") return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError(e.Message);
            }
            return false;
        }

        private static bool IsValidTouchPList(string path)
        {
            if (!File.Exists(path)) return false;
            if ((new FileInfo(path).Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) return false;

            try
            {
                //<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
                var doc = XDocument.Load(path);
                // ReSharper disable once PossibleNullReferenceException
                if (doc.DocumentType.Name == "plist")
                {
                    var shortVersionElement = doc.XPathSelectElement("plist/dict/key[string()='CFBundleShortVersionString']");
                    var valueElement = shortVersionElement?.NextNode as XElement;
                    if (valueElement != null && valueElement.Name == "string") return true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError(e.Message);
            }

            return false;
        }
    }
}
