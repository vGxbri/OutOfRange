using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BlurPausa : MonoBehaviour
{
    [Header("Configuración")]
    [Range(0f, 10f)]
    public float intensidadBlur = 3f;
    [Range(1, 4)]
    public int pasadas = 2;

    private Material materialBlur;
    private bool blurActivo = false;

    void Awake()
    {
        Shader shader = Shader.Find("Custom/BlurPausa");
        if (shader != null && shader.isSupported)
        {
            materialBlur = new Material(shader);
        }
    }

    public void ActivarBlur()
    {
        blurActivo = true;
    }

    public void DesactivarBlur()
    {
        blurActivo = false;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!blurActivo || materialBlur == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        materialBlur.SetFloat("_BlurSize", intensidadBlur);

        RenderTexture temp = RenderTexture.GetTemporary(src.width, src.height);

        // Aplicar blur en varias pasadas para mayor intensidad
        for (int i = 0; i < pasadas; i++)
        {
            if (i == 0)
                Graphics.Blit(src, temp, materialBlur, 0);   // Horizontal
            else
                Graphics.Blit(dest, temp, materialBlur, 0);

            Graphics.Blit(temp, dest, materialBlur, 1);      // Vertical
        }

        RenderTexture.ReleaseTemporary(temp);
    }

    void OnDestroy()
    {
        if (materialBlur != null)
            Destroy(materialBlur);
    }
}
