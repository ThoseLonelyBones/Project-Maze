using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Audio_Director : MonoBehaviour
{

    private AudioClip[] scene_sfx, scene_music, game_sfx, game_music;

    [SerializeField]
    private AudioSource music_audio_source;
    private AudioSource[] sfx_audio_source;

    private GameObject music_source;
    private GameObject[] sfx_source;

    private void Awake()
    {
       music_audio_source = music_source.GetComponent<AudioSource>();
       sfx_audio_source[0] = sfx_source[0].GetComponent<AudioSource>();
       sfx_audio_source[1] = sfx_source[1].GetComponent<AudioSource>();
       sfx_audio_source[2] = sfx_source[2].GetComponent<AudioSource>();

       string music_filepath = Path.Combine(Application.dataPath, "GameAssets/Audio/Game/Music");
       StartCoroutine(LoadGameMusic(music_filepath));

       string audio_filepath = Path.Combine(Application.dataPath, "GameAssets/Audio/Game/Audio");
       StartCoroutine(LoadGameSFX(audio_filepath));

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * Play Music, SFX or Silence them both. Standard Audio Director Functions
     * 
     */
    public void PlayMusic(int music_index, bool scene = false)
    {
        Debug.Log("Playing Music clip number: " + music_index);
        if(music_audio_source.isPlaying)
        {
            StartCoroutine(MusicChange(music_index, 3f, false));
        }
        else
        {
            if(scene)
            {
                music_audio_source.clip = scene_music[music_index];
                StartCoroutine(MusicFadeIn(3f));
            }
            else
            {
                music_audio_source.clip = game_music[music_index];
                StartCoroutine(MusicFadeIn(3f));
            }

        }

    }
    public void PlaySFX(int sfx_index, bool scene = false)
    {
        Debug.Log("Playing Sound Effect number: " + sfx_index);
        for(int x = 1; x < sfx_source.Length; x++)
        {
            if(!sfx_audio_source[x].isPlaying)
            {
                if(scene)
                {
                    sfx_audio_source[x].clip = scene_sfx[sfx_index];
                    break;
                }
                else
                {
                    sfx_audio_source[x].clip = game_sfx[sfx_index];
                    break;
                }
                
            }
        }
    }
    public void Silence()
    {
        StartCoroutine(MusicFadeOut(3f));

        for (int x = 0; x < sfx_source.Length; x++)
        {
            if(!sfx_audio_source[x].isPlaying)
            {
                sfx_audio_source[x].Stop();
            }
        }

        
    }

    /*
     * Load the current scene's SFX and Music from GameFiles. Having every single instance of a SFX or music repeated through the code is INSANE. I'd rather have a string array of names to load when a current scene is started, then subsequently
     * use the loaded audio elements to play in the scene using flags as normal. Much more memory efficent!
     */
    public void LoadSceneAudio(string[] sfx, string[] music = null)
    {
        if(music != null)
        {
            string music_filepath = Path.Combine(Application.dataPath, "GameAssets/Audio/Scene/Music");
            StartCoroutine(LoadSceneMusic(music_filepath, music));
        }

        string audio_filepath = Path.Combine(Application.dataPath, "GameAssets/Audio/Scene/Audio");
        StartCoroutine(LoadSceneSFX(audio_filepath, sfx));


    }
    IEnumerator LoadSceneMusic(string filepath, string[] filenames)
    {
        scene_music = new AudioClip[filenames.Length];

        for(int x = 0; x < scene_music.Length; x++)
        {
            string audioclip_path = Path.Combine(filepath, filenames[x]);
            var audio = Resources.Load<AudioClip>(audioclip_path);
            scene_music[x] = audio;
        }
        yield return null;
    }

    IEnumerator LoadSceneSFX(string filepath, string[] filenames)
    {
        scene_sfx = new AudioClip[filenames.Length];

        for (int x = 0; x < scene_sfx.Length; x++)
        {
            string audioclip_path = Path.Combine(filepath, filenames[x]);
            var audio = Resources.Load<AudioClip>(audioclip_path);
            scene_sfx[x] = audio;
        }
        yield return null;
    }

    // The game's music and SFX. These stay consistent in the game, and I find no reason to load them in every single time I load a new scene. These are loaded once on Game Start and then are always available.
    IEnumerator LoadGameMusic(string filepath)
    {
        game_music = Resources.LoadAll<AudioClip>(filepath);
        yield return null;
    }

    IEnumerator LoadGameSFX(string filepath)
    {
        game_sfx = Resources.LoadAll<AudioClip>(filepath);
        yield return null;
    }


    /*
     * Music Fadein and FadeOut, used when changing tracks of the game.
     * Easy as that, really. They are just given a specific fade time and then count up using deltatime until that time has passed, fairly standard timed-action Coroutine.
     * 
     */
    IEnumerator MusicFadeIn(float fade_time)
    {
        float time = 0f;
        float volume = 0f;
        music_audio_source.volume = volume;
        music_audio_source.Play();

        while (time < fade_time)
        {
            time += Time.deltaTime;
            music_audio_source.volume = Mathf.Lerp(volume, 1f, time / fade_time);
            yield return null;
        }

        music_audio_source.volume = 1f;
    }

    IEnumerator MusicFadeOut(float fade_time)
    {

        float time = 0f;
        float volume = music_audio_source.volume;

        while (time < fade_time)
        {
            time += Time.deltaTime;
            music_audio_source.volume = Mathf.Lerp(volume, 0f, time / fade_time);
            yield return null;
        }

        music_audio_source.volume = 1f;

        music_audio_source.Stop();
        music_audio_source.volume = volume;
    }

    IEnumerator MusicChange(int music_index, float fade_time, bool scene = false)
    {
        StartCoroutine(MusicFadeOut(fade_time));
        yield return new WaitForSeconds(fade_time);
        if(scene)
        {
            music_audio_source.clip = scene_music[music_index];
        }
        else
        {
            music_audio_source.clip = game_music[music_index];
        }
        
        StartCoroutine(MusicFadeIn(fade_time));
    }

}
