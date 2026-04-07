using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManejoDeSom : MonoBehaviour
{
    // Instância Singleton
    public static ManejoDeSom Instance { get; private set; }

    // Fonte principal para sons 2D (UI, música de fundo, etc.)
    // Será criada automaticamente se não houver um AudioSource no GameObject
    private AudioSource _systemSource;

    // Lista de AudioSources usados para sons 3D em loop/controle (ambiente, objetos etc.)
    private List<AudioSource> _activeSources = new List<AudioSource>();

    // Configurações padrão para fontes 3D criadas em tempo de execução
    [SerializeField] private float defaultMinDistance = 1f;
    [SerializeField] private float defaultMaxDistance = 500f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Tenta obter um AudioSource existente no mesmo GameObject; caso não exista, cria um
        _systemSource = GetComponent<AudioSource>();
        if (_systemSource == null)
        {
            _systemSource = gameObject.AddComponent<AudioSource>();
            _systemSource.playOnAwake = false;
            _systemSource.spatialBlend = 0f; // 2D
        }
    }

    #region 2D Sound Controls (systemSource)

    // Toca um clip em loop (ou não, dependendo do parâmetro)
    public void Play2D(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (clip == null) return;
        _systemSource.Stop();
        _systemSource.clip = clip;
        _systemSource.volume = volume;
        _systemSource.loop = loop;
        _systemSource.spatialBlend = 0f;
        _systemSource.Play();
    }

    public void Pause2D()
    {
        if (_systemSource.isPlaying)
            _systemSource.Pause();
    }

    public void Resume2D()
    {
        if (_systemSource.clip != null && !_systemSource.isPlaying)
            _systemSource.UnPause();
    }

    public void Stop2D()
    {
        _systemSource.Stop();
        _systemSource.clip = null;
    }

    // Versão one-shot para 2D (não altera o clip principal)
    public void Play2DOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        _systemSource.spatialBlend = 0f;
        _systemSource.PlayOneShot(clip, volume);
    }

    #endregion

    #region 3D Sound Controls (activeSources)

    // Cria e retorna um AudioSource 3D já tocando (loop se desejado). O chamador pode guardar a referência
    public AudioSource Play3D(AudioClip clip, Vector3 position, float volume = 1f, bool loop = true)
    {
        if (clip == null) return null;

        GameObject go = new GameObject("3DSound_" + clip.name);
        go.transform.position = position;
        go.transform.parent = this.transform; // organiza na hierarquia

        AudioSource src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.spatialBlend = 1f; // totalmente 3D
        src.volume = volume;
        src.loop = loop;
        src.minDistance = defaultMinDistance;
        src.maxDistance = defaultMaxDistance;
        src.playOnAwake = false;
        src.Play();

        _activeSources.Add(src);

        if (!loop)
        {
            StartCoroutine(CleanupAfterPlayback(src));
        }

        return src;
    }

    // Para um AudioSource 3D controlado
    public void Pause3D(AudioSource source)
    {
        if (source == null) return;
        if (source.isPlaying) source.Pause();
    }

    public void Resume3D(AudioSource source)
    {
        if (source == null) return;
        if (source.clip != null && !source.isPlaying) source.UnPause();
    }

    public void Stop3D(AudioSource source)
    {
        if (source == null) return;
        source.Stop();
        _activeSources.Remove(source);
        if (source.gameObject != null)
            Destroy(source.gameObject);
    }

    // OneShot 3D: usa PlayClipAtPoint (cria e destroi automaticamente)
    public void Play3DOneShot(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    // Para limpar referências de fontes que terminaram (não loop)
    private IEnumerator CleanupAfterPlayback(AudioSource src)
    {
        if (src == null) yield break;
        // espera o fim do clip (caso seja interrompido/alterado, checa se ainda existe)
        yield return new WaitForSecondsRealtime(src.clip != null ? src.clip.length : 0f);
        if (src == null) yield break;
        _activeSources.Remove(src);
        if (src.gameObject != null)
            Destroy(src.gameObject);
    }

    // Stop e limpa todas as fontes 3D ativas
    public void StopAll3D()
    {
        foreach (var src in new List<AudioSource>(_activeSources))
        {
            if (src == null) continue;
            src.Stop();
            if (src.gameObject != null) Destroy(src.gameObject);
        }
        _activeSources.Clear();
    }

    #endregion
}

