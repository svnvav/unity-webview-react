using UnityEngine;

public class AppController : MonoBehaviour
{
  public WebViewController WebViewController;

  public void LoadAboutComponent() {

    string data = "[" +
                    "{" +
                      "headerText: '" + "General characteristics" + "'," +
                      "values: [" +
                        "{" +
                          "label: '" + "Platform" + "'," +
                          "value: 'Android'" +
                        "}," +
                        "{" +
                          "label: '" + "Two SIM card support" + "'," +
                          "value: '"+ "yes" + "'" +
                        "}," +
                        "{" +
                          "label: '" + "Camera" + "'," +
                          "value: '13 " + "MP" + "'" +
                        "}," +
                        "{" +
                          "label: '" + "Memory" + "'," +
                          "value: '32/64 " + "GB" + "'" +
                        "}," +
                        "{" +
                          "label: '" + "RAM" + "'," +
                          "value: '3 " + "GB" + "'" +
                        "}," +
                        "{" +
                          "label: '" + "Battery" + "'," +
                          "value: '4100 " + "mAh" + "'" +
                        "}," +
                        "{" +
                          "label: '" + "Weight" + "'," +
                          "value: '175 " + "g" + "'" +
                        "}," +
                        "{" +
                          "label: '" + "Size(WxHxT)"+"'," +
                          "value: '76x151x8.45" + "mm" + "'" +
                        "}" +
                      "]" +
                    "}," +
                    "{" +
                      "headerText: '" + "Screen" + "'," +
                      "values: [" +
                        "{" +
                          "label: '" + "Diagonal" + "'," +
                          "value: '5.5\"'" +
                        "},    " +
                        "{" +
                          "label: '" + "Resolution" + "'," +
                          "value: '1920x1080'" +
                        "}" +
                      "]" +
                    "}," +
                    "{" +
                      "headerText: '" + "Communication" + "'," +
                      "values: [" +
                        "{" +
                          "label: '" + "Standart" + "'," +
                          "value: 'GSM 900/1800/1900, 3G, 4G LTE, LTE-A'" +
                        "}," +
                        "{" +
                          "label: 'Wi-Fi'," +
                          "value: '" + "yes" + "'" +
                        "}," +
                        "{" +
                          "label: 'Bluetooth'," +
                          "value: '"+"yes" + "'" +
                        "}" +
                      "]" +
                    "}" +
                  "]";
    
    WebViewController.LoadPage("about-component.html", "window.createAboutComponent({list: " + data + "});");
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      Application.Quit();
    }
  }
}