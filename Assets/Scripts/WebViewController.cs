using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class WebViewController : MonoBehaviour {

	public IWebView WebView;
  public GreeWebViewWrapper GreeWebView;

  private string
    persistentComponentsPath, //for Android
    streamingComponentsPath, //for others
    componentsPath;


  private void Awake() {
    WebView = GreeWebView;
    GreeWebView.gameObject.SetActive(true);
    streamingComponentsPath = Application.streamingAssetsPath + "/WebComponents/";
    persistentComponentsPath = Application.persistentDataPath + "/WebComponents/";
    #if UNITY_ANDROID && !UNITY_EDITOR
    componentsPath = persistentComponentsPath;
    StartCoroutine(LoadFilesFromDirectory(streamingComponentsPath));
    #else
    componentsPath = streamingComponentsPath;
    #endif
  }
  
  //чтобы можно было использовать, как обработчик нажатия кнопки
  public void LoadPage(string url) {
    LoadPage(url, "");
  }

  public void LoadPage(string url, string jsEvalString) {
    WebView.Init();

    if (url.StartsWith("https")) {
      WebView.LoadWebPage(url.Replace(" ", "%20"), jsEvalString);
    }
    else if (url.StartsWith("http")) {
      #if UNTIY_IOS //iOS не пропускает запросы по незащищенному протоколу
      Application.OpenURL(url);
      WebView.Hide();
      #else
      WebView.LoadWebPage(url.Replace(" ", "%20"), jsEvalString);
      #endif
    }
    else {
      var dst = Path.Combine(componentsPath, url);
      WebView.LoadLocalComponent("file://" + dst.Replace(" ", "%20"), jsEvalString);
    }
  }


  #region Loading region

  /// <summary>
  /// Копирует файлы из path в persistentComponentsPath
  /// </summary>
  IEnumerator LoadFilesFromDirectory(string path) {
    
    if (!Directory.Exists(persistentComponentsPath)) {
      Directory.CreateDirectory(
        persistentComponentsPath
      );
    }
    
    byte[] result;
    
    string[] filesTable = new string[] {
      "about-component.html",
      "about-component.js",
      "OpenSans-Regular.ttf"
    };
    WWW _www;
    foreach (string item in filesTable) {
      _www = new WWW(streamingComponentsPath + item);
      yield return _www;
      result = _www.bytes;
      var dst = Path.Combine(persistentComponentsPath, item);
      File.WriteAllBytes(dst, result);
    }
  }

  void OnDestroy() {
    #if UNITY_ANDROID
    RecursiveRemoveFolders(persistentComponentsPath);
    #endif
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
