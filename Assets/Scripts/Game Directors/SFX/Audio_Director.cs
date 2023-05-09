using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

// Audio Director works with the music and audio of the game! It uses the flag and scriptable object system to find and load the correct sound files. This is a doozy, so there may be a bit of comments
public class Audio_Director : MonoBehaviour
{   
   /* 
    *  These are the audioclip arrays that contain each and every sound and music file in the game... well, not quite. Music is loaded in from the start, because it is used as part of the gameplay. Loading back in the music for each scene, where it can potentially be used is silly.
    *   Here are also the array for the game's sound effects and the scene's sound effects. There is a direct distinction between the two:
    *   Game Soundeffects are loaded in from the start. The only Game Soundeffect in this version of the game is the typing soundeffect.
    *   Scene Soundeffects are loaded when a scene is loaded. They are taken from their asset folder using an array of strings which corrseponds to the file names. Then, they are ordered as they are presented in that array, subsequently they can be called back
    *   into the game by using the same index they utilize in that array, using the 's' flag during text. s00, and thus Array position 0 is reserved for empty sounds. This is used to clean the sound effects from the game when needed (such as Ammon's voice when he's no longer speaking).
    *
    */
    [SerializeField]
    private AudioClip[] scene_sfx, scene_music, game_sfx, game_music;

    /* 
     * To play audioclips, one needs AudioSources. The game has a few audio sources available... music_audio_source is for music, writing is exclusively from the writing sound and game_sfx_audio could be used for further game audio sound (if they were present) 
     * Scene_sfx_audio_source is an array of three audio sources, with more that can be added if required. This means that usually only upwards of three sounds can play simultaneously
     * Audiosources are handled by the various Play() functions.
     */
    [SerializeField]
    public AudioSource music_audio_source;
    [SerializeField]
    private AudioSource writing_sfx_audio_source, game_sfx_audio_source;
    [SerializeField]
    private AudioSource[] scene_sfx_audio_source;

    // Used as game objects to initialize the various audiosources... why? Because there may be a problem inserting them from the Unity Editor and then transfering scene. Objects are tied to the scene they are in, and given that the audio director, or better
    // The audio director gameobject that contains this script and the audiosources is not bound by scenes, it's just a safer way to prevent a possiblity that the new scene may have trouble loading the components of the audio_director object.
    [SerializeField]
    private GameObject music_source;
    [SerializeField]
    private GameObject[] sfx_source;

    // The various volumes of the game. These read the various PlayerPreferences and use them to determine the volume of the various audio elements of the game
    public float game_sfx_volume, scene_sfx_volume, music_volume;

    // To find the audio of the game, the game needs to find its resource path. When built into an actual game, it attempts to go through its internal Resources Folder, which in turn means that the audio file need to be there when the game is built.
    private string resourcePath;


    private void Awake()
    {
        // Massive Initialization
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
       // here it completes the resourcepath.
        resourcePath = Application.dataPath + "/Resources/";

      // Loading the Game's Music (This will be touched into detail where the function is written)
       string game_music_path = "Audio/Game/Music";
       Debug.Log("Retriving Game Music from: " + game_music_path);
       StartCoroutine(LoadGameMusic(game_music_path));
      
      // Loading the Game's SFX (This will be touched into detail where the function is written)
       string game_sfx_path = "Audio/Game/SFX";
       Debug.Log("Retriving Game SFX from: " + game_sfx_path);
       StartCoroutine(LoadGameSFX(game_sfx_path));

        
    }
    // Start is called before the first frame update
    void Start()
    {
        // More initialization! Moreeeee! This sets the volume of each and every audiosource.
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
        // onUpdate, check if the volume is different. If it is, then update the volume to the new value.
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
     * It takes in input an index, the music index, and a boolean to determine whether the music is from a scene or not
     */
    public void PlayMusic(int music_index, bool scene = false)
    {   
        // if it recieves index 0, it stops playing... I told you index 0 was special!
        if (music_index == 0)
        {
            StopMusic();
        }
        else
        {
            // Music index is used with --... because the original array is actually filled to 0, and doesn't start at 1 like the saved array on the various scriptable objects
            Debug.Log("Playing Music clip number: " + music_index);
            music_index--;
            // If the music is already playing then... do a music change!
            if (music_audio_source.isPlaying)
            {
                StartCoroutine(MusicChange(music_index, 3f, false));
            }
            else
            {
                // based on the type of audio, either scene or game, pick a different audioclip array to choose from.
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
    // Just as before, this takes an index. However, this function is specifically built to play SceneSFX so it requires no boolean to confirm it
    public void PlaySceneSFX(int sfx_index)
    {
        // if index is 0, stop any sound
        if (sfx_index == 0)
        {
            StopSceneSFX();
        }
        else
        {
            // else... play through that array of sounds. Remember that this array skips the scene_sfx array skips 0.
            Debug.Log("Playing Scene Sound Effect number: " + sfx_index);
            for (int x = 0; x < sfx_source.Length; x++)
            {
                // if an audio source isn't playing, then assign it to the first available, else don't play the sound.
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

    // Just as before, this takes an index. This function is specifically built to play GameSFX so it requires no bool confirm
    public void PlayGameSFX(int sfx_index)
    {
        
        // It checks the index, and, if the index is 1 it means play the writing sound (as its the first sound loaded in the game) on the writing sfx audio audiosource, else play whatever other sound is required on the game sfx audio audiosource.
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

    // Starts a coroutine that stops the current track with a fadeout. When m00 is called, this is what happens.
    public void StopMusic()
    {
        StartCoroutine(FadeOut(3f, music_audio_source, 0));
    }

    // This stops the Scene SFX by going through each scene SFX audiosource and fading them out individually
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

    // Stop GameSFX stops both the game sfx audiosource and the writing sfx audio source
    public void StopGameSFX()
    {
        StartCoroutine(FadeOut(0.2f, writing_sfx_audio_source, 1));
        StartCoroutine(FadeOut(0.2f, game_sfx_audio_source, 1));
    }

    // Silence! Stops everything.
    public void Silence()
    {
        StopMusic();
        StopSceneSFX();
        StopGameSFX();
    }

    // This is a function I made in the attempt to not get a stupid glitch where the music doubles up if you enter the credits... guess what didn't work? This function right here. Although it still does play music in the main menu when required
    public void MainMenuMusic()
    {
        music_audio_source.Stop();
        if (music_audio_source.GetComponent<AudioClip>() == game_music[0])
        {
            //music_audio_source.Play();
            Debug.Log("Keep on Playing!");
        }
        else
        {
            PlayMusic(1);
            Debug.Log("Let's start the beat!");
        }
    }

    /*
     * Load the current scene's SFX and Music from GameFiles. Having every single instance of a SFX or music repeated through the code is INSANE. I'd rather have a string array of names to load when a current scene is started, then subsequently
     * use the loaded audio elements to play in the scene using flags as normal. Much more memory efficent!
     */

    // This function begins the two coroutines to gather the scene's audio and music. Music is by default null because no further music was added during the game and it's not required to load up the sounds... however if one wants to add music to various
    // scenes when they have the time I'll appreciate.
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
        Debug.Log("Loading Scene SFX!");

        scene_sfx = new AudioClip[filenames.Length];

        for (int x = 1; x < scene_sfx.Length; x++)
        {
            string audioclip_path = filepath + "/" + filenames[x];
            audioclip_path = audioclip_path.Replace("\\", "/");
            string file_resourcePath = Path.Combine(resourcePath, audioclip_path);
            file_resourcePath = file_resourcePath.Replace("\\", "/");
            Debug.Log("Scene Audioclip " + x + "'s path: " + resourcePath);
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
        Debug.Log("Loading Game Music!");

        // Get the resource folderpath for the music... it's a bit messy and it trickles up and down the .mp3 a few times for a few reasons:
        // Resources.Load doesn't want the resource folderpath, they want the path of the folders in the resource folder. If you attempt to use the Resource folderpath on Resource.Load<AudioClip> what will happen is that it will attempt to find
        // Resources/"Resource Folderpath"/Audio/Game/Music, which isn't correct. So it requires a special formatting. That formatting is filenames_resources. It also doesn't want the .mp3 in it... however, Directory.GetFiles copies all MP3 files.
        // Including their extension, which we need to remove from filenames_resources... BUT the extension is needed to check if the file actually exists with File.Exists, so we need to keep both filenames and filenames resources. Multiple times during
        // this function there is string formatting, that's required because of the way Windows saves Datapaths (Unity occasionally doesn't mind it, but sometimes it throws a hissyfit about it... so just in case).
        string file_resourcePath = Path.Combine(resourcePath, filepath);
        file_resourcePath = file_resourcePath.Replace("\\", "/");

        string[] filenames = Directory.GetFiles(file_resourcePath, "*.mp3");
        string[] filenames_resources = new string[filenames.Length];

        // This loads every single filename in the game music folder, then it uses those filenames...
        for(int y = 0; y < filenames_resources.Length; y++)
        {
            filenames_resources[y] = Path.Combine(filepath, Path.GetFileName(filenames[y]));
            filenames_resources[y] = filenames_resources[y].Replace(".mp3", "");
        }

        // ... down here! This is the game loading all the audio clips it can actually find in the game's Resource/Game/Music folder and then loading them in the game.
        game_music = new AudioClip[(filenames_resources.Length)];                               

        for (int x = 0; x < game_music.Length; x++)
        {
            filenames_resources[x] = filenames_resources[x].Replace("\\", "/");
            Debug.Log("MusicClip " + x + "'s complete path: " + filenames_resources[x]);

            // however this still requires the regular filename...
            if (File.Exists(filenames[x]))
            {
                Debug.Log("Loading Music Clip now!");
                AudioClip audioclip = Resources.Load<AudioClip>(filenames_resources[x]);
                game_music[x] = audioclip;
            }
            else
            {
                Debug.Log("Audioclip does not exist!");
            }

        }
        yield return null;
    }

    // This does the same thing as LoadGameMusic, as it takes all the files... Right now it only loads the writing sounds.
    IEnumerator LoadGameSFX(string filepath)
    {
        Debug.Log("Loading Game SFX!");

        string file_resourcePath = Path.Combine(resourcePath, filepath);
        file_resourcePath = file_resourcePath.Replace("\\", "/");

        string[] filenames = Directory.GetFiles(file_resourcePath, "*.mp3");
        string[] filenames_resources = new string[filenames.Length];

        for (int y = 0; y < filenames_resources.Length; y++)
        {
            filenames_resources[y] = Path.Combine(filepath, Path.GetFileName(filenames[y]));
            filenames_resources[y] = filenames_resources[y].Replace(".mp3", "");
        }

        game_sfx = new AudioClip[(filenames_resources.Length)];

        for (int x = 0; x < game_sfx.Length; x++)
        {
            filenames_resources[x] = filenames_resources[x].Replace("\\", "/");
            Debug.Log(filenames_resources[x]);

            Debug.Log("MusicClip " + x + "'s path: " + filenames_resources[x]);
            if (File.Exists(filenames[x]))
            {
                Debug.Log("Loading Music Clip now!");
                AudioClip audioclip = Resources.Load<AudioClip>(filenames_resources[x]);
                game_sfx[x] = audioclip;
            }
            else
            {
                Debug.Log("Audioclip does not exist!");
            }

        }
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
        // This takes a volume source an int that determines which type of volume that specific music or sound should be using
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

        // Again this is a transition done from volume 0 to the desired volume selected by the user using Linear Interpolation to smooth the transition.
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

    // The fadeout coroutine is pretty much identical to the previous one, instead of using the volume as the starting point it instead it's the end point of the Linear Interpolation.
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

    // This stops the current track, and then plays a new track... simple as that.
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
