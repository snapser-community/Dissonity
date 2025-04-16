using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class WebGLPostBuild : IPostprocessBuildWithReport
{
  public int callbackOrder => 0;

  public void OnPostprocessBuild(BuildReport report)
  {
    if (report.summary.platform != BuildTarget.WebGL)
      return;
#if DISCORD_BUILD
        string indexPath = Path.Combine(report.summary.outputPath, "index.html");

        if (!File.Exists(indexPath))
        {
            UnityEngine.Debug.LogError("index.html not found in WebGL build output.");
            return;
        }

        string htmlString = File.ReadAllText(indexPath);

        if (!htmlString.Contains("var unityInstance;") && htmlString.Contains("createUnityInstance"))
        {
            string tab = "  ";

            // Inject var declaration
            htmlString = htmlString.Replace(
                "createUnityInstance",
                $"var unityInstance;\n{tab}{tab}{tab}createUnityInstance"
            );

            // Inject assignment into the .then(...) chain
            htmlString = htmlString.Replace(
                "})",
                "}).then(instance => {\n" +
                $"{tab}{tab}{tab}{tab}unityInstance = instance;\n" +
                $"{tab}{tab}{tab}}})"
            );
        }

        File.WriteAllText(indexPath, htmlString);
        UnityEngine.Debug.Log("WebGL post-build: index.html updated.");
#endif
  }
}
