using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class WebViewController : MonoBehaviour {

	public IWebView WebView;
  
  public GreeWebViewWrapper GreeWebView;


  private void Awake() {
    WebView = GreeWebView;
    GreeWebView.gameObject.SetActive(true);
    StartCoroutine(LoadFilesFromDirectory(Application.streamingAssetsPath + "/WebComponents/"));
  }
  
  //for availability of using buttons
  public void LoadPage(string url) {
    LoadPage(url, "");
  }

  public void LoadPage(string url, string jsEvalString) {
    WebView.Init();

    if (url.StartsWith("https")) {    
      WebView.LoadWebPage(url.Replace(" ", "%20"), jsEvalString);
    }
    else if (url.StartsWith("http")) {
      #if UNTIY_IOS
      Application.OpenURL(url);
      WebView.Hide();
      #else
      WebView.LoadWebPage(url.Replace(" ", "%20"), jsEvalString);
      #endif
    }
    else {
      var dst = Path.Combine(Application.persistentDataPath + "/WebComponents", url);
      WebView.LoadLocalComponent("file://" + dst.Replace(" ", "%20"), jsEvalString);
    }
  }


  #region Loading region

  List<string> pathsStack = new List<string>();

  private string GetPathFromStack() {
    string result = string.Empty;
    for (int i = 0; i < pathsStack.Count; i++) {
      result += "/" + pathsStack[i];
    }

    return result;
  }

  IEnumerator LoadFilesFromDirectory(string path) {
    
    if (!Directory.Exists(Application.persistentDataPath + "/WebComponents/")) {
      Directory.CreateDirectory(
        Application.persistentDataPath + "/WebComponents/"
      );
    }
    
    byte[] result;
    
#if UNITY_ANDROID && !UNITY_EDITOR
    string[] filesTable = new string[] {
      "about-component.html",
      "about-component.js",
      "OpenSans-Regular.ttf"
    };
    WWW _www;
    foreach (string item in filesTable) {
      _www = new WWW(Application.streamingAssetsPath + "/WebComponents/" + item);
      yield return _www;
      result = _www.bytes;
      var dst = Path.Combine(Application.persistentDataPath + "/WebComponents", item);
      File.WriteAllBytes(dst, result);
    }
#else
    string[] Directories = Directory.GetDirectories(path);
    int slashIndex;
    if (Directories.Length > 0) {
      foreach (string tempPath in Directories) {
        slashIndex = tempPath.LastIndexOf("/");
        string folderName = tempPath.Substring(
          slashIndex + 1,
          tempPath.Length - slashIndex - 1
        );
        pathsStack.Add(folderName);

        if(!Directory.Exists(Application.persistentDataPath + "/WebComponents/" + GetPathFromStack())){
          Directory.CreateDirectory(
            Application.persistentDataPath + "/WebComponents/" + GetPathFromStack()
          );     
        }

        yield return LoadFilesFromDirectory(tempPath);
        pathsStack.Remove(pathsStack.Last());
      }
    }

    string[] files = Directory.GetFiles(path);

    foreach (string src in files) {

      slashIndex = src.LastIndexOf("/");
      var dst = src.Substring(slashIndex + 1, src.Length - slashIndex - 1);
      dst = Path.Combine(Application.persistentDataPath + "/WebComponents/" +
                         GetPathFromStack(), dst);

      result = File.ReadAllBytes(src);

      File.WriteAllBytes(dst, result);
    }

#endif
  }

  void OnDestroy() {
    RecursiveRemoveFolders(Application.persistentDataPath + "/WebComponents");
  }

  private void RecursiveRemoveFolders(string path) {
    if (!Directory.Exists(path))
      return;
    string[] Directories = Directory.GetDirectories(path);
    if (Directories.Length > 0) {
      foreach (string tempPath in Directories) {
        RecursiveRemoveFolders(tempPath);
      }
    }

    string[] filesDst = Directory.GetFiles(path);
    foreach (string item in filesDst) {
      File.Delete(item);
    }
  }

  #endregion
}
