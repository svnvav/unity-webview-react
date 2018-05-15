
public interface IWebView
{

  void Init();
  
  void LoadWebPage(string url, string jsEvalString);
  
  void LoadLocalComponent(string url, string jsEvalString);

  void Show();

  void Hide();

  void Destroy();
}
