using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ImageDownloader
{
    public static async UniTask<Texture2D> DownloadImageTexture(string url)
    {
        using var webRequest = UnityWebRequestTexture.GetTexture(url);
        var requestOperation = webRequest.SendWebRequest();

        await UniTask.WaitWhile(() => !requestOperation.isDone);

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{webRequest.error}");
            
            return null;
        }
        
        return DownloadHandlerTexture.GetContent(webRequest);
    }
}