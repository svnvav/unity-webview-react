/*
 * Copyright (C) 2012 GREE, Inc.
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *claim that you wrote the original software. If you use this software
 *in a product, an acknowledgment in the product documentation would be
 *appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GreeWebViewWrapper : MonoBehaviour, IWebView
{
  public WebViewObject WebView;
  public GameObject NavigationPanel;
  public Button BackButton;
  public Button ForwardButton;

  private string m_jsEvalString = "";

  public void Init() {
    
    WebView.Init(
      cb: msg => { onCall(msg); },
      ld: msg => { onLoad(msg); },
      enableWKWebView: true,
      transparent: true
    );
    
    WebView.SetVisibility(false);
  }

  /// <summary>
  /// callback-метод, который обрабатывает сообщения, полученные от Веб-компонентов
  /// </summary>
  private void onCall(string msg) {
    if (NavigationPanel.activeInHierarchy) {
      BackButton.interactable = WebView.CanGoBack();
      ForwardButton.interactable = WebView.CanGoForward();
    }

    string[] parameters = msg.Trim().Split(' ');

    switch (parameters[0]) {
      case "close":
        Hide();
        break;
    }
  }

  /// <summary>
  /// callback-метод, который выполняется по завершении загрузки компонента
  /// </summary>
  /// <param name="msg">url загруженного ресурса</param>
  private void onLoad(string msg) {
    if (msg.StartsWith("http")) {
      NavigationPanel.SetActive(true);
      BackButton.interactable = WebView.CanGoBack();
      ForwardButton.interactable = WebView.CanGoForward();
    }

    if (!string.IsNullOrEmpty(m_jsEvalString)) {
      WebView.EvaluateJS(m_jsEvalString);
      m_jsEvalString = "";
    }

#if !UNITY_ANDROID || UNITY_EDITOR
    //Не в Android необходимо инициализировать объект Unity с методом call,
    //чтобе обеспечить компоненту возможность взаимодествия с приложением
    WebView.EvaluateJS(@"
				 window.Unity = {
					call: function(msg) { 
					   var iframe = document.createElement('IFRAME'); 
					   iframe.setAttribute('src', 'unity:' + msg); 
					   document.documentElement.appendChild(iframe); 
					   iframe.parentNode.removeChild(iframe); 
					   iframe = null; 
					} 
				 };
			");
#endif
    Show();
  }

  public void LoadWebPage(string url, string jsEvalString) {
    m_jsEvalString = jsEvalString;
    NavigationPanel.SetActive(true);
    WebView.SetMargins(0, 100, 0, 0);//to place navigation bar
    WebView.LoadURL(url);
  }
  
  public void LoadLocalComponent(string urlPath, string jsEvalString) {
    Debug.Log(urlPath);
    m_jsEvalString = jsEvalString;
    NavigationPanel.SetActive(false);
    WebView.SetMargins(0, 0, 0, 0);
    WebView.LoadURL(urlPath);
  }

  public void Show() {
    WebView.SetVisibility(true);
  }

  public void Hide() {
    NavigationPanel.SetActive(false);
    WebView.SetVisibility(false);
  }

  public void Destroy() {
    Hide();
    DestroyImmediate(WebView);
  }
}