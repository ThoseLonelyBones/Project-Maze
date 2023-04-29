using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public class Audio_Director : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] scene_sfx, scene_music, game_sfx, game_music;

    [SerializeField]
    private AudioSource music_audio_source;

    [SerializeField]
    private AudioSource writing_sfx_audio_source, game_sfx_audio_source;

    [SerializeField]
    private AudioSource[] scene_sfx_audio_source;

    [SerializeField]
    private GameObject music_source;
    [SerializeField]
    private GameObject[] sfx_source;

    public float game_sfx_volume, scene_sfx_volume, music_volume;

    private string resourcePath;

    private void Awake()
    {
       music_audio_source = music_source.GetComponent<AudioSource>();
       scene_sfx_audio_source[0] = sfx_source[0].GetComponent<AudioSource>();
       scene_sfx_audio_source[1] = sfx_source[1].GetComponent<AudioSource>();
       scene_sfx_audio_source[2] = sfx_source[2].GetComponent<AudioSource>();

       for (int x = 0; x < scene_sfx_audio_source.Length; x++)
       {
           if(scene_sfx_audio_source[x] == null)
            {
                Debug.Log("Audio Source Unreachable...");
            }
       }

       string music_filepath = Path.Combine(Application.dataPath, "Game Assets/Audio/Game/Music");
       Debug.Log("Retriving Game Music from: " + music_filepath);
       StartCoroutine(LoadGameMusic(music_filepath));

       string audio_filepath = Path.Combine(Application.dataPath, "Game Assets/Audio/Game/SFX");
       Debug.Log("Retriving Game SFX from: " + audio_filepath);
       StartCoroutine(LoadGameSFX(audio_filepath));

        resourcePath = Application.dataPath + "/Resources/";
    }
    // Start is called before the first frame update
    void Start()
    {

        music_volume = PlayerPrefs.GetFloat("music_volume");
        if(float.IsNaN(music_volume))
        {
            music_volume = 0.5f;
            game_sfx_volume = 0.5f;
            scene_sfx_volume = 0.5f;
        }
        else
        {
            music_audio_source.volume = music_volume;
            game_sfx_volume = PlayerPrefs.GetFloat("game_sfx_volume");
            game_sfx_audio_source.volume = game_sfx_volume;
            scene_sfx_volume = PlayerPrefs.GetFloat("scene_sfx_volume");
            scene_sfx_audio_source[0].volume = scene_sfx_volume;
            scene_sfx_audio_source[1].volume = scene_sfx_volume;
            scene_sfx_audio_source[2].volume = scene_sfx_volume;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(music_volume != PlayerPrefs.GetFloat("music_volume") || game_sfx_volume != PlayerPrefs.GetFloat("game_sfx_volume") ||  scene_sfx_volume != PlayerPrefs.GetFloat("scene_sfx_volume"))
        {
            music_volume = PlayerPrefs.GetFloat("music_volume");
            music_audio_source.volume = music_volume;
            game_sfx_volume = PlayerPrefs.GetFloat("game_sfx_volume");
            game_sfx_audio_source.volume = game_sfx_volume;
            scene_sfx_volume = PlayerPrefs.GetFloat("scene_sfx_volume");
            scene_sfx_audio_source[0].volume = scene_sfx_volume;
            scene_sfx_audio_source[1].volume = scene_sfx_volume;
            scene_sfx_audio_source[2].volume = scene_sfx_volume;
        }
    }

    /*
     * Play Music, SFX or Silence them both. Standard Audio Director Functions
     * 
     */
    public void PlayMusic(int music_index, bool scene = false)
    {
        if (music_index == 0)
        {
            StopMusic();
        }
        else
        {

            Debug.Log("Playing Music clip number: " + music_index);
            music_index--;
            if (music_audio_source.isPlaying)
            {
                StartCoroutine(MusicChange(music_index, 3f, false));
            }
            else
            {
                if (scene)
                {
                    music_audio_source.clip = scene_music[music_index];
                    StartCoroutine(FadeIn(3f, music_audio_source, 0));
                }
                else
                {
                    music_audio_source.clip = game_music[music_index];
                    StartCoroutine(FadeIn(3f, music_audio_source, 0));
                }

            }
        }
       

    }
    public void PlaySceneSFX(int sfx_index)
    {
        if (sfx_index == 0)
        {
            StopSceneSFX();
        }
        else
        {
            Debug.Log("Playing Scene Sound Effect number: " + sfx_index);
            for (int x = 0; x < sfx_source.Length; x++)
            {
                if (!scene_sfx_audio_source[x].isPlaying)
                {
                    Debug.Log("Playing Sound!");
                    scene_sfx_audio_source[x].clip = scene_sfx[(sfx_index)];
                    scene_sfx_audio_source[x].Play();
                    break;
                }
            }

        }

    }

    public void PlayGameSFX(int sfx_index)
    {

        Debug.Log("Playing Game Sound Effect number: " + sfx_index);
        if(sfx_index == 1)
        {
            writing_sfx_audio_source.clip = game_sfx[(sfx_index - 1)];
            writing_sfx_audio_source.volume = game_sfx_volume;

            writing_sfx_audio_source.Play();
        }
        else
        {
            game_sfx_audio_source.clip = game_sfx[(sfx_index - 1)];
            game_sfx_audio_source.volume = game_sfx_volume;

            game_sfx_audio_source.Play();
        }
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOut(3f, music_audio_source, 0));
    }

    public void StopSceneSFX()
    {
        for (int x = 0; x < sfx_source.Length; x++)
        {
            if (scene_sfx_audio_source[x].isPlaying)
            {
                StartCoroutine(FadeOut(0.2f, scene_sfx_audio_source[x], 2));
            }
        }
    }

    public void StopGameSFX()
    {
        StartCoroutine(FadeOut(0.2f, writing_sfx_audio_source, 1));
        StartCoroutine(FadeOut(0.2f, game_sfx_audio_source, 1));
    }

    public void Silence()
    {
        StopMusic();
        StopSceneSFX();
        StopGameSFX();
    }

    /*
     * Load the current scene's SFX and Music from GameFiles. Having every single instance of a SFX or music repeated through the code is INSANE. I'd rather have a string array of names to load when a current scene is started, then subsequently
     * use the loaded audio elements to play in the scene using flags as normal. Much more memory efficent!
     */
    public void LoadSceneAudio(string[] sfx, string[] music = null)
    {
        Debug.Log("Loading Scene Audio!");
        if(music != null)
        {
            string music_filepath = "Audio/Scene/Music";
            StartCoroutine(LoadSceneMusic(music_filepath, music));
        }

        string audio_filepath = "Audio/Scene/SFX";
        Debug.Log("Audio_filepath = " + audio_filepath);
        StartCoroutine(LoadSceneSFX(audio_filepath, sfx));
    }

    IEnumerator LoadSceneMusic(string filepath, string[] filenames)
    {
        scene_music = new AudioClip[filenames.Length];

        for(int x = 0; x < scene_music.Length; x++)
        {
            string audioclip_path = Path.Combine(filepath, filenames[x]);
            audioclip_path += ".mp3";
            audioclip_path = audioclip_path.Replace("\\", "/");
            var audio = Resources.Load<AudioClip>(audioclip_path);
            scene_music[x] = audio;
        }
        yield return null;
    }

    IEnumerator LoadSceneSFX(string filepath, string[] filenames)
    {
        Debug.Log("Loading Game SFX!");

        scene_sfx = new AudioClip[filenames.Length];

        for (int x = 1; x < scene_sfx.Length; x++)
        {
            string audioclip_path = filepath + "/" + filenames[x];
            audioclip_path = audioclip_path.Replace("\\", "/");
            string file_resourcePath = Path.Combine(resourcePath, audioclip_path);
            file_resourcePath = file_resourcePath.Replace("\\", "/");
            Debug.Log("Audioclip " + x + "'s path: " + resourcePath);
            if (File.Exists(file_resourcePath + ".mp3"))
            {
                Debug.Log("Loading Audioclip now!");
                AudioClip audioclip = Resources.Load<AudioClip>(audioclip_path);
                scene_sfx[x] = audioclip;
            }
            else
            {
                Debug.Log("Audioclip does not exist!");
            }

        }
        yield return null;
    }


    // The game's music and SFX. These stay consistent in the game, and I find no reason to load them in every single time I load a new scene. These are loaded once on the Game Start from the Main Menu and then are always available.
    // As they are associated with a permanent gameobject (AudioDirector) they don't cease to exist once the main gameplay loop is called into action.
    IEnumerator LoadGameMusic(string filepath)
    {
        string[] mp3_files = Directory.GetFiles(filepath, "*.mp3");
        game_music = new AudioClip[mp3_files.Length];

        for(int x = 0; x < game_music.Length; x++)
        {
            string s = mp3_files[x];
            game_music[x] = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets" + s.Substring(Application.dataPath.Length));
        }

        Debug.Log("All Game Music is loaded!");

        yield return null;
    }

    IEnumerator LoadGameSFX(string filepath)
    {
        
        string[] mp3_files = Directory.GetFiles(filepath, "*.mp3");
        game_sfx = new AudioClip[mp3_files.Length];
        for (int x = 0; x < game_sfx.Length; x++)
        {
            string s = mp3_files[x];
            game_sfx[x] = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets" + s.Substring(Application.dataPath.Length));
        }

        Debug.Log("All Game SFX are loaded!");

        yield return null;
    }


    /*
     * Music Fadein and FadeOut, used when changing tracks of the game.
     * Easy as that, really. They are just given a specific fade time and then count up using deltatime until that time has passed, fairly standard timed-action Coroutine.
     * 
     */
    IEnumerator FadeIn(float fade_time, AudioSource audiosource, int volume_source = 3)
    {
        float maxvolume;
        float volume_reset;

        switch(volume_source)
        {
            case 0:
                maxvolume = music_volume;
                volume_reset = music_volume;
                break;
            case 1:
                maxvolume = game_sfx_volume;
                volume_reset = game_sfx_volume;
                break;
            case 2:
                maxvolume = scene_sfx_volume;
                volume_reset = scene_sfx_volume;
                break;
            default:
                maxvolume = 1f;
                volume_reset = 1f;
                break;
        }

        float time = 0f;
        float volume = 0f;
        audiosource.volume = volume;
        audiosource.Play();

        while (time < fade_time)
        {
            time += Time.deltaTime;
            audiosource.volume = Mathf.Lerp(volume, maxvolume, time / fade_time);
            yield return null;
        }

        audiosource.volume = volume_reset;
    }

    IEnumerator FadeOut(float fade_time, AudioSource audiosource, int volume_source = 3)
    {

        float volume_reset;

        switch (volume_source)
        {
            case 0:
                audiosource.volume = music_volume;
                volume_reset = music_volume;
                break;
            case 1:
                audiosource.volume = game_sfx_volume;
                volume_reset = game_sfx_volume;
                break;
            case 2:
                audiosource.volume = scene_sfx_volume;
                volume_reset = scene_sfx_volume;
                break;
            default:
                audiosource.volume = 1f;
                volume_reset = 1f;
                break;
        }

        float time = 0f;
        float volume = audiosource.volume;

        while (time < fade_time)
        {
            time += Time.deltaTime;
            audiosource.volume = Mathf.Lerp(volume, 0f, time / fade_time);
            yield return null;
        }

        audiosource.volume = volume_reset;
        audiosource.Stop();
    }

    IEnumerator MusicChange(int music_index, float fade_time, bool scene = false)
    {
        StartCoroutine(FadeOut(fade_time, music_audio_source, 0));
        yield return new WaitForSeconds(fade_time);
        if(scene)
        {
            music_audio_source.clip = scene_music[music_index];
        }
        else
        {
            music_audio_source.clip = game_music[music_index];
        }
        
        StartCoroutine(FadeIn(fade_time, music_audio_source, 0));
    }

}
